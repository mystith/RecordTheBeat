using RecordTheBeat.Data;
using RecordTheBeat.Data.Basic;
using RecordTheBeat.Data.HitObjects;
using System;
using System.IO;
using System.Text;

namespace RecordTheBeat.Parsing
{
    public static class Parse
    {
        public static void SkipString(BinaryReader br)
        {
            int result = 0;
            int shift = 0;

            if (br.ReadByte() == 0)
            {
                return;
            }

            while (true)
            {
                byte v = br.ReadByte();

                result |= (v & 0b0111_1111) << shift;

                if ((v & 0b1000_0000) == 0)
                    break;

                shift += 7;
            }

            br.BaseStream.Position += result;
        }

        public static string ParseString(BinaryReader br)
        {
            int result = 0;
            int shift = 0;

            if (br.ReadByte() == 0)
            {
                return "";
            }

            while (true)
            {
                byte v = br.ReadByte();

                result |= (v & 0b0111_1111) << shift;

                if ((v & 0b1000_0000) == 0)
                    break;

                shift += 7;
            }

            return Encoding.UTF8.GetString(br.ReadBytes(result));
        }

        public static IntDoublePair ParseIntDouble(BinaryReader br)
        {
            br.BaseStream.Position++;
            IntDoublePair res = new IntDoublePair() { A = BitConverter.ToInt32(br.ReadBytes(4), 0) };
            br.BaseStream.Position++;
            res.B = BitConverter.ToDouble(br.ReadBytes(8), 0);
            return res;
        }

        public static short ParseShort(BinaryReader br)
        {
            return BitConverter.ToInt16(br.ReadBytes(2), 0);
        }

        public static int ParseInteger(BinaryReader br)
        {
            return BitConverter.ToInt32(br.ReadBytes(4), 0);
        }

        public static long ParseLong(BinaryReader br)
        {
            return BitConverter.ToInt64(br.ReadBytes(8), 0);
        }
    }
}
