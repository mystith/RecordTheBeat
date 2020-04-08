using CircleHelper.Enums;

namespace CircleHelper.Data
{
    public class Movement
    {
        public long Time { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public KeyInfo KeysPressed { get; set; }
    }
}
