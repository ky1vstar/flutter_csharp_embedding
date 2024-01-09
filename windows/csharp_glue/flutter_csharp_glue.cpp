#include "flutter_csharp_glue.h"
#include <flutter/plugin_registry.h>
#include "flutter/generated_plugin_registrant.h"

class DelegatedPluginRegistry : public flutter::PluginRegistry {
  public:
    explicit DelegatedPluginRegistry(CSharpGlueRegisterPluginsCallback delegate);

    ~DelegatedPluginRegistry() = default;

    FlutterDesktopPluginRegistrarRef GetRegistrarForPlugin(
      const std::string& plugin_name) override;

  private:
    CSharpGlueRegisterPluginsCallback delegate_;
};

DelegatedPluginRegistry::DelegatedPluginRegistry(CSharpGlueRegisterPluginsCallback delegate) {
  delegate_ = delegate;
}

FlutterDesktopPluginRegistrarRef DelegatedPluginRegistry::GetRegistrarForPlugin(
  const std::string& plugin_name) {
  return delegate_(plugin_name.c_str());
}

FLUTTER_CSHARP_GLUE_EXPORT void CSharpGlueRegisterPlugins(CSharpGlueRegisterPluginsCallback delegate) {
  DelegatedPluginRegistry registry(delegate);
  RegisterPlugins(&registry);
}