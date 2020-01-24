using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using CircleHelper.Data.HitObjects;
using CircleHelper.Enums;

namespace CircleHelper.Data
{
    public struct Beatmap
    {
        #region General
        public string AudioFilename { get; }
        public int AudioLeadIn { get; }
        public int PreviewTime { get; }
        public bool Countdown { get; }
        public string SampleSet { get; }
        public double StackLeniency { get; }
        public GameMode Mode { get; }
        public bool LetterboxInBreaks { get; }
        public bool WidescreenStoryboard { get; }
        public bool StoryFireInFront { get; }
        public bool SpecialStyle { get; }
        public bool EpilepsyWarning { get; }
        public bool UseSkinSprites { get; }
        #endregion
        #region Metadata
        public string Title { get; }
        public string TitleUnicode { get; }
        public string Artist { get; }
        public string ArtistUnicode { get; }
        public string Creator { get; }
        public string Version { get; }
        public string Source { get; }
        public List<string> Tags { get; }
        public int BeatmapID { get; }
        public int BeatmapSetID { get; }
        #endregion
        #region Difficulty
        public double HPDrainRate { get; }
        public double CircleSize { get; }
        public double OverallDifficulty { get; }
        public double ApproachRate { get; }
        public double SliderMultiplier { get; }
        public double SliderTickRate { get; }
        #endregion
        #region Events
        public string Background { get; }
        public List<Vector2> Breaks { get; }
        #endregion
        #region Colors
        public List<Color> ComboColors { get; }
        public Color SliderBody { get; }
        public Color SliderTrackOverride { get; }
        public Color SliderBorder { get; }
        #endregion
        #region Game
        public List<TimingPoint> TimingPoints { get; }
        public List<HitCircle> HitCircles { get; }
        public List<Slider> Sliders { get; }
        public List<Spinner> Spinners { get; }

        public int MaxCombo { get; }
        public int FirstObject { get; }

        public double TotalPP { get; }
        #endregion
    }
}