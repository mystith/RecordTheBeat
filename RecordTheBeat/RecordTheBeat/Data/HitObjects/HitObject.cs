using RecordTheBeat.Enums.HitObjects;

namespace RecordTheBeat.Data.HitObjects
{
    public class HitObject
    {
        int X { get; set; }
        int Y { get; set; }

        int Time { get; set; }

        HitObjectType ObjectType { get; set; }

        HitSoundType Hitsound { get; set; }

        HitsoundExtras Extras { get; set; }
    }
}
