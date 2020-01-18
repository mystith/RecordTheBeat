using Serilog.Events;

namespace RecordTheBeat.Data
{
    public class Configuration
    {
        public VideoSettings VideoConfig { get; set; }
        public LogEventLevel LogDepth { get; set; }

        public Configuration()
        {
            VideoConfig = new VideoSettings();
            LogDepth = LogEventLevel.Information;
        }
    }
}