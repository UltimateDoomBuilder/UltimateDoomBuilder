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
#include "GLTexture.h"
#include "GLRenderDevice.h"
#include <stdexcept>

GLTexture::GLTexture()
{
}

GLTexture::~GLTexture()
{
	Finalize();
}

void GLTexture::Finalize()
{
	if (Device)
		Invalidate();
}

void GLTexture::Set2DImage(int width, int height, PixelFormat format)
{
	// This really shouldn't be here. The calling code should send valid input and this should throw an error.
	if (width < 1) width = 16;
	if (height < 1) height = 16;
	mCubeTexture = false;
	mWidth = width;
	mHeight = height;
	mFormat = format;
}

void GLTexture::SetCubeImage(int size, PixelFormat format)
{
	mCubeTexture = true;
	mWidth = size;
	mHeight = size;
	mFormat = format;
}

bool GLTexture::SetPixels(GLRenderDevice* device, const void* data)
{
	GLint texture = GetTexture(device);
	if (!texture) return false;

	GLint oldActiveTex = GL_TEXTURE0;
	glGetIntegerv(GL_ACTIVE_TEXTURE, &oldActiveTex);
	glActiveTexture(GL_TEXTURE0);
	GLint oldBinding = 0;
	glGetIntegerv(GL_TEXTURE_BINDING_2D, &oldBinding);

	//
	
	glBindBuffer(GL_PIXEL_UNPACK_BUFFER, 0);
	glBindTexture(GL_TEXTURE_2D, mTexture);
	glTexImage2D(GL_TEXTURE_2D, 0, ToInternalFormat(mFormat), mWidth, mHeight, 0, ToDataFormat(mFormat), ToDataType(mFormat), data);
	if (data != nullptr) 
		glGenerateMipmap(GL_TEXTURE_2D);

	//

	glBindTexture(GL_TEXTURE_2D, oldBinding);
	glActiveTexture(oldActiveTex);

	return true;
}

bool GLTexture::SetCubePixels(GLRenderDevice* device, CubeMapFace face, const void* data)
{
	static GLint cubeMapFaceToGL[] =
	{
		GL_TEXTURE_CUBE_MAP_POSITIVE_X, GL_TEXTURE_CUBE_MAP_POSITIVE_Y, GL_TEXTURE_CUBE_MAP_POSITIVE_Z,
		GL_TEXTURE_CUBE_MAP_NEGATIVE_X, GL_TEXTURE_CUBE_MAP_NEGATIVE_Y, GL_TEXTURE_CUBE_MAP_NEGATIVE_Z
	};

	GLint texture = GetTexture(device);
	if (!texture) return false;

	GLint oldActiveTex = GL_TEXTURE0;
	glGetIntegerv(GL_ACTIVE_TEXTURE, &oldActiveTex);
	glActiveTexture(GL_TEXTURE0);
	GLint oldBinding = 0;
	glGetIntegerv(GL_TEXTURE_BINDING_CUBE_MAP, &oldBinding);

	//

	glBindBuffer(GL_PIXEL_UNPACK_BUFFER, 0);
	glBindTexture(GL_TEXTURE_CUBE_MAP, mTexture);
	glTexImage2D(cubeMapFaceToGL[(int)face], 0, ToInternalFormat(mFormat), mWidth, mHeight, 0, ToDataFormat(mFormat), ToDataType(mFormat), data);
	if (data != nullptr && face == CubeMapFace::NegativeZ)
		glGenerateMipmap(GL_TEXTURE_CUBE_MAP);

	//

	glBindTexture(GL_TEXTURE_CUBE_MAP, oldBinding);
	glActiveTexture(oldActiveTex);

	return true;
}

void GLTexture::Invalidate()
{
	if (mDepthRenderbuffer) glDeleteRenderbuffers(1, &mDepthRenderbuffer);
	if (mFramebuffer) glDeleteFramebuffers(1, &mFramebuffer);
	if (mTexture) glDeleteTextures(1, &mTexture);
	if (mPBO) glDeleteBuffers(1, &mPBO);
	mDepthRenderbuffer = 0;
	mFramebuffer = 0;
	mTexture = 0;
	mPBO = 0;
	if (Device) Device->mTextures.erase(ItTexture);
	Device = nullptr;
}

