using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Collections;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Map.Events;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Robust.Shared.GameObjects;

public abstract class SharedMapSystem : EntitySystem
{
	public struct TilesEnumerator
	{
		private readonly SharedMapSystem _mapSystem;

		private readonly EntityUid _uid;

		private readonly MapGridComponent _grid;

		private readonly bool _ignoreEmpty;

		private readonly Predicate<TileRef>? _predicate;

		private readonly int _lowerY;

		private readonly int _upperX;

		private readonly int _upperY;

		private int _x;

		private int _y;

		public TilesEnumerator(SharedMapSystem mapSystem, bool ignoreEmpty, Predicate<TileRef>? predicate, EntityUid uid, MapGridComponent grid, Box2 aabb)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			_mapSystem = mapSystem;
			_uid = uid;
			_grid = grid;
			_ignoreEmpty = ignoreEmpty;
			_predicate = predicate;
			Unsafe.SkipInit(out Vector2i val);
			((Vector2i)(ref val))._002Ector((int)Math.Floor(aabb.Left), (int)Math.Floor(aabb.Bottom));
			Unsafe.SkipInit(out Vector2i val2);
			((Vector2i)(ref val2))._002Ector((int)Math.Ceiling(aabb.Right), (int)Math.Ceiling(aabb.Top));
			_x = val.X;
			_y = val.Y;
			_lowerY = val.Y;
			_upperX = val2.X;
			_upperY = val2.Y;
		}

