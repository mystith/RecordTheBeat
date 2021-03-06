﻿using System;
using System.Collections;
using System.Collections.Generic;
using CircleHelper.Data.Basic;
using CircleHelper.Data.HitObjects;
using CircleHelper.Enums;

namespace CircleHelper.Data
{
    //Holds beatmap information contained within the .db file
    public class BeatmapMeta
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
        
        public Dictionary<Mods, double> STDModSR { get; set; }
        public Dictionary<Mods, double> TaikoModSR { get; set; }
        public Dictionary<Mods, double> CTBModSR { get; set; }
        public Dictionary<Mods, double> ManiaModSR { get; set; }
        
        public int DrainTime { get; set; }
        public int TotalTime { get; set; }
        
        public int AudioPreviewTime { get; set; }
        
        public List<TimingPoint> TimingPoints { get; set; }
        
        public int BeatmapID { get; set; }
        public int BeatmapSetID { get; set; }
        public int ThreadID { get; set; }
        
        public int STDRank { get; set; }
        public int TaikoRank { get; set; }
        public int CTBRank { get; set; }
        public int ManiaRank { get; set; }
        
        public short LocalOffset { get; set; }
        public Single StackLeniency { get; set; }
        public Mode Gamemode { get; set; }
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
