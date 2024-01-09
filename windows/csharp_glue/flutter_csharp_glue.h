#ifndef CSHARP_GLUE_H_
#define CSHARP_GLUE_H_

#include <flutter_windows.h>

#ifdef _WIN32
#define FLUTTER_CSHARP_GLUE_EXPORT __declspec(dllexport)
#else
#define FLUTTER_CSHARP_GLUE_EXPORT __attribute__((visibility("default")))
#endif

#if defined(__cplusplus)
extern "C" {
#endif

typedef FlutterDesktopPluginRegistrarRef(*CSharpGlueRegisterPluginsCallback)(const char* /* plugin_name */);

FLUTTER_CSHARP_GLUE_EXPORT void CSharpGlueRegisterPlugins(CSharpGlueRegisterPluginsCallback delegate);

#if defined(__cplusplus)
}  // extern "C"
#endif

#endif  // CSHARP_GLUE_H_
