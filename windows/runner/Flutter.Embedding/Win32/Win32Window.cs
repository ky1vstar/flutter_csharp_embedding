#pragma warning disable CA1416

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Flutter.Embedding.Interop;
using static Windows.Win32.PInvoke;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;

namespace Flutter.Embedding.Win32
{
  public class Win32Window
  {
    private delegate bool EnableNonClientDpiScaling(IntPtr hwnd);

    private static int gActiveWindowCount = 0;

    private static readonly WindowClassRegistrar ClassRegistrar = new WindowClassRegistrar();

    private bool quitOnClose = false;
    private HWND windowHandle = HWND.Null;
    private HWND childContent = HWND.Null;

    public Win32Window()
    {
      gActiveWindowCount++;
    }

    ~Win32Window()
    {
      gActiveWindowCount--;
      Destroy();
    }

    public void RunMessageLoop()
    {
      unsafe
      {
        MSG msg;
        while (GetMessage(&msg, HWND.Null, 0, 0))
        {
          TranslateMessage(&msg);
          DispatchMessage(&msg);
        }
      }
    }

    public bool CreateAndShow(string title, Point origin, Size size)
    {
      Destroy();

      string windowClass = ClassRegistrar.GetWindowClass();

      var monitor = MonitorFromPoint(origin, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
      var dpi = FlutterDesktopGetDpiForMonitor(monitor);
      var scaleFactor = dpi / 96.0;

      var thisHandle = GCHandle.Alloc(this);
      unsafe
      {
        windowHandle = CreateWindowEx(
          0, windowClass, title, WINDOW_STYLE.WS_OVERLAPPEDWINDOW | WINDOW_STYLE.WS_VISIBLE,
          Scale(origin.X, scaleFactor), Scale(origin.Y, scaleFactor),
          Scale(size.Width, scaleFactor), Scale(size.Height, scaleFactor),
          HWND.Null, null, GetModuleHandle((string)null), (void*)GCHandle.ToIntPtr(thisHandle));
      }

      if (windowHandle == IntPtr.Zero)
      {
        return false;
      }

      return OnCreate();
    }

    public IntPtr GetHandle()
    {
      return windowHandle;
    }

    public void SetQuitOnClose(bool quitOnClose)
    {
      this.quitOnClose = quitOnClose;
    }

    public Rectangle GetClientArea()
    {
      RECT frame;
      GetClientRect(windowHandle, out frame);
      return frame;
    }

    public void SetChildContent(IntPtr content)
    {
      childContent = (HWND)content;
      SetParent(childContent, windowHandle);
      RECT frame = GetClientArea();

      MoveWindow(childContent, frame.left, frame.top, frame.right - frame.left,
          frame.bottom - frame.top, true);

      SetFocus(childContent);
    }

    public void Destroy()
    {
      OnDestroy();

      if (!windowHandle.IsNull)
      {
        DestroyWindow(windowHandle);
        windowHandle = HWND.Null;
      }

      if (gActiveWindowCount == 0)
      {
        ClassRegistrar.UnregisterWindowClass();
      }
    }

    private static void EnableFullDpiSupportIfAvailable(IntPtr hwnd)
    {
      var user32Module = LoadLibrary("User32.dll");
      if (user32Module.IsInvalid)
      {
        return;
      }

      IntPtr enableNonClientDpiScaling = GetProcAddress(user32Module, "EnableNonClientDpiScaling");
      if (enableNonClientDpiScaling != IntPtr.Zero)
      { 
        Marshal.GetDelegateForFunctionPointer<EnableNonClientDpiScaling>(enableNonClientDpiScaling)(hwnd);
        FreeLibrary((HMODULE)user32Module.DangerousGetHandle());
      }
    }

    internal unsafe static LRESULT WndProc(HWND hwnd, uint msg, WPARAM wParam, LPARAM lParam)
    {
      if (msg == WM_NCCREATE)
      {
        var windowStruct = (CREATESTRUCTW)Marshal.PtrToStructure(lParam, typeof(CREATESTRUCTW));
        SetWindowLongPtr(hwnd, WINDOW_LONG_PTR_INDEX.GWLP_USERDATA, (IntPtr)windowStruct.lpCreateParams);

        var thatHandle = GCHandle.FromIntPtr((IntPtr)windowStruct.lpCreateParams);
        var that = (Win32Window)thatHandle.Target;
        EnableFullDpiSupportIfAvailable(hwnd);
        that.windowHandle = hwnd;
      }
      else if (Win32Window.GetThisFromHandle(hwnd) is Win32Window that)
      {
        return that.MessageHandler(hwnd, msg, wParam, lParam);
      }

      return DefWindowProc(hwnd, msg, wParam, lParam);
    }

    internal virtual LRESULT MessageHandler(HWND hwnd, uint message, WPARAM wparam, LPARAM lparam)
    {
      switch (message)
      {
        case WM_DESTROY:
          windowHandle = HWND.Null;
          Destroy();
          if (quitOnClose)
          {
            PostQuitMessage(0);
          }
          return new LRESULT();

        case WM_DPICHANGED:
          RECT newRectSize = (RECT)Marshal.PtrToStructure(lparam, typeof(RECT));
          int newWidth = newRectSize.right - newRectSize.left;
          int newHeight = newRectSize.bottom - newRectSize.top;

          SetWindowPos(hwnd, HWND.Null, newRectSize.left, newRectSize.top, newWidth,
              newHeight, SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);

          return new LRESULT();

        case WM_SIZE:
          RECT rect = GetClientArea();
          if (!childContent.IsNull)
          {
            // Size and position the child window.
            MoveWindow(childContent, rect.left, rect.top, rect.right - rect.left,
                rect.bottom - rect.top, true);
          }
          return new LRESULT();

        case WM_ACTIVATE:
          if (!childContent.IsNull)
          {
            SetFocus(childContent);
          }
          return new LRESULT();
      }

      return DefWindowProc(windowHandle, message, wparam, lparam);
    }

    private static int Scale(int source, double scaleFactor)
    {
      return (int)(source * scaleFactor);
    }

    public virtual bool OnCreate()
    {
      // No-op; provided for subclasses.
      return true;
    }

    public virtual void OnDestroy()
    {
      // No-op; provided for subclasses.
    }

    private static Win32Window GetThisFromHandle(HWND window)
    {
      var ptr = GetWindowLongPtr(window, WINDOW_LONG_PTR_INDEX.GWLP_USERDATA);
      if (ptr == IntPtr.Zero)
      {
        return null;
      }
      var thisHandle = GCHandle.FromIntPtr(ptr);
      return (Win32Window)thisHandle.Target;
    }
  }

