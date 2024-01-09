using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Flutter.Embedding.Interop;

namespace Flutter.Embedding
{
  public class FlutterEngine : IPluginRegistry
  {
    public FlutterEngine(DartProject project) {
      var entrypointArguments = project.DartEntrypointArguments ?? new List<string>();
      using (var entrypointArgs = new StringArray(entrypointArguments))
      {
        var engineProperties = new FlutterDesktopEngineProperties
        {
          assets_path = project.AssetsPath,
          icu_data_path = project.IcuDataPath,
          aot_library_path = project.AotLibraryPath,
          dart_entrypoint = project.DartEntrypoint,
          dart_entrypoint_argc = entrypointArgs.Length,
          dart_entrypoint_argv = entrypointArgs.Handle,
        };

        Engine = FlutterDesktopEngineCreate(ref engineProperties);
      }

      Messenger = new DefaultBinaryMessenger(FlutterDesktopEngineGetMessenger(Engine));
    }

    /// <summary>
    /// Whether the engine is valid or not.
    /// </summary>
    public bool IsValid => !Engine.IsInvalid;

    /// <summary>
    /// Handle for interacting with the C API's engine reference.
    /// </summary>
    protected internal FlutterDesktopEngine Engine { get; private set; } = new FlutterDesktopEngine();

    public IBinaryMessenger Messenger { get; }

    private bool HasBeenRun = false;
    private bool OwnsEngine = true;

    public bool Run()
    {
      return Run(null);
    }

    public bool Run(string entryPoint)
    {
      if (!IsValid)
      {
        Trace.TraceWarning("Cannot run an engine that failed creation.");
        return false;
      }
      if (HasBeenRun)
      {
        Trace.TraceWarning("Cannot run an engine more than once.");
        return false;
      }
      var runSucceeded = FlutterDesktopEngineRun(Engine, entryPoint);
      if (!runSucceeded)
      {
        Trace.TraceWarning("Failed to start engine.");
      }
      HasBeenRun = true;
      return runSucceeded;
    }

    public void ShutDown()
    {
      if (IsValid && OwnsEngine)
      {
        FlutterDesktopEngineDestroy(Engine);
      }
      Engine = new FlutterDesktopEngine();
    }

    public TimeSpan ProcessMessages()
    {
      var nanoseconds = FlutterDesktopEngineProcessMessages(Engine);
      return TimeSpan.FromTicks((long)(nanoseconds / 100));
    }

    public void ReloadSystemFonts() 
    {
      FlutterDesktopEngineReloadSystemFonts(Engine);
    } 

    public FlutterDesktopPluginRegistrar GetRegistrarForPlugin(string pluginName)
    {
      if (IsValid)
      {
        return FlutterDesktopEngineGetPluginRegistrar(Engine, pluginName);
      }
      Trace.TraceWarning("Cannot get plugin registrar on an engine that isn't running; call Run first.");
      return new FlutterDesktopPluginRegistrar();
    }

    internal FlutterDesktopEngine RelinquishEngine() 
    {
      OwnsEngine = false;
      return Engine;
    }
  }
}
