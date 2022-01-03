using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CodeImp.DoomBuilder.Rendering
{
    public enum TextureFormat : int
    {
        Rgba8,
        Bgra8,
        Rg16f,
        Rgba16f,
        R32f,
        Rg32f,
        Rgb32f,
        Rgba32f,
        D32f_S8,
        D24_S8
    }

    public class BaseTexture : IDisposable
    {
        public BaseTexture()
        {
            Handle = Texture_New();
            if (Handle == IntPtr.Zero)
                throw new Exception("Texture_New failed");
        }

        ~BaseTexture()
        {
            Dispose();
        }

        public bool Disposed { get { return Handle == IntPtr.Zero; } }

        public void Dispose()
        {
            if (!Disposed)
            {
                Texture_Delete(Handle);
                Handle = IntPtr.Zero;
            }
        }

        internal IntPtr Handle;

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        protected static extern IntPtr Texture_New();

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        protected static extern void Texture_Delete(IntPtr handle);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        protected static extern void Texture_Set2DImage(IntPtr handle, int width, int height, TextureFormat format);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        protected static extern void Texture_SetCubeImage(IntPtr handle, int size, TextureFormat format);
    }

    public class Texture : BaseTexture
    {
        public Texture(int width, int height, TextureFormat format)
        {
            Width = width;
            Height = height;
            Format = format;
            Texture_Set2DImage(Handle, Width, Height, Format);
        }

        public Texture(RenderDevice device, System.Drawing.Bitmap bitmap)
        {
            Width = bitmap.Width;
            Height = bitmap.Height;
            Format = TextureFormat.Bgra8;
            Texture_Set2DImage(Handle, Width, Height, Format);
            device.SetPixels(this, bitmap);
        }

        public Texture(RenderDevice device, System.Drawing.Image image)
        {
            using (var bitmap = new System.Drawing.Bitmap(image))
            {
                Width = bitmap.Width;
                Height = bitmap.Height;
                Format = TextureFormat.Bgra8;
                Texture_Set2DImage(Handle, Width, Height, Format);
                device.SetPixels(this, bitmap);
            }
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public TextureFormat Format { get; private set; }

        public object Tag { get; set; }
        public int UserData { get; set; }
    }

    public class CubeTexture : BaseTexture
    {
        public CubeTexture(RenderDevice device, int size)
        {
            Texture_SetCubeImage(Handle, size, TextureFormat.Bgra8);
        }
    }

    public enum CubeMapFace : int { PositiveX, PositiveY, PositiveZ, NegativeX, NegativeY, NegativeZ }
}
