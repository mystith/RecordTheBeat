using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using CircleHelper.Data;
using CircleHelper.Data.HitObjects;
using CircleHelper.Enums;
using CircleHelper.Enums.HitObjects;
using Serilog;

namespace CircleHelper.Parsing
{
    public static class BeatmapParser
    {
        public static Beatmap Parse(string filename)
        {
            Log.Information("Loading beatmap file {0}", filename);
            List<BeatmapMeta> beatmaps = new List<BeatmapMeta>();

            if (!File.Exists(filename))
            {
                Log.Error("Beatmap file not found");
                throw new FileNotFoundException("Beatmap file not found");
            }

            Beatmap beatmap = new Beatmap();
            
            using (StringReader sr = new StringReader(File.ReadAllText(filename)))
            {
                string section = "";
                while (true)
                {
                    string line = sr.ReadLine();

                    if (line.StartsWith("["))
                    {
                        section = line.Substring(1, line.Length - 2);
                        continue;
                    }

                    switch (section)
                    {
                        case "General":
                        case "Editor":
                        case "Metadata":
                        case "Difficulty":
                            ParseParameters(sr, ref beatmap); break;
                        case "Events":
                            ParseEvents(sr, ref beatmap); break;
                        case "TimingPoints":
                            ParseTimingPoints(sr, ref beatmap); break;
                        case "Colours":
                            ParseColors(sr, ref beatmap); break;
                        case "HitObjects":
                            ParseHitObjects(sr, ref beatmap); break;
                    }
                }
            }
        }

        private static void ParseEvents(StringReader sr, ref Beatmap map)
        {
            List<Break> breaks = new List<Break>();
            while (true)
            {
                string line = sr.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    map.Breaks = breaks;
                    return;
                }

                string[] parts = line.Split(',');

                //video and storyboard support will be added later on
                if (parts[0] == "0" || parts[0] == "Background") //background
                {
                    map.Background = parts[2];
                }
                else if (parts[0] == "2" || parts[0] == "Break") //breaks
                {
                    int start = Convert.ToInt32(parts[1]);
                    int end = Convert.ToInt32(parts[1]);
                    breaks.Add(new Break() {StartTime = start, EndTime = end});
                }
            }
        }
        
