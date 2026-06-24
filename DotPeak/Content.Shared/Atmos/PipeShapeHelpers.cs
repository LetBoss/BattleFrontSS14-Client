// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.PipeShapeHelpers
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;

#nullable disable
namespace Content.Shared.Atmos;

public static class PipeShapeHelpers
{
  public static PipeDirection ToBaseDirection(this PipeShape shape)
  {
    switch (shape)
    {
      case PipeShape.Half:
        return PipeDirection.South;
      case PipeShape.Straight:
        return PipeDirection.Longitudinal;
      case PipeShape.Bend:
        return PipeDirection.SWBend;
      case PipeShape.TJunction:
        return PipeDirection.TSouth;
      case PipeShape.Fourway:
        return PipeDirection.Fourway;
      default:
        throw new ArgumentOutOfRangeException(nameof (shape), $"{shape} does not have an associated {"PipeDirection"}.");
    }
  }
}
