using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleHelper.Data.Basic
{
    public struct Vector2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector2D(double A, double B)
        {
            this.X = A;
            this.Y = B;
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public static Vector2D operator +(Vector2D left, Vector2D right)
        {
            return new Vector2D(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2D operator +(Vector2D left, double right)
        {
            return new Vector2D(left.X + right, left.Y + right);
        }

        public static Vector2D operator -(Vector2D left, Vector2D right)
        {
            return new Vector2D(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2D operator -(Vector2D left, double right)
        {
            return new Vector2D(left.X - right, left.Y - right);
        }

        public static Vector2D operator *(Vector2D left, Vector2D right)
        {
            return new Vector2D(left.X * right.X, left.Y * right.Y);
        }

        public static Vector2D operator *(Vector2D left, double right)
        {
            return new Vector2D(left.X * right, left.Y * right);
        }

        public static Vector2D operator /(Vector2D left, Vector2D right)
        {
            return new Vector2D(left.X / right.X, left.Y / right.Y);
        }

        public static Vector2D operator /(Vector2D left, double right)
        {
            return new Vector2D(left.X / right, left.Y / right);
        }
    }
}
