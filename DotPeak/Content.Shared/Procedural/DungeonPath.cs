// Decompiled with JetBrains decompiler
// Type: Content.Shared.Procedural.DungeonPath
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Procedural;

public sealed record DungeonPath(string Tile, string Wall, HashSet<Vector2i> Tiles)
{
  public string Tile = Tile;
  public string Wall = Wall;

  public HashSet<Vector2i> Tiles { get; init; } = Tiles;

  [CompilerGenerated]
  public void Deconstruct(out string Tile, out string Wall, out HashSet<Vector2i> Tiles)
  {
    Tile = this.Tile;
    Wall = this.Wall;
    Tiles = this.Tiles;
  }
}
