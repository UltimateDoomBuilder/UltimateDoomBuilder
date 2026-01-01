using System;
using System.Runtime.InteropServices;

namespace CodeImp.DoomBuilder.Rendering
{
    public class VertexBuffer : IDisposable
    {
        public VertexBuffer()
        {
            Handle = VertexBuffer_New();
            if (Handle == IntPtr.Zero)
                throw new Exception("VertexBuffer_New failed");
        }

        ~VertexBuffer()
        {
            Dispose();
        }

        public bool Disposed { get { return Handle == IntPtr.Zero; } }

        public void Dispose()
        {
            if (!Disposed)
            {
                VertexBuffer_Delete(Handle);
                Handle = IntPtr.Zero;
            }
        }

        internal IntPtr Handle;

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr VertexBuffer_New();

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void VertexBuffer_Delete(IntPtr handle);
    }
}
