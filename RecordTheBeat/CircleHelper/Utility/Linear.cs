using CircleHelper.Data.Basic;

namespace CircleHelper.Utility
{
    public static class Linear
    {
        public static Vector2D[] LinearLine(Vector2D[] anchors, int resolution)
        {
            Vector2D[] points = new Vector2D[resolution];
            
            for (int i = 0; i < resolution; i++)
            {
                points[i] = Lerp(anchors[0], anchors[1], i / (double) resolution);
            }
            
            return points;
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