// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Native.Libc
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Native;

internal static class Libc
{
  public const int RTLD_LAZY = 1;
  public const int RTLD_NOW = 2;
  public const int RTLD_BINDING_MASK = 3;
  public const int RTLD_NOLOAD = 4;
  public const int RTLD_DEEPBIND = 8;
  public const int RTLD_GLOBAL = 256 /*0x0100*/;
  public const int RTLD_LOCAL = 0;
  public const int RTLD_NODELETE = 4096 /*0x1000*/;

  [DllImport("libdl.so.2")]
  public static extern IntPtr dlopen([MarshalAs((UnmanagedType) 0)] string name, int flags);
}
