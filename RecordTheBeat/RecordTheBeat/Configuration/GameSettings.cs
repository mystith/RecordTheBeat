namespace RecordTheBeat.Configuration
{
    public class GameSettings
    {
        public bool ShowKeyOverlay { get; set; }
        
        public bool CursorTrail { get; set; }
        public bool SmoothTrail { get; set; }
        
        public bool ShowAccuracy { get; set; }
        public bool ShowCombo { get; set; }
        public bool ShowHP { get; set; }
        public bool ShowScore { get; set; }
        
        public string SkinName { get; set; }

        public double CursorSize { get; set; }

        public GameSettings()
        {
            ShowKeyOverlay = false;
            
            CursorTrail = true;
            SmoothTrail = true;

            ShowAccuracy = false;
            ShowCombo = false;
            ShowHP = false;
            ShowScore = false;

            SkinName = "";

            CursorSize = 1.0;
        }
    }
}