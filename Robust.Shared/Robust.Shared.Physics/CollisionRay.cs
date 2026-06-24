using System;
using System.Numerics;
using Robust.Shared.Maths;

namespace Robust.Shared.Physics;

[Serializable]
public readonly struct CollisionRay : IEquatable<CollisionRay>
{
	private readonly Ray _ray;

	private readonly int _collisionMask;

	public Vector2 Position => _ray.Position;

	public Vector2 Direction => _ray.Direction;

	public int CollisionMask => _collisionMask;

	public CollisionRay(Vector2 position, Vector2 direction, int collisionMask)
	{
		_ray = new Ray(position, direction);
		_collisionMask = collisionMask;
	}

	public bool Intersects(Box2 box, out float distance, out Vector2 hitPos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _ray.Intersects(box, out distance, out hitPos);
	}

	public bool Equals(CollisionRay other)
	{
		if (Position.Equals(other.Position))
		{
			return Direction.Equals(other.Direction);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj is CollisionRay other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (Position.GetHashCode() * 397) ^ Direction.GetHashCode();
	}

	public static bool operator ==(CollisionRay a, CollisionRay b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(CollisionRay a, CollisionRay b)
	{
		return !(a == b);
	}

	public static implicit operator Ray(CollisionRay a)
	{
		return a._ray;
	}
}
