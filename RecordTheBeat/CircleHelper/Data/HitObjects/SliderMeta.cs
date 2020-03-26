using CircleHelper.Data.Basic;
using CircleHelper.Enums.HitObjects;
using System.Collections.Generic;

namespace CircleHelper.Data.HitObjects
{
    public class Slider : HitObject
    {
        public SliderType Type { get; set; }
        public IEnumerable<Vector2> CurvePoints { get; set; }
        public int Repeat { get; set; }
        public double PixelLength { get; set; }
        public IEnumerable<HitSoundType> EdgeHitsounds { get; set; }
        public IEnumerable<Vector2> EdgeAdditions { get; set; }
    }
}
