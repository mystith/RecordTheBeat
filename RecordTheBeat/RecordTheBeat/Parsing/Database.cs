using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using RecordTheBeat.Data;
using RecordTheBeat.Data.Basic;
using System.Linq;

namespace RecordTheBeat.Parsing
{
    public class Database
    {
        public List<DBeatmapInfo> Beatmaps { get; }
        public Database(string inputFile)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            if(!File.Exists(String.Concat(inputFile, ".TEMP"))) File.Copy(inputFile, String.Concat(inputFile, ".TEMP"));
            using (FileStream file = new FileStream(String.Concat(inputFile, ".TEMP"), FileMode.Open))
            {
                file.SetLength(Math.Max(0, file.Length - 4));

                using (BinaryReader br = new BinaryReader(file))
                {
                    int version = Parse.ParseInteger(br);
                    br.BaseStream.Position += 13;
                    Parse.SkipString(br);
                    int numMaps = Parse.ParseInteger(br);

                    Beatmaps = new List<DBeatmapInfo>();
                    for (int i = 0; i < numMaps; i++)
                    {
                        long index = br.BaseStream.Position;
                        DBeatmapInfo bm = new DBeatmapInfo
                        {
                            Size = Parse.ParseInteger(br)
                        };

                        for (int _ = 0; _ < 7; _++) Parse.SkipString(br);

                        bm.MD5Hash = Parse.ParseString(br);
                        bm.OSUFile = Parse.ParseString(br);

                        br.BaseStream.Position += 15;

                        if(version < 20140609)
                        {
                            br.BaseStream.Position += 12;
                        } else
                        {
                            br.BaseStream.Position += 24;
                        }

                        if (version >= 20140609)
                        {
                            for (int _ = 0; _ < 4; _++)
                            {
                                int brbr = Parse.ParseInteger(br);
                                br.BaseStream.Position += brbr * 14;
                            }
                        }

                        br.BaseStream.Position += 12;
                        br.BaseStream.Position += Parse.ParseInteger(br) * 17 + 27;
                        for (int _ = 0; _ < 2; _++) Parse.SkipString(br);
                        br.BaseStream.Position += 2;
                        
                        Parse.SkipString(br);
                        br.BaseStream.Position += 10;

                        bm.FolderName = Parse.ParseString(br);
                        Beatmaps.Add(bm);
                        br.BaseStream.Position = index + bm.Size + 4;
                    }
                }
            }

            File.Delete(String.Concat(inputFile, ".TEMP"));
            stopwatch.Stop();
        }
    }
}