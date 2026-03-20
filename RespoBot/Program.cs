using System.Threading;
using System.Threading.Tasks;

namespace RespoBot
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            RespoBot respoBot = new();

            await Task.Delay(Timeout.Infinite);
        }
    }
}