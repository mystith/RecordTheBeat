﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RecordTheBeat.Utility
{
    public class Bezier
    {
        public static List<PointF> BezierCurveDistributed(PointF[] anchors, int resolution, float accuracy) //bezier line with equally distributed points
        {
            //calculate initial bezier curve
            PointF[] curve = BezierCurve(anchors, resolution);
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

        private static PointF[] BezierCurve(PointF[] points, int resolution)
        {
            PointF[] output = new PointF[resolution];

            //calculate bezier points along entire line
            for (int i = 0; i < resolution; i++)
            {
                output[i] = BezierPoint(points, 1 / (float) resolution * i);
            }

            return output;
        }

        private static PointF BezierPoint(PointF[] points, float t)
        {
            PointF[] currentSet = points;
            
            //interpolate using the curve points multiple times with respect to T, reducing down to one point (interpolate two different lines separately, interpolate those interpolations)
            for (int i = 0; i < points.Length - 1; i++)
            {
                if (currentSet.Length < 2) continue;
                
                //interpolate the current set of lines to reduce the amount of points by 1
                PointF[] newSet = new PointF[currentSet.Length - 1];
                for (int j = 0; j < currentSet.Length - 1; j++)
                {
                    newSet[j] = Lerp(currentSet[j], currentSet[j + 1], t);
                }

                currentSet = newSet;
            }

            return currentSet[0];
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