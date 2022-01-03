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

#include "../Backend.h"
#include "OpenGLContext.h"
#include <list>

class GLSharedVertexBuffer;
class GLShader;
class GLShaderManager;
class GLVertexBuffer;
class GLIndexBuffer;
class GLTexture;

class GLRenderDevice : public RenderDevice
{
public:
	GLRenderDevice(void* disp, void* window);
	~GLRenderDevice();

	void DeclareUniform(UniformName name, const char* glslname, UniformType type) override;
	void DeclareShader(ShaderName index, const char* name, const char* vertexshader, const char* fragmentshader) override;
	void SetShader(ShaderName name) override;
	void SetUniform(UniformName name, const void* values, int count, int bytesize) override;
	void SetVertexBuffer(VertexBuffer* buffer) override;
	void SetIndexBuffer(IndexBuffer* buffer) override;
	void SetAlphaBlendEnable(bool value) override;
	void SetAlphaTestEnable(bool value) override;
	void SetCullMode(Cull mode) override;
	void SetBlendOperation(BlendOperation op) override;
	void SetSourceBlend(Blend blend) override;
	void SetDestinationBlend(Blend blend) override;
	void SetFillMode(FillMode mode) override;
	void SetMultisampleAntialias(bool value) override;
	void SetZEnable(bool value) override;
	void SetZWriteEnable(bool value) override;
	void SetTexture(int unit, Texture* texture) override;
	void SetSamplerFilter(int unit, TextureFilter minfilter, TextureFilter magfilter, MipmapFilter mipfilter, float maxanisotropy) override;
	void SetSamplerState(int unit, TextureAddress address) override;
	bool Draw(PrimitiveType type, int startIndex, int primitiveCount) override;
	bool DrawIndexed(PrimitiveType type, int startIndex, int primitiveCount) override;
	bool DrawData(PrimitiveType type, int startIndex, int primitiveCount, const void* data) override;
	bool StartRendering(bool clear, int backcolor, Texture* target, bool usedepthbuffer) override;
	bool FinishRendering() override;
	bool Present() override;
	bool ClearTexture(int backcolor, Texture* texture) override;
	bool CopyTexture(Texture* dst, CubeMapFace face) override;

	bool SetVertexBufferData(VertexBuffer* buffer, void* data, int64_t size, VertexFormat format) override;
	bool SetVertexBufferSubdata(VertexBuffer* buffer, int64_t destOffset, void* data, int64_t size) override;
	bool SetIndexBufferData(IndexBuffer* buffer, void* data, int64_t size) override;

	bool SetPixels(Texture* texture, const void* data) override;
	bool SetCubePixels(Texture* texture, CubeMapFace face, const void* data) override;
	void* MapPBO(Texture* texture) override;
	bool UnmapPBO(Texture* texture) override;

	bool InvalidateTexture(GLTexture* texture);

	void GarbageCollectBuffer(int size, VertexFormat format);

	bool ApplyViewport();
	bool ApplyChanges();
	bool ApplyVertexBuffer();
	bool ApplyIndexBuffer();
	bool ApplyShader();
	bool ApplyUniforms();
	bool ApplyTextures();
	bool ApplyRasterizerState();
	bool ApplyBlendState();
	bool ApplyDepthState();

	void CheckContext();
	void RequireContext();

	bool CheckGLError();

	GLShader* GetActiveShader();

	GLint GetGLMinFilter(TextureFilter filter, MipmapFilter mipfilter);

	static std::mutex& GetMutex();
	static void DeleteObject(GLVertexBuffer* buffer);
	static void DeleteObject(GLIndexBuffer* buffer);
	static void DeleteObject(GLTexture* texture);

	void ProcessDeleteList(bool finalize = false);

	std::unique_ptr<IOpenGLContext> Context;

	struct DeleteList
	{
		std::vector<GLVertexBuffer*> VertexBuffers;
		std::vector<GLIndexBuffer*> IndexBuffers;
		std::vector<GLTexture*> Textures;
	} mDeleteList;
	
	struct TextureUnit
	{
		GLTexture* Tex = nullptr;
		TextureAddress WrapMode = TextureAddress::Wrap;
		GLuint SamplerHandle = 0;
		TextureFilter MinFilter = TextureFilter::Nearest;
		TextureFilter MagFilter = TextureFilter::Nearest;
		MipmapFilter MipFilter = MipmapFilter::None;
		float MaxAnisotropy = 1;
	} mTextureUnit[10];

	struct SamplerFilterKey
	{
		GLuint MinFilter = 0;
		GLuint MagFilter = 0;
		float MaxAnisotropy = 0.0f;

		bool operator<(const SamplerFilterKey& b) const { return memcmp(this, &b, sizeof(SamplerFilterKey)) < 0; }
		bool operator==(const SamplerFilterKey& b) const { return memcmp(this, &b, sizeof(SamplerFilterKey)) == 0; }
		bool operator!=(const SamplerFilterKey& b) const { return memcmp(this, &b, sizeof(SamplerFilterKey)) != 0; }
	};
	
	struct SamplerFilter
    {
        GLuint WrapModes[2] = { 0, 0 };
    };

  std::map<SamplerFilterKey, SamplerFilter> mSamplers;

  SamplerFilterKey GetSamplerFilterKey(TextureFilter filter, MipmapFilter mipFilter, float maxAnisotropy);

	int mVertexBuffer = -1;
	int64_t mVertexBufferStartIndex = 0;

	GLIndexBuffer* mIndexBuffer = nullptr;

	std::unique_ptr<GLSharedVertexBuffer> mSharedVertexBuffers[2];

	std::list<GLTexture*> mTextures;
	std::list<GLIndexBuffer*> mIndexBuffers;

	std::unique_ptr<GLShaderManager> mShaderManager;
	ShaderName mShaderName = {};

	struct UniformInfo
	{
		std::string Name;
		UniformType Type = {};
		int LastUpdate = 0;
		int Count = 0;
		std::vector<uint8_t> Data;
	};

	std::vector<UniformInfo> mUniformInfo;

	GLuint mStreamVertexBuffer = 0;
	GLuint mStreamVAO = 0;

	Cull mCullMode = Cull::None;
	FillMode mFillMode = FillMode::Solid;
	bool mAlphaTest = false;

	bool mAlphaBlend = false;
	BlendOperation mBlendOperation = BlendOperation::Add;
	Blend mSourceBlend = Blend::SourceAlpha;
	Blend mDestinationBlend = Blend::InverseSourceAlpha;

	bool mDepthTest = false;
	bool mDepthWrite = false;

	bool mNeedApply = true;
	bool mShaderChanged = true;
	bool mUniformsChanged = true;
	bool mTexturesChanged = true;
	bool mIndexBufferChanged = true;
	bool mVertexBufferChanged = true;
	bool mDepthStateChanged = true;
	bool mBlendStateChanged = true;
	bool mRasterizerStateChanged = true;

	bool mContextIsCurrent = false;

	int mViewportWidth = 0;
	int mViewportHeight = 0;
};
