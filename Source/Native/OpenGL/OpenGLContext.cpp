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
#include "OpenGLContext.h"
#include "../Backend.h"
#include <stdexcept>

class OpenGLLoadFunctions
{
public:
	OpenGLLoadFunctions() { ogl_LoadFunctions(); }
};

#ifdef WIN32

#include <CommCtrl.h>

#define WGL_CONTEXT_MAJOR_VERSION_ARB           0x2091
#define WGL_CONTEXT_MINOR_VERSION_ARB           0x2092
#define WGL_CONTEXT_LAYER_PLANE_ARB             0x2093
#define WGL_CONTEXT_FLAGS_ARB                   0x2094
#define WGL_CONTEXT_DEBUG_BIT_ARB               0x0001
#define WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB  0x0002
#define ERROR_INVALID_VERSION_ARB               0x2095

#define WGL_NUMBER_CL_PIXEL_FORMATS             0x2000
#define WGL_DRAW_TO_WINDOW                   0x2001
#define WGL_DRAW_TO_BITMAP                   0x2002
#define WGL_ACCELERATION                     0x2003
#define WGL_NEED_PALETTE                     0x2004
#define WGL_NEED_SYSTEM_PALETTE              0x2005
#define WGL_SWAP_LAYER_BUFFERS               0x2006
#define WGL_SWAP_METHOD                      0x2007
#define WGL_NUMBER_OVERLAYS                  0x2008
#define WGL_NUMBER_UNDERLAYS                 0x2009
#define WGL_TRANSPARENT                      0x200A
#define WGL_TRANSPARENT_VALUE                0x200B
#define WGL_SHARE_DEPTH                      0x200C
#define WGL_SHARE_STENCIL                    0x200D
#define WGL_SHARE_ACCUM                      0x200E
#define WGL_SUPPORT_GDI                      0x200F
#define WGL_SUPPORT_OPENGL                   0x2010
#define WGL_DOUBLE_BUFFER                    0x2011
#define WGL_STEREO                           0x2012
#define WGL_PIXEL_TYPE                       0x2013
#define WGL_COLOR_BITS                       0x2014
#define WGL_RED_BITS                         0x2015
#define WGL_RED_SHIFT                        0x2016
#define WGL_GREEN_BITS                       0x2017
#define WGL_GREEN_SHIFT                      0x2018
#define WGL_BLUE_BITS                        0x2019
#define WGL_BLUE_SHIFT                       0x201A
#define WGL_ALPHA_BITS                       0x201B
#define WGL_ALPHA_SHIFT                      0x201C
#define WGL_ACCUM_BITS                       0x201D
#define WGL_ACCUM_RED_BITS                   0x201E
#define WGL_ACCUM_GREEN_BITS                 0x201F
#define WGL_ACCUM_BLUE_BITS                  0x2020
#define WGL_ACCUM_ALPHA_BITS                 0x2021
#define WGL_DEPTH_BITS                       0x2022
#define WGL_STENCIL_BITS                     0x2023
#define WGL_AUX_BUFFERS                      0x2024
#define WGL_NO_ACCELERATION                  0x2025
#define WGL_GENERIC_ACCELERATION             0x2026
#define WGL_FULL_ACCELERATION                0x2027
#define WGL_SWAP_EXCHANGE                    0x2028
#define WGL_SWAP_COPY                        0x2029
#define WGL_SWAP_UNDEFINED                   0x202A
#define WGL_TYPE_RGBA                        0x202B
#define WGL_TYPE_COLORINDEX                  0x202C
#define WGL_SAMPLE_BUFFERS                   0x2041
#define WGL_SAMPLES                          0x2042

/////////////////////////////////////////////////////////////////////////////

class OpenGLContext : public IOpenGLContext
{
public:
	OpenGLContext(void* window);
	~OpenGLContext();

	void MakeCurrent() override;
	void ClearCurrent() override;
	void SwapBuffers() override;
	bool IsCurrent() override;

	int GetWidth() const override;
	int GetHeight() const override;

	bool IsValid() const { return context != 0; }

private:
	HWND window;
	HDC dc;
	HGLRC context;

	typedef HGLRC(WINAPI* ptr_wglCreateContextAttribsARB)(HDC, HGLRC, const int*);
	typedef BOOL(WINAPI* ptr_wglGetPixelFormatAttribivEXT)(HDC, int, int, UINT, int*, int*);
	typedef BOOL(WINAPI* ptr_wglGetPixelFormatAttribfvEXT)(HDC, int, int, UINT, int*, FLOAT*);
	typedef BOOL(WINAPI* ptr_wglChoosePixelFormatEXT)(HDC, const int*, const FLOAT*, UINT, int*, UINT*);
	typedef GLenum(WINAPI* ptr_glError)();

	struct CreateFunctions
	{
		ptr_wglCreateContextAttribsARB wglCreateContextAttribsARB = nullptr;
		ptr_wglGetPixelFormatAttribivEXT wglGetPixelFormatAttribivEXT = nullptr;
		ptr_wglGetPixelFormatAttribfvEXT wglGetPixelFormatAttribfvEXT = nullptr;
		ptr_wglChoosePixelFormatEXT wglChoosePixelFormatEXT = nullptr;
		ptr_glError error = nullptr;
	};

	static CreateFunctions GetCreateFunctions(HWND window);
	static HGLRC CreateGL3Context(HWND window, HDC hdc, HGLRC share_context);
};

/////////////////////////////////////////////////////////////////////////////

