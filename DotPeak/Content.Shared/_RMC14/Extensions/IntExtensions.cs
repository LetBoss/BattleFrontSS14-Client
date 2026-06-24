// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Extensions.IntExtensions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable disable
namespace Content.Shared._RMC14.Extensions;

public static class IntExtensions
{
  public static void Cap(ref this int value, int at)
  {
    at = Math.Abs(at);
    if (value > at)
    {
      value = at;
    }
    else
    {
      if (value >= -at)
        return;
      value = -at;
    }
  }
}
