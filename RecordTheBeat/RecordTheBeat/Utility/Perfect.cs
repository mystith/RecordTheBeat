using System;
using System.Drawing;

namespace RecordTheBeat.Utility
{
    public class Perfect
    {
        //not finished
        public PointF[] CircularCurve(PointF[] anchors, int resolution)
        {
            PointF[] points = new PointF[resolution];
            
            float x1sq = anchors[0].X * anchors[0].X;
            float x2sq = anchors[1].X * anchors[1].X;
            float x3sq = anchors[2].X * anchors[2].X;

            float y1sq = anchors[0].Y * anchors[0].Y;
            float y2sq = anchors[1].Y * anchors[1].Y;
            float y3sq = anchors[2].Y * anchors[2].Y;

            float a = anchors[0].X * (anchors[1].Y - anchors[2].Y) - anchors[0].Y * (anchors[1].X - anchors[2].X) + anchors[1].X * anchors[2].Y - anchors[2].X * anchors[1].Y;
            float b = (x1sq + y1sq) * (anchors[2].Y - anchors[1].Y) + (x2sq + y2sq) * (anchors[0].Y - anchors[2].Y) + (x3sq + y3sq) * (anchors[1].Y - anchors[0].Y);
            float c = (x1sq + y1sq) * (anchors[1].X - anchors[2].X) + (x2sq + y2sq) * (anchors[2].X - anchors[0].X) + (x3sq + y3sq) * (anchors[0].X - anchors[1].X);
            float d = (x1sq + y1sq) * (anchors[2].X * anchors[1].Y - anchors[1].X * anchors[2].Y) + (x2sq + y2sq) * (anchors[0].X * anchors[2].Y - anchors[2].X * anchors[0].Y) + (x3sq + y3sq) * (anchors[1].X * anchors[0].Y - anchors[0].X * anchors[1].Y);

            float x = -b / (2 * a);
            float y = -c / (2 * a);

            float r = (float)Math.Sqrt((b * b + c * c - 4 * a * d) / (4 * a * a));

            float theta1 = (float)Math.Atan2(anchors[0].Y - y, anchors[0].X - x);
            float theta2 = (float)Math.Atan2(anchors[1].Y - y, anchors[1].X - x);
            float theta3 = (float)Math.Atan2(anchors[2].Y - y, anchors[2].X - x);

            return points;
        }

        public float Max(float a, float b, float c)
        {
            return a > b ? (a > c ? a : c) : (b > c ? b : c);
        }
        
        public float Min(float a, float b, float c)
        {
            return a < b ? (a < c ? a : c) : (b < c ? b : c);
        }
    }
}