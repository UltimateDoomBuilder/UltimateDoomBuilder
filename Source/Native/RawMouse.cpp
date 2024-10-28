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
#include "RawMouse.h"

#ifdef WIN32

#ifndef HID_USAGE_PAGE_GENERIC
#define HID_USAGE_PAGE_GENERIC		((USHORT) 0x01)
#endif

#ifndef HID_USAGE_GENERIC_MOUSE
#define HID_USAGE_GENERIC_MOUSE	((USHORT) 0x02)
#endif

#ifndef HID_USAGE_GENERIC_JOYSTICK
#define HID_USAGE_GENERIC_JOYSTICK	((USHORT) 0x04)
#endif

#ifndef HID_USAGE_GENERIC_GAMEPAD
#define HID_USAGE_GENERIC_GAMEPAD	((USHORT) 0x05)
#endif

#ifndef RIDEV_INPUTSINK
#define RIDEV_INPUTSINK	(0x100)
#endif

class RawMouseWindowClass
{
public:
	RawMouseWindowClass()
	{
		WNDCLASSEX windowClassDesc;
		memset(&windowClassDesc, 0, sizeof(WNDCLASSEX));
		windowClassDesc.cbSize = sizeof(WNDCLASSEX);
		windowClassDesc.lpszClassName = ClassName;
		windowClassDesc.hInstance = GetModuleHandle(nullptr);
		windowClassDesc.lpfnWndProc = &RawMouse::WindowProc;
		RegisterClassEx(&windowClassDesc);
	}

	const TCHAR* ClassName = TEXT("RawMouseWindow");
};

RawMouse::RawMouse(void* ownerWindow)
{
	static RawMouseWindowClass win32class;
	handle = CreateWindowEx(0, win32class.ClassName, TEXT(""), WS_POPUP, 0, 0, 100, 100, 0, 0, GetModuleHandle(nullptr), this);

	RAWINPUTDEVICE rid;
	rid.usUsagePage = HID_USAGE_PAGE_GENERIC;
	rid.usUsage = HID_USAGE_GENERIC_MOUSE;
	rid.dwFlags = RIDEV_INPUTSINK;
	rid.hwndTarget = handle;
	RegisterRawInputDevices(&rid, 1, sizeof(RAWINPUTDEVICE));
}

RawMouse::~RawMouse()
{
	if (handle)
		DestroyWindow(handle);
}

float RawMouse::GetX()
{
	float result = x;
	x = 0;
	return result;
}

float RawMouse::GetY()
{
	float result = y;
	y = 0;
	return result;
}

LRESULT RawMouse::OnMessage(INT message, WPARAM wparam, LPARAM lparam)
{
	if (message == WM_INPUT)
	{
		HRAWINPUT rawinputHandle = (HRAWINPUT)lparam;
		UINT size = 0;
		UINT result = GetRawInputData(rawinputHandle, RID_INPUT, 0, &size, sizeof(RAWINPUTHEADER));
		if (result == 0 && size > 0)
		{
			std::vector<uint32_t> buf((size + 3) / 4);
			result = GetRawInputData(rawinputHandle, RID_INPUT, buf.data(), &size, sizeof(RAWINPUTHEADER));
			if (result >= 0)
			{
				RAWINPUT* rawinput = (RAWINPUT*)buf.data();
				if (rawinput->header.dwType == RIM_TYPEMOUSE)
				{
					x += rawinput->data.mouse.lLastX;
					y += rawinput->data.mouse.lLastY;
				}
			}
		}
		return 0;
	}
	else
	{
		return DefWindowProc(handle, message, wparam, lparam);
	}
}

LRESULT RawMouse::WindowProc(HWND handle, UINT message, WPARAM wparam, LPARAM lparam)
{
	if (message == WM_CREATE)
	{
		CREATESTRUCT* createInfo = (CREATESTRUCT*)lparam;
		auto window = reinterpret_cast<RawMouse*>(createInfo->lpCreateParams);
		window->handle = handle;
		SetWindowLongPtr(handle, GWLP_USERDATA, reinterpret_cast<ULONG_PTR>(window));
		return window->OnMessage(message, wparam, lparam);
	}
	else
	{
		auto window = reinterpret_cast<RawMouse*>(GetWindowLongPtr(handle, GWLP_USERDATA));
		if (window)
			return window->OnMessage(message, wparam, lparam);
		else
			return DefWindowProc(handle, message, wparam, lparam);
	}
}

#else

RawMouse::RawMouse(void* ownerWindow)
{
}

RawMouse::~RawMouse()
{
}

float RawMouse::GetX()
{
	return 0;
}

float RawMouse::GetY()
{
	return 0;
}

#endif

/////////////////////////////////////////////////////////////////////////////

extern "C"
{

RawMouse* RawMouse_New(void* hwnd)
{
#if defined(WIN32)
	return new RawMouse(hwnd);
#else
	return nullptr;
#endif
}

void RawMouse_Delete(RawMouse* mouse)
{
	delete mouse;
}

float RawMouse_GetX(RawMouse* mouse)
{
	return mouse->GetX();
}

float RawMouse_GetY(RawMouse* mouse)
{
	return mouse->GetY();
}

#ifdef UDB_LINUX
#include <X11/extensions/Xfixes.h>

static Display *display = NULL;
#endif

void MouseInput_ShowCursor(bool show)
{
#ifdef UDB_LINUX
	if (display == NULL)
	{
		display = XOpenDisplay(NULL);
		if (display == NULL)
			return;
    }

	if (show)
		XFixesShowCursor(display, DefaultRootWindow(display));
	else
		XFixesHideCursor(display, DefaultRootWindow(display));
	XSync(display, True);
#endif
}

}
