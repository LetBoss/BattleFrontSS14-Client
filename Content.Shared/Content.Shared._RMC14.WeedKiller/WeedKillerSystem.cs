using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Dropship;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared.Coordinates;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.WeedKiller;

public sealed class WeedKillerSystem : EntitySystem
{
	[Dependency]
	private AreaSystem _area;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private SharedMarineAnnounceSystem _marineAnnounce;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private RMCCameraShakeSystem _rmcCameraShake;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	private EntityQuery<DeletedByWeedKillerComponent> _deletedByWeedKillerQuery;

	private static readonly EntProtoId WeedKiller = EntProtoId.op_Implicit("RMCGasWeedKiller");

	private TimeSpan _dropshipDelay;

	private TimeSpan _disableDuration;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_deletedByWeedKillerQuery = ((EntitySystem)this).GetEntityQuery<DeletedByWeedKillerComponent>();
		((EntitySystem)this).SubscribeLocalEvent<DropshipLaunchedFromWarshipEvent>((EntityEventRefHandler<DropshipLaunchedFromWarshipEvent>)OnDropshipLaunchedFromWarship, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCWeedKillerDropshipDelaySeconds, (Action<int>)delegate(int v)
		{
			_dropshipDelay = TimeSpan.FromSeconds(v);
		}, true);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCWeedKillerDisableDurationMinutes, (Action<int>)delegate(int v)
		{
			_disableDuration = TimeSpan.FromMinutes(v);
		}, true);
	}

	private void OnDropshipLaunchedFromWarship(ref DropshipLaunchedFromWarshipEvent ev)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || ((EntitySystem)this).Count<WeedKillerComponent>() > 0)
		{
			return;
		}
		EntityUid? destination = ev.Dropship.Comp.Destination;
		if (destination.HasValue)
		{
			EntityUid destination2 = destination.GetValueOrDefault();
			EntityCoordinates coordinates = destination2.ToCoordinates();
			if (_area.TryGetArea(coordinates, out Entity<AreaComponent>? lzArea, out EntityPrototype _) && !string.IsNullOrWhiteSpace(lzArea.Value.Comp.LinkedLz))
			{
				CreateWeedKiller(Entity<DropshipComponent>.op_Implicit(ev.Dropship), coordinates);
			}
		}
	}

	public void CreateWeedKiller(EntityUid dropship, EntityCoordinates coordinates)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid id = ((EntitySystem)this).Spawn((string)null, (ComponentRegistry)null, true);
		WeedKillerComponent comp = ((EntitySystem)this).EnsureComp<WeedKillerComponent>(id);
		comp.DeployAt = _timing.CurTime + _dropshipDelay;
		comp.DisableAt = _timing.CurTime + _dropshipDelay + _disableDuration;
		comp.Dropship = dropship;
		((EntitySystem)this).Dirty(id, (IComponent)(object)comp, (MetaDataComponent)null);
		if (!_area.TryGetArea(coordinates, out Entity<AreaComponent>? lzArea, out EntityPrototype _))
		{
			return;
		}
		EntityQueryEnumerator<AreaComponent> areas = ((EntitySystem)this).EntityQueryEnumerator<AreaComponent>();
		EntityUid areaId = default(EntityUid);
		AreaComponent areaComp = default(AreaComponent);
		while (areas.MoveNext(ref areaId, ref areaComp))
		{
			string? linkedLz = areaComp.LinkedLz;
			if (linkedLz != null && linkedLz.Contains(','))
			{
				if (!(from x in areaComp.LinkedLz.Split(',')
					select x.Trim()).Contains<string>(lzArea.Value.Comp.LinkedLz))
				{
					continue;
				}
			}
			else if (areaComp.LinkedLz != lzArea.Value.Comp.LinkedLz)
			{
				continue;
			}
			EntityPrototype obj = ((EntitySystem)this).Prototype(areaId, (MetaDataComponent)null);
			string proto = ((obj != null) ? obj.ID : null);
			if (proto != null)
			{
				comp.AreaPrototypes.Add(EntProtoId.op_Implicit(proto));
			}
			comp.LinkedAreas.Add(areaId);
		}
		EntityUid? gridId = _transform.GetGrid(coordinates);
		MapGridComponent grid = default(MapGridComponent);
		AreaGridComponent areaGrid = default(AreaGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid) || !((EntitySystem)this).TryComp<AreaGridComponent>(gridId, ref areaGrid))
		{
			return;
		}
		foreach (var (position, areaId2) in areaGrid.Areas)
		{
			if (comp.AreaPrototypes.Contains(EntProtoId<AreaComponent>.op_Implicit(areaId2)))
			{
				comp.Positions.Add((Entity<MapGridComponent>.op_Implicit((gridId.Value, grid)), position));
			}
		}
	}

	private void KillWeeds(Entity<WeedKillerComponent> killer)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		AreaComponent area = default(AreaComponent);
		foreach (EntityUid areaId in killer.Comp.LinkedAreas)
		{
			if (((EntitySystem)this).TryComp<AreaComponent>(areaId, ref area))
			{
				area.WeedKilling = true;
				((EntitySystem)this).Dirty(areaId, (IComponent)(object)area, (MetaDataComponent)null);
			}
		}
		EntityUid? dropship = killer.Comp.Dropship;
		if (dropship.HasValue)
		{
			Filter mapFilter = Filter.BroadcastMap(_transform.GetMapId(Entity<TransformComponent>.op_Implicit(dropship.Value)));
			_audio.PlayGlobal(killer.Comp.Sound, mapFilter, false, (AudioParams?)null);
			_rmcCameraShake.ShakeCamera(mapFilter, 3, 1);
			_marineAnnounce.AnnounceARESStaging(null, base.Loc.GetString("rmc-weed-killer-deploying", (ValueTuple<string, object>)("dropship", ((EntitySystem)this).Name(dropship.Value, (MetaDataComponent)null))));
		}
		foreach (var position in killer.Comp.Positions)
		{
			((EntitySystem)this).Spawn(EntProtoId.op_Implicit(WeedKiller), _map.ToCoordinates(Entity<MapGridComponent>.op_Implicit(position.Grid), position.Indices, Entity<MapGridComponent>.op_Implicit(position.Grid)));
			RMCAnchoredEntitiesEnumerator anchored = _rmcMap.GetAnchoredEntitiesEnumerator(position.Grid, position.Indices, null, (DirectionFlag)0);
			EntityUid anchoredId;
			while (anchored.MoveNext(out anchoredId))
			{
				if (_deletedByWeedKillerQuery.HasComp(anchoredId))
				{
					((EntitySystem)this).QueueDel((EntityUid?)anchoredId);
				}
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<WeedKillerComponent> killer = ((EntitySystem)this).EntityQueryEnumerator<WeedKillerComponent>();
		EntityUid uid = default(EntityUid);
		WeedKillerComponent comp = default(WeedKillerComponent);
		AreaComponent area = default(AreaComponent);
		while (killer.MoveNext(ref uid, ref comp))
		{
			if (!comp.Deployed)
			{
				if (time < comp.DeployAt)
				{
					continue;
				}
				comp.Deployed = true;
				((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
				KillWeeds(Entity<WeedKillerComponent>.op_Implicit((uid, comp)));
			}
			if (comp.Disabled || time < comp.DisableAt)
			{
				continue;
			}
			comp.Disabled = true;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
			foreach (EntityUid areaId in comp.LinkedAreas)
			{
				if (((EntitySystem)this).TryComp<AreaComponent>(areaId, ref area))
				{
					area.WeedKilling = false;
					((EntitySystem)this).Dirty(areaId, (IComponent)(object)area, (MetaDataComponent)null);
				}
			}
		}
	}
}
