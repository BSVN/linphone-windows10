using Serilog;
using System;
using System.Threading.Tasks;
using Windows.Media.Capture;

namespace BelledonneCommunications.Linphone
{
    public static class Utility
    {
        public static async Task<bool> IsMicrophoneAvailable()
        {
            try
            {
                MediaCapture mediaCapture = new MediaCapture();
                var settings = new MediaCaptureInitializationSettings();
                settings.StreamingCaptureMode = StreamingCaptureMode.Audio;
                await mediaCapture.InitializeAsync(settings);
            }
            catch (Exception ex)
            {
                var _logger = Log.Logger.ForContext("SourceContext", nameof(Utility));

                _logger.Warning(ex, "Can't access to the microphone.");

                return false;
            }

            return true;
        }
    }
}
