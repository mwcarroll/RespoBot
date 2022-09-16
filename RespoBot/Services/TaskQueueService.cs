using System.Collections.Concurrent;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
using System.Threading;
using RespoBot.Helpers;

namespace RespoBot.Services
{
    public class TaskQueueService
    {
        private readonly ILogger<TaskQueueService> _logger;
        private readonly iRApi.IDataClient _iRacingDataClient;

        private PriorityQueue<TaskQueueItem, TaskQueuePriority> _taskQueue = new(new TaskQueuePriorityComparer());
        private ConcurrentBag<TaskInFlightItem> _tasksInFlight = new();
        private Dictionary<Guid, DateTimeOffset> _taskGroupStarted = new();

        private DateTimeOffset _rateLimitReset = DateTimeOffset.UtcNow.AddMinutes(1);
        private int _totalRateLimit = 240;
        private int _rateLimitRemaining = 240;

        private readonly SemaphoreSlim _semaphoreSlim = new(1);

        public TaskQueueService(ILogger<TaskQueueService> logger, iRApi.IDataClient iRacingDataClient)
        {
            _logger = logger;
            _iRacingDataClient = iRacingDataClient;
        }

        public void Run()
        {
            Task.Run(() => { RunQueue(); });
            PeriodicTask.Run(() => { RunRateInfoUpdate(); }, TimeSpan.FromSeconds(15));
        }

        public void QueueRequest(Func<Task> task, Guid group, TaskQueuePriority priority = TaskQueuePriority.Low)
        {
            TaskQueueItem queuedTask = new()
            {
                Id = Guid.NewGuid(),
                Group = group,
                Task = task
            };

            TaskInFlightItem taskInFlight = new()
            {
                Id = queuedTask.Id,
                Group = group
            };

            if (!_taskGroupStarted.ContainsKey(group))
                _taskGroupStarted.Add(group, DateTimeOffset.UtcNow);

            _tasksInFlight.Add(taskInFlight);
            _taskQueue.EnsureCapacity(_tasksInFlight.Count);
            _taskQueue.Enqueue(queuedTask, priority);
            
        }

        public IEnumerable<T> GetResponses<T>(Guid group){
            List<TaskInFlightItem> tasks = _tasksInFlight.Where(task => task.Group.Equals(group)).ToList();

            // wait for tasks to be fired
            while(tasks.Any(x => x.Task is null))
            {
                Task.Delay(1);
            }

            // wait for tasks to complete
            Task.WaitAll(tasks.Select(x => x.Task).ToArray());

            IEnumerable<iRApi.Common.DataResponse<T>> responses = tasks.Select(x => (x.Task as Task<iRApi.Common.DataResponse<T>>).Result);

            _semaphoreSlim.Wait();
            List<TaskInFlightItem> newListOfTasksInFlight = _tasksInFlight.ToList();
            newListOfTasksInFlight.RemoveAll(x => x.Group.Equals(group));

            _tasksInFlight = new ConcurrentBag<TaskInFlightItem>(newListOfTasksInFlight);
            _semaphoreSlim.Release();

            _taskGroupStarted.Remove(group);

            iRApi.Common.DataResponse<T> responseWithHighestRateLimitReset = responses.MaxBy(x => x.RateLimitReset);

            if (_rateLimitReset < responseWithHighestRateLimitReset.RateLimitReset)
            {
                _semaphoreSlim.Wait();

                DateTimeOffset now = DateTimeOffset.UtcNow;

                _rateLimitReset = (DateTimeOffset)responseWithHighestRateLimitReset.RateLimitReset;
                _totalRateLimit = (int)responseWithHighestRateLimitReset.TotalRateLimit;
                _rateLimitRemaining = (int)responseWithHighestRateLimitReset.RateLimitRemaining;

                foreach (Guid key in _taskGroupStarted.Keys.ToList())
                {
                    _taskGroupStarted[key] = now;
                }

                _semaphoreSlim.Release();
            }

            foreach (var response in responses)
            {
                yield return response.Data;
            }
        }

