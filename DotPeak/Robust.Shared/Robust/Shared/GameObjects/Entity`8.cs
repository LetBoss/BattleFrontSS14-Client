// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.Entity`8
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
public record struct Entity<T1, T2, T3, T4, T5, T6, T7, T8>(
  EntityUid owner,
  T1 comp1,
  T2 comp2,
  T3 comp3,
  T4 comp4,
  T5 comp5,
  T6 comp6,
  T7 comp7,
  T8 comp8) : IFluentEntityUid, IAsType<EntityUid>
  where T1 : IComponent?
  where T2 : IComponent?
  where T3 : IComponent?
  where T4 : IComponent?
  where T5 : IComponent?
  where T6 : IComponent?
  where T7 : IComponent?
  where T8 : IComponent?
{
  public EntityUid Owner = owner;
  public T1 Comp1 = comp1;
  public T2 Comp2 = comp2;
  public T3 Comp3 = comp3;
  public T4 Comp4 = comp4;
  public T5 Comp5 = comp5;
  public T6 Comp6 = comp6;
  public T7 Comp7 = comp7;
  public T8 Comp8 = comp8;

  readonly EntityUid IFluentEntityUid.FluentOwner => this.Owner;

  public static implicit operator Entity<T1, T2, T3, T4, T5, T6, T7, T8>(
    (EntityUid Owner, T1 Comp1, T2 Comp2, T3 Comp3, T4 Comp4, T5 Comp5, T6 Comp6, T7 Comp7, T8 Comp8) tuple)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(tuple.Owner, tuple.Comp1, tuple.Comp2, tuple.Comp3, tuple.Comp4, tuple.Comp5, tuple.Comp6, tuple.Comp7, tuple.Comp8);
  }

  public static implicit operator Entity<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?>(EntityUid owner)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(owner, default (T1), default (T2), default (T3), default (T4), default (T5), default (T6), default (T7), default (T8));
  }

  public static implicit operator EntityUid(Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent)
  {
    return ent.Owner;
  }

  public static implicit operator T1(Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent) => ent.Comp1;

  public static implicit operator T2(Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent) => ent.Comp2;

  public static implicit operator T3(Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent) => ent.Comp3;

  public static implicit operator T4(Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent) => ent.Comp4;

  public static implicit operator T5(Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent) => ent.Comp5;

  public static implicit operator T6(Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent) => ent.Comp6;

  public static implicit operator T7(Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent) => ent.Comp7;

  public static implicit operator T8(Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent) => ent.Comp8;

  public readonly void Deconstruct(
    out EntityUid owner,
    out T1 comp1,
    out T2 comp2,
    out T3 comp3,
    out T4 comp4,
    out T5 comp5,
    out T6 comp6,
    out T7 comp7,
    out T8 comp8)
  {
    owner = this.Owner;
    comp1 = this.Comp1;
    comp2 = this.Comp2;
    comp3 = this.Comp3;
    comp4 = this.Comp4;
    comp5 = this.Comp5;
    comp6 = this.Comp6;
    comp7 = this.Comp7;
    comp8 = this.Comp8;
  }

  public static implicit operator Entity<T1, T2?, T3?, T4?, T5?, T6?, T7?, T8?>(
    (EntityUid Owner, T1 Comp1) tuple)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(tuple.Owner, tuple.Comp1, default (T2), default (T3), default (T4), default (T5), default (T6), default (T7), default (T8));
  }

  public static implicit operator Entity<T1, T2, T3?, T4?, T5?, T6?, T7?, T8?>(
    (EntityUid Owner, T1 Comp1, T2 Comp2) tuple)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(tuple.Owner, tuple.Comp1, tuple.Comp2, default (T3), default (T4), default (T5), default (T6), default (T7), default (T8));
  }

  public static implicit operator Entity<T1, T2, T3, T4?, T5?, T6?, T7?, T8?>(
    (EntityUid Owner, T1 Comp1, T2 Comp2, T3 Comp3) tuple)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(tuple.Owner, tuple.Comp1, tuple.Comp2, tuple.Comp3, default (T4), default (T5), default (T6), default (T7), default (T8));
  }

  public static implicit operator Entity<T1, T2, T3, T4, T5?, T6?, T7?, T8?>(
    (EntityUid Owner, T1 Comp1, T2 Comp2, T3 Comp3, T4 Comp4) tuple)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(tuple.Owner, tuple.Comp1, tuple.Comp2, tuple.Comp3, tuple.Comp4, default (T5), default (T6), default (T7), default (T8));
  }

  public static implicit operator Entity<T1, T2, T3, T4, T5, T6?, T7?, T8?>(
    (EntityUid Owner, T1 Comp1, T2 Comp2, T3 Comp3, T4 Comp4, T5 Comp5) tuple)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(tuple.Owner, tuple.Comp1, tuple.Comp2, tuple.Comp3, tuple.Comp4, tuple.Comp5, default (T6), default (T7), default (T8));
  }

  public static implicit operator Entity<T1, T2, T3, T4, T5, T6, T7?, T8?>(
    (EntityUid Owner, T1 Comp1, T2 Comp2, T3 Comp3, T4 Comp4, T5 Comp5, T6 Comp6) tuple)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(tuple.Owner, tuple.Comp1, tuple.Comp2, tuple.Comp3, tuple.Comp4, tuple.Comp5, tuple.Comp6, default (T7), default (T8));
  }

  public static implicit operator Entity<T1, T2, T3, T4, T5, T6, T7, T8?>(
    (EntityUid Owner, T1 Comp1, T2 Comp2, T3 Comp3, T4 Comp4, T5 Comp5, T6 Comp6, T7 Comp7) tuple)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(tuple.Owner, tuple.Comp1, tuple.Comp2, tuple.Comp3, tuple.Comp4, tuple.Comp5, tuple.Comp6, tuple.Comp7, default (T8));
  }

  public static implicit operator Entity<T1, T2?, T3?, T4?, T5?, T6?, T7?, T8?>(Entity<T1> ent)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(ent.Owner, ent.Comp, default (T2), default (T3), default (T4), default (T5), default (T6), default (T7), default (T8));
  }

  public static implicit operator Entity<T1, T2, T3?, T4?, T5?, T6?, T7?, T8?>(Entity<T1, T2> ent)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(ent.Owner, ent.Comp1, ent.Comp2, default (T3), default (T4), default (T5), default (T6), default (T7), default (T8));
  }

  public static implicit operator Entity<T1, T2, T3, T4?, T5?, T6?, T7?, T8?>(Entity<T1, T2, T3> ent)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(ent.Owner, ent.Comp1, ent.Comp2, ent.Comp3, default (T4), default (T5), default (T6), default (T7), default (T8));
  }

  public static implicit operator Entity<T1, T2, T3, T4, T5?, T6?, T7?, T8?>(
    Entity<T1, T2, T3, T4> ent)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(ent.Owner, ent.Comp1, ent.Comp2, ent.Comp3, ent.Comp4, default (T5), default (T6), default (T7), default (T8));
  }

  public static implicit operator Entity<T1, T2, T3, T4, T5, T6?, T7?, T8?>(
    Entity<T1, T2, T3, T4, T5> ent)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(ent.Owner, ent.Comp1, ent.Comp2, ent.Comp3, ent.Comp4, ent.Comp5, default (T6), default (T7), default (T8));
  }

  public static implicit operator Entity<T1, T2, T3, T4, T5, T6, T7?, T8?>(
    Entity<T1, T2, T3, T4, T5, T6> ent)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(ent.Owner, ent.Comp1, ent.Comp2, ent.Comp3, ent.Comp4, ent.Comp5, ent.Comp6, default (T7), default (T8));
  }

  public static implicit operator Entity<T1, T2, T3, T4, T5, T6, T7, T8?>(
    Entity<T1, T2, T3, T4, T5, T6, T7> ent)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(ent.Owner, ent.Comp1, ent.Comp2, ent.Comp3, ent.Comp4, ent.Comp5, ent.Comp6, ent.Comp7, default (T8));
  }

  public static implicit operator Entity<T1>(Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent)
  {
    return new Entity<T1>(ent.Owner, ent.Comp1);
  }

  public static implicit operator Entity<T1, T2>(Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent)
  {
    return new Entity<T1, T2>(ent.Owner, ent.Comp1, ent.Comp2);
  }

  public static implicit operator Entity<T1, T2, T3>(Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent)
  {
    return new Entity<T1, T2, T3>(ent.Owner, ent.Comp1, ent.Comp2, ent.Comp3);
  }

  public static implicit operator Entity<T1, T2, T3, T4>(Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent)
  {
    return new Entity<T1, T2, T3, T4>(ent.Owner, ent.Comp1, ent.Comp2, ent.Comp3, ent.Comp4);
  }

  public static implicit operator Entity<T1, T2, T3, T4, T5>(
    Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent)
  {
    return new Entity<T1, T2, T3, T4, T5>(ent.Owner, ent.Comp1, ent.Comp2, ent.Comp3, ent.Comp4, ent.Comp5);
  }

  public static implicit operator Entity<T1, T2, T3, T4, T5, T6>(
    Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent)
  {
    return new Entity<T1, T2, T3, T4, T5, T6>(ent.Owner, ent.Comp1, ent.Comp2, ent.Comp3, ent.Comp4, ent.Comp5, ent.Comp6);
  }

  public static implicit operator Entity<T1, T2, T3, T4, T5, T6, T7>(
    Entity<T1, T2, T3, T4, T5, T6, T7, T8> ent)
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7>(ent.Owner, ent.Comp1, ent.Comp2, ent.Comp3, ent.Comp4, ent.Comp5, ent.Comp6, ent.Comp7);
  }

  public override readonly int GetHashCode() => this.Owner.GetHashCode();

  public readonly Entity<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?> AsNullable()
  {
    return new Entity<T1, T2, T3, T4, T5, T6, T7, T8>(this.Owner, this.Comp1, this.Comp2, this.Comp3, this.Comp4, this.Comp5, this.Comp6, this.Comp7, this.Comp8);
  }

  public readonly EntityUid AsType() => this.Owner;

  [CompilerGenerated]
  public readonly bool Equals(Entity<
  #nullable disable
  T1, T2, T3, T4, T5, T6, T7, T8> other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.Owner, other.Owner) && EqualityComparer<T1>.Default.Equals(this.Comp1, other.Comp1) && EqualityComparer<T2>.Default.Equals(this.Comp2, other.Comp2) && EqualityComparer<T3>.Default.Equals(this.Comp3, other.Comp3) && EqualityComparer<T4>.Default.Equals(this.Comp4, other.Comp4) && EqualityComparer<T5>.Default.Equals(this.Comp5, other.Comp5) && EqualityComparer<T6>.Default.Equals(this.Comp6, other.Comp6) && EqualityComparer<T7>.Default.Equals(this.Comp7, other.Comp7) && EqualityComparer<T8>.Default.Equals(this.Comp8, other.Comp8);
  }
}
