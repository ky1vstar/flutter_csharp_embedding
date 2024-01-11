namespace Flutter.Embedding
{
  public class FlutterPluginRegistrar
  {
    public FlutterPluginRegistrar(FlutterDesktopPluginRegistrar registrar) 
    {
      Registrar = registrar;
    }

    internal FlutterDesktopPluginRegistrar Registrar { get; }
  }
}