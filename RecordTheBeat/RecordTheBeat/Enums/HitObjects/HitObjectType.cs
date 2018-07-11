using System;

namespace RecordTheBeat.Enums.HitObjects
{
    [Flags]
    public enum HitObjectType
    {
        Circle = 1,
        Slider = 2,
        NewCombo = 4,
        Spinner = 8,
        Bit4 = 16,
        Bit5 = 32,
        Bit6 = 64,
        HoldNode = 128
    }
}