GLuint GLTexture::GetTexture(GLRenderDevice* device)
{
	if (mTexture == 0)
	{
		if (Device == nullptr)
		{
			Device = device;
			ItTexture = Device->mTextures.insert(Device->mTextures.end(), this);
		}

		GLint oldActiveTex = GL_TEXTURE0;
		glGetIntegerv(GL_ACTIVE_TEXTURE, &oldActiveTex);
		glActiveTexture(GL_TEXTURE0);

		glGenTextures(1, &mTexture);

		if (!IsCubeTexture())
		{
			GLint oldBinding = 0;
			glGetIntegerv(GL_TEXTURE_BINDING_2D, &oldBinding);

			glBindBuffer(GL_PIXEL_UNPACK_BUFFER, 0);
			glBindTexture(GL_TEXTURE_2D, mTexture);
			glTexImage2D(GL_TEXTURE_2D, 0, ToInternalFormat(mFormat), mWidth, mHeight, 0, ToDataFormat(mFormat), ToDataType(mFormat), nullptr);

			glBindTexture(GL_TEXTURE_2D, oldBinding);
		}
		else
		{
			GLint oldBinding = 0;
			glGetIntegerv(GL_TEXTURE_BINDING_CUBE_MAP, &oldBinding);

			glBindBuffer(GL_PIXEL_UNPACK_BUFFER, 0);
			glBindTexture(GL_TEXTURE_CUBE_MAP, mTexture);
			glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_X, 0, ToInternalFormat(mFormat), mWidth, mHeight, 0, ToDataFormat(mFormat), ToDataType(mFormat), nullptr);
			glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_Y, 0, ToInternalFormat(mFormat), mWidth, mHeight, 0, ToDataFormat(mFormat), ToDataType(mFormat), nullptr);
			glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_Z, 0, ToInternalFormat(mFormat), mWidth, mHeight, 0, ToDataFormat(mFormat), ToDataType(mFormat), nullptr);
			glTexImage2D(GL_TEXTURE_CUBE_MAP_NEGATIVE_X, 0, ToInternalFormat(mFormat), mWidth, mHeight, 0, ToDataFormat(mFormat), ToDataType(mFormat), nullptr);
			glTexImage2D(GL_TEXTURE_CUBE_MAP_NEGATIVE_Y, 0, ToInternalFormat(mFormat), mWidth, mHeight, 0, ToDataFormat(mFormat), ToDataType(mFormat), nullptr);
			glTexImage2D(GL_TEXTURE_CUBE_MAP_NEGATIVE_Z, 0, ToInternalFormat(mFormat), mWidth, mHeight, 0, ToDataFormat(mFormat), ToDataType(mFormat), nullptr);

			glBindTexture(GL_TEXTURE_CUBE_MAP, oldBinding);
		}

		glActiveTexture(oldActiveTex);
	}
	return mTexture;
}

GLuint GLTexture::GetFramebuffer(GLRenderDevice* device, bool usedepthbuffer)
{
	if (!usedepthbuffer)
	{
		if (mFramebuffer == 0)
		{
			GLuint texture = GetTexture(device);
			glGenFramebuffers(1, &mFramebuffer);
			glBindFramebuffer(GL_FRAMEBUFFER, mFramebuffer);
			glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, texture, 0);
			if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
				throw std::runtime_error("glCheckFramebufferStatus did not return GL_FRAMEBUFFER_COMPLETE");
		}
		return mFramebuffer;
	}
	else
	{
		if (mDepthRenderbuffer == 0)
		{
			if (Device == nullptr)
			{
				Device = device;
				ItTexture = Device->mTextures.insert(Device->mTextures.end(), this);
			}

			glGenRenderbuffers(1, &mDepthRenderbuffer);
			glBindRenderbuffer(GL_RENDERBUFFER, mDepthRenderbuffer);
			glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH24_STENCIL8, mWidth, mHeight);
			glBindRenderbuffer(GL_RENDERBUFFER, 0);
		}

		if (mFramebuffer == 0)
		{
			GLuint texture = GetTexture(device);
			glGenFramebuffers(1, &mFramebuffer);
			glBindFramebuffer(GL_FRAMEBUFFER, mFramebuffer);
			glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, texture, 0);
			glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_STENCIL_ATTACHMENT, GL_RENDERBUFFER, mDepthRenderbuffer);
			if (glCheckFramebufferStatus(GL_FRAMEBUFFER) != GL_FRAMEBUFFER_COMPLETE)
				throw std::runtime_error("glCheckFramebufferStatus did not return GL_FRAMEBUFFER_COMPLETE");
			glBindFramebuffer(GL_FRAMEBUFFER, 0);
		}
		return mFramebuffer;
	}
}

GLuint GLTexture::GetPBO(GLRenderDevice* device)
{
	if (mPBO == 0)
	{
		if (Device == nullptr)
		{
			Device = device;
			ItTexture = Device->mTextures.insert(Device->mTextures.end(), this);
		}

		glGenBuffers(1, &mPBO);
		glBindBuffer(GL_PIXEL_UNPACK_BUFFER, mPBO);
		glBufferData(GL_PIXEL_UNPACK_BUFFER, mWidth*mHeight * 4, NULL, GL_STREAM_DRAW);
	}

	return mPBO;
}

GLint GLTexture::ToInternalFormat(PixelFormat format)
{
	static GLint cvt[] =
	{
		GL_RGBA8,
		GL_RGBA8,
		GL_RG16F,
		GL_RGBA16F,
		GL_R32F,
		GL_RG32F,
		GL_RGB32F,
		GL_RGBA32F,
		GL_DEPTH32F_STENCIL8,
		GL_DEPTH24_STENCIL8
	};
	return cvt[(int)format];
}

GLenum GLTexture::ToDataFormat(PixelFormat format)
{
	static GLint cvt[] =
	{
		GL_RGBA,
		GL_BGRA,
		GL_RG,
		GL_RGBA,
		GL_RED,
		GL_RG,
		GL_RGB,
		GL_RGBA,
		GL_DEPTH_STENCIL,
		GL_DEPTH_STENCIL
	};
	return cvt[(int)format];
}

GLenum GLTexture::ToDataType(PixelFormat format)
{
	static GLint cvt[] =
	{
		GL_UNSIGNED_BYTE,
		GL_UNSIGNED_BYTE,
		GL_FLOAT,
		GL_FLOAT,
		GL_FLOAT,
		GL_FLOAT,
		GL_FLOAT,
		GL_FLOAT,
		GL_FLOAT_32_UNSIGNED_INT_24_8_REV,
		GL_UNSIGNED_INT_24_8
	};
	return cvt[(int)format];
}