  public class WindowClassRegistrar
  {
    private static readonly string WindowClassName = "FLUTTER_RUNNER_WIN32_WINDOW";
    private static WindowClassRegistrar instance;
    private bool classRegistered = false;

    public static WindowClassRegistrar GetInstance()
    {
      if (instance == null)
      {
        instance = new WindowClassRegistrar();
      }
      return instance;
    }

    public string GetWindowClass()
    {
      if (!classRegistered)
      {
        unsafe
        {
          fixed (char* windowClassNamePtr = WindowClassName)
          {
            var windowClass = new WNDCLASSW();
            windowClass.hCursor = LoadCursor(HINSTANCE.Null, IDC_ARROW);
            windowClass.lpszClassName = windowClassNamePtr;
            windowClass.style = WNDCLASS_STYLES.CS_HREDRAW | WNDCLASS_STYLES.CS_VREDRAW;
            windowClass.cbClsExtra = 0;
            windowClass.cbWndExtra = 0;
            windowClass.hInstance = GetModuleHandle(new PCWSTR());
            //windowClass.hIcon = LoadIcon(windowClass.hInstance, (IntPtr)ResourceIdentifier.IDI_APP_ICON);
            windowClass.hbrBackground = HBRUSH.Null;
            windowClass.lpszMenuName = new PCWSTR();
            windowClass.lpfnWndProc = Win32Window.WndProc;

            RegisterClass(windowClass);
          }
        }
        classRegistered = true;
      }

      return WindowClassName;
    }

    public void UnregisterWindowClass()
    {
      UnregisterClass(WindowClassName, GetModuleHandle((string)null));
      classRegistered = false;
    }
  }
}
