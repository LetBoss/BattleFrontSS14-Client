// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.TileChangedEntry
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Map;
using Robust.Shared.Maths;

#nullable disable
namespace Robust.Shared.GameObjects;

public readonly record struct TileChangedEntry(
  Tile NewTile,
  Tile OldTile,
  Vector2i ChunkIndex,
  Vector2i GridIndices)
{
  public bool EmptyChanged
  {
    get
    {
      Tile tile = this.OldTile;
      int num1 = tile.IsEmpty ? 1 : 0;
      tile = this.NewTile;
      int num2 = tile.IsEmpty ? 1 : 0;
      return num1 != num2;
    }
  }
}
