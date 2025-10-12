using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;

namespace RespoBot.Commands.SlashCommands;

public class GetTrackedUsersCommand(IDbContext db) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("get-tracked-users", "Lists all tracked users.")]
    public async Task GetTrackedUsers()
    {
        StringBuilder output = new();

        foreach (DataContext.TrackedMember trackedMember in (await db.TrackedMembers.FindAllAsync()).OrderBy(x => x.Name))
        {
            output.Append($"{trackedMember.Name}\n");
        }
        
        await RespondAsync(text: output.ToString(), ephemeral: false);
    }
}