// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Input.BoundKeyFunction
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.Input;

[NetSerializable]
[Serializable]
public struct BoundKeyFunction(string name) : 
  IComparable,
  IComparable<BoundKeyFunction>,
  IEquatable<BoundKeyFunction>,
  ISelfSerialize
{
  public readonly string FunctionName = name;

  public static implicit operator BoundKeyFunction(string name) => new BoundKeyFunction(name);

  public override readonly string ToString() => $"KeyFunction({this.FunctionName})";

  public readonly int CompareTo(object? obj)
  {
    return obj is BoundKeyFunction other ? this.CompareTo(other) : 1;
  }

  public readonly int CompareTo(BoundKeyFunction other)
  {
    return string.Compare(this.FunctionName, other.FunctionName, StringComparison.InvariantCultureIgnoreCase);
  }

  public override readonly bool Equals(object? obj)
  {
    return obj is BoundKeyFunction other && this.Equals(other);
  }

  public readonly bool Equals(BoundKeyFunction other) => other.FunctionName == this.FunctionName;

  public override readonly int GetHashCode() => this.FunctionName.GetHashCode();

  public static bool operator ==(BoundKeyFunction a, BoundKeyFunction b)
  {
    return a.FunctionName == b.FunctionName;
  }

  public static bool operator !=(BoundKeyFunction a, BoundKeyFunction b) => !(a == b);

  public void Deserialize(string value) => this = new BoundKeyFunction(value);

  public readonly string Serialize() => this.FunctionName;
}
