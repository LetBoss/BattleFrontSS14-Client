// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.EntityStringRepresentation
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Player;
using System;

#nullable enable
namespace Robust.Shared.GameObjects;

public readonly record struct EntityStringRepresentation(
  EntityUid Uid,
  NetEntity Nuid,
  bool Deleted,
  string? Name = null,
  string? Prototype = null,
  ICommonSession? Session = null) : IFormattable
{
  public EntityStringRepresentation(Entity<MetaDataComponent> entity)
    : this(entity.Owner, entity.Comp)
  {
  }

  public EntityStringRepresentation(EntityUid uid, MetaDataComponent meta, ActorComponent? actor = null)
    : this(uid, meta.NetEntity, meta.EntityDeleted, meta.EntityName, meta.EntityPrototype?.ID, actor?.PlayerSession)
  {
  }

  public override string ToString()
  {
    if (this.Deleted && this.Name == null)
      return $"{this.Uid}/n{this.Nuid}D";
    return $"{this.Name} ({this.Uid}/n{this.Nuid}{(this.Prototype != null ? ", " + this.Prototype : "")}{(this.Session != null ? ", " + this.Session.Name : "")}){(this.Deleted ? "D" : "")}";
  }

  public string ToString(string? format, IFormatProvider? formatProvider) => this.ToString();

  public static implicit operator string(EntityStringRepresentation rep) => rep.ToString();
}
