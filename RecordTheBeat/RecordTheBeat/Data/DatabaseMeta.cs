using System;
using System.Collections.Generic;

namespace RecordTheBeat.Data
{
    public class DatabaseMeta
    {
        public int Version { get; set; }
        public int FolderCount { get; set; }
        public bool AccountUnlocked { get; set; }
        public DateTime DateUnlocked { get; set; }
        public string PlayerName { get; set; }
        public IEnumerable<BeatmapMeta> Beatmaps { get; set; }
        public int UserPermissions { get; set; }
    }
}