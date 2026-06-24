// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.KeyFunctionId
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.Input;

[NetSerializable]
[Serializable]
public readonly struct KeyFunctionId(int id) : IEquatable<KeyFunctionId>
{
  private readonly int _value = id;

  public static explicit operator int(KeyFunctionId funcId) => funcId._value;

  public override string ToString() => this._value.ToString();

  public bool Equals(KeyFunctionId other) => this._value == other._value;

  public override bool Equals(object? obj) => obj is KeyFunctionId other && this.Equals(other);

  public override int GetHashCode() => this._value;

  public static bool operator ==(KeyFunctionId left, KeyFunctionId right) => left.Equals(right);

  public static bool operator !=(KeyFunctionId left, KeyFunctionId right) => !left.Equals(right);
}
