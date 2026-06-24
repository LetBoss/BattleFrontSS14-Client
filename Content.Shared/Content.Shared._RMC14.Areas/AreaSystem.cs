using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.GameStates;
using Content.Shared._RMC14.Warps;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.GameTicking;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Areas;

public sealed class AreaSystem : EntitySystem
{
	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private SharedGameTicker _gameTicker;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private SharedRMCPvsSystem _rmcPvs;

	[Dependency]
	private SharedRMCWarpSystem _rmcWarp;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private TurfSystem _turf;

	private static readonly ProtoId<TagPrototype> WallTag = ProtoId<TagPrototype>.op_Implicit("Wall");

	private EntityQuery<AreaComponent> _areaQuery;

	private EntityQuery<AreaGridComponent> _areaGridQuery;

	private EntityQuery<AreaLabelComponent> _areaLabelQuery;

	private EntityQuery<DamageableComponent> _damageableQuery;

	private EntityQuery<MapGridComponent> _mapGridQuery;

	private EntityQuery<MinimapColorComponent> _minimapColorQuery;

	private EntityQuery<XenoConstructComponent> _xenoConstruct;

	private readonly List<EntityUid> _toRender = new List<EntityUid>();

	private TimeSpan _earlySpreadHiveTime;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		_areaQuery = ((EntitySystem)this).GetEntityQuery<AreaComponent>();
		_areaGridQuery = ((EntitySystem)this).GetEntityQuery<AreaGridComponent>();
		_areaLabelQuery = ((EntitySystem)this).GetEntityQuery<AreaLabelComponent>();
		_damageableQuery = ((EntitySystem)this).GetEntityQuery<DamageableComponent>();
		_mapGridQuery = ((EntitySystem)this).GetEntityQuery<MapGridComponent>();
		_minimapColorQuery = ((EntitySystem)this).GetEntityQuery<MinimapColorComponent>();
		_xenoConstruct = ((EntitySystem)this).GetEntityQuery<XenoConstructComponent>();
		((EntitySystem)this).SubscribeLocalEvent<AreaGridComponent, MapInitEvent>((EntityEventRefHandler<AreaGridComponent, MapInitEvent>)OnAreaGridMapInit, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<int>(((EntitySystem)this).Subs, _config, RMCCVars.RMCHiveSpreadEarlyMinutes, (Action<int>)delegate(int v)
		{
			_earlySpreadHiveTime = TimeSpan.FromMinutes(v);
		}, true);
	}

	private void OnAreaGridMapInit(Entity<AreaGridComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		_toRender.Add(Entity<AreaGridComponent>.op_Implicit(ent));
		foreach (EntProtoId<AreaComponent> area in ent.Comp.Areas.Values.DistinctBy<EntProtoId<AreaComponent>, string>((EntProtoId<AreaComponent> a) => a.Id))
		{
			if (ent.Comp.AreaEntities.ContainsKey(area))
			{
				((EntitySystem)this).Log.Warning($"Duplicate area {area} found in entity {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<AreaGridComponent>.op_Implicit(ent), (MetaDataComponent)null)}");
			}
			else
			{
				EnsureAreaEntityExists(ent.Comp, area);
			}
		}
	}

