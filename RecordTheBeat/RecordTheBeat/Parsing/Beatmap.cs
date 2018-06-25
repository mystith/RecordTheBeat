using RecordTheBeat.Enums;
using System.Collections.Generic;

namespace RecordTheBeat.Parsing
{
    public class Beatmap
    {
        #region General
        public string AudioFilename { get; }
        public int AudioLeadIn { get; }
        public int PreviewTime { get; }
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
        #endregion

        public Beatmap(string osuFile)
        {
            
        }
    }
}