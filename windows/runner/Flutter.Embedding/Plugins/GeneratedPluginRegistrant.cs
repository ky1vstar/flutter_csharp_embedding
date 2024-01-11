namespace Flutter.Embedding
{
  public static class GeneratedPluginRegistrant
  {
    public static void RegisterPlugins(IPluginRegistry registry)
    {
      Interop.CSharpGlueRegisterPlugins((pluginName) => registry.GetRegistrarForPlugin(pluginName).Registrar);
    }
  }
}