using RespoBot.Helpers;

namespace RespoBot.Tasks.EventHandlers
{
    internal class MemberInfoUpdatedEventHandlerService
    {
        private readonly ILogger<MemberInfoUpdatedEventHandlerService> _logger;
        private readonly IDbContext _db;
        private readonly RateLimitedIRacingApiClient _iRacing;

        public MemberInfoUpdatedEventHandlerService(ILogger<MemberInfoUpdatedEventHandlerService> logger, IDbContext db, RateLimitedIRacingApiClient iRacing)
        {
            _logger = logger;
            _db = db;
            _iRacing = iRacing;
        }

        public void Run(object sender, EventArgs.MemberInfoUpdatedEventArgs e)
        {
            _logger.LogDebug($"MemberInfoUpdated: Event handled.");
        }
    }
}
