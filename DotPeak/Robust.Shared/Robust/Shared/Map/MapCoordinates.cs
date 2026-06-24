// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.MapCoordinates
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Map;

[DataRecord]
[NetSerializable]
[Serializable]
public readonly record struct MapCoordinates(Vector2 position, MapId mapId) : 
  ISpanFormattable,
  IFormattable
{
  public static readonly MapCoordinates Nullspace = new MapCoordinates(Vector2.Zero, MapId.Nullspace);
  public readonly Vector2 Position = position;
  public readonly MapId MapId = mapId;

  public float X => this.Position.X;

  public float Y => this.Position.Y;

  public MapCoordinates(float x, float y, MapId mapId)
    : this(new Vector2(x, y), mapId)
  {
  }

  public override string ToString()
  {
    return $"Map={this.MapId}, X={this.Position.X:N2}, Y={this.Position.Y:N2}";
  }

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
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).\u002Ector(12, 3, span1);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral("Map=");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<MapId>(this.MapId);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral(", X=");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<float>(this.Position.X, "N2");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral(", Y=");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<float>(this.Position.Y, "N2");
    ref BufferInterpolatedStringHandler local2 = ref interpolatedStringHandler;
    return FormatHelpers.TryFormatInto(span2, ref local1, ref local2);
  }

  public bool InRange(MapCoordinates otherCoords, float range)
  {
    return !(otherCoords.MapId != this.MapId) && (double) (otherCoords.Position - this.Position).LengthSquared() < (double) range * (double) range;
  }

  public void Deconstruct(out float x, out float y)
  {
    x = this.X;
    y = this.Y;
  }

  public void Deconstruct(out MapId mapId, out float x, out float y)
  {
    mapId = this.MapId;
    x = this.X;
    y = this.Y;
  }

  public MapCoordinates Offset(Vector2 offset)
  {
    return new MapCoordinates(this.Position + offset, this.MapId);
  }

  public MapCoordinates Offset(float x, float y) => this.Offset(new Vector2(x, y));

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return EqualityComparer<Vector2>.Default.GetHashCode(this.Position) * -1521134295 + EqualityComparer<MapId>.Default.GetHashCode(this.MapId);
  }

  [CompilerGenerated]
  public bool Equals(MapCoordinates other)
  {
    return EqualityComparer<Vector2>.Default.Equals(this.Position, other.Position) && EqualityComparer<MapId>.Default.Equals(this.MapId, other.MapId);
  }
}
