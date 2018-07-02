using RecordTheBeat.Enums.HitObjects;

namespace RecordTheBeat.Data.HitObjects
{
    public class HitObject
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Time { get; set; }

        public HitObjectType ObjectType { get; set; }

        public HitSoundType Hitsound { get; set; }

        public HitsoundExtras Extras { get; set; }
    }
}
