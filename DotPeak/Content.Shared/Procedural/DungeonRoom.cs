// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonRoom
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Procedural;

public sealed record DungeonRoom(
  HashSet<Vector2i> Tiles,
  Vector2 Center,
  Box2i Bounds,
  HashSet<Vector2i> Exterior)
{
  public readonly List<Vector2i> Entrances = new List<Vector2i>();
  public readonly HashSet<Vector2i> Exterior = Exterior;

  public HashSet<Vector2i> Tiles { get; init; } = Tiles;

  public Vector2 Center { get; init; } = Center;

  public Box2i Bounds { get; init; } = Bounds;

  [CompilerGenerated]
  public void Deconstruct(
    out HashSet<Vector2i> Tiles,
    out Vector2 Center,
    out Box2i Bounds,
    out HashSet<Vector2i> Exterior)
  {
    Tiles = this.Tiles;
    Center = this.Center;
    Bounds = this.Bounds;
    Exterior = this.Exterior;
  }
}
