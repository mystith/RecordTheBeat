using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using CircleHelper.Data;
using CircleHelper.Enums;
using Serilog;

namespace CircleHelper.Parsing
{
    public static class ReplayParser
    {
        public static Replay Parse(string filename)
        {
            Stopwatch sw = Stopwatch.StartNew();

            if (!File.Exists(filename))
            {
                Log.Error("Replay file not found");
                throw new FileNotFoundException("Replay file not found");
            }

            Replay replay;
            byte[] fileBytes = File.ReadAllBytes(filename);
            using (MemoryStream memoryStr = new MemoryStream(fileBytes))
            {
                using (BinaryReader br = new BinaryReader(memoryStr))
                {
                    replay = new Replay()
                    {
                        Gamemode = (Mode)br.ReadByte(),
                        Version = br.ReadInt32(),
                        BeatmapMD5 = ReadString(br),
                        Player = ReadString(br),
                        ReplayMD5 = ReadString(br),
                        Num300s = br.ReadInt16(),
                        Num100s = br.ReadInt16(),
                        Num50s = br.ReadInt16(),
                        NumGekis = br.ReadInt16(),
                        NumKatus = br.ReadInt16(),
                        NumMisses = br.ReadInt16(),
                        TotalScore = br.ReadInt32(),
                        MaxCombo = br.ReadInt16(),
                        PerfectCombo = br.ReadByte() == 1,
                        ModsUsed = (Mods)br.ReadInt32(),
                        HPData = ReadHPData(br),
                        Time = br.ReadInt64(),
                        CursorData = ReadCursorData(br),
                        OnlineID = br.ReadInt64()
                    };
                }
            }

            return replay;
        }

        private static HPValue[] ReadHPData(BinaryReader br)
        {
            string hpString = ReadString(br);
            string[] values = hpString.Split(',');

            HPValue[] result = new HPValue[values.Length];

            int i = 0;
            foreach (string s in values)
            {
                string[] spl = s.Split('|');

                if (spl.Length < 2) continue;
                
                result[i] = new HPValue()
                {
                    Time = Convert.ToInt32(spl[0]),
                    Value = Convert.ToSingle(spl[1]),
                };

                i++;
            }
            
            return result;
        }

        private static Movement[] ReadCursorData(BinaryReader br)
        {
            int replayLength = br.ReadInt32();
            byte[] rawData = br.ReadBytes(replayLength);

            string filename = $"TEMP-{rawData.GetHashCode()}";
            File.WriteAllBytes(filename, rawData);

            Process sevZip = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "7za",
                    Arguments = $"e { filename } -aoa",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                }
            };

            sevZip.Start();

            bool success = false;
            while (!sevZip.StandardOutput.EndOfStream)
            {
                string line = sevZip.StandardOutput.ReadLine();
                
                if (line.Contains("Everything is Ok")) success = true;
                
                Log.Information(line);
            }
            
            if(!success)
            {
                Log.Error("Cursor data decompression failed");
                throw new FormatException("Cursor data decompression failed");
            }

            string decompressed = File.ReadAllText($"{filename}~");
            
            File.Delete(filename);
            File.Delete($"{filename}~");
            
            string[] movementFrames = decompressed.Split(",");
            
            Movement[] processed = new Movement[movementFrames.Length];
            
            long time = 0;

            int i = 0;
            foreach (string s in movementFrames)
            {
                string[] frame = s.Split('|');

                if (frame.Length != 4) continue;
                
                time += Convert.ToInt64(frame[0]);

                processed[i] = new Movement()
                {
                    Time = time,
                    X = Convert.ToSingle(frame[3]),
                    Y = Convert.ToSingle(frame[3]),
                    KeysPressed = (KeyInfo) Convert.ToInt32(frame[3])
                };
                
                i++;
            }
            
            return processed;
        }
        private static byte[] ReadStringBytes(BinaryReader br) //reads the string format used in osu files
        {
            byte initial = br.ReadByte(); //a single byte which will be either 0x00, indicating that the next two parts are not present, or 0x0b (decimal 11), indicating that the next two parts are present. If it is 0x0b, there will then be a ULEB128, representing the byte length of the following string, and then the string itself, encoded in UTF-8.
            int byteLength = 0;
            int shift = 0;
            
            if (initial == 0)
                return new byte[0];

            while (true)
            {
                byte b = br.ReadByte();

                //reads 7 more bits to byteLength (& is to get only 7 bits from b)
                byteLength |= (b & 0b0111_1111) << shift;

                //is b the last byte? if yes then first bit will be 0
                if ((b & 0b1000_0000) == 0)
                    break;

                //shift the or 7 bits to the left
                shift += 7;
            }

            return br.ReadBytes(byteLength);
        }
        
        private static string ReadString(BinaryReader br) //reads the string format used in osu files
        {
            byte initial = br.ReadByte(); //a single byte which will be either 0x00, indicating that the next two parts are not present, or 0x0b (decimal 11), indicating that the next two parts are present. If it is 0x0b, there will then be a ULEB128, representing the byte length of the following string, and then the string itself, encoded in UTF-8.
            int byteLength = 0;
            int shift = 0;
            
            if (initial == 0)
                return "";

            while (true)
            {
                byte b = br.ReadByte();

                //reads 7 more bits to byteLength (& is to get only 7 bits from b)
                byteLength |= (b & 0b0111_1111) << shift;

                //is b the last byte? if yes then first bit will be 0
                if ((b & 0b1000_0000) == 0)
                    break;

                //shift the or 7 bits to the left
                shift += 7;
            }

            return Encoding.UTF8.GetString(br.ReadBytes(byteLength));
        }
    }
}