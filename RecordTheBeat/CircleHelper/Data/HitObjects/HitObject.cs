using CircleHelper.Enums.HitObjects;

namespace CircleHelper.Data.HitObjects
{
    public class HitObject
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Time { get; set; }

        public HitObjectType ObjectType { get; set; }

        public HitSoundType Hitsound { get; set; }

        public HitSoundExtras Extras { get; set; }
    }
}
