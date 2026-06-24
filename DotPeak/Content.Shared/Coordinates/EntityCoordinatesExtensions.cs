// Decompiled with JetBrains decompiler
// Type: Content.Shared.Coordinates.EntityCoordinatesExtensions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using System.Numerics;

#nullable disable
namespace Content.Shared.Coordinates;

public static class EntityCoordinatesExtensions
{
  public static EntityCoordinates ToCoordinates(this EntityUid id)
  {
    return new EntityCoordinates(id, new Vector2(0.0f, 0.0f));
  }

  public static EntityCoordinates ToCoordinates(this EntityUid id, Vector2 offset)
  {
    return new EntityCoordinates(id, offset);
  }

  public static EntityCoordinates ToCoordinates(this EntityUid id, float x, float y)
  {
    return new EntityCoordinates(id, x, y);
  }
}
