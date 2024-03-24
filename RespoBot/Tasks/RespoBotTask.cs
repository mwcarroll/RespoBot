using System.Threading;
using System.Threading.Tasks;

namespace RespoBot.Tasks;

public class RespoBotTask
{
    public Task Run(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!cancellationToken.IsCancellationRequested)
                Main();
        }

        return Task.CompletedTask;
    }

    protected virtual async void Main()
    {
        
    }
}