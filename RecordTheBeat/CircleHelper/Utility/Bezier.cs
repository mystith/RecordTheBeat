using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CircleHelper.Utility
{
    public class Bezier
    {
        public static List<PointF> BezierLineDistributed(PointF[] anchors, int resolution) //bezier line with equally distributed points
        {
            PointF[] line = BezierLine(anchors, resolution);
            Dictionary<PointF, float> points = new Dictionary<PointF, float>();

            float largest = 0;

            float last = 0;
            for (int i = 0; i < line.Length - 1; i++)
            {
                float len = Dist(line[i], line[i + 1]);

                if (len > largest) largest = len;

                last = len + (i == 0 ? 0 : last);
                points.Add(line[i], last);
            }
            
            points.Add(line[line.Length - 1], 0);

            float curveLength = last;

            List<PointF> newLine = new List<PointF>();
            
            for (int i = 0; i < line.Length; i += 5)
            {
                var closest = points.OrderBy(o => Dist(o.Value, curveLength / (line.Length - 1) * i));
                newLine.Add(closest.First().Key);
            }

            return newLine;
        }

        private static float Dist(float a, float b)
        {
            return Math.Abs(b - a);
        }
        
        private static float Dist(PointF a, PointF b)
        {
            return (float)Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
        }

        private static PointF[] BezierLine(PointF[] points, int resolution)
        {
            PointF[] output = new PointF[resolution];

            for (int i = 0; i < resolution; i++)
            {
                output[i] = BezierPoint(points, 1 / (float) resolution * i);
            }

            return output;
        }

        private static PointF BezierPoint(PointF[] points, float t)
        {
            PointF[] currentSet = points;
            for (int i = 0; i < points.Length - 1; i++)
            {
                if (currentSet.Length < 2) continue;
                
                PointF[] newSet = new PointF[currentSet.Length - 1];
                for (int j = 0; j < currentSet.Length - 1; j++)
                {
                    newSet[j] = Lerp(currentSet[j], currentSet[j + 1], t);
                }

                currentSet = newSet;
            }

            return currentSet[0];
        }
        
        private static float Lerp(float a, float b, float t)
        {
            return a * (1 - t) + b * t;
        }

        private static PointF Lerp(PointF a, PointF b, float t)
        {
            return new PointF(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t));
        }
    }
}