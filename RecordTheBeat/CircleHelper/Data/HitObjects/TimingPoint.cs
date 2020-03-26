using CircleHelper.Enums;

namespace CircleHelper.Data.HitObjects
{
    public class TimingPoint
    {
        public int Time { get; set; }
        public double BPM { get; set; }
        public double MillisPerBeat { get; set; }
        public int Meter { get; set; }
        public int SampleSet { get; set; }
        public int SampleIndex { get; set; }
        public int Volume { get; set; }
        public bool Inherited { get; set; }
        public TimingEffect Effects { get; set; }
    }
}
