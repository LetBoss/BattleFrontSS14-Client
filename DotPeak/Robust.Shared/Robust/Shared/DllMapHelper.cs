// Decompiled with JetBrains decompiler
// Type: Robust.Shared.DllMapHelper
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

#nullable enable
namespace Robust.Shared;

internal static class DllMapHelper
{
  [Conditional("NETCOREAPP")]
  public static void RegisterSimpleMap(Assembly assembly, string baseName)
  {
    DllMapHelper.RegisterExplicitMap(assembly, baseName + ".dll", $"lib{baseName}.so", $"lib{baseName}.dylib");
  }

  [Conditional("NETCOREAPP")]
  public static void RegisterExplicitMap(
    Assembly assembly1,
    string baseName,
    string linuxName,
    string macName)
  {
    NativeLibrary.SetDllImportResolver(assembly1, (DllImportResolver) ((name, assembly2, path) =>
    {
      if (name != baseName)
        return IntPtr.Zero;
      string libraryName = (string) null;
      if (OperatingSystem.IsLinux())
        libraryName = linuxName;
      if (OperatingSystem.IsMacOS())
        libraryName = macName;
      IntPtr handle;
      return libraryName != null && NativeLibrary.TryLoad(libraryName, assembly2, path, out handle) ? handle : IntPtr.Zero;
    }));
  }
}
