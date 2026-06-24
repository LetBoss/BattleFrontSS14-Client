// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.ProtoId`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Toolshed.TypeParsers;
using System;

#nullable enable
namespace Robust.Shared.Prototypes;

[PreferOtherType(typeof (EntityPrototype), typeof (EntProtoId))]
[Serializable]
public readonly record struct ProtoId<T>(string Id) : 
  IEquatable<string>,
  IComparable<ProtoId<T>>,
  IAsType<string>
  where T : class, IPrototype
{
  public static implicit operator string(ProtoId<T> protoId) => protoId.Id;

  public static implicit operator ProtoId<T>(T proto) => new ProtoId<T>(proto.ID);

  public static implicit operator ProtoId<T>(string id) => new ProtoId<T>(id);

  public static implicit operator ProtoId<T>?(string? id)
  {
    return id != null ? new ProtoId<T>?(new ProtoId<T>(id)) : new ProtoId<T>?();
  }

  public bool Equals(string? other) => this.Id == other;

  public int CompareTo(ProtoId<T> other)
  {
    return string.Compare(this.Id, other.Id, StringComparison.Ordinal);
  }

  public string AsType() => this.Id;

  public override string ToString() => this.Id ?? string.Empty;
}
