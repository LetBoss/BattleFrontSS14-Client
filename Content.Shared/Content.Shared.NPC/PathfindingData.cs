using System;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC;

[Serializable]
[NetSerializable]
public struct PathfindingData : IEquatable<PathfindingData>
{
	public PathfindingBreadcrumbFlag Flags;

	public int CollisionLayer;

	public int CollisionMask;

	public float Damage;

	public bool IsFreeSpace
	{
		get
		{
			if (Flags == PathfindingBreadcrumbFlag.None)
			{
				return Damage.Equals(0f);
			}
			return false;
		}
	}

	public PathfindingData(PathfindingBreadcrumbFlag flag, int layer, int mask, float damage)
	{
		Flags = flag;
		CollisionLayer = layer;
		CollisionMask = mask;
		Damage = damage;
	}

	public bool IsEquivalent(PathfindingData other)
	{
		if (CollisionLayer.Equals(other.CollisionLayer) && CollisionMask.Equals(other.CollisionMask))
		{
			return Flags.Equals(other.Flags);
		}
		return false;
	}

	public bool Equals(PathfindingData other)
	{
		if (CollisionLayer.Equals(other.CollisionLayer) && CollisionMask.Equals(other.CollisionMask) && Flags.Equals(other.Flags))
		{
			return Damage.Equals(other.Damage);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is PathfindingData other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine((int)Flags, CollisionLayer, CollisionMask);
	}
}
