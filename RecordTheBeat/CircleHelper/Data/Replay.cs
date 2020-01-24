using CircleHelper.Enums;

namespace CircleHelper.Data
{
    public class Replay
    {
        public Mode Gamemode { get; set; }
        public int Version { get; set; }
        public string BeatmapMD5 { get; set; }
        public string Player { get; set; }
        public string ReplayMD5 { get; set; }
        public short Num300s { get; set; }
        public short Num100s { get; set; }
        public short Num50s { get; set; }
        public short NumGekis { get; set; }
        public short NumKatus { get; set; }
        public short NumMisses { get; set; }
        public int TotalScore { get; set; }
        public short MaxCombo { get; set; }
        public bool PerfectCombo { get; set; }
        public Mods ModsUsed { get; set; }
        
    }
}