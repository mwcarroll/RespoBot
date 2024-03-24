using System;
using System.Threading;
using System.Threading.Tasks;

namespace RespoBot.Tasks;

public class PeriodicRespoBotTask : RespoBotTask
{
    protected int Interval { get; private set; }

    protected PeriodicRespoBotTask(int interval)
    {
        Interval = interval;
    }
    
    public new async Task Run(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(Interval), cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
                Main();
        }
    }

    public virtual Task Run()
    {
        return Run(CancellationToken.None);
    }
}