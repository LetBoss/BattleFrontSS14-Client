// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.EntProtoId
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Toolshed.TypeParsers;
using System;

#nullable enable
namespace Robust.Shared.Prototypes;

[NetSerializable]
[Serializable]
public readonly record struct EntProtoId(string Id) : 
  IEquatable<string>,
  IComparable<EntProtoId>,
  IAsType<string>,
  IAsType<ProtoId<EntityPrototype>>
{
  public static implicit operator string(EntProtoId protoId) => protoId.Id;

  public static implicit operator EntProtoId(EntityPrototype proto) => new EntProtoId(proto.ID);

  public static implicit operator EntProtoId(string id) => new EntProtoId(id);

  public static implicit operator EntProtoId?(string? id)
  {
    return id != null ? new EntProtoId?(new EntProtoId(id)) : new EntProtoId?();
  }

  public bool Equals(string? other) => this.Id == other;

  public int CompareTo(EntProtoId other)
  {
    return string.Compare(this.Id, other.Id, StringComparison.Ordinal);
  }

  string IAsType<string>.AsType() => this.Id;

  ProtoId<EntityPrototype> IAsType<ProtoId<EntityPrototype>>.AsType()
  {
    return new ProtoId<EntityPrototype>(this.Id);
  }

  public override string ToString() => this.Id ?? string.Empty;
}
