using System;
using System.Linq;
using Content.Shared.SubFloor;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;

namespace Content.Client.SubFloor;

public sealed class TrayScanRevealSystem : EntitySystem
{
	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedMapSystem _map;

	public bool IsUnderRevealingEntity(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? grid = _transform.GetGrid(Entity<TransformComponent>.op_Implicit(uid));
		if (!grid.HasValue)
		{
			return false;
		}
		MapGridComponent item = ((EntitySystem)this).Comp<MapGridComponent>(grid.Value);
		Vector2i gridOrMapTilePosition = _transform.GetGridOrMapTilePosition(uid, (TransformComponent)null);
		return HasTrayScanReveal(Entity<MapGridComponent>.op_Implicit((grid.Value, item)), gridOrMapTilePosition);
	}

	private bool HasTrayScanReveal(Entity<MapGridComponent> ent, Vector2i position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return _map.GetAnchoredEntities(ent, position).Any((Func<EntityUid, bool>)base.HasComp<TrayScanRevealComponent>);
	}
}
