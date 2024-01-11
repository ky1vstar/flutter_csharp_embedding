// Copyright 2020 Samsung Electronics Co., Ltd. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using Flutter.Embedding;
using System;
using System.Runtime.InteropServices;

namespace Flutter.Embedding
{
  internal static class Interop
  {
    #region flutter_windows.h
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void VoidCallback(IntPtr user_data);

    [StructLayout(LayoutKind.Sequential)]
    public struct FlutterDesktopEngineProperties
    {
      [MarshalAs(UnmanagedType.LPWStr)]
      public string assets_path;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string icu_data_path;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string aot_library_path;
      [MarshalAs(UnmanagedType.LPStr)]
      public string dart_entrypoint;
      public int dart_entrypoint_argc;
      public IntPtr dart_entrypoint_argv;
    }

    [DllImport("flutter_windows.dll")]
    public static extern FlutterDesktopViewController FlutterDesktopViewControllerCreate(
        int width,
        int height,
        FlutterDesktopEngine engine);

    [DllImport("flutter_windows.dll")]
    public static extern void FlutterDesktopViewControllerDestroy(FlutterDesktopViewController controller);

    [DllImport("flutter_windows.dll")]
    public static extern FlutterDesktopEngine FlutterDesktopViewControllerGetEngine(
        FlutterDesktopViewController controller);

    [DllImport("flutter_windows.dll")]
    public static extern FlutterDesktopView FlutterDesktopViewControllerGetView(
        FlutterDesktopViewController controller);

    [DllImport("flutter_windows.dll")]
    public static extern void FlutterDesktopViewControllerForceRedraw(FlutterDesktopViewController controller);

    [DllImport("flutter_windows.dll", CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool FlutterDesktopViewControllerHandleTopLevelWindowProc(
        FlutterDesktopViewController controller, IntPtr hwnd, uint message,
        UIntPtr wparam, IntPtr lparam, out IntPtr result);

    [DllImport("flutter_windows.dll")]
    public static extern FlutterDesktopEngine FlutterDesktopEngineCreate(
        ref FlutterDesktopEngineProperties engine_properties);

    [DllImport("flutter_windows.dll")]
    public static extern void FlutterDesktopEngineDestroy(FlutterDesktopEngine engine);

    [DllImport("flutter_windows.dll")]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool FlutterDesktopEngineRun(
        FlutterDesktopEngine engine, [MarshalAs(UnmanagedType.LPStr)] string entry_point);

    [DllImport("flutter_windows.dll")]
    public static extern UInt64 FlutterDesktopEngineProcessMessages(FlutterDesktopEngine engine);

    [DllImport("flutter_windows.dll")]
    public static extern void FlutterDesktopEngineReloadSystemFonts(FlutterDesktopEngine engine);

    [DllImport("flutter_windows.dll")]
    public static extern FlutterDesktopPluginRegistrar FlutterDesktopEngineGetPluginRegistrar(
        FlutterDesktopEngine engine, [MarshalAs(UnmanagedType.LPStr)] string plugin_name);

    [DllImport("flutter_windows.dll")]
    public static extern FlutterDesktopMessenger FlutterDesktopEngineGetMessenger(FlutterDesktopEngine engine);

    [DllImport("flutter_windows.dll")]
    public static extern FlutterDesktopTextureRegistrar FlutterDesktopEngineGetTextureRegistrar(
        FlutterDesktopEngine engine);

    [DllImport("flutter_windows.dll")]
    public static extern void FlutterDesktopEngineSetNextFrameCallback(
        FlutterDesktopEngine engine, VoidCallback callback, IntPtr user_data);

    [DllImport("flutter_windows.dll")]
    public static extern IntPtr FlutterDesktopViewGetHWND(FlutterDesktopView view);

    [DllImport("flutter_windows.dll")]
    public static extern uint FlutterDesktopGetDpiForHWND(IntPtr hwnd);

    [DllImport("flutter_windows.dll")]
    public static extern uint FlutterDesktopGetDpiForMonitor(IntPtr monitor);

    [DllImport("flutter_windows.dll")]
    public static extern void FlutterDesktopResyncOutputStreams();
    #endregion

    #region flutter_messenger.h
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FlutterDesktopBinaryReply(IntPtr data, uint data_size, IntPtr user_data);

    [StructLayout(LayoutKind.Sequential)]
    public struct FlutterDesktopMessage
    {
      public uint struct_size;
      [MarshalAs(UnmanagedType.LPStr)]
      public string channel;
      public IntPtr message;
      public uint message_size;
      public IntPtr response_handle;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FlutterDesktopMessageCallback(
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(FlutterDesktopMessenger.Marshaler))]
            FlutterDesktopMessenger messenger, IntPtr message, IntPtr user_data);

    [DllImport("flutter_windows.dll")]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool FlutterDesktopMessengerSend(
        FlutterDesktopMessenger messenger, [MarshalAs(UnmanagedType.LPStr)] string channel,
        IntPtr message, uint message_size);

    [DllImport("flutter_windows.dll")]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool FlutterDesktopMessengerSendWithReply(
        FlutterDesktopMessenger messenger, [MarshalAs(UnmanagedType.LPStr)] string channel, 
        IntPtr message, uint message_size, FlutterDesktopBinaryReply reply, IntPtr user_data);

    [DllImport("flutter_windows.dll")]
    public static extern void FlutterDesktopMessengerSendResponse(
        FlutterDesktopMessenger messenger, IntPtr handle, IntPtr data, uint data_length);

    [DllImport("flutter_windows.dll")]
    public static extern void FlutterDesktopMessengerSetCallback(
        FlutterDesktopMessenger messenger, [MarshalAs(UnmanagedType.LPStr)] string channel, 
        FlutterDesktopMessageCallback callback, IntPtr user_data);
    #endregion

    #region flutter_plugin_registrar.h
    [DllImport("flutter_windows.dll")]
    public static extern FlutterDesktopMessenger FlutterDesktopPluginRegistrarGetMessenger(
        FlutterDesktopPluginRegistrar registrar);

    [DllImport("flutter_windows.dll")]
    public static extern FlutterDesktopTextureRegistrar FlutterDesktopRegistrarGetTextureRegistrar(
      FlutterDesktopPluginRegistrar registrar);
    #endregion

    #region flutter_csharp_glue.h
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(FlutterDesktopPluginRegistrar.Marshaler))]
    public delegate FlutterDesktopPluginRegistrar CSharpGlueRegisterPluginsCallback(
      [MarshalAs(UnmanagedType.LPStr)] string plugin_name);

    [DllImport("flutter_csharp_glue.dll")]
    public static extern FlutterDesktopTextureRegistrar CSharpGlueRegisterPlugins(
       CSharpGlueRegisterPluginsCallback callback);
    #endregion
  }
}