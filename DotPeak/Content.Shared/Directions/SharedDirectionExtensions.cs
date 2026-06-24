// Decompiled with JetBrains decompiler
// Type: Content.Shared.Directions.SharedDirectionExtensions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Map;
using Robust.Shared.Maths;

#nullable disable
namespace Content.Shared.Directions;

public static class SharedDirectionExtensions
{
  public static EntityCoordinates Offset(this EntityCoordinates coordinates, Direction direction)
  {
    return ((EntityCoordinates) ref coordinates).Offset(DirectionExtensions.ToVec(direction));
  }
}
