using CircleHelper.Data.Basic;
using CircleHelper.Enums.HitObjects;
using System.Collections.Generic;

namespace CircleHelper.Data.HitObjects
{
    public class Slider : HitObject
    {
        public SliderType Type { get; set; }
        public List<Vector2> CurvePoints { get; set; }
        public int Repeat { get; set; }
        public double PixelLength { get; set; }
        public List<HitSoundType> EdgeHitsounds { get; set; }
        public List<HitSoundSample> EdgeAdditions { get; set; }
    }
}
