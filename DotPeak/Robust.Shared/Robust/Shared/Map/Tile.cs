// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.Tile
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.ComponentModel;

#nullable enable
namespace Robust.Shared.Map;

[Serializable]
public readonly struct Tile(int typeId, byte flags = 0, byte variant = 0, byte rotationMirroring = 0) : 
  IEquatable<Tile>,
  ISpanFormattable,
  IFormattable
{
  public readonly int TypeId = typeId;
  public readonly byte Flags = flags;
  public readonly byte Variant = variant;
  public readonly byte RotationMirroring = rotationMirroring;
  public static readonly Tile Empty = new Tile(0);

  public bool IsEmpty => this.TypeId == 0;

  public static byte DirectionToByte(Direction direction, bool throwIfDiagonal = false)
  {
    switch ((int) direction)
    {
      case 0:
        return 0;
      case 2:
        return 1;
      case 4:
        return 2;
      case 6:
        return 3;
      default:
        if (throwIfDiagonal)
          throw new InvalidEnumArgumentException(nameof (direction), (int) direction, typeof (Direction));
        return 0;
    }
  }

  public static bool operator ==(Tile a, Tile b) => a.Equals(b);

  public static bool operator !=(Tile a, Tile b) => !a.Equals(b);

  public override string ToString()
  {
    return $"Tile {this.TypeId}, {this.Flags}, {this.Variant}, {this.RotationMirroring}";
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
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).\u002Ector(9, 3, span1);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral("Tile ");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<int>(this.TypeId);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral(", ");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<byte>(this.Flags);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral(", ");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<byte>(this.Variant);
    ref BufferInterpolatedStringHandler local2 = ref interpolatedStringHandler;
    return FormatHelpers.TryFormatInto(span2, ref local1, ref local2);
  }

  public bool Equals(Tile other)
  {
    return this.TypeId == other.TypeId && (int) this.Flags == (int) other.Flags && (int) this.Variant == (int) other.Variant && (int) this.RotationMirroring == (int) other.RotationMirroring;
  }

  public override bool Equals(object? obj)
  {
    return obj != null && obj is Tile other && this.Equals(other);
  }

  public override int GetHashCode()
  {
    return this.TypeId.GetHashCode() * 397 ^ this.Flags.GetHashCode() ^ this.Variant.GetHashCode() ^ this.RotationMirroring.GetHashCode();
  }

  public Tile WithVariant(byte variant) => new Tile(this.TypeId, this.Flags, variant);

  public Tile WithFlag(byte flag) => new Tile(this.TypeId, flag, this.Variant);
}
