// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Enumerators.ChunkEnumerator
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Robust.Shared.Map.Enumerators;

internal ref struct ChunkEnumerator
{
  private Dictionary<Vector2i, MapChunk> _chunks;
  private Vector2i _chunkLB;
  private Vector2i _chunkRT;
  private int _xIndex;
  private int _yIndex;

  internal ChunkEnumerator(Dictionary<Vector2i, MapChunk> chunks, Box2 localAABB, int chunkSize)
  {
    this._chunks = chunks;
    this._chunkLB = new Vector2i((int) Math.Floor((double) localAABB.Left / (double) chunkSize), (int) Math.Floor((double) localAABB.Bottom / (double) chunkSize));
    this._chunkRT = new Vector2i((int) Math.Floor((double) localAABB.Right / (double) chunkSize), (int) Math.Floor((double) localAABB.Top / (double) chunkSize));
    this._xIndex = this._chunkLB.X;
    this._yIndex = this._chunkLB.Y;
  }

  public bool MoveNext([NotNullWhen(true)] out MapChunk? chunk)
  {
    if (this._yIndex > this._chunkRT.Y)
    {
      this._yIndex = this._chunkLB.Y;
      ++this._xIndex;
    }
    for (int xIndex = this._xIndex; xIndex <= this._chunkRT.X; ++xIndex)
    {
      for (int yIndex = this._yIndex; yIndex <= this._chunkRT.Y; ++yIndex)
      {
        Vector2i key;
        // ISSUE: explicit constructor call
        ((Vector2i) ref key).\u002Ector(xIndex, yIndex);
        if (this._chunks.TryGetValue(key, out chunk))
        {
          this._xIndex = xIndex;
          this._yIndex = yIndex + 1;
          return true;
        }
      }
      this._yIndex = this._chunkLB.Y;
    }
    chunk = (MapChunk) null;
    return false;
  }
}
