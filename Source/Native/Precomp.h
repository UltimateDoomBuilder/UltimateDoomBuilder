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

#define _CRT_SECURE_NO_WARNINGS

#include <cstdint>
#include <algorithm>
#include <vector>
#include <map>
#include <memory>
#include <string>

#ifdef WIN32
#include <Windows.h>
#undef min
#undef max
#endif

#include "OpenGL/gl_load/gl_system.h"

#define APART(x) (static_cast<uint32_t>(x) >> 24)
#define RPART(x) ((static_cast<uint32_t>(x) >> 16)  & 0xff)
#define GPART(x) ((static_cast<uint32_t>(x) >> 8)  & 0xff)
#define BPART(x) (static_cast<uint32_t>(x) & 0xff)
