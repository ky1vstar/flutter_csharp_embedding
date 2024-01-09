using System.Collections.Generic;

namespace Flutter.Embedding
{
  public class DartProject
  {
    public DartProject(string assetsPath, string icuDataPath, string aotLibraryPath)
    {
      AssetsPath = assetsPath;
      IcuDataPath = icuDataPath;
      AotLibraryPath = aotLibraryPath;
    }

    public DartProject(string path)
      : this(path + "\\flutter_assets", path + "\\icudtl.dat", path + "\\app.so")
    {
    }

    internal string AssetsPath { get; }
    internal string IcuDataPath { get; }
    internal string AotLibraryPath { get; }
    public string DartEntrypoint;
    public List<string> DartEntrypointArguments;
  }
}
