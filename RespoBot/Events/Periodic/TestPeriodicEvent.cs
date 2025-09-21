namespace RespoBot.Events.Periodic;

public class TestPeriodicEvent(ILogger<TestPeriodicEvent> logger) : PeriodicEvent(5000)
{
    protected override void Main()
    {
        logger.LogDebug("Test");
    }
}