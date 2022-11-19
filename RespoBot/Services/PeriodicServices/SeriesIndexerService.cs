using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RespoBot.Services.PeriodicServices
{
    public class SeriesIndexerService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TaskQueueService> _logger;
        private readonly TaskQueueService _taskQueueService;
        private readonly IDbContext _db;
        private readonly iRApi.IDataClient _iRacingDataClient;

        public SeriesIndexerService(IConfiguration configuration, ILogger<TaskQueueService> logger, TaskQueueService taskQueueService, IDbContext db, iRApi.IDataClient iRacingDataClient)
        {
            _configuration = configuration;
            _logger = logger;
            _taskQueueService = taskQueueService;
            _db = db;
            _iRacingDataClient = iRacingDataClient;
        }

        public void Run()
        {
            Task.Run(() => { RunSeriesIndexer(); });

            //PeriodicTask.Run(() => { RunSubSessionIndexer(); }, TimeSpan.FromMinutes(1));
        }

        private async void RunSeriesIndexer()
        {
            Guid seriesRequestGroup = Guid.NewGuid();
            Guid seriesAssetRequestGroup = Guid.NewGuid();

            _taskQueueService.QueueRequest(
                () =>
                {
                    return _iRacingDataClient.GetSeasonsAsync(includeSeries: true);
                },
                seriesRequestGroup
            );

            iRApi.Series.SeasonSeries[] seasonSeries = _taskQueueService.GetResponses<iRApi.Series.SeasonSeries[]>(seriesRequestGroup).FirstOrDefault();

            foreach(iRApi.Series.SeasonSeries series in seasonSeries)
            {
                foreach(iRApi.Series.Schedule schedule in series.Schedules)
                {

                }
            }


            //_iRacingDataClient.getser

        }
    }
}
