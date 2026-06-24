// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ZStd
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using SharpZstd.Interop;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared.Utility;

public static class ZStd
{
  public static int CompressBound(int length) => (int) Zstd.ZSTD_COMPRESSBOUND((UIntPtr) length);

  public static unsafe int Compress(Span<byte> into, ReadOnlySpan<byte> data, int compressionLevel = 3)
  {
    fixed (byte* numPtr1 = &into.GetPinnableReference())
      fixed (byte* numPtr2 = &data.GetPinnableReference())
      {
        int length1 = into.Length;
        byte* numPtr3 = numPtr2;
        int length2 = data.Length;
        int num = compressionLevel;
        UIntPtr code = Zstd.ZSTD_compress((void*) numPtr1, (UIntPtr) length1, (void*) numPtr3, (UIntPtr) length2, num);
        ZStdException.ThrowIfError(code);
        return (int) code;
      }
  }

  [ModuleInitializer]
  internal static void InitZStd()
  {
    try
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      NativeLibrary.SetDllImportResolver(typeof (Zstd).Assembly, ZStd.\u003C\u003EO.\u003C0\u003E__ResolveZstd ?? (ZStd.\u003C\u003EO.\u003C0\u003E__ResolveZstd = new DllImportResolver(ZStd.ResolveZstd)));
    }
    catch (InvalidOperationException ex)
    {
    }
  }

  private static IntPtr ResolveZstd(string name, Assembly assembly, DllImportSearchPath? path)
  {
    if (name == "zstd")
    {
      if (OperatingSystem.IsLinux())
      {
        IntPtr handle;
        if (NativeLibrary.TryLoad("zstd.so", assembly, path, out handle) || NativeLibrary.TryLoad("libzstd.so.1", assembly, path, out handle) || NativeLibrary.TryLoad("libzstd.so", assembly, path, out handle))
          return handle;
      }
      else
      {
        IntPtr handle;
        if (OperatingSystem.IsMacOS() && NativeLibrary.TryLoad("libzstd.1.dylib", assembly, path, out handle))
          return handle;
      }
    }
    return IntPtr.Zero;
  }
}