OpenGLContext::OpenGLContext(void* windowptr) : window((HWND)windowptr)
{
	dc = GetDC(window);
	context = CreateGL3Context(window, dc, 0);
	if (context)
	{
		MakeCurrent();
		static OpenGLLoadFunctions loadFunctions;
		ClearCurrent();
	}
}

OpenGLContext::~OpenGLContext()
{
	if (context)
		wglDeleteContext(context);
	if (dc)
		ReleaseDC(window, dc);
}

void OpenGLContext::MakeCurrent()
{
	if (!IsCurrent())
		wglMakeCurrent(dc, context);
}

bool OpenGLContext::IsCurrent()
{
	return (wglGetCurrentContext() == context);
}

void OpenGLContext::ClearCurrent()
{
	wglMakeCurrent(0, 0);
}

void OpenGLContext::SwapBuffers()
{
	wglMakeCurrent(dc, context);
	::SwapBuffers(dc);
}

int OpenGLContext::GetWidth() const
{
	RECT box = { 0 };
	GetClientRect(window, &box);
	return box.right - box.left;
}

int OpenGLContext::GetHeight() const
{
	RECT box = { 0 };
	GetClientRect(window, &box);
	return box.bottom - box.top;
}

OpenGLContext::CreateFunctions OpenGLContext::GetCreateFunctions(HWND window)
{
	CreateFunctions functions;

	HWND queryWindow = CreateWindowEx(0, WC_STATIC, TEXT(""), WS_CHILD, 0, 0, 256, 256, window, 0, GetModuleHandle(0), 0);
	if (queryWindow)
	{
		HDC queryDC = GetDC(queryWindow);
		if (queryDC)
		{
			PIXELFORMATDESCRIPTOR pfd;
			memset(&pfd, 0, sizeof(PIXELFORMATDESCRIPTOR));
			pfd.nSize = sizeof(PIXELFORMATDESCRIPTOR);
			pfd.nVersion = 1;
			pfd.dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER;
			pfd.iPixelType = PFD_TYPE_RGBA;
			pfd.cColorBits = 32;
			pfd.cStencilBits = 8;
			pfd.cDepthBits = 16;
			int pixelformat = ChoosePixelFormat(queryDC, &pfd);
			SetPixelFormat(queryDC, pixelformat, &pfd);

			HGLRC queryContext = wglCreateContext(queryDC);
			if (queryContext)
			{
				wglMakeCurrent(queryDC, queryContext);

				functions.wglCreateContextAttribsARB = (ptr_wglCreateContextAttribsARB)wglGetProcAddress("wglCreateContextAttribsARB");
				functions.wglGetPixelFormatAttribivEXT = (ptr_wglGetPixelFormatAttribivEXT)wglGetProcAddress("wglGetPixelFormatAttribivEXT");
				functions.wglGetPixelFormatAttribfvEXT = (ptr_wglGetPixelFormatAttribfvEXT)wglGetProcAddress("wglGetPixelFormatAttribfvEXT");
				functions.wglChoosePixelFormatEXT = (ptr_wglChoosePixelFormatEXT)wglGetProcAddress("wglChoosePixelFormatEXT");

				HMODULE opengl32 = LoadLibrary("opengl32.dll");
				if (opengl32)
				{
					functions.error = reinterpret_cast<ptr_glError>(GetProcAddress(opengl32, "glGetError"));
					FreeLibrary(opengl32);
				}

				wglMakeCurrent(0, 0);
			}

			// Clean up query window
			ReleaseDC(queryWindow, queryDC);
		}
		DestroyWindow(queryWindow);
	}

	return functions;
}

HGLRC OpenGLContext::CreateGL3Context(HWND window, HDC hdc, HGLRC share_context)
{
	CreateFunctions functions = GetCreateFunctions(window);
	if (!functions.wglCreateContextAttribsARB)
	{
		SetError("No OpenGL driver supporting OpenGL 3 found");
		return 0;
	}

	PIXELFORMATDESCRIPTOR pfd;
	memset(&pfd, 0, sizeof(PIXELFORMATDESCRIPTOR));
	pfd.nSize = sizeof(PIXELFORMATDESCRIPTOR);
	pfd.nVersion = 1;
	pfd.dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER;
	pfd.iPixelType = PFD_TYPE_RGBA;
	pfd.cColorBits = 32;
	pfd.cStencilBits = 8;
	pfd.cDepthBits = 16;

	int pixelformat = 0;
	if (functions.wglChoosePixelFormatEXT)
	{
		const int WGL_DRAW_TO_WINDOW_ARB = 0x2001;
		const int WGL_SUPPORT_OPENGL_ARB = 0x2010;
		const int WGL_DOUBLE_BUFFER_ARB = 0x2011;
		const int WGL_ACCELERATION_ARB = 0x2003;
		const int WGL_FULL_ACCELERATION_ARB = 0x2027;
		const int WGL_RED_BITS_ARB = 0x2015;
		const int WGL_GREEN_BITS_ARB = 0x2017;
		const int WGL_BLUE_BITS_ARB = 0x2019;
		const int WGL_ALPHA_BITS_ARB = 0x201B;
		const int WGL_DEPTH_BITS_ARB = 0x2022;
		const int WGL_STENCIL_BITS_ARB = 0x2023;

		std::vector<float> floatattributes = { 0.0f, 0.0f };
		std::vector<int> attributes =
		{
			WGL_DRAW_TO_WINDOW_ARB, GL_TRUE,
			WGL_SUPPORT_OPENGL_ARB, GL_TRUE,
			WGL_DOUBLE_BUFFER_ARB, GL_TRUE,
			WGL_ACCELERATION_ARB, WGL_FULL_ACCELERATION_ARB,
			WGL_RED_BITS_ARB, 8,
			WGL_GREEN_BITS_ARB, 8,
			WGL_BLUE_BITS_ARB, 8,
			WGL_ALPHA_BITS_ARB, 8,
			WGL_DEPTH_BITS_ARB, 16,
			WGL_STENCIL_BITS_ARB, 8,
			0, 0
		};

		unsigned int numFormats = 0;
		BOOL result = functions.wglChoosePixelFormatEXT(hdc, attributes.data(), floatattributes.data(), 1, &pixelformat, &numFormats);
		if (!result)
			pixelformat = 0;
	}

	if (pixelformat == 0)
		pixelformat = ChoosePixelFormat(hdc, &pfd);

	if (pixelformat == 0)
	{
		SetError("No compatible OpenGL pixel format found!");
		return 0;
	}

	BOOL result = SetPixelFormat(hdc, pixelformat, &pfd);
	if (!result)
	{
		SetError("OpenGL pixel format could not be set: SetPixelFormat failed!");
		return 0;
	}

	const int WGL_CONTEXT_CORE_PROFILE_BIT_ARB = 1;
	const int WGL_CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB = 2;
	const int WGL_CONTEXT_PROFILE_MASK_ARB = 0x9126;

	for (int profile : { WGL_CONTEXT_CORE_PROFILE_BIT_ARB, WGL_CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB })
	{
		for (int version : { 46, 45, 44, 43, 42, 41, 40, 33, 32 })
		{
			// some drivers only provide 3.2+ as forward-compatible core (e.g. macOS)
			int flags = (profile == WGL_CONTEXT_CORE_PROFILE_BIT_ARB) ? WGL_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB : 0;
			std::vector<int> int_attributes =
			{
				WGL_CONTEXT_MAJOR_VERSION_ARB, version / 10,
				WGL_CONTEXT_MINOR_VERSION_ARB, version % 10,
				WGL_CONTEXT_PROFILE_MASK_ARB, profile,
				WGL_CONTEXT_FLAGS_ARB, flags,
				0, 0
			};
			HGLRC opengl3_context = functions.wglCreateContextAttribsARB(hdc, share_context, int_attributes.data());
			if (opengl3_context)
				return opengl3_context;
		}
	}

	// Grab the error from the last create attempt
	if (functions.error)
		SetError("No OpenGL 3.2 support found (error code %d)", (int)functions.error());
	else
		SetError("No OpenGL 3.2 support found");
	return 0;
}

/////////////////////////////////////////////////////////////////////////////

std::unique_ptr<IOpenGLContext> IOpenGLContext::Create(void* disp, void* window)
{
	auto ctx = std::make_unique<OpenGLContext>(window);
	if (!ctx->IsValid()) return nullptr;
	return ctx;
}

#elif defined(__APPLE__)

class OpenGLContext : public IOpenGLContext
{
public:
	OpenGLContext(void* window) { }
	~OpenGLContext() { }

	void MakeCurrent() override { }
	void ClearCurrent() override { }
	void SwapBuffers() override { }
	bool IsCurrent() override { return false; }

	int GetWidth() const override { return 320; }
	int GetHeight() const override { return 200; }

	bool IsValid() const { return false; }

private:
};

std::unique_ptr<IOpenGLContext> IOpenGLContext::Create(void* disp, void* window)
{
	auto ctx = std::make_unique<OpenGLContext>(window);
	if (!ctx->IsValid()) return nullptr;
	return ctx;
}

#else

#include <X11/Xlib.h>
#include <X11/Xutil.h>
#include <GL/glx.h>

#define GLFUNC

typedef GLXContext(*ptr_glXCreateContextAttribs)(::Display* dpy, GLXFBConfig config, GLXContext share_list, Bool direct, const int* attrib_list);

class GL_GLXFunctions
{
public:
	typedef XVisualInfo* (GLFUNC* ptr_glXChooseVisual)(::Display* dpy, int screen, int* attrib_list);
	typedef void (GLFUNC* ptr_glXCopyContext)(::Display* dpy, GLXContext src, GLXContext dst, unsigned long mask);
	typedef GLXContext(GLFUNC* ptr_glXCreateContext)(::Display* dpy, XVisualInfo* vis, GLXContext share_list, Bool direct);
	typedef GLXPixmap(GLFUNC* ptr_glXCreateGLXPixmap)(::Display* dpy, XVisualInfo* vis, Pixmap pixmap);
	typedef void (GLFUNC* ptr_glXDestroyContext)(::Display* dpy, GLXContext ctx);
	typedef void (GLFUNC* ptr_glXDestroyGLXPixmap)(::Display* dpy, GLXPixmap pix);
	typedef int (GLFUNC* ptr_glXGetConfig)(::Display* dpy, XVisualInfo* vis, int attrib, int* value);
	typedef GLXContext(GLFUNC* ptr_glXGetCurrentContext)(void);
	typedef GLXDrawable(GLFUNC* ptr_glXGetCurrentDrawable)(void);
	typedef Bool(GLFUNC* ptr_glXIsDirect)(::Display* dpy, GLXContext ctx);
	typedef Bool(GLFUNC* ptr_glXMakeCurrent)(::Display* dpy, GLXDrawable drawable, GLXContext ctx);
	typedef Bool(GLFUNC* ptr_glXQueryExtension)(::Display* dpy, int* error_base, int* event_base);
	typedef Bool(GLFUNC* ptr_glXQueryVersion)(::Display* dpy, int* major, int* minor);
	typedef void (GLFUNC* ptr_glXSwapBuffers)(::Display* dpy, GLXDrawable drawable);
	typedef void (GLFUNC* ptr_glXUseXFont)(Font font, int first, int count, int list_base);
	typedef void (GLFUNC* ptr_glXWaitGL)(void);
	typedef void (GLFUNC* ptr_glXWaitX)(void);
	typedef const char* (GLFUNC* ptr_glXGetClientString)(::Display* dpy, int name);
	typedef const char* (GLFUNC* ptr_glXQueryServerString)(::Display* dpy, int screen, int name);
	typedef const char* (GLFUNC* ptr_glXQueryExtensionsString)(::Display* dpy, int screen);
	typedef ::Display* (GLFUNC* ptr_glXGetCurrentDisplay)(void);
	typedef GLXFBConfig* (GLFUNC* ptr_glXChooseFBConfig)(::Display* dpy, int screen, const int* attrib_list, int* nelements);
	typedef GLXContext(GLFUNC* ptr_glXCreateNewContext)(::Display* dpy, GLXFBConfig config, int render_type, GLXContext share_list, Bool direct);
	typedef GLXPbuffer(GLFUNC* ptr_glXCreatePbuffer)(::Display* dpy, GLXFBConfig config, const int* attrib_list);
	typedef GLXPixmap(GLFUNC* ptr_glXCreatePixmap)(::Display* dpy, GLXFBConfig config, Pixmap pixmap, const int* attrib_list);
	typedef GLXWindow(GLFUNC* ptr_glXCreateWindow)(::Display* dpy, GLXFBConfig config, Window win, const int* attrib_list);
	typedef void (GLFUNC* ptr_glXDestroyPbuffer)(::Display* dpy, GLXPbuffer pbuf);
	typedef void (GLFUNC* ptr_glXDestroyPixmap)(::Display* dpy, GLXPixmap pixmap);
	typedef void (GLFUNC* ptr_glXDestroyWindow)(::Display* dpy, GLXWindow win);
	typedef GLXDrawable(GLFUNC* ptr_glXGetCurrentReadDrawable)(void);
	typedef int (GLFUNC* ptr_glXGetFBConfigAttrib)(::Display* dpy, GLXFBConfig config, int attribute, int* value);
	typedef GLXFBConfig* (GLFUNC* ptr_glXGetFBConfigs)(::Display* dpy, int screen, int* nelements);
	typedef void (GLFUNC* ptr_glXGetSelectedEvent)(::Display* dpy, GLXDrawable draw, unsigned long* event_mask);
	typedef XVisualInfo* (GLFUNC* ptr_glXGetVisualFromFBConfig)(::Display* dpy, GLXFBConfig config);
	typedef Bool(GLFUNC* ptr_glXMakeContextCurrent)(::Display* display, GLXDrawable draw, GLXDrawable read, GLXContext ctx);
	typedef int (GLFUNC* ptr_glXQueryContext)(::Display* dpy, GLXContext ctx, int attribute, int* value);
	typedef void (GLFUNC* ptr_glXQueryDrawable)(::Display* dpy, GLXDrawable draw, int attribute, unsigned int* value);
	typedef void (GLFUNC* ptr_glXSelectEvent)(::Display* dpy, GLXDrawable draw, unsigned long event_mask);

	typedef __GLXextFuncPtr(GLFUNC* ptr_glXGetProcAddress) (const GLubyte*);
	typedef void (*(GLFUNC* ptr_glXGetProcAddressARB)(const GLubyte* procName))(void);

public:
	ptr_glXChooseVisual glXChooseVisual;
	ptr_glXCopyContext glXCopyContext;
	ptr_glXCreateContext glXCreateContext;
	ptr_glXCreateGLXPixmap glXCreateGLXPixmap;
	ptr_glXDestroyContext glXDestroyContext;
	ptr_glXDestroyGLXPixmap glXDestroyGLXPixmap;
	ptr_glXGetConfig glXGetConfig;
	ptr_glXGetCurrentContext glXGetCurrentContext;
	ptr_glXGetCurrentDrawable glXGetCurrentDrawable;
	ptr_glXIsDirect glXIsDirect;
	ptr_glXMakeCurrent glXMakeCurrent;
	ptr_glXQueryExtension glXQueryExtension;
	ptr_glXQueryVersion glXQueryVersion;
	ptr_glXSwapBuffers glXSwapBuffers;
	ptr_glXUseXFont glXUseXFont;
	ptr_glXWaitGL glXWaitGL;
	ptr_glXWaitX glXWaitX;
	ptr_glXGetClientString glXGetClientString;
	ptr_glXQueryServerString glXQueryServerString;
	ptr_glXQueryExtensionsString glXQueryExtensionsString;
	ptr_glXGetCurrentDisplay glXGetCurrentDisplay;
	ptr_glXChooseFBConfig glXChooseFBConfig;
	ptr_glXCreateNewContext glXCreateNewContext;
	ptr_glXCreatePbuffer glXCreatePbuffer;
	ptr_glXCreatePixmap glXCreatePixmap;
	ptr_glXCreateWindow glXCreateWindow;
	ptr_glXDestroyPbuffer glXDestroyPbuffer;
	ptr_glXDestroyPixmap glXDestroyPixmap;
	ptr_glXDestroyWindow glXDestroyWindow;
	ptr_glXGetCurrentReadDrawable glXGetCurrentReadDrawable;
	ptr_glXGetFBConfigAttrib glXGetFBConfigAttrib;
	ptr_glXGetFBConfigs glXGetFBConfigs;
	ptr_glXGetSelectedEvent glXGetSelectedEvent;
	ptr_glXGetVisualFromFBConfig glXGetVisualFromFBConfig;
	ptr_glXMakeContextCurrent glXMakeContextCurrent;
	ptr_glXQueryContext glXQueryContext;
	ptr_glXQueryDrawable glXQueryDrawable;
	ptr_glXSelectEvent glXSelectEvent;
	ptr_glXGetProcAddress glXGetProcAddress;
	ptr_glXGetProcAddressARB glXGetProcAddressARB;
};

class OpenGLContext : public IOpenGLContext
{
public:
	OpenGLContext(void* display, void* window);
	~OpenGLContext();

	void MakeCurrent() override;
	void ClearCurrent() override;
	void SwapBuffers() override;
	bool IsCurrent() override;

	int GetWidth() const override;
	int GetHeight() const override;

	bool IsValid() const { return opengl_context != 0; }

private:
	void CreateContext();

	::Display* disp = nullptr;
	::Window window = 0;
	GLXContext opengl_context = 0;

	XVisualInfo* opengl_visual_info = nullptr;
	GLXFBConfig fbconfig;

	GL_GLXFunctions glx;

	typedef void(ProcAddress)();
	ProcAddress* get_proc_address(const char *function_name) const;

	GLXContext create_context_glx_1_3(GLXContext shared_context);
	void create_glx_1_3(::Display* disp);
	
	bool is_glx_extension_supported(const char* ext_name);
	
	int major_version = 3;
	int minor_version = 2;
	
	void* opengl_lib_handle = nullptr;
};

GL_GLXFunctions glx_global;

#include <cstdio>

#define GL_USE_DLOPEN // Using dlopen for linux by default

#ifdef GL_USE_DLOPEN
#define GL_OPENGL_LIBRARY "libGL.so.1"
#include <dlfcn.h>
#endif

#ifdef GL_USE_DLOPEN
#define GL_LOAD_GLFUNC(x) dlsym(opengl_lib_handle, # x)
#else
#define GL_LOAD_GLFUNC(x) &x
#endif

OpenGLContext::OpenGLContext(void* display, void* window) : disp((::Display*)display), window((::Window)window)
{
	try
	{
		CreateContext();
		glx_global = glx;
	}
	catch (const std::exception& e)
	{
		// to do: maybe provide a way to query what the creation error was
	}

	if (opengl_context)
	{
		MakeCurrent();
		static OpenGLLoadFunctions loadFunctions;
		ClearCurrent();
	}
}

OpenGLContext::~OpenGLContext()
{
	if (opengl_visual_info)
	{
		XFree(opengl_visual_info);
		opengl_visual_info = nullptr;
	}

	if (opengl_context)
	{
		if (glx.glXGetCurrentContext() == opengl_context)
			ClearCurrent();

		if (disp)
			glx.glXDestroyContext(disp, opengl_context);

		opengl_context = nullptr;
	}
}

void OpenGLContext::MakeCurrent()
{
	glx.glXMakeCurrent(disp, window, opengl_context);
}

bool OpenGLContext::IsCurrent()
{
	return (glx.glXGetCurrentContext() == opengl_context);
}

void OpenGLContext::ClearCurrent()
{
	glx.glXMakeCurrent(0, 0, opengl_context);
}

void OpenGLContext::SwapBuffers()
{
	glx.glXSwapBuffers(disp, window);
}

int OpenGLContext::GetWidth() const
{
	::Window root_window;
	int x, y;
	unsigned int width, height, border_width, depth;
	Status result = XGetGeometry(disp, window, &root_window, &x, &y, &width, &height, &border_width, &depth);
	return (result != 0) ? width: 0;
}

int OpenGLContext::GetHeight() const
{
	::Window root_window;
	int x, y;
	unsigned int width, height, border_width, depth;
	Status result = XGetGeometry(disp, window, &root_window, &x, &y, &width, &height, &border_width, &depth);
	return (result != 0) ? height : 0;
}

OpenGLContext::ProcAddress* OpenGLContext::get_proc_address(const char *function_name) const
{
	if (glx.glXGetProcAddressARB)
		return glx.glXGetProcAddressARB((GLubyte*)function_name);
	else if (glx.glXGetProcAddress)
		return glx.glXGetProcAddress((GLubyte*)function_name);
	else
		return nullptr;
}

