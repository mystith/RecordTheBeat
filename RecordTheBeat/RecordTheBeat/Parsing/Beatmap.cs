using RecordTheBeat.Data.Basic;
using RecordTheBeat.Data.HitObjects;
using RecordTheBeat.Enums;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using RecordTheBeat.Enums.HitObjects;
using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RecordTheBeat.Parsing
{
    public class Beatmap
    {
        #region General
        public string AudioFilename { get; }
        public int AudioLeadIn { get; }
        public int PreviewTime { get; }
        public bool Countdown { get; }
        public string SampleSet { get; }
        public double StackLeniency { get; }
        public GameMode Mode { get; }
        public bool LetterboxInBreaks { get; }
        public bool WidescreenStoryboard { get; }
        public bool StoryFireInFront { get; }
        public bool SpecialStyle { get; }
        public bool EpilepsyWarning { get; }
        public bool UseSkinSprites { get; }
        #endregion
        #region Metadata
        public string Title { get; }
        public string TitleUnicode { get; }
        public string Artist { get; }
        public string ArtistUnicode { get; }
        public string Creator { get; }
        public string Version { get; }
        public string Source { get; }
        public List<string> Tags { get; }
        public int BeatmapID { get; }
        public int BeatmapSetID { get; }
        #endregion
        #region Difficulty
        public double HPDrainRate { get; }
        public double CircleSize { get; }
        public double OverallDifficulty { get; }
        public double ApproachRate { get; }
        public double SliderMultiplier { get; }
        public double SliderTickRate { get; }
        #endregion
        #region Events
        public string Background { get; }
        public List<Vector2> Breaks { get; }
        #endregion
        #region Colors
        public List<Color> ComboColors { get; }
        public Color SliderBody { get; }
        public Color SliderTrackOverride { get; }
        public Color SliderBorder { get; }
        #endregion
        #region Game
        public List<TimingPoint> TimingPoints { get; }
        public List<HitCircle> HitCircles { get; }
        public List<Slider> Sliders { get; }
        public List<Spinner> Spinners { get; }

        public int MaxCombo { get; }
        public int FirstObject { get; }

        public double TotalPP { get; }
        #endregion

        public Beatmap(string osuFile)
        {
            List<string> osuFileLines = File.ReadAllLines(osuFile).ToList();

            HitCircles = new List<HitCircle>();
            Sliders = new List<Slider>();
            Spinners = new List<Spinner>();
            ComboColors = new List<Color>();

            foreach (string str in osuFileLines)
            {
                if (str.Contains("[HitObjects]")) break;
                if (!str.Contains(":")) continue;

                string[] split = str.Split(':');
                switch (split[0].ToLower())
                {
                    case "audiofilename": AudioFilename = new String(split[1].Skip(1).ToArray()); continue;
                    case "audioleadin": AudioLeadIn = int.Parse(split[1]); continue;
                    case "previewtime": PreviewTime = int.Parse(split[1]); continue;
                    case "countdown": Countdown = split[1].Contains("1"); continue;
                    case "sampleset": SampleSet = split[1]; continue;
                    case "stackleniency": StackLeniency = double.Parse(split[1]); continue;
                    case "mode": Mode = (GameMode)int.Parse(split[1]); continue;
                    case "letterboxincontinues": LetterboxInBreaks = split[1].Contains("1"); continue;
                    case "widescreenstoryboard": WidescreenStoryboard = split[1].Contains("1"); continue;
                    case "title": Title = split[1]; continue;
                    case "titleunicode": TitleUnicode = split[1]; continue;
                    case "artist": Artist = split[1]; continue;
                    case "artistunicode": ArtistUnicode = split[1]; continue;
                    case "creator": Creator = split[1]; continue;
                    case "version": Version = split[1]; continue;
                    case "source": Source = split[1]; continue;
                    case "tags": Tags = split[1].Split(' ').ToList(); continue;
                    case "beatmapid": BeatmapID = int.Parse(split[1]); continue;
                    case "beatmapsetid": BeatmapSetID = int.Parse(split[1]); continue;
                    case "hpdrainrate": HPDrainRate = double.Parse(split[1]); continue;
                    case "circlesize": CircleSize = double.Parse(split[1]); continue;
                    case "overalldifficulty": OverallDifficulty = double.Parse(split[1]); continue;
                    case "approachrate": ApproachRate = double.Parse(split[1]); continue;
                    case "slidermultiplier": SliderMultiplier = double.Parse(split[1]); continue;
                    case "slidertickrate": SliderTickRate = double.Parse(split[1]); continue;
                }

                if (split[0].ToLower().StartsWith("combo"))
                {
                    split = split[1].Split(',');
                    ComboColors.Add(Color.FromArgb(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2])));
                }
            }

            int index = osuFileLines.IndexOf(osuFileLines.Where(o => o.ToLower().Equals("//background and video events")).First()) + 1;

            if (osuFileLines.ElementAt(index).Contains("\""))
                Background = osuFileLines[index].Split('"')[1];

            IEnumerable<string> timingPoints = osuFileLines.SkipWhile(o => !o.ToLower().Contains("timingpoints")).TakeWhile(o => !o.ToLower().Contains("col") && !o.ToLower().Contains("hitobjects"));

            TimingPoints = new List<TimingPoint>();
            foreach (string timingPoint in timingPoints)
            {
                if (string.IsNullOrWhiteSpace(timingPoint) || timingPoint[0] == '[') continue;

                string[] split = timingPoint.Split(',');

                TimingPoint tp = new TimingPoint
                {
                    Offset = double.Parse(split[0]),
                    MillisPerBeat = double.Parse(split[1]),
                    Meter = int.Parse(split[2]),
                    SampleSet = int.Parse(split[3]),
                    SampleIndex = int.Parse(split[4]),
                    Volume = int.Parse(split[5]),
                    Inherited = split[6].Contains('1'),
                    Kiai = split[7].Contains('1')
                };
                TimingPoints.Add(tp);
            }

            IEnumerable<string> hitObjects = osuFileLines.SkipWhile(o => !o.ToLower().Contains("hitobjects"));

            foreach (string hitObject in hitObjects)
            {
                if (string.IsNullOrWhiteSpace(hitObject) || hitObject[0] == '[') continue;

                string[] split = hitObject.Split(',');

                string splast = split.Last();
                string[] extras = splast.Contains(":") ? splast.Split(':') : new string[5] { "0", "0", "0", "0", "" };

                HitObject hobj = new HitObject
                {
                    X = int.Parse(split[0]),
                    Y = int.Parse(split[1]),
                    Time = int.Parse(split[2]),
                    ObjectType = (HitObjectType)int.Parse(split[3]),
                    Hitsound = (HitSoundType)int.Parse(split[4]),
                    Extras = new HitsoundExtras()
                    {
                        SampleSet = int.Parse(extras[0]),
                        AdditionSet = int.Parse(extras[1]),
                        CustomIndex = int.Parse(extras[2]),
                        SampleVolume = int.Parse(extras[3]),
                        Filename = extras[4]
                    }
                };

                if ((hobj.ObjectType & HitObjectType.Circle) == HitObjectType.Circle)
                {
                    HitCircles.Add(new HitCircle()
                    {
                        X = hobj.X,
                        Y = hobj.Y,
                        Time = hobj.Time,
                        ObjectType = hobj.ObjectType,
                        Hitsound = hobj.Hitsound,
                        Extras = hobj.Extras
                    });
                }
                else if ((hobj.ObjectType & HitObjectType.Spinner) == HitObjectType.Spinner)
                {
                    Spinner spinner = new Spinner()
                    {
                        X = hobj.X,
                        Y = hobj.Y,
                        Time = hobj.Time,
                        ObjectType = hobj.ObjectType,
                        Hitsound = hobj.Hitsound,
                        Extras = hobj.Extras
                    };

                    spinner.EndTime = int.Parse(split[5]);

                    Spinners.Add(spinner);
                }
                else if ((hobj.ObjectType & HitObjectType.Slider) == HitObjectType.Slider)
                {
                    Slider slider = new Slider()
                    {
                        X = hobj.X,
                        Y = hobj.Y,
                        Time = hobj.Time,
                        ObjectType = hobj.ObjectType,
                        Hitsound = hobj.Hitsound,
                        Extras = hobj.Extras,
                        Points = new List<Vector2D>()
                    };

                    slider.Repeat = int.Parse(split[6]);
                    slider.PixelLength = double.Parse(split[7]);

                    slider.Points.Add(new Vector2D(slider.X, slider.Y));

                    string[] sliderInfo = split[5].Split('|');
                    string type = sliderInfo[0];

                    slider.Type = type == "L" ? SliderType.Linear : (type == "P" ? SliderType.Perfect : (type == "B" ? SliderType.Bezier : SliderType.Catmull));

                    slider.CurvePoints = sliderInfo.Skip(1).Select(o => o.Split(':')).Select(o => new Vector2(int.Parse(o[0]), int.Parse(o[1])));

                    if (slider.Type == SliderType.Linear)
                    {
                        slider.Points.Add(slider.CurvePoints.First().ToDoublePair());
                    }
                    else if (slider.Type == SliderType.Perfect)
                    {
                        slider.Points.AddRange(PerfectCurve(slider));
                    }
                    else if (slider.Type == SliderType.Bezier)
                    {
                        slider.Points.AddRange(BezierCurve(slider));
                    }
                    else if (slider.Type == SliderType.Catmull)
                    {
                        slider.Points.AddRange(CatmullCurve(slider));
                    }

                    Sliders.Add(slider);
                }
            }

            Process proc = new Process
            {
                StartInfo = new ProcessStartInfo("Utilities/oppai.exe", $"\"{osuFile}\" -ojson")
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            proc.Start();

            string line = proc.StandardOutput.ReadLine();
            var obj = JObject.Parse(line);
            TotalPP = (double)obj.SelectToken("pp");
            MaxCombo = (int)obj.SelectToken("max_combo");

            FirstObject = Math.Min(Math.Min(Sliders.Count > 0 ? Sliders.First().Time : 9999, HitCircles.Count > 0 ? HitCircles.First().Time : 9999), Spinners.Count > 0 ? Spinners.First().Time : 9999);
        }

        public List<Vector2D> CatmullCurve(Slider slider) //Actual catmull calculations from https://github.com/SneakyBrian/Catmull-Rom-Sample
        {
            int count = slider.CurvePoints.Count();
            if (count < 4)
                return BezierCurve(slider);

            List<Vector2D> splinePoints = new List<Vector2D>();

            for (int i = 0; i < count - 3; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    splinePoints.Add(PointOnCurve(slider.CurvePoints.ElementAt(i), slider.CurvePoints.ElementAt(i + 1), slider.CurvePoints.ElementAt(i + 2), slider.CurvePoints.ElementAt(i + 3), (1f / count) * j));
                }
            }

            splinePoints.Add(slider.CurvePoints.ElementAt(count - 2).ToDoublePair());

            return splinePoints;
        }

        public static Vector2D PointOnCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, double t)
        {
            Vector2D ret = new Vector2D(0, 0);

            double t2 = t * t;
            double t3 = t2 * t;

            ret.X = 0.5f * ((2.0f * p1.X) +
            (-p0.X + p2.X) * t +
            (2.0f * p0.X - 5.0f * p1.X + 4 * p2.X - p3.X) * t2 +
            (-p0.X + 3.0f * p1.X - 3.0f * p2.X + p3.X) * t3);

            ret.Y = 0.5f * ((2.0f * p1.Y) +
            (-p0.Y + p2.Y) * t +
            (2.0f * p0.Y - 5.0f * p1.Y + 4 * p2.Y - p3.Y) * t2 +
            (-p0.Y + 3.0f * p1.Y - 3.0f * p2.Y + p3.Y) * t3);

            return ret;
        }

        private List<Vector2D> BezierCurve(Slider slider) //Actual bezier calculations from StackOverflow
        {
            List<List<Vector2>> realCurvePoints = new List<List<Vector2>>();

            int curvePointsCount = slider.CurvePoints.Count();

            List<Vector2> buffer = new List<Vector2>();
            for (int i = 0; i < curvePointsCount; i++)
            {
                Vector2 item = slider.CurvePoints.ElementAt(i);
                buffer.Add(item);

                if (curvePointsCount - 1 > i && item == slider.CurvePoints.ElementAt(i + 1))
                {
                    realCurvePoints.Add(buffer);
                    buffer.Clear();
                }
            }

            List<Vector2D> output = new List<Vector2D>();
            foreach (List<Vector2> bezier in realCurvePoints)
            {
                Vector2D[] points = new Vector2D[bezier.Count + 1];
                for (int i = 0; i <= bezier.Count; i++)
                {
                    double t = (double)i / bezier.Count;
                    points[i] = GetBezierPoint(t, bezier, 0, bezier.Count);
                }

                output.AddRange(points);
            }

            return output;
        }

        private Vector2D GetBezierPoint(double t, List<Vector2> controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index].ToDoublePair();
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            return new Vector2D((1 - t) * P0.X + t * P1.X, (1 - t) * P0.Y + t * P1.Y);
        }

        private List<Vector2D> PerfectCurve(Slider slider)
        {
            if (slider.CurvePoints.Count() > 3)
                return BezierCurve(slider);

            Vector2D a = new Vector2D(slider.X, slider.Y);
            Vector2D b = slider.CurvePoints.First().ToDoublePair();
            Vector2D c = slider.CurvePoints.Last().ToDoublePair();

            double d = pythagorean(b, c);
            double e = pythagorean(a, c);
            double f = pythagorean(a, b);

            double aSq = d * d;
            double bSq = e * e;
            double cSq = f * f;

            if (AlmostEquals(aSq, 0) || AlmostEquals(bSq, 0) || AlmostEquals(cSq, 0))
                return BezierCurve(slider);

            double s = aSq * (bSq + cSq - aSq);
            double t = bSq * (aSq + cSq - bSq);
            double u = cSq * (aSq + bSq - cSq);

            double sum = s + t + u;

            if (AlmostEquals(sum, 0))
                return BezierCurve(slider);

            Vector2D centre = (a * s + b * t + c * u) / sum;
            Vector2D dA = a - centre;
            Vector2D dC = c - centre;

            double r = dA.Length();

            double thetaStart = Math.Atan2(dA.Y, dA.X);
            double thetaEnd = Math.Atan2(dC.Y, dC.X);

            List<Vector2D> points = new List<Vector2D>();
            for (double theta = Math.Min(thetaStart, thetaEnd); theta < Math.Max(thetaStart, thetaEnd); theta += 1)
            {
                points.Add(new Vector2D(Math.Cos(theta) * r, Math.Sin(theta)));
            }
            return points;
        }

        private double pythagorean(Vector2D a, Vector2D b)
        {
            double lengthA = Math.Abs(a.X - b.X);
            double lengthB = Math.Abs(a.Y - b.Y);

            return lengthA * lengthA + lengthB * lengthB;
        }

        private bool AlmostEquals(double a, double b)
        {
            if (Math.Abs(a - b) < 0.01)
            {
                return true;
            }
            return false;
        }
    }
}