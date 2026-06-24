using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared.Atmos;
using Content.Shared.Coordinates;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Map;

public sealed class RMCMapSystem : EntitySystem
{
	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private IMapManager _mapManager;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private TagSystem _tag;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private TurfSystem _turf;

	private static readonly ProtoId<TagPrototype> StructureTag = ProtoId<TagPrototype>.op_Implicit("Structure");

	private EntityQuery<MapGridComponent> _mapGridQuery;

	public readonly ImmutableArray<AtmosDirection> AtmosCardinalDirections = ImmutableArray.Create(AtmosDirection.South, AtmosDirection.East, AtmosDirection.North, AtmosDirection.West);

	public readonly ImmutableArray<Direction> CardinalDirections = ImmutableArray.Create<Direction>((Direction)4, (Direction)0, (Direction)2, (Direction)6);

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_mapGridQuery = ((EntitySystem)this).GetEntityQuery<MapGridComponent>();
		((EntitySystem)this).SubscribeLocalEvent<RMCDeleteAnchoredOnInitComponent, MapInitEvent>((EntityEventRefHandler<RMCDeleteAnchoredOnInitComponent, MapInitEvent>)OnDestroyAnchoredOnInit, (Type[])null, (Type[])null);
	}

	private void OnDestroyAnchoredOnInit(Entity<RMCDeleteAnchoredOnInitComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Enabled || _net.IsClient)
		{
			return;
		}
		RMCAnchoredEntitiesEnumerator anchored = GetAnchoredEntitiesEnumerator(Entity<RMCDeleteAnchoredOnInitComponent>.op_Implicit(ent), null, (DirectionFlag)0);
		EntityUid uid;
		while (anchored.MoveNext(out uid))
		{
			if (!(uid == ent.Owner) && !((EntitySystem)this).TerminatingOrDeleted(uid, (MetaDataComponent)null) && !base.EntityManager.IsQueuedForDeletion(uid) && !_entityWhitelist.IsWhitelistFailOrNull(ent.Comp.Whitelist, uid))
			{
				((EntitySystem)this).QueueDel((EntityUid?)uid);
			}
		}
	}

	public RMCAnchoredEntitiesEnumerator GetAnchoredEntitiesEnumerator(EntityUid ent, Direction? offset = null, DirectionFlag facing = (DirectionFlag)0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return GetAnchoredEntitiesEnumerator(ent.ToCoordinates(), offset, facing);
	}

	public RMCAnchoredEntitiesEnumerator GetAnchoredEntitiesEnumerator(EntityCoordinates coords, Direction? offset = null, DirectionFlag facing = (DirectionFlag)0)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? grid = _transform.GetGrid(coords);
		if (grid.HasValue)
		{
			EntityUid gridId = grid.GetValueOrDefault();
			MapGridComponent gridComp = default(MapGridComponent);
			if (_mapGridQuery.TryComp(gridId, ref gridComp))
			{
				Vector2i indices = _map.CoordinatesToTile(gridId, gridComp, coords);
				return GetAnchoredEntitiesEnumerator(Entity<MapGridComponent>.op_Implicit((gridId, gridComp)), indices, offset, facing);
			}
		}
		return RMCAnchoredEntitiesEnumerator.Empty;
	}

	public RMCAnchoredEntitiesEnumerator GetAnchoredEntitiesEnumerator(MapCoordinates coords, Direction? offset = null, DirectionFlag facing = (DirectionFlag)0)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		EntityUid gridId = default(EntityUid);
		MapGridComponent gridComp = default(MapGridComponent);
		if (!_mapManager.TryFindGridAt(coords, ref gridId, ref gridComp))
		{
			return RMCAnchoredEntitiesEnumerator.Empty;
		}
		Vector2i indices = _map.CoordinatesToTile(gridId, gridComp, coords);
		return GetAnchoredEntitiesEnumerator(Entity<MapGridComponent>.op_Implicit((gridId, gridComp)), indices, offset, facing);
	}

	public RMCAnchoredEntitiesEnumerator GetAnchoredEntitiesEnumerator(Entity<MapGridComponent> grid, Vector2i indices, Direction? offset = null, DirectionFlag facing = (DirectionFlag)0)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (offset.HasValue)
		{
			indices = DirectionExtensions.Offset(indices, offset.Value);
		}
		AnchoredEntitiesEnumerator anchored = _map.GetAnchoredEntitiesEnumerator(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), indices);
		return new RMCAnchoredEntitiesEnumerator(_transform, anchored, facing);
	}

	public RMCAnchoredEntitiesEnumerator<T> GetAnchoredEntitiesEnumerator<T>(EntityUid ent, Direction? offset = null, DirectionFlag facing = (DirectionFlag)0) where T : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return GetAnchoredEntitiesEnumerator<T>(ent.ToCoordinates(), offset, facing);
	}

	public RMCAnchoredEntitiesEnumerator<T> GetAnchoredEntitiesEnumerator<T>(EntityCoordinates coords, Direction? offset = null, DirectionFlag facing = (DirectionFlag)0) where T : IComponent
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? grid = _transform.GetGrid(coords);
		if (grid.HasValue)
		{
			EntityUid gridId = grid.GetValueOrDefault();
			MapGridComponent gridComp = default(MapGridComponent);
			if (_mapGridQuery.TryComp(gridId, ref gridComp))
			{
				Vector2i indices = _map.CoordinatesToTile(gridId, gridComp, coords);
				return GetAnchoredEntitiesEnumerator<T>(Entity<MapGridComponent>.op_Implicit((gridId, gridComp)), indices, offset, facing);
			}
		}
		return RMCAnchoredEntitiesEnumerator<T>.Empty;
	}

	public RMCAnchoredEntitiesEnumerator<T> GetAnchoredEntitiesEnumerator<T>(MapCoordinates coords, Direction? offset = null, DirectionFlag facing = (DirectionFlag)0) where T : IComponent
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		EntityUid gridId = default(EntityUid);
		MapGridComponent gridComp = default(MapGridComponent);
		if (!_mapManager.TryFindGridAt(coords, ref gridId, ref gridComp))
		{
			return RMCAnchoredEntitiesEnumerator<T>.Empty;
		}
		Vector2i indices = _map.CoordinatesToTile(gridId, gridComp, coords);
		return GetAnchoredEntitiesEnumerator<T>(Entity<MapGridComponent>.op_Implicit((gridId, gridComp)), indices, offset, facing);
	}

	public RMCAnchoredEntitiesEnumerator<T> GetAnchoredEntitiesEnumerator<T>(Entity<MapGridComponent> grid, Vector2i indices, Direction? offset = null, DirectionFlag facing = (DirectionFlag)0) where T : IComponent
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (offset.HasValue)
		{
			indices = DirectionExtensions.Offset(indices, offset.Value);
		}
		AnchoredEntitiesEnumerator anchored = _map.GetAnchoredEntitiesEnumerator(Entity<MapGridComponent>.op_Implicit(grid), Entity<MapGridComponent>.op_Implicit(grid), indices);
		return new RMCAnchoredEntitiesEnumerator<T>((IEntityManager)(object)base.EntityManager, _transform, anchored, facing);
	}

	public bool HasAnchoredEntityEnumerator<T>(EntityCoordinates coords, out Entity<T> ent, Direction? offset = null, DirectionFlag facing = (DirectionFlag)0) where T : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		ent = default(Entity<T>);
		RMCAnchoredEntitiesEnumerator anchored = GetAnchoredEntitiesEnumerator(coords, offset, facing);
		EntityUid uid;
		T comp = default(T);
		while (anchored.MoveNext(out uid))
		{
			if (((EntitySystem)this).TryComp<T>(uid, ref comp))
			{
				ent = Entity<T>.op_Implicit((uid, comp));
				return true;
			}
		}
		return false;
	}

	public bool HasAnchoredEntityEnumerator<T>(EntityCoordinates coords, Direction? offset = null, DirectionFlag facing = (DirectionFlag)0) where T : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		Entity<T> ent;
		return HasAnchoredEntityEnumerator<T>(coords, out ent, offset, facing);
	}

	public bool TryGetTileRefForEnt(EntityCoordinates ent, out Entity<MapGridComponent> grid, out TileRef tile)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		grid = default(Entity<MapGridComponent>);
		tile = default(TileRef);
		EntityUid? grid2 = _transform.GetGrid(ent);
		if (grid2.HasValue)
		{
			EntityUid gridId = grid2.GetValueOrDefault();
			MapGridComponent gridComp = default(MapGridComponent);
			if (_mapGridQuery.TryComp(gridId, ref gridComp))
			{
				EntityCoordinates coords = _transform.GetMoverCoordinates(ent);
				grid = Entity<MapGridComponent>.op_Implicit((gridId, gridComp));
				if (!_map.TryGetTileRef(gridId, gridComp, coords, ref tile))
				{
					return false;
				}
				return true;
			}
		}
		return false;
	}

	public bool IsTileBlocked(EntityCoordinates coordinates, CollisionGroup group = CollisionGroup.Impassable)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!_turf.TryGetTileRef(coordinates, out var turf))
		{
			return false;
		}
		return _turf.IsTileBlocked(turf.Value, group);
	}

	public bool IsTileBlocked(MapCoordinates coordinates, CollisionGroup group = CollisionGroup.Impassable)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return IsTileBlocked(_transform.ToCoordinates(coordinates), group);
	}

	public bool TileHasAnyTag(EntityCoordinates coordinates, params ProtoId<TagPrototype>[] tag)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		RMCAnchoredEntitiesEnumerator anchored = GetAnchoredEntitiesEnumerator(coordinates, null, (DirectionFlag)0);
		EntityUid uid;
		while (anchored.MoveNext(out uid))
		{
			if (_tag.HasAnyTag(uid, tag))
			{
				return true;
			}
		}
		return false;
	}

	public bool TileHasStructure(EntityCoordinates coordinates)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return TileHasAnyTag(coordinates, StructureTag);
	}

	public bool TryGetTileDef(EntityCoordinates coordinates, [NotNullWhen(true)] out ContentTileDefinition? def)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		def = null;
		EntityUid? grid = _transform.GetGrid(coordinates);
		if (grid.HasValue)
		{
			EntityUid gridId = grid.GetValueOrDefault();
			MapGridComponent grid2 = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
			{
				Vector2i indices = _map.TileIndicesFor(gridId, grid2, coordinates);
				ITileDefinition defUncast = default(ITileDefinition);
				if (!_map.TryGetTileDef(grid2, indices, ref defUncast))
				{
					return false;
				}
				def = (ContentTileDefinition)(object)defUncast;
				return true;
			}
		}
		return false;
	}

	public bool TryGetTileDef(MapCoordinates coordinates, [NotNullWhen(true)] out ContentTileDefinition? def)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return TryGetTileDef(_transform.ToCoordinates(coordinates), out def);
	}

	public bool CanBuildOn(EntityCoordinates coordinates, CollisionGroup group = CollisionGroup.Impassable)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (!IsTileBlocked(coordinates, group))
		{
			return !TileHasStructure(coordinates);
		}
		return false;
	}

	public EntityCoordinates SnapToGrid(EntityCoordinates coordinates)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? gridId = _transform.GetGrid(coordinates);
		MapGridComponent grid = default(MapGridComponent);
		if (!gridId.HasValue || !((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid))
		{
			MapCoordinates mapPos = _transform.ToMapCoordinates(coordinates, true);
			float mapX = (float)(int)Math.Floor(((MapCoordinates)(ref mapPos)).X) + 0.5f;
			float mapY = (float)(int)Math.Floor(((MapCoordinates)(ref mapPos)).Y) + 0.5f;
			((MapCoordinates)(ref mapPos))._002Ector(new Vector2(mapX, mapY), mapPos.MapId);
			return _transform.ToCoordinates(Entity<TransformComponent>.op_Implicit(coordinates.EntityId), mapPos);
		}
		ushort tileSize = grid.TileSize;
		Vector2 position = _transform.WithEntityId(coordinates, gridId.Value).Position;
		float x = (float)(int)Math.Floor(position.X / (float)(int)tileSize) + (float)(int)tileSize / 2f;
		float y = (float)(int)Math.Floor(position.Y / (float)(int)tileSize) + (float)(int)tileSize / 2f;
		EntityCoordinates gridPos = default(EntityCoordinates);
		((EntityCoordinates)(ref gridPos))._002Ector(gridId.Value, new Vector2(x, y));
		return _transform.WithEntityId(gridPos, coordinates.EntityId);
	}
}
