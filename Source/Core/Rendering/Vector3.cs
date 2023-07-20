using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeImp.DoomBuilder.Rendering
{
    public struct Vector3f
    {
        public Vector3f(float v)
        {
            X = v;
            Y = v;
            Z = v;
        }

        public Vector3f(Vector2f xy, float z)
        {
            X = xy.X;
            Y = xy.Y;
            Z = z;
        }

        public Vector3f(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X;
        public float Y;
        public float Z;

        public static Vector4f Transform(Vector3f vector, Matrix transform)
        {
            return new Vector4f(
                (((vector.X * transform.M11) + (vector.Y * transform.M21)) + (vector.Z * transform.M31)) + transform.M41,
                (((vector.X * transform.M12) + (vector.Y * transform.M22)) + (vector.Z * transform.M32)) + transform.M42,
                (((vector.X * transform.M13) + (vector.Y * transform.M23)) + (vector.Z * transform.M33)) + transform.M43,
                (((vector.X * transform.M14) + (vector.Y * transform.M24)) + (vector.Z * transform.M34)) + transform.M44);
        }

        public static Vector3f Hermite(Vector3f value1, Vector3f tangent1, Vector3f value2, Vector3f tangent2, float amount)
        {
            float squared = amount * amount;
            float cubed = amount * squared;
            float part1 = ((2.0f * cubed) - (3.0f * squared)) + 1.0f;
            float part2 = (-2.0f * cubed) + (3.0f * squared);
            float part3 = (cubed - (2.0f * squared)) + amount;
            float part4 = cubed - squared;
            return new Vector3f(
                (((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4),
                (((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4),
                (((value1.Z * part1) + (value2.Z * part2)) + (tangent1.Z * part3)) + (tangent2.Z * part4));
        }

        public static float DistanceSquared(Vector3f a, Vector3f b)
        {
            Vector3f c = b - a;
            return Vector3f.Dot(c, c);
        }

        public static float Dot(Vector3f a, Vector3f b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public static Vector3f Cross(Vector3f left, Vector3f right)
        {
            Vector3f result = new Vector3f();
            result.X = left.Y * right.Z - left.Z * right.Y;
            result.Y = left.Z * right.X - left.X * right.Z;
            result.Z = left.X * right.Y - left.Y * right.X;
            return result;
        }

        public float Length()
        {
            return (float)Math.Sqrt(Vector3f.Dot(this, this));
        }

        public static Vector3f Normalize(Vector3f v)
        {
            v.Normalize();
            return v;
        }

        public void Normalize()
        {
            float len = Length();
            if (len > 0.0f)
            {
                X /= len;
                Y /= len;
                Z /= len;
            }
        }

        public override bool Equals(object o)
        {
            if (o is Vector3f)
            {
                Vector3f v = (Vector3f)o;
                return this == v;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
        }

        public static Vector3f operator *(Vector3f vec, float scalar)
        {
            return new Vector3f(vec.X * scalar, vec.Y * scalar, vec.Z * scalar);
        }

        public static Vector3f operator *(float scalar, Vector3f vec)
        {
            return new Vector3f(vec.X * scalar, vec.Y * scalar, vec.Z * scalar);
        }

        public static Vector3f operator +(Vector3f left, Vector3f right)
        {
            return new Vector3f(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Vector3f operator -(Vector3f left, Vector3f right)
        {
            return new Vector3f(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static Vector3f operator -(Vector3f v)
        {
            return new Vector3f(-v.X, -v.Y, -v.Z);
        }

        public static bool operator ==(Vector3f left, Vector3f right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        public static bool operator !=(Vector3f left, Vector3f right)
        {
            return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
        }
    }

    public struct Vector3i
    {
        public Vector3i(int v)
        {
            X = v;
            Y = v;
            Z = v;
        }

        public Vector3i(Vector2i xy, int z)
        {
            X = xy.X;
            Y = xy.Y;
            Z = z;
        }

        public Vector3i(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int X;
        public int Y;
        public int Z;

        public override bool Equals(object o)
        {
            if (o is Vector3i)
            {
                Vector3i v = (Vector3i)o;
                return this == v;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
        }

        public static Vector3i operator +(Vector3i left, Vector3i right)
        {
            return new Vector3i(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Vector3i operator -(Vector3i left, Vector3i right)
        {
            return new Vector3i(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static Vector3i operator -(Vector3i v)
        {
            return new Vector3i(-v.X, -v.Y, -v.Z);
        }

        public static bool operator ==(Vector3i left, Vector3i right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }

        public static bool operator !=(Vector3i left, Vector3i right)
        {
            return left.X != right.X || left.Y != right.Y || left.Z != right.Z;
        }
    }
}
