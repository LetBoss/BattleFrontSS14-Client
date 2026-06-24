using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;

namespace Content.Shared.Maps;

public static class TurfLookupExtensions
{
	public static void GetEntitiesInTile(this EntityLookupSystem lookupSystem, TileRef turf, HashSet<EntityUid> intersecting, LookupFlags flags = (LookupFlags)4)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Box2Rotated bounds = lookupSystem.GetWorldBounds(turf, (Matrix3x2?)null, (Angle?)null);
		bounds.Box = ((Box2)(ref bounds.Box)).Scale(0.9f);
		lookupSystem.GetEntitiesIntersecting(turf.GridUid, bounds, intersecting, flags);
	}

	public static HashSet<EntityUid> GetEntitiesInTile(this EntityLookupSystem lookupSystem, TileRef turf, LookupFlags flags = (LookupFlags)4)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		HashSet<EntityUid> intersecting = new HashSet<EntityUid>();
		lookupSystem.GetEntitiesInTile(turf, intersecting, flags);
		return intersecting;
	}
}
