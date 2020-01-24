using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using CircleHelper.Data;
using CircleHelper.Enums;
using Serilog;

namespace CircleHelper.Parsing
{
    public class ReplayParser
    {
        public Replay Parse(string filename)
        {
            Stopwatch sw = Stopwatch.StartNew();
            List<BeatmapMeta> beatmaps = new List<BeatmapMeta>();

            if (!File.Exists(filename))
            {
                Log.Error("Replay file not found");
                return null;
            }
            
            DatabaseMeta database = new DatabaseMeta();
            
            byte[] fileBytes = File.ReadAllBytes(filename);
            using (MemoryStream memoryStr = new MemoryStream(fileBytes))
            {
                using (BinaryReader br = new BinaryReader(memoryStr))
                {
                    //for (int i = 0; i < mapCount; i++)
                    //{
                        
                    //}

                    database.UserPermissions = br.ReadInt32();
                }
            }

            //database.Beatmaps = beatmaps;
            //return database;
            return null;
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