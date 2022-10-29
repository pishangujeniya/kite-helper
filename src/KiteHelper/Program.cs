
using System.Diagnostics;
using KiteHelper.Helpers;
using Serilog;

namespace KiteHelper
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));

            Console.WriteLine("Downloading Kite Instruments CSV");
            // Getting KiteInstrumentsCsv in before starting the application
            KiteInstruments.KiteInstrumentsCsv = await KiteConnectSdk.KiteSdk.GetInstrumentsCsv();

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
