using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;

namespace RespoBot.Commands.SlashCommands;

public class GetTrackedUsersCommand(ILogger<IDbContext> logger, IDbContext db) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("get-tracked-users", "Lists all tracked users.")]
    public async Task GetTrackedUsers()
    {
        StringBuilder output = new();

        try
        {
            foreach (DataContext.TrackedMember trackedMember in (await db.TrackedMembers.FindAllAsync()).OrderBy(x =>
                         x.Name))
            {
                output.Append($"{trackedMember.Name}\n");
            }
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Critical, ex, "An error occurred while retrieving tracked users from the database.");
            output.Append("An error occurred while retrieving tracked users from the database.");
            
            await RespondAsync(text: output.ToString(), ephemeral: false);
            return;
        }

        await RespondAsync(text: output.ToString(), ephemeral: false);
    }
}