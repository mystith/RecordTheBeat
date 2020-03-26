using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using CircleHelper.Data.HitObjects;
using CircleHelper.Enums;

namespace CircleHelper.Data
{
    //Holds beatmap information contained within the .osu file
    public class Beatmap
    {
        #region General
        public string AudioFilename { get; set; }
        public int AudioLeadIn { get; set; }
        public string AudioHash { get; set; }
        public int PreviewTime { get; set; }
        public CountdownSpeed Countdown { get; set; }
        public string SampleSet { get; set; }
        public double StackLeniency { get; set; }
        public GameMode Mode { get; set; }
        public bool LetterboxInBreaks { get; set; }
        public bool WidescreenStoryboard { get; set; }
        public bool StoryFireInFront { get; set; }
        public bool SpecialStyle { get; set; }
        public bool EpilepsyWarning { get; set; }
        public bool UseSkinSprites { get; set; }
        public bool AlwaysShowPlayfield { get; set; }
        public string OverlayPosition { get; set; }
        public string SkinPreference { get; set; }
        public int CountdownOffset { get; set; }
        public bool SamplesMatchPlaybackRate { get; set; }
        #endregion
        #region Editing
        public int[] Bookmarks { get; set; }
        public double DistanceSpacing { get; set; }
        public double BeatDivisor { get; set; }
        public int GridSize { get; set; }
        public double TimelineZoom { get; set; }
        #endregion
        #region Metadata
        public string Title { get; set; }
        public string TitleUnicode { get; set; }
        public string Artist { get; set; }
        public string ArtistUnicode { get; set; }
        public string Creator { get; set; }
        public string Version { get; set; }
        public string Source { get; set; }
        public string[] Tags { get; set; }
        public int BeatmapID { get; set; }
        public int BeatmapSetID { get; set; }
        #endregion
        #region Difficulty
        public double HPDrainRate { get; set; }
        public double CircleSize { get; set; }
        public double OverallDifficulty { get; set; }
        public double ApproachRate { get; set; }
        public double SliderMultiplier { get; set; }
        public double SliderTickRate { get; set; }
        #endregion
        #region Events
        public string Background { get; set; }
        public IEnumerable<Break> Breaks { get; set; }
        #endregion
        #region Colors
        public IEnumerable<Color> ComboColors { get; set; }
        public Color SliderTrackOverride { get; set; }
        public Color SliderBorder { get; set; }
        #endregion
        #region Game
        public IEnumerable<TimingPoint> TimingPoints { get; set; }
        public IEnumerable<HitCircle> HitCircles { get; set; }
        public IEnumerable<Slider> Sliders { get; set; }
        public IEnumerable<Spinner> Spinners { get; set; }

        public int MaxCombo { get; set; }
        public int FirstObject { get; set; }

        public double TotalPP { get; set; }
        #endregion
    }
}