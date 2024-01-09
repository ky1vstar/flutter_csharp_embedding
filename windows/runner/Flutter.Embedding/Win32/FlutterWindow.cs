using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Windows.Win32.PInvoke;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Foundation;
using Windows.Win32.Graphics.Gdi;

namespace Flutter.Embedding.Win32
{
  public class FlutterWindow : Win32Window
  {
    public FlutterWindow(DartProject project)
    {
      Project = project;
    }

    private DartProject Project;
    private FlutterViewController FlutterController;

    override public bool OnCreate()
    {
      if (!base.OnCreate())
      {
        return false;
      }

      var frame = GetClientArea();

      // The size here must match the window dimensions to avoid unnecessary surface
      // creation / destruction in the startup path.
      FlutterController = new FlutterViewController(frame.Width, frame.Height, Project);
      // Ensure that basic setup of the controller was successful.
      if (FlutterController.Engine == null || FlutterController.View == null)
      {
        return false;
      }
      Interop.CSharpGlueRegisterPlugins((pluginName) => FlutterController.Engine.GetRegistrarForPlugin(pluginName).DangerousGetHandle());
      SetChildContent(FlutterController.View.GetNativeWindow());
      return true;
    }

    public override void OnDestroy()
    {
      if (FlutterController != null)
      {
        FlutterController.Destroy();
        FlutterController = null;
      }

      base.OnDestroy();
    }

    override internal LRESULT MessageHandler(HWND hwnd, uint message, WPARAM wparam, LPARAM lparam)
    {
      if (FlutterController != null)
      {
        var result = FlutterController.HandleTopLevelWindowProc(hwnd, message, wparam, lparam);
        if (result.HasValue)
        {
          return (LRESULT)result;
        }
      }

      switch (message)
      {
        case WM_FONTCHANGE:
          FlutterController.Engine.ReloadSystemFonts();
          break;
      }

      return base.MessageHandler(hwnd, message, wparam, lparam);
    }
  }
}
