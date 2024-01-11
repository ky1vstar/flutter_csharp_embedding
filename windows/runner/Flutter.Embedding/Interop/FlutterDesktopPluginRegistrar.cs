// Copyright 2020 Samsung Electronics Co., Ltd. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

#pragma warning disable 1591

using System;
using System.Runtime.InteropServices;

namespace Flutter.Embedding
{
  public class FlutterDesktopPluginRegistrar : SafeHandle
  {
    public FlutterDesktopPluginRegistrar() : base(IntPtr.Zero, true)
    {
    }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
      SetHandle(IntPtr.Zero);
      return true;
    }

    internal class Marshaler : ICustomMarshaler
    {
      private static readonly Marshaler _instance = new Marshaler();

      public void CleanUpManagedData(object ManagedObj)
      {
      }

      public void CleanUpNativeData(IntPtr pNativeData)
      {
      }

      public int GetNativeDataSize()
      {
        return IntPtr.Size;
      }

      public IntPtr MarshalManagedToNative(object ManagedObj)
      {
        if (ManagedObj is FlutterDesktopPluginRegistrar messenger)
        {
          return messenger.handle;
        }
        return IntPtr.Zero;
      }

      public object MarshalNativeToManaged(IntPtr pNativeData)
      {
        return new FlutterDesktopPluginRegistrar()
        {
          handle = pNativeData
        };
      }

      public static ICustomMarshaler GetInstance(string s)
      {
        return _instance;
      }
    }
  }
}
