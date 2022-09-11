using Discord.WebSocket;

namespace RespoBot.Services.PeriodicDiscordServices
{
    public class RaceService : PeriodicDiscordService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EntryPoint> _logger;

        private readonly DiscordSocketClient _discordClient;

        public RaceService(IConfiguration configuration, ILogger<EntryPoint> logger, DiscordSocketClient discordClient)
            : base(configuration, logger, discordClient, nameof(RaceService))
        {
            _configuration = configuration;
            _logger = logger;

            _discordClient = discordClient;
        }

        protected override void Run()
        {

        }
    }
}
