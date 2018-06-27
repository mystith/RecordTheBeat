using RecordTheBeat.Data.Basic;
using RecordTheBeat.Enums.HitObjects;
using System.Collections.Generic;

namespace RecordTheBeat.Data.HitObjects
{
    public class Slider
    {
        public SliderType Type { get; set; }
        public List<DoublePair> Points { get; set; }
        public int Repeat { get; set; }
        public double PixelLength { get; set; }
        public List<HitSoundType> EdgeHitsounds { get; set; }
        public List<IntPair> EdgeAdditions { get; set; }
    }
}
