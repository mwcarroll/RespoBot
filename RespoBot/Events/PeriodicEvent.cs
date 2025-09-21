using System;
using System.Threading;
using System.Threading.Tasks;

namespace RespoBot.Events;

public class PeriodicEvent : BaseEvent
{
    protected int Interval { get; private set; }

    protected PeriodicEvent(int interval)
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