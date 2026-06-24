using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Analyzers;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Collision;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Shapes;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;

namespace Robust.Shared.Map;

[Virtual]
internal class MapManager : IMapManagerInternal, IMapManager, IEntityEventSubscriber
{
	private record struct GridQueryState<T, TState>(GridCallback<TState> Callback, TState State, Box2 WorldAABB, T Shape, Transform Transform, B2DynamicTree<(EntityUid Uid, FixturesComponent Fixtures, MapGridComponent Grid)> Tree, SharedMapSystem MapSystem, MapManager MapManager, SharedTransformSystem TransformSystem, bool Approximate);

	[Robust.Shared.IoC.Dependency]
	public readonly IGameTiming GameTiming;

	[Robust.Shared.IoC.Dependency]
	public readonly IEntityManager EntityManager;

	[Robust.Shared.IoC.Dependency]
	private readonly IManifoldManager _manifolds;

	[Robust.Shared.IoC.Dependency]
	private readonly ILogManager _logManager;

	[Robust.Shared.IoC.Dependency]
	private readonly IConsoleHost _conhost;

	private ISawmill _sawmill;

	private SharedMapSystem _mapSystem;

	private SharedPhysicsSystem _physics;

	private SharedTransformSystem _transformSystem;

	private EntityQuery<GridTreeComponent> _gridTreeQuery;

	private EntityQuery<MapGridComponent> _gridQuery;

	public bool SuppressOnTileChanged { get; set; }

	public void Initialize()
	{
		_gridTreeQuery = EntityManager.GetEntityQuery<GridTreeComponent>();
		_gridQuery = EntityManager.GetEntityQuery<MapGridComponent>();
		InitializeMapPausing();
		_sawmill = _logManager.GetSawmill("system.map");
	}

	public void Startup()
	{
		_physics = EntityManager.System<SharedPhysicsSystem>();
		_transformSystem = EntityManager.System<SharedTransformSystem>();
		_mapSystem = EntityManager.System<SharedMapSystem>();
		_sawmill.Debug("Starting...");
	}

	public void Shutdown()
	{
		_sawmill.Debug("Stopping...");
		EntityQueryEnumerator<MapComponent> entityQueryEnumerator = EntityManager.EntityQueryEnumerator<MapComponent>();
		EntityUid uid;
		MapComponent comp;
		while (entityQueryEnumerator.MoveNext(out uid, out comp))
		{
			EntityManager.DeleteEntity(uid);
		}
	}

	public void Restart()
	{
		_sawmill.Debug("Restarting...");
		EntityQueryEnumerator<MapComponent> entityQueryEnumerator = EntityManager.EntityQueryEnumerator<MapComponent>();
		EntityUid uid;
		MapComponent comp;
		while (entityQueryEnumerator.MoveNext(out uid, out comp))
		{
			EntityManager.DeleteEntity(uid);
		}
	}

	public MapGridComponent CreateGrid(MapId currentMapId, ushort chunkSize = 16)
	{
		return CreateGrid(GetMapEntityIdOrThrow(currentMapId), chunkSize, default(EntityUid));
	}

	public MapGridComponent CreateGrid(MapId currentMapId, in GridCreateOptions options)
	{
		return CreateGrid(GetMapEntityIdOrThrow(currentMapId), options.ChunkSize, default(EntityUid));
	}

	public MapGridComponent CreateGrid(MapId currentMapId)
	{
		return CreateGrid(currentMapId, in GridCreateOptions.Default);
	}

	public Entity<MapGridComponent> CreateGridEntity(MapId currentMapId, GridCreateOptions? options = null)
	{
		return CreateGridEntity(GetMapEntityIdOrThrow(currentMapId), options);
	}

	public Entity<MapGridComponent> CreateGridEntity(EntityUid map, GridCreateOptions? options = null)
	{
		GridCreateOptions valueOrDefault = options.GetValueOrDefault();
		if (!options.HasValue)
		{
			valueOrDefault = GridCreateOptions.Default;
			options = valueOrDefault;
		}
		return CreateGrid(map, options.Value.ChunkSize, default(EntityUid));
	}

