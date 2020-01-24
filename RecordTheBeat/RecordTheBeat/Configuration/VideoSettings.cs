namespace RecordTheBeat.Configuration
{
    public class VideoSettings
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public double FrameRate { get; set; }

        public string Codec { get; set; }
        public string Format { get; set; }
        
        public string ExportPath { get; set; }
        
        public string AdditionalArgs { get; set; }

        public string CommandOverride { get; set; }
        public VideoSettings()
        {
            Width = 1920;
            Height = 1080;
            FrameRate = 60;

            Codec = "libx264";
            Format = "yuv420p";

            ExportPath = "final.mp4";
            AdditionalArgs = "";

            CommandOverride = "";
        }
    }
}