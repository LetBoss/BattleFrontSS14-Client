// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Timing.GameTick
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.Timing;

[NetSerializable]
[Serializable]
public readonly struct GameTick(uint value) : IEquatable<GameTick>, IComparable<GameTick>
{
  public static readonly GameTick Zero = new GameTick(0U);
  public static readonly GameTick First = new GameTick(1U);
  public static readonly GameTick MaxValue = new GameTick(uint.MaxValue);
  public readonly uint Value = value;

  public bool Equals(GameTick other) => (int) this.Value == (int) other.Value;

  public override bool Equals(object? obj)
  {
    return obj != null && obj is GameTick other && this.Equals(other);
  }

  public override int GetHashCode() => (int) this.Value;

  public static bool operator ==(GameTick a, GameTick b) => (int) a.Value == (int) b.Value;

  public static bool operator !=(GameTick a, GameTick b) => (int) a.Value != (int) b.Value;

  public int CompareTo(GameTick other) => this.Value.CompareTo(other.Value);

  public static bool operator >(GameTick a, GameTick b) => a.Value > b.Value;

  public static bool operator >=(GameTick a, GameTick b) => a.Value >= b.Value;

  public static bool operator <(GameTick a, GameTick b) => a.Value < b.Value;

  public static bool operator <=(GameTick a, GameTick b) => a.Value <= b.Value;

  public static GameTick operator +(GameTick a, uint b) => new GameTick(a.Value + b);

  public static GameTick operator -(GameTick a, uint b) => new GameTick(a.Value - b);

  public override string ToString() => this.Value.ToString();
}
