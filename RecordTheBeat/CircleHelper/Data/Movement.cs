using CircleHelper.Enums;

namespace CircleHelper.Data
{
    public struct Movement
    {
        public long TimeSince { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public KeyInfo KeysPressed { get; set; }
    }
}
