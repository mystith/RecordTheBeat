using System;
using System.Collections;
using System.Collections.Generic;
using RecordTheBeat.Data.Basic;
using RecordTheBeat.Data.HitObjects;
using RecordTheBeat.Enums;

namespace RecordTheBeat.Data
{
    public struct BeatmapMeta
    {
        public int Size { get; set; }
        
        public string ArtistName { get; set; }
        public string ArtistUnicode { get; set; }
        public string Title { get; set; }
        public string TitleUnicode { get; set; }
        public string Creator { get; set; }
        public string Difficulty { get; set; }
        public string AudioFile { get; set; }
        public string MD5 { get; set; }
        public string MapFile { get; set; }
        
        public RankedStatus Status { get; set; }
        
        public short HitCircleCount { get; set; }
        public short SliderCount { get; set; }
        public short SpinnerCount { get; set; }
        
        public long LastModification { get; set; }
        
        public Single ApproachRate { get; set; }
        public Single CircleSize { get; set; }
        public Single HPDrain { get; set; }
        public Single OverallDifficulty { get; set; }
        
        public double SliderVelocity { get; set; }
        
        public IEnumerable<(int Mod, double StarRating)> STDModSR { get; set; }
        public IEnumerable<(int Mod, double StarRating)> TaikoModSR { get; set; }
        public IEnumerable<(int Mod, double StarRating)> CTBModSR { get; set; }
        public IEnumerable<(int Mod, double StarRating)> ManiaModSR { get; set; }
        
        public int DrainTime { get; set; }
        public int TotalTime { get; set; }
        
        public int AudioPreviewTime { get; set; }
        
        public IEnumerable<TimingPoint> TimingPoints { get; set; }
        
        public int BeatmapID { get; set; }
        public int BeatmapSetID { get; set; }
        public int ThreadID { get; set; }
        
        public int STDRank { get; set; }
        public int TaikoRank { get; set; }
        public int CTBRank { get; set; }
        public int ManiaRank { get; set; }
        
        public short LocalOffset { get; set; }
        public Single StackLeniency { get; set; }
        public Mode GameplayMode { get; set; }
        public string Source { get; set; }
        public string Tags { get; set; }
        public short OnlineOffset { get; set; }
        public string TitleFont { get; set; }
        public bool Unplayed { get; set; }
        public long LastPlay { get; set; }
        public bool osz2 { get; set; }
        public string FolderName { get; set; }
        public long LastUpdate { get; set; }
        
        public bool IgnoreSound { get; set; }
        public bool IgnoreSkin { get; set; }
        public bool DisableSB { get; set; }
        public bool DisableVideo { get; set; }
        public bool VisualOverride { get; set; }
        public byte ManiaScrollSpeed { get; set; }
    }
}
