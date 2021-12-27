/*
**  BuilderNative Renderer
**  Copyright (c) 2019 Magnus Norddahl
**
**  This software is provided 'as-is', without any express or implied
**  warranty.  In no event will the authors be held liable for any damages
**  arising from the use of this software.
**
**  Permission is granted to anyone to use this software for any purpose,
**  including commercial applications, and to alter it and redistribute it
**  freely, subject to the following restrictions:
**
**  1. The origin of this software must not be misrepresented; you must not
**     claim that you wrote the original software. If you use this software
**     in a product, an acknowledgment in the product documentation would be
**     appreciated but is not required.
**  2. Altered source versions must be plainly marked as such, and must not be
**     misrepresented as being the original software.
**  3. This notice may not be removed or altered from any source distribution.
*/

#pragma once

#include <string>
#include <mutex>

enum class CubeMapFace : int { PositiveX, PositiveY, PositiveZ, NegativeX, NegativeY, NegativeZ };
enum class VertexFormat : int32_t { Flat, World };
enum class DeclarationUsage : int32_t { Position, Color, TextureCoordinate, Normal };

enum class Cull : int { None, Clockwise };
enum class Blend : int { InverseSourceAlpha, SourceAlpha, One };
enum class BlendOperation : int { Add, ReverseSubtract };
enum class FillMode : int { Solid, Wireframe };
enum class TextureAddress : int { Wrap, Clamp };
enum class ShaderFlags : int { None, Debug };
enum class PrimitiveType : int { LineList, TriangleList, TriangleStrip };
enum class TextureFilter : int { Nearest, Linear };
enum class MipmapFilter : int { None, Nearest, Linear };
enum class UniformType : int { Vec4f, Vec3f, Vec2f, Float, Mat4, Vec4i, Vec3i, Vec2i, Int, Vec4fArray, Vec3fArray, Vec2fArray };

enum class PixelFormat : int
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
	D24_S8,
	A2Bgr10,
	A2Rgb10_snorm
};

typedef int UniformName;
typedef int ShaderName;

class VertexBuffer;
class IndexBuffer;
class Texture;

class RenderDevice
{
public:
	virtual ~RenderDevice() = default;

	virtual void DeclareUniform(UniformName name, const char* glslname, UniformType type) = 0;
	virtual void DeclareShader(ShaderName index, const char* name, const char* vertexshader, const char* fragmentshader) = 0;
	virtual void SetShader(ShaderName name) = 0;
	virtual void SetUniform(UniformName name, const void* values, int count, int bytesize) = 0;
	virtual void SetVertexBuffer(VertexBuffer* buffer) = 0;
	virtual void SetIndexBuffer(IndexBuffer* buffer) = 0;
	virtual void SetAlphaBlendEnable(bool value) = 0;
	virtual void SetAlphaTestEnable(bool value) = 0;
	virtual void SetCullMode(Cull mode) = 0;
	virtual void SetBlendOperation(BlendOperation op) = 0;
	virtual void SetSourceBlend(Blend blend) = 0;
	virtual void SetDestinationBlend(Blend blend) = 0;
	virtual void SetFillMode(FillMode mode) = 0;
	virtual void SetMultisampleAntialias(bool value) = 0;
	virtual void SetZEnable(bool value) = 0;
	virtual void SetZWriteEnable(bool value) = 0;
	virtual void SetTexture(int unit, Texture* texture) = 0;
	virtual void SetSamplerFilter(int unit, TextureFilter minfilter, TextureFilter magfilter, MipmapFilter mipfilter, float maxanisotropy) = 0;
	virtual void SetSamplerState(int unit, TextureAddress address) = 0;
	virtual bool Draw(PrimitiveType type, int startIndex, int primitiveCount) = 0;
	virtual bool DrawIndexed(PrimitiveType type, int startIndex, int primitiveCount) = 0;
	virtual bool DrawData(PrimitiveType type, int startIndex, int primitiveCount, const void* data) = 0;
	virtual bool StartRendering(bool clear, int backcolor, Texture* target, bool usedepthbuffer) = 0;
	virtual bool FinishRendering() = 0;
	virtual bool Present() = 0;
	virtual bool ClearTexture(int backcolor, Texture* texture) = 0;
	virtual bool CopyTexture(Texture* dst, CubeMapFace face) = 0;
	virtual bool SetVertexBufferData(VertexBuffer* buffer, void* data, int64_t size, VertexFormat format) = 0;
	virtual bool SetVertexBufferSubdata(VertexBuffer* buffer, int64_t destOffset, void* data, int64_t size) = 0;
	virtual bool SetIndexBufferData(IndexBuffer* buffer, void* data, int64_t size) = 0;
	virtual bool SetPixels(Texture* texture, const void* data) = 0;
	virtual bool SetCubePixels(Texture* texture, CubeMapFace face, const void* data) = 0;
	virtual void* MapPBO(Texture* texture) = 0;
	virtual bool UnmapPBO(Texture* texture) = 0;
};

class VertexBuffer
{
public:
	virtual ~VertexBuffer() = default;

	static const int FlatStride = 24;
	static const int WorldStride = 36;
};

class IndexBuffer
{
public:
	virtual ~IndexBuffer() = default;
};

class Texture
{
public:
	virtual ~Texture() = default;
	virtual void Set2DImage(int width, int height, PixelFormat format) = 0;
	virtual void SetCubeImage(int size, PixelFormat format) = 0;
};

class Backend
{
public:
	virtual ~Backend() = default;

	static Backend* Get();

	virtual RenderDevice* NewRenderDevice(void* disp, void* window) = 0;
	virtual void DeleteRenderDevice(RenderDevice* device) = 0;

	virtual VertexBuffer* NewVertexBuffer() = 0;
	virtual void DeleteVertexBuffer(VertexBuffer* buffer) = 0;

	virtual IndexBuffer* NewIndexBuffer() = 0;
	virtual void DeleteIndexBuffer(IndexBuffer* buffer) = 0;

	virtual Texture* NewTexture() = 0;
	virtual void DeleteTexture(Texture* texture) = 0;
};

void SetError(const char* fmt, ...);
const char* GetError();
