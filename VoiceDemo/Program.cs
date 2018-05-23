using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace VoiceDemo
{
    public static class Program
    {
        public static Task Main(string[] args) => WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build().RunAsync();
    }
}
