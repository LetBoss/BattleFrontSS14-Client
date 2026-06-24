// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.ComponentTreeEntry`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Robust.Shared.Physics;

public readonly struct ComponentTreeEntry<T> : 
  IEquatable<ComponentTreeEntry<T>>,
  IComparable<ComponentTreeEntry<T>>
  where T : IComponent
{
  public T Component { get; init; }

  public TransformComponent Transform { get; init; }

  public EntityUid Uid => this.Component.Owner;

  public int CompareTo(ComponentTreeEntry<T> other) => this.Uid.CompareTo(other.Uid);

  public bool Equals(ComponentTreeEntry<T> other) => this.Uid.Equals(other.Uid);

  public void Deconstruct(out T component, out TransformComponent xform)
  {
    component = this.Component;
    xform = this.Transform;
  }

  public static implicit operator Entity<T, TransformComponent>(ComponentTreeEntry<T> entry)
  {
    return new Entity<T, TransformComponent>(entry.Uid, entry.Component, entry.Transform);
  }

  public static implicit operator ComponentTreeEntry<T>((T, TransformComponent) tuple)
  {
    return new ComponentTreeEntry<T>()
    {
      Component = tuple.Item1,
      Transform = tuple.Item2
    };
  }
}
