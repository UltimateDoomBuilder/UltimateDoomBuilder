using System;

namespace CodeImp.DoomBuilder.Rendering
{
    public struct Color4
    {
        public Color4(int argb)
        {
            uint v = (uint)argb;
            Alpha = ((v >> 24) & 0xff) / 255.0f;
            Red = ((v >> 16) & 0xff) / 255.0f;
            Green = ((v >> 8) & 0xff) / 255.0f;
            Blue = (v & 0xff) / 255.0f;
        }

        public Color4(float r, float g, float b, float a)
        {
            Red = r;
            Green = g;
            Blue = b;
            Alpha = a;
        }

        public Color4(Vector4f c)
        {
            Red = c.X;
            Green = c.Y;
            Blue = c.Z;
            Alpha = c.W;
        }

        public Color4(System.Drawing.Color c)
        {
            Red = c.R / 255.0f;
            Green = c.G / 255.0f;
            Blue = c.B / 255.0f;
            Alpha = c.A / 255.0f;
        }

        public float Red;
        public float Green;
        public float Blue;
        public float Alpha;

        public int ToArgb()
        {
            uint r = (uint)Math.Max(Math.Min(Red * 255.0f, 255.0f), 0.0f);
            uint g = (uint)Math.Max(Math.Min(Green * 255.0f, 255.0f), 0.0f);
            uint b = (uint)Math.Max(Math.Min(Blue * 255.0f, 255.0f), 0.0f);
            uint a = (uint)Math.Max(Math.Min(Alpha * 255.0f, 255.0f), 0.0f);
            return (int)((a << 24) | (r << 16) | (g << 8) | b);
        }

        public System.Drawing.Color ToColor()
        {
            return System.Drawing.Color.FromArgb(ToArgb());
        }

        public Vector4f ToVector()
        {
            return new Vector4f(Red, Green, Blue, Alpha);
        }

        public override bool Equals(object o)
        {
            if (o is Color4)
            {
                Color4 v = (Color4)o;
                return this == v;
            }
            else
            {
                return false;
            }
        }

        public static Color4 operator +(Color4 left, Color4 right)
        {
            return new Color4(left.Red + right.Red, left.Green + right.Green, left.Blue + right.Blue, left.Alpha + right.Alpha);
        }

        public static Color4 operator -(Color4 left, Color4 right)
        {
            return new Color4(left.Red - right.Red, left.Green - right.Green, left.Blue - right.Blue, left.Alpha - right.Alpha);
        }

        public static Color4 operator -(Color4 v)
        {
            return new Color4(-v.Red, -v.Green, -v.Blue, -v.Alpha);
        }

        public override int GetHashCode()
        {
            return Red.GetHashCode() + Green.GetHashCode() + Blue.GetHashCode() + Alpha.GetHashCode();
        }

        public static bool operator ==(Color4 left, Color4 right)
        {
            return left.Red == right.Red && left.Green == right.Green && left.Blue == right.Blue && left.Alpha == right.Alpha;
        }

        public static bool operator !=(Color4 left, Color4 right)
        {
            return left.Red != right.Red || left.Green != right.Green || left.Blue != right.Blue || left.Alpha != right.Alpha;
        }
    }
}
