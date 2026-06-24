// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetUserId
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.Network;

[NetSerializable]
[Serializable]
public struct NetUserId(Guid userId) : IEquatable<NetUserId>, ISelfSerialize
{
  public readonly Guid UserId = userId;

  public override bool Equals(object? obj)
  {
    bool flag;
    switch (obj)
    {
      case Guid guid:
        flag = this.Equals((object) guid);
        break;
      case NetUserId other:
        flag = this.Equals(other);
        break;
      default:
        flag = false;
        break;
    }
    return flag;
  }

  public bool Equals(NetUserId other) => this.UserId == other.UserId;

  public override int GetHashCode() => this.UserId.GetHashCode();

  public override string ToString() => this.UserId.ToString();

  public static bool operator ==(NetUserId id1, NetUserId id2) => id1.Equals(id2);

  public static bool operator !=(NetUserId id1, NetUserId id2) => !(id1 == id2);

  public static implicit operator Guid(NetUserId id) => id.UserId;

  public static explicit operator NetUserId(Guid id) => new NetUserId(id);

  void ISelfSerialize.Deserialize(string value) => this = (NetUserId) Guid.Parse(value);

  string ISelfSerialize.Serialize() => this.ToString();
}