        private static void ParseTimingPoints(StringReader sr, ref Beatmap map)
        {
            List<TimingPoint> timingPoints = new List<TimingPoint>();
            while (true)
            {
                string line = sr.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    map.TimingPoints = timingPoints;
                    return;
                }

                string[] parts = line.Split(',');

                TimingPoint tp = new TimingPoint()
                {
                    Time = Convert.ToInt32(parts[0]),
                    MillisPerBeat = Convert.ToDouble(parts[1]),
                    Meter = Convert.ToInt32(parts[2]),
                    SampleSet = Convert.ToInt32(parts[3]),
                    SampleIndex = Convert.ToInt32(parts[4]),
                    Volume = Convert.ToInt32(parts[5]),
                    Inherited = parts[6] != "1",
                    Effects = (TimingEffect) Convert.ToInt32(parts[7])
                };

                tp.BPM = 1000 / tp.MillisPerBeat * 60;
                
                timingPoints.Add(tp);
            }
        }
        
        private static void ParseColors(StringReader sr, ref Beatmap map)
        {
            List<Color> colors = new List<Color>();
            while (true)
            {
                string line = sr.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    map.ComboColors = colors;
                    return;
                }

                //spaces are removed in order to make parsing later easier
                string[] parts = line.Replace(" ", "").Split(':');
                if (parts[0].Contains("Combo"))
                {
                    string[] colorComponents = parts[1].Split(',');
                    
                    Color c = Color.FromArgb(Convert.ToInt32(colorComponents[0]),
                        Convert.ToInt32(colorComponents[1]),
                        Convert.ToInt32(colorComponents[2]));
                } else if (parts[0] == "SliderTrackOverride")
                {
                    string[] colorComponents = parts[1].Split(',');
                    
                    map.SliderTrackOverride = Color.FromArgb(Convert.ToInt32(colorComponents[0]),
                        Convert.ToInt32(colorComponents[1]),
                        Convert.ToInt32(colorComponents[2]));
                } else if (parts[0] == "SliderBorder")
                {
                    string[] colorComponents = parts[1].Split(',');
                    
                    map.SliderBorder = Color.FromArgb(Convert.ToInt32(colorComponents[0]),
                        Convert.ToInt32(colorComponents[1]),
                        Convert.ToInt32(colorComponents[2]));
                }
            }
        }
        
        private static void ParseHitObjects(StringReader sr, ref Beatmap map)
        {
            List<HitCircle> circles = new List<HitCircle>();
            List<Slider> sliders = new List<Slider>();
            List<Spinner> spinners = new List<Spinner>();
            
            while (true)
            {
                string line = sr.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    map.HitCircles = circles;
                    map.Sliders = sliders;
                    map.Spinners = spinners;
                    return;
                }
                
                string[] parts = line.Split(',');
                HitObject ho = new HitObject()
                {
                    X = int.Parse(parts[0]),
                    Y = int.Parse(parts[1]),
                    Time = int.Parse(parts[2]),
                    ObjectType = (HitObjectType)int.Parse(parts[3]),
                    Hitsound = (HitSoundType)int.Parse(parts[4])
                };
                //ho.

                if ((ho.ObjectType & HitObjectType.Circle) != 0)
                {
                    circles.Add((HitCircle)ho);
                }
                else if ((ho.ObjectType & HitObjectType.Slider) != 0)
                {
                    Slider slider = (Slider) ho;
                    string[] pointData = parts[5].Split('|');

                    switch (pointData[0][0]) //Gets first character of first part of pointData, aka the character signalling the type of slider
                    {
                        case 'B': slider.Type = SliderType.Bezier; break;
                        case 'P': slider.Type = SliderType.Perfect; break;
                        case 'L': slider.Type = SliderType.Linear; break;
                        case 'C': slider.Type = SliderType.Catmull; break;
                    }

                    for (int i = 1; i < pointData.Length; i++)
                    {
                        string[] point = pointData[i].Split(':');
                        
                        Vector2 v = new Vector2(float.Parse(point[0]),float.Parse(point[1]));
                    }

                    slider.Repeat = int.Parse(parts[6]);
                    slider.PixelLength = double.Parse(parts[7]);
                    slider.EdgeHitsounds = parts[8].Split('|').Select(o => (HitSoundType) int.Parse(o)).ToList();
                    
                    slider.EdgeAdditions = new List<HitSoundSample>();
                    
                    foreach (string sample in parts[9].Split('|'))
                    {
                        string[] sets = sample.Split(':');
                        slider.EdgeAdditions.Add(new HitSoundSample()
                        {
                            SampleSet = (SampleSet)int.Parse(sets[0]),
                            AdditionSet = (SampleSet)int.Parse(sets[1])
                        });
                    }
                    
                    sliders.Add(slider);
                } 
                else if ((ho.ObjectType & HitObjectType.Spinner) != 0)
                {
                    Spinner s = (Spinner)ho;
                    s.EndTime = int.Parse(parts[5]);
                    
                    spinners.Add(s);
                }
            }
        }

        private static void ParseParameters(StringReader sr, ref Beatmap map)
        {
            while (true)
            {
                string line = sr.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                    return;

                string[] parts = line.Split(':');

                if (parts[1].StartsWith(' '))
                    parts[1] = parts[1].Substring(1, parts[1].Length - 1);

                switch (parts[0])
                {
                    //General
                    case "AudioFilename":
                        map.AudioFilename = parts[1];
                        break;
                    case "AudioLeadIn":
                        map.AudioLeadIn = Convert.ToInt32(parts[1]);
                        break;
                    case "AudioHash":
                        map.AudioHash = parts[1];
                        break;
                    case "PreviewTime":
                        map.PreviewTime = Convert.ToInt32(parts[1]);
                        break;
                    case "Countdown":
                        map.Countdown = (CountdownSpeed) Convert.ToInt32(parts[1]);
                        break;
                    case "SampleSet":
                        map.SampleSet = parts[1];
                        break;
                    case "StackLeniency":
                        map.StackLeniency = Convert.ToDouble(parts[1]);
                        break;
                    case "Mode":
                        map.Mode = (GameMode) Convert.ToInt32(parts[1]);
                        break;
                    case "LetterboxInBreaks":
                        map.LetterboxInBreaks = parts[1] == "1";
                        break;
                    case "StoryFireInFront":
                        map.StoryFireInFront = parts[1] == "1";
                        break;
                    case "UseSkinSprites":
                        map.UseSkinSprites = parts[1] == "1";
                        break;
                    case "AlwaysShowPlayfield":
                        map.AlwaysShowPlayfield = parts[1] == "1";
                        break;
                    case "OverlayPosition":
                        map.OverlayPosition = parts[1];
                        break;
                    case "SkinPreference":
                        map.SkinPreference = parts[1];
                        break;
                    case "EpilepsyWarning":
                        map.EpilepsyWarning = parts[1] == "1";
                        break;
                    case "CountdownOffset":
                        map.CountdownOffset = Convert.ToInt32(parts[1]);
                        break;
                    case "SpecialStyle":
                        map.SpecialStyle = parts[1] == "1";
                        break;
                    case "WidescreenStoryboard":
                        map.WidescreenStoryboard = parts[1] == "1";
                        break;
                    case "SamplesMatchPlaybackRate":
                        map.SamplesMatchPlaybackRate = parts[1] == "1";
                        break;

                    //Editor
                    case "Bookmarks":
                        map.Bookmarks = parts[1].Split(',').Select(o => Convert.ToInt32(o)).ToArray();
                        break;
                    case "DistanceSpacing":
                        map.DistanceSpacing = Convert.ToDouble(parts[1]);
                        break;
                    case "BeatDivisor":
                        map.BeatDivisor = Convert.ToDouble(parts[1]);
                        break;
                    case "GridSize":
                        map.GridSize = Convert.ToInt32(parts[1]);
                        break;
                    case "TimelineZoom":
                        map.TimelineZoom = Convert.ToDouble(parts[1]);
                        break;

                    //Metadata
                    case "Title":
                        map.Title = parts[1];
                        break;
                    case "TitleUnicode":
                        map.TitleUnicode = parts[1];
                        break;
                    case "Artist":
                        map.Artist = parts[1];
                        break;
                    case "ArtistUnicode":
                        map.ArtistUnicode = parts[1];
                        break;
                    case "Creator":
                        map.Creator = parts[1];
                        break;
                    case "Version":
                        map.Version = parts[1];
                        break;
                    case "Source":
                        map.Source = parts[1];
                        break;
                    case "Tags":
                        map.Tags = parts[1].Split(' ');
                        break;
                    case "BeatmapID":
                        map.BeatmapID = Convert.ToInt32(parts[1]);
                        break;
                    case "BeatmapSetID":
                        map.BeatmapSetID = Convert.ToInt32(parts[1]);
                        break;

                    //Difficulty
                    case "HPDrainRate":
                        map.HPDrainRate = Convert.ToDouble(parts[1]);
                        break;
                    case "CircleSize":
                        map.CircleSize = Convert.ToDouble(parts[1]);
                        break;
                    case "OverallDifficulty":
                        map.OverallDifficulty = Convert.ToDouble(parts[1]);
                        break;
                    case "ApproachRate":
                        map.ApproachRate = Convert.ToDouble(parts[1]);
                        break;
                    case "SliderMultiplier":
                        map.SliderMultiplier = Convert.ToDouble(parts[1]);
                        break;
                    case "SliderTickRate":
                        map.SliderTickRate = Convert.ToDouble(parts[1]);
                        break;
                }
            }
        }
    }
}