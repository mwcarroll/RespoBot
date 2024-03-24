using System;
using System.Threading.Tasks;

namespace RespoBot.Tasks.Periodic;

public class TestPeriodicRespoBotTask : PeriodicRespoBotTask
{
    private readonly ILogger<TestPeriodicRespoBotTask> _logger;
    
    public TestPeriodicRespoBotTask(ILogger<TestPeriodicRespoBotTask> logger)
        : base(5000)
    {
        _logger = logger;
    }

    protected override void Main()
    {
        _logger.LogDebug("Test");
    }
}