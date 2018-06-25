using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using RecordTheBeat.Data;

namespace RecordTheBeat.Parsing
{
    public class Database
    {
        public List<DBeatmapInfo> Beatmaps { get; }
        public Database(string inputFile)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            using (FileStream file = new FileStream(inputFile, FileMode.Open))
            {
                file.SetLength(Math.Max(0, file.Length - 4));

                using (BinaryReader br = new BinaryReader(file))
                {
                    br.BaseStream.Position += 17;
                    Parse.SkipString(br);
                    int numMaps = Parse.ParseInteger(br);

                    Beatmaps = new List<DBeatmapInfo>();
                    for (int i = 0; i < numMaps; i++)
                    {
                        long index = br.BaseStream.Position;
                        DBeatmapInfo bm = new DBeatmapInfo
                        {
                            Size = BitConverter.ToInt32(br.ReadBytes(4), 0),
                            ArtistName = Parse.ParseString(br),
                            ArtistUnicode = Parse.ParseString(br),
                            Title = Parse.ParseString(br),
                            TitleUnicode = Parse.ParseString(br),
                            Creator = Parse.ParseString(br),
                            Difficulty = Parse.ParseString(br),
                            AudioName = Parse.ParseString(br),
                            MD5Hash = Parse.ParseString(br),
                            OSUFile = Parse.ParseString(br)
                        };
                        Beatmaps.Add(bm);
                        br.BaseStream.Position = index + bm.Size + 4;
                    }
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"Finished parsing database file, took { stopwatch.Elapsed.TotalMilliseconds } ms");
        }
    }
}