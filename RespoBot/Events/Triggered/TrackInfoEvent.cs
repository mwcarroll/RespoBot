using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aydsko.iRacingData.Tracks;
using RespoBot.Helpers;

namespace RespoBot.Events.Triggered
{
    internal class TrackInfoEvent
    {
        private readonly ILogger<TrackInfoEvent> _logger;
        private readonly IConfiguration _configuration;
        private readonly RateLimitedIRacingApiClient _iRacing;

        public TrackInfoEvent(ILogger<TrackInfoEvent> logger, IConfiguration configuration, RateLimitedIRacingApiClient iRacing)
        {
            _logger = logger;
            _configuration = configuration;
            _iRacing = iRacing;
        }

        public void Run()
        {
            Task.Run(TrackIndexer);
        }

        private async void TrackIndexer()
        {
            List<Task<iRApi.Common.DataResponse<Track[]>>> trackListTasks = [];
            
            if (!_iRacing.DataClient.IsLoggedIn)
            {
                await _iRacing.DataClient.LoginExternalAsync();
            }
            
            trackListTasks.Add(
                _iRacing.ExecuteAsync<Track[]>(
                    () => _iRacing.DataClient.GetTracksAsync()
                )
            );
            
            await Task.WhenAll(trackListTasks.ToArray<Task>().Union(trackListTasks.ToArray<Task>()));
            
            List<Track[]> tracksList = trackListTasks.Select(x => x.Result.Data).ToList();

            List<Track> tracks = tracksList[0].ToList().OrderBy(x => x.TrackName).ToList();
            
            foreach(Track track in tracks)
            {
                Console.WriteLine($"{track.TrackName}{(!string.IsNullOrEmpty(track.ConfigName) ? $" - {track.ConfigName}" : "")}: {track.PitRoadSpeedLimit} mph");

                if (track.PitRoadSpeedLimit.Equals(0))
                {
                    
                }
            }
            Console.WriteLine("");
        }
    }
}
