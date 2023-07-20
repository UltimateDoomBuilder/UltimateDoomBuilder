using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeImp.DoomBuilder.GZBuilder.Models
{
    internal class IQMModelLoader : ModelLoader
    {
        public static ModelLoadResult Load(ref BoundingBoxSizes bbs, Dictionary<int, string> skins, Stream s, int frame)
        {
            try
            {
                var reader = new IQMFileReader(s);

                if (!reader.ReadBytes(16).SequenceEqual(Encoding.ASCII.GetBytes("INTERQUAKEMODEL\0")))
                    throw new Exception("Not an IQM file!");

                uint version = reader.ReadUInt32();
                if (version != 2)
                    throw new Exception("Unsupported IQM version");

                uint filesize = reader.ReadUInt32();
                uint flags = reader.ReadUInt32();
                uint num_text = reader.ReadUInt32();
                uint ofs_text = reader.ReadUInt32();
                uint num_meshes = reader.ReadUInt32();
                uint ofs_meshes = reader.ReadUInt32();
                uint num_vertexarrays = reader.ReadUInt32();
                uint num_vertices = reader.ReadUInt32();
                uint ofs_vertexarrays = reader.ReadUInt32();
                uint num_triangles = reader.ReadUInt32();
                uint ofs_triangles = reader.ReadUInt32();
                uint ofs_adjacency = reader.ReadUInt32();
                uint num_joints = reader.ReadUInt32();
                uint ofs_joints = reader.ReadUInt32();
                uint num_poses = reader.ReadUInt32();
                uint ofs_poses = reader.ReadUInt32();
                uint num_anims = reader.ReadUInt32();
                uint ofs_anims = reader.ReadUInt32();
                uint num_frames = reader.ReadUInt32();
                uint num_framechannels = reader.ReadUInt32();
                uint ofs_frames = reader.ReadUInt32();
                uint ofs_bounds = reader.ReadUInt32();
                uint num_comment = reader.ReadUInt32();
                uint ofs_comment = reader.ReadUInt32();
                uint num_extensions = reader.ReadUInt32();
                uint ofs_extensions = reader.ReadUInt32();

                if (num_text == 0)
                    throw new Exception("IQM model needs material names");

                reader.SeekTo(ofs_text);
                var text = reader.ReadBytes((int)num_text);
                text[text.Length - 1] = 0;

                var Meshes = new List<IQMMesh>();
                var Indexes = new int[3 * num_triangles];
                var Adjacency = new int[3 * num_triangles];
                var Joints = new List<IQMJoint>();
                var Poses = new List<IQMPose>();
                var Anims = new List<IQMAnim>();
                var Bounds = new List<IQMBounds>();
                var VertexArrays = new List<IQMVertexArray>();
                var baseframe = new List<IQMMatrix>();
                var inversebaseframe = new List<IQMMatrix>();
                var TRSData = new List<TRS>();

                reader.SeekTo(ofs_meshes);
                for (int i = 0; i < num_meshes; i++)
                {
                    var mesh = new IQMMesh();
                    mesh.Name = reader.ReadName(text);
                    mesh.Material = reader.ReadName(text);
                    mesh.FirstVertex = reader.ReadUInt32();
                    mesh.NumVertices = reader.ReadUInt32();
                    mesh.FirstTriangle = reader.ReadUInt32();
                    mesh.NumTriangles = reader.ReadUInt32();
                    Meshes.Add(mesh);
                }

                reader.SeekTo(ofs_triangles);
                for (int i = 0; i < num_triangles * 3; i++)
                {
                    Indexes[i] = reader.ReadInt32();
                }

                reader.SeekTo(ofs_adjacency);
                for (int i = 0; i < num_triangles * 3; i++)
                {
                    Adjacency[i] = reader.ReadInt32();
                }

                reader.SeekTo(ofs_joints);
                for (int i = 0; i < num_joints; i++)
                {
                    var joint = new IQMJoint();
                    joint.Name = reader.ReadName(text);
                    joint.Parent = reader.ReadInt32();
                    joint.Translate.X = reader.ReadSingle();
                    joint.Translate.Y = reader.ReadSingle();
                    joint.Translate.Z = reader.ReadSingle();
                    joint.Quaternion.X = reader.ReadSingle();
                    joint.Quaternion.Y = reader.ReadSingle();
                    joint.Quaternion.Z = reader.ReadSingle();
                    joint.Quaternion.W = reader.ReadSingle();
                    joint.Quaternion.Normalize();
                    joint.Scale.X = reader.ReadSingle();
                    joint.Scale.Y = reader.ReadSingle();
                    joint.Scale.Z = reader.ReadSingle();
                    Joints.Add(joint);
                }

                reader.SeekTo(ofs_poses);
                for (int j = 0; j < num_poses; j++)
                {
                    var pose = new IQMPose();
                    pose.Parent = reader.ReadInt32();
                    pose.ChannelMask = reader.ReadUInt32();
                    for (int i = 0; i < 10; i++) pose.ChannelOffset[i] = reader.ReadSingle();
                    for (int i = 0; i < 10; i++) pose.ChannelScale[i] = reader.ReadSingle();
                    Poses.Add(pose);
                }

                reader.SeekTo(ofs_anims);
                for (int i = 0; i < num_anims; i++)
                {
                    var anim = new IQMAnim();
                    anim.Name = reader.ReadName(text);
                    anim.FirstFrame = reader.ReadUInt32();
                    anim.NumFrames = reader.ReadUInt32();
                    anim.Framerate = reader.ReadSingle();
                    anim.Loop = (reader.ReadUInt32() & 1) == 1;
                    Anims.Add(anim);
                }

                for (uint i = 0; i < num_joints; i++)
                {
                    var bf = new IQMMatrix();
                    var ibf = new IQMMatrix();

                    IQMJoint j = Joints[(int)i];

                    var m = new IQMMatrix();
                    m.LoadIdentity();
                    m.Translate(j.Translate.X, j.Translate.Y, j.Translate.Z);
                    m.MultQuaternion(j.Quaternion);
                    m.Scale(j.Scale.X, j.Scale.Y, j.Scale.Z);

                    var invm = new IQMMatrix();
                    invm = m.InverseMatrix();

                    if (j.Parent >= 0)
                    {
                        bf.LoadMatrix(baseframe[j.Parent]);
                        bf.MultMatrix(m);
                        ibf = invm;
                        ibf.MultMatrix(inversebaseframe[j.Parent]);
                    }
                    else
                    {
                        bf = m;
                        ibf = invm;
                    }

                    baseframe.Add(bf);
                    inversebaseframe.Add(ibf);
                }

                if (num_frames != 0)
                {
                    reader.SeekTo(ofs_frames);
                    for (uint i = 0; i < num_frames; i++)
                    {
                        for (uint j = 0; j < num_poses; j++)
                        {
                            IQMPose p = Poses[(int)j];

                            var translate = new Vector3f();
                            translate.X = p.ChannelOffset[0]; if ((p.ChannelMask & 0x01) != 0) translate.X += reader.ReadUInt16() * p.ChannelScale[0];
                            translate.Y = p.ChannelOffset[1]; if ((p.ChannelMask & 0x02) != 0) translate.Y += reader.ReadUInt16() * p.ChannelScale[1];
                            translate.Z = p.ChannelOffset[2]; if ((p.ChannelMask & 0x04) != 0) translate.Z += reader.ReadUInt16() * p.ChannelScale[2];

                            var quaternion = new Vector4f();
                            quaternion.X = p.ChannelOffset[3]; if ((p.ChannelMask & 0x08) != 0) quaternion.X += reader.ReadUInt16() * p.ChannelScale[3];
                            quaternion.Y = p.ChannelOffset[4]; if ((p.ChannelMask & 0x10) != 0) quaternion.Y += reader.ReadUInt16() * p.ChannelScale[4];
                            quaternion.Z = p.ChannelOffset[5]; if ((p.ChannelMask & 0x20) != 0) quaternion.Z += reader.ReadUInt16() * p.ChannelScale[5];
                            quaternion.W = p.ChannelOffset[6]; if ((p.ChannelMask & 0x40) != 0) quaternion.W += reader.ReadUInt16() * p.ChannelScale[6];
                            quaternion.Normalize();

                            var scale = new Vector3f();
                            scale.X = p.ChannelOffset[7]; if ((p.ChannelMask & 0x80) != 0) scale.X += reader.ReadUInt16() * p.ChannelScale[7];
                            scale.Y = p.ChannelOffset[8]; if ((p.ChannelMask & 0x100) != 0) scale.Y += reader.ReadUInt16() * p.ChannelScale[8];
                            scale.Z = p.ChannelOffset[9]; if ((p.ChannelMask & 0x200) != 0) scale.Z += reader.ReadUInt16() * p.ChannelScale[9];

                            var trs = new TRS();
                            trs.translation = translate;
                            trs.rotation = quaternion;
                            trs.scaling = scale;
                            TRSData.Add(trs);
                        }
                    }
                }
                else
                {
                    num_frames = 1;
                    for (uint j = 0; j < num_joints; j++)
                    {
                        var translate = new Vector3f();
                        translate.X = Joints[(int)j].Translate.X;
                        translate.Y = Joints[(int)j].Translate.Y;
                        translate.Z = Joints[(int)j].Translate.Z;

                        var quaternion = new Vector4f();
                        quaternion.X = Joints[(int)j].Quaternion.X;
                        quaternion.Y = Joints[(int)j].Quaternion.Y;
                        quaternion.Z = Joints[(int)j].Quaternion.Z;
                        quaternion.W = Joints[(int)j].Quaternion.W;
                        quaternion.Normalize();

                        var scale = new Vector3f();
                        scale.X = Joints[(int)j].Scale.X;
                        scale.Y = Joints[(int)j].Scale.Y;
                        scale.Z = Joints[(int)j].Scale.Z;

                        var trs = new TRS();
                        trs.translation = translate;
                        trs.rotation = quaternion;
                        trs.scaling = scale;
                        TRSData.Add(trs);
                    }
                }

                reader.SeekTo(ofs_bounds);
                for (int i = 0; i < num_frames; i++)
                {
                    var bound = new IQMBounds();
                    bound.BBMins[0] = reader.ReadSingle();
                    bound.BBMins[1] = reader.ReadSingle();
                    bound.BBMins[2] = reader.ReadSingle();
                    bound.BBMaxs[0] = reader.ReadSingle();
                    bound.BBMaxs[1] = reader.ReadSingle();
                    bound.BBMaxs[2] = reader.ReadSingle();
                    bound.XYRadius = reader.ReadSingle();
                    bound.Radius = reader.ReadSingle();
                    Bounds.Add(bound);
                }

                reader.SeekTo(ofs_vertexarrays);
                for (int i = 0; i < num_vertexarrays; i++)
                {
                    var vertexArray = new IQMVertexArray();
                    vertexArray.Type = (IQMVertexArrayType)reader.ReadUInt32();
                    vertexArray.Flags = reader.ReadUInt32();
                    vertexArray.Format = (IQMVertexArrayFormat)reader.ReadUInt32();
                    vertexArray.Size = reader.ReadUInt32();
                    vertexArray.Offset = reader.ReadUInt32();
                    VertexArrays.Add(vertexArray);
                }

                var verts = new IQMVertex[num_vertices];

                foreach (IQMVertexArray vertexArray in VertexArrays)
                {
                    reader.SeekTo(vertexArray.Offset);
                    if (vertexArray.Type == IQMVertexArrayType.IQM_POSITION)
                    {
                        LoadPosition(reader, vertexArray, verts);
                    }
                    else if (vertexArray.Type == IQMVertexArrayType.IQM_TEXCOORD)
                    {
                        LoadTexcoord(reader, vertexArray, verts);
                    }
                    else if (vertexArray.Type == IQMVertexArrayType.IQM_NORMAL)
                    {
                        LoadNormal(reader, vertexArray, verts);
                    }
                    else if (vertexArray.Type == IQMVertexArrayType.IQM_BLENDINDEXES)
                    {
                        LoadBlendIndexes(reader, vertexArray, verts);
                    }
                    else if (vertexArray.Type == IQMVertexArrayType.IQM_BLENDWEIGHTS)
                    {
                        LoadBlendWeights(reader, vertexArray, verts);
                    }
                }

                // Convert vertices to a single frame by applying the bones:

                if (frame >= num_frames)
                    frame = 0;

                List<IQMMatrix> bones = CalculateBones(frame, frame, 0.0f, Joints, baseframe, inversebaseframe, TRSData);
                List<IQMMatrix> normalbones = ToNormalMatrixBones(bones);

                float angleOfsetCos = (float)Math.Cos(-Angle2D.PIHALF);
                float angleOfsetSin = (float)Math.Sin(-Angle2D.PIHALF);

                var worldverts = new WorldVertex[num_vertices];
                for (int i = 0; i < (int)num_vertices; i++)
                {
                    IQMVertex v = verts[i];

                    if (v.boneweightX != 0 || v.boneweightY != 0 || v.boneweightZ != 0 || v.boneweightW != 0)
                    {
                        float totalWeight = v.boneweightX + v.boneweightY + v.boneweightZ + v.boneweightW;
                        var boneWeight = new Vector4f(v.boneweightX / totalWeight, v.boneweightY / totalWeight, v.boneweightZ / totalWeight, v.boneweightW / totalWeight);

                        var pos = new Vector3f(0.0f, 0.0f, 0.0f);
                        var normal = new Vector3f(0.0f, 0.0f, 0.0f);

                        if (v.boneweightX != 0)
                        {
                            pos += bones[v.boneindexX].MultVector(v.pos) * boneWeight.X;
                            normal += normalbones[v.boneindexX].MultVector(v.normal) * boneWeight.X;
                        }

                        if (v.boneweightY != 0)
                        {
                            pos += bones[v.boneindexY].MultVector(v.pos) * boneWeight.Y;
                            normal += normalbones[v.boneindexY].MultVector(v.normal) * boneWeight.Y;
                        }

                        if (v.boneweightZ != 0)
                        {
                            pos += bones[v.boneindexZ].MultVector(v.pos) * boneWeight.Z;
                            normal += normalbones[v.boneindexZ].MultVector(v.normal) * boneWeight.Z;
                        }

                        if (v.boneweightW != 0)
                        {
                            pos += bones[v.boneindexW].MultVector(v.pos) * boneWeight.W;
                            normal += normalbones[v.boneindexW].MultVector(v.normal) * boneWeight.W;
                        }

                        v.pos = pos;
                        v.normal = normal;
                    }

                    // Fix rotation angle
                    float rx = angleOfsetCos * v.pos.X - angleOfsetSin * v.pos.Y;
                    float ry = angleOfsetSin * v.pos.X + angleOfsetCos * v.pos.Y;

                    worldverts[i].x = rx;
                    worldverts[i].y = ry;
                    worldverts[i].z = v.pos.Z;
                    worldverts[i].nx = v.normal.X;
                    worldverts[i].ny = v.normal.Y;
                    worldverts[i].nz = v.normal.Z;
                    worldverts[i].u = v.u;
                    worldverts[i].v = v.v;
                    worldverts[i].c = -1;
                }

                // Create the mesh:

                ModelLoadResult result = new ModelLoadResult();
                foreach (IQMMesh mesh in Meshes)
                {
                    // UDB doesn't support sharing vertices between skins.

                    var skinverts = new WorldVertex[mesh.NumVertices];
                    var skinindexes = new int[mesh.NumTriangles * 3];

                    uint firstVertex = mesh.FirstVertex;
                    uint firstIndex = mesh.FirstTriangle * 3;

                    for (uint i = 0; i < mesh.NumVertices; i++)
                    {
                        skinverts[i] = worldverts[mesh.FirstVertex + i];
                    }

                    for (uint i = 0; i < mesh.NumTriangles * 3; i++)
                    {
                        skinindexes[i] = Indexes[firstIndex + i] - (int)firstVertex;
                    }

                    result.Meshes.Add(new Mesh(General.Map.Graphics, skinverts, skinindexes));
                    result.Skins.Add(mesh.Material);
                }
                return result;
            }
            catch (Exception e)
            {
                ModelLoadResult result = new ModelLoadResult();
                result.Errors = e.Message;
                return result;
            }
        }

        static void LoadPosition(IQMFileReader reader, IQMVertexArray vertexArray, IQMVertex[] verts)
        {
            if (vertexArray.Format == IQMVertexArrayFormat.IQM_FLOAT && vertexArray.Size == 3)
            {
                for (int i = 0; i < verts.Length; i++)
                {
                    verts[i].pos.X = reader.ReadSingle();
                    verts[i].pos.Y = reader.ReadSingle();
                    verts[i].pos.Z = reader.ReadSingle();
                }
            }
            else
            {
                throw new Exception("Unsupported IQM_POSITION vertex format");
            }
        }

        static void LoadTexcoord(IQMFileReader reader, IQMVertexArray vertexArray, IQMVertex[] verts)
        {
            if (vertexArray.Format == IQMVertexArrayFormat.IQM_FLOAT && vertexArray.Size == 2)
            {
                for (int i = 0; i < verts.Length; i++)
                {
                    verts[i].u = reader.ReadSingle();
                    verts[i].v = reader.ReadSingle();
                }
            }
            else
            {
                throw new Exception("Unsupported IQM_TEXCOORD vertex format");
            }
        }

        static void LoadNormal(IQMFileReader reader, IQMVertexArray vertexArray, IQMVertex[] verts)
        {
            if (vertexArray.Format == IQMVertexArrayFormat.IQM_FLOAT && vertexArray.Size == 3)
            {
                for (int i = 0; i < verts.Length; i++)
                {
                    verts[i].normal.X = reader.ReadSingle();
                    verts[i].normal.Y = reader.ReadSingle();
                    verts[i].normal.Z = reader.ReadSingle();
                }
            }
            else
            {
                throw new Exception("Unsupported IQM_NORMAL vertex format");
            }
        }

        static void LoadBlendIndexes(IQMFileReader reader, IQMVertexArray vertexArray, IQMVertex[] verts)
        {
            if (vertexArray.Format == IQMVertexArrayFormat.IQM_UBYTE && vertexArray.Size == 4)
            {
                for (int i = 0; i < verts.Length; i++)
                {
                    verts[i].boneindexX = reader.ReadByte();
                    verts[i].boneindexY = reader.ReadByte();
                    verts[i].boneindexZ = reader.ReadByte();
                    verts[i].boneindexW = reader.ReadByte();
                }
            }
            else if (vertexArray.Format == IQMVertexArrayFormat.IQM_INT && vertexArray.Size == 4)
            {
                for (int i = 0; i < verts.Length; i++)
                {
                    verts[i].boneindexX = (byte)reader.ReadInt32();
                    verts[i].boneindexY = (byte)reader.ReadInt32();
                    verts[i].boneindexZ = (byte)reader.ReadInt32();
                    verts[i].boneindexW = (byte)reader.ReadInt32();
                }
            }
            else
            {
                throw new Exception("Unsupported IQM_BLENDINDEXES vertex format");
            }
        }

        static void LoadBlendWeights(IQMFileReader reader, IQMVertexArray vertexArray, IQMVertex[] verts)
        {
            if (vertexArray.Format == IQMVertexArrayFormat.IQM_UBYTE && vertexArray.Size == 4)
            {
                for (int i = 0; i < verts.Length; i++)
                {
                    verts[i].boneweightX = reader.ReadByte();
                    verts[i].boneweightY = reader.ReadByte();
                    verts[i].boneweightZ = reader.ReadByte();
                    verts[i].boneweightW = reader.ReadByte();
                }
            }
            else if (vertexArray.Format == IQMVertexArrayFormat.IQM_FLOAT && vertexArray.Size == 4)
            {
                for (int i = 0; i < verts.Length; i++)
                {
                    verts[i].boneweightX = (byte)Clamp(reader.ReadSingle() * 255.0f, 0.0f, 255.0f);
                    verts[i].boneweightY = (byte)Clamp(reader.ReadSingle() * 255.0f, 0.0f, 255.0f);
                    verts[i].boneweightZ = (byte)Clamp(reader.ReadSingle() * 255.0f, 0.0f, 255.0f);
                    verts[i].boneweightW = (byte)Clamp(reader.ReadSingle() * 255.0f, 0.0f, 255.0f);
                }
            }
            else
            {
                throw new Exception("Unsupported IQM_BLENDWEIGHTS vertex format");
            }
        }

        static List<IQMMatrix> ToNormalMatrixBones(List<IQMMatrix> bones)
        {
            var normalbones = new List<IQMMatrix>();
            foreach (IQMMatrix m in bones)
            {
                IQMMatrix nm = new IQMMatrix();
                nm.LoadMatrix(m);
                for (int i = 0; i < 4; i++)
                {
                    nm.Matrix[i + 3 * 4] = 0;
                    nm.Matrix[i * 4 + 3] = 0;
                }
                normalbones.Add(nm);
            }
            return normalbones;
        }

        static List<IQMMatrix> CalculateBones(int frame1, int frame2, float t, List<IQMJoint> Joints, List<IQMMatrix> baseframe, List<IQMMatrix> inversebaseframe, List<TRS> animationFrames)
        {
            var bones = new List<IQMMatrix>();
            if (Joints.Count != 0)
            {
                int numbones = Joints.Count;

                frame1 = Clamp(frame1, 0, (animationFrames.Count - 1) / numbones);
                frame2 = Clamp(frame2, 0, (animationFrames.Count - 1) / numbones);

                int offset1 = frame1 * numbones;
                int offset2 = frame2 * numbones;
                float invt = 1.0f - t;

                for (int i = 0; i < numbones; i++)
                {
                    TRS from = animationFrames[offset1 + i];
                    TRS to = animationFrames[offset2 + i];

                    var bone = new TRS();
                    bone.translation = from.translation * invt + to.translation * t;
                    bone.rotation = from.rotation * invt;
                    if (Vector4f.Dot(bone.rotation, to.rotation * t) < 0)
                    {
                        bone.rotation.X *= -1;
                        bone.rotation.Y *= -1;
                        bone.rotation.Z *= -1;
                        bone.rotation.W *= -1;
                    }
                    bone.rotation += to.rotation * t;
                    bone.rotation.Normalize();
                    bone.scaling = from.scaling * invt + to.scaling * t;

                    var m = new IQMMatrix();
                    m.LoadIdentity();
                    m.Translate(bone.translation.X, bone.translation.Y, bone.translation.Z);
                    m.MultQuaternion(bone.rotation);
                    m.Scale(bone.scaling.X, bone.scaling.Y, bone.scaling.Z);

                    if (Joints[i].Parent >= 0)
                    {
                        var result = new IQMMatrix();
                        result.LoadMatrix(bones[Joints[i].Parent]);
                        result.MultMatrix(baseframe[Joints[i].Parent]);
                        result.MultMatrix(m);
                        result.MultMatrix(inversebaseframe[i]);
                        bones.Add(result);
                    }
                    else
                    {
                        var result = new IQMMatrix();
                        result.LoadMatrix(m);
                        result.MultMatrix(inversebaseframe[i]);
                        bones.Add(result);
                    }
                }
            }
            return bones;
        }

        static float Clamp(float v, float minval, float maxval)
        {
            return Math.Max(Math.Min(v, maxval), minval);
        }

        static int Clamp(int v, int minval, int maxval)
        {
            return Math.Max(Math.Min(v, maxval), minval);
        }
    }

    class IQMMatrix
    {
        public float[] Matrix = new float[16];

        public void LoadIdentity()
        {
            // fill matrix with 0s
            for (int i = 0; i < 16; ++i)
                Matrix[i] = 0.0f;

            // fill diagonal with 1s
            for (int i = 0; i < 4; ++i)
                Matrix[i + i * 4] = 1.0f;
        }

        public void LoadMatrix(IQMMatrix m)
        {
            for (int i = 0; i < 16; i++)
                Matrix[i] = m.Matrix[i];
        }

        public void Translate(float x, float y, float z)
        {
            Matrix[12] = Matrix[0] * x + Matrix[4] * y + Matrix[8] * z + Matrix[12];
            Matrix[13] = Matrix[1] * x + Matrix[5] * y + Matrix[9] * z + Matrix[13];
            Matrix[14] = Matrix[2] * x + Matrix[6] * y + Matrix[10] * z + Matrix[14];
        }

        public void Scale(float x, float y, float z)
        {
            Matrix[0] *= x; Matrix[1] *= x; Matrix[2] *= x; Matrix[3] *= x;
            Matrix[4] *= y; Matrix[5] *= y; Matrix[6] *= y; Matrix[7] *= y;
            Matrix[8] *= z; Matrix[9] *= z; Matrix[10] *= z; Matrix[11] *= z;
        }

        public void MultMatrix(IQMMatrix m)
        {
            MultMatrix(m.Matrix);
        }

        void MultMatrix(float[] aMatrix)
        {
            var res = new float[16];
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < 4; ++j)
                {
                    res[j * 4 + i] = 0.0f;
                    for (int k = 0; k < 4; ++k)
                    {
                        res[j * 4 + i] += Matrix[k * 4 + i] * aMatrix[j * 4 + k];
                    }
                }
            }
            Matrix = res;
        }

        public void MultQuaternion(Vector4f q)
        {
            var m = new float[16];
            m[0 * 4 + 0] = 1.0f - 2.0f * q.Y * q.Y - 2.0f * q.Z * q.Z;
            m[1 * 4 + 0] = 2.0f * q.X * q.Y - 2.0f * q.W * q.Z;
            m[2 * 4 + 0] = 2.0f * q.X * q.Z + 2.0f * q.W * q.Y;
            m[0 * 4 + 1] = 2.0f * q.X * q.Y + 2.0f * q.W * q.Z;
            m[1 * 4 + 1] = 1.0f - 2.0f * q.X * q.X - 2.0f * q.Z * q.Z;
            m[2 * 4 + 1] = 2.0f * q.Y * q.Z - 2.0f * q.W * q.X;
            m[0 * 4 + 2] = 2.0f * q.X * q.Z - 2.0f * q.W * q.Y;
            m[1 * 4 + 2] = 2.0f * q.Y * q.Z + 2.0f * q.W * q.X;
            m[2 * 4 + 2] = 1.0f - 2.0f * q.X * q.X - 2.0f * q.Y * q.Y;
            m[3 * 4 + 3] = 1.0f;
            MultMatrix(m);
        }

        public Vector3f MultVector(Vector3f v)
        {
            var result = MultVector(new Vector4f(v, 1.0f));
            return new Vector3f(result.X, result.Y, result.Z);
        }

        public Vector4f MultVector(Vector4f v)
        {
            var result = new Vector4f();
            result.X = Matrix[0 * 4 + 0] * v.X + Matrix[1 * 4 + 0] * v.Y + Matrix[2 * 4 + 0] * v.Z + Matrix[3 * 4 + 0] * v.W;
            result.Y = Matrix[0 * 4 + 1] * v.X + Matrix[1 * 4 + 1] * v.Y + Matrix[2 * 4 + 1] * v.Z + Matrix[3 * 4 + 1] * v.W;
            result.Z = Matrix[0 * 4 + 2] * v.X + Matrix[1 * 4 + 2] * v.Y + Matrix[2 * 4 + 2] * v.Z + Matrix[3 * 4 + 2] * v.W;
            result.W = Matrix[0 * 4 + 3] * v.X + Matrix[1 * 4 + 3] * v.Y + Matrix[2 * 4 + 3] * v.Z + Matrix[3 * 4 + 3] * v.W;
            return result;
        }

        public IQMMatrix InverseMatrix()
        {
            var result = new IQMMatrix();

            // Calculate mat4 determinant
            float det = mat4Determinant(Matrix);

            // Inverse unknown when determinant is close to zero
            if (Math.Abs(det) < 1e-15)
            {
                for (int i = 0; i < 16; i++)
                    result.Matrix[i] = 0.0f;
            }
            else
            {
                mat4Adjoint(Matrix, result.Matrix);

                float invDet = 1.0f / det;
                for (int i = 0; i < 16; i++)
                {
                    result.Matrix[i] = result.Matrix[i] * invDet;
                }
            }

            return result;
        }

        static float mat3Determinant(float[] mMat3x3)
        {
            return mMat3x3[0] * (mMat3x3[4] * mMat3x3[8] - mMat3x3[5] * mMat3x3[7]) +
                mMat3x3[1] * (mMat3x3[5] * mMat3x3[6] - mMat3x3[8] * mMat3x3[3]) +
                mMat3x3[2] * (mMat3x3[3] * mMat3x3[7] - mMat3x3[4] * mMat3x3[6]);
        }

        static float mat4Determinant(float[] matrix)
        {
            var mMat3x3_a = new float[]
            {
                matrix[1 * 4 + 1], matrix[2 * 4 + 1], matrix[3 * 4 + 1],
                matrix[1 * 4 + 2], matrix[2 * 4 + 2], matrix[3 * 4 + 2],
                matrix[1 * 4 + 3], matrix[2 * 4 + 3], matrix[3 * 4 + 3]
            };

            var mMat3x3_b = new float[]
            {
                matrix[1 * 4 + 0], matrix[2 * 4 + 0], matrix[3 * 4 + 0],
                matrix[1 * 4 + 2], matrix[2 * 4 + 2], matrix[3 * 4 + 2],
                matrix[1 * 4 + 3], matrix[2 * 4 + 3], matrix[3 * 4 + 3]
            };

            var mMat3x3_c = new float[]
            {
                matrix[1 * 4 + 0], matrix[2 * 4 + 0], matrix[3 * 4 + 0],
                matrix[1 * 4 + 1], matrix[2 * 4 + 1], matrix[3 * 4 + 1],
                matrix[1 * 4 + 3], matrix[2 * 4 + 3], matrix[3 * 4 + 3]
            };

            var mMat3x3_d = new float[]
            {
                matrix[1 * 4 + 0], matrix[2 * 4 + 0], matrix[3 * 4 + 0],
                matrix[1 * 4 + 1], matrix[2 * 4 + 1], matrix[3 * 4 + 1],
                matrix[1 * 4 + 2], matrix[2 * 4 + 2], matrix[3 * 4 + 2]
            };

            float a, b, c, d;
            float value;

            a = mat3Determinant(mMat3x3_a);
            b = mat3Determinant(mMat3x3_b);
            c = mat3Determinant(mMat3x3_c);
            d = mat3Determinant(mMat3x3_d);

            value = matrix[0 * 4 + 0] * a;
            value -= matrix[0 * 4 + 1] * b;
            value += matrix[0 * 4 + 2] * c;
            value -= matrix[0 * 4 + 3] * d;

            return value;
        }

        static void mat4Adjoint(float[] matrix, float[] result)
        {
            var mMat3x3_a = new float[]
            {
                matrix[1 * 4 + 1], matrix[2 * 4 + 1], matrix[3 * 4 + 1],
                matrix[1 * 4 + 2], matrix[2 * 4 + 2], matrix[3 * 4 + 2],
                matrix[1 * 4 + 3], matrix[2 * 4 + 3], matrix[3 * 4 + 3]
            };

            var mMat3x3_b = new float[]
            {
                matrix[1 * 4 + 0], matrix[2 * 4 + 0], matrix[3 * 4 + 0],
                matrix[1 * 4 + 2], matrix[2 * 4 + 2], matrix[3 * 4 + 2],
                matrix[1 * 4 + 3], matrix[2 * 4 + 3], matrix[3 * 4 + 3]
            };

            var mMat3x3_c = new float[]
            {
                matrix[1 * 4 + 0], matrix[2 * 4 + 0], matrix[3 * 4 + 0],
                matrix[1 * 4 + 1], matrix[2 * 4 + 1], matrix[3 * 4 + 1],
                matrix[1 * 4 + 3], matrix[2 * 4 + 3], matrix[3 * 4 + 3]
            };

            var mMat3x3_d = new float[]
            {
                matrix[1 * 4 + 0], matrix[2 * 4 + 0], matrix[3 * 4 + 0],
                matrix[1 * 4 + 1], matrix[2 * 4 + 1], matrix[3 * 4 + 1],
                matrix[1 * 4 + 2], matrix[2 * 4 + 2], matrix[3 * 4 + 2]
            };

            var mMat3x3_e = new float[]
            {
                matrix[0 * 4 + 1], matrix[2 * 4 + 1], matrix[3 * 4 + 1],
                matrix[0 * 4 + 2], matrix[2 * 4 + 2], matrix[3 * 4 + 2],
                matrix[0 * 4 + 3], matrix[2 * 4 + 3], matrix[3 * 4 + 3]
            };

            var mMat3x3_f = new float[]
            {
                matrix[0 * 4 + 0], matrix[2 * 4 + 0], matrix[3 * 4 + 0],
                matrix[0 * 4 + 2], matrix[2 * 4 + 2], matrix[3 * 4 + 2],
                matrix[0 * 4 + 3], matrix[2 * 4 + 3], matrix[3 * 4 + 3]
            };

            var mMat3x3_g = new float[]
            {
                matrix[0 * 4 + 0], matrix[2 * 4 + 0], matrix[3 * 4 + 0],
                matrix[0 * 4 + 1], matrix[2 * 4 + 1], matrix[3 * 4 + 1],
                matrix[0 * 4 + 3], matrix[2 * 4 + 3], matrix[3 * 4 + 3]
            };

            var mMat3x3_h = new float[]
            {
                matrix[0 * 4 + 0], matrix[2 * 4 + 0], matrix[3 * 4 + 0],
                matrix[0 * 4 + 1], matrix[2 * 4 + 1], matrix[3 * 4 + 1],
                matrix[0 * 4 + 2], matrix[2 * 4 + 2], matrix[3 * 4 + 2]
            };

            var mMat3x3_i = new float[]
            {
                matrix[0 * 4 + 1], matrix[1 * 4 + 1], matrix[3 * 4 + 1],
                matrix[0 * 4 + 2], matrix[1 * 4 + 2], matrix[3 * 4 + 2],
                matrix[0 * 4 + 3], matrix[1 * 4 + 3], matrix[3 * 4 + 3]
            };

            var mMat3x3_j = new float[]
            {
                matrix[0 * 4 + 0], matrix[1 * 4 + 0], matrix[3 * 4 + 0],
                matrix[0 * 4 + 2], matrix[1 * 4 + 2], matrix[3 * 4 + 2],
                matrix[0 * 4 + 3], matrix[1 * 4 + 3], matrix[3 * 4 + 3]
            };

            var mMat3x3_k = new float[]
            {
                matrix[0 * 4 + 0], matrix[1 * 4 + 0], matrix[3 * 4 + 0],
                matrix[0 * 4 + 1], matrix[1 * 4 + 1], matrix[3 * 4 + 1],
                matrix[0 * 4 + 3], matrix[1 * 4 + 3], matrix[3 * 4 + 3]
            };

            var mMat3x3_l = new float[]
            {
                matrix[0 * 4 + 0], matrix[1 * 4 + 0], matrix[3 * 4 + 0],
                matrix[0 * 4 + 1], matrix[1 * 4 + 1], matrix[3 * 4 + 1],
                matrix[0 * 4 + 2], matrix[1 * 4 + 2], matrix[3 * 4 + 2]
            };

            var mMat3x3_m = new float[]
            {
                matrix[0 * 4 + 1], matrix[1 * 4 + 1], matrix[2 * 4 + 1],
                matrix[0 * 4 + 2], matrix[1 * 4 + 2], matrix[2 * 4 + 2],
                matrix[0 * 4 + 3], matrix[1 * 4 + 3], matrix[2 * 4 + 3]
            };

            var mMat3x3_n = new float[]
            {
                matrix[0 * 4 + 0], matrix[1 * 4 + 0], matrix[2 * 4 + 0],
                matrix[0 * 4 + 2], matrix[1 * 4 + 2], matrix[2 * 4 + 2],
                matrix[0 * 4 + 3], matrix[1 * 4 + 3], matrix[2 * 4 + 3]
            };

            var mMat3x3_o = new float[]
            {
                matrix[0 * 4 + 0], matrix[1 * 4 + 0], matrix[2 * 4 + 0],
                matrix[0 * 4 + 1], matrix[1 * 4 + 1], matrix[2 * 4 + 1],
                matrix[0 * 4 + 3], matrix[1 * 4 + 3], matrix[2 * 4 + 3]
            };

            var mMat3x3_p = new float[]
            {
                matrix[0 * 4 + 0], matrix[1 * 4 + 0], matrix[2 * 4 + 0],
                matrix[0 * 4 + 1], matrix[1 * 4 + 1], matrix[2 * 4 + 1],
                matrix[0 * 4 + 2], matrix[1 * 4 + 2], matrix[2 * 4 + 2]
            };

            result[0 * 4 + 0] = mat3Determinant(mMat3x3_a);
            result[1 * 4 + 0] = -mat3Determinant(mMat3x3_b);
            result[2 * 4 + 0] = mat3Determinant(mMat3x3_c);
            result[3 * 4 + 0] = -mat3Determinant(mMat3x3_d);
            result[0 * 4 + 1] = -mat3Determinant(mMat3x3_e);
            result[1 * 4 + 1] = mat3Determinant(mMat3x3_f);
            result[2 * 4 + 1] = -mat3Determinant(mMat3x3_g);
            result[3 * 4 + 1] = mat3Determinant(mMat3x3_h);
            result[0 * 4 + 2] = mat3Determinant(mMat3x3_i);
            result[1 * 4 + 2] = -mat3Determinant(mMat3x3_j);
            result[2 * 4 + 2] = mat3Determinant(mMat3x3_k);
            result[3 * 4 + 2] = -mat3Determinant(mMat3x3_l);
            result[0 * 4 + 3] = -mat3Determinant(mMat3x3_m);
            result[1 * 4 + 3] = mat3Determinant(mMat3x3_n);
            result[2 * 4 + 3] = -mat3Determinant(mMat3x3_o);
            result[3 * 4 + 3] = mat3Determinant(mMat3x3_p);
        }
    }

    class TRS
    {
        public Vector3f translation = new Vector3f(0, 0, 0);
        public Vector4f rotation = new Vector4f(0, 0, 0, 1);
        public Vector3f scaling = new Vector3f(0, 0, 0);
    }

    struct IQMVertex
    {
        public Vector3f pos, normal;
        public float u, v;
        public byte boneindexX, boneindexY, boneindexZ, boneindexW;
        public byte boneweightX, boneweightY, boneweightZ, boneweightW;
    }

    class IQMMesh
    {
        public string Name;
        public string Material;
        public uint FirstVertex;
        public uint NumVertices;
        public uint FirstTriangle;
        public uint NumTriangles;
    };

    enum IQMVertexArrayType
    {
        IQM_POSITION = 0,     // float, 3
        IQM_TEXCOORD = 1,     // float, 2
        IQM_NORMAL = 2,       // float, 3
        IQM_TANGENT = 3,      // float, 4
        IQM_BLENDINDEXES = 4, // ubyte, 4
        IQM_BLENDWEIGHTS = 5, // ubyte, 4
        IQM_COLOR = 6,        // ubyte, 4
        IQM_CUSTOM = 0x10
    };

    enum IQMVertexArrayFormat
    {
        IQM_BYTE = 0,
        IQM_UBYTE = 1,
        IQM_SHORT = 2,
        IQM_USHORT = 3,
        IQM_INT = 4,
        IQM_UINT = 5,
        IQM_HALF = 6,
        IQM_FLOAT = 7,
        IQM_DOUBLE = 8,
    };

    class IQMVertexArray
    {
        public IQMVertexArrayType Type;
        public uint Flags;
        public IQMVertexArrayFormat Format;
        public uint Size;
        public uint Offset;
    };

    class IQMJoint
    {
        public string Name;
        public int Parent; // parent < 0 means this is a root bone
        public Vector3f Translate;
        public Vector4f Quaternion;
        public Vector3f Scale;
    };

    class IQMPose
    {
        public int Parent; // parent < 0 means this is a root bone
        public uint ChannelMask; // mask of which 10 channels are present for this joint pose
        public float[] ChannelOffset = new float[10];
        public float[] ChannelScale = new float[10];
        // channels 0..2 are translation <Tx, Ty, Tz> and channels 3..6 are quaternion rotation <Qx, Qy, Qz, Qw>
        // rotation is in relative/parent local space
        // channels 7..9 are scale <Sx, Sy, Sz>
        // output = (input*scale)*rotation + translation
    };

    class IQMAnim
    {
        public string Name;
        public uint FirstFrame;
        public uint NumFrames;
        public float Framerate;
        public bool Loop;
    };

    class IQMBounds
    {
        public float[] BBMins = new float[3];
        public float[] BBMaxs = new float[3];
        public float XYRadius;
        public float Radius;
    };

    class IQMFileReader : BinaryReader
    {
        public IQMFileReader(Stream s) : base(s)
        {
        }

        public string ReadName(byte[] textBuffer)
        {
            uint nameOffset = ReadUInt32();
            if (nameOffset >= textBuffer.Length)
                throw new Exception("Name offset out of bounds");

            for (uint i = nameOffset; i < (uint)textBuffer.Length; i++)
            {
                if (textBuffer[i] == 0)
                {
                    return Encoding.ASCII.GetString(textBuffer, (int)nameOffset, (int)(i - nameOffset));
                }
            }

            throw new Exception("Name not null terminated");
        }

        public void SeekTo(uint newPos)
        {
            BaseStream.Seek(newPos, SeekOrigin.Begin);
        }
    }
}
