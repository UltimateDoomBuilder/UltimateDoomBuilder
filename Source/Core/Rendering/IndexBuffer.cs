using System;
using System.Runtime.InteropServices;

namespace CodeImp.DoomBuilder.Rendering
{
    public class IndexBuffer : IDisposable
    {
        public IndexBuffer()
        {
            Handle = IndexBuffer_New();
            if (Handle == IntPtr.Zero)
                throw new Exception("IndexBuffer_New failed");
        }

        ~IndexBuffer()
        {
            Dispose();
        }

        public bool Disposed { get { return Handle == IntPtr.Zero; } }

        public void Dispose()
        {
            if (!Disposed)
            {
                IndexBuffer_Delete(Handle);
                Handle = IntPtr.Zero;
            }
        }

        internal IntPtr Handle;

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr IndexBuffer_New();

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void IndexBuffer_Delete(IntPtr handle);
    }
}