	[Obsolete("Use HasComponent<MapGridComponent>(uid)")]
	public bool IsGrid(EntityUid uid)
	{
		return EntityManager.HasComponent<MapGridComponent>(uid);
	}

	public IEnumerable<MapGridComponent> GetAllMapGrids(MapId mapId)
	{
		AllEntityQueryEnumerator<MapGridComponent, TransformComponent> query = EntityManager.AllEntityQueryEnumerator<MapGridComponent, TransformComponent>();
		MapGridComponent comp;
		TransformComponent comp2;
		while (query.MoveNext(out comp, out comp2))
		{
			if (comp2.MapID == mapId)
			{
				yield return comp;
			}
		}
	}

	public IEnumerable<Entity<MapGridComponent>> GetAllGrids(MapId mapId)
	{
		AllEntityQueryEnumerator<MapGridComponent, TransformComponent> query = EntityManager.AllEntityQueryEnumerator<MapGridComponent, TransformComponent>();
		EntityUid uid;
		MapGridComponent comp;
		TransformComponent comp2;
		while (query.MoveNext(out uid, out comp, out comp2))
		{
			if (!(comp2.MapID != mapId))
			{
				yield return (Owner: uid, Comp: comp);
			}
		}
	}

	public virtual void DeleteGrid(EntityUid euid)
	{
		if (EntityManager.TryGetComponent<MapGridComponent>(euid, out MapGridComponent _) && EntityManager.TryGetComponent<MetaDataComponent>(euid, out MetaDataComponent component2) && (int)component2.EntityLifeStage < 4)
		{
			EntityManager.DeleteEntity(euid);
		}
	}

