// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.MapId
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.Map;

[NetSerializable]
[Serializable]
public readonly struct MapId(int value) : IEquatable<MapId>
{
  public static readonly MapId Nullspace = new MapId(0);
  internal readonly int Value = value;

  public bool Equals(MapId other) => this.Value == other.Value;

  public override bool Equals(object? obj)
  {
    return obj != null && obj is MapId other && this.Equals(other);
  }

  public override int GetHashCode() => this.Value;

  public static bool operator ==(MapId a, MapId b) => a.Value == b.Value;

  public static bool operator !=(MapId a, MapId b) => !(a == b);

  public static explicit operator int(MapId self) => self.Value;

  public override string ToString()
  {
    if (!this.IsClientSide)
      return this.Value.ToString();
    return $"c{-this.Value}";
  }

  public bool IsClientSide => this.Value < 0;
}
