using NAudio.Wave;
using RecordTheBeat.Data;
using RecordTheBeat.Parsing;
using SuperfastBlur;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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
        private static List<Font> fonts;
        private static int width;
        private static int height;
        private static int FPS;
        private static int compression;
        private static string osu_path;

        [STAThread]
        static void Main(string[] args)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Please select osu! replay.",
                Filter = "osu! replay|*.osr|All Files|*",
                Multiselect = true
            };

            if (!File.Exists("config.ini"))
            {
                string potentialOsuFolder = String.Concat(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "\\osu!");
                Console.WriteLine(potentialOsuFolder);
                if (!Directory.Exists(potentialOsuFolder))
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog
                    {
                        Description = "Please select the osu! folder."
                    };

                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        osu_path = fbd.SelectedPath;
                    }
                } else
                {
                    osu_path = potentialOsuFolder;
                    Console.WriteLine("osu! path detected as {0}, if incorrect please fix in config.ini", potentialOsuFolder);
                }

                File.WriteAllLines("config.ini", new string[] { String.Concat("osupath=", osu_path), "compression=23", "framerate=60", "width=1920", "height=1080" });
            } else
            {
                foreach (string line in File.ReadAllLines("config.ini"))
                {
                    switch (readINILineKey(line).ToLower()) {
                        case "osupath": readINILineValue(line, out osu_path); break;
                        case "compression": readINILineValue(line, out compression); break;
                        case "framerate": readINILineValue(line, out FPS); break;
                        case "width": readINILineValue(line, out width); break;
                        case "height": readINILineValue(line, out height); break;
                    }
                }
            }

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Parallel.ForEach(ofd.FileNames, item => CreateVideo(item, item.GetHashCode()));
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        private static Task CreateVideo(string replayFile, int taskNumber)
        {
            Stopwatch sw = Stopwatch.StartNew();

            Replay replay = new Replay(replayFile);
            Console.WriteLine("[{0}] Replay file parsed, Parsing database file...", taskNumber);

            Directory.CreateDirectory(replay.ReplayMD5);

            Beatmap map = null;

            Database db = new Database(string.Concat(osu_path, "/osu!.db"));

            Console.WriteLine("[{0}] Finished parsing database file, Parsing beatmap file", taskNumber);

            DBeatmapInfo info = db.Beatmaps.First(o => o.MD5Hash == replay.BeatmapMD5);
            string beatmapPath = String.Concat(osu_path, "\\Songs\\", info.FolderName, "\\");

            map = new Beatmap(String.Concat(beatmapPath, info.OSUFile));

            Console.WriteLine("[{0}] Beatmap file parsed, Drawing frames...", taskNumber);

            Mp3FileReader file = new Mp3FileReader(String.Concat(beatmapPath, "\\", map.AudioFilename));

            PrivateFontCollection privateFontCollection = new PrivateFontCollection();
            privateFontCollection.AddFontFile("Fonts/Roboto-Regular.ttf");
            privateFontCollection.AddFontFile("Fonts/Roboto-Light.ttf");

            fonts = new List<Font>
                {
                    new Font(privateFontCollection.Families[0], 220, FontStyle.Regular),
                    new Font(privateFontCollection.Families[0], 104, FontStyle.Regular),
                    new Font(privateFontCollection.Families[0], 70, FontStyle.Regular),
                    new Font(privateFontCollection.Families[0], 40, FontStyle.Regular),
                    new Font(privateFontCollection.Families[1], 34, FontStyle.Regular)
                };

            Bitmap blurredBG = (Bitmap)Image.FromFile(String.Concat(beatmapPath, "\\", map.Background));

            GaussianBlur gb = new GaussianBlur(blurredBG);
            blurredBG = gb.Process(10);

            bool skip = map.FirstObject >= 5500;

            int taskCount = 100;

            Task[] taskList = new Task[taskCount];
            int frameCount = 240;//(int)(file.TotalTime.TotalSeconds * FPS + 12 * FPS); //60fps, 3 seconds in starting sequence (+1s of fading), skip section (info below), 4 seconds in ending sequences (+1s of fading)

            //Skip section:
            //If the map starts long enough* after the song plays, there will be a skip section where 2 seconds of the start is played, and then the start of the map is skipped to (with 1 second of starting buffer)
            //If map starts as the song plays, there will be a 3 second buffer before the song plays

            //*  Time >= 5.5s then skip

            for (int i = 0; i < Math.Ceiling(frameCount / (double)taskCount); i++)
            {
                for (int k = 0; k < taskCount && (i * taskCount + k) < frameCount; k++)
                {
                    int frame = (i * taskCount + k);
                    taskList[k] = Task.Factory.StartNew(() => DrawFrame(map, replay, frame < 240 ? blurredBG : null, frame));
                }
            }

            Task.WaitAll(taskList);
            Console.WriteLine("[{0}] Frames drawn, rendering...", taskNumber);

            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo("Utilities/ffmpeg.exe", $"-y -framerate {FPS} -i {replay.ReplayMD5}/%d.png -c:v libx264 -pix_fmt yuv420p -crf {compression} {replay.ReplayMD5}.mp4")
                {
                    CreateNoWindow = false,
                    ErrorDialog = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = false
                }
            };

            proc.Start();
            proc.WaitForExit();

            Directory.Delete(replay.ReplayMD5, true);
            sw.Stop();

            Console.WriteLine("[{0}] Replay rendered, Took {1} seconds.", taskNumber, sw.Elapsed.TotalSeconds);
            return Task.CompletedTask;
        }

        private static string readINILineKey(string line)
        {
            return line.Split('=')[0];
        }

        private static void readINILineValue(string line, out string value)
        {
            value = String.Join("=", line.Split('=').Skip(1));
        }

        private static void readINILineValue(string line, out int value)
        {
            value = int.Parse(String.Join("=", line.Split('=').Skip(1)));
        }

        public static Task DrawFrame(Beatmap map, Replay replay, Bitmap blurredBG, int frame)
        {
            using (Bitmap bmp = new Bitmap(1920, 1080))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                    if (blurredBG != null)
                    {
                        lock (blurredBG)
                        {
                            g.DrawImage(blurredBG, new Rectangle(0, 0, width, height));
                        }

                        StringFormat format = new StringFormat
                        {
                            LineAlignment = StringAlignment.Far,
                            Alignment = StringAlignment.Center
                        };

                        Ensure.DrawString(bmp, g, map.ArtistUnicode, fonts[1], new Point(width / 2, 285), format);
                        Ensure.DrawString(bmp, g, map.TitleUnicode, fonts[2], new Point(width / 2, 388), format);
                        Ensure.DrawString(bmp, g, String.Concat("Played by ", replay.PlayerName), fonts[3], new Point(width / 2, 492), format);
                        Ensure.DrawString(bmp, g, String.Concat(Math.Round(replay.Accuracy * 100) / 100, "% - ", replay.MaxCombo, "x / ", map.MaxCombo), fonts[4], new Point(width / 2, 555), format);
                        Ensure.DrawString(bmp, g, replay.Score >= 10000 ? replay.Score.ToString("n0") : replay.Score.ToString("d"), fonts[4], new Point(width / 2, 620), format);
                        Ensure.DrawString(bmp, g, String.Concat(replay.Misses, "x miss"), fonts[4], new Point(width / 2, 685), format);
                        Ensure.DrawString(bmp, g, replay.Rank, fonts[0], new Point(width / 2, 1045), format);

                        if (frame > (3 * FPS)) g.FillRectangle(new SolidBrush(Color.FromArgb((int)(((frame - (3D * FPS) + 1) / FPS) * 255), 0, 0, 0)), 0, 0, width, height);
                    }

                    bmp.Save(String.Concat(replay.ReplayMD5, "\\", frame, ".png"));
                }
            }

            return Task.CompletedTask;
        }
    }
}
