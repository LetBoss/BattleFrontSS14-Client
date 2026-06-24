using System;
using System.Collections.Generic;
using Content.Shared.Directions;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Abilities.Goliath;

public sealed class GoliathTentacleSystem : EntitySystem
{
	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GoliathSummonTentacleAction>((EntityEventHandler<GoliathSummonTentacleAction>)OnSummonAction, (Type[])null, (Type[])null);
	}

	private void OnSummonAction(GoliathSummonTentacleAction args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		_popup.PopupPredicted(base.Loc.GetString("tentacle-ability-use-popup", (ValueTuple<string, object>)("entity", args.Performer)), args.Performer, args.Performer, PopupType.SmallCaution);
		_stun.TryStun(args.Performer, TimeSpan.FromSeconds(0.800000011920929), refresh: false);
		EntityCoordinates coords = args.Target;
		List<EntityCoordinates> spawnPos = new List<EntityCoordinates>();
		spawnPos.Add(coords);
		List<Direction> dirs = new List<Direction>();
		dirs.AddRange(args.OffsetDirections);
		for (int i = 0; i < 3; i++)
		{
			Direction dir = RandomExtensions.PickAndTake<Direction>(_random, (IList<Direction>)dirs);
			spawnPos.Add(coords.Offset(dir));
		}
		EntityUid? grid = _transform.GetGrid(coords);
		if (!grid.HasValue)
		{
			return;
		}
		EntityUid grid2 = grid.GetValueOrDefault();
		MapGridComponent gridComp = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(grid2, ref gridComp))
		{
			return;
		}
		TileRef tileRef = default(TileRef);
		foreach (EntityCoordinates pos in spawnPos)
		{
			if (_map.TryGetTileRef(grid2, gridComp, pos, ref tileRef) && !_turf.IsSpace(tileRef) && !_turf.IsTileBlocked(tileRef, CollisionGroup.Impassable) && _net.IsServer)
			{
				((EntitySystem)this).Spawn(EntProtoId.op_Implicit(args.EntityId), pos);
			}
		}
		((HandledEntityEventArgs)args).Handled = true;
	}
}
