using System;
using System.Drawing;
using CircleHelper.Data.Basic;

namespace CircleHelper.Utility
{
    public static class Perfect
    {
        public static Vector2D[] CircularCurve(Vector2D[] anchors, int resolution)
        {
            Vector2D[] points = new Vector2D[resolution];
            
            double x1sq = anchors[0].X * anchors[0].X;
            double x2sq = anchors[1].X * anchors[1].X;
            double x3sq = anchors[2].X * anchors[2].X;

            double y1sq = anchors[0].Y * anchors[0].Y;
            double y2sq = anchors[1].Y * anchors[1].Y;
            double y3sq = anchors[2].Y * anchors[2].Y;

            double a = anchors[0].X * (anchors[1].Y - anchors[2].Y) - anchors[0].Y * (anchors[1].X - anchors[2].X) + anchors[1].X * anchors[2].Y - anchors[2].X * anchors[1].Y;
            double b = (x1sq + y1sq) * (anchors[2].Y - anchors[1].Y) + (x2sq + y2sq) * (anchors[0].Y - anchors[2].Y) + (x3sq + y3sq) * (anchors[1].Y - anchors[0].Y);
            double c = (x1sq + y1sq) * (anchors[1].X - anchors[2].X) + (x2sq + y2sq) * (anchors[2].X - anchors[0].X) + (x3sq + y3sq) * (anchors[0].X - anchors[1].X);
            double d = (x1sq + y1sq) * (anchors[2].X * anchors[1].Y - anchors[1].X * anchors[2].Y) + (x2sq + y2sq) * (anchors[0].X * anchors[2].Y - anchors[2].X * anchors[0].Y) + (x3sq + y3sq) * (anchors[1].X * anchors[0].Y - anchors[0].X * anchors[1].Y);

            double x = -b / (2 * a);
            double y = -c / (2 * a);

            double r = Math.Sqrt((b * b + c * c - 4 * a * d) / (4 * a * a));

            double thetaA = Math.Atan2(anchors[0].Y - y, anchors[0].X - x);
            double thetaB = Math.Atan2(anchors[2].Y - y, anchors[2].X - x);

            double minTheta = Math.Min(thetaA, thetaB);
            double maxTheta = Math.Max(thetaA, thetaB);

            bool direction = minTheta == thetaA; //is minTheta the starting point of the slider? t/f

            for (int i = 0; i < resolution; i++)
            {
                double angle = Lerp(direction ? minTheta : maxTheta, direction ? maxTheta : minTheta, i / (double)resolution);
                points[i] = new Vector2D(x + Math.Cos(angle) * r, y + Math.Sin(angle) * r);
            }
            
            return points;
        }

        public static double Max(double a, double b, double c)
        {
            return a > b ? (a > c ? a : c) : (b > c ? b : c);
        }
        
        public static double Min(double a, double b, double c)
        {
            return a < b ? (a < c ? a : c) : (b < c ? b : c);
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