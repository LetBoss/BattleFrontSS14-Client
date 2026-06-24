// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ContentPack.PathHelpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.IO;

#nullable enable
namespace Robust.Shared.ContentPack;

internal static class PathHelpers
{
  internal static string GetExecutableDirectory()
  {
    string location = typeof (PathHelpers).Assembly.Location;
    return !(location == string.Empty) ? Path.GetDirectoryName(location) : throw new InvalidOperationException("Cannot find path of executable.");
  }

  public static string ExecutableRelativeFile(string file)
  {
    return Path.GetFullPath(Path.Combine(PathHelpers.GetExecutableDirectory(), file));
  }

  public static IEnumerable<string> GetFiles(string path)
  {
    return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
  }

  public static bool IsFileInUse(IOException exception)
  {
    bool flag;
    switch (exception.HResult & (int) ushort.MaxValue)
    {
      case 32 /*0x20*/:
        flag = true;
        break;
      case 33:
        flag = true;
        break;
      default:
        flag = false;
        break;
    }
    return flag;
  }

  public static bool IsFileSystemCaseSensitive()
  {
    return !OperatingSystem.IsWindows() && !OperatingSystem.IsMacOS();
  }

  internal static string SafeGetResourcePath(string baseDir, ResPath path)
  {
    string relativeSystemPath = path.ToRelativeSystemPath();
    string resourcePath = !relativeSystemPath.Contains("\\..") && !relativeSystemPath.Contains("/..") && !relativeSystemPath.StartsWith("..") ? Path.GetFullPath(Path.Join(baseDir, relativeSystemPath)) : throw new InvalidOperationException($"This branch should never be reached. Path: {path}");
    if (!resourcePath.StartsWith(baseDir) && resourcePath != baseDir.TrimEnd(Path.DirectorySeparatorChar))
      throw new InvalidOperationException($"This branch should never be reached. Path: {path}");
    return resourcePath;
  }
}
