
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Geometry;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Text;
using System.Linq;
using CodeImp.DoomBuilder.Rendering.Shaders;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
    public class RenderDeviceException : Exception
    {
        public RenderDeviceException(string message) : base(message) { }
    }

    public class RenderDevice : IDisposable
    {
		public RenderDevice(RenderTargetControl rendertarget)
		{
            RenderTarget = rendertarget;

            CreateDevice();

            DeclareUniform(UniformName.rendersettings, "rendersettings", UniformType.Vec4f);
            DeclareUniform(UniformName.projection, "projection", UniformType.Mat4);
            DeclareUniform(UniformName.desaturation, "desaturation", UniformType.Float);
            DeclareUniform(UniformName.highlightcolor, "highlightcolor", UniformType.Vec4f);
            DeclareUniform(UniformName.view, "view", UniformType.Mat4);
            DeclareUniform(UniformName.world, "world", UniformType.Mat4);
            DeclareUniform(UniformName.modelnormal, "modelnormal", UniformType.Mat4);
            DeclareUniform(UniformName.FillColor, "fillColor", UniformType.Vec4f);
            DeclareUniform(UniformName.vertexColor, "vertexColor", UniformType.Vec4f);
            DeclareUniform(UniformName.stencilColor, "stencilColor", UniformType.Vec4f);
            DeclareUniform(UniformName.lightPosAndRadius, "lightPosAndRadius", UniformType.Vec4fArray);
            DeclareUniform(UniformName.lightOrientation, "lightOrientation", UniformType.Vec4fArray);
            DeclareUniform(UniformName.light2Radius, "light2Radius", UniformType.Vec2fArray);
            DeclareUniform(UniformName.lightColor, "lightColor", UniformType.Vec4fArray);
            DeclareUniform(UniformName.ignoreNormals, "ignoreNormals", UniformType.Float);
            DeclareUniform(UniformName.spotLight, "spotLight", UniformType.Float);
            DeclareUniform(UniformName.campos, "campos", UniformType.Vec4f);
            DeclareUniform(UniformName.texturefactor, "texturefactor", UniformType.Vec4f);
            DeclareUniform(UniformName.fogsettings, "fogsettings", UniformType.Vec4f);
            DeclareUniform(UniformName.fogcolor, "fogcolor", UniformType.Vec4f);
            DeclareUniform(UniformName.sectorfogcolor, "sectorfogcolor", UniformType.Vec4f);
            DeclareUniform(UniformName.lightsEnabled, "lightsEnabled", UniformType.Float);
			DeclareUniform(UniformName.slopeHandleLength, "slopeHandleLength", UniformType.Float);
            
            // volte: classic rendering
            DeclareUniform(UniformName.drawPaletted, "drawPaletted", UniformType.Int);
            DeclareUniform(UniformName.colormapSize, "colormapSize", UniformType.Vec2i);
            DeclareUniform(UniformName.doomlightlevels, "doomlightlevels", UniformType.Int);
            DeclareUniform(UniformName.sectorLightLevel, "sectorLightLevel", UniformType.Int);

            // 2d fsaa
            CompileShader(ShaderName.display2d_fsaa, "display2d.shader", "display2d_fsaa");
            
            // 2d normal
            CompileShader(ShaderName.display2d_normal, "display2d.shader", "display2d_normal");
            CompileShader(ShaderName.display2d_fullbright, "display2d.shader", "display2d_fullbright");

            // 2d things
            CompileShader(ShaderName.things2d_thing, "things2d.shader", "things2d_thing");
            CompileShader(ShaderName.things2d_sprite, "things2d.shader", "things2d_sprite");
            CompileShader(ShaderName.things2d_fill, "things2d.shader", "things2d_fill");

            // non-fog 3d shaders
            CompileShader(ShaderName.world3d_main, "world3d.shader", "world3d_main");
            CompileShader(ShaderName.world3d_fullbright, "world3d.shader", "world3d_fullbright");
            CompileShader(ShaderName.world3d_main_highlight, "world3d.shader", "world3d_main_highlight");
            CompileShader(ShaderName.world3d_fullbright_highlight, "world3d.shader", "world3d_fullbright_highlight");
            CompileShader(ShaderName.world3d_vertex_color, "world3d.shader", "world3d_vertex_color");
            CompileShader(ShaderName.world3d_main_vertexcolor, "world3d.shader", "world3d_main_vertexcolor");
            CompileShader(ShaderName.world3d_constant_color, "world3d.shader", "world3d_constant_color");
            
            // classic rendering
            CompileShader(ShaderName.world3d_classic, "world3d.shader", "world3d_classic");
            CompileShader(ShaderName.world3d_classic_highlight, "world3d.shader", "world3d_classic_highlight");

            // skybox shader
            CompileShader(ShaderName.world3d_skybox, "world3d_skybox.shader", "world3d_skybox");

            // fog 3d shaders
            CompileShader(ShaderName.world3d_main_fog, "world3d.shader", "world3d_main_fog");
            CompileShader(ShaderName.world3d_main_highlight_vertexcolor, "world3d.shader", "world3d_highlight_vertexcolor");
            CompileShader(ShaderName.world3d_main_highlight_fog, "world3d.shader", "world3d_main_highlight_fog");
            CompileShader(ShaderName.world3d_main_fog_vertexcolor, "world3d.shader", "world3d_main_fog_vertexcolor");
            CompileShader(ShaderName.world3d_main_highlight_fog_vertexcolor, "world3d.shader", "world3d_main_highlight_fog_vertexcolor");

			// Slope handle
			CompileShader(ShaderName.world3d_slope_handle, "world3d.shader", "world3d_slope_handle");

            SetupSettings();
        }

        ~RenderDevice()
        {
            Dispose();
        }

        void CreateDevice()
        {
            // Grab the X11 Display handle by abusing reflection to access internal classes in the mono implementation.
            // That's par for the course for everything in Linux, so yeah..
            IntPtr display = IntPtr.Zero;
            Type xplatui = Type.GetType("System.Windows.Forms.XplatUIX11, System.Windows.Forms");
            if (xplatui != null)
            {
                display = (IntPtr)xplatui.GetField("DisplayHandle", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            }

            Handle = RenderDevice_New(display, RenderTarget.Handle);
            if (Handle == IntPtr.Zero)
            {
                StringBuilder sb = new StringBuilder(4096);
                BuilderNative_GetError(sb, sb.Capacity);
                throw new RenderDeviceException(string.Format("Could not create render device: {0}", sb));
            }
        }

        public bool Disposed { get { return Handle == IntPtr.Zero; } }

        void ThrowIfFailed(bool result)
        {
            if (!result)
            {
                StringBuilder sb = new StringBuilder(4096);
                BuilderNative_GetError(sb, sb.Capacity);
                throw new RenderDeviceException(sb.ToString());
            }
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                RenderDevice_Delete(Handle);
                Handle = IntPtr.Zero;
            }
        }

        public void DeclareUniform(UniformName name, string variablename, UniformType type)
        {
            RenderDevice_DeclareUniform(Handle, name, variablename, type);
        }

        public void DeclareShader(ShaderName name, string vertResourceName, string fragResourceName)
        {
            RenderDevice_DeclareShader(Handle, name, name.ToString(), GetResourceText(vertResourceName), GetResourceText(fragResourceName));
        }

        // save precompiled shaders -- don't build from scratch every time
        private static Dictionary<string, ShaderGroup> precompiledGroups = new Dictionary<string, ShaderGroup>();
        public void CompileShader(ShaderName internalName, string groupName, string shaderName)
        {
            ShaderGroup sg;

            if (precompiledGroups.ContainsKey(groupName))
                sg = precompiledGroups[groupName];
            else sg = ShaderCompiler.Compile(GetResourceText(groupName));

            Shader s = sg.GetShader(shaderName);

            if (s == null)
                throw new RenderDeviceException(string.Format("Shader {0}::{1} not found", groupName, shaderName));

            /*General.WriteLogLine(string.Format("===========================================\nDBG: loading shader {0} / {1}\n\nVertex source: {2}\n\nFragment source: {3}\n\n===========================================",
                groupName, shaderName, s.GetVertexSource(), s.GetFragmentSource()));*/
            RenderDevice_DeclareShader(Handle, internalName, internalName.ToString(), s.GetVertexSource(), s.GetFragmentSource());
        }

        static string GetResourceText(string name)
        {
            string fullname = string.Format("CodeImp.DoomBuilder.Resources.{0}", name);
            using (Stream stream = General.ThisAssembly.GetManifestResourceStream(fullname))
            {
                if (stream == null)
                    throw new Exception(string.Format("Resource {0} not found!", fullname));
                byte[] data = new byte[stream.Length];
                if (stream.Read(data, 0, data.Length) != data.Length)
                    throw new Exception("Could not read resource stream");
                int start = 0;
                if (data.Length >= 3 && data[0] == 0xef && data[1] == 0xbb && data[2] == 0xbf)
                    start = 3;
                return Encoding.UTF8.GetString(data, start, data.Length - start);
            }
        }

        public void SetShader(ShaderName shader)
        {
            RenderDevice_SetShader(Handle, shader);
        }

        public void SetUniform(UniformName uniform, bool value)
        {
            RenderDevice_SetUniform(Handle, uniform, new float[] { value ? 1.0f : 0.0f }, 1, sizeof(float));
        }

        public void SetUniform(UniformName uniform, float value)
        {
            RenderDevice_SetUniform(Handle, uniform, new float[] { value }, 1, sizeof(float));
        }

        public void SetUniform(UniformName uniform, Vector2f value)
        {
            RenderDevice_SetUniform(Handle, uniform, new float[] { value.X, value.Y }, 1, sizeof(float) * 2);
        }

        public void SetUniform(UniformName uniform, Vector3f value)
        {
            RenderDevice_SetUniform(Handle, uniform, new float[] { value.X, value.Y, value.Z }, 1, sizeof(float) * 3);
        }

        public void SetUniform(UniformName uniform, Vector4f value)
        {
            RenderDevice_SetUniform(Handle, uniform, new float[] { value.X, value.Y, value.Z, value.W }, 1, sizeof(float) * 4);
        }

        public void SetUniform(UniformName uniform, Color4 value)
        {
            RenderDevice_SetUniform(Handle, uniform, new float[] { value.Red, value.Green, value.Blue, value.Alpha }, 1, sizeof(float) * 4);
        }

        public void SetUniform(UniformName uniform, Matrix matrix)
        {
            RenderDevice_SetUniform(Handle, uniform, ref matrix, 1, sizeof(float) * 16);
        }

        public void SetUniform(UniformName uniform, ref Matrix matrix)
        {
            RenderDevice_SetUniform(Handle, uniform, ref matrix, 1, sizeof(float) * 16);
        }

        public void SetUniform(UniformName uniform, int value)
        {
            RenderDevice_SetUniform(Handle, uniform, new int[] { value }, 1, sizeof(int));
        }

        public void SetUniform(UniformName uniform, Vector2i value)
        {
            RenderDevice_SetUniform(Handle, uniform, new int[] { value.X, value.Y }, 1, sizeof(int) * 2);
        }

        public void SetUniform(UniformName uniform, Vector3i value)
        {
            RenderDevice_SetUniform(Handle, uniform, new int[] { value.X, value.Y, value.Z }, 1, sizeof(int) * 3);
        }

        public void SetUniform(UniformName uniform, Vector4i value)
        {
            RenderDevice_SetUniform(Handle, uniform, new int[] { value.X, value.Y, value.Z, value.W }, 1, sizeof(int) * 4);
        }

        public void SetUniform(UniformName uniform, Vector2f[] value)
        {
            float[] conv = new float[value.Length * 2];
            int cv = 0;
            for (int i = 0; i < conv.Length; i += 2)
            {
                conv[i] = value[cv].X;
                conv[i + 1] = value[cv].Y;
                cv++;
            }
            RenderDevice_SetUniform(Handle, uniform, conv, value.Length, sizeof(float) * conv.Length);
        }

        public void SetUniform(UniformName uniform, Vector3f[] value)
        {
            float[] conv = new float[value.Length * 3];
            int cv = 0;
            for (int i = 0; i < conv.Length; i += 3)
            {
                conv[i] = value[cv].X;
                conv[i + 1] = value[cv].Y;
                conv[i + 2] = value[cv].Z;
                cv++;
            }
            RenderDevice_SetUniform(Handle, uniform, conv, value.Length, sizeof(float) * conv.Length);
        }

        public void SetUniform(UniformName uniform, Vector4f[] value)
        {
            float[] conv = new float[value.Length * 4];
            int cv = 0;
            for (int i = 0; i < conv.Length; i += 4)
            {
                conv[i] = value[cv].X;
                conv[i + 1] = value[cv].Y;
                conv[i + 2] = value[cv].Z;
                conv[i + 3] = value[cv].W;
                cv++;
            }
            RenderDevice_SetUniform(Handle, uniform, conv, value.Length, sizeof(float) * conv.Length);
        }

        public void SetVertexBuffer(VertexBuffer buffer)
        {
            RenderDevice_SetVertexBuffer(Handle, buffer != null ? buffer.Handle : IntPtr.Zero);
        }

        public void SetIndexBuffer(IndexBuffer buffer)
        {
            RenderDevice_SetIndexBuffer(Handle, buffer != null ? buffer.Handle : IntPtr.Zero);
        }

        public void SetAlphaBlendEnable(bool value)
        {
            RenderDevice_SetAlphaBlendEnable(Handle, value);
        }

        public void SetAlphaTestEnable(bool value)
        {
            RenderDevice_SetAlphaTestEnable(Handle, value);
        }

        public void SetCullMode(Cull mode)
        {
            RenderDevice_SetCullMode(Handle, mode);
        }

        public void SetBlendOperation(BlendOperation op)
        {
            RenderDevice_SetBlendOperation(Handle, op);
        }

        public void SetSourceBlend(Blend blend)
        {
            RenderDevice_SetSourceBlend(Handle, blend);
        }

        public void SetDestinationBlend(Blend blend)
        {
            RenderDevice_SetDestinationBlend(Handle, blend);
        }

        public void SetFillMode(FillMode mode)
        {
            RenderDevice_SetFillMode(Handle, mode);
        }

        public void SetMultisampleAntialias(bool value)
        {
            RenderDevice_SetMultisampleAntialias(Handle, value);
        }

        public void SetZEnable(bool value)
        {
            RenderDevice_SetZEnable(Handle, value);
        }

        public void SetZWriteEnable(bool value)
        {
            RenderDevice_SetZWriteEnable(Handle, value);
        }

        public void SetTexture(BaseTexture value, int unit = 0)
        {
            RenderDevice_SetTexture(Handle, unit, value != null ? value.Handle : IntPtr.Zero);
        }

        public void SetSamplerFilter(TextureFilter filter, int unit = 0)
        {
            SetSamplerFilter(filter, filter, MipmapFilter.None, 0.0f, unit);
        }

        public void SetSamplerFilter(TextureFilter minfilter, TextureFilter magfilter, MipmapFilter mipfilter, float maxanisotropy, int unit = 0)
        {
            RenderDevice_SetSamplerFilter(Handle, unit, minfilter, magfilter, mipfilter, maxanisotropy);
        }

        public void SetSamplerState(TextureAddress address, int unit = 0)
        {
            RenderDevice_SetSamplerState(Handle, unit, address);
        }

        public void DrawIndexed(PrimitiveType type, int startIndex, int primitiveCount)
        {
            ThrowIfFailed(RenderDevice_DrawIndexed(Handle, type, startIndex, primitiveCount));
        }

        public void Draw(PrimitiveType type, int startIndex, int primitiveCount)
        {
            ThrowIfFailed(RenderDevice_Draw(Handle, type, startIndex, primitiveCount));
        }

        public void Draw(PrimitiveType type, int startIndex, int primitiveCount, FlatVertex[] data)
        {
            ThrowIfFailed(RenderDevice_DrawData(Handle, type, startIndex, primitiveCount, data));
        }

        public void StartRendering(bool clear, Color4 backcolor)
        {
            ThrowIfFailed(RenderDevice_StartRendering(Handle, clear, backcolor.ToArgb(), IntPtr.Zero, true));
        }

        public void StartRendering(bool clear, Color4 backcolor, Texture target, bool usedepthbuffer)
        {
            ThrowIfFailed(RenderDevice_StartRendering(Handle, clear, backcolor.ToArgb(), target.Handle, usedepthbuffer));
        }

        public void FinishRendering()
        {
            ThrowIfFailed(RenderDevice_FinishRendering(Handle));
        }

        public void Present()
        {
            ThrowIfFailed(RenderDevice_Present(Handle));
        }

        public void ClearTexture(Color4 backcolor, Texture texture)
        {
            ThrowIfFailed(RenderDevice_ClearTexture(Handle, backcolor.ToArgb(), texture.Handle));
        }

        public void CopyTexture(CubeTexture dst, CubeMapFace face)
        {
            ThrowIfFailed(RenderDevice_CopyTexture(Handle, dst.Handle, face));
        }

        public void SetBufferData(IndexBuffer buffer, int[] data)
        {
            ThrowIfFailed(RenderDevice_SetIndexBufferData(Handle, buffer.Handle, data, data.Length * Marshal.SizeOf<int>()));
        }

        public void SetBufferData(VertexBuffer buffer, int length, VertexFormat format)
        {
            int stride = (format == VertexFormat.Flat) ? FlatVertex.Stride : WorldVertex.Stride;
            ThrowIfFailed(RenderDevice_SetVertexBufferData(Handle, buffer.Handle, IntPtr.Zero, length * stride, format));
        }

        public void SetBufferData(VertexBuffer buffer, FlatVertex[] data)
        {
            ThrowIfFailed(RenderDevice_SetVertexBufferData(Handle, buffer.Handle, data, data.Length * Marshal.SizeOf<FlatVertex>(), VertexFormat.Flat));
        }

        public void SetBufferData(VertexBuffer buffer, WorldVertex[] data)
        {
            ThrowIfFailed(RenderDevice_SetVertexBufferData(Handle, buffer.Handle, data, data.Length * Marshal.SizeOf<WorldVertex>(), VertexFormat.World));
        }

        public void SetBufferSubdata(VertexBuffer buffer, long destOffset, FlatVertex[] data)
        {
            ThrowIfFailed(RenderDevice_SetVertexBufferSubdata(Handle, buffer.Handle, destOffset * FlatVertex.Stride, data, data.Length * FlatVertex.Stride));
        }

        public void SetBufferSubdata(VertexBuffer buffer, long destOffset, WorldVertex[] data)
        {
            ThrowIfFailed(RenderDevice_SetVertexBufferSubdata(Handle, buffer.Handle, destOffset * WorldVertex.Stride, data, data.Length * WorldVertex.Stride));
        }

        public void SetBufferSubdata(VertexBuffer buffer, FlatVertex[] data, long size)
        {
            if (size < 0 || size > data.Length) throw new ArgumentOutOfRangeException("size");
            ThrowIfFailed(RenderDevice_SetVertexBufferSubdata(Handle, buffer.Handle, 0, data, size * FlatVertex.Stride));
        }

        public void SetPixels(Texture texture, System.Drawing.Bitmap bitmap)
        {
            System.Drawing.Imaging.BitmapData bmpdata = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                ThrowIfFailed(RenderDevice_SetPixels(Handle, texture.Handle, bmpdata.Scan0));
            }
            finally
            {
                bitmap.UnlockBits(bmpdata);
            }
        }

        public void SetPixels(CubeTexture texture, CubeMapFace face, System.Drawing.Bitmap bitmap)
        {
            System.Drawing.Imaging.BitmapData bmpdata = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Size.Width, bitmap.Size.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                ThrowIfFailed(RenderDevice_SetCubePixels(Handle, texture.Handle, face, bmpdata.Scan0));
            }
            finally
            {
                bitmap.UnlockBits(bmpdata);
            }
        }

        public unsafe void SetPixels(Texture texture, uint* pixeldata)
        {
            ThrowIfFailed(RenderDevice_SetPixels(Handle, texture.Handle, new IntPtr(pixeldata)));
        }

        public unsafe void* MapPBO(Texture texture)
        {
            void* ptr = RenderDevice_MapPBO(Handle, texture.Handle).ToPointer();
            ThrowIfFailed(ptr != null);
            return ptr;
        }

        public void UnmapPBO(Texture texture)
        {
            ThrowIfFailed(RenderDevice_UnmapPBO(Handle, texture.Handle));
        }

        internal void RegisterResource(IRenderResource res)
        {
        }

        internal void UnregisterResource(IRenderResource res)
        {
        }

        public void SetupSettings()
		{
			// Setup renderstates
			SetAlphaBlendEnable(false);
			SetAlphaTestEnable(false);
			SetCullMode(Cull.None);
			SetDestinationBlend(Blend.InverseSourceAlpha);
			SetFillMode(FillMode.Solid);
			SetMultisampleAntialias((General.Settings.AntiAliasingSamples > 0));
			SetSourceBlend(Blend.SourceAlpha);
			SetUniform(UniformName.texturefactor, new Color4(1f, 1f, 1f, 1f));
			SetZEnable(false);
			SetZWriteEnable(false);
			
			// Texture addressing
			SetSamplerState(TextureAddress.Wrap);
			
            //mxd. It's still nice to have anisotropic filtering when texture filtering is disabled
            TextureFilter magminfilter = (General.Settings.VisualBilinear ? TextureFilter.Linear : TextureFilter.Nearest);
            SetSamplerFilter(
                magminfilter,
                magminfilter,
                General.Settings.VisualBilinear ? MipmapFilter.Linear : MipmapFilter.Nearest,
                General.Settings.FilterAnisotropy);

            // Initialize presentations
            Presentation.Initialize();
		}

        IntPtr Handle;

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr RenderDevice_New(IntPtr display, IntPtr window);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_Delete(IntPtr handle);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void RenderDevice_DeclareUniform(IntPtr handle, UniformName name, string variablename, UniformType type);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void RenderDevice_DeclareShader(IntPtr handle, ShaderName index, string name, string vertexShader, string fragShader);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        static extern void BuilderNative_GetError(StringBuilder str, int length);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern bool RenderDevice_SetShader(IntPtr handle, ShaderName name);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetUniform(IntPtr handle, UniformName name, int[] data, int count, int bytesize);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetUniform(IntPtr handle, UniformName name, float[] data, int count, int bytesize);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetUniform(IntPtr handle, UniformName name, ref Matrix data, int count, int bytesize);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetVertexBuffer(IntPtr handle, IntPtr buffer);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetIndexBuffer(IntPtr handle, IntPtr buffer);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetAlphaBlendEnable(IntPtr handle, bool value);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetAlphaTestEnable(IntPtr handle, bool value);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetCullMode(IntPtr handle, Cull mode);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetBlendOperation(IntPtr handle, BlendOperation op);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetSourceBlend(IntPtr handle, Blend blend);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetDestinationBlend(IntPtr handle, Blend blend);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetFillMode(IntPtr handle, FillMode mode);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetMultisampleAntialias(IntPtr handle, bool value);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetZEnable(IntPtr handle, bool value);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetZWriteEnable(IntPtr handle, bool value);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetTexture(IntPtr handle, int unit, IntPtr texture);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetSamplerFilter(IntPtr handle, int unit, TextureFilter minfilter, TextureFilter magfilter, MipmapFilter mipfilter, float maxanisotropy);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern void RenderDevice_SetSamplerState(IntPtr handle, int unit, TextureAddress address);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern bool RenderDevice_Draw(IntPtr handle, PrimitiveType type, int startIndex, int primitiveCount);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern bool RenderDevice_DrawIndexed(IntPtr handle, PrimitiveType type, int startIndex, int primitiveCount);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern bool RenderDevice_DrawData(IntPtr handle, PrimitiveType type, int startIndex, int primitiveCount, FlatVertex[] data);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern bool RenderDevice_StartRendering(IntPtr handle, bool clear, int backcolor, IntPtr target, bool usedepthbuffer);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern bool RenderDevice_FinishRendering(IntPtr handle);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern bool RenderDevice_Present(IntPtr handle);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern bool RenderDevice_ClearTexture(IntPtr handle, int backcolor, IntPtr texture);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern bool RenderDevice_CopyTexture(IntPtr handle, IntPtr dst, CubeMapFace face);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern bool RenderDevice_SetIndexBufferData(IntPtr handle, IntPtr buffer, int[] data, long size);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern bool RenderDevice_SetVertexBufferData(IntPtr handle, IntPtr buffer, IntPtr data, long size, VertexFormat format);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern bool RenderDevice_SetVertexBufferData(IntPtr handle, IntPtr buffer, FlatVertex[] data, long size, VertexFormat format);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern bool RenderDevice_SetVertexBufferData(IntPtr handle, IntPtr buffer, WorldVertex[] data, long size, VertexFormat format);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern bool RenderDevice_SetVertexBufferSubdata(IntPtr handle, IntPtr buffer, long destOffset, FlatVertex[] data, long sizeInBytes);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        static extern bool RenderDevice_SetVertexBufferSubdata(IntPtr handle, IntPtr buffer, long destOffset, WorldVertex[] data, long sizeInBytes);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        protected static extern bool RenderDevice_SetPixels(IntPtr handle, IntPtr texture, IntPtr data);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        protected static extern IntPtr RenderDevice_MapPBO(IntPtr handle, IntPtr texture);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        protected static extern bool RenderDevice_UnmapPBO(IntPtr handle, IntPtr texture);

        [DllImport("BuilderNative", CallingConvention = CallingConvention.Cdecl)]
        protected static extern bool RenderDevice_SetCubePixels(IntPtr handle, IntPtr texture, CubeMapFace face, IntPtr data);

        //mxd. Anisotropic filtering steps
        public static readonly List<float> AF_STEPS = new List<float> { 1.0f, 2.0f, 4.0f, 8.0f, 16.0f };

        //mxd. Antialiasing steps
        public static readonly List<int> AA_STEPS = new List<int> { 0, 2, 4, 8 };

        internal RenderTargetControl RenderTarget { get; private set; }

		// This makes a Vector3 from Vector3D
		public static Vector3f V3(Vector3D v3d)
		{
			return new Vector3f((float)v3d.x, (float)v3d.y, (float)v3d.z);
		}

		// This makes a Vector3D from Vector3
		public static Vector3D V3D(Vector3f v3)
		{
			return new Vector3D(v3.X, v3.Y, v3.Z);
		}

		// This makes a Vector2 from Vector2D
		public static Vector2f V2(Vector2D v2d)
		{
			return new Vector2f((float)v2d.x, (float)v2d.y);
		}

		// This makes a Vector2D from Vector2
		public static Vector2D V2D(Vector2f v2)
		{
			return new Vector2D(v2.X, v2.Y);
		}
    }

    public enum ShaderName : int
    {
        display2d_fsaa,
        display2d_normal,
        display2d_fullbright,
        things2d_thing,
        things2d_sprite,
        things2d_fill,
        world3d_main,
        world3d_fullbright,
        world3d_main_highlight,
        world3d_fullbright_highlight,
        world3d_main_vertexcolor,
        world3d_skybox,
        world3d_main_highlight_vertexcolor,
        world3d_p7,
        world3d_main_fog,
        world3d_p9,
        world3d_main_highlight_fog,
        world3d_p11,
        world3d_main_fog_vertexcolor,
        world3d_p13,
        world3d_main_highlight_fog_vertexcolor,
        world3d_vertex_color,
        world3d_constant_color,
		world3d_slope_handle,
        world3d_classic,
        world3d_p19,
        world3d_classic_highlight
    }

    public enum UniformType : int
    {
        Vec4f,
        Vec3f,
        Vec2f,
        Float,
        Mat4,
        Vec4i,
        Vec3i,
        Vec2i,
        Int,
        Vec4fArray,
        Vec3fArray,
        Vec2fArray
    }

    public enum UniformName : int
    {
        rendersettings,
        projection,
        desaturation,
        highlightcolor,
        view,
        world,
        modelnormal,
        FillColor,
        vertexColor,
        stencilColor,
        lightPosAndRadius,
        lightOrientation,
        light2Radius,
        lightColor,
        ignoreNormals,
        spotLight,
        campos,
        texturefactor,
        fogsettings,
        fogcolor,
        sectorfogcolor,
        lightsEnabled,
		slopeHandleLength,
        drawPaletted,
        colormapSize,
        sectorLightLevel,
        doomlightlevels
    }

    public enum VertexFormat : int { Flat, World }
    public enum Cull : int { None, Clockwise }
    public enum Blend : int { InverseSourceAlpha, SourceAlpha, One }
    public enum BlendOperation : int { Add, ReverseSubtract }
    public enum FillMode : int { Solid, Wireframe }
    public enum TextureAddress : int { Wrap, Clamp }
    public enum PrimitiveType : int { LineList, TriangleList, TriangleStrip }
    public enum TextureFilter : int { Nearest, Linear }
    public enum MipmapFilter : int { None, Nearest, Linear}
}
