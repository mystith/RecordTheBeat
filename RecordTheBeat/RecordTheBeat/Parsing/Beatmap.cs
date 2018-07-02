using RecordTheBeat.Data.Basic;
using RecordTheBeat.Data.HitObjects;
using RecordTheBeat.Enums;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using RecordTheBeat.Enums.HitObjects;
using System;

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
        public List<IntPair> Breaks { get; }
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
        #endregion

        public Beatmap(string osuFile)
        {
            List<string> osuFileLines = File.ReadAllLines(osuFile).ToList();

            foreach (string str in osuFileLines)
            {
                if (str.Contains("[HitObjects]")) break;
                if (!str.Contains(":")) continue;

                string[] split = str.Split(':');
                switch (split[0].ToLower())
                {
                    case "audiofilename": AudioFilename = split[1]; continue;
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

            IEnumerable<string> timingPoints = osuFileLines.SkipWhile(o => !o.ToLower().Contains("timingpoints")).TakeWhile(o => !o.ToLower().Contains("hitobjects"));

            TimingPoints = new List<TimingPoint>();
            foreach (string timingPoint in timingPoints)
            {
                if (string.IsNullOrWhiteSpace(timingPoint) || timingPoint[0] == '[') continue;

                string[] split = timingPoint.Split(',');

                TimingPoint tp = new TimingPoint
                {
                    Offset = int.Parse(split[0]),
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
                string[] extras = split.Last().Split(':');

                HitObject ho = new HitObject
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

                if(ho.ObjectType == HitObjectType.Circle)
                {
                    HitCircles.Add((HitCircle)ho);
                } else if(ho.ObjectType == HitObjectType.Spinner)
                {
                    Spinner spinner = (Spinner)ho;
                    spinner.EndTime = int.Parse(split[5]);
                } else if(ho.ObjectType == HitObjectType.Slider) {
                    Slider slider = (Slider)ho;
                    slider.Points.Add(new DoublePair() { A = slider.X, B = slider.Y });

                    string[] sliderInfo = split[5].Split('|');
                    string type = sliderInfo[0];

                    slider.Type = type == "L" ? SliderType.Linear : (type == "P" ? SliderType.Perfect : (type == "B" ? SliderType.Bezier : SliderType.Catmull));

                    IEnumerable<IntPair> curvePoints = sliderInfo.Skip(1).Select(o => o.Split(':')).Select(o => new IntPair() { A = int.Parse(o[0]), B = int.Parse(o[1]) });
                    
                    if(slider.Type == SliderType.Linear)
                    {
                        slider.Points.Add(curvePoints.First().ToDoublePair());
                    } else if(slider.Type == SliderType.Perfect)
                    {

                    }
                }
            }
        }
    }
}