// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.TileRef
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Robust.Shared.Map;

public readonly struct TileRef : IEquatable<TileRef>, ISpanFormattable, IFormattable
{
  public readonly EntityUid GridUid;
  public readonly Vector2i GridIndices;
  public readonly Tile Tile;

  public static TileRef Zero => new TileRef(EntityUid.Invalid, Vector2i.Zero, Tile.Empty);

  internal TileRef(EntityUid gridUid, int xIndex, int yIndex, Tile tile)
    : this(gridUid, new Vector2i(xIndex, yIndex), tile)
  {
  }

  internal TileRef(EntityUid gridUid, Vector2i gridIndices, Tile tile)
  {
    this.GridUid = gridUid;
    this.GridIndices = gridIndices;
    this.Tile = tile;
  }

  public int X => this.GridIndices.X;

  public int Y => this.GridIndices.Y;

  public override string ToString() => $"TileRef: {this.X},{this.Y} ({this.Tile})";

  public string ToString(string? format, IFormatProvider? formatProvider) => this.ToString();

  public bool TryFormat(
    Span<char> destination,
    out int charsWritten,
    ReadOnlySpan<char> format,
    IFormatProvider? provider)
  {
    Span<char> span1 = destination;
    Span<char> span2 = span1;
    ref int local1 = ref charsWritten;
    BufferInterpolatedStringHandler interpolatedStringHandler;
    // ISSUE: explicit constructor call
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).\u002Ector(13, 3, span1);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral("TileRef: ");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<int>(this.X);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral(",");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<int>(this.Y);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral(" (");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<Tile>(this.Tile);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral(")");
    ref BufferInterpolatedStringHandler local2 = ref interpolatedStringHandler;
    return FormatHelpers.TryFormatInto(span2, ref local1, ref local2);
  }

  public bool Equals(TileRef other)
  {
    return this.GridUid.Equals(other.GridUid) && ((Vector2i) ref this.GridIndices).Equals(other.GridIndices) && this.Tile.Equals(other.Tile);
  }

  public override bool Equals(object? obj)
  {
    return obj != null && obj is TileRef other && this.Equals(other);
  }

  public static bool operator ==(TileRef a, TileRef b) => a.Equals(b);

  public static bool operator !=(TileRef a, TileRef b) => !a.Equals(b);

  public override int GetHashCode()
  {
    return (this.GridUid.GetHashCode() * 397 ^ this.GridIndices.GetHashCode()) * 397 ^ this.Tile.GetHashCode();
  }
}
