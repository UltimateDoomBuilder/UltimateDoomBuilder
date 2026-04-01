namespace CodeImp.DoomBuilder.Rendering
{
    public struct Vector2f
    {
        public Vector2f(float v)
        {
            X = v;
            Y = v;
        }

        public Vector2f(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X;
        public float Y;

        public static Vector2f Hermite(Vector2f value1, Vector2f tangent1, Vector2f value2, Vector2f tangent2, float amount)
        {
            float squared = amount * amount;
            float cubed = amount * squared;
            float part1 = ((2.0f * cubed) - (3.0f * squared)) + 1.0f;
            float part2 = (-2.0f * cubed) + (3.0f * squared);
            float part3 = (cubed - (2.0f * squared)) + amount;
            float part4 = cubed - squared;
            return new Vector2f(
                (((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4),
                (((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4));
        }

        public override bool Equals(object o)
        {
            if (o is Vector2f)
            {
                Vector2f v = (Vector2f)o;
                return this == v;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public static Vector2f operator +(Vector2f left, Vector2f right)
        {
            return new Vector2f(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2f operator -(Vector2f left, Vector2f right)
        {
            return new Vector2f(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2f operator -(Vector2f v)
        {
            return new Vector2f(-v.X, -v.Y);
        }

        public static bool operator ==(Vector2f left, Vector2f right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Vector2f left, Vector2f right)
        {
            return left.X != right.X || left.Y != right.Y;
        }
    }

    public struct Vector2i
    {
        public Vector2i(int v)
        {
            X = v;
            Y = v;
        }

        public Vector2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X;
        public int Y;

        public override bool Equals(object o)
        {
            if (o is Vector2i)
            {
                Vector2i v = (Vector2i)o;
                return this == v;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public static Vector2i operator +(Vector2i left, Vector2i right)
        {
            return new Vector2i(left.X + right.X, left.Y + right.Y);
        }

        public static Vector2i operator -(Vector2i left, Vector2i right)
        {
            return new Vector2i(left.X - right.X, left.Y - right.Y);
        }

        public static Vector2i operator -(Vector2i v)
        {
            return new Vector2i(-v.X, -v.Y);
        }

        public static bool operator ==(Vector2i left, Vector2i right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Vector2i left, Vector2i right)
        {
            return left.X != right.X || left.Y != right.Y;
        }
    }
}
