// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Enumerators.ChunkIndicesEnumerator
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable disable
namespace Robust.Shared.Map.Enumerators;

public struct ChunkIndicesEnumerator
{
  private readonly Vector2i _chunkLB;
  private readonly Vector2i _chunkRT;
  private int _xIndex;
  private int _yIndex;

  public ChunkIndicesEnumerator(Vector2 viewPos, float range, float chunkSize)
  {
    Vector2 vector2 = new Vector2(range, range);
    this._chunkLB = Vector2Helpers.Floored((viewPos - vector2) / chunkSize);
    this._chunkRT = Vector2Helpers.Floored((viewPos + vector2) / chunkSize);
    this._xIndex = this._chunkLB.X;
    this._yIndex = this._chunkLB.Y;
  }

  public ChunkIndicesEnumerator(Box2 localAABB, int chunkSize)
  {
    this._chunkLB = Vector2Helpers.Floored(localAABB.BottomLeft / (float) chunkSize);
    this._chunkRT = Vector2Helpers.Floored(localAABB.TopRight / (float) chunkSize);
    this._xIndex = this._chunkLB.X;
    this._yIndex = this._chunkLB.Y;
  }

  public bool MoveNext([NotNullWhen(true)] out Vector2i? indices)
  {
    if (this._yIndex > this._chunkRT.Y)
    {
      this._yIndex = this._chunkLB.Y;
      ++this._xIndex;
    }
    if (this._xIndex > this._chunkRT.X)
    {
      indices = new Vector2i?();
      return false;
    }
    indices = new Vector2i?(new Vector2i(this._xIndex, this._yIndex));
    ++this._yIndex;
    return true;
  }
}
