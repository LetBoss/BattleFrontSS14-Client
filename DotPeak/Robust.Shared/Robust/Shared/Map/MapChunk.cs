// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.MapChunk
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#nullable enable
namespace Robust.Shared.Map;

internal sealed class MapChunk
{
  private const int SnapCellStartingCapacity = 1;
  private readonly Vector2i _gridIndices;
  [Robust.Shared.ViewVariables.ViewVariables]
  internal readonly Tile[,] Tiles;
  private readonly MapChunk.SnapGridCell[,] _snapGrid;
  [Robust.Shared.ViewVariables.ViewVariables]
  internal HashSet<string> Fixtures = new HashSet<string>();

  [Robust.Shared.ViewVariables.ViewVariables]
  internal int FilledTiles { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public Box2i CachedBounds { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public GameTick LastTileModifiedTick { get; set; }

  public bool SuppressCollisionRegeneration { get; set; }

  public MapChunk(int x, int y, ushort chunkSize)
  {
    this._gridIndices = new Vector2i(x, y);
    this.ChunkSize = chunkSize;
    this.Tiles = new Tile[(int) this.ChunkSize, (int) this.ChunkSize];
    this._snapGrid = new MapChunk.SnapGridCell[(int) this.ChunkSize, (int) this.ChunkSize];
  }

  public ushort ChunkSize { get; }

  public int X => this._gridIndices.X;

  public int Y => this._gridIndices.Y;

  public Vector2i Indices => this._gridIndices;

  public Tile GetTile(ushort xIndex, ushort yIndex)
  {
    if ((int) xIndex >= (int) this.ChunkSize)
      throw new ArgumentOutOfRangeException(nameof (xIndex), "Tile indices out of bounds.");
    if ((int) yIndex >= (int) this.ChunkSize)
      throw new ArgumentOutOfRangeException(nameof (yIndex), "Tile indices out of bounds.");
    return this.Tiles[(int) xIndex, (int) yIndex];
  }

  public Tile GetTile(Vector2i indices) => this.Tiles[indices.X, indices.Y];

  public Vector2i GridTileToChunkTile(Vector2i gridTile)
  {
    return new Vector2i(MathHelper.Mod(gridTile.X, (int) this.ChunkSize), MathHelper.Mod(gridTile.Y, (int) this.ChunkSize));
  }

  public Vector2i ChunkTileToGridTile(Vector2i chunkTile)
  {
    return Vector2i.op_Addition(chunkTile, Vector2i.op_Multiply(this._gridIndices, (int) this.ChunkSize));
  }

  public IEnumerable<EntityUid> GetSnapGridCell(ushort xCell, ushort yCell)
  {
    if ((int) xCell >= (int) this.ChunkSize)
      throw new ArgumentOutOfRangeException(nameof (xCell), "Tile indices out of bounds.");
    if ((int) yCell >= (int) this.ChunkSize)
      throw new ArgumentOutOfRangeException(nameof (yCell), "Tile indices out of bounds.");
    return (IEnumerable<EntityUid>) this._snapGrid[(int) xCell, (int) yCell].Center ?? (IEnumerable<EntityUid>) Array.Empty<EntityUid>();
  }

  internal List<EntityUid>? GetSnapGrid(ushort xCell, ushort yCell)
  {
    if ((int) xCell >= (int) this.ChunkSize)
      throw new ArgumentOutOfRangeException(nameof (xCell), "Tile indices out of bounds.");
    if ((int) yCell >= (int) this.ChunkSize)
      throw new ArgumentOutOfRangeException(nameof (yCell), "Tile indices out of bounds.");
    return this._snapGrid[(int) xCell, (int) yCell].Center;
  }

  public void AddToSnapGridCell(ushort xCell, ushort yCell, EntityUid euid)
  {
    if ((int) xCell >= (int) this.ChunkSize)
      throw new ArgumentOutOfRangeException(nameof (xCell), "Tile indices out of bounds.");
    if ((int) yCell >= (int) this.ChunkSize)
      throw new ArgumentOutOfRangeException(nameof (yCell), "Tile indices out of bounds.");
    ref MapChunk.SnapGridCell local1 = ref this._snapGrid.Address((int) xCell, (int) yCell);
    ref List<EntityUid> local2 = ref local1.Center;
    if (local2 == null)
      local2 = new List<EntityUid>(1);
    local1.Center.Add(euid);
  }

  public void RemoveFromSnapGridCell(ushort xCell, ushort yCell, EntityUid euid)
  {
    if ((int) xCell >= (int) this.ChunkSize)
      throw new ArgumentOutOfRangeException(nameof (xCell), "Tile indices out of bounds.");
    if ((int) yCell >= (int) this.ChunkSize)
      throw new ArgumentOutOfRangeException(nameof (yCell), "Tile indices out of bounds.");
    this._snapGrid[(int) xCell, (int) yCell].Center?.Remove(euid);
  }

  public override string ToString() => $"Chunk {this._gridIndices}";

  internal bool TrySetTile(
    ushort xIndex,
    ushort yIndex,
    Tile tile,
    out Tile oldTile,
    out bool shapeChanged)
  {
    if ((int) xIndex >= this.Tiles.Length)
      throw new ArgumentOutOfRangeException(nameof (xIndex), "Tile indices out of bounds.");
    if ((int) yIndex >= this.Tiles.Length)
      throw new ArgumentOutOfRangeException(nameof (yIndex), "Tile indices out of bounds.");
    shapeChanged = false;
    ref Tile local = ref this.Tiles.Address((int) xIndex, (int) yIndex);
    if (local == tile)
    {
      oldTile = new Tile();
      return false;
    }
    if (local.IsEmpty)
    {
      if (!tile.IsEmpty)
      {
        ++this.FilledTiles;
        shapeChanged = true;
      }
    }
    else if (tile.IsEmpty)
    {
      --this.FilledTiles;
      shapeChanged = true;
    }
    oldTile = local;
    local = tile;
    return true;
  }

  [Conditional("DEBUG")]
  public void ValidateChunk()
  {
    int num = 0;
    Tile[,] tiles = this.Tiles;
    int upperBound1 = tiles.GetUpperBound(0);
    int upperBound2 = tiles.GetUpperBound(1);
    for (int lowerBound1 = tiles.GetLowerBound(0); lowerBound1 <= upperBound1; ++lowerBound1)
    {
      for (int lowerBound2 = tiles.GetLowerBound(1); lowerBound2 <= upperBound2; ++lowerBound2)
      {
        if (!tiles[lowerBound1, lowerBound2].IsEmpty)
          ++num;
      }
    }
  }

  private struct SnapGridCell
  {
    public List<EntityUid>? Center;
  }
}