        private async void RunQueue()
        {
            if (_taskQueue.Count > 0)
            {
                while (_rateLimitRemaining < (_totalRateLimit - (_totalRateLimit * 0.875)))
                {
                    _taskQueue.TryPeek(out TaskQueueItem taskQueueItemPeek, out TaskQueuePriority priorityPeek);
                    if (priorityPeek.Equals(TaskQueuePriority.High))
                        continue;

                    _logger.LogWarning("Delaying queue by 250ms to avoid rate limiting...");
                    await Task.Delay(250);
                }

                _taskQueue.TryDequeue(out TaskQueueItem taskQueueItem, out TaskQueuePriority priority);
                if (taskQueueItem != null)
                {
                    await Task.Delay(GetPerRequestDelay(taskQueueItem.Group));

                    await _semaphoreSlim.WaitAsync().ConfigureAwait(false);

                    TaskInFlightItem taskInFlight = _tasksInFlight.First(x => x.Id.Equals(taskQueueItem.Id));                    
                    taskInFlight.Task = taskQueueItem.Task.Invoke();

                    _semaphoreSlim.Release();
                }
            }
            else
                await Task.Delay(100);

            RunQueue();
        }

        private int GetPerRequestDelay(Guid group)
        {
            int delay = 75;

            _taskGroupStarted.TryGetValue(group, out DateTimeOffset now);

            if (_tasksInFlight.Count > _rateLimitRemaining)
            {
                TimeSpan timespan = TimeSpan.FromMinutes(1);
                if ((_rateLimitReset - now) > timespan)
                    timespan = (_rateLimitReset - now);

                int calculatedDelay = (int)((timespan.TotalMilliseconds) / (_totalRateLimit * 0.8333));
                delay = Math.Max(delay, calculatedDelay);

                _logger.LogInformation($"Per request delay: {delay}ms (calculated: {calculatedDelay}ms)");
            }
            else
                _logger.LogInformation($"Per request delay: {delay}ms");

            return delay;
        }

        private async void RunRateInfoUpdate()
        {
            Guid group = Guid.NewGuid();
            QueueRequest(
                () => { return _iRacingDataClient.GetEventTypesAsync(); },
                group,
                TaskQueuePriority.High);

            List<TaskInFlightItem> tasks = _tasksInFlight.Where(task => task.Group.Equals(group)).ToList();

            // wait for tasks to be fired
            while (tasks.Any(x => x.Task is null))
            {
                await Task.Delay(1);
            }

            // wait for tasks to complete
            Task.WaitAll(tasks.Select(x => x.Task).ToArray());

            IEnumerable<iRApi.Common.DataResponse<iRApi.Constants.EventType[]>> responses = tasks.Select(x => (x.Task as Task<iRApi.Common.DataResponse<iRApi.Constants.EventType[]>>).Result);

            await _semaphoreSlim.WaitAsync().ConfigureAwait(false);
            List<TaskInFlightItem> newListOfTasksInFlight = _tasksInFlight.ToList();
            newListOfTasksInFlight.RemoveAll(x => x.Group.Equals(group));

            _tasksInFlight = new ConcurrentBag<TaskInFlightItem>(newListOfTasksInFlight);
            _semaphoreSlim.Release();

            _taskGroupStarted.Remove(group);

            iRApi.Common.DataResponse<iRApi.Constants.EventType[]> responseWithHighestRateLimitReset = responses.MaxBy(x => x.RateLimitReset);

            if (_rateLimitReset < responseWithHighestRateLimitReset.RateLimitReset)
            {
                await _semaphoreSlim.WaitAsync().ConfigureAwait(false);

                _rateLimitReset = (DateTimeOffset)responseWithHighestRateLimitReset.RateLimitReset;
                _totalRateLimit = (int)responseWithHighestRateLimitReset.TotalRateLimit;
                _rateLimitRemaining = (int)responseWithHighestRateLimitReset.RateLimitRemaining;

                DateTimeOffset now = DateTimeOffset.UtcNow;

                foreach (Guid key in _taskGroupStarted.Keys.ToList())
                {
                    _taskGroupStarted[key] = now;
                }

                _semaphoreSlim.Release();
            }
        }
    }

    public class TaskQueueItem
    {
        public Guid Id { get; set; }
        public Guid Group { get; set; }
        public Func<Task> Task { get; set; }
    }

    public class TaskInFlightItem
    {
        public Guid Id { get; set; }
        public Guid Group { get; set; }
        public Task Task { get; set; }
    }

    public enum TaskQueuePriority
    {
        Low,
        High
    }

    public class TaskQueuePriorityComparer : IComparer<TaskQueuePriority>
    {
        // highest to lowest
        public int Compare(TaskQueuePriority x, TaskQueuePriority y) => y - x;
    }
}
