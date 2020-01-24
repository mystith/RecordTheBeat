using Serilog.Events;

namespace RecordTheBeat.Configuration
{
    public class Config
    {
        public VideoSettings VideoConfig { get; private set; }
        public GameSettings GameConfig { get; private set; }
        
        public string TitleScreen { get; private set; }
        public LogEventLevel LogDepth { get; private set; }

        public Config()
        {
            VideoConfig = new VideoSettings();
            GameConfig = new GameSettings();
            LogDepth = LogEventLevel.Information;
            TitleScreen = "";
        }
    }
}