// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.Entity`4
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
public record struct Entity<T1, T2, T3, T4>(
  EntityUid owner,
  T1 comp1,
  T2 comp2,
  T3 comp3,
  T4 comp4) : IFluentEntityUid, IAsType<EntityUid>
  where T1 : IComponent?
  where T2 : IComponent?
  where T3 : IComponent?
  where T4 : IComponent?
{
  public EntityUid Owner = owner;
  public T1 Comp1 = comp1;
  public T2 Comp2 = comp2;
  public T3 Comp3 = comp3;
  public T4 Comp4 = comp4;

  readonly EntityUid IFluentEntityUid.FluentOwner => this.Owner;

  public static implicit operator Entity<T1, T2, T3, T4>(
    (EntityUid Owner, T1 Comp1, T2 Comp2, T3 Comp3, T4 Comp4) tuple)
  {
    return new Entity<T1, T2, T3, T4>(tuple.Owner, tuple.Comp1, tuple.Comp2, tuple.Comp3, tuple.Comp4);
  }

  public static implicit operator Entity<T1?, T2?, T3?, T4?>(EntityUid owner)
  {
    return new Entity<T1, T2, T3, T4>(owner, default (T1), default (T2), default (T3), default (T4));
  }

  public static implicit operator EntityUid(Entity<T1, T2, T3, T4> ent) => ent.Owner;

  public static implicit operator T1(Entity<T1, T2, T3, T4> ent) => ent.Comp1;

  public static implicit operator T2(Entity<T1, T2, T3, T4> ent) => ent.Comp2;

  public static implicit operator T3(Entity<T1, T2, T3, T4> ent) => ent.Comp3;

  public static implicit operator T4(Entity<T1, T2, T3, T4> ent) => ent.Comp4;

  public readonly void Deconstruct(
    out EntityUid owner,
    out T1 comp1,
    out T2 comp2,
    out T3 comp3,
    out T4 comp4)
  {
    owner = this.Owner;
    comp1 = this.Comp1;
    comp2 = this.Comp2;
    comp3 = this.Comp3;
    comp4 = this.Comp4;
  }

  public static implicit operator Entity<T1, T2?, T3?, T4?>((EntityUid Owner, T1 Comp1) tuple)
  {
    return new Entity<T1, T2, T3, T4>(tuple.Owner, tuple.Comp1, default (T2), default (T3), default (T4));
  }

  public static implicit operator Entity<T1, T2, T3?, T4?>(
    (EntityUid Owner, T1 Comp1, T2 Comp2) tuple)
  {
    return new Entity<T1, T2, T3, T4>(tuple.Owner, tuple.Comp1, tuple.Comp2, default (T3), default (T4));
  }

  public static implicit operator Entity<T1, T2, T3, T4?>(
    (EntityUid Owner, T1 Comp1, T2 Comp2, T3 Comp3) tuple)
  {
    return new Entity<T1, T2, T3, T4>(tuple.Owner, tuple.Comp1, tuple.Comp2, tuple.Comp3, default (T4));
  }

  public static implicit operator Entity<T1, T2?, T3?, T4?>(Entity<T1> ent)
  {
    return new Entity<T1, T2, T3, T4>(ent.Owner, ent.Comp, default (T2), default (T3), default (T4));
  }

  public static implicit operator Entity<T1, T2, T3?, T4?>(Entity<T1, T2> ent)
  {
    return new Entity<T1, T2, T3, T4>(ent.Owner, ent.Comp1, ent.Comp2, default (T3), default (T4));
  }

  public static implicit operator Entity<T1, T2, T3, T4?>(Entity<T1, T2, T3> ent)
  {
    return new Entity<T1, T2, T3, T4>(ent.Owner, ent.Comp1, ent.Comp2, ent.Comp3, default (T4));
  }

  public static implicit operator Entity<T1>(Entity<T1, T2, T3, T4> ent)
  {
    return new Entity<T1>(ent.Owner, ent.Comp1);
  }

  public static implicit operator Entity<T1, T2>(Entity<T1, T2, T3, T4> ent)
  {
    return new Entity<T1, T2>(ent.Owner, ent.Comp1, ent.Comp2);
  }

  public static implicit operator Entity<T1, T2, T3>(Entity<T1, T2, T3, T4> ent)
  {
    return new Entity<T1, T2, T3>(ent.Owner, ent.Comp1, ent.Comp2, ent.Comp3);
  }

  public override readonly int GetHashCode() => this.Owner.GetHashCode();

  public readonly Entity<T1?, T2?, T3?, T4?> AsNullable()
  {
    return new Entity<T1, T2, T3, T4>(this.Owner, this.Comp1, this.Comp2, this.Comp3, this.Comp4);
  }

  public readonly EntityUid AsType() => this.Owner;

  [CompilerGenerated]
  public readonly bool Equals(Entity<
  #nullable disable
  T1, T2, T3, T4> other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.Owner, other.Owner) && EqualityComparer<T1>.Default.Equals(this.Comp1, other.Comp1) && EqualityComparer<T2>.Default.Equals(this.Comp2, other.Comp2) && EqualityComparer<T3>.Default.Equals(this.Comp3, other.Comp3) && EqualityComparer<T4>.Default.Equals(this.Comp4, other.Comp4);
  }
}
