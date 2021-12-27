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
#include "GLShader.h"
#include "GLRenderDevice.h"
#include <stdexcept>

void GLShader::Setup(const std::string& identifier, const std::string& vertexShader, const std::string& fragmentShader, bool alphatest)
{
	mIdentifier = identifier;
	mVertexText = vertexShader;
	mFragmentText = fragmentShader;
	mAlphatest = alphatest;
}

bool GLShader::CheckCompile(GLRenderDevice* device)
{
	bool firstCall = !mProgramBuilt;
	if (firstCall)
	{
		mProgramBuilt = true;
		CreateProgram(device);
		glUseProgram(mProgram);
		glUniform1i(glGetUniformLocation(mProgram, "texture1"), 0);
		glUniform1i(glGetUniformLocation(mProgram, "texture2"), 1);
		glUniform1i(glGetUniformLocation(mProgram, "texture3"), 2);
		glUseProgram(0);
	}

	return !mErrors.size();
}

std::string GLShader::GetCompileError()
{
	std::string lines = "Error compiling ";
	if (!mVertexShader)
		lines += "vertex ";
	else if (!mFragmentShader)
		lines += "fragment ";
	lines += "shader \"" + mIdentifier + "\":\r\n";
	for (auto c : mErrors)
	{
		if (c == '\r')
			continue;
		if (c == '\n')
			lines += "\r\n";
		else lines += c;
	}
	return lines;
}

void GLShader::Bind()
{
	if (!mProgram || !mProgramBuilt || mErrors.size())
		return;

	glUseProgram(mProgram);
}

void GLShader::CreateProgram(GLRenderDevice* device)
{
	const char* prefixNAT = R"(
		#version 330
		#line 1
	)";
	const char* prefixAT = R"(
		#version 330
		#define ALPHA_TEST
		#line 1
	)";

	const char* prefix = mAlphatest ? prefixAT : prefixNAT;

	mVertexShader = CompileShader(prefix + mVertexText, GL_VERTEX_SHADER);
	if (!mVertexShader)
		return;

	mFragmentShader = CompileShader(prefix + mFragmentText, GL_FRAGMENT_SHADER);
	if (!mFragmentShader)
		return;

	mProgram = glCreateProgram();
	glAttachShader(mProgram, mVertexShader);
	glAttachShader(mProgram, mFragmentShader);
	glBindAttribLocation(mProgram, (GLuint)DeclarationUsage::Position, "AttrPosition");
	glBindAttribLocation(mProgram, (GLuint)DeclarationUsage::Color, "AttrColor");
	glBindAttribLocation(mProgram, (GLuint)DeclarationUsage::TextureCoordinate, "AttrUV");
	glBindAttribLocation(mProgram, (GLuint)DeclarationUsage::Normal, "AttrNormal");
	glLinkProgram(mProgram);

	GLint status = 0;
	glGetProgramiv(mProgram, GL_LINK_STATUS, &status);
	if (status != GL_TRUE)
	{
		GLsizei length = 0;
		glGetProgramiv(mProgram, GL_INFO_LOG_LENGTH, &length);
		std::vector<GLchar> errors(length + (size_t)1);
		glGetProgramInfoLog(mProgram, (GLsizei)errors.size(), &length, errors.data());
		mErrors = { errors.begin(), errors.begin() + length };

		glDeleteProgram(mProgram);
		glDeleteShader(mVertexShader);
		glDeleteShader(mFragmentShader);
		mProgram = 0;
		mVertexShader = 0;
		mFragmentShader = 0;
		return;
	}

	UniformLastUpdates.resize(device->mUniformInfo.size());
	UniformLocations.resize(device->mUniformInfo.size(), (GLuint)-1);

	int count = (int)UniformLocations.size();
	for (int i = 0; i < count; i++)
	{
		const auto& name = device->mUniformInfo[i].Name;
		if (!name.empty())
			UniformLocations[i] = glGetUniformLocation(mProgram, name.c_str());
	}
}

GLuint GLShader::CompileShader(const std::string& code, GLenum type)
{
	GLuint shader = glCreateShader(type);
	const GLchar* sources[] = { (GLchar*)code.data() };
	const GLint lengths[] = { (GLint)code.size() };
	glShaderSource(shader, 1, sources, lengths);
	glCompileShader(shader);

	GLint status = 0;
	glGetShaderiv(shader, GL_COMPILE_STATUS, &status);
	if (status != GL_TRUE)
	{
		GLsizei length = 0;
		glGetShaderiv(shader, GL_INFO_LOG_LENGTH, &length);
		std::vector<GLchar> errors(length + (size_t)1);
		glGetShaderInfoLog(shader, (GLsizei)errors.size(), &length, errors.data());
		mErrors = { errors.begin(), errors.begin() + length };
		glDeleteShader(shader);
		return 0;
	}
	return shader;
}

void GLShader::ReleaseResources()
{
	if (mProgram)
		glDeleteProgram(mProgram);
	if (mVertexShader)
		glDeleteShader(mVertexShader);
	if (mFragmentShader)
		glDeleteShader(mFragmentShader);
	mProgram = 0;
	mVertexShader = 0;
	mFragmentShader = 0;
}
