using RecordTheBeat.Data.Basic;
using RecordTheBeat.Enums.HitObjects;
using System.Collections.Generic;

namespace RecordTheBeat.Data.HitObjects
{
    public class Slider : HitObject
    {
        public SliderType Type { get; set; }
        public List<Vector2D> Points { get; set; }
        public IEnumerable<Vector2> CurvePoints { get; set; }
        public int Repeat { get; set; }
        public double PixelLength { get; set; }
        public List<HitSoundType> EdgeHitsounds { get; set; }
        public List<Vector2> EdgeAdditions { get; set; }
    }
}
