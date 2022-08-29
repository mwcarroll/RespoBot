using Discord.Interactions;
using System.Threading.Tasks;

namespace RespoBot.Commands
{
    public class ExampleCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping", "ping/pong")]
        public async Task Ping()
        {
            await RespondAsync($"Pong");
        }
    }
}
