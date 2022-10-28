
using System.Diagnostics;
using KiteHelper;
using Microsoft.AspNetCore.Hosting;
using Serilog;

namespace KiteHelper
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

            IHost host = CreateHostBuilder(args).Build();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(configure =>
                {
                    configure.UseStartup<Startup>();

                })
                .UseSerilog();
        }
    }
}
