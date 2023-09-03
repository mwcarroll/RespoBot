using System.Threading.Tasks;
using Discord.Interactions;

namespace RespoBot.Commands
{
    public class ExampleCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("ping", "Pings the bot and returns its latency.")]
        public async Task PingPong()
            => await RespondAsync(text: $":ping_pong: It took me {Context.Client.Latency}ms to respond to you!", ephemeral: true);
    }
}
