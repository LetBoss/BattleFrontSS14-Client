// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.Entity`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Toolshed.TypeParsers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.GameObjects;

[NotYamlSerializable]
public record struct Entity<T>(EntityUid owner, T comp) : IFluentEntityUid, IAsType<EntityUid> where T : IComponent?
{
  public EntityUid Owner = owner;
  public T Comp = comp;

  readonly EntityUid IFluentEntityUid.FluentOwner => this.Owner;

  public static implicit operator Entity<T>((EntityUid Owner, T Comp) tuple)
  {
    return new Entity<T>(tuple.Owner, tuple.Comp);
  }

  public static implicit operator Entity<T?>(EntityUid owner) => new Entity<T>(owner, default (T));

  public static implicit operator EntityUid(Entity<T> ent) => ent.Owner;

  public static implicit operator T(Entity<T> ent) => ent.Comp;

  public readonly void Deconstruct(out EntityUid owner, out T comp)
  {
    owner = this.Owner;
    comp = this.Comp;
  }

  public override readonly int GetHashCode() => this.Owner.GetHashCode();

  public readonly Entity<T?> AsNullable() => new Entity<T>(this.Owner, this.Comp);

  public readonly EntityUid AsType() => this.Owner;

  [CompilerGenerated]
  public readonly bool Equals(Entity<
  #nullable disable
  T> other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.Owner, other.Owner) && EqualityComparer<T>.Default.Equals(this.Comp, other.Comp);
  }
}
