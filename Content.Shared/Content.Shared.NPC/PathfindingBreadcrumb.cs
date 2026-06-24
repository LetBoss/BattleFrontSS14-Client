using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC;

[Serializable]
[NetSerializable]
public struct PathfindingBreadcrumb(Vector2i coordinates, int layer, int mask, float damage, PathfindingBreadcrumbFlag flags = PathfindingBreadcrumbFlag.None) : IEquatable<PathfindingBreadcrumb>
{
	public Vector2i Coordinates = coordinates;

	public PathfindingData Data = new PathfindingData(flags, layer, mask, damage);

	public static readonly PathfindingBreadcrumb Invalid = new PathfindingBreadcrumb
	{
		Data = new PathfindingData(PathfindingBreadcrumbFlag.None, -1, -1, 0f)
	};

	public bool Equivalent(PathfindingBreadcrumb other)
	{
		return Data.Equals(other.Data);
	}

	public bool Equals(PathfindingBreadcrumb other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (((Vector2i)(ref Coordinates)).Equals(other.Coordinates))
		{
			return Data.Equals(other.Data);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is PathfindingBreadcrumb other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return HashCode.Combine<Vector2i, PathfindingData>(Coordinates, Data);
	}
}
