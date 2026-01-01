namespace CodeImp.DoomBuilder.Rendering
{
    public struct Color3
    {
        public Color3(float r, float g, float b)
        {
            Red = r;
            Green = g;
            Blue = b;
        }

        public Color3(Vector3f c)
        {
            Red = c.X;
            Green = c.Y;
            Blue = c.Z;
        }

        public Color3(System.Drawing.Color c)
        {
            Red = c.R / 255.0f;
            Green = c.G / 255.0f;
            Blue = c.B / 255.0f;
        }

        public float Red;
        public float Green;
        public float Blue;

        public override bool Equals(object o)
        {
            if (o is Color3)
            {
                Color3 v = (Color3)o;
                return this == v;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return Red.GetHashCode() + Green.GetHashCode() + Blue.GetHashCode();
        }

        public static bool operator ==(Color3 left, Color3 right)
        {
            return left.Red == right.Red && left.Green == right.Green && left.Blue == right.Blue;
        }

        public static bool operator !=(Color3 left, Color3 right)
        {
            return left.Red != right.Red || left.Green != right.Green || left.Blue != right.Blue;
        }
    }
}
