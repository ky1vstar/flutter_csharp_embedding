﻿// Copyright 2021 Samsung Electronics Co., Ltd. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using System.IO;

namespace Flutter.Embedding.Common
{
  internal static class StreamExtensions
  {
    public static bool HasRemaining(this Stream stream)
    {
      return stream.Position < stream.Length;
    }
  }
}