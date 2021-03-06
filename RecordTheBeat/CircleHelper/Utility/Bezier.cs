﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CircleHelper.Data.Basic;

namespace CircleHelper.Utility
{
    public class Bezier
    {
        public static Vector2D[] BezierCurveDistributed(Vector2D[] anchors, int resolution, double accuracy) //equally distribute points of a line, accuracy should add to 1
        {
            Vector2D[] curve = BezierCurve(anchors, resolution);
            Dictionary<Vector2D, double> points = new Dictionary<Vector2D, double>();
            
            //calculate length of curve at each point, store largest segment (final point will be added later)
            double largest = 0;
            double last = 0;
            for (int i = 0; i < curve.Length - 1; i++)
            {
                //calculate length of segment
                double len = Dist(curve[i], curve[i + 1]);

                //check if largest segment
                if (len > largest) largest = len;

                //add segment length to total length, add point to list
                last = len + (i == 0 ? 0 : last);
                points.Add(curve[i], last);
            }
            
            //add final point
            points.Add(curve[curve.Length - 1], last);

            //create new list consisting of distributed points
            double curveLength = last;
            Vector2D[] newCurve = new Vector2D[curve.Length];
            for (int i = 0; i < curve.Length; i++)
            {
                double least = double.MaxValue;
                Vector2D closest = new Vector2D();
                
                //find the point with the least difference between the length at the point on the curve and what the length at the point on the curve should be, if distributed perfectly
                //linear interpolation is used to get a more accurate answer, but is optional (set accuracy to 1)
                for(int j = 0; j < points.Count - 1; j++)
                {
                    var a = points.ElementAt(j);
                    var b = points.ElementAt(j + 1);

                    for (double k = 0; k <= 1; k += accuracy)
                    {
                        double len = Lerp(a.Value, b.Value, k);
                        double dist = Dist(len, curveLength / (curve.Length - 1) * i);
                        
                        if (dist < least)
                        {
                            least = dist;
                            closest = Lerp(a.Key, b.Key, k);
                        }
                    }
                }
                
                newCurve[i] = closest;
            }

            return newCurve;
        }

        private static Vector2D[] BezierCurve(Vector2D[] points, int resolution)
        {
            Vector2D[] output = new Vector2D[resolution];

            //calculate bezier points along entire line
            for (int i = 0; i < resolution; i++)
            {
                output[i] = BezierPoint(points, 1 / (double) resolution * i);
            }

            return output;
        }

        private static Vector2D BezierPoint(Vector2D[] points, double t)
        {
            Vector2D[] currentSet = points;
            
            //interpolate using the curve points multiple times with respect to T, reducing down to one point (interpolate two different lines separately, interpolate those interpolations)
            for (int i = 0; i < points.Length - 1; i++)
            {
                if (currentSet.Length < 2) continue;
                
                //interpolate the current set of lines to reduce the amount of points by 1
                Vector2D[] newSet = new Vector2D[currentSet.Length - 1];
                for (int j = 0; j < currentSet.Length - 1; j++)
                {
                    newSet[j] = Lerp(currentSet[j], currentSet[j + 1], t);
                }

                currentSet = newSet;
            }

            return currentSet[0];
        }
        
        private static double Dist(double a, double b)
        {
            return Math.Abs(b - a);
        }
        
        private static double Dist(Vector2D a, Vector2D b)
        {
            return (double)Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y));
        }
        
        private static double Lerp(double a, double b, double t)
        {
            //average a and b with a weight of t, adding bias to b the higher t is
            return a * (1 - t) + b * t;
        }

        private static Vector2D Lerp(Vector2D a, Vector2D b, double t)
        {
            return new Vector2D(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t));
        }
    }
}