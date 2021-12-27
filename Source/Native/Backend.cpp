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

#include "Precomp.h"
#include "Backend.h"
#include "OpenGL/GLBackend.h"

namespace
{
	std::string mLastError;
	std::string mReturnError;
	char mSetErrorBuffer[4096];
}

void SetError(const char* fmt, ...)
{
	va_list va;
	va_start(va, fmt);
	mSetErrorBuffer[0] = 0;
#ifdef WIN32
	_vsnprintf(mSetErrorBuffer, sizeof(mSetErrorBuffer) - 1, fmt, va);
#else
	vsnprintf(mSetErrorBuffer, sizeof(mSetErrorBuffer) - 1, fmt, va);
#endif
	va_end(va);
	mSetErrorBuffer[sizeof(mSetErrorBuffer) - 1] = 0;
	mLastError = mSetErrorBuffer;
}

const char* GetError()
{
	mReturnError.swap(mLastError);
	mLastError.clear();
	return mReturnError.c_str();
}

/////////////////////////////////////////////////////////////////////////////

Backend* Backend::Get()
{
	static std::unique_ptr<Backend> backend;
	if (!backend)
		backend.reset(new GLBackend());
	return backend.get();
}

/////////////////////////////////////////////////////////////////////////////

extern "C"
{
	RenderDevice* RenderDevice_New(void* disp, void* window)
	{
		return Backend::Get()->NewRenderDevice(disp, window);
	}

	void RenderDevice_Delete(RenderDevice* device)
	{
		Backend::Get()->DeleteRenderDevice(device);
	}

	void BuilderNative_GetError(char *out, int len)
	{
        std::string result = mLastError;
        result = result.substr(0, len);
        std::copy(result.begin(), result.end(), out);
        out[std::min(len - 1, (int)result.size())] = 0;
	}

	void RenderDevice_DeclareUniform(RenderDevice* device, UniformName name, const char* variablename, UniformType type)
	{
		device->DeclareUniform(name, variablename, type);
	}

	void RenderDevice_DeclareShader(RenderDevice* device, ShaderName index, const char* name, const char* vertexshader, const char* fragmentshader)
	{
		device->DeclareShader(index, name, vertexshader, fragmentshader);
	}

	void RenderDevice_SetShader(RenderDevice* device, ShaderName name)
	{
		device->SetShader(name);
	}

	void RenderDevice_SetUniform(RenderDevice* device, UniformName name, const void* values, int count, int bytesize)
	{
		device->SetUniform(name, values, count, bytesize);
	}

	void RenderDevice_SetVertexBuffer(RenderDevice* device, VertexBuffer* buffer)
	{
		device->SetVertexBuffer(buffer);
	}

	void RenderDevice_SetIndexBuffer(RenderDevice* device, IndexBuffer* buffer)
	{
		device->SetIndexBuffer(buffer);
	}

	void RenderDevice_SetAlphaBlendEnable(RenderDevice* device, bool value)
	{
		device->SetAlphaBlendEnable(value);
	}

	void RenderDevice_SetAlphaTestEnable(RenderDevice* device, bool value)
	{
		device->SetAlphaTestEnable(value);
	}

	void RenderDevice_SetCullMode(RenderDevice* device, Cull mode)
	{
		device->SetCullMode(mode);
	}

	void RenderDevice_SetBlendOperation(RenderDevice* device, BlendOperation op)
	{
		device->SetBlendOperation(op);
	}

	void RenderDevice_SetSourceBlend(RenderDevice* device, Blend blend)
	{
		device->SetSourceBlend(blend);
	}

	void RenderDevice_SetDestinationBlend(RenderDevice* device, Blend blend)
	{
		device->SetDestinationBlend(blend);
	}

	void RenderDevice_SetFillMode(RenderDevice* device, FillMode mode)
	{
		device->SetFillMode(mode);
	}

	void RenderDevice_SetMultisampleAntialias(RenderDevice* device, bool value)
	{
		device->SetMultisampleAntialias(value);
	}

	void RenderDevice_SetZEnable(RenderDevice* device, bool value)
	{
		device->SetZEnable(value);
	}

	void RenderDevice_SetZWriteEnable(RenderDevice* device, bool value)
	{
		device->SetZWriteEnable(value);
	}

	void RenderDevice_SetTexture(RenderDevice* device, int unit, Texture* texture)
	{
		device->SetTexture(unit, texture);
	}

	void RenderDevice_SetSamplerFilter(RenderDevice* device, int unit, TextureFilter minfilter, TextureFilter magfilter, MipmapFilter mipfilter, float maxanisotropy)
	{
		device->SetSamplerFilter(unit, minfilter, magfilter, mipfilter, maxanisotropy);
	}

	void RenderDevice_SetSamplerState(RenderDevice* device, int unit, TextureAddress address)
	{
		device->SetSamplerState(unit, address);
	}

	bool RenderDevice_Draw(RenderDevice* device, PrimitiveType type, int startIndex, int primitiveCount)
	{
		return device->Draw(type, startIndex, primitiveCount);
	}

	bool RenderDevice_DrawIndexed(RenderDevice* device, PrimitiveType type, int startIndex, int primitiveCount)
	{
		return device->DrawIndexed(type, startIndex, primitiveCount);
	}

	bool RenderDevice_DrawData(RenderDevice* device, PrimitiveType type, int startIndex, int primitiveCount, const void* data)
	{
		return device->DrawData(type, startIndex, primitiveCount, data);
	}

	bool RenderDevice_StartRendering(RenderDevice* device, bool clear, int backcolor, Texture* target, bool usedepthbuffer)
	{
		return device->StartRendering(clear, backcolor, target, usedepthbuffer);
	}

	bool RenderDevice_FinishRendering(RenderDevice* device)
	{
		return device->FinishRendering();
	}

	bool RenderDevice_Present(RenderDevice* device)
	{
		return device->Present();
	}

	bool RenderDevice_ClearTexture(RenderDevice* device, int backcolor, Texture* texture)
	{
		return device->ClearTexture(backcolor, texture);
	}

	bool RenderDevice_CopyTexture(RenderDevice* device, Texture* dst, CubeMapFace face)
	{
		return device->CopyTexture(dst, face);
	}

	bool RenderDevice_SetVertexBufferData(RenderDevice* device, VertexBuffer* buffer, void* data, int64_t size, VertexFormat format)
	{
		return device->SetVertexBufferData(buffer, data, size, format);
	}

	bool RenderDevice_SetVertexBufferSubdata(RenderDevice* device, VertexBuffer* buffer, int64_t destOffset, void* data, int64_t size)
	{
		return device->SetVertexBufferSubdata(buffer, destOffset, data, size);
	}

	bool RenderDevice_SetIndexBufferData(RenderDevice* device, IndexBuffer* buffer, void* data, int64_t size)
	{
		return device->SetIndexBufferData(buffer, data, size);
	}

	bool RenderDevice_SetPixels(RenderDevice* device, Texture* texture, const void* data)
	{
		return device->SetPixels(texture, data);
	}

	bool RenderDevice_SetCubePixels(RenderDevice* device, Texture* texture, CubeMapFace face, const void* data)
	{
		return device->SetCubePixels(texture, face, data);
	}

	void* RenderDevice_MapPBO(RenderDevice* device, Texture* texture)
	{
		return device->MapPBO(texture);
	}

	bool RenderDevice_UnmapPBO(RenderDevice* device, Texture* texture)
	{
		return device->UnmapPBO(texture);
	}

	////////////////////////////////////////////////////////////////////////////

	IndexBuffer* IndexBuffer_New()
	{
		return Backend::Get()->NewIndexBuffer();
	}

	void IndexBuffer_Delete(IndexBuffer* buffer)
	{
		Backend::Get()->DeleteIndexBuffer(buffer);
	}

	////////////////////////////////////////////////////////////////////////////

	VertexBuffer* VertexBuffer_New()
	{
		return Backend::Get()->NewVertexBuffer();
	}

	void VertexBuffer_Delete(VertexBuffer* buffer)
	{
		Backend::Get()->DeleteVertexBuffer(buffer);
	}

	////////////////////////////////////////////////////////////////////////////

	Texture* Texture_New()
	{
		return Backend::Get()->NewTexture();
	}

	void Texture_Delete(Texture* tex)
	{
		return Backend::Get()->DeleteTexture(tex);
	}

	void Texture_Set2DImage(Texture* tex, int width, int height, PixelFormat format)
	{
		tex->Set2DImage(width, height, format);
	}

	void Texture_SetCubeImage(Texture* tex, int size, PixelFormat format)
	{
		tex->SetCubeImage(size, format);
	}
}
