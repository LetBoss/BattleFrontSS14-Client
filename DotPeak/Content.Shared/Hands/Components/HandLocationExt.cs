// Decompiled with JetBrains decompiler
// Type: Content.Shared.Hands.Components.HandLocationExt
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable disable
namespace Content.Shared.Hands.Components;

public static class HandLocationExt
{
  public static HandUILocation GetUILocation(this HandLocation location)
  {
    switch (location)
    {
      case HandLocation.Left:
        return HandUILocation.Left;
      case HandLocation.Middle:
        return HandUILocation.Right;
      case HandLocation.Right:
        return HandUILocation.Right;
      default:
        throw new ArgumentOutOfRangeException(nameof (location), (object) location, (string) null);
    }
  }
}