	private void EnsureAreaEntityExists(AreaGridComponent areaGrid, EntProtoId<AreaComponent> area)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!areaGrid.AreaEntities.ContainsKey(area))
		{
			EntityUid areaEnt = ((EntitySystem)this).Spawn(EntProtoId<AreaComponent>.op_Implicit(area), MapCoordinates.Nullspace, (ComponentRegistry)null, default(Angle));
			areaGrid.AreaEntities[area] = areaEnt;
			_rmcPvs.AddGlobalOverride(areaEnt);
		}
	}

	public void ReplaceArea(AreaGridComponent areaGrid, Vector2i position, EntProtoId<AreaComponent> area)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		areaGrid.Areas[position] = area;
		EnsureAreaEntityExists(areaGrid, area);
	}

	public bool TryGetArea(Entity<MapGridComponent, AreaGridComponent?> grid, Vector2i indices, [NotNullWhen(true)] out Entity<AreaComponent>? area, [NotNullWhen(true)] out EntityPrototype? areaPrototype)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		area = null;
		areaPrototype = null;
		if (!((EntitySystem)this).Resolve<AreaGridComponent>(Entity<MapGridComponent, AreaGridComponent>.op_Implicit(grid), ref grid.Comp2, false))
		{
			return false;
		}
		if (!grid.Comp2.Areas.TryGetValue(indices, out EntProtoId<AreaComponent> areaProtoId))
		{
			return false;
		}
		if (!_prototypes.TryIndex(EntProtoId<AreaComponent>.op_Implicit(areaProtoId), ref areaPrototype))
		{
			return false;
		}
		AreaComponent areaComp = default(AreaComponent);
		if (!grid.Comp2.AreaEntities.TryGetValue(areaProtoId, out var areaEnt) || !((EntitySystem)this).TryComp<AreaComponent>(areaEnt, ref areaComp))
		{
			return false;
		}
		area = Entity<AreaComponent>.op_Implicit((areaEnt, areaComp));
		return true;
	}

	public bool TryGetArea(EntityCoordinates coordinates, [NotNullWhen(true)] out Entity<AreaComponent>? area, [NotNullWhen(true)] out EntityPrototype? areaPrototype)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		area = null;
		areaPrototype = null;
		EntityUid? grid = _transform.GetGrid(coordinates);
		if (grid.HasValue)
		{
			EntityUid gridId = grid.GetValueOrDefault();
			MapGridComponent grid2 = default(MapGridComponent);
			AreaGridComponent areaGrid = default(AreaGridComponent);
			if (_mapGridQuery.TryComp(gridId, ref grid2) && _areaGridQuery.TryComp(gridId, ref areaGrid))
			{
				Vector2i indices = _map.CoordinatesToTile(gridId, grid2, coordinates);
				return TryGetArea(Entity<MapGridComponent, AreaGridComponent>.op_Implicit((gridId, grid2, areaGrid)), indices, out area, out areaPrototype);
			}
		}
		return false;
	}

	public bool TryGetArea(MapCoordinates coordinates, [NotNullWhen(true)] out Entity<AreaComponent>? area, [NotNullWhen(true)] out EntityPrototype? areaPrototype)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return TryGetArea(_transform.ToCoordinates(coordinates), out area, out areaPrototype);
	}

	public bool TryGetArea(EntityUid coordinates, [NotNullWhen(true)] out Entity<AreaComponent>? area, [NotNullWhen(true)] out EntityPrototype? areaPrototype)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return TryGetArea(coordinates.ToCoordinates(), out area, out areaPrototype);
	}

	public bool TryGetAllAreas(EntityCoordinates coordinates, [NotNullWhen(true)] out Entity<AreaGridComponent>? areaGrid)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		areaGrid = null;
		EntityUid? map = _transform.GetMap(coordinates);
		if (map.HasValue)
		{
			EntityUid mapId = map.GetValueOrDefault();
			AreaGridComponent areaGridComp = default(AreaGridComponent);
			if (_areaGridQuery.TryComp(mapId, ref areaGridComp))
			{
				areaGrid = Entity<AreaGridComponent>.op_Implicit((mapId, areaGridComp));
				return true;
			}
		}
		return false;
	}

	public bool BioscanBlocked(EntityUid coordinates, out string? name)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		name = null;
		if (!TryGetArea(coordinates, out Entity<AreaComponent>? area, out EntityPrototype areaProto))
		{
			return false;
		}
		name = areaProto.Name;
		return area.Value.Comp.AvoidBioscan;
	}

	public bool IsWeatherEnabled(Entity<MapGridComponent> grid, Vector2i indices)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetArea(Entity<MapGridComponent, AreaGridComponent>.op_Implicit(grid), indices, out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			return false;
		}
		if (IsRoofed(new EntityCoordinates(grid.Owner, Vector2i.op_Implicit(indices)), (Predicate<Entity<RoofingEntityComponent>>)((Entity<RoofingEntityComponent> r) => !r.Comp.CanMortarPlace)))
		{
			return false;
		}
		return area.Value.Comp.WeatherEnabled;
	}

	public bool IsLightBlocked(Entity<MapGridComponent> grid, Vector2i indices)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetArea(Entity<MapGridComponent, AreaGridComponent>.op_Implicit(grid), indices, out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			return false;
		}
		if (IsRoofed(new EntityCoordinates(grid.Owner, Vector2i.op_Implicit(indices)), (Predicate<Entity<RoofingEntityComponent>>)((Entity<RoofingEntityComponent> r) => !r.Comp.CanMortarPlace)))
		{
			return true;
		}
		return !area.Value.Comp.WeatherEnabled;
	}

	public bool CanCAS(EntityCoordinates coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetArea(coordinates, out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			return false;
		}
		if (IsRoofed(coordinates, (Entity<RoofingEntityComponent> r) => !r.Comp.CanCAS))
		{
			return false;
		}
		return area.Value.Comp.CAS;
	}

	public bool CanMortarFire(EntityCoordinates coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetArea(coordinates, out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			return false;
		}
		if (IsRoofed(coordinates, (Entity<RoofingEntityComponent> r) => !r.Comp.CanMortarFire))
		{
			return false;
		}
		return area.Value.Comp.MortarFire;
	}

	public bool CanMortarPlacement(EntityCoordinates coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetArea(coordinates, out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			return false;
		}
		if (IsRoofed(coordinates, (Entity<RoofingEntityComponent> r) => !r.Comp.CanMortarPlace))
		{
			return false;
		}
		return area.Value.Comp.MortarPlacement;
	}

	public bool CanOrbitalBombard(EntityCoordinates coordinates, out bool roofed)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		roofed = false;
		if (!TryGetArea(coordinates, out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			return false;
		}
		if (IsRoofed(coordinates, (Entity<RoofingEntityComponent> r) => !r.Comp.CanOrbitalBombard))
		{
			roofed = true;
			return false;
		}
		return area.Value.Comp.OB;
	}

	public bool CanFulton(EntityCoordinates coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetArea(coordinates, out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			return false;
		}
		if (IsRoofed(coordinates, (Entity<RoofingEntityComponent> r) => !r.Comp.CanFulton))
		{
			return false;
		}
		return area.Value.Comp.Fulton;
	}

	public bool CanLase(EntityCoordinates coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetArea(coordinates, out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			return false;
		}
		if (IsRoofed(coordinates, (Entity<RoofingEntityComponent> r) => !r.Comp.CanLase))
		{
			return false;
		}
		return area.Value.Comp.Lasing;
	}

	public bool CanMedevac(EntityCoordinates coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetArea(coordinates, out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			return false;
		}
		if (IsRoofed(coordinates, (Entity<RoofingEntityComponent> r) => !r.Comp.CanMedevac))
		{
			return false;
		}
		return area.Value.Comp.Medevac;
	}

	public bool CanParadrop(EntityCoordinates coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetArea(coordinates, out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			return false;
		}
		if (IsRoofed(coordinates, (Entity<RoofingEntityComponent> r) => !r.Comp.CanParadrop))
		{
			return false;
		}
		return area.Value.Comp.Paradropping;
	}

	private bool IsRoofed(EntityCoordinates coordinates, Predicate<Entity<RoofingEntityComponent>> predicate)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<RoofingEntityComponent> roofs = ((EntitySystem)this).EntityQueryEnumerator<RoofingEntityComponent>();
		EntityUid uid = default(EntityUid);
		RoofingEntityComponent roof = default(RoofingEntityComponent);
		float distance = default(float);
		while (roofs.MoveNext(ref uid, ref roof))
		{
			if (predicate(Entity<RoofingEntityComponent>.op_Implicit((uid, roof))) && ((EntityCoordinates)(ref coordinates)).TryDistance((IEntityManager)(object)base.EntityManager, uid.ToCoordinates(), ref distance) && distance <= roof.Range)
			{
				return true;
			}
		}
		return false;
	}

	private bool IsRoofed(MapCoordinates mapCoordinates, Predicate<Entity<RoofingEntityComponent>> predicate)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<RoofingEntityComponent> roofs = ((EntitySystem)this).EntityQueryEnumerator<RoofingEntityComponent>();
		EntityUid uid = default(EntityUid);
		RoofingEntityComponent roof = default(RoofingEntityComponent);
		while (roofs.MoveNext(ref uid, ref roof))
		{
			if (predicate(Entity<RoofingEntityComponent>.op_Implicit((uid, roof))) && (mapCoordinates.Position - _transform.ToMapCoordinates(uid.ToCoordinates(), true).Position).Length() <= roof.Range)
			{
				return true;
			}
		}
		return false;
	}

	public bool CanResinPopup(Entity<MapGridComponent, AreaGridComponent?> grid, Vector2i indices, EntityUid? user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetArea(grid, indices, out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			return true;
		}
		if (area.Value.Comp.WeedKilling)
		{
			if (user.HasValue)
			{
				_popup.PopupClient("This area is unsuited to host the hive!", user.Value, user.Value, PopupType.MediumCaution);
			}
			return false;
		}
		if (area.Value.Comp.ResinAllowed)
		{
			return true;
		}
		if (_gameTicker.RoundDuration() > _earlySpreadHiveTime)
		{
			return true;
		}
		if (user.HasValue)
		{
			_popup.PopupClient("It's too early to spread the hive this far.", user.Value, user.Value, PopupType.MediumCaution);
		}
		return false;
	}

	public bool CanSupplyDrop(MapCoordinates mapCoordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetArea(mapCoordinates, out Entity<AreaComponent>? area, out EntityPrototype _))
		{
			return false;
		}
		if (IsRoofed(mapCoordinates, (Entity<RoofingEntityComponent> r) => !r.Comp.CanSupplyDrop))
		{
			return false;
		}
		return area.Value.Comp.SupplyDrop;
	}

	public void TrySetCanOrbitalBombardRoofing(Entity<RoofingEntityComponent?> roofing, bool ob)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RoofingEntityComponent>(Entity<RoofingEntityComponent>.op_Implicit(roofing), ref roofing.Comp, false) && roofing.Comp.CanOrbitalBombard != ob)
		{
			roofing.Comp.CanOrbitalBombard = ob;
			((EntitySystem)this).Dirty<RoofingEntityComponent>(roofing, (MetaDataComponent)null);
		}
	}

	public string GetAreaName(EntityUid coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetArea(coordinates.ToCoordinates(), out Entity<AreaComponent>? _, out EntityPrototype area2))
		{
			return base.Loc.GetString("rmc-tacmap-alert-no-area");
		}
		return area2.Name;
	}

	public override void Update(float frameTime)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			AreaGridComponent areaGrid = default(AreaGridComponent);
			MapGridComponent mapGrid = default(MapGridComponent);
			TileRef? tileRefNullable = default(TileRef?);
			EntityUid? anchored = default(EntityUid?);
			MinimapColorComponent minimapColor = default(MinimapColorComponent);
			AreaComponent areaComp = default(AreaComponent);
			AreaComponent areaComp2 = default(AreaComponent);
			foreach (EntityUid ent in _toRender)
			{
				if (!((EntitySystem)this).TryComp<AreaGridComponent>(ent, ref areaGrid) || !((EntitySystem)this).TryComp<MapGridComponent>(ent, ref mapGrid))
				{
					continue;
				}
				areaGrid.Colors.Clear();
				((EntitySystem)this).Dirty(ent, (IComponent)(object)areaGrid, (MetaDataComponent)null);
				GridTileEnumerator tiles = _map.GetAllTilesEnumerator(ent, mapGrid, true);
				Dictionary<EntProtoId<AreaComponent>, (int, int)> areasOccupied = new Dictionary<EntProtoId<AreaComponent>, (int, int)>();
				(int, int) value;
				while (((GridTileEnumerator)(ref tiles)).MoveNext(ref tileRefNullable))
				{
					TileRef tileRef = tileRefNullable.Value;
					Vector2i pos = tileRef.GridIndices;
					AnchoredEntitiesEnumerator anchoredEnumerator = _map.GetAnchoredEntitiesEnumerator(ent, mapGrid, pos);
					bool found = false;
					bool invincibleWall = false;
					bool xenoConstruct = false;
					while (((AnchoredEntitiesEnumerator)(ref anchoredEnumerator)).MoveNext(ref anchored))
					{
						if (_minimapColorQuery.TryComp(anchored, ref minimapColor))
						{
							areaGrid.Colors[pos] = minimapColor.Color;
							found = true;
						}
						if (_areaLabelQuery.HasComp(anchored))
						{
							areaGrid.Labels[pos] = _rmcWarp.GetName(anchored.Value) ?? ((EntitySystem)this).Name(anchored.Value, (MetaDataComponent)null);
						}
						if (!invincibleWall && _tag.HasTag(anchored.Value, WallTag) && !_damageableQuery.HasComp(anchored.Value))
						{
							invincibleWall = true;
						}
						if (_xenoConstruct.HasComp(anchored))
						{
							xenoConstruct = true;
						}
					}
					areaGrid.Areas.TryGetValue(pos, out EntProtoId<AreaComponent> area);
					(int, int)? areaOccupied = null;
					if (xenoConstruct)
					{
						value = areaOccupied.GetValueOrDefault();
						if (!areaOccupied.HasValue)
						{
							value = Extensions.GetOrNew<EntProtoId<AreaComponent>, (int, int)>(areasOccupied, area);
							areaOccupied = value;
						}
						areaOccupied = (areaOccupied.Value.Item1 + 1, areaOccupied.Value.Item2);
					}
					if (!invincibleWall)
					{
						value = areaOccupied.GetValueOrDefault();
						if (!areaOccupied.HasValue)
						{
							value = Extensions.GetOrNew<EntProtoId<AreaComponent>, (int, int)>(areasOccupied, area);
							areaOccupied = value;
						}
						areaOccupied = (areaOccupied.Value.Item1, areaOccupied.Value.Item2 + 1);
					}
					if (areaOccupied.HasValue)
					{
						areasOccupied[area] = areaOccupied.Value;
					}
					if (!found)
					{
						ContentTileDefinition tile = _turf.GetContentTileDefinition(tileRef);
						if (tile.MinimapColor != default(Color))
						{
							areaGrid.Colors[pos] = tile.MinimapColor;
						}
						else if (areaGrid.Areas.TryGetValue(pos, out area) && area.TryGet(ref areaComp, _prototypes, _compFactory) && areaComp.MinimapColor != default(Color))
						{
							areaGrid.Colors[pos] = ((Color)(ref areaComp.MinimapColor)).WithAlpha(0.5f);
						}
						else
						{
							areaGrid.Colors[pos] = Color.FromHex((ReadOnlySpan<char>)"#6c6767d8", (Color?)null);
						}
					}
				}
				foreach (KeyValuePair<EntProtoId<AreaComponent>, (int, int)> item in areasOccupied)
				{
					item.Deconstruct(out var key, out value);
					(int, int) tuple = value;
					EntProtoId<AreaComponent> areaProto = key;
					var (resin, buildable) = tuple;
					if (areaGrid.AreaEntities.TryGetValue(areaProto, out var area2) && _areaQuery.TryComp(area2, ref areaComp2))
					{
						areaComp2.ResinConstructCount = resin;
						areaComp2.BuildableTiles = buildable;
						((EntitySystem)this).Dirty(area2, (IComponent)(object)areaComp2, (MetaDataComponent)null);
					}
				}
				((EntitySystem)this).Dirty(ent, (IComponent)(object)areaGrid, (MetaDataComponent)null);
			}
		}
		finally
		{
			_toRender.Clear();
		}
	}
}
