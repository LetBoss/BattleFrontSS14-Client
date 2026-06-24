using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.OnCollide;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Random;

namespace Content.Shared._RMC14.Vehicle;

public sealed class RMCVehicleDestroyedFireSystem : EntitySystem
{
	[Dependency]
	private SharedRMCFlammableSystem _flammable;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedOnCollideSystem _onCollide;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private VehicleSystem _vehicles;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCVehicleDestroyedFireComponent, RMCVehicleFrameDestroyedEvent>((EntityEventRefHandler<RMCVehicleDestroyedFireComponent, RMCVehicleFrameDestroyedEvent>)OnVehicleFrameDestroyed, (Type[])null, (Type[])null);
	}

	private void OnVehicleFrameDestroyed(Entity<RMCVehicleDestroyedFireComponent> ent, ref RMCVehicleFrameDestroyedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && !(args.Vehicle != ent.Owner))
		{
			IgniteInterior(ent);
			IgniteExterior(ent);
		}
	}

	private void IgniteInterior(Entity<RMCVehicleDestroyedFireComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		MapGridComponent interiorGridComp = default(MapGridComponent);
		if (!_vehicles.TryGetInteriorGrid(ent.Owner, out var interiorGrid) || !((EntitySystem)this).TryComp<MapGridComponent>(interiorGrid, ref interiorGridComp))
		{
			return;
		}
		Entity<CollideChainComponent> chain = _onCollide.SpawnChain();
		foreach (TileRef tile in _map.GetAllTiles(interiorGrid, interiorGridComp, true))
		{
			_flammable.SpawnFire(_map.GridTileToLocal(interiorGrid, interiorGridComp, tile.GridIndices), ent.Comp.InteriorFire, Entity<CollideChainComponent>.op_Implicit(chain), 1, null, null, out var _);
		}
	}

	private void IgniteExterior(Entity<RMCVehicleDestroyedFireComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? gridUid = ((EntitySystem)this).Transform(ent.Owner).GridUid;
		if (!gridUid.HasValue)
		{
			return;
		}
		EntityUid gridUid2 = gridUid.GetValueOrDefault();
		MapGridComponent gridComp = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(gridUid2, ref gridComp))
		{
			return;
		}
		Box2 vehicleBounds = _lookup.GetWorldAABB(ent.Owner, (TransformComponent)null);
		int padding = gridComp.TileSize * Math.Max(ent.Comp.ExteriorPaddingTiles, 0);
		Box2 outerBounds = default(Box2);
		((Box2)(ref outerBounds))._002Ector(vehicleBounds.Left - (float)padding, vehicleBounds.Bottom - (float)padding, vehicleBounds.Right + (float)padding, vehicleBounds.Top + (float)padding);
		HashSet<Vector2i> blocked = new HashSet<Vector2i>();
		foreach (TileRef tile in _map.GetTilesIntersecting(gridUid2, gridComp, vehicleBounds, true, (Predicate<TileRef>)null))
		{
			blocked.Add(tile.GridIndices);
		}
		Entity<CollideChainComponent> chain = _onCollide.SpawnChain();
		foreach (TileRef tile2 in _map.GetTilesIntersecting(gridUid2, gridComp, outerBounds, true, (Predicate<TileRef>)null))
		{
			if (!blocked.Contains(tile2.GridIndices) && RandomExtensions.Prob(_random, ent.Comp.ExteriorFireChance))
			{
				_flammable.SpawnFire(_map.GridTileToLocal(gridUid2, gridComp, tile2.GridIndices), ent.Comp.ExteriorFire, Entity<CollideChainComponent>.op_Implicit(chain), 1, null, null, out var _);
			}
		}
	}
}