void OpenGLContext::CreateContext()
{
#ifdef GL_USE_DLOPEN
	opengl_lib_handle = dlopen(GL_OPENGL_LIBRARY, RTLD_NOW | RTLD_GLOBAL);
	if (!opengl_lib_handle)
		throw std::runtime_error(std::string("Cannot open opengl library: ") + GL_OPENGL_LIBRARY);
#endif

	glx.glXChooseVisual = (GL_GLXFunctions::ptr_glXChooseVisual) GL_LOAD_GLFUNC(glXChooseVisual);
	glx.glXCopyContext = (GL_GLXFunctions::ptr_glXCopyContext) GL_LOAD_GLFUNC(glXCopyContext);
	glx.glXCreateContext = (GL_GLXFunctions::ptr_glXCreateContext) GL_LOAD_GLFUNC(glXCreateContext);
	glx.glXCreateGLXPixmap = (GL_GLXFunctions::ptr_glXCreateGLXPixmap) GL_LOAD_GLFUNC(glXCreateGLXPixmap);
	glx.glXDestroyContext = (GL_GLXFunctions::ptr_glXDestroyContext) GL_LOAD_GLFUNC(glXDestroyContext);
	glx.glXDestroyGLXPixmap = (GL_GLXFunctions::ptr_glXDestroyGLXPixmap) GL_LOAD_GLFUNC(glXDestroyGLXPixmap);
	glx.glXGetConfig = (GL_GLXFunctions::ptr_glXGetConfig) GL_LOAD_GLFUNC(glXGetConfig);
	glx.glXGetCurrentContext = (GL_GLXFunctions::ptr_glXGetCurrentContext) GL_LOAD_GLFUNC(glXGetCurrentContext);
	glx.glXGetCurrentDrawable = (GL_GLXFunctions::ptr_glXGetCurrentDrawable) GL_LOAD_GLFUNC(glXGetCurrentDrawable);
	glx.glXIsDirect = (GL_GLXFunctions::ptr_glXIsDirect) GL_LOAD_GLFUNC(glXIsDirect);
	glx.glXMakeCurrent = (GL_GLXFunctions::ptr_glXMakeCurrent) GL_LOAD_GLFUNC(glXMakeCurrent);
	glx.glXQueryExtension = (GL_GLXFunctions::ptr_glXQueryExtension) GL_LOAD_GLFUNC(glXQueryExtension);
	glx.glXQueryVersion = (GL_GLXFunctions::ptr_glXQueryVersion) GL_LOAD_GLFUNC(glXQueryVersion);
	glx.glXSwapBuffers = (GL_GLXFunctions::ptr_glXSwapBuffers) GL_LOAD_GLFUNC(glXSwapBuffers);
	glx.glXUseXFont = (GL_GLXFunctions::ptr_glXUseXFont) GL_LOAD_GLFUNC(glXUseXFont);
	glx.glXWaitGL = (GL_GLXFunctions::ptr_glXWaitGL) GL_LOAD_GLFUNC(glXWaitGL);
	glx.glXWaitX = (GL_GLXFunctions::ptr_glXWaitX) GL_LOAD_GLFUNC(glXWaitX);
	glx.glXGetClientString = (GL_GLXFunctions::ptr_glXGetClientString) GL_LOAD_GLFUNC(glXGetClientString);
	glx.glXQueryServerString = (GL_GLXFunctions::ptr_glXQueryServerString) GL_LOAD_GLFUNC(glXQueryServerString);
	glx.glXQueryExtensionsString = (GL_GLXFunctions::ptr_glXQueryExtensionsString) GL_LOAD_GLFUNC(glXQueryExtensionsString);
	glx.glXGetCurrentDisplay = (GL_GLXFunctions::ptr_glXGetCurrentDisplay) GL_LOAD_GLFUNC(glXGetCurrentDisplay);
	glx.glXChooseFBConfig = (GL_GLXFunctions::ptr_glXChooseFBConfig) GL_LOAD_GLFUNC(glXChooseFBConfig);
	glx.glXCreateNewContext = (GL_GLXFunctions::ptr_glXCreateNewContext) GL_LOAD_GLFUNC(glXCreateNewContext);
	glx.glXCreatePbuffer = (GL_GLXFunctions::ptr_glXCreatePbuffer) GL_LOAD_GLFUNC(glXCreatePbuffer);
	glx.glXCreatePixmap = (GL_GLXFunctions::ptr_glXCreatePixmap) GL_LOAD_GLFUNC(glXCreatePixmap);
	glx.glXCreateWindow = (GL_GLXFunctions::ptr_glXCreateWindow) GL_LOAD_GLFUNC(glXCreateWindow);
	glx.glXDestroyPbuffer = (GL_GLXFunctions::ptr_glXDestroyPbuffer) GL_LOAD_GLFUNC(glXDestroyPbuffer);
	glx.glXDestroyPixmap = (GL_GLXFunctions::ptr_glXDestroyPixmap) GL_LOAD_GLFUNC(glXDestroyPixmap);
	glx.glXDestroyWindow = (GL_GLXFunctions::ptr_glXDestroyWindow) GL_LOAD_GLFUNC(glXDestroyWindow);
	glx.glXGetCurrentReadDrawable = (GL_GLXFunctions::ptr_glXGetCurrentReadDrawable) GL_LOAD_GLFUNC(glXGetCurrentReadDrawable);
	glx.glXGetFBConfigAttrib = (GL_GLXFunctions::ptr_glXGetFBConfigAttrib) GL_LOAD_GLFUNC(glXGetFBConfigAttrib);
	glx.glXGetFBConfigs = (GL_GLXFunctions::ptr_glXGetFBConfigs) GL_LOAD_GLFUNC(glXGetFBConfigs);
	glx.glXGetSelectedEvent = (GL_GLXFunctions::ptr_glXGetSelectedEvent) GL_LOAD_GLFUNC(glXGetSelectedEvent);
	glx.glXGetVisualFromFBConfig = (GL_GLXFunctions::ptr_glXGetVisualFromFBConfig) GL_LOAD_GLFUNC(glXGetVisualFromFBConfig);
	glx.glXMakeContextCurrent = (GL_GLXFunctions::ptr_glXMakeContextCurrent) GL_LOAD_GLFUNC(glXMakeContextCurrent);
	glx.glXQueryContext = (GL_GLXFunctions::ptr_glXQueryContext) GL_LOAD_GLFUNC(glXQueryContext);
	glx.glXQueryDrawable = (GL_GLXFunctions::ptr_glXQueryDrawable) GL_LOAD_GLFUNC(glXQueryDrawable);
	glx.glXSelectEvent = (GL_GLXFunctions::ptr_glXSelectEvent) GL_LOAD_GLFUNC(glXSelectEvent);

	glx.glXGetProcAddressARB = (GL_GLXFunctions::ptr_glXGetProcAddressARB) GL_LOAD_GLFUNC(glXGetProcAddressARB);
	glx.glXGetProcAddress = (GL_GLXFunctions::ptr_glXGetProcAddress) GL_LOAD_GLFUNC(glXGetProcAddress);

	if ((glx.glXDestroyContext == nullptr) ||
		(glx.glXMakeCurrent == nullptr) ||
		(glx.glXGetCurrentContext == nullptr) ||
		(glx.glXChooseVisual == nullptr) ||
		(glx.glXIsDirect == nullptr) ||
		(glx.glXGetConfig == nullptr) ||
		(glx.glXQueryExtensionsString == nullptr) ||
		(glx.glXQueryVersion == nullptr) ||
		(glx.glXGetVisualFromFBConfig == nullptr) ||
		(glx.glXCreateNewContext == nullptr) ||
		(glx.glXCreateContext == nullptr))
	{
		throw std::runtime_error("Cannot obtain required OpenGL GLX functions");
	}

	if ((glx.glXGetProcAddressARB == nullptr) && (glx.glXGetProcAddress == nullptr))
	{
		throw std::runtime_error("Cannot obtain required OpenGL GLX functions");
	}

	// FBConfigs were added in GLX version 1.3.
	int glx_major, glx_minor;
	if (!glx.glXQueryVersion(disp, &glx_major, &glx_minor) || ((glx_major == 1) && (glx_minor < 3)) || (glx_major < 1))
		throw std::runtime_error("GLX 1.3 or better is required");

	create_glx_1_3(disp);

	//if (!glx.glXIsDirect(disp, opengl_context))
	//	printf("No hardware acceleration available. I hope you got a really fast machine.\n");
}

