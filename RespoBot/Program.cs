using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Logging;
using RespoBot.Events.Args;
using RespoBot.Events.Triggered;

namespace RespoBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            RespoBot respoBot = new  RespoBot();
            
            DiscordSocketClient discordClient = RespoBot.ServiceProvider.GetRequiredService<DiscordSocketClient>();
            
            NewTrackedMemberEvent newTrackedMemberEvent = RespoBot.ServiceProvider.GetRequiredService<NewTrackedMemberEvent>();
            
            // newTrackedMemberEvent.OnNewTrackedMember(new NewTrackedMemberEventArgs(123456, 12345678901234567890));
            
            //// Grab the Scheduler instance from the Factory
            //StdSchedulerFactory factory = new StdSchedulerFactory();
            //IScheduler scheduler = await factory.GetScheduler();

            //// and start it off
            //await scheduler.Start();

            //// define the job and tie it to our HelloJob class
            //IJobDetail job = JobBuilder.Create<HelloJob>()
            //    .WithIdentity("job1", "group1")
            //    .Build();

            //// Trigger the job to run now, and then repeat every 10 seconds
            //ITrigger trigger = TriggerBuilder.Create()
            //    .WithIdentity("trigger1", "group1")
            //    .StartNow()
            //    .WithSimpleSchedule(x => x
            //        .WithIntervalInSeconds(10)
            //        .RepeatForever())
            //    .Build();

            ////var trigger1 = TriggerBuilder.Create().WithSchedule(new ScheduleBuilder().)

            //// Tell Quartz to schedule the job using our trigger
            //await scheduler.ScheduleJob(job, trigger);

            //// some sleep to show what's happening
            //await Task.Delay(TimeSpan.FromSeconds(60));

            //// and last shut down the scheduler when you are ready to close your program
            //await scheduler.Shutdown();

            await Task.Delay(Timeout.Infinite);
        }
    }
}