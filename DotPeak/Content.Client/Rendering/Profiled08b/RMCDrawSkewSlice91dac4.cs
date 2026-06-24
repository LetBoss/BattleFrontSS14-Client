// Decompiled with JetBrains decompiler
// Type: Content.Client.Rendering.Profiled08b.RMCDrawSkewSlice91dac4
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#nullable disable
namespace Content.Client.Rendering.Profiled08b;

internal readonly struct RMCDrawSkewSlice91dac4(
  byte Grid,
  byte CellX,
  byte CellY,
  byte SizePercent,
  Color Color) : IEquatable<RMCDrawSkewSlice91dac4>
{
  public byte Grid { get; init; } = Grid;

  public byte CellX { get; init; } = CellX;

  public byte CellY { get; init; } = CellY;

  public byte SizePercent { get; init; } = SizePercent;

  public Color Color { get; init; } = Color;

  [CompilerGenerated]
  public override string ToString()
  {
    StringBuilder builder = new StringBuilder();
    builder.Append("RMCWeaponProfileOverlayState");
    builder.Append(" { ");
    // ISSUE: reference to a compiler-generated method
    if (this._mc6e4480cb1ad(builder))
      builder.Append(' ');
    builder.Append('}');
    return builder.ToString();
  }

  [CompilerGenerated]
  public static bool operator !=(RMCDrawSkewSlice91dac4 left, RMCDrawSkewSlice91dac4 right)
  {
    return !(left == right);
  }

  [CompilerGenerated]
  public static bool operator ==(RMCDrawSkewSlice91dac4 left, RMCDrawSkewSlice91dac4 right)
  {
    return left.Equals(right);
  }

  [CompilerGenerated]
  public override int GetHashCode()
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return (((EqualityComparer<byte>.Default.GetHashCode(this._f0a36d90eccfa) * -1521134295 + EqualityComparer<byte>.Default.GetHashCode(this._f6e2c17bcd7f1)) * -1521134295 + EqualityComparer<byte>.Default.GetHashCode(this._f5a32a6054187)) * -1521134295 + EqualityComparer<byte>.Default.GetHashCode(this._fdd755d252529)) * -1521134295 + EqualityComparer<Color>.Default.GetHashCode(this._f8f8d79c1e6b4);
  }

  [CompilerGenerated]
  public override bool Equals(object obj)
  {
    return obj is RMCDrawSkewSlice91dac4 other && this.Equals(other);
  }

  [CompilerGenerated]
  public bool Equals(RMCDrawSkewSlice91dac4 other)
  {
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    return EqualityComparer<byte>.Default.Equals(this._f0a36d90eccfa, other._f0a36d90eccfa) && EqualityComparer<byte>.Default.Equals(this._f6e2c17bcd7f1, other._f6e2c17bcd7f1) && EqualityComparer<byte>.Default.Equals(this._f5a32a6054187, other._f5a32a6054187) && EqualityComparer<byte>.Default.Equals(this._fdd755d252529, other._fdd755d252529) && EqualityComparer<Color>.Default.Equals(this._f8f8d79c1e6b4, other._f8f8d79c1e6b4);
  }

  [CompilerGenerated]
  public void Deconstruct(
    out byte Grid,
    out byte CellX,
    out byte CellY,
    out byte SizePercent,
    out Color Color)
  {
    Grid = this.Grid;
    CellX = this.CellX;
    CellY = this.CellY;
    SizePercent = this.SizePercent;
    Color = this.Color;
  }
}
