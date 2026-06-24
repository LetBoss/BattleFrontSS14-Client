using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Shuttles.Components;
using Content.Shared.Shuttles.UI.MapObjects;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Systems;

namespace Content.Shared.Shuttles.Systems;

public abstract class SharedShuttleSystem : EntitySystem
{
	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private ItemSlotsSystem _itemSlots;

	[Dependency]
	protected FixtureSystem Fixtures;

	[Dependency]
	protected SharedMapSystem Maps;

	[Dependency]
	protected SharedPhysicsSystem Physics;

	[Dependency]
	protected SharedTransformSystem XformSystem;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	public const float FTLRange = 256f;

	public const float FTLBufferRange = 8f;

	public const float TileDensityMultiplier = 0.5f;

	private EntityQuery<MapGridComponent> _gridQuery;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	private EntityQuery<TransformComponent> _xformQuery;

	private List<Entity<MapGridComponent>> _grids = new List<Entity<MapGridComponent>>();

	public override void Initialize()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FixturesComponent, GridFixtureChangeEvent>((ComponentEventHandler<FixturesComponent, GridFixtureChangeEvent>)OnGridFixtureChange, (Type[])null, (Type[])null);
		_gridQuery = ((EntitySystem)this).GetEntityQuery<MapGridComponent>();
		_physicsQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		_xformQuery = ((EntitySystem)this).GetEntityQuery<TransformComponent>();
	}

	private void OnGridFixtureChange(EntityUid uid, FixturesComponent manager, GridFixtureChangeEvent args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<string, Fixture> fixture in args.NewFixtures)
		{
			Physics.SetDensity(uid, fixture.Key, fixture.Value, 0.5f, false, manager);
			Fixtures.SetRestitution(uid, fixture.Key, fixture.Value, 0.1f, false, manager);
		}
	}

	public bool CanFTLTo(EntityUid shuttleUid, MapId targetMap, EntityUid consoleUid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		EntityUid mapUid = Maps.GetMapOrInvalid((MapId?)targetMap);
		if (_xformQuery.GetComponent(shuttleUid).MapID == targetMap)
		{
			return true;
		}
		FTLDestinationComponent destination = default(FTLDestinationComponent);
		if (!((EntitySystem)this).TryComp<FTLDestinationComponent>(mapUid, ref destination) || !destination.Enabled)
		{
			return false;
		}
		if (destination.RequireCoordinateDisk)
		{
			ItemSlotsComponent slot = default(ItemSlotsComponent);
			if (!((EntitySystem)this).TryComp<ItemSlotsComponent>(consoleUid, ref slot))
			{
				return false;
			}
			if (!_itemSlots.TryGetSlot(consoleUid, SharedShuttleConsoleComponent.DiskSlotName, out ItemSlot itemSlot, slot) || !itemSlot.HasItem)
			{
				return false;
			}
			EntityUid? item = itemSlot.Item;
			if (item.HasValue)
			{
				EntityUid disk = item.GetValueOrDefault();
				if (((EntityUid)(ref disk)).Valid)
				{
					ShuttleDestinationCoordinatesComponent diskCoordinates = null;
					if (!((EntitySystem)this).Resolve<ShuttleDestinationCoordinatesComponent>(disk, ref diskCoordinates, true))
					{
						return false;
					}
					EntityUid? diskCoords = diskCoordinates.Destination;
					FTLDestinationComponent diskDestination = default(FTLDestinationComponent);
					if (!diskCoords.HasValue || !((EntitySystem)this).TryComp<FTLDestinationComponent>(diskCoords.Value, ref diskDestination) || diskDestination != destination)
					{
						return false;
					}
					goto IL_00db;
				}
			}
			return false;
		}
		goto IL_00db;
		IL_00db:
		if (((EntitySystem)this).HasComp<FTLMapComponent>(mapUid))
		{
			return false;
		}
		return _whitelistSystem.IsWhitelistPassOrNull(destination.Whitelist, shuttleUid);
	}

	public IEnumerable<(ShuttleExclusionObject Exclusion, MapCoordinates Coordinates)> GetExclusions(MapId mapId, List<ShuttleExclusionObject> exclusions)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		foreach (ShuttleExclusionObject exc in exclusions)
		{
			MapCoordinates beaconCoords = XformSystem.ToMapCoordinates(((EntitySystem)this).GetCoordinates(exc.Coordinates), true);
			if (!(beaconCoords.MapId != mapId))
			{
				yield return (Exclusion: exc, Coordinates: beaconCoords);
			}
		}
	}

	public IEnumerable<(ShuttleBeaconObject Beacon, MapCoordinates Coordinates)> GetBeacons(MapId mapId, List<ShuttleBeaconObject> beacons)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		foreach (ShuttleBeaconObject beacon in beacons)
		{
			MapCoordinates beaconCoords = XformSystem.ToMapCoordinates(((EntitySystem)this).GetCoordinates(beacon.Coordinates), true);
			if (!(beaconCoords.MapId != mapId))
			{
				yield return (Beacon: beacon, Coordinates: beaconCoords);
			}
		}
	}

	public bool CanDraw(EntityUid gridUid, PhysicsComponent? physics = null, IFFComponent? iffComp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<PhysicsComponent>(gridUid, ref physics, true))
		{
			return true;
		}
		if ((int)physics.BodyType != 4 && physics.Mass < 10f)
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve<IFFComponent>(gridUid, ref iffComp, false))
		{
			return true;
		}
		return (iffComp.Flags & IFFFlags.Hide) == 0;
	}

	public bool IsBeaconMap(EntityUid mapUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		FTLDestinationComponent ftlDest = default(FTLDestinationComponent);
		if (((EntitySystem)this).TryComp<FTLDestinationComponent>(mapUid, ref ftlDest))
		{
			return ftlDest.BeaconsOnly;
		}
		return false;
	}

	public bool CanFTLBeacon(NetCoordinates nCoordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityCoordinates coordinates = ((EntitySystem)this).GetCoordinates(nCoordinates);
		return ((EntitySystem)this).HasComp<MapComponent>(coordinates.EntityId);
	}

	public float GetFTLRange(EntityUid shuttleUid)
	{
		return 256f;
	}

	public float GetFTLBufferRange(EntityUid shuttleUid, MapGridComponent? grid = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!_gridQuery.Resolve(shuttleUid, ref grid, true))
		{
			return 0f;
		}
		Box2 localAABB = grid.LocalAABB;
		return ((Box2)(ref localAABB)).MaxDimension / 2f + 8f;
	}

	public bool FTLFree(EntityUid shuttleUid, EntityCoordinates coordinates, Angle angle, List<ShuttleExclusionObject>? exclusionZones)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent shuttlePhysics = default(PhysicsComponent);
		TransformComponent shuttleXform = default(TransformComponent);
		if (!_physicsQuery.TryGetComponent(shuttleUid, ref shuttlePhysics) || !_xformQuery.TryGetComponent(shuttleUid, ref shuttleXform))
		{
			return false;
		}
		_grids.Clear();
		MapCoordinates mapCoordinates = XformSystem.ToMapCoordinates(coordinates, true);
		Vector2 ourPos = Maps.GetGridPosition(Entity<PhysicsComponent, TransformComponent>.op_Implicit((shuttleUid, shuttlePhysics, shuttleXform)));
		Vector2 targetPosition = mapCoordinates.Position;
		if ((targetPosition - ourPos).Length() > 256f)
		{
			return false;
		}
		if (exclusionZones != null)
		{
			foreach (ShuttleExclusionObject exclusion in exclusionZones)
			{
				MapCoordinates exclusionCoords = XformSystem.ToMapCoordinates(((EntitySystem)this).GetCoordinates(exclusion.Coordinates), true);
				if (!(exclusionCoords.MapId != mapCoordinates.MapId) && (mapCoordinates.Position - exclusionCoords.Position).Length() <= exclusion.Range)
				{
					return false;
				}
			}
		}
		PhysShapeCircle circle = new PhysShapeCircle(GetFTLBufferRange(shuttleUid) + 8f, targetPosition);
		_mapManager.FindGridsIntersecting<PhysShapeCircle>(mapCoordinates.MapId, circle, Transform.Empty, ref _grids, false, false);
		foreach (Entity<MapGridComponent> grid in _grids)
		{
			if (!(grid.Owner == shuttleUid))
			{
				return false;
			}
		}
		return true;
	}

	protected virtual void UpdateIFFInterfaces(EntityUid gridUid, IFFComponent component)
	{
	}

	public Color GetIFFColor(EntityUid gridUid, bool self = false, IFFComponent? component = null)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (self)
		{
			return IFFComponent.SelfColor;
		}
		if (!((EntitySystem)this).Resolve<IFFComponent>(gridUid, ref component, false))
		{
			return IFFComponent.IFFColor;
		}
		return component.Color;
	}

	public string? GetIFFLabel(EntityUid gridUid, bool self = false, IFFComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		string entName = ((EntitySystem)this).MetaData(gridUid).EntityName;
		if (self)
		{
			return entName;
		}
		if (((EntitySystem)this).Resolve<IFFComponent>(gridUid, ref component, false) && (component.Flags & (IFFFlags.HideLabel | IFFFlags.Hide)) != IFFFlags.None)
		{
			return null;
		}
		if (!string.IsNullOrEmpty(entName))
		{
			return entName;
		}
		return base.Loc.GetString("shuttle-console-unknown");
	}

	public void SetIFFColor(EntityUid gridUid, Color color, IFFComponent? component = null)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (component == null)
		{
			component = ((EntitySystem)this).EnsureComp<IFFComponent>(gridUid);
		}
		if (!((Color)(ref component.Color)).Equals(color))
		{
			component.Color = color;
			((EntitySystem)this).Dirty(gridUid, (IComponent)(object)component, (MetaDataComponent)null);
			UpdateIFFInterfaces(gridUid, component);
		}
	}

	public void AddIFFFlag(EntityUid gridUid, IFFFlags flags, IFFComponent? component = null)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (component == null)
		{
			component = ((EntitySystem)this).EnsureComp<IFFComponent>(gridUid);
		}
		if ((component.Flags & flags) != flags)
		{
			component.Flags |= flags;
			((EntitySystem)this).Dirty(gridUid, (IComponent)(object)component, (MetaDataComponent)null);
			UpdateIFFInterfaces(gridUid, component);
		}
	}

	public void RemoveIFFFlag(EntityUid gridUid, IFFFlags flags, IFFComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<IFFComponent>(gridUid, ref component, false) && (component.Flags & flags) != IFFFlags.None)
		{
			component.Flags &= (IFFFlags)(byte)(~(int)flags);
			((EntitySystem)this).Dirty(gridUid, (IComponent)(object)component, (MetaDataComponent)null);
			UpdateIFFInterfaces(gridUid, component);
		}
	}
}
