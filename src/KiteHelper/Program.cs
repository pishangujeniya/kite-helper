
using System.Diagnostics;
using KiteHelper.Helpers;
using KiteHelper.Infrastructure;
using Serilog;

namespace KiteHelper
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

            IHost host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                DatabaseContext? context = scope.ServiceProvider.GetService<DatabaseContext>();
                if (context != null)
                {
                    await KiteInstrumentsHelper.LoadKiteInstrumentsCsv(context);
                }
            }

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
