// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.NetCoordinates
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Map;

[NetSerializable]
[Serializable]
public readonly struct NetCoordinates : IEquatable<NetCoordinates>, ISpanFormattable, IFormattable
{
  public static readonly NetCoordinates Invalid = new NetCoordinates(NetEntity.Invalid, Vector2.Zero);
  public readonly NetEntity NetEntity;
  public readonly Vector2 Position;

  public float X => this.Position.X;

  public float Y => this.Position.Y;

  public NetCoordinates(NetEntity netEntity, Vector2 position)
  {
    this.NetEntity = netEntity;
    this.Position = position;
  }

  public NetCoordinates(NetEntity netEntity, float x, float y)
  {
    this.NetEntity = netEntity;
    this.Position = new Vector2(x, y);
  }

  public bool Equals(NetCoordinates other)
  {
    return this.NetEntity.Equals(other.NetEntity) && this.Position.Equals(other.Position);
  }

  public override bool Equals(object? obj) => obj is NetCoordinates other && this.Equals(other);

  public override int GetHashCode()
  {
    return HashCode.Combine<NetEntity, Vector2>(this.NetEntity, this.Position);
  }

  public void Deconstruct(out NetEntity entId, out Vector2 localPos)
  {
    entId = this.NetEntity;
    localPos = this.Position;
  }

  public override string ToString()
  {
    return $"NetEntity={this.NetEntity}, X={this.Position.X:N2}, Y={this.Position.Y:N2}";
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
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).\u002Ector(18, 3, span1);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral("NetEntity=");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<NetEntity>(this.NetEntity);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral(", X=");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<float>(this.Position.X, "N2");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral(", Y=");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<float>(this.Position.Y, "N2");
    ref BufferInterpolatedStringHandler local2 = ref interpolatedStringHandler;
    return FormatHelpers.TryFormatInto(span2, ref local1, ref local2);
  }
}
