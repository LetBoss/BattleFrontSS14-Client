// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ZStdException
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using SharpZstd.Interop;
using System;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Utility;

[Serializable]
internal sealed class ZStdException : Exception
{
  public ZStdException()
  {
  }

  public ZStdException(string message)
    : base(message)
  {
  }

  public ZStdException(string message, Exception inner)
    : base(message, inner)
  {
  }

  public static unsafe ZStdException FromCode(UIntPtr code)
  {
    return new ZStdException(Marshal.PtrToStringUTF8((IntPtr) Zstd.ZSTD_getErrorName(code)));
  }

  public static void ThrowIfError(UIntPtr code)
  {
    if (Zstd.ZSTD_isError(code) != 0U)
      throw ZStdException.FromCode(code);
  }
}
