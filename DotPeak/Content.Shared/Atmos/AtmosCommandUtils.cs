// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.AtmosCommandUtils
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable enable
namespace Content.Shared.Atmos;

public sealed class AtmosCommandUtils
{
  public static bool TryParseGasID(string str, out int x)
  {
    x = -1;
    Gas result;
    if (Enum.TryParse<Gas>(str, true, out result))
      x = (int) result;
    else if (!int.TryParse(str, out x))
      return false;
    return x >= 0 && x < 9;
  }
}