void OpenGLContext::create_glx_1_3(::Display* disp)
{
	if (glx.glXChooseFBConfig == nullptr)
		throw std::runtime_error("Cannot find the glXChooseFBConfig function");

	// Setup OpenGL:
	int gl_attribs_single[] =
	{
		GLX_X_RENDERABLE, True,
		//GLX_RENDER_TYPE, GLX_RGBA_BIT,
		GLX_DEPTH_SIZE, 16,
		GLX_STENCIL_SIZE, 8,
		GLX_BUFFER_SIZE, 24,
		None
	};

	std::vector<int> gl_attribs;
	gl_attribs.reserve(64);

	gl_attribs.push_back(GLX_X_RENDERABLE);
	gl_attribs.push_back(True);
	gl_attribs.push_back(GLX_DRAWABLE_TYPE);
	gl_attribs.push_back(GLX_WINDOW_BIT);
	//gl_attribs.push_back(GLX_RENDER_TYPE);
	//gl_attribs.push_back(GLX_RGBA_BIT);
	gl_attribs.push_back(GLX_X_VISUAL_TYPE);
	gl_attribs.push_back(GLX_TRUE_COLOR);
	gl_attribs.push_back(GLX_RED_SIZE);
	gl_attribs.push_back(8);
	gl_attribs.push_back(GLX_GREEN_SIZE);
	gl_attribs.push_back(8);
	gl_attribs.push_back(GLX_BLUE_SIZE);
	gl_attribs.push_back(8);
	gl_attribs.push_back(GLX_ALPHA_SIZE);
	gl_attribs.push_back(0);
	gl_attribs.push_back(GLX_DEPTH_SIZE);
	gl_attribs.push_back(24);
	gl_attribs.push_back(GLX_STENCIL_SIZE);
	gl_attribs.push_back(8);
	gl_attribs.push_back(GLX_DOUBLEBUFFER);
	gl_attribs.push_back(True);
	gl_attribs.push_back(GLX_STEREO);
	gl_attribs.push_back(False);
	gl_attribs.push_back(None);

	// get an appropriate visual
	int fb_count;
	GLXFBConfig* fbc = glx.glXChooseFBConfig(disp, DefaultScreen(disp), &gl_attribs[0], &fb_count);

	if (!fbc)
	{
		printf("Requested visual not supported by your OpenGL implementation. Falling back on singlebuffered Visual!\n");
		fbc = glx.glXChooseFBConfig(disp, DefaultScreen(disp), gl_attribs_single, &fb_count);
		if (!fbc)
			throw std::runtime_error(" glxChooseFBConfig failed");
		fbconfig = fbc[0];
	}
	else
	{
		if (!glx.glXGetFBConfigAttrib)
			throw std::runtime_error("Cannot find function glXGetFBConfigAttrib");

		int required_samples = 1; // desc.multisampling();
		int desired_config = 0;
		int max_sample_buffers = 0;
		int max_samples = 0;
		// Find the best fitting multisampling option
		for (int i = 0; i < fb_count; i++)
		{
			int samp_buf, samples;
			glx.glXGetFBConfigAttrib(disp, fbc[i], GLX_SAMPLE_BUFFERS, &samp_buf);
			glx.glXGetFBConfigAttrib(disp, fbc[i], GLX_SAMPLES, &samples);

			// Samples are most important, because they are variable
			if (max_samples < required_samples)
			{
				if (samples > max_samples)
				{
					max_samples = samples;
					desired_config = i;
				}
			}

			// Use the maximum sample buffer
			if (max_samples == samples)	// Only check if the sample is valid
			{
				if (max_sample_buffers < samp_buf)
				{
					max_sample_buffers = samp_buf;
					desired_config = i;
				}
			}
		}
		fbconfig = fbc[desired_config];
	}

	XFree(fbc);

	if (opengl_visual_info) XFree(opengl_visual_info);
	opengl_visual_info = glx.glXGetVisualFromFBConfig(disp, fbconfig);
	if (opengl_visual_info == nullptr)
	{
		throw std::runtime_error("glXGetVisualFromFBConfig failed");
	}

	// create a GLX context
	opengl_context = create_context_glx_1_3(nullptr);
}

