using Serilog;
using System.IO;
using Windows.Storage;

namespace BelledonneCommunications.Linphone
{
    public class Logger
    {
        public static void ConfigureLogger()
        {
            string logFilePath = Path.Combine(Path.Combine(ApplicationData.Current.LocalFolder.Path, "logs\\log-.txt"));

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Serilog started!");
        }
    }
}
