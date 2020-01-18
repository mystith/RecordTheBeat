using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using RecordTheBeat.Data;
using RecordTheBeat.Data.HitObjects;
using RecordTheBeat.Enums;
using Serilog;

namespace RecordTheBeat.Parsing
{
    public class DatabaseParser
    {
        public DatabaseMeta Parse(string filename)
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<BeatmapMeta> beatmaps = new List<BeatmapMeta>();

            if (!File.Exists(filename))
            {
                Log.Error("Database file not found");
                return null;
            }
            
            DatabaseMeta database = new DatabaseMeta();
            
            byte[] fileBytes = File.ReadAllBytes(filename);
            using (MemoryStream memoryStr = new MemoryStream(fileBytes))
            {
                using (BinaryReader br = new BinaryReader(memoryStr))
                {
                    database.Version = br.ReadInt32();
                    database.FolderCount = br.ReadInt32();
                    database.AccountUnlocked = br.ReadBoolean();
                    database.DateUnlocked = new DateTime(br.ReadInt64());
                    database.PlayerName = ReadString(br);

                    int mapCount = br.ReadInt32();

                    for (int i = 0; i < mapCount; i++)
                    {
                        BeatmapMeta bm = new BeatmapMeta
                        {
                            Size = database.Version < 20191106 ? br.ReadInt32() : 0,
                            
                            ArtistName = ReadString(br),
                            ArtistUnicode = ReadString(br),
                            Title = ReadString(br),
                            TitleUnicode = ReadString(br),
                            Creator = ReadString(br),
                            Difficulty = ReadString(br),
                            AudioFile = ReadString(br),
                            MD5 = ReadString(br),
                            MapFile = ReadString(br),
                            
                            Status = (RankedStatus) br.ReadByte(),
                           
                            HitCircleCount = br.ReadInt16(),
                            SliderCount = br.ReadInt16(),
                            SpinnerCount = br.ReadInt16(),
                            
                            LastModification = br.ReadInt64(),
                            
                            ApproachRate = database.Version < 20140609 ? br.ReadByte() : br.ReadSingle(),
                            CircleSize = database.Version < 20140609 ? br.ReadByte() : br.ReadSingle(),
                            HPDrain = database.Version < 20140609 ? br.ReadByte() : br.ReadSingle(),
                            OverallDifficulty = database.Version < 20140609 ? br.ReadByte() : br.ReadSingle(),
                            
                            SliderVelocity = br.ReadDouble(),
                            
                            STDModSR = database.Version < 20140609 ? null : ReadStarRatings(br),
                            TaikoModSR = database.Version < 20140609 ? null : ReadStarRatings(br),
                            CTBModSR = database.Version < 20140609 ? null : ReadStarRatings(br),
                            ManiaModSR = database.Version < 20140609 ? null : ReadStarRatings(br),
                            
                            DrainTime = br.ReadInt32(),
                            TotalTime = br.ReadInt32(),
                            
                            AudioPreviewTime = br.ReadInt32(),
                            
                            TimingPoints = ReadTimingPoints(br),
                            
                            BeatmapID = br.ReadInt32(),
                            BeatmapSetID = br.ReadInt32(),
                            ThreadID = br.ReadInt32(),
                            
                            STDRank = (int) br.ReadByte(),
                            TaikoRank = (int) br.ReadByte(),
                            CTBRank = (int) br.ReadByte(),
                            ManiaRank = (int) br.ReadByte(),
                            
                            LocalOffset = br.ReadInt16(),
                            StackLeniency = br.ReadSingle(),
                            GameplayMode = (Mode) br.ReadByte(),
                            Source = ReadString(br),
                            Tags = ReadString(br),
                            OnlineOffset = br.ReadInt16(),
                            TitleFont = ReadString(br),
                            Unplayed = br.ReadBoolean(),
                            LastPlay = br.ReadInt64(),
                            osz2 = br.ReadBoolean(),
                            FolderName = ReadString(br),
                            LastUpdate = br.ReadInt64(),
                            
                            IgnoreSound = br.ReadBoolean(),
                            IgnoreSkin = br.ReadBoolean(),
                            DisableSB = br.ReadBoolean(),
                            DisableVideo = br.ReadBoolean(),
                            VisualOverride = br.ReadBoolean()
                        };

                        if (database.Version < 20140609)
                            br.BaseStream.Position += 2; //skips an unknown short
                        br.BaseStream.Position += 4; //skips 2nd last mod time ?

                        bm.ManiaScrollSpeed = br.ReadByte();
                        
                        beatmaps.Add(bm);
                        Log.Debug("Read {0}", bm.Title); 
                    }

                    database.UserPermissions = br.ReadInt32();
                }
            }

            database.Beatmaps = beatmaps;
            return database;
        }

        private IEnumerable<TimingPoint> ReadTimingPoints(BinaryReader br)
        {
            List<TimingPoint> points = new List<TimingPoint>();
            
            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                TimingPoint tp = new TimingPoint
                {
                    BPM = br.ReadDouble(), 
                    Offset = br.ReadDouble(), 
                    Inherited = !br.ReadBoolean()
                };
                
                points.Add(tp);
            }

            return points;
        }

        private IEnumerable<(int Mod, double StarRating)> ReadStarRatings(BinaryReader br)
        {
            var result = new List<(int Mod, double StarRating)>();
            
            int count = br.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                (int Mod, double StarRating) msr = (0, 0);
                br.BaseStream.Position++; //skip byte that signifies its an int
                msr.Mod = br.ReadInt32();
                br.BaseStream.Position++; //skip byte that signifies its a double
                msr.StarRating = br.ReadDouble();
                
                result.Add(msr);
            }

            return result;
        }

        public string ReadString(BinaryReader br)
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