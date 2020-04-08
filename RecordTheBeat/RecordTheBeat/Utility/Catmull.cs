using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RecordTheBeat.Utility
{
    public class Catmull
    {
        public static List<PointF> CatmullCurveDistributed(PointF[] anchors, int resolution, float accuracy) //equally distribute points of a line, accuracy should add to 1
        {
            PointF[] curve = CatmullCurve(anchors, resolution);
            Dictionary<PointF, float> points = new Dictionary<PointF, float>();
            
            //calculate length of curve at each point, store largest segment (final point will be added later)
            float largest = 0;
            float last = 0;
            for (int i = 0; i < curve.Length - 1; i++)
            {
                //calculate length of segment
                float len = Dist(curve[i], curve[i + 1]);

                //check if largest segment
                if (len > largest) largest = len;

                //add segment length to total length, add point to list
                last = len + (i == 0 ? 0 : last);
                points.Add(curve[i], last);
            }
            
            //add final point
            points.Add(curve[curve.Length - 1], last);

            //create new list consisting of distributed points
            float curveLength = last;
            List<PointF> newCurve = new List<PointF>();
            for (int i = 0; i < curve.Length; i++)
            {
                double least = double.MaxValue;
                PointF closest = new PointF();
                
                //find the point with the least difference between the length at the point on the curve and what the length at the point on the curve should be, if distributed perfectly
                //linear interpolation is used to get a more accurate answer, but is optional (set accuracy to 1)
                for(int j = 0; j < points.Count - 1; j++)
                {
                    var a = points.ElementAt(j);
                    var b = points.ElementAt(j + 1);

                    for (float k = 0; k <= 1; k += accuracy)
                    {
                        float len = Lerp(a.Value, b.Value, k);
                        float dist = Dist(len, curveLength / (curve.Length - 1) * i);
                        
                        if (dist < least)
                        {
                            least = dist;
                            closest = Lerp(a.Key, b.Key, k);
                        }
                    }
                }
                
                newCurve.Add(closest);
            }

            return newCurve;
        }
        
        static PointF[] CatmullCurve(PointF[] points, int resolution)
        {
            PointF[] output = new PointF[resolution];

            for (int i = 0; i < resolution; i++)
            {
                output[i] = CatmullPoint(points, 1 / (float) resolution * i);
            }

            return output;
        }

        static PointF CatmullPoint(PointF[] points, float t)
        {
            float ax = 2 * points[1].X;
            float bx = (points[2].X - points[0].X) * t;
            float cx = (2 * points[0].X - 5 * points[1].X + 4 * points[2].X - points[3].X) * t * t;
            float dx = (3 * points[1].X - 3 * points[2].X + points[3].X - points[0].X) * t * t * t;
            
            float ay = 2 * points[1].Y;
            float by = (points[2].Y - points[0].Y) * t;
            float cy = (2 * points[0].Y - 5 * points[1].Y + 4 * points[2].Y - points[3].Y) * t * t;
            float dy = (3 * points[1].Y - 3 * points[2].Y + points[3].Y - points[0].Y) * t * t * t;
            
            PointF pf = new PointF((ax + bx + cx + dx) / 2,(ay + by + cy + dy) / 2);
            return pf;
        }
        
        private static float Dist(float a, float b)
        {
            return Math.Abs(b - a);
        }
        
        private static float Dist(PointF a, PointF b)
        {
            return (float)Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
        }
        
        private static float Lerp(float a, float b, float t)
        {
            //average a and b with a weight of t, adding bias to b the higher t is
            return a * (1 - t) + b * t;
        }

        private static PointF Lerp(PointF a, PointF b, float t)
        {
            return new PointF(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t));
        }
    }
}