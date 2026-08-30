using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace CodeImp.DoomBuilder.Rendering
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 4)]
    public struct Matrix
    {
        public float M11, M12, M13, M14;
        public float M21, M22, M23, M24;
        public float M31, M32, M33, M34;
        public float M41, M42, M43, M44;

#if USE_CSHARP_MATH

        public static Matrix Null
        {
            get
            {
                Matrix m = new Matrix();
                m.M11 = 0.0f;
                m.M12 = 0.0f;
                m.M13 = 0.0f;
                m.M14 = 0.0f;
                m.M21 = 0.0f;
                m.M22 = 0.0f;
                m.M23 = 0.0f;
                m.M24 = 0.0f;
                m.M31 = 0.0f;
                m.M32 = 0.0f;
                m.M33 = 0.0f;
                m.M34 = 0.0f;
                m.M41 = 0.0f;
                m.M42 = 0.0f;
                m.M43 = 0.0f;
                m.M44 = 0.0f;
                return m;
            }
        }

        public static Matrix Identity
        {
            get
            {
                Matrix m = Null;
                m.M11 = 1.0f;
                m.M22 = 1.0f;
                m.M33 = 1.0f;
                m.M44 = 1.0f;
                return m;
            }
        }

        public static Matrix Translation(Vector3 v)
        {
            Matrix m = Identity;
            m.M41 = v.X;
            m.M42 = v.Y;
            m.M43 = v.Z;
            return m;
        }

        public static Matrix Translation(float x, float y, float z)
        {
            Matrix m = Identity;
            m.M41 = x;
            m.M42 = y;
            m.M43 = z;
            return m;
        }

        public static Matrix RotationX(float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            Matrix result = Null;
            result.M11 = 1.0f;
            result.M22 = cos;
            result.M23 = sin;
            result.M32 = -sin;
            result.M33 = cos;
            result.M44 = 1.0f;
            return result;
        }

        public static Matrix RotationY(float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            Matrix result = Null;
            result.M11 = cos;
            result.M13 = -sin;
            result.M22 = 1.0f;
            result.M31 = sin;
            result.M33 = cos;
            result.M44 = 1.0f;
            return result;
        }

        public static Matrix RotationZ(float angle)
        {
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);

            Matrix result = Null;
            result.M11 = cos;
            result.M12 = sin;
            result.M21 = -sin;
            result.M22 = cos;
            result.M33 = 1.0f;
            result.M44 = 1.0f;
            return result;
        }

        public static Matrix Scaling(float x, float y, float z)
        {
            Matrix result = Null;
            result.M11 = x;
            result.M22 = y;
            result.M33 = z;
            result.M44 = 1.0f;
            return result;
        }

        public static Matrix Scaling(Vector3 v)
        {
            Matrix result = Null;
            result.M11 = v.X;
            result.M22 = v.Y;
            result.M33 = v.Z;
            result.M44 = 1.0f;
            return result;
        }

        public static Matrix Multiply(Matrix left, Matrix right)
        {
            Matrix result = new Matrix();
            result.M11 = (left.M11 * right.M11) + (left.M12 * right.M21) + (left.M13 * right.M31) + (left.M14 * right.M41);
            result.M12 = (left.M11 * right.M12) + (left.M12 * right.M22) + (left.M13 * right.M32) + (left.M14 * right.M42);
            result.M13 = (left.M11 * right.M13) + (left.M12 * right.M23) + (left.M13 * right.M33) + (left.M14 * right.M43);
            result.M14 = (left.M11 * right.M14) + (left.M12 * right.M24) + (left.M13 * right.M34) + (left.M14 * right.M44);
            result.M21 = (left.M21 * right.M11) + (left.M22 * right.M21) + (left.M23 * right.M31) + (left.M24 * right.M41);
            result.M22 = (left.M21 * right.M12) + (left.M22 * right.M22) + (left.M23 * right.M32) + (left.M24 * right.M42);
            result.M23 = (left.M21 * right.M13) + (left.M22 * right.M23) + (left.M23 * right.M33) + (left.M24 * right.M43);
            result.M24 = (left.M21 * right.M14) + (left.M22 * right.M24) + (left.M23 * right.M34) + (left.M24 * right.M44);
            result.M31 = (left.M31 * right.M11) + (left.M32 * right.M21) + (left.M33 * right.M31) + (left.M34 * right.M41);
            result.M32 = (left.M31 * right.M12) + (left.M32 * right.M22) + (left.M33 * right.M32) + (left.M34 * right.M42);
            result.M33 = (left.M31 * right.M13) + (left.M32 * right.M23) + (left.M33 * right.M33) + (left.M34 * right.M43);
            result.M34 = (left.M31 * right.M14) + (left.M32 * right.M24) + (left.M33 * right.M34) + (left.M34 * right.M44);
            result.M41 = (left.M41 * right.M11) + (left.M42 * right.M21) + (left.M43 * right.M31) + (left.M44 * right.M41);
            result.M42 = (left.M41 * right.M12) + (left.M42 * right.M22) + (left.M43 * right.M32) + (left.M44 * right.M42);
            result.M43 = (left.M41 * right.M13) + (left.M42 * right.M23) + (left.M43 * right.M33) + (left.M44 * right.M43);
            result.M44 = (left.M41 * right.M14) + (left.M42 * right.M24) + (left.M43 * right.M34) + (left.M44 * right.M44);
            return result;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            return Matrix.Multiply(a, b);
        }

#else

        public static Matrix Null
        {
            get
            {
                Matrix result = new Matrix();
                Matrix_Null(out result);
                return result;
            }
        }

        public static Matrix Identity
        {
            get
            {
                Matrix result = new Matrix();
                Matrix_Identity(out result);
                return result;
            }
        }

        public static Matrix Translation(Vector3f v)
        {
            Matrix result = new Matrix();
            Matrix_Translation(v.X, v.Y, v.Z, out result);
            return result;
        }

        public static Matrix Translation(float x, float y, float z)
        {
            Matrix result = new Matrix();
            Matrix_Translation(x, y, z, out result);
            return result;
        }

        public static Matrix RotationX(float angle)
        {
            Matrix result = new Matrix();
            Matrix_RotationX(angle, out result);
            return result;
        }

        public static Matrix RotationY(float angle)
        {
            Matrix result = new Matrix();
            Matrix_RotationY(angle, out result);
            return result;
        }

        public static Matrix RotationZ(float angle)
        {
            Matrix result = new Matrix();
            Matrix_RotationZ(angle, out result);
            return result;
        }

        public static Matrix Scaling(float x, float y, float z)
        {
            Matrix result = new Matrix();
            Matrix_Scaling(x, y, z, out result);
            return result;
        }

        public static Matrix Scaling(Vector3f v)
        {
            Matrix result = new Matrix();
            Matrix_Scaling(v.X, v.Y, v.Z, out result);
            return result;
        }

        public static Matrix Multiply(Matrix left, Matrix right)
        {
            Matrix result = new Matrix();
            Matrix_Multiply(ref left, ref right, out result);
            return result;
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            Matrix result = new Matrix();
            Matrix_Multiply(ref a, ref b, out result);
            return result;
        }

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void Matrix_Null(out Matrix c);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void Matrix_Identity(out Matrix c);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void Matrix_Translation(float x, float y, float z, out Matrix c);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void Matrix_RotationX(float angle, out Matrix c);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void Matrix_RotationY(float angle, out Matrix c);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void Matrix_RotationZ(float angle, out Matrix c);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void Matrix_Scaling(float x, float y, float z, out Matrix c);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void Matrix_Multiply(ref Matrix a, ref Matrix b, out Matrix c);

