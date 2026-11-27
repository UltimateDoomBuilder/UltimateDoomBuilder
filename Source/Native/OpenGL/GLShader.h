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
#include "GLRenderDevice.h"

class GLShader
{
public:
	void ReleaseResources();

	void Setup(const std::string& identifier, const std::string& vertexShader, const std::string& fragmentShader, bool alphatest);
	bool CheckCompile(GLRenderDevice *device);
	void Bind();
	bool IsProgramBuilt() const { return mProgramBuilt; }

	std::string GetCompileError();

	std::vector<int> UniformLastUpdates;
	std::vector<GLuint> UniformLocations;

private:
	void CreateProgram(GLRenderDevice* device);
	GLuint CompileShader(const std::string& code, GLenum type);

	std::string mIdentifier;
	std::string mVertexText;
	std::string mFragmentText;
	bool mAlphatest = false;
	bool mProgramBuilt = false;

	GLuint mProgram = 0;
	GLuint mVertexShader = 0;
	GLuint mFragmentShader = 0;
	std::string mErrors;
};
