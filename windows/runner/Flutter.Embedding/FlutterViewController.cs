using System;
using System.Diagnostics;
using static Flutter.Embedding.Interop;

namespace Flutter.Embedding
{
  public class FlutterViewController
  {
    public FlutterViewController(int width, int height, DartProject project) 
    {
      Engine = new FlutterEngine(project);
      Controller = FlutterDesktopViewControllerCreate(width, height, Engine.RelinquishEngine());
      if (Controller.IsInvalid)
      {
        Trace.TraceError("Failed to create view controller.");
        return;
      }
      View = new FlutterView(FlutterDesktopViewControllerGetView(Controller));
    }

    public FlutterEngine Engine { get; }
    public FlutterView View { get; }

    private FlutterDesktopViewController Controller;

    public void Destroy()
    {
      if (!Controller.IsInvalid)
      {
        FlutterDesktopViewControllerDestroy(Controller);
      }
    }

    public void ForceRedraw()
    {
      FlutterDesktopViewControllerForceRedraw(Controller);
    }

    public IntPtr? HandleTopLevelWindowProc(IntPtr hwnd, uint message, UIntPtr wParam, IntPtr lParam)
    {
      var result = IntPtr.Zero;
      var handled = FlutterDesktopViewControllerHandleTopLevelWindowProc(Controller, hwnd, message, wParam, lParam, out result);
      return handled ? (IntPtr)result : null;
    }
  }
}
