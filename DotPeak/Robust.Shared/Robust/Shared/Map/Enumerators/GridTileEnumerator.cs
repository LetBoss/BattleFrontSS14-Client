// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Enumerators.GridTileEnumerator
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Map.Enumerators;

public struct GridTileEnumerator
{
  private readonly EntityUid _gridUid;
  private Dictionary<Vector2i, MapChunk>.Enumerator _chunkEnumerator;
  private readonly ushort _chunkSize;
  private int _index;
  private readonly bool _ignoreEmpty;

  internal GridTileEnumerator(
    EntityUid gridUid,
    Dictionary<Vector2i, MapChunk>.Enumerator chunkEnumerator,
    ushort chunkSize,
    bool ignoreEmpty)
  {
    this._gridUid = gridUid;
    this._chunkEnumerator = chunkEnumerator;
    this._chunkSize = chunkSize;
    this._index = (int) this._chunkSize * (int) this._chunkSize;
    this._ignoreEmpty = ignoreEmpty;
  }

  public bool MoveNext([NotNullWhen(true)] out TileRef? tileRef)
  {
    Vector2i key;
    ushort num1;
    ushort num2;
    Tile tile;
    do
    {
      if (this._index == (int) this._chunkSize * (int) this._chunkSize)
      {
        if (!this._chunkEnumerator.MoveNext())
        {
          tileRef = new TileRef?();
          return false;
        }
        this._index = 0;
      }
      MapChunk mapChunk;
      (key, mapChunk) = this._chunkEnumerator.Current;
      num1 = (ushort) ((uint) this._index / (uint) this._chunkSize);
      num2 = (ushort) ((uint) this._index % (uint) this._chunkSize);
      int xIndex = (int) num1;
      int yIndex = (int) num2;
      tile = mapChunk.GetTile((ushort) xIndex, (ushort) yIndex);
      ++this._index;
    }
    while (this._ignoreEmpty && tile.IsEmpty);
    int xIndex1 = (int) num1 + key.X * (int) this._chunkSize;
    int yIndex1 = (int) num2 + key.Y * (int) this._chunkSize;
    tileRef = new TileRef?(new TileRef(this._gridUid, xIndex1, yIndex1, tile));
    return true;
  }
}
