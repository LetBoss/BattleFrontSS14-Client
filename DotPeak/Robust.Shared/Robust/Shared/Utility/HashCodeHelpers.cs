// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.HashCodeHelpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Utility;

public static class HashCodeHelpers
{
  public static void AddArray<T>(ref this HashCode hc, T[]? array)
  {
    if (array == null)
    {
      hc.Add<int>(0);
    }
    else
    {
      hc.Add<int>(array.Length);
      foreach (T obj in array)
        hc.Add<T>(obj);
    }
  }
}
