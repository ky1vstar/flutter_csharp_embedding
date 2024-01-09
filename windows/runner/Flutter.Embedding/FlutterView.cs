using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Flutter.Embedding.Interop;

namespace Flutter.Embedding
{
  public class FlutterView
  {
    public FlutterView(FlutterDesktopView view) 
    {
      View = view;
    }

    private FlutterDesktopView View;

    public IntPtr GetNativeWindow()
    {
      return FlutterDesktopViewGetHWND(View);
    }
  }
}
