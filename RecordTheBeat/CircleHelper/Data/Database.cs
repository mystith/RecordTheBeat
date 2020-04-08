using System;
using System.Collections.Generic;

namespace CircleHelper.Data
{
    public class Database
    {
        public int Version { get; set; }
        public int FolderCount { get; set; }
        public bool AccountUnlocked { get; set; }
        public DateTime DateUnlocked { get; set; }
        public string PlayerName { get; set; }
        public List<BeatmapMeta> Beatmaps { get; set; }
        public int UserPermissions { get; set; }
    }
}