// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ByteHelpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Utility;

public static class ByteHelpers
{
  private static readonly string[] ByteSuffixes = new string[9]
  {
    "B",
    "KiB",
    "MiB",
    "GiB",
    "TiB",
    "PiB",
    "EiB",
    "ZiB",
    "YiB"
  };

  public static string FormatKibibytes(long bytes) => $"{bytes / 1024L /*0x0400*/} KiB";

  public static string FormatBytes(long bytes)
  {
    double num = (double) bytes;
    int index;
    for (index = 0; index < ByteHelpers.ByteSuffixes.Length && num >= 1024.0; ++index)
      num /= 1024.0;
    return $"{Math.Round(num, 2)} {ByteHelpers.ByteSuffixes[index]}";
  }
}
