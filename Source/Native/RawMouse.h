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

#ifdef WIN32

class RawMouse
{
public:
	RawMouse(void* ownerWindow);
	~RawMouse();

	float GetX();
	float GetY();

private:
	LRESULT OnMessage(INT message, WPARAM wparam, LPARAM lparam);
	static LRESULT CALLBACK WindowProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam);

	HWND handle = 0;
	int x = 0;
	int y = 0;
	// previous position for devices that report MOUSE_MOVE_ABSOLUTE
	bool haveAbs = false;
	long lastAbsX = 0;
	long lastAbsY = 0;

	friend class RawMouseWindowClass;
};

#else

class RawMouse
{
public:
	RawMouse(void* ownerWindow);
	~RawMouse();

	float GetX();
	float GetY();
};

#endif
