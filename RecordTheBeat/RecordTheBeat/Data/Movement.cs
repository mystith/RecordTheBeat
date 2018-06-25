using RecordTheBeat.Enums;

namespace RecordTheBeat.Data
{
    public class Movement
    {
        public long TimeSince { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public KeyInfo KeysPressed { get; set; }
    }
}
