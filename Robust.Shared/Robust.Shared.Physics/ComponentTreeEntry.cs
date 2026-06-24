using System;
using Robust.Shared.GameObjects;

namespace Robust.Shared.Physics;

public readonly struct ComponentTreeEntry<T> : IEquatable<ComponentTreeEntry<T>>, IComparable<ComponentTreeEntry<T>> where T : IComponent
{
	public T Component { get; init; }

	public TransformComponent Transform { get; init; }

	public EntityUid Uid => Component.Owner;

	public int CompareTo(ComponentTreeEntry<T> other)
	{
		return Uid.CompareTo(other.Uid);
	}

	public bool Equals(ComponentTreeEntry<T> other)
	{
		return Uid.Equals(other.Uid);
	}

	public void Deconstruct(out T component, out TransformComponent xform)
	{
		component = Component;
		xform = Transform;
	}

	public static implicit operator Entity<T, TransformComponent>(ComponentTreeEntry<T> entry)
	{
		return new Entity<T, TransformComponent>(entry.Uid, entry.Component, entry.Transform);
	}

	public static implicit operator ComponentTreeEntry<T>((T, TransformComponent) tuple)
	{
		ComponentTreeEntry<T> result = default(ComponentTreeEntry<T>);
		(result.Component, result.Transform) = tuple;
		return result;
	}
}
