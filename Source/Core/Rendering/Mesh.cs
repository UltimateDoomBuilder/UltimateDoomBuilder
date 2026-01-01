using System;

namespace CodeImp.DoomBuilder.Rendering
{
    internal class Mesh : IDisposable
    {
        public Mesh(RenderDevice graphics, WorldVertex[] vertexData, int[] indexData)
        {
            graphics.SetBufferData(Vertices, vertexData);
            graphics.SetBufferData(Indices, indexData);
            PrimitivesCount = indexData.Length / 3;
        }

        ~Mesh()
        {
            Dispose();
        }

        internal void Draw(RenderDevice device)
        {
            device.SetVertexBuffer(Vertices);
            device.SetIndexBuffer(Indices);
            device.DrawIndexed(PrimitiveType.TriangleList, 0, PrimitivesCount);
            device.SetIndexBuffer(null);
            device.SetVertexBuffer(null);
        }

        public void Dispose()
        {
            Vertices.Dispose();
            Indices.Dispose();
        }

        VertexBuffer Vertices = new VertexBuffer();
        IndexBuffer Indices = new IndexBuffer();
        int PrimitivesCount;
    }
}