bool OpenGLContext::is_glx_extension_supported(const char* ext_name)
{
	const char* ext_string = glx.glXQueryExtensionsString(disp, opengl_visual_info->screen);
	if (ext_string)
	{
		const char* start;
		const char* where, * terminator;

		// Extension names should not have spaces.
		where = strchr(ext_name, ' ');
		if (where || *ext_name == '\0')
			return false;

		int ext_len = strlen(ext_name);

		// It takes a bit of care to be fool-proof about parsing the OpenGL extensions string. Don't be fooled by sub-strings, etc.
		for (start = ext_string; ; )
		{
			where = strstr(start, ext_name);

			if (!where)
				break;

			terminator = where + ext_len;

			if (where == start || *(where - 1) == ' ')
				if (*terminator == ' ' || *terminator == '\0')
					return true;

			start = terminator;
		}
	}
	return false;
}

static bool cl_ctxErrorOccurred = false;
static int cl_ctxErrorHandler(::Display* dpy, XErrorEvent* ev)
{
	cl_ctxErrorOccurred = true;
	return 0;
}

GLXContext OpenGLContext::create_context_glx_1_3(GLXContext shared_context)
{
	GLXContext context;

	context = glx.glXCreateNewContext(disp, fbconfig, GLX_RGBA_TYPE, shared_context, True);
	if (context == nullptr)
		throw std::runtime_error("glXCreateContext failed");

	ptr_glXCreateContextAttribs glXCreateContextAttribs = nullptr;

	if (is_glx_extension_supported("GLX_ARB_create_context"))
	{
		if (glx.glXGetProcAddressARB)
			glXCreateContextAttribs = (ptr_glXCreateContextAttribs)glx.glXGetProcAddressARB((GLubyte*) "glXCreateContextAttribsARB");
		if (glx.glXGetProcAddress)
			glXCreateContextAttribs = (ptr_glXCreateContextAttribs)glx.glXGetProcAddress((GLubyte*) "glXCreateContextAttribsARB");
	}

	if (glXCreateContextAttribs)
	{
		// Install an X error handler so the application won't exit if GL 3.0 context allocation fails.
		//
		// Note this error handler is global.  All display connections in all threads
		// of a process use the same error handler, so be sure to guard against other
		// threads issuing X commands while this code is running.
		int (*oldHandler)(::Display*, XErrorEvent*) = XSetErrorHandler(&cl_ctxErrorHandler);

		GLXContext context_gl3 = 0;
		for (int profile : { 1/*GLX_CONTEXT_CORE_PROFILE_BIT_ARB*/, 2 /*GLX_CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB*/ })
		{
			for (int version : { 46, 45, 44, 43, 42, 41, 40, 33, 32 })
			{
				std::vector<int> int_attributes;
				int_attributes.push_back(0x2091);	// GLX_CONTEXT_MAJOR_VERSION_ARB
				int_attributes.push_back(version / 10);
				int_attributes.push_back(0x2092);	// GLX_CONTEXT_MINOR_VERSION_ARB
				int_attributes.push_back(version % 10);
				int_attributes.push_back(0x9126);	// GLX_CONTEXT_PROFILE_MASK_ARB
				int_attributes.push_back(profile);
				int_attributes.push_back(None);

				cl_ctxErrorOccurred = false;

				context_gl3 = glXCreateContextAttribs(disp, fbconfig, shared_context, True, int_attributes.data());

				if (cl_ctxErrorOccurred && context_gl3)
				{
					glx.glXDestroyContext(disp, context_gl3);
					context_gl3 = nullptr;
				}

				if (context_gl3)
					break;
			}

			if (context_gl3)
				break;
		}

		// Restore the original error handler
		XSetErrorHandler(oldHandler);

		if (context_gl3)
		{
			glx.glXDestroyContext(disp, context);
			context = context_gl3;
		}
	}
	return context;
}

std::unique_ptr<IOpenGLContext> IOpenGLContext::Create(void* disp, void* window)
{
	auto ctx = std::make_unique<OpenGLContext>(disp, window);
	if (!ctx->IsValid()) return nullptr;
	return ctx;
}

void* GL_GetProcAddress(const char* function_name)
{
	if (glx_global.glXGetProcAddressARB)
		return (void*)glx_global.glXGetProcAddressARB((GLubyte*)function_name);
	else if (glx_global.glXGetProcAddress)
		return (void*)glx_global.glXGetProcAddress((GLubyte*)function_name);
	else
		return nullptr;
}

#endif
