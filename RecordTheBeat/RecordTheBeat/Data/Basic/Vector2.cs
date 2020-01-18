namespace RecordTheBeat.Data.Basic
{
    public struct Vector2
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2(int A, int B)
        {
            this.X = A;
            this.Y = B;
        }

        public Vector2D ToDoublePair()
        {
            return new Vector2D(X = X, Y = Y);
        }

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2 operator +(Vector2 left, int right)
        {
            return new Vector2(left.X + right, left.Y + right);
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2 operator -(Vector2 left, int right)
        {
            return new Vector2(left.X - right, left.Y - right);
        }

        public static Vector2 operator *(Vector2 left, Vector2 right)
        {
            return new Vector2(left.X * right.X, left.Y * right.Y);
        }

        public static Vector2 operator *(Vector2 left, int right)
        {
            return new Vector2(left.X * right, left.Y * right);
        }
    }
}
