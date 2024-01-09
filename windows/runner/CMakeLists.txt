cmake_minimum_required(VERSION 3.23)
project(runner LANGUAGES CSharp)

# Create assembly info with the build version.
CONFIGURE_FILE("AssemblyInfo.cs.template" "${CMAKE_BINARY_DIR}/${PROJECT_NAME}/AssemblyInfo.cs")

# Define the application target. To change its name, change BINARY_NAME in the
# top-level CMakeLists.txt, not the value here, or `flutter run` will no longer
# work.
#
# Any new source files that you add to the application should be added here.
# PS: Get-ChildItem -Path $path -Recurse -Filter "*.cs" | select FullName
add_executable(${BINARY_NAME}
  "${CMAKE_BINARY_DIR}/${PROJECT_NAME}/AssemblyInfo.cs"
  Main.cs
  Flutter.Embedding/DartProject.cs
  Flutter.Embedding/FlutterEngine.cs
  Flutter.Embedding/FlutterException.cs
  Flutter.Embedding/FlutterView.cs
  Flutter.Embedding/FlutterViewController.cs
  Flutter.Embedding/IPluginRegistry.cs
  Flutter.Embedding/MissingPluginException.cs
  Flutter.Embedding/Channels/BasicMessageChannel.cs
  Flutter.Embedding/Channels/BinaryCodec.cs
  Flutter.Embedding/Channels/DefaultBinaryMessenger.cs
  Flutter.Embedding/Channels/EventChannel.cs
  Flutter.Embedding/Channels/EventStreamHandler.cs
  Flutter.Embedding/Channels/IBinaryMessenger.cs
  Flutter.Embedding/Channels/IEventSink.cs
  Flutter.Embedding/Channels/IEventStreamHandler.cs
  Flutter.Embedding/Channels/IMessageCodec.cs
  Flutter.Embedding/Channels/IMethodCodec.cs
  Flutter.Embedding/Channels/MethodCall.cs
  Flutter.Embedding/Channels/MethodChannel.cs
  Flutter.Embedding/Channels/StandardMessageCodec.cs
  Flutter.Embedding/Channels/StandardMessageHelper.cs
  Flutter.Embedding/Channels/StandardMethodCodec.cs
  Flutter.Embedding/Channels/StringCodec.cs
  Flutter.Embedding/Common/PinnedObject.cs
  Flutter.Embedding/Common/StreamExtensions.cs
  Flutter.Embedding/Common/StringArray.cs
  Flutter.Embedding/Interop/FlutterDesktopEngine.cs
  Flutter.Embedding/Interop/FlutterDesktopMessenger.cs
  Flutter.Embedding/Interop/FlutterDesktopPluginRegistrar.cs
  Flutter.Embedding/Interop/FlutterDesktopTextureRegistrar.cs
  Flutter.Embedding/Interop/FlutterDesktopView.cs
  Flutter.Embedding/Interop/FlutterDesktopViewController.cs
  Flutter.Embedding/Interop/Interop.cs
  Flutter.Embedding/Win32/FlutterWindow.cs
  Flutter.Embedding/Win32/Win32Window.cs
  Flutter.Embedding/Win32/Windows.Win32.NativeMethods.g.cs
)

# Set the .NET Framework version for the executable.
set_property(TARGET ${BINARY_NAME} PROPERTY DOTNET_SDK "Microsoft.NET.Sdk")
set_property(TARGET ${BINARY_NAME} PROPERTY DOTNET_TARGET_FRAMEWORK "net6.0")

# Set the C# language version (defaults to 3.0).
set(CMAKE_CSharp_FLAGS "/langversion:latest")

# Allow unsafe code.
target_compile_options(${BINARY_NAME} PUBLIC "/unsafe")

# Disable implicit assembly generation that collide with our assembly info.
set_property(TARGET ${BINARY_NAME} PROPERTY VS_GLOBAL_GenerateAssemblyInfo "false")

# Apply the standard set of build settings. This can be removed for applications
# that need different build settings.
apply_standard_settings(${BINARY_NAME})

# Disable Windows macros that collide with C++ standard library functions.
target_compile_definitions(${BINARY_NAME} PRIVATE "NOMINMAX")

# Add dependency libraries and include directories. Add any application-specific
# dependencies here.
target_link_libraries(${BINARY_NAME} PRIVATE flutter flutter_wrapper_app)
target_link_libraries(${BINARY_NAME} PRIVATE "dwmapi.lib")
target_include_directories(${BINARY_NAME} PRIVATE "${CMAKE_SOURCE_DIR}")

# Run the Flutter tool portions of the build. This must not be removed.
add_dependencies(${BINARY_NAME} flutter_assemble)
