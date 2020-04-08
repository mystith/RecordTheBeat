using System;

namespace CircleHelper.Enums.HitObjects
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
        HoldNote = 128
    }
}