#endif

        public static Matrix LookAt(Vector3f eye, Vector3f target, Vector3f up)
        {
            Vector3f zaxis = Vector3f.Normalize(target - eye);
            Vector3f xaxis = Vector3f.Normalize(Vector3f.Cross(up, zaxis));
            Vector3f yaxis = Vector3f.Cross(zaxis, xaxis);

            Matrix result = Null;
            result.M11 = -xaxis.X;
            result.M12 = yaxis.X;
            result.M13 = -zaxis.X;
            result.M21 = -xaxis.Y;
            result.M22 = yaxis.Y;
            result.M23 = -zaxis.Y;
            result.M31 = -xaxis.Z;
            result.M32 = yaxis.Z;
            result.M33 = -zaxis.Z;
            result.M44 = 1.0f;
            return Matrix.Translation(-eye) * result;
        }

        public static Matrix PerspectiveFov(float fov, float aspect, float znear, float zfar)
        {
            float f = (float)(1.0 / Math.Tan(fov * 0.5f));

            Matrix result = Null;
            result.M11 = f / aspect;
            result.M22 = f;
            result.M33 = (zfar + znear) / (znear - zfar);
            result.M34 = 2.0f * zfar * znear / (znear - zfar);
            result.M43 = -1.0f;
            return result;
        }

        public static Matrix4x4 ToMatrix4x4(Matrix matrix)
        {
            return new Matrix4x4(
                matrix.M11, matrix.M21, matrix.M31, matrix.M41,
                matrix.M12, matrix.M22, matrix.M32, matrix.M42,
                matrix.M13, matrix.M23, matrix.M33, matrix.M43,
                matrix.M14, matrix.M24, matrix.M34, matrix.M44
            );
        }


        public static Matrix FromMatrix4x4(Matrix4x4 matrix)
        {
            Matrix result;

            result.M11 = matrix.M11;
            result.M12 = matrix.M21;
            result.M13 = matrix.M31;
            result.M14 = matrix.M41;
            result.M21 = matrix.M12;
            result.M22 = matrix.M22;
            result.M23 = matrix.M32;
            result.M24 = matrix.M42;
            result.M31 = matrix.M13;
            result.M32 = matrix.M23;
            result.M33 = matrix.M33;
            result.M34 = matrix.M43;
            result.M41 = matrix.M14;
            result.M42 = matrix.M24;
            result.M43 = matrix.M34;
            result.M44 = matrix.M44;

            return result;
        }

        public static Matrix Invert(Matrix matrix)
        {
            Matrix4x4 matrix4x4 = ToMatrix4x4(matrix);
            Matrix4x4.Invert(matrix4x4, out Matrix4x4 result);
            return FromMatrix4x4(result);
        }

        public override bool Equals(object o)
        {
            if (o is Matrix)
            {
                Matrix v = (Matrix)o;
                return this == v;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return
                M11.GetHashCode() + M12.GetHashCode() + M13.GetHashCode() + M14.GetHashCode() +
                M21.GetHashCode() + M22.GetHashCode() + M23.GetHashCode() + M24.GetHashCode() +
                M31.GetHashCode() + M32.GetHashCode() + M33.GetHashCode() + M34.GetHashCode() +
                M41.GetHashCode() + M42.GetHashCode() + M43.GetHashCode() + M44.GetHashCode();
        }

        public static bool operator ==(Matrix left, Matrix right)
        {
            return
                left.M11 == right.M11 && left.M12 == right.M12 && left.M13 == right.M13 && left.M14 == right.M14 &&
                left.M21 == right.M21 && left.M22 == right.M22 && left.M23 == right.M23 && left.M24 == right.M24 &&
                left.M31 == right.M31 && left.M32 == right.M32 && left.M33 == right.M33 && left.M34 == right.M34 &&
                left.M41 == right.M41 && left.M42 == right.M42 && left.M43 == right.M43 && left.M44 == right.M44;
        }

        public static bool operator !=(Matrix left, Matrix right)
        {
            return
                left.M11 != right.M11 || left.M12 != right.M12 || left.M13 != right.M13 || left.M14 != right.M14 ||
                left.M21 != right.M21 || left.M22 != right.M22 || left.M23 != right.M23 || left.M24 != right.M24 ||
                left.M31 != right.M31 || left.M32 != right.M32 || left.M33 != right.M33 || left.M34 != right.M34 ||
                left.M41 != right.M41 || left.M42 != right.M42 || left.M43 != right.M43 || left.M44 != right.M44;
        }
    }
}
