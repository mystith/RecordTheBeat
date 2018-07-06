using NAudio.Wave;
using RecordTheBeat.Data;
using RecordTheBeat.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RecordTheBeat
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Please select osu! replay.",
                Filter = "osu! replay|*.osr|All Files|*"
            };

            string osuFolder = "";
            if (!File.Exists("config.ini"))
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog
                {
                    Description = "Please select the osu! folder."
                };

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    osuFolder = fbd.SelectedPath;
                    File.WriteAllText("config.ini", String.Concat("osupath=", fbd.SelectedPath));
                }
            } else
            {
                osuFolder = String.Join("=", File.ReadAllLines("config.ini")[0].Split('=').Skip(1));
            }

            if (true)//ofd.ShowDialog() == DialogResult.OK)
            {
                ofd.FileName = "C:/replay.osr";
                Replay replay = new Replay(ofd.FileName);

                Beatmap map = null;

                Database db = new Database(string.Concat(osuFolder, "/osu!.db"));

                DBeatmapInfo info = db.Beatmaps.First(o => o.MD5Hash == replay.BeatmapMD5);
                string beatmapPath = String.Concat(osuFolder, "\\Songs\\", info.FolderName, "\\", info.OSUFile);
                Console.WriteLine(beatmapPath);
                map = new Beatmap(beatmapPath);

                Console.WriteLine("Drawing frames...");

                Mp3FileReader file = new Mp3FileReader(String.Concat(String.Join("\\", beatmapPath.Split('\\').Reverse().Skip(1).Reverse()), "\\", map.AudioFilename));
                Console.WriteLine(file.TotalTime);

                //PrivateFontCollection privateFontCollection = new PrivateFontCollection();
                //privateFontCollection.AddFontFile("Roboto-Regular.ttf");

                //Font boldfont = new Font(privateFontCollection.Families[0], 24, FontStyle.Regular);

                //int frameCount = 780;
                //for (int i = 0; i < frameCount; i++)
                //{
                //    using (Bitmap b = new Bitmap(1920, 1080))
                //    {
                //        using (Graphics g = Graphics.FromImage(b))
                //        {
                //        }
                //    }
                //}
            }
            Console.ReadLine();
        }
    }
}
