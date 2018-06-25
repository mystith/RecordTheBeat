using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using RecordTheBeat.Enums;
using RecordTheBeat.Data;

namespace RecordTheBeat.Parsing
{
    public class Replay
    {
        public GameMode Mode { get; }
        public int Version { get; }
        public string BeatmapMD5 { get; }
        public string PlayerName { get; }
        public string ReplayMD5 { get; }
        public short Hit300 { get; }
        public short Hit100 { get; }
        public short Hit50 { get; }
        public short Geki { get; }
        public short Katu { get; }
        public short Misses { get; }
        public int Score { get; }
        public short MaxCombo { get; }
        public bool PerfCombo { get; }
        public Mods ModsUsed { get; }
        public IEnumerable<HPValue> LifeBar { get; }
        public long Timestamp { get; }
        public int LengthOfReplay { get; }
        public IEnumerable<Movement> ReplayData { get; }

        public Replay(string inputFile)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            string temporary = "";
            using (FileStream file = new FileStream(inputFile, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(file))
                {
                    Mode = (GameMode)br.ReadByte();
                    Version = Parse.ParseInteger(br);
                    BeatmapMD5 = Parse.ParseString(br);
                    PlayerName = Parse.ParseString(br);
                    ReplayMD5 = Parse.ParseString(br);
                    Hit300 = Parse.ParseShort(br);
                    Hit100 = Parse.ParseShort(br);
                    Hit50 = Parse.ParseShort(br);
                    Geki = Parse.ParseShort(br);
                    Katu = Parse.ParseShort(br);
                    Misses = Parse.ParseShort(br);
                    Score = Parse.ParseInteger(br);
                    MaxCombo = Parse.ParseShort(br);
                    PerfCombo = br.ReadByte() == 1;
                    ModsUsed = (Mods)Parse.ParseInteger(br);
                    LifeBar = Parse.ParseString(br).Skip(1).ToString().Split(',').Select(o => o.Split('|')).Select(o => new HPValue() { TimeMillis = int.Parse(o[0]), Value = float.Parse(o[1]) });
                    Timestamp = Parse.ParseLong(br);
                    LengthOfReplay = Parse.ParseInteger(br);

                    temporary = string.Concat("TEMP-", ReplayMD5);
                    File.WriteAllBytes(temporary, br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position - 8)));
                }
            }

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "7za.exe",
                    Arguments = $"e { temporary }",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            proc.WaitForExit();

            ReplayData = File.ReadAllText($"{ temporary }~").Split(',').Where(o => !string.IsNullOrWhiteSpace(o)).Select(o => o.Split('|')).Select(o => new Movement() { TimeSince = long.Parse(o[0]), X = float.Parse(o[1]), Y = float.Parse(o[2]), KeysPressed = (KeyInfo)int.Parse(o[3]) });

            File.Delete(temporary);
            File.Delete($"{ temporary }~");

            stopwatch.Stop();
            Console.WriteLine($"Finished parsing replay, took { stopwatch.Elapsed.TotalMilliseconds } ms");
        }
    }
}