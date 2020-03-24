using CircleHelper.Enums;

namespace CircleHelper.Data
{
    public struct Movement
    {
        public long Time { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public KeyInfo KeysPressed { get; set; }
    }
}
