// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Map.WindowId
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.Map;

[NetSerializable]
[Serializable]
public readonly struct WindowId(int value) : IEquatable<WindowId>
{
  public static readonly WindowId Invalid = new WindowId();
  public static readonly WindowId Main = new WindowId(1);
  internal readonly int Value = value;

  public bool Equals(WindowId other) => this.Value == other.Value;

  public override bool Equals(object? obj)
  {
    return obj != null && obj is WindowId other && this.Equals(other);
  }

  public override int GetHashCode() => this.Value;

  public static bool operator ==(WindowId a, WindowId b) => a.Value == b.Value;

  public static bool operator !=(WindowId a, WindowId b) => !(a == b);

  public static explicit operator int(WindowId self) => self.Value;

  public override string ToString() => $"Window {this.Value}";
}