		public bool MoveNext(out TileRef tile)
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			Unsafe.SkipInit(out Vector2i val);
			while (true)
			{
				if (_x >= _upperX)
				{
					tile = TileRef.Zero;
					return false;
				}
				((Vector2i)(ref val))._002Ector(_x, _y);
				_y++;
				if (_y >= _upperY)
				{
					_x++;
					_y = _lowerY;
				}
				Vector2i key = _mapSystem.GridTileToChunkIndices(_uid, _grid, val);
				if (_grid.Chunks.TryGetValue(key, out MapChunk value))
				{
					Vector2i val2 = value.GridTileToChunkTile(val);
					tile = _mapSystem.GetTileRef(_uid, _grid, value, (ushort)val2.X, (ushort)val2.Y);
					if ((!_ignoreEmpty || !tile.Tile.IsEmpty) && (_predicate == null || _predicate(tile)))
					{
						return true;
					}
				}
				else if (!_ignoreEmpty)
				{
					tile = new TileRef(_uid, val.X, val.Y, Tile.Empty);
					if (_predicate == null || _predicate(tile))
					{
						break;
					}
				}
			}
			return true;
		}
	}

	[Serializable]
	[NetSerializable]
	private sealed class MapLightComponentState : ComponentState
	{
		public Color AmbientLightColor;
	}

	[Robust.Shared.IoC.Dependency]
	private readonly ITileDefinitionManager _tileMan;

	[Robust.Shared.IoC.Dependency]
	private readonly IGameTiming _timing;

	[Robust.Shared.IoC.Dependency]
	protected readonly IMapManager MapManager;

	[Robust.Shared.IoC.Dependency]
	private readonly IMapManagerInternal _mapInternal;

	[Robust.Shared.IoC.Dependency]
	private readonly INetManager _netManager;

	[Robust.Shared.IoC.Dependency]
	private readonly FixtureSystem _fixtures;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedPhysicsSystem _physics;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedTransformSystem _transform;

	[Robust.Shared.IoC.Dependency]
	private readonly MetaDataSystem _meta;

	private EntityQuery<FixturesComponent> _fixturesQuery;

	private EntityQuery<MapComponent> _mapQuery;

	private EntityQuery<MapGridComponent> _gridQuery;

	private EntityQuery<MetaDataComponent> _metaQuery;

	private EntityQuery<TransformComponent> _xformQuery;

	protected HashSet<MapId> UsedIds = new HashSet<MapId>();

	protected int LastMapId;

	private Dictionary<EntityUid, MapId> _reserved = new Dictionary<EntityUid, MapId>();

	internal Dictionary<MapId, EntityUid> Maps { get; } = new Dictionary<MapId, EntityUid>();

	public EntityCoordinates AlignToGrid(EntityCoordinates coordinates)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (_gridQuery.TryGetComponent(coordinates.EntityId, out MapGridComponent component))
		{
			Vector2i tile = CoordinatesToTile(coordinates.EntityId, component, coordinates);
			return ToCenterCoordinates(coordinates.EntityId, tile, component);
		}
		MapCoordinates mapCoordinates = _transform.ToMapCoordinates(coordinates);
		if (_mapInternal.TryFindGridAt(mapCoordinates, out EntityUid uid, out component))
		{
			Vector2i tile2 = CoordinatesToTile(uid, component, coordinates);
			return ToCenterCoordinates(uid, tile2, component);
		}
		return coordinates;
	}

	public EntityCoordinates ToCoordinates(TileRef tileRef, MapGridComponent? gridComponent = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return ToCoordinates(tileRef.GridUid, tileRef.GridIndices, gridComponent);
	}

	public EntityCoordinates ToCoordinates(EntityUid gridUid, Vector2i tile, MapGridComponent? gridComponent = null)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!_gridQuery.Resolve(gridUid, ref gridComponent))
		{
			return EntityCoordinates.Invalid;
		}
		return new EntityCoordinates(gridUid, Vector2i.op_Implicit(tile * (int)gridComponent.TileSize));
	}

	public EntityCoordinates ToCenterCoordinates(TileRef tileRef, MapGridComponent? gridComponent = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return ToCenterCoordinates(tileRef.GridUid, tileRef.GridIndices, gridComponent);
	}

	public EntityCoordinates ToCenterCoordinates(EntityUid gridUid, Vector2i tile, MapGridComponent? gridComponent = null)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!_gridQuery.Resolve(gridUid, ref gridComponent))
		{
			return EntityCoordinates.Invalid;
		}
		return new EntityCoordinates(gridUid, Vector2i.op_Implicit(tile * (int)gridComponent.TileSize) + gridComponent.TileSizeHalfVector);
	}

	public override void Initialize()
	{
		base.Initialize();
		_fixturesQuery = GetEntityQuery<FixturesComponent>();
		_mapQuery = GetEntityQuery<MapComponent>();
		_gridQuery = GetEntityQuery<MapGridComponent>();
		_metaQuery = GetEntityQuery<MetaDataComponent>();
		_xformQuery = GetEntityQuery<TransformComponent>();
		InitializeMap();
		InitializeGrid();
		SubscribeLocalEvent<MapLightComponent, ComponentGetState>(OnMapLightGetState);
		SubscribeLocalEvent<MapLightComponent, ComponentHandleState>(OnMapLightHandleState);
	}

	public static ulong ToBitmask(Vector2i index, byte chunkSize = 8)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return (ulong)(1L << index.X + index.Y * chunkSize);
	}

	public static bool FromBitmask(Vector2i index, ulong bitmask, byte chunkSize = 8)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		ulong num = ToBitmask(index, chunkSize);
		return (num & bitmask) == num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i GetChunkIndices(Vector2 tile, int chunkSize)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2i((int)Math.Floor(tile.X / (float)chunkSize), (int)Math.Floor(tile.Y / (float)chunkSize));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i GetChunkIndices(Vector2 tile, byte chunkSize)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2i((int)Math.Floor(tile.X / (float)(int)chunkSize), (int)Math.Floor(tile.Y / (float)(int)chunkSize));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i GetChunkIndices(Vector2i tile, int chunkSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2i((int)Math.Floor((float)tile.X / (float)chunkSize), (int)Math.Floor((float)tile.Y / (float)chunkSize));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i GetChunkIndices(Vector2i tile, byte chunkSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2i((int)Math.Floor((float)tile.X / (float)(int)chunkSize), (int)Math.Floor((float)tile.Y / (float)(int)chunkSize));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i GetChunkRelative(Vector2 tile, int chunkSize)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		int num = MathHelper.Mod((int)Math.Floor(tile.X), chunkSize);
		int num2 = MathHelper.Mod((int)Math.Floor(tile.Y), chunkSize);
		return new Vector2i(num, num2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i GetChunkRelative(Vector2 tile, byte chunkSize)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		int num = MathHelper.Mod((int)Math.Floor(tile.X), (int)chunkSize);
		int num2 = MathHelper.Mod((int)Math.Floor(tile.Y), (int)chunkSize);
		return new Vector2i(num, num2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i GetChunkRelative(Vector2i tile, int chunkSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		int num = MathHelper.Mod(tile.X, chunkSize);
		int num2 = MathHelper.Mod(tile.Y, chunkSize);
		return new Vector2i(num, num2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2i GetChunkRelative(Vector2i tile, byte chunkSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		int num = MathHelper.Mod(tile.X, (int)chunkSize);
		int num2 = MathHelper.Mod(tile.Y, (int)chunkSize);
		return new Vector2i(num, num2);
	}

	public static Vector2i GetDirection(Vector2i position, Direction dir, int dist = 1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected I4, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		return (Vector2i)((int)dir switch
		{
			2 => position + new Vector2i(dist, 0), 
			1 => position + new Vector2i(dist, -dist), 
			0 => position + new Vector2i(0, -dist), 
			7 => position + new Vector2i(-dist, -dist), 
			6 => position + new Vector2i(-dist, 0), 
			5 => position + new Vector2i(-dist, dist), 
			4 => position + new Vector2i(0, dist), 
			3 => position + new Vector2i(dist, dist), 
			_ => throw new NotImplementedException(), 
		});
	}

	private void InitializeGrid()
	{
		SubscribeLocalEvent<MapGridComponent, ComponentGetState>(OnGridGetState);
		SubscribeLocalEvent<MapGridComponent, ComponentHandleState>(OnGridHandleState);
		SubscribeLocalEvent<MapGridComponent, ComponentAdd>(OnGridAdd);
		SubscribeLocalEvent<MapGridComponent, ComponentInit>(OnGridInit);
		SubscribeLocalEvent<MapGridComponent, ComponentStartup>(OnGridStartup);
		SubscribeLocalEvent<MapGridComponent, ComponentShutdown>(OnGridRemove);
		SubscribeLocalEvent<MapGridComponent, MoveEvent>(OnGridMove);
	}

	public Vector2 GetGridPosition(Entity<PhysicsComponent?> grid, Vector2 worldPos, Angle worldRot)
	{
		if (!Resolve(grid.Owner, ref grid.Comp))
		{
			return Vector2.Zero;
		}
		Vector2 localCenter = grid.Comp.LocalCenter;
		return worldPos + ((Angle)(ref worldRot)).RotateVec(ref localCenter);
	}

	public Vector2 GetGridPosition(Entity<PhysicsComponent?, TransformComponent?> grid)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!Resolve(grid.Owner, ref grid.Comp1, ref grid.Comp2))
		{
			return Vector2.Zero;
		}
		var (worldPos, worldRot) = _transform.GetWorldPositionRotation(grid.Comp2);
		return GetGridPosition((Owner: grid.Owner, Comp: grid.Comp1), worldPos, worldRot);
	}

	private void OnGridBoundsChange(EntityUid uid, MapGridComponent component)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!(component.MapProxy == DynamicTree.Proxy.Free))
		{
			TransformComponent transformComponent = Comp<TransformComponent>(uid);
			Box2 aabb = GetWorldAABB(uid, component, transformComponent);
			if (TryComp(transformComponent.MapUid, out GridTreeComponent comp))
			{
				comp.Tree.MoveProxy(component.MapProxy, in aabb);
			}
			_physics.MovedGrids.Add(uid);
		}
	}

	private void OnGridMove(EntityUid uid, MapGridComponent component, ref MoveEvent args)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (args.ParentChanged)
		{
			OnParentChange(uid, component, ref args);
		}
		else if (!(component.MapProxy == DynamicTree.Proxy.Free))
		{
			TransformComponent component2 = args.Component;
			Box2 aabb = GetWorldAABB(uid, component, component2);
			if (TryComp(component2.MapUid, out GridTreeComponent comp))
			{
				comp.Tree.MoveProxy(component.MapProxy, in aabb);
			}
			_physics.MovedGrids.Add(uid);
		}
	}

	private void OnParentChange(EntityUid uid, MapGridComponent component, ref MoveEvent args)
	{
		UpdatePvsChunks(args.Entity);
		Entity<TransformComponent, MetaDataComponent> entity = args.Entity;
		var (_, transformComponent2, metaDataComponent2) = (Entity<TransformComponent, MetaDataComponent>)(ref entity);
		if ((int)metaDataComponent2.EntityLifeStage < 2 || args.Component.LifeStage == ComponentLifeStage.Starting)
		{
			return;
		}
		base.Log.Info($"Grid {ToPrettyString(uid, metaDataComponent2)} changed parent. Old parent: {ToPrettyString(args.OldPosition.EntityId)}. New parent: {ToPrettyString(transformComponent2.ParentUid)}");
		if (!transformComponent2.MapUid.HasValue && (int)metaDataComponent2.EntityLifeStage < 4 && _netManager.IsServer)
		{
			base.Log.Error($"Grid {ToPrettyString(uid, metaDataComponent2)} was moved to nullspace! AAAAAAAAAAAAAAAAAAAAAAAAA! {Environment.StackTrace}");
		}
		EntityUid parentUid = transformComponent2.ParentUid;
		EntityUid? mapUid = transformComponent2.MapUid;
		if (parentUid != mapUid && (int)metaDataComponent2.EntityLifeStage < 4 && _netManager.IsServer)
		{
			base.Log.Error($"Grid {ToPrettyString(uid, metaDataComponent2)} is parented to {ToPrettyString(transformComponent2._parent)} which is not a map.  y'all need jesus. {Environment.StackTrace}");
			return;
		}
		EntityUid mapOrInvalid = GetMapOrInvalid(_transform.ToMapCoordinates(args.OldPosition).MapId);
		if (component.MapProxy != DynamicTree.Proxy.Free)
		{
			_physics.MovedGrids.Remove(uid);
			RemoveGrid(uid, component, mapOrInvalid);
		}
		if (transformComponent2.MapUid.HasValue)
		{
			_physics.MovedGrids.Add(uid);
			AddGrid(uid, component);
		}
	}

	protected virtual void UpdatePvsChunks(Entity<TransformComponent, MetaDataComponent> grid)
	{
	}

	private void OnGridHandleState(EntityUid uid, MapGridComponent component, ref ComponentHandleState args)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		IComponentState current = args.Current;
		Vector2i key;
		ChunkDatum value;
		if (!(current is MapGridComponentDeltaState mapGridComponentDeltaState))
		{
			if (!(current is MapGridComponentState mapGridComponentState))
			{
				return;
			}
			component.LastTileModifiedTick = mapGridComponentState.LastTileModifiedTick;
			component.ChunkSize = mapGridComponentState.ChunkSize;
			foreach (Vector2i key2 in component.Chunks.Keys)
			{
				if (!mapGridComponentState.FullGridData.ContainsKey(key2))
				{
					ApplyChunkData(uid, component, key2, ChunkDatum.Empty);
				}
			}
			foreach (KeyValuePair<Vector2i, ChunkDatum> fullGridDatum in mapGridComponentState.FullGridData)
			{
				fullGridDatum.Deconstruct(out key, out value);
				Vector2i index = key;
				ChunkDatum data = value;
				ApplyChunkData(uid, component, index, data);
			}
		}
		else
		{
			component.ChunkSize = mapGridComponentDeltaState.ChunkSize;
			if (mapGridComponentDeltaState.ChunkData == null)
			{
				return;
			}
			foreach (KeyValuePair<Vector2i, ChunkDatum> chunkDatum in mapGridComponentDeltaState.ChunkData)
			{
				chunkDatum.Deconstruct(out key, out value);
				Vector2i index2 = key;
				ChunkDatum data2 = value;
				ApplyChunkData(uid, component, index2, data2);
			}
			component.LastTileModifiedTick = mapGridComponentDeltaState.LastTileModifiedTick;
		}
		RegenerateAabb(component);
		OnGridBoundsChange(uid, component);
	}

	private void ApplyChunkData(EntityUid uid, MapGridComponent component, Vector2i index, ChunkDatum data)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		Entity<MapGridComponent> entity = new Entity<MapGridComponent>(uid, component);
		bool shapeChanged;
		if (data.IsDeleted())
		{
			if (!component.Chunks.Remove(index, out MapChunk value))
			{
				return;
			}
			value.SuppressCollisionRegeneration = true;
			for (ushort num2 = 0; num2 < component.ChunkSize; num2++)
			{
				for (ushort num3 = 0; num3 < component.ChunkSize; num3++)
				{
					if (value.TrySetTile(num2, num3, Tile.Empty, out var oldTile, out shapeChanged))
					{
						Vector2i gridIndices = value.ChunkTileToGridTile(Vector2i.op_Implicit((ValueTuple<int, int>)(num2, num3)));
						TileRef tileRef = new TileRef(uid, gridIndices, Tile.Empty);
						_mapInternal.RaiseOnTileChanged(entity, tileRef, oldTile, index);
					}
				}
			}
			value.CachedBounds = Box2i.Empty;
			value.SuppressCollisionRegeneration = false;
			return;
		}
		MapChunk orAddChunk = GetOrAddChunk(uid, component, index);
		orAddChunk.Fixtures.Clear();
		orAddChunk.Fixtures.UnionWith(data.Fixtures);
		orAddChunk.SuppressCollisionRegeneration = true;
		ValueList<TileChangedEntry> valueList = default(ValueList<TileChangedEntry>);
		Unsafe.SkipInit(out Vector2i chunkTile);
		for (ushort num4 = 0; num4 < component.ChunkSize; num4++)
		{
			for (ushort num5 = 0; num5 < component.ChunkSize; num5++)
			{
				Tile tile = data.TileData[num++];
				if (orAddChunk.TrySetTile(num4, num5, tile, out var oldTile2, out shapeChanged))
				{
					((Vector2i)(ref chunkTile))._002Ector((int)num4, (int)num5);
					Vector2i gridIndices2 = orAddChunk.ChunkTileToGridTile(chunkTile);
					valueList.Add(new TileChangedEntry(tile, oldTile2, orAddChunk.Indices, gridIndices2));
				}
			}
		}
		TileChangedEvent args = new TileChangedEvent(entity, valueList.ToArray());
		EntityManager.EventBus.RaiseLocalEvent(entity.Owner, ref args, broadcast: true);
		orAddChunk.CachedBounds = data.CachedBounds.Value;
		orAddChunk.SuppressCollisionRegeneration = false;
	}

	private void OnGridGetState(EntityUid uid, MapGridComponent component, ref ComponentGetState args)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		if (args.FromTick <= component.CreationTick)
		{
			GetFullState(uid, component, ref args);
			return;
		}
		GameTick fromTick = args.FromTick;
		Dictionary<Vector2i, ChunkDatum> dictionary;
		if (component.LastTileModifiedTick < fromTick)
		{
			dictionary = null;
		}
		else
		{
			dictionary = new Dictionary<Vector2i, ChunkDatum>();
			foreach (var (gameTick, key) in component.ChunkDeletionHistory)
			{
				if (!(gameTick < fromTick) || !(fromTick != GameTick.Zero))
				{
					if (!component.Chunks.TryGetValue(key, out MapChunk value))
					{
						dictionary.Add(key, ChunkDatum.Empty);
					}
					else if (value.LastTileModifiedTick < fromTick)
					{
						base.Log.Error($"Encountered un-deleted chunk with an old last-modified tick on grid {ToPrettyString(uid)}");
					}
				}
			}
			foreach (var (key2, mapChunk2) in GetMapChunks(uid, component))
			{
				if (mapChunk2.LastTileModifiedTick < fromTick)
				{
					continue;
				}
				Tile[] array = new Tile[component.ChunkSize * component.ChunkSize];
				for (int i = 0; i < component.ChunkSize; i++)
				{
					for (int j = 0; j < component.ChunkSize; j++)
					{
						array[i * component.ChunkSize + j] = mapChunk2.GetTile((ushort)i, (ushort)j);
					}
				}
				HashSet<string> hashSet = mapChunk2.Fixtures;
				if (_netManager.IsClient)
				{
					hashSet = new HashSet<string>(hashSet);
				}
				dictionary.Add(key2, ChunkDatum.CreateModified(array, hashSet, mapChunk2.CachedBounds));
			}
		}
		args.State = new MapGridComponentDeltaState(component.ChunkSize, dictionary, component.LastTileModifiedTick);
	}

	private void GetFullState(EntityUid uid, MapGridComponent component, ref ComponentGetState args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<Vector2i, ChunkDatum> dictionary = new Dictionary<Vector2i, ChunkDatum>();
		foreach (KeyValuePair<Vector2i, MapChunk> mapChunk2 in GetMapChunks(uid, component))
		{
			mapChunk2.Deconstruct(out var key, out var value);
			Vector2i key2 = key;
			MapChunk mapChunk = value;
			Tile[] array = new Tile[component.ChunkSize * component.ChunkSize];
			for (int i = 0; i < component.ChunkSize; i++)
			{
				for (int j = 0; j < component.ChunkSize; j++)
				{
					array[i * component.ChunkSize + j] = mapChunk.GetTile((ushort)i, (ushort)j);
				}
			}
			HashSet<string> hashSet = mapChunk.Fixtures;
			if (_netManager.IsClient)
			{
				hashSet = new HashSet<string>(hashSet);
			}
			dictionary.Add(key2, ChunkDatum.CreateModified(array, hashSet, mapChunk.CachedBounds));
		}
		args.State = new MapGridComponentState(component.ChunkSize, dictionary, component.LastTileModifiedTick);
	}

	private void OnGridAdd(EntityUid uid, MapGridComponent component, ComponentAdd args)
	{
		GridAddEvent args2 = new GridAddEvent(uid);
		RaiseLocalEvent(uid, args2, broadcast: true);
	}

	private void OnGridInit(EntityUid uid, MapGridComponent component, ComponentInit args)
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent component2 = _xformQuery.GetComponent(uid);
		GameTick curTick = _timing.CurTick;
		foreach (MapChunk value in component.Chunks.Values)
		{
			value.LastTileModifiedTick = curTick;
		}
		component.LastTileModifiedTick = curTick;
		if (component2.MapUid.HasValue && component2.MapUid != uid)
		{
			_transform.SetParent(uid, component2, component2.MapUid.Value);
		}
		if (!_mapQuery.HasComponent(uid))
		{
			Box2 aabb = GetWorldAABB(uid, component);
			if (TryComp(component2.MapUid, out GridTreeComponent comp))
			{
				DynamicTree.Proxy mapProxy = comp.Tree.CreateProxy(in aabb, uint.MaxValue, (uid, _fixturesQuery.Comp(uid), component));
				component.MapProxy = mapProxy;
			}
			if (component2.MapUid.HasValue)
			{
				_physics.MovedGrids.Add(uid);
			}
		}
		GridInitializeEvent args2 = new GridInitializeEvent(uid);
		RaiseLocalEvent(uid, args2, broadcast: true);
	}

	private void OnGridStartup(EntityUid uid, MapGridComponent component, ComponentStartup args)
	{
		GridStartupEvent args2 = new GridStartupEvent(uid);
		RaiseLocalEvent(uid, args2, broadcast: true);
	}

	private void OnGridRemove(EntityUid uid, MapGridComponent component, ComponentShutdown args)
	{
		base.Log.Info($"Removing grid {ToPrettyString(uid)}");
		if (TryComp(uid, out TransformComponent comp) && comp.MapUid.HasValue)
		{
			RemoveGrid(uid, component, comp.MapUid.Value);
		}
		component.MapProxy = DynamicTree.Proxy.Free;
		RaiseLocalEvent(uid, new GridRemovalEvent(uid), broadcast: true);
	}

	private Box2 GetWorldAABB(EntityUid uid, MapGridComponent grid, TransformComponent? xform = null)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Box2 result;
		if (!Resolve(uid, ref xform))
		{
			result = default(Box2);
			return result;
		}
		(Vector2 WorldPosition, Angle WorldRotation) worldPositionRotation = _transform.GetWorldPositionRotation(xform);
		Vector2 item = worldPositionRotation.WorldPosition;
		Angle item2 = worldPositionRotation.WorldRotation;
		result = grid.LocalAABB;
		Box2Rotated val = new Box2Rotated(((Box2)(ref result)).Translated(item), item2, item);
		return ((Box2Rotated)(ref val)).CalcBoundingBox();
	}

	private void AddGrid(EntityUid uid, MapGridComponent grid)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Box2 aabb = GetWorldAABB(uid, grid);
		if (_xformQuery.TryGetComponent(uid, out TransformComponent component))
		{
			if (TryComp(component.MapUid, out GridTreeComponent comp))
			{
				DynamicTree.Proxy mapProxy = comp.Tree.CreateProxy(in aabb, uint.MaxValue, (uid, _fixturesQuery.Comp(uid), grid));
				grid.MapProxy = mapProxy;
			}
			if (component.MapUid.HasValue)
			{
				_physics.MovedGrids.Add(uid);
			}
		}
	}

	private void RemoveGrid(EntityUid uid, MapGridComponent grid, EntityUid mapUid)
	{
		if (grid.MapProxy != DynamicTree.Proxy.Free && TryComp(mapUid, out GridTreeComponent comp))
		{
			comp.Tree.DestroyProxy(grid.MapProxy);
		}
		grid.MapProxy = DynamicTree.Proxy.Free;
		if (mapUid.IsValid())
		{
			_physics.MovedGrids.Remove(uid);
		}
	}

	private void RemoveChunk(EntityUid uid, MapGridComponent grid, Vector2i origin)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (grid.Chunks.TryGetValue(origin, out MapChunk value))
		{
			if (_netManager.IsServer)
			{
				grid.ChunkDeletionHistory.Add((_timing.CurTick, value.Indices));
			}
			value.Fixtures.Clear();
			grid.Chunks.Remove(origin);
			if (grid.Chunks.Count == 0)
			{
				RaiseLocalEvent(uid, new EmptyGridEvent
				{
					GridId = uid
				}, broadcast: true);
			}
		}
	}

	private void RegenerateCollision(EntityUid uid, MapGridComponent grid, MapChunk mapChunk)
	{
		RegenerateCollision(uid, grid, new HashSet<MapChunk> { mapChunk });
	}

	internal void RegenerateCollision(EntityUid uid, MapGridComponent grid, IReadOnlySet<MapChunk> chunks)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if (HasComp<MapComponent>(uid))
		{
			ClearEmptyMapChunks(uid, grid, chunks);
			return;
		}
		Dictionary<MapChunk, List<Box2i>> dictionary = new Dictionary<MapChunk, List<Box2i>>(chunks.Count);
		List<MapChunk> list = new List<MapChunk>();
		foreach (MapChunk chunk in chunks)
		{
			GridChunkPartition.PartitionChunk(chunk, out Box2i bounds, out List<Box2i> rectangles);
			chunk.CachedBounds = bounds;
			if (chunk.FilledTiles > 0)
			{
				dictionary.Add(chunk, rectangles);
				continue;
			}
			FixturesComponent fixturesComponent = null;
			PhysicsComponent body = null;
			TransformComponent xform = null;
			foreach (string fixture in chunk.Fixtures)
			{
				chunk.Fixtures.Remove(fixture);
				FixtureSystem fixtures = _fixtures;
				FixturesComponent manager = fixturesComponent;
				fixtures.DestroyFixture(uid, fixture, updates: false, body, manager, xform);
			}
			RemoveChunk(uid, grid, chunk.Indices);
			list.Add(chunk);
		}
		RegenerateAabb(grid);
		if (!Deleted(uid))
		{
			_physics.WakeBody(uid);
			OnGridBoundsChange(uid, grid);
			RegenerateGridBoundsEvent message = new RegenerateGridBoundsEvent(uid, dictionary, list);
			RaiseLocalEvent(ref message);
		}
	}

	private void RegenerateAabb(MapGridComponent grid)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		Box2 val = (grid.LocalAABB = default(Box2));
		foreach (MapChunk value in grid.Chunks.Values)
		{
			Box2i cachedBounds = value.CachedBounds;
			Vector2i size = ((Box2i)(ref cachedBounds)).Size;
			if (!((Vector2i)(ref size)).Equals(Vector2i.Zero))
			{
				val = grid.LocalAABB;
				if (((Box2)(ref val)).Size == Vector2.Zero)
				{
					Box2i val3 = ((Box2i)(ref cachedBounds)).Translated(value.Indices * (int)value.ChunkSize);
					grid.LocalAABB = Box2i.op_Implicit(val3);
					continue;
				}
				Box2i val4 = ((Box2i)(ref cachedBounds)).Translated(value.Indices * (int)value.ChunkSize);
				val = grid.LocalAABB;
				Box2 val5 = Box2i.op_Implicit(val4);
				grid.LocalAABB = ((Box2)(ref val)).Union(ref val5);
			}
		}
	}

	private void ClearEmptyMapChunks(EntityUid uid, MapGridComponent grid, IReadOnlySet<MapChunk> modified)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		foreach (MapChunk item in modified)
		{
			if (item.FilledTiles <= 0)
			{
				RemoveChunk(uid, grid, item.Indices);
			}
		}
	}

	public TileRef GetTileRef(Entity<MapGridComponent> grid, MapCoordinates coords)
	{
		return GetTileRef(grid.Owner, grid.Comp, coords);
	}

	public TileRef GetTileRef(EntityUid uid, MapGridComponent grid, MapCoordinates coords)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetTileRef(uid, grid, CoordinatesToTile(uid, grid, coords));
	}

	public TileRef GetTileRef(Entity<MapGridComponent> grid, EntityCoordinates coords)
	{
		return GetTileRef(grid.Owner, grid.Comp, coords);
	}

	public TileRef GetTileRef(EntityUid uid, MapGridComponent grid, EntityCoordinates coords)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetTileRef(uid, grid, CoordinatesToTile(uid, grid, coords));
	}

	public TileRef GetTileRef(Entity<MapGridComponent> grid, Vector2i tileCoordinates)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return GetTileRef(grid.Owner, grid.Comp, tileCoordinates);
	}

	public TileRef GetTileRef(EntityUid uid, MapGridComponent grid, Vector2i tileCoordinates)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Vector2i key = GridTileToChunkIndices(uid, grid, tileCoordinates);
		if (!grid.Chunks.TryGetValue(key, out MapChunk value))
		{
			return new TileRef(uid, tileCoordinates.X, tileCoordinates.Y, default(Tile));
		}
		Vector2i val = value.GridTileToChunkTile(tileCoordinates);
		return GetTileRef(uid, grid, value, (ushort)val.X, (ushort)val.Y);
	}

	internal TileRef GetTileRef(EntityUid uid, MapGridComponent grid, MapChunk mapChunk, ushort xIndex, ushort yIndex)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (xIndex >= mapChunk.ChunkSize)
		{
			throw new ArgumentOutOfRangeException("xIndex", "Tile indices out of bounds.");
		}
		if (yIndex >= mapChunk.ChunkSize)
		{
			throw new ArgumentOutOfRangeException("yIndex", "Tile indices out of bounds.");
		}
		Vector2i gridIndices = mapChunk.ChunkTileToGridTile(new Vector2i((int)xIndex, (int)yIndex));
		return new TileRef(uid, gridIndices, mapChunk.GetTile(xIndex, yIndex));
	}

	public IEnumerable<TileRef> GetAllTiles(EntityUid uid, MapGridComponent grid, bool ignoreEmpty = true)
	{
		Unsafe.SkipInit(out int num);
		Unsafe.SkipInit(out int num2);
		foreach (MapChunk chunk in grid.Chunks.Values)
		{
			for (ushort x = 0; x < grid.ChunkSize; x++)
			{
				for (ushort y = 0; y < grid.ChunkSize; y++)
				{
					Tile tile = chunk.GetTile(x, y);
					if (!ignoreEmpty || !tile.IsEmpty)
					{
						Vector2i val = new Vector2i((int)x, (int)y) + chunk.Indices * (int)grid.ChunkSize;
						((Vector2i)(ref val)).Deconstruct(ref num, ref num2);
						int xIndex = num;
						int yIndex = num2;
						yield return new TileRef(uid, xIndex, yIndex, tile);
					}
				}
			}
		}
	}

	public GridTileEnumerator GetAllTilesEnumerator(EntityUid uid, MapGridComponent grid, bool ignoreEmpty = true)
	{
		return new GridTileEnumerator(uid, grid.Chunks.GetEnumerator(), grid.ChunkSize, ignoreEmpty);
	}

	public void SetTile(Entity<MapGridComponent> grid, EntityCoordinates coordinates, Tile tile)
	{
		SetTile(grid.Owner, grid.Comp, coordinates, tile);
	}

	public void SetTile(Entity<MapGridComponent> grid, Vector2i gridIndices, Tile tile)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SetTile(grid.Owner, grid.Comp, gridIndices, tile);
	}

	public void SetTiles(Entity<MapGridComponent> grid, List<(Vector2i GridIndices, Tile Tile)> tiles)
	{
		SetTiles(grid.Owner, grid.Comp, tiles);
	}

	public void SetTile(EntityUid uid, MapGridComponent grid, EntityCoordinates coords, Tile tile)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Vector2i val = CoordinatesToTile(uid, grid, coords);
		SetTile(uid, grid, new Vector2i(val.X, val.Y), tile);
	}

	public void SetTile(EntityUid uid, MapGridComponent grid, Vector2i gridIndices, Tile tile)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Vector2i val = GridTileToChunkIndices(uid, grid, gridIndices);
		if (!grid.Chunks.TryGetValue(val, out MapChunk value))
		{
			if (tile.IsEmpty)
			{
				return;
			}
			value = (grid.Chunks[val] = new MapChunk(val.X, val.Y, grid.ChunkSize)
			{
				LastTileModifiedTick = _timing.CurTick
			});
		}
		Vector2i val2 = value.GridTileToChunkTile(gridIndices);
		SetChunkTile(uid, grid, value, (ushort)val2.X, (ushort)val2.Y, tile, out var _);
	}

	public void SetTiles(EntityUid uid, MapGridComponent grid, List<(Vector2i GridIndices, Tile Tile)> tiles)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		if (tiles.Count == 0)
		{
			return;
		}
		HashSet<MapChunk> hashSet = new HashSet<MapChunk>(Math.Max(1, tiles.Count / grid.ChunkSize));
		ValueList<TileChangedEntry> valueList = new ValueList<TileChangedEntry>(tiles.Count);
		MapManager.SuppressOnTileChanged = true;
		foreach (var tile in tiles)
		{
			Vector2i item = tile.GridIndices;
			Tile item2 = tile.Tile;
			Vector2i val = GridTileToChunkIndices(uid, grid, item);
			if (!grid.Chunks.TryGetValue(val, out MapChunk value))
			{
				if (item2.IsEmpty)
				{
					continue;
				}
				value = (grid.Chunks[val] = new MapChunk(val.X, val.Y, grid.ChunkSize)
				{
					LastTileModifiedTick = _timing.CurTick
				});
			}
			Vector2i val2 = value.GridTileToChunkTile(item);
			value.SuppressCollisionRegeneration = true;
			if (SetChunkTile(uid, grid, value, (ushort)val2.X, (ushort)val2.Y, item2, out var oldTile))
			{
				hashSet.Add(value);
				valueList.Add(new TileChangedEntry(item2, oldTile, val2, item));
			}
		}
		foreach (MapChunk item3 in hashSet)
		{
			item3.SuppressCollisionRegeneration = false;
		}
		TileChangedEvent args = new TileChangedEvent((Owner: uid, Comp: grid), valueList.ToArray());
		RaiseLocalEvent(uid, ref args, broadcast: true);
		RegenerateCollision(uid, grid, hashSet);
		MapManager.SuppressOnTileChanged = false;
	}

	public TilesEnumerator GetLocalTilesEnumerator(EntityUid uid, MapGridComponent grid, Box2 aabb, bool ignoreEmpty = true, Predicate<TileRef>? predicate = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return new TilesEnumerator(this, ignoreEmpty, predicate, uid, grid, aabb);
	}

	public TilesEnumerator GetTilesEnumerator(EntityUid uid, MapGridComponent grid, Box2 aabb, bool ignoreEmpty = true, Predicate<TileRef>? predicate = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Box2 aabb2 = Matrix3Helpers.TransformBox(_transform.GetInvWorldMatrix(uid), ref aabb);
		return new TilesEnumerator(this, ignoreEmpty, predicate, uid, grid, aabb2);
	}

	public TilesEnumerator GetTilesEnumerator(EntityUid uid, MapGridComponent grid, Box2Rotated bounds, bool ignoreEmpty = true, Predicate<TileRef>? predicate = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Box2 aabb = Matrix3Helpers.TransformBox(_transform.GetInvWorldMatrix(uid), ref bounds);
		return new TilesEnumerator(this, ignoreEmpty, predicate, uid, grid, aabb);
	}

	public IEnumerable<TileRef> GetLocalTilesIntersecting(EntityUid uid, MapGridComponent grid, Box2 localAABB, bool ignoreEmpty = true, Predicate<TileRef>? predicate = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		TilesEnumerator enumerator = new TilesEnumerator(this, ignoreEmpty, predicate, uid, grid, localAABB);
		TileRef tile;
		while (enumerator.MoveNext(out tile))
		{
			yield return tile;
		}
	}

	public IEnumerable<TileRef> GetLocalTilesIntersecting(EntityUid uid, MapGridComponent grid, Box2Rotated localArea, bool ignoreEmpty = true, Predicate<TileRef>? predicate = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Box2 aabb = ((Box2Rotated)(ref localArea)).CalcBoundingBox();
		TilesEnumerator enumerator = new TilesEnumerator(this, ignoreEmpty, predicate, uid, grid, aabb);
		TileRef tile;
		while (enumerator.MoveNext(out tile))
		{
			yield return tile;
		}
	}

	public IEnumerable<TileRef> GetTilesIntersecting(EntityUid uid, MapGridComponent grid, Box2Rotated worldArea, bool ignoreEmpty = true, Predicate<TileRef>? predicate = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Box2 aabb = Matrix3Helpers.TransformBox(_transform.GetInvWorldMatrix(uid), ref worldArea);
		TilesEnumerator enumerator = new TilesEnumerator(this, ignoreEmpty, predicate, uid, grid, aabb);
		TileRef tile;
		while (enumerator.MoveNext(out tile))
		{
			yield return tile;
		}
	}

	public IEnumerable<TileRef> GetTilesIntersecting(EntityUid uid, MapGridComponent grid, Box2 worldArea, bool ignoreEmpty = true, Predicate<TileRef>? predicate = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Box2 aabb = Matrix3Helpers.TransformBox(_transform.GetInvWorldMatrix(uid), ref worldArea);
		TilesEnumerator enumerator = new TilesEnumerator(this, ignoreEmpty, predicate, uid, grid, aabb);
		TileRef tile;
		while (enumerator.MoveNext(out tile))
		{
			yield return tile;
		}
	}

	public IEnumerable<TileRef> GetLocalTilesIntersecting(EntityUid uid, MapGridComponent grid, Circle localCircle, bool ignoreEmpty = true, Predicate<TileRef>? predicate = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Unsafe.SkipInit(out Box2 aabb);
		((Box2)(ref aabb))._002Ector(localCircle.Position.X - localCircle.Radius, localCircle.Position.Y - localCircle.Radius, localCircle.Position.X + localCircle.Radius, localCircle.Position.Y + localCircle.Radius);
		TilesEnumerator tileEnumerator = GetLocalTilesEnumerator(uid, grid, aabb, ignoreEmpty, predicate);
		TileRef tile;
		while (tileEnumerator.MoveNext(out tile))
		{
			if (Vector2Helpers.IsShorterThanOrEqualTo(Vector2i.op_Implicit(tile.GridIndices) + grid.TileSizeHalfVector - localCircle.Position, localCircle.Radius))
			{
				yield return tile;
			}
		}
	}

	public IEnumerable<TileRef> GetTilesIntersecting(EntityUid uid, MapGridComponent grid, Circle worldArea, bool ignoreEmpty = true, Predicate<TileRef>? predicate = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Unsafe.SkipInit(out Box2 worldArea2);
		((Box2)(ref worldArea2))._002Ector(worldArea.Position.X - worldArea.Radius, worldArea.Position.Y - worldArea.Radius, worldArea.Position.X + worldArea.Radius, worldArea.Position.Y + worldArea.Radius);
		EntityCoordinates circleGridPos = new EntityCoordinates(uid, WorldToLocal(uid, grid, worldArea.Position));
		foreach (TileRef item in GetTilesIntersecting(uid, grid, worldArea2, ignoreEmpty, predicate))
		{
			if (GridTileToLocal(uid, grid, item.GridIndices).TryDistance(EntityManager, _transform, circleGridPos, out var distance) && distance <= worldArea.Radius)
			{
				yield return item;
			}
		}
	}

	private bool TryGetTile(EntityUid uid, MapGridComponent grid, Vector2i indices, bool ignoreEmpty, [NotNullWhen(true)] out TileRef? tileRef, Predicate<TileRef>? predicate = null)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		Vector2i key = GridTileToChunkIndices(uid, grid, indices);
		if (grid.Chunks.TryGetValue(key, out MapChunk value))
		{
			Vector2i val = value.GridTileToChunkTile(indices);
			TileRef tileRef2 = GetTileRef(uid, grid, value, (ushort)val.X, (ushort)val.Y);
			if (ignoreEmpty && tileRef2.Tile.IsEmpty)
			{
				tileRef = null;
				return false;
			}
			if (predicate == null || predicate(tileRef2))
			{
				tileRef = tileRef2;
				return true;
			}
		}
		else if (!ignoreEmpty)
		{
			TileRef tileRef3 = new TileRef(uid, indices.X, indices.Y, Tile.Empty);
			if (predicate == null || predicate(tileRef3))
			{
				tileRef = tileRef3;
				return true;
			}
		}
		tileRef = null;
		return false;
	}

	internal MapChunk GetOrAddChunk(EntityUid uid, MapGridComponent grid, int xIndex, int yIndex)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return GetOrAddChunk(uid, grid, new Vector2i(xIndex, yIndex));
	}

	internal bool TryGetChunk(EntityUid uid, MapGridComponent grid, Vector2i chunkIndices, [NotNullWhen(true)] out MapChunk? chunk)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return grid.Chunks.TryGetValue(chunkIndices, out chunk);
	}

	internal MapChunk GetOrAddChunk(EntityUid uid, MapGridComponent grid, Vector2i chunkIndices)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (grid.Chunks.TryGetValue(chunkIndices, out MapChunk value))
		{
			return value;
		}
		MapChunk mapChunk = new MapChunk(chunkIndices.X, chunkIndices.Y, grid.ChunkSize)
		{
			LastTileModifiedTick = _timing.CurTick
		};
		return grid.Chunks[chunkIndices] = mapChunk;
	}

	public bool HasChunk(EntityUid uid, MapGridComponent grid, Vector2i chunkIndices)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return grid.Chunks.ContainsKey(chunkIndices);
	}

	internal IReadOnlyDictionary<Vector2i, MapChunk> GetMapChunks(EntityUid uid, MapGridComponent grid)
	{
		return grid.Chunks;
	}

	internal ChunkEnumerator GetMapChunks(EntityUid uid, MapGridComponent grid, Box2 worldAABB)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Box2 localAABB = Matrix3Helpers.TransformBox(_transform.GetInvWorldMatrix(uid), ref worldAABB);
		return GetLocalMapChunks(uid, grid, localAABB);
	}

	internal ChunkEnumerator GetMapChunks(EntityUid uid, MapGridComponent grid, Box2Rotated worldArea)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Box2 localAABB = Matrix3Helpers.TransformBox(_transform.GetInvWorldMatrix(uid), ref worldArea);
		return GetLocalMapChunks(uid, grid, localAABB);
	}

	internal ChunkEnumerator GetLocalMapChunks(EntityUid uid, MapGridComponent grid, Box2 localAABB)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Box2 localAABB2;
		if (_mapQuery.HasComponent(uid))
		{
			localAABB2 = localAABB;
		}
		else
		{
			Box2 localAABB3 = grid.LocalAABB;
			localAABB2 = ((Box2)(ref localAABB3)).Intersect(ref localAABB);
		}
		return new ChunkEnumerator(grid.Chunks, localAABB2, grid.ChunkSize);
	}

	public int AnchoredEntityCount(EntityUid uid, MapGridComponent grid, Vector2i pos)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Vector2i key = GridTileToChunkIndices(uid, grid, pos);
		if (!grid.Chunks.TryGetValue(key, out MapChunk value))
		{
			return 0;
		}
		Vector2i val = value.GridTileToChunkTile(pos);
		Unsafe.SkipInit(out int num);
		Unsafe.SkipInit(out int num2);
		((Vector2i)(ref val)).Deconstruct(ref num, ref num2);
		int num3 = num;
		int num4 = num2;
		return value.GetSnapGrid((ushort)num3, (ushort)num4)?.Count ?? 0;
	}

	public IEnumerable<EntityUid> GetAnchoredEntities(Entity<MapGridComponent> grid, MapCoordinates coords)
	{
		return GetAnchoredEntities(grid.Owner, grid.Comp, coords);
	}

	public IEnumerable<EntityUid> GetAnchoredEntities(EntityUid uid, MapGridComponent grid, MapCoordinates coords)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetAnchoredEntities(uid, grid, TileIndicesFor(uid, grid, coords));
	}

	public IEnumerable<EntityUid> GetAnchoredEntities(Entity<MapGridComponent> grid, EntityCoordinates coords)
	{
		return GetAnchoredEntities(grid.Owner, grid.Comp, coords);
	}

	public IEnumerable<EntityUid> GetAnchoredEntities(EntityUid uid, MapGridComponent grid, EntityCoordinates coords)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetAnchoredEntities(uid, grid, TileIndicesFor(uid, grid, coords));
	}

	public IEnumerable<EntityUid> GetAnchoredEntities(Entity<MapGridComponent> grid, Vector2i pos)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return GetAnchoredEntities(grid.Owner, grid.Comp, pos);
	}

	public IEnumerable<EntityUid> GetAnchoredEntities(EntityUid uid, MapGridComponent grid, Vector2i pos)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Vector2i key = GridTileToChunkIndices(uid, grid, pos);
		if (!grid.Chunks.TryGetValue(key, out MapChunk value))
		{
			return Enumerable.Empty<EntityUid>();
		}
		Vector2i val = value.GridTileToChunkTile(pos);
		return value.GetSnapGridCell((ushort)val.X, (ushort)val.Y);
	}

	public void GetAnchoredEntities(Entity<MapGridComponent> grid, Vector2i pos, List<EntityUid> list)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Vector2i key = GridTileToChunkIndices(grid.Owner, grid.Comp, pos);
		if (grid.Comp.Chunks.TryGetValue(key, out MapChunk value))
		{
			Vector2i val = value.GridTileToChunkTile(pos);
			List<EntityUid> snapGrid = value.GetSnapGrid((ushort)val.X, (ushort)val.Y);
			if (snapGrid != null)
			{
				list.AddRange(snapGrid);
			}
		}
	}

	public AnchoredEntitiesEnumerator GetAnchoredEntitiesEnumerator(EntityUid uid, MapGridComponent grid, Vector2i pos)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Vector2i key = GridTileToChunkIndices(uid, grid, pos);
		if (!grid.Chunks.TryGetValue(key, out MapChunk value))
		{
			return AnchoredEntitiesEnumerator.Empty;
		}
		Vector2i val = value.GridTileToChunkTile(pos);
		List<EntityUid> snapGrid = value.GetSnapGrid((ushort)val.X, (ushort)val.Y);
		if (snapGrid != null)
		{
			return new AnchoredEntitiesEnumerator(snapGrid.GetEnumerator());
		}
		return AnchoredEntitiesEnumerator.Empty;
	}

	public IEnumerable<EntityUid> GetLocalAnchoredEntities(EntityUid uid, MapGridComponent grid, Box2 localAABB)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		TilesEnumerator enumerator = new TilesEnumerator(this, ignoreEmpty: true, null, uid, grid, localAABB);
		TileRef tile;
		while (enumerator.MoveNext(out tile))
		{
			AnchoredEntitiesEnumerator anchoredEnumerator = GetAnchoredEntitiesEnumerator(uid, grid, tile.GridIndices);
			EntityUid? uid2;
			while (anchoredEnumerator.MoveNext(out uid2))
			{
				yield return uid2.Value;
			}
		}
	}

	public IEnumerable<EntityUid> GetAnchoredEntities(EntityUid uid, MapGridComponent grid, Box2 worldAABB)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Box2 aabb = Matrix3Helpers.TransformBox(_transform.GetInvWorldMatrix(uid), ref worldAABB);
		TilesEnumerator enumerator = new TilesEnumerator(this, ignoreEmpty: true, null, uid, grid, aabb);
		TileRef tile;
		while (enumerator.MoveNext(out tile))
		{
			AnchoredEntitiesEnumerator anchoredEnumerator = GetAnchoredEntitiesEnumerator(uid, grid, tile.GridIndices);
			EntityUid? uid2;
			while (anchoredEnumerator.MoveNext(out uid2))
			{
				yield return uid2.Value;
			}
		}
	}

	public IEnumerable<EntityUid> GetAnchoredEntities(EntityUid uid, MapGridComponent grid, Box2Rotated worldBounds)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		foreach (TileRef item in GetTilesIntersecting(uid, grid, worldBounds))
		{
			foreach (EntityUid anchoredEntity in GetAnchoredEntities(uid, grid, item.GridIndices))
			{
				yield return anchoredEntity;
			}
		}
	}

	public Vector2i TileIndicesFor(EntityUid uid, MapGridComponent grid, EntityCoordinates coords)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return SnapGridLocalCellFor(uid, grid, LocalToGrid(uid, grid, coords));
	}

	public Vector2i TileIndicesFor(Entity<MapGridComponent> grid, EntityCoordinates coords)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return TileIndicesFor(grid.Owner, grid.Comp, coords);
	}

	public Vector2i TileIndicesFor(EntityUid uid, MapGridComponent grid, MapCoordinates worldPos)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Vector2 localPos = WorldToLocal(uid, grid, worldPos.Position);
		return SnapGridLocalCellFor(uid, grid, localPos);
	}

	public Vector2i TileIndicesFor(Entity<MapGridComponent> grid, MapCoordinates coords)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return TileIndicesFor(grid.Owner, grid.Comp, coords);
	}

	private Vector2i SnapGridLocalCellFor(EntityUid uid, MapGridComponent grid, Vector2 localPos)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)Math.Floor(localPos.X / (float)(int)grid.TileSize);
		int num2 = (int)Math.Floor(localPos.Y / (float)(int)grid.TileSize);
		return new Vector2i(num, num2);
	}

	public bool IsAnchored(EntityUid uid, MapGridComponent grid, EntityCoordinates coords, EntityUid euid)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Vector2i pos = TileIndicesFor(uid, grid, coords);
		if (!TryChunkAndOffsetForTile(uid, grid, pos, out MapChunk chunk, out Vector2i offset))
		{
			return false;
		}
		return chunk.GetSnapGrid((ushort)offset.X, (ushort)offset.Y)?.Contains(euid) ?? false;
	}

	public bool AddToSnapGridCell(EntityUid gridUid, MapGridComponent grid, Vector2i pos, EntityUid euid)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!TryChunkAndOffsetForTile(gridUid, grid, pos, out MapChunk chunk, out Vector2i offset))
		{
			return false;
		}
		if (chunk.GetTile((ushort)offset.X, (ushort)offset.Y).IsEmpty)
		{
			return false;
		}
		chunk.AddToSnapGridCell((ushort)offset.X, (ushort)offset.Y, euid);
		return true;
	}

	public bool AddToSnapGridCell(EntityUid gridUid, MapGridComponent grid, EntityCoordinates coords, EntityUid euid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return AddToSnapGridCell(gridUid, grid, TileIndicesFor(gridUid, grid, coords), euid);
	}

	public void RemoveFromSnapGridCell(EntityUid gridUid, MapGridComponent grid, Vector2i pos, EntityUid euid)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Vector2i key = GridTileToChunkIndices(gridUid, grid, pos);
		if (grid.Chunks.TryGetValue(key, out MapChunk value))
		{
			Vector2i val = value.GridTileToChunkTile(pos);
			value.RemoveFromSnapGridCell((ushort)val.X, (ushort)val.Y, euid);
		}
	}

	public void RemoveFromSnapGridCell(EntityUid gridUid, MapGridComponent grid, EntityCoordinates coords, EntityUid euid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		RemoveFromSnapGridCell(gridUid, grid, TileIndicesFor(gridUid, grid, coords), euid);
	}

	private bool TryChunkAndOffsetForTile(EntityUid uid, MapGridComponent grid, Vector2i pos, [NotNullWhen(true)] out MapChunk? chunk, out Vector2i offset)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Vector2i key = GridTileToChunkIndices(uid, grid, pos);
		if (!grid.Chunks.TryGetValue(key, out chunk))
		{
			offset = default(Vector2i);
			return false;
		}
		offset = chunk.GridTileToChunkTile(pos);
		return true;
	}

	public IEnumerable<EntityUid> GetInDir(EntityUid uid, MapGridComponent grid, EntityCoordinates position, Direction dir)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Vector2i direction = GetDirection(TileIndicesFor(uid, grid, position), dir);
		return GetAnchoredEntities(uid, grid, direction);
	}

	public IEnumerable<EntityUid> GetOffset(EntityUid uid, MapGridComponent grid, EntityCoordinates coords, Vector2i offset)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		Vector2i pos = TileIndicesFor(uid, grid, coords) + offset;
		return GetAnchoredEntities(uid, grid, pos);
	}

	public IEnumerable<EntityUid> GetLocal(EntityUid uid, MapGridComponent grid, EntityCoordinates coords)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetAnchoredEntities(uid, grid, TileIndicesFor(uid, grid, coords));
	}

	public EntityCoordinates DirectionToGrid(EntityUid uid, MapGridComponent grid, EntityCoordinates coords, Direction direction)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return GridTileToLocal(uid, grid, GetDirection(TileIndicesFor(uid, grid, coords), direction));
	}

	public IEnumerable<EntityUid> GetCardinalNeighborCells(EntityUid uid, MapGridComponent grid, EntityCoordinates coords)
	{
		Vector2i position = TileIndicesFor(uid, grid, coords);
		foreach (EntityUid anchoredEntity in GetAnchoredEntities(uid, grid, position))
		{
			yield return anchoredEntity;
		}
		foreach (EntityUid anchoredEntity2 in GetAnchoredEntities(uid, grid, position + new Vector2i(0, 1)))
		{
			yield return anchoredEntity2;
		}
		foreach (EntityUid anchoredEntity3 in GetAnchoredEntities(uid, grid, position + new Vector2i(0, -1)))
		{
			yield return anchoredEntity3;
		}
		foreach (EntityUid anchoredEntity4 in GetAnchoredEntities(uid, grid, position + new Vector2i(1, 0)))
		{
			yield return anchoredEntity4;
		}
		foreach (EntityUid anchoredEntity5 in GetAnchoredEntities(uid, grid, position + new Vector2i(-1, 0)))
		{
			yield return anchoredEntity5;
		}
	}

	public IEnumerable<EntityUid> GetCellsInSquareArea(EntityUid uid, MapGridComponent grid, EntityCoordinates coords, int n)
	{
		Vector2i position = TileIndicesFor(uid, grid, coords);
		int y = -n;
		while (y <= n)
		{
			int num;
			for (int x = -n; x <= n; x = num)
			{
				AnchoredEntitiesEnumerator enumerator = GetAnchoredEntitiesEnumerator(uid, grid, position + new Vector2i(x, y));
				EntityUid? uid2;
				while (enumerator.MoveNext(out uid2))
				{
					yield return uid2.Value;
				}
				num = x + 1;
			}
			num = y + 1;
			y = num;
		}
	}

	public Vector2 WorldToLocal(EntityUid uid, MapGridComponent grid, Vector2 posWorld)
	{
		Matrix3x2 invWorldMatrix = _transform.GetInvWorldMatrix(uid);
		return Vector2.Transform(posWorld, invWorldMatrix);
	}

	public EntityCoordinates MapToGrid(EntityUid uid, MapCoordinates posWorld)
	{
		MapId mapID = _xformQuery.GetComponent(uid).MapID;
		if (posWorld.MapId != mapID)
		{
			throw new ArgumentException($"Grid {uid} is on map {mapID}, but coords are on map {posWorld.MapId}.", "posWorld");
		}
		if (!_gridQuery.TryGetComponent(uid, out MapGridComponent component))
		{
			return new EntityCoordinates(GetMapOrInvalid(posWorld.MapId), new Vector2(posWorld.X, posWorld.Y));
		}
		return new EntityCoordinates(uid, WorldToLocal(uid, component, posWorld.Position));
	}

	public Vector2 LocalToWorld(EntityUid uid, MapGridComponent grid, Vector2 posLocal)
	{
		Matrix3x2 worldMatrix = _transform.GetWorldMatrix(uid);
		return Vector2.Transform(posLocal, worldMatrix);
	}

	public Vector2i WorldToTile(EntityUid uid, MapGridComponent grid, Vector2 posWorld)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = WorldToLocal(uid, grid, posWorld);
		int num = (int)Math.Floor(vector.X / (float)(int)grid.TileSize);
		int num2 = (int)Math.Floor(vector.Y / (float)(int)grid.TileSize);
		return new Vector2i(num, num2);
	}

	public Vector2i LocalToTile(EntityUid uid, MapGridComponent grid, EntityCoordinates coordinates)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = LocalToGrid(uid, grid, coordinates);
		return new Vector2i((int)Math.Floor(vector.X / (float)(int)grid.TileSize), (int)Math.Floor(vector.Y / (float)(int)grid.TileSize));
	}

	public Vector2i CoordinatesToTile(EntityUid uid, MapGridComponent grid, MapCoordinates coords)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = WorldToLocal(uid, grid, coords.Position);
		int num = (int)Math.Floor(vector.X / (float)(int)grid.TileSize);
		int num2 = (int)Math.Floor(vector.Y / (float)(int)grid.TileSize);
		return new Vector2i(num, num2);
	}

	public Vector2i CoordinatesToTile(EntityUid uid, MapGridComponent grid, EntityCoordinates coords)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = LocalToGrid(uid, grid, coords);
		int num = (int)Math.Floor(vector.X / (float)(int)grid.TileSize);
		int num2 = (int)Math.Floor(vector.Y / (float)(int)grid.TileSize);
		return new Vector2i(num, num2);
	}

	public Vector2i LocalToChunkIndices(EntityUid uid, MapGridComponent grid, EntityCoordinates gridPos)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = LocalToGrid(uid, grid, gridPos);
		int num = (int)Math.Floor(vector.X / (float)(grid.TileSize * grid.ChunkSize));
		int num2 = (int)Math.Floor(vector.Y / (float)(grid.TileSize * grid.ChunkSize));
		return new Vector2i(num, num2);
	}

	public Vector2 LocalToGrid(EntityUid uid, MapGridComponent grid, EntityCoordinates position)
	{
		if (!(position.EntityId == uid))
		{
			return WorldToLocal(uid, grid, _transform.ToMapCoordinates(position).Position);
		}
		return position.Position;
	}

	public bool CollidesWithGrid(EntityUid uid, MapGridComponent grid, Vector2i indices)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Vector2i key = GridTileToChunkIndices(uid, grid, indices);
		if (!grid.Chunks.TryGetValue(key, out MapChunk value))
		{
			return false;
		}
		Vector2i val = value.GridTileToChunkTile(indices);
		return value.GetTile((ushort)val.X, (ushort)val.Y).TypeId != Tile.Empty.TypeId;
	}

	public Vector2i GridTileToChunkIndices(EntityUid uid, MapGridComponent grid, Vector2i gridTile)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return GridTileToChunkIndices(grid, gridTile);
	}

	public Vector2i GridTileToChunkIndices(MapGridComponent grid, Vector2i gridTile)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)Math.Floor((float)gridTile.X / (float)(int)grid.ChunkSize);
		int num2 = (int)Math.Floor((float)gridTile.Y / (float)(int)grid.ChunkSize);
		return new Vector2i(num, num2);
	}

	public EntityCoordinates GridTileToLocal(EntityUid uid, MapGridComponent grid, Vector2i gridTile)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Vector2 position = TileCenterToVector(uid, grid, gridTile);
		return new EntityCoordinates(uid, position);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2 TileToVector(Entity<MapGridComponent> grid, Vector2i gridTile)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(gridTile.X * grid.Comp.TileSize, gridTile.Y * grid.Comp.TileSize);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2 TileCenterToVector(EntityUid uid, MapGridComponent grid, Vector2i gridTile)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return TileCenterToVector((Owner: uid, Comp: grid), gridTile);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector2 TileCenterToVector(Entity<MapGridComponent> grid, Vector2i gridTile)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(gridTile.X * grid.Comp.TileSize, gridTile.Y * grid.Comp.TileSize) + grid.Comp.TileSizeHalfVector;
	}

	public Vector2 GridTileToWorldPos(EntityUid uid, MapGridComponent grid, Vector2i gridTile)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		float x = (float)(gridTile.X * grid.TileSize) + (float)(int)grid.TileSize / 2f;
		float y = (float)(gridTile.Y * grid.TileSize) + (float)(int)grid.TileSize / 2f;
		return Vector2.Transform(new Vector2(x, y), _transform.GetWorldMatrix(uid));
	}

	public MapCoordinates GridTileToWorld(EntityUid uid, MapGridComponent grid, Vector2i gridTile)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		MapId mapID = _xformQuery.GetComponent(uid).MapID;
		return new MapCoordinates(GridTileToWorldPos(uid, grid, gridTile), mapID);
	}

	public bool TryGetTileRef(EntityUid uid, MapGridComponent grid, Vector2i indices, out TileRef tile)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Vector2i key = GridTileToChunkIndices(uid, grid, indices);
		if (!grid.Chunks.TryGetValue(key, out MapChunk value))
		{
			tile = default(TileRef);
			return false;
		}
		Vector2i val = value.GridTileToChunkTile(indices);
		tile = GetTileRef(uid, grid, value, (ushort)val.X, (ushort)val.Y);
		return true;
	}

	public bool TryGetTile(MapGridComponent grid, Vector2i indices, out Tile tile)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Vector2i key = GridTileToChunkIndices(grid, indices);
		if (!grid.Chunks.TryGetValue(key, out MapChunk value))
		{
			tile = default(Tile);
			return false;
		}
		Vector2i val = value.GridTileToChunkTile(indices);
		tile = value.Tiles[val.X, val.Y];
		return true;
	}

	public bool TryGetTileDef(MapGridComponent grid, Vector2i indices, [NotNullWhen(true)] out ITileDefinition? tileDef)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetTile(grid, indices, out var tile))
		{
			tileDef = null;
			return false;
		}
		tileDef = _tileMan[tile.TypeId];
		return true;
	}

	public bool TryGetTileRef(EntityUid uid, MapGridComponent grid, EntityCoordinates coords, out TileRef tile)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return TryGetTileRef(uid, grid, CoordinatesToTile(uid, grid, coords), out tile);
	}

	public bool TryGetTileRef(EntityUid uid, MapGridComponent grid, Vector2 worldPos, out TileRef tile)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return TryGetTileRef(uid, grid, WorldToTile(uid, grid, worldPos), out tile);
	}

	internal Box2 CalcWorldAABB(EntityUid uid, MapGridComponent grid, MapChunk mapChunk)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		(Vector2 WorldPosition, Angle WorldRotation) worldPositionRotation = _transform.GetWorldPositionRotation(uid);
		Vector2 item = worldPositionRotation.WorldPosition;
		Angle item2 = worldPositionRotation.WorldRotation;
		Vector2i indices = mapChunk.Indices;
		ushort tileSize = grid.TileSize;
		ushort chunkSize = mapChunk.ChunkSize;
		Vector2 vector = Vector2i.op_Implicit(indices * (int)tileSize * (int)chunkSize);
		Vector2 vector2 = item + ((Angle)(ref item2)).RotateVec(ref vector);
		Box2i cachedBounds = mapChunk.CachedBounds;
		Box2 val = Box2i.op_Implicit(((Box2i)(ref cachedBounds)).Scale((int)tileSize));
		Box2Rotated val2 = new Box2Rotated(((Box2)(ref val)).Translated(vector2), item2, vector2);
		return ((Box2Rotated)(ref val2)).CalcBoundingBox();
	}

	private void OnTileModified(EntityUid uid, MapGridComponent grid, MapChunk mapChunk, Vector2i tileIndices, Tile newTile, Tile oldTile, bool shapeChanged)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Vector2i gridIndices = mapChunk.ChunkTileToGridTile(tileIndices);
		mapChunk.LastTileModifiedTick = _timing.CurTick;
		grid.LastTileModifiedTick = _timing.CurTick;
		Dirty(uid, grid);
		if (!MapManager.SuppressOnTileChanged)
		{
			TileRef tileRef = new TileRef(uid, gridIndices, newTile);
			_mapInternal.RaiseOnTileChanged((Owner: uid, Comp: grid), tileRef, oldTile, mapChunk.Indices);
		}
		if (shapeChanged && !mapChunk.SuppressCollisionRegeneration)
		{
			RegenerateCollision(uid, grid, mapChunk);
		}
	}

	internal bool SetChunkTile(EntityUid uid, MapGridComponent grid, MapChunk chunk, ushort xIndex, ushort yIndex, Tile tile, out Tile oldTile)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!chunk.TrySetTile(xIndex, yIndex, tile, out oldTile, out var shapeChanged))
		{
			return false;
		}
		Unsafe.SkipInit(out Vector2i tileIndices);
		((Vector2i)(ref tileIndices))._002Ector((int)xIndex, (int)yIndex);
		OnTileModified(uid, grid, chunk, tileIndices, tile, oldTile, shapeChanged);
		return true;
	}

	public void SetAmbientLight(MapId mapId, Color color)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid map = GetMap(mapId);
		MapLightComponent mapLightComponent = EnsureComp<MapLightComponent>(map);
		if (!((Color)(ref mapLightComponent.AmbientLightColor)).Equals(color))
		{
			mapLightComponent.AmbientLightColor = color;
			Dirty(map, mapLightComponent);
		}
	}

	private void OnMapLightGetState(EntityUid uid, MapLightComponent component, ref ComponentGetState args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		args.State = new MapLightComponentState
		{
			AmbientLightColor = component.AmbientLightColor
		};
	}

	private void OnMapLightHandleState(EntityUid uid, MapLightComponent component, ref ComponentHandleState args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (args.Current is MapLightComponentState mapLightComponentState)
		{
			component.AmbientLightColor = mapLightComponentState.AmbientLightColor;
		}
	}

	private void InitializeMap()
	{
		SubscribeLocalEvent<MapComponent, ComponentAdd>(OnComponentAdd);
		SubscribeLocalEvent<MapComponent, ComponentInit>(OnCompInit);
		SubscribeLocalEvent<MapComponent, ComponentStartup>(OnCompStartup);
		SubscribeLocalEvent<MapComponent, MapInitEvent>(OnMapInit);
		SubscribeLocalEvent<MapComponent, ComponentShutdown>(OnMapRemoved);
		SubscribeLocalEvent<MapComponent, ComponentHandleState>(OnMapHandleState);
		SubscribeLocalEvent<MapComponent, ComponentGetState>(OnMapGetState);
	}

	public bool MapExists([NotNullWhen(true)] MapId? mapId)
	{
		if (mapId.HasValue)
		{
			return Maps.ContainsKey(mapId.Value);
		}
		return false;
	}

	public EntityUid GetMap(MapId mapId)
	{
		return Maps[mapId];
	}

	public EntityUid GetMapOrInvalid(MapId? mapId)
	{
		if (TryGetMap(mapId, out var uid))
		{
			return uid.Value;
		}
		return EntityUid.Invalid;
	}

	public bool TryGetMap([NotNullWhen(true)] MapId? mapId, [NotNullWhen(true)] out EntityUid? uid)
	{
		if (!mapId.HasValue || !Maps.TryGetValue(mapId.Value, out var value))
		{
			uid = null;
			return false;
		}
		uid = value;
		return true;
	}

	private void OnMapHandleState(EntityUid uid, MapComponent component, ref ComponentHandleState args)
	{
		if (!(args.Current is MapComponentState mapComponentState))
		{
			return;
		}
		if (component.MapId == MapId.Nullspace)
		{
			if (mapComponentState.MapId == MapId.Nullspace)
			{
				throw new Exception($"Received invalid map state for {ToPrettyString(uid)}");
			}
			AssignMapId((Owner: uid, Comp: component), mapComponentState.MapId);
			RecursiveMapIdUpdate(uid, uid, component.MapId);
		}
		if (component.MapId != mapComponentState.MapId)
		{
			throw new Exception($"Received invalid map state for {ToPrettyString(uid)}");
		}
		component.LightingEnabled = mapComponentState.LightingEnabled;
		component.MapInitialized = mapComponentState.Initialized;
		if ((int)LifeStage(uid) >= 2)
		{
			SetPaused(uid, mapComponentState.MapPaused);
		}
		else
		{
			component.MapPaused = mapComponentState.MapPaused;
		}
	}

	private void RecursiveMapIdUpdate(EntityUid uid, EntityUid mapUid, MapId mapId)
	{
		TransformComponent transformComponent = Transform(uid);
		transformComponent.MapUid = mapUid;
		transformComponent.MapID = mapId;
		transformComponent._mapIdInitialized = true;
		foreach (EntityUid child in transformComponent._children)
		{
			RecursiveMapIdUpdate(child, mapUid, mapId);
		}
	}

	private void OnMapGetState(EntityUid uid, MapComponent component, ref ComponentGetState args)
	{
		args.State = new MapComponentState(component.MapId, component.LightingEnabled, component.MapPaused, component.MapInitialized);
	}

	protected MapId TakeNextMapId()
	{
		MapId nextMapId = GetNextMapId();
		LastMapId = nextMapId.Value;
		return nextMapId;
	}

	internal abstract MapId GetNextMapId();

	private void OnComponentAdd(EntityUid uid, MapComponent component, ComponentAdd args)
	{
		EnsureComp<GridTreeComponent>(uid);
	}

	internal MapId AllocateMapId(EntityUid ent)
	{
		MapId mapId = (_reserved[ent] = TakeNextMapId());
		MapId mapId3 = mapId;
		Maps.Add(mapId3, ent);
		UsedIds.Add(mapId3);
		return mapId3;
	}

	internal void AssignMapId(Entity<MapComponent> map, MapId? id = null)
	{
		if (map.Comp.MapId != MapId.Nullspace)
		{
			if (id.HasValue)
			{
				MapId mapId = map.Comp.MapId;
				MapId? mapId2 = id;
				if (mapId != mapId2)
				{
					QueueDel(map.Owner);
					throw new Exception($"Map entity {ToPrettyString(map.Owner)} has already been assigned an id");
				}
			}
			if (!Maps.TryGetValue(map.Comp.MapId, out var value) || value != map.Owner)
			{
				QueueDel(map.Owner);
				throw new Exception($"Map entity {ToPrettyString(map.Owner)} was improperly assigned a map id?");
			}
			return;
		}
		if (_reserved.TryGetValue(map.Owner, out var value2))
		{
			map.Comp.MapId = value2;
			return;
		}
		map.Comp.MapId = id ?? TakeNextMapId();
		if (IsClientSide(map) != map.Comp.MapId.IsClientSide)
		{
			throw new Exception("Attempting to assign a client-side map id to a networked entity or vice-versa");
		}
		if (!UsedIds.Add(map.Comp.MapId))
		{
			base.Log.Warning($"Re-using a previously used map id ({map.Comp.MapId}) for map entity {ToPrettyString(map)}");
		}
		if (Maps.TryAdd(map.Comp.MapId, map.Owner) || Maps[map.Comp.MapId] == map.Owner)
		{
			return;
		}
		QueueDel(map);
		throw new Exception($"Attempted to assign an existing mapId {map.Comp} to a map entity {ToPrettyString(map.Owner)}");
	}

	private void OnCompInit(Entity<MapComponent> map, ref ComponentInit args)
	{
		AssignMapId(map);
		MapChangedEvent args2 = new MapChangedEvent(map, map.Comp.MapId, created: true);
		RaiseLocalEvent(map, args2, broadcast: true);
		RaiseLocalEvent(args: new MapCreatedEvent(map, map.Comp.MapId), uid: map, broadcast: true);
	}

	private void OnMapInit(EntityUid uid, MapComponent component, MapInitEvent args)
	{
		component.MapInitialized = true;
		Dirty(uid, component);
	}

	private void OnCompStartup(EntityUid uid, MapComponent component, ComponentStartup args)
	{
		component.MapPaused |= !component.MapInitialized;
		if (component.MapPaused)
		{
			component.MapPaused = false;
			SetPaused(uid, paused: true);
		}
	}

	private void OnMapRemoved(EntityUid uid, MapComponent component, ComponentShutdown args)
	{
		Maps.Remove(component.MapId);
		MapChangedEvent args2 = new MapChangedEvent(uid, component.MapId, created: false);
		RaiseLocalEvent(uid, args2, broadcast: true);
		MapRemovedEvent args3 = new MapRemovedEvent(uid, component.MapId);
		RaiseLocalEvent(uid, args3, broadcast: true);
	}

	public EntityUid CreateMap(out MapId mapId, bool runMapInit = true)
	{
		mapId = TakeNextMapId();
		return CreateMap(mapId, runMapInit);
	}

	public EntityUid CreateMap(bool runMapInit = true)
	{
		MapId mapId;
		return CreateMap(out mapId, runMapInit);
	}

	public EntityUid CreateMap(MapId mapId, bool runMapInit = true)
	{
		if (Maps.ContainsKey(mapId))
		{
			throw new ArgumentException($"Map with id {mapId} already exists");
		}
		if (mapId == MapId.Nullspace)
		{
			throw new ArgumentException("Cannot create a null-space map");
		}
		if (_netManager.IsServer && mapId.IsClientSide)
		{
			throw new ArgumentException("Attempted to create a client-side map on the server?");
		}
		if (_netManager.IsClient && _netManager.IsConnected && !mapId.IsClientSide)
		{
			throw new ArgumentException("Attempted to create a client-side map entity with a non client-side map ID?");
		}
		if (UsedIds.Contains(mapId))
		{
			base.Log.Warning($"Re-using MapId: {mapId}");
		}
		var (entityUid2, item, meta) = (Entity<MapComponent, MetaDataComponent>)(ref CreateUninitializedMap());
		AssignMapId((Owner: entityUid2, Comp: item), mapId);
		EntityManager.InitializeEntity(entityUid2, meta);
		EntityManager.StartEntity(entityUid2);
		if (runMapInit)
		{
			InitializeMap((Owner: entityUid2, Comp: item));
		}
		else
		{
			SetPaused((Owner: entityUid2, Comp: item), paused: true);
		}
		return entityUid2;
	}

	public Entity<MapComponent, MetaDataComponent> CreateUninitializedMap()
	{
		MetaDataComponent meta;
		EntityUid entityUid = EntityManager.CreateEntityUninitialized(null, out meta);
		_meta.SetEntityName(entityUid, "Map Entity", meta);
		return (Owner: entityUid, Comp1: AddComp<MapComponent>(entityUid), Comp2: meta);
	}

	public void DeleteMap(MapId mapId)
	{
		if (TryGetMap(mapId, out var uid))
		{
			Del(uid);
		}
	}

	public void QueueDeleteMap(MapId mapId)
	{
		if (TryGetMap(mapId, out var uid))
		{
			QueueDel(uid);
		}
	}

	public IEnumerable<MapId> GetAllMapIds()
	{
		return Maps.Keys;
	}

	public bool IsInitialized(MapId mapId)
	{
		if (mapId == MapId.Nullspace)
		{
			return true;
		}
		if (!Maps.TryGetValue(mapId, out var value))
		{
			throw new ArgumentException($"Map {mapId} does not exist.");
		}
		return IsInitialized(value);
	}

	public bool IsInitialized(EntityUid? map)
	{
		if (!map.HasValue)
		{
			return true;
		}
		return IsInitialized(map.Value);
	}

	public bool IsInitialized(Entity<MapComponent?> map)
	{
		if (!_mapQuery.Resolve(map, ref map.Comp))
		{
			return false;
		}
		return map.Comp.MapInitialized;
	}

	public void InitializeMap(MapId mapId, bool unpause = true)
	{
		if (!Maps.TryGetValue(mapId, out var value))
		{
			throw new ArgumentException($"Map {mapId} does not exist.");
		}
		InitializeMap(value, unpause);
	}

	public void InitializeMap(Entity<MapComponent?> map, bool unpause = true)
	{
		if (_mapQuery.Resolve(map, ref map.Comp))
		{
			if (map.Comp.MapInitialized)
			{
				throw new ArgumentException($"Map {ToPrettyString(map)} is already initialized.");
			}
			RecursiveMapInit(map.Owner);
			if (unpause)
			{
				SetPaused(map, paused: false);
			}
		}
	}

	internal void RecursiveMapInit(EntityUid entity)
	{
		List<EntityUid> list = new List<EntityUid> { entity };
		for (int i = 0; i < list.Count; i++)
		{
			EntityUid entityUid = list[i];
			if (_metaQuery.TryComp(entityUid, out MetaDataComponent component) && component.EntityLifeStage != EntityLifeStage.MapInitialized)
			{
				list.AddRange(Transform(entityUid)._children);
				EntityManager.RunMapInit(entityUid, component);
			}
		}
	}

	public bool IsPaused(MapId mapId)
	{
		if (mapId == MapId.Nullspace)
		{
			return false;
		}
		if (!Maps.TryGetValue(mapId, out var value))
		{
			throw new ArgumentException($"Map {mapId} does not exist.");
		}
		return IsPaused(value);
	}

	public bool IsPaused(Entity<MapComponent?> map)
	{
		if (!_mapQuery.Resolve(map, ref map.Comp))
		{
			return false;
		}
		if (!map.Comp.MapPaused)
		{
			return !map.Comp.MapInitialized;
		}
		return true;
	}

	public void SetPaused(MapId mapId, bool paused)
	{
		if (!Maps.TryGetValue(mapId, out var value))
		{
			throw new ArgumentException($"Map {mapId} does not exist.");
		}
		SetPaused(value, paused);
	}

	public void SetPaused(Entity<MapComponent?> map, bool paused)
	{
		if (_mapQuery.Resolve(map, ref map.Comp) && map.Comp.MapPaused != paused)
		{
			map.Comp.MapPaused = paused;
			if ((int)map.Comp.LifeStage >= 3)
			{
				Dirty(map);
				RecursiveSetPaused(map, paused);
			}
		}
	}

	internal void RecursiveSetPaused(EntityUid entity, bool paused)
	{
		_meta.SetEntityPaused(entity, paused);
		foreach (EntityUid child in Transform(entity)._children)
		{
			RecursiveSetPaused(child, paused);
		}
	}
}