	void IMapManagerInternal.RaiseOnTileChanged(Entity<MapGridComponent> entity, TileRef tileRef, Tile oldTile, Vector2i chunk)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!SuppressOnTileChanged)
		{
			TileChangedEvent args = new TileChangedEvent(entity, tileRef, oldTile, chunk);
			EntityManager.EventBus.RaiseLocalEvent(entity.Owner, ref args, broadcast: true);
		}
	}

	protected Entity<MapGridComponent> CreateGrid(EntityUid map, ushort chunkSize, EntityUid forcedGridEuid)
	{
		EntityUid entityUid = EntityManager.CreateEntityUninitialized(null, forcedGridEuid);
		MapGridComponent mapGridComponent = EntityManager.AddComponent<MapGridComponent>(entityUid);
		mapGridComponent.ChunkSize = chunkSize;
		_sawmill.Debug($"Binding new grid {entityUid}");
		EntityManager.GetComponent<TransformComponent>(entityUid).AttachParent(map);
		MetaDataComponent component = EntityManager.GetComponent<MetaDataComponent>(entityUid);
		EntityManager.System<MetaDataSystem>().SetEntityName(entityUid, "grid", component);
		EntityManager.InitializeComponents(entityUid, component);
		EntityManager.StartComponents(entityUid);
		return (Owner: entityUid, Comp: mapGridComponent);
	}

	public virtual void DeleteMap(MapId mapId)
	{
		_mapSystem.DeleteMap(mapId);
	}

	public MapId CreateMap(MapId? mapId = null)
	{
		if (mapId.HasValue)
		{
			_mapSystem.CreateMap(mapId.Value);
			return mapId.Value;
		}
		_mapSystem.CreateMap(out var mapId2);
		return mapId2;
	}

	public bool MapExists([NotNullWhen(true)] MapId? mapId)
	{
		return _mapSystem.MapExists(mapId);
	}

	public EntityUid GetMapEntityId(MapId mapId)
	{
		return _mapSystem.GetMapOrInvalid(mapId);
	}

	public EntityUid GetMapEntityIdOrThrow(MapId mapId)
	{
		return _mapSystem.GetMap(mapId);
	}

	public bool TryGetMap([NotNullWhen(true)] MapId? mapId, [NotNullWhen(true)] out EntityUid? uid)
	{
		return _mapSystem.TryGetMap(mapId, out uid);
	}

	public IEnumerable<MapId> GetAllMapIds()
	{
		return _mapSystem.GetAllMapIds();
	}

	public bool IsMap(EntityUid uid)
	{
		return EntityManager.HasComponent<MapComponent>(uid);
	}

	public void SetMapPaused(MapId mapId, bool paused)
	{
		_mapSystem.SetPaused(mapId, paused);
	}

	public void SetMapPaused(EntityUid uid, bool paused)
	{
		_mapSystem.SetPaused(uid, paused);
	}

	public void DoMapInitialize(MapId mapId)
	{
		_mapSystem.InitializeMap(mapId);
	}

	public bool IsMapInitialized(MapId mapId)
	{
		return _mapSystem.IsInitialized(mapId);
	}

	public bool IsMapPaused(MapId mapId)
	{
		return _mapSystem.IsPaused(mapId);
	}

	public bool IsMapPaused(EntityUid uid)
	{
		return _mapSystem.IsPaused(uid);
	}

	private void InitializeMapPausing()
	{
		_conhost.RegisterCommand("pausemap", "Pauses a map, pausing all simulation processing on it.", "pausemap <map ID>", delegate(IConsoleShell shell, string _, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteError("Need to supply a valid MapId");
			}
			else
			{
				MapId mapId = new MapId(int.Parse(args[0], CultureInfo.InvariantCulture));
				if (!MapExists(mapId))
				{
					shell.WriteError("That map does not exist.");
				}
				else
				{
					SetMapPaused(mapId, paused: true);
				}
			}
		});
		_conhost.RegisterCommand("querymappaused", "Check whether a map is paused or not.", "querymappaused <map ID>", delegate(IConsoleShell shell, string _, string[] args)
		{
			MapId mapId = new MapId(int.Parse(args[0], CultureInfo.InvariantCulture));
			if (!MapExists(mapId))
			{
				shell.WriteError("That map does not exist.");
			}
			else
			{
				shell.WriteLine(_mapSystem.IsPaused(mapId).ToString());
			}
		});
		_conhost.RegisterCommand("unpausemap", "unpauses a map, resuming all simulation processing on it.", "Usage: unpausemap <map ID>", delegate(IConsoleShell shell, string _, string[] args)
		{
			if (args.Length != 1)
			{
				shell.WriteLine("Need to supply a valid MapId");
			}
			else
			{
				MapId mapId = new MapId(int.Parse(args[0], CultureInfo.InvariantCulture));
				if (!MapExists(mapId))
				{
					shell.WriteLine("That map does not exist.");
				}
				else
				{
					SetMapPaused(mapId, paused: false);
				}
			}
		});
	}

	private bool IsIntersecting<T>(ChunkEnumerator enumerator, T shape, Transform shapeTransform, Entity<FixturesComponent> grid) where T : IPhysShape
	{
		Transform xfB = _physics.GetPhysicsTransform(grid);
		MapChunk chunk;
		while (enumerator.MoveNext(out chunk))
		{
			foreach (string fixture2 in chunk.Fixtures)
			{
				Fixture fixture = grid.Comp.Fixtures[fixture2];
				for (int i = 0; i < fixture.Shape.ChildCount; i++)
				{
					if (_manifolds.TestOverlap(shape, 0, fixture.Shape, i, in shapeTransform, in xfB))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public void FindGridsIntersecting<T>(MapId mapId, T shape, Transform transform, ref List<Entity<MapGridComponent>> grids, bool approx = false, bool includeMap = true) where T : IPhysShape
	{
		if (_mapSystem.TryGetMap(mapId, out var uid))
		{
			FindGridsIntersecting(uid.Value, shape, transform, ref grids, approx, includeMap);
		}
	}

	public void FindGridsIntersecting<T>(MapId mapId, T shape, Transform transform, GridCallback callback, bool approx = false, bool includeMap = true) where T : IPhysShape
	{
		if (_mapSystem.TryGetMap(mapId, out var uid))
		{
			FindGridsIntersecting(uid.Value, shape, transform, callback, includeMap, approx);
		}
	}

	public void FindGridsIntersecting(MapId mapId, Box2 worldAABB, GridCallback callback, bool approx = false, bool includeMap = true)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (_mapSystem.TryGetMap(mapId, out var uid))
		{
			FindGridsIntersecting(uid.Value, worldAABB, callback, approx, includeMap);
		}
	}

	public void FindGridsIntersecting<TState>(MapId mapId, Box2 worldAABB, ref TState state, GridCallback<TState> callback, bool approx = false, bool includeMap = true)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (_mapSystem.TryGetMap(mapId, out var uid))
		{
			FindGridsIntersecting(uid.Value, worldAABB, ref state, callback, approx, includeMap);
		}
	}

	public void FindGridsIntersecting(MapId mapId, Box2 worldAABB, ref List<Entity<MapGridComponent>> grids, bool approx = false, bool includeMap = true)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (_mapSystem.TryGetMap(mapId, out var uid))
		{
			FindGridsIntersecting(uid.Value, worldAABB, ref grids, approx, includeMap);
		}
	}

	public void FindGridsIntersecting(MapId mapId, Box2Rotated worldBounds, GridCallback callback, bool approx = false, bool includeMap = true)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (_mapSystem.TryGetMap(mapId, out var uid))
		{
			FindGridsIntersecting(uid.Value, worldBounds, callback, approx, includeMap);
		}
	}

	public void FindGridsIntersecting<TState>(MapId mapId, Box2Rotated worldBounds, ref TState state, GridCallback<TState> callback, bool approx = false, bool includeMap = true)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (_mapSystem.TryGetMap(mapId, out var uid))
		{
			FindGridsIntersecting(uid.Value, worldBounds, ref state, callback, approx, includeMap);
		}
	}

	public void FindGridsIntersecting(MapId mapId, Box2Rotated worldBounds, ref List<Entity<MapGridComponent>> grids, bool approx = false, bool includeMap = true)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (_mapSystem.TryGetMap(mapId, out var uid))
		{
			FindGridsIntersecting(uid.Value, worldBounds, ref grids, approx, includeMap);
		}
	}

	public void FindGridsIntersecting<T>(EntityUid mapEnt, T shape, Transform transform, GridCallback callback, bool approx = false, bool includeMap = true) where T : IPhysShape
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		FindGridsIntersecting(mapEnt, shape, shape.ComputeAABB(transform, 0), transform, callback, approx, includeMap);
	}

	private void FindGridsIntersecting<T>(EntityUid mapEnt, T shape, Box2 worldAABB, Transform transform, GridCallback callback, bool approx = false, bool includeMap = true) where T : IPhysShape
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		GridCallback state = callback;
		FindGridsIntersecting(mapEnt, shape, worldAABB, transform, ref state, delegate(EntityUid uid, MapGridComponent grid, ref GridCallback reference)
		{
			return reference(uid, grid);
		}, approx, includeMap);
	}

	public void FindGridsIntersecting<T, TState>(EntityUid mapEnt, T shape, Transform transform, ref TState state, GridCallback<TState> callback, bool approx = false, bool includeMap = true) where T : IPhysShape
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		FindGridsIntersecting(mapEnt, shape, shape.ComputeAABB(transform, 0), transform, ref state, callback, approx, includeMap);
	}

	private void FindGridsIntersecting<T, TState>(EntityUid mapEnt, T shape, Box2 worldAABB, Transform transform, ref TState state, GridCallback<TState> callback, bool approx = false, bool includeMap = true) where T : IPhysShape
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!_gridTreeQuery.TryGetComponent(mapEnt, out GridTreeComponent component))
		{
			return;
		}
		if (includeMap && _gridQuery.TryGetComponent(mapEnt, out MapGridComponent component2))
		{
			callback(mapEnt, component2, ref state);
		}
		GridQueryState<T, TState> state2 = new GridQueryState<T, TState>(callback, state, worldAABB, shape, transform, component.Tree, _mapSystem, this, _transformSystem, approx);
		component.Tree.Query(ref state2, delegate(ref GridQueryState<T, TState> reference, DynamicTree.Proxy proxy)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			(EntityUid, FixturesComponent, MapGridComponent) userData = reference.Tree.GetUserData(proxy);
			Matrix3x2 invWorldMatrix = reference.TransformSystem.GetInvWorldMatrix(userData.Item1);
			Box2 worldAABB2 = reference.WorldAABB;
			Box2 localAABB = Matrix3Helpers.TransformBox(invWorldMatrix, ref worldAABB2);
			ChunkEnumerator localMapChunks = reference.MapSystem.GetLocalMapChunks(userData.Item1, userData.Item3, localAABB);
			if (reference.Approximate)
			{
				if (!localMapChunks.MoveNext(out MapChunk _))
				{
					return true;
				}
			}
			else if (!reference.MapManager.IsIntersecting(localMapChunks, reference.Shape, reference.Transform, (Owner: userData.Item1, Comp: userData.Item2)))
			{
				return true;
			}
			TState state3 = reference.State;
			bool result = reference.Callback(userData.Item1, userData.Item3, ref state3);
			reference.State = state3;
			return result;
		}, in worldAABB);
		state = state2.State;
	}

	public void FindGridsIntersecting(EntityUid mapEnt, List<IPhysShape> shapes, Transform transform, ref List<Entity<MapGridComponent>> entities, bool approx = false, bool includeMap = true)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		foreach (IPhysShape shape in shapes)
		{
			FindGridsIntersecting(mapEnt, shape, shape.ComputeAABB(transform, 0), transform, ref entities);
		}
	}

	public void FindGridsIntersecting<T>(EntityUid mapEnt, T shape, Transform transform, ref List<Entity<MapGridComponent>> grids, bool approx = false, bool includeMap = true) where T : IPhysShape
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		FindGridsIntersecting(mapEnt, shape, shape.ComputeAABB(transform, 0), transform, ref grids, approx, includeMap);
	}

	public void FindGridsIntersecting<T>(EntityUid mapEnt, T shape, Box2 worldAABB, Transform transform, ref List<Entity<MapGridComponent>> grids, bool approx = false, bool includeMap = true) where T : IPhysShape
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		List<Entity<MapGridComponent>> state = grids;
		FindGridsIntersecting<T, List<Entity<MapGridComponent>>>(mapEnt, shape, worldAABB, transform, ref state, delegate(EntityUid uid, MapGridComponent grid, ref List<Entity<MapGridComponent>> list)
		{
			list.Add((Owner: uid, Comp: grid));
			return true;
		}, approx, includeMap);
	}

	public void FindGridsIntersecting(EntityUid mapEnt, Box2 worldAABB, GridCallback callback, bool approx = false, bool includeMap = true)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		SlimPolygon shape = new SlimPolygon(worldAABB);
		FindGridsIntersecting(mapEnt, shape, worldAABB, Transform.Empty, callback, approx, includeMap);
	}

	public void FindGridsIntersecting<TState>(EntityUid mapEnt, Box2 worldAABB, ref TState state, GridCallback<TState> callback, bool approx = false, bool includeMap = true)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		SlimPolygon shape = new SlimPolygon(worldAABB);
		FindGridsIntersecting(mapEnt, shape, worldAABB, Transform.Empty, ref state, callback, approx, includeMap);
	}

	public void FindGridsIntersecting(EntityUid mapEnt, Box2 worldAABB, ref List<Entity<MapGridComponent>> grids, bool approx = false, bool includeMap = true)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		SlimPolygon shape = new SlimPolygon(worldAABB);
		FindGridsIntersecting(mapEnt, shape, worldAABB, Transform.Empty, ref grids, approx, includeMap);
	}

	public void FindGridsIntersecting(EntityUid mapEnt, Box2Rotated worldBounds, GridCallback callback, bool approx = false, bool includeMap = true)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SlimPolygon shape = new SlimPolygon(in worldBounds);
		FindGridsIntersecting(mapEnt, shape, ((Box2Rotated)(ref worldBounds)).CalcBoundingBox(), Transform.Empty, callback, approx, includeMap);
	}

	public void FindGridsIntersecting<TState>(EntityUid mapEnt, Box2Rotated worldBounds, ref TState state, GridCallback<TState> callback, bool approx = false, bool includeMap = true)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SlimPolygon shape = new SlimPolygon(in worldBounds);
		FindGridsIntersecting(mapEnt, shape, ((Box2Rotated)(ref worldBounds)).CalcBoundingBox(), Transform.Empty, ref state, callback, approx, includeMap);
	}

	public void FindGridsIntersecting(EntityUid mapEnt, Box2Rotated worldBounds, ref List<Entity<MapGridComponent>> grids, bool approx = false, bool includeMap = true)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		SlimPolygon shape = new SlimPolygon(in worldBounds);
		FindGridsIntersecting(mapEnt, shape, ((Box2Rotated)(ref worldBounds)).CalcBoundingBox(), Transform.Empty, ref grids, approx, includeMap);
	}

	public bool TryFindGridAt(EntityUid mapEnt, Vector2 worldPos, out EntityUid uid, [NotNullWhen(true)] out MapGridComponent? grid)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = new Vector2(0.2f, 0.2f);
		Unsafe.SkipInit(out Box2 worldAABB);
		((Box2)(ref worldAABB))._002Ector(worldPos - vector, worldPos + vector);
		uid = EntityUid.Invalid;
		grid = null;
		(EntityUid, MapGridComponent, Vector2, SharedMapSystem, SharedTransformSystem) state = (uid, grid, worldPos, _mapSystem, _transformSystem);
		FindGridsIntersecting<(EntityUid, MapGridComponent, Vector2, SharedMapSystem, SharedTransformSystem)>(mapEnt, worldAABB, ref state, delegate(EntityUid iUid, MapGridComponent iGrid, ref (EntityUid uid, MapGridComponent? grid, Vector2 worldPos, SharedMapSystem mapSystem, SharedTransformSystem xformSystem) reference)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			Matrix3x2 invWorldMatrix = reference.xformSystem.GetInvWorldMatrix(iUid);
			Vector2 tile = Vector2.Transform(reference.worldPos, invWorldMatrix);
			Vector2i chunkIndices = SharedMapSystem.GetChunkIndices(tile, iGrid.ChunkSize);
			if (!iGrid.Chunks.TryGetValue(chunkIndices, out MapChunk value))
			{
				return true;
			}
			Vector2i chunkRelative = SharedMapSystem.GetChunkRelative(tile, iGrid.ChunkSize);
			if (value.GetTile(chunkRelative).IsEmpty)
			{
				return true;
			}
			reference.uid = iUid;
			reference.grid = iGrid;
			return false;
		}, approx: true, includeMap: false);
		if (state.Item2 == null && _gridQuery.TryGetComponent(mapEnt, out MapGridComponent component))
		{
			uid = mapEnt;
			grid = component;
			return true;
		}
		(uid, grid, _, _, _) = state;
		return grid != null;
	}

	public bool TryFindGridAt(MapId mapId, Vector2 worldPos, out EntityUid uid, [NotNullWhen(true)] out MapGridComponent? grid)
	{
		if (_mapSystem.TryGetMap(mapId, out var uid2))
		{
			return TryFindGridAt(uid2.Value, worldPos, out uid, out grid);
		}
		uid = default(EntityUid);
		grid = null;
		return false;
	}

	public bool TryFindGridAt(MapCoordinates mapCoordinates, out EntityUid uid, [NotNullWhen(true)] out MapGridComponent? grid)
	{
		return TryFindGridAt(mapCoordinates.MapId, mapCoordinates.Position, out uid, out grid);
	}

	MapGridComponent IMapManager.CreateGrid(MapId currentMapId, in GridCreateOptions options)
	{
		return CreateGrid(currentMapId, in options);
	}
}
