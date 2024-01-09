cmake_minimum_required(VERSION 3.15)
project(flutter_csharp_glue LANGUAGES CXX)

add_library(flutter_csharp_glue SHARED
  "flutter_csharp_glue.cpp"
  "${FLUTTER_MANAGED_DIR}/generated_plugin_registrant.cc"
)
apply_standard_settings(flutter_csharp_glue)
target_compile_definitions(flutter_csharp_glue PRIVATE "NOMINMAX")
target_link_libraries(flutter_csharp_glue PRIVATE flutter flutter_wrapper_app)
target_include_directories(flutter_csharp_glue PRIVATE "${CMAKE_SOURCE_DIR}")
add_dependencies(flutter_csharp_glue flutter_assemble)

# Link plugins to C# glue
foreach(plugin ${FLUTTER_PLUGIN_LIST})
  target_link_libraries(flutter_csharp_glue PRIVATE ${plugin}_plugin)
endforeach(plugin)

# === Installation ===
install(TARGETS flutter_csharp_glue RUNTIME DESTINATION "${CMAKE_INSTALL_PREFIX}"
  COMPONENT Runtime)
