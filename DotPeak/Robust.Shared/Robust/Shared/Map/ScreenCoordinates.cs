// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.ScreenCoordinates
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Map;

[NetSerializable]
[Serializable]
public readonly struct ScreenCoordinates : 
  IEquatable<ScreenCoordinates>,
  ISpanFormattable,
  IFormattable
{
  public static readonly ScreenCoordinates Invalid = new ScreenCoordinates(Vector2.Zero, WindowId.Invalid);
  public readonly Vector2 Position;
  public readonly WindowId Window;

  public float X => this.Position.X;

  public float Y => this.Position.Y;

  public ScreenCoordinates(Vector2 position, WindowId window)
  {
    this.Position = position;
    this.Window = window;
  }

  public ScreenCoordinates(float x, float y, WindowId window)
  {
    this.Position = new Vector2(x, y);
    this.Window = window;
  }

  public bool IsValid => this.Window != WindowId.Invalid;

  public override string ToString()
  {
    return $"({this.Position.X}, {this.Position.Y}, W{this.Window.Value})";
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
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).\u002Ector(7, 3, span1);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral("(");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<float>(this.Position.X);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral(", ");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<float>(this.Position.Y);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral(", W");
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendFormatted<int>(this.Window.Value);
    ((BufferInterpolatedStringHandler) ref interpolatedStringHandler).AppendLiteral(")");
    ref BufferInterpolatedStringHandler local2 = ref interpolatedStringHandler;
    return FormatHelpers.TryFormatInto(span2, ref local1, ref local2);
  }

  public bool Equals(ScreenCoordinates other)
  {
    return this.Position.Equals(other.Position) && this.Window == other.Window;
  }

  public override bool Equals(object? obj)
  {
    return obj != null && obj is ScreenCoordinates other && this.Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine<Vector2, WindowId>(this.Position, this.Window);
  }

  public static bool operator ==(ScreenCoordinates a, ScreenCoordinates b) => a.Equals(b);

  public static bool operator !=(ScreenCoordinates a, ScreenCoordinates b) => !a.Equals(b);

  public void Deconstruct(out Vector2 pos, out WindowId window)
  {
    pos = this.Position;
    window = this.Window;
  }
}
