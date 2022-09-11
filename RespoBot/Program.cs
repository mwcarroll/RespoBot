using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace RespoBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IServiceCollection services = Startup.ConfigureServices();
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            await serviceProvider.GetService<EntryPoint>()!.Run(args);
        }

        public static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}