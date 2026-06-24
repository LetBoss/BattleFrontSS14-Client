// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Enumerators.NearestChunkEnumerator
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared.Map.Enumerators;

public record struct NearestChunkEnumerator
{
  private readonly Vector2i _chunkLB;
  private readonly Vector2i _chunkRT;
  private ValueList<Vector2i> _chunks;
  private int _n;

  public NearestChunkEnumerator(Box2 localAABB, int chunkSize)
  {
    this._n = 0;
    this._chunks = new ValueList<Vector2i>();
    this._chunkLB = Vector2Helpers.Floored(localAABB.BottomLeft / (float) chunkSize);
    this._chunkRT = Vector2Helpers.Floored(localAABB.TopRight / (float) chunkSize);
    this.InitializeChunks(new Vector2i(chunkSize, chunkSize));
  }

  public NearestChunkEnumerator(Box2 localAABB, Vector2i chunkSize)
  {
    this._n = 0;
    this._chunks = new ValueList<Vector2i>();
    this._chunkLB = Vector2Helpers.Floored(localAABB.BottomLeft / Vector2i.op_Implicit(chunkSize));
    this._chunkRT = Vector2Helpers.Floored(localAABB.TopRight / Vector2i.op_Implicit(chunkSize));
    this.InitializeChunks(chunkSize);
  }

  private void InitializeChunks(Vector2i chunkSize)
  {
    Vector2 vector2_1 = Vector2i.op_Implicit(this._chunkLB) * Vector2i.op_Implicit(chunkSize);
    Vector2 vector2_2 = Vector2i.op_Implicit(this._chunkRT) * Vector2i.op_Implicit(chunkSize);
    Vector2 halfChunk = new Vector2((float) chunkSize.X / 2f, (float) chunkSize.Y / 2f);
    Vector2 center = (vector2_2 - vector2_1) / 2f + vector2_1;
    for (int x = this._chunkLB.X; x < this._chunkRT.X; ++x)
    {
      for (int y = this._chunkLB.Y; y < this._chunkRT.Y; ++y)
        this._chunks.Add(Vector2i.op_Multiply(new Vector2i(x, y), chunkSize));
    }
    this._chunks.Sort((Comparison<Vector2i>) ((c1, c2) => (Vector2i.op_Implicit(c1) + halfChunk - center).LengthSquared().CompareTo((Vector2i.op_Implicit(c2) + halfChunk - center).LengthSquared())));
  }

  public bool MoveNext([NotNullWhen(true)] out Vector2i? indices)
  {
    if (this._n >= this._chunks.Count)
    {
      indices = new Vector2i?();
      return false;
    }
    indices = new Vector2i?(this._chunks[this._n++]);
    return true;
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((EqualityComparer<Vector2i>.Default.GetHashCode(this._chunkLB) * -1521134295 + EqualityComparer<Vector2i>.Default.GetHashCode(this._chunkRT)) * -1521134295 + EqualityComparer<ValueList<Vector2i>>.Default.GetHashCode(this._chunks)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this._n);
  }

  [CompilerGenerated]
  public readonly bool Equals(NearestChunkEnumerator other)
  {
    return EqualityComparer<Vector2i>.Default.Equals(this._chunkLB, other._chunkLB) && EqualityComparer<Vector2i>.Default.Equals(this._chunkRT, other._chunkRT) && EqualityComparer<ValueList<Vector2i>>.Default.Equals(this._chunks, other._chunks) && EqualityComparer<int>.Default.Equals(this._n, other._n);
  }
}
