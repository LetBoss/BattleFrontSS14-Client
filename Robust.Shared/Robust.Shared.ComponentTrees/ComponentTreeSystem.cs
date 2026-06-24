using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;

namespace Robust.Shared.ComponentTrees;

public abstract class ComponentTreeSystem<TTreeComp, TComp> : EntitySystem where TTreeComp : Component, IComponentTreeComponent<TComp>, new() where TComp : Component, IComponentTreeEntry<TComp>
{
	private struct QueryState<TPredicateState>(QueryState inner, TPredicateState predicateState, Func<Entity<TComp, TransformComponent>, TPredicateState, bool> ignore) : IDone
	{
		public readonly TPredicateState PredicateState = predicateState;

		public readonly Func<Entity<TComp, TransformComponent>, TPredicateState, bool> Ignore = ignore;

		public QueryState Inner = inner;

		public bool Done => Inner.Done;
	}

	private struct QueryState(float maxLength, List<RayCastResults>? list = null) : IDone
	{
		public readonly float MaxLength = maxLength;

		public readonly List<RayCastResults>? List = list;

		public RayCastResults? Result = null;

		[MemberNotNullWhen(false, "List")]
		public readonly bool ReturnOnFirstHit
		{
			[MemberNotNullWhen(false, "List")]
			get
			{
				return List == null;
			}
		}

		public bool Done => Result.HasValue;
	}

	private interface IDone
	{
		bool Done { get; }
	}

	[Robust.Shared.IoC.Dependency]
	private readonly RecursiveMoveSystem _recursiveMoveSys;

	[Robust.Shared.IoC.Dependency]
	protected readonly SharedTransformSystem XformSystem;

	[Robust.Shared.IoC.Dependency]
	private readonly IMapManager _mapManager;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedMapSystem _mapSystem;

	private readonly Queue<ComponentTreeEntry<TComp>> _updateQueue = new Queue<ComponentTreeEntry<TComp>>();

	protected EntityQuery<TComp> Query;

	private bool _initialized;

	protected virtual bool Enabled => true;

	protected abstract bool DoFrameUpdate { get; }

	protected abstract bool DoTickUpdate { get; }

	protected virtual int InitialCapacity { get; } = 256;

	protected abstract bool Recursive { get; }

	public override void Initialize()
	{
		base.Initialize();
		if (Enabled)
		{
			_initialized = true;
			base.UpdatesOutsidePrediction = DoTickUpdate;
			base.UpdatesAfter.Add(typeof(SharedTransformSystem));
			base.UpdatesAfter.Add(typeof(SharedPhysicsSystem));
			SubscribeLocalEvent<MapCreatedEvent>(MapManagerOnMapCreated);
			SubscribeLocalEvent<GridInitializeEvent>(MapManagerOnGridCreated);
			SubscribeLocalEvent<TComp, ComponentStartup>(OnCompStartup);
			SubscribeLocalEvent<TComp, ComponentRemove>(OnCompRemoved);
			if (Recursive)
			{
				_recursiveMoveSys.OnTreeRecursiveMove += HandleRecursiveMove;
				_recursiveMoveSys.AddSubscription();
			}
			else
			{
				SubscribeLocalEvent<TComp, MoveEvent>(HandleMove);
			}
			SubscribeLocalEvent<TTreeComp, EntityTerminatingEvent>(OnTerminating);
			SubscribeLocalEvent<TTreeComp, ComponentAdd>(OnTreeAdd);
			SubscribeLocalEvent<TTreeComp, ComponentRemove>(OnTreeRemove);
			Query = GetEntityQuery<TComp>();
		}
	}

	public override void Shutdown()
	{
		if (_initialized)
		{
			_initialized = false;
			if (Recursive)
			{
				_recursiveMoveSys.OnTreeRecursiveMove -= HandleRecursiveMove;
			}
		}
	}

	private bool CheckEnabled()
	{
		if (_initialized)
		{
			return true;
		}
		base.Log.Error("Attempted to use disabled lookup tree");
		return false;
	}

	private void HandleRecursiveMove(EntityUid uid, TransformComponent xform)
	{
		if (Query.TryGetComponent(uid, out TComp component))
		{
			QueueTreeUpdate(uid, component, xform);
		}
	}

	private void HandleMove(EntityUid uid, TComp component, ref MoveEvent args)
	{
		QueueTreeUpdate(uid, component, args.Component);
	}

	public void QueueTreeUpdate(EntityUid uid, TComp component, TransformComponent? xform = null)
	{
		if (_initialized && !component.TreeUpdateQueued && Resolve(uid, ref xform))
		{
			component.TreeUpdateQueued = true;
			_updateQueue.Enqueue((component, xform));
		}
	}

	public void QueueTreeUpdate(Entity<TComp> entity, TransformComponent? xform = null)
	{
		QueueTreeUpdate(entity.Owner, entity.Comp, xform);
	}

	protected virtual void OnCompStartup(EntityUid uid, TComp component, ComponentStartup args)
	{
		QueueTreeUpdate(uid, component);
	}

	protected virtual void OnCompRemoved(EntityUid uid, TComp component, ComponentRemove args)
	{
		RemoveFromTree(component);
	}

	protected virtual void OnTreeAdd(EntityUid uid, TTreeComp component, ComponentAdd args)
	{
		component.Tree = new DynamicTree<ComponentTreeEntry<TComp>>(ExtractAabb, null, 1f / 32f, InitialCapacity);
	}

	protected virtual void OnTreeRemove(EntityUid uid, TTreeComp component, ComponentRemove args)
	{
		foreach (ComponentTreeEntry<TComp> item in component.Tree)
		{
			item.Component.TreeUid = null;
			item.Component.Tree = null;
		}
		component.Tree.Clear();
	}

	protected virtual void OnTerminating(EntityUid uid, TTreeComp component, ref EntityTerminatingEvent args)
	{
		RemComp(uid, component);
	}

	private void MapManagerOnMapCreated(MapCreatedEvent e)
	{
		EnsureComp<TTreeComp>(e.Uid);
	}

	private void MapManagerOnGridCreated(GridInitializeEvent ev)
	{
		EnsureComp<TTreeComp>(ev.EntityUid);
	}

	public override void Update(float frameTime)
	{
		if (DoTickUpdate && _initialized)
		{
			UpdateTreePositions();
		}
	}

	public override void FrameUpdate(float frameTime)
	{
		if (DoFrameUpdate && _initialized)
		{
			UpdateTreePositions();
		}
	}

	public void UpdateTreePositions()
	{
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!CheckEnabled() || _updateQueue.Count == 0)
			{
				return;
			}
			EntityQuery<TTreeComp> entityQuery = GetEntityQuery<TTreeComp>();
			ComponentTreeEntry<TComp> result;
			while (_updateQueue.TryDequeue(out result))
			{
				ComponentTreeEntry<TComp> componentTreeEntry = result;
				componentTreeEntry.Deconstruct(out TComp component, out TransformComponent xform);
				TComp val = component;
				TransformComponent transformComponent = xform;
				val.TreeUpdateQueued = false;
				if (!val.Running)
				{
					continue;
				}
				if (!val.AddToTree || val.Deleted || !transformComponent.MapUid.HasValue)
				{
					RemoveFromTree(val);
					continue;
				}
				EntityUid? entityUid = transformComponent.GridUid ?? transformComponent.MapUid;
				if (!entityQuery.TryGetComponent(entityUid, out TTreeComp component2) && !val.TreeUid.HasValue)
				{
					continue;
				}
				Vector2 pos;
				Angle rot;
				if (val.TreeUid == entityUid)
				{
					(pos, rot) = XformSystem.GetRelativePositionRotation(result.Transform, entityUid.Value);
					component2.Tree.Update(in result, ExtractAabb(in result, pos, rot));
					continue;
				}
				RemoveFromTree(val);
				if (component2 == null)
				{
					break;
				}
				val.TreeUid = entityUid;
				val.Tree = component2.Tree;
				(pos, rot) = XformSystem.GetRelativePositionRotation(result.Transform, entityUid.Value);
				component2.Tree.Add(in result, ExtractAabb(in result, pos, rot));
			}
		}
		finally
		{
			_updateQueue.Clear();
		}
	}

	private void RemoveFromTree(TComp component)
	{
		DynamicTree<ComponentTreeEntry<TComp>>? tree = component.Tree;
		if (tree != null)
		{
			ComponentTreeEntry<TComp> item = new ComponentTreeEntry<TComp>
			{
				Component = component
			};
			tree.Remove(in item);
		}
		component.Tree = null;
		component.TreeUid = null;
	}

	protected virtual Box2 ExtractAabb(in ComponentTreeEntry<TComp> entry)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!entry.Component.TreeUid.HasValue)
		{
			return default(Box2);
		}
		var (pos, rot) = XformSystem.GetRelativePositionRotation(entry.Transform, entry.Component.TreeUid.Value);
		return ExtractAabb(in entry, pos, rot);
	}

	protected abstract Box2 ExtractAabb(in ComponentTreeEntry<TComp> entry, Vector2 pos, Angle rot);

	public IEnumerable<(EntityUid, TTreeComp)> GetIntersectingTrees(MapId mapId, Box2Rotated worldBounds)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return GetIntersectingTrees(mapId, ((Box2Rotated)(ref worldBounds)).CalcBoundingBox());
	}

	public IEnumerable<(EntityUid Uid, TTreeComp Comp)> GetIntersectingTrees(MapId mapId, Box2 worldAABB)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckEnabled())
		{
			return Array.Empty<(EntityUid, TTreeComp)>();
		}
		UpdateTreePositions();
		ValueList<(EntityUid, TTreeComp)> valueList = default(ValueList<(EntityUid, TTreeComp)>);
		if (mapId == MapId.Nullspace)
		{
			return valueList;
		}
		(EntityManager, ValueList<(EntityUid, TTreeComp)>) state = (EntityManager, valueList);
		_mapManager.FindGridsIntersecting<(EntityManager, ValueList<(EntityUid, TTreeComp)>)>(mapId, worldAABB, ref state, delegate(EntityUid entityUid, MapGridComponent grid, ref (EntityManager EntityManager, ValueList<(EntityUid, TTreeComp)> trees) tuple)
		{
			if (tuple.EntityManager.TryGetComponent<TTreeComp>(entityUid, out TTreeComp component))
			{
				tuple.trees.Add((entityUid, component));
			}
			return true;
		}, approx: false, includeMap: false);
		if (_mapSystem.TryGetMap(mapId, out var uid) && TryComp(uid, out TTreeComp comp))
		{
			state.Item2.Add((uid.Value, comp));
		}
		return state.Item2;
	}

	public HashSet<ComponentTreeEntry<TComp>> QueryAabb(MapId mapId, Box2 worldBounds, bool approx = true)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return this.QueryAabb(mapId, new Box2Rotated(worldBounds, default(Angle), default(Vector2)), approx);
	}

	public void QueryAabb(HashSet<Entity<TComp, TransformComponent>> results, MapId mapId, Box2 worldBounds, bool approx = true)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		this.QueryAabb(results, mapId, new Box2Rotated(worldBounds, default(Angle), default(Vector2)), approx);
	}

	public HashSet<ComponentTreeEntry<TComp>> QueryAabb(MapId mapId, Box2Rotated worldBounds, bool approx = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		HashSet<ComponentTreeEntry<TComp>> hashSet = new HashSet<ComponentTreeEntry<TComp>>();
		QueryAabb(hashSet, mapId, worldBounds, approx);
		return hashSet;
	}

	[Obsolete("Use Entity<T> variant")]
	internal void QueryAabb(HashSet<ComponentTreeEntry<TComp>> results, MapId mapId, Box2Rotated worldBounds, bool approx = true)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckEnabled())
		{
			return;
		}
		foreach (var intersectingTree in GetIntersectingTrees(mapId, worldBounds))
		{
			EntityUid item = intersectingTree.Item1;
			TTreeComp item2 = intersectingTree.Item2;
			Box2 aabb = Matrix3Helpers.TransformBox(XformSystem.GetInvWorldMatrix(item), ref worldBounds);
			item2.Tree.QueryAabb<HashSet<ComponentTreeEntry<TComp>>>(ref results, delegate(ref HashSet<ComponentTreeEntry<TComp>> state, in ComponentTreeEntry<TComp> value)
			{
				state.Add(value);
				return true;
			}, aabb, approx);
		}
	}

	public void QueryAabb(HashSet<Entity<TComp, TransformComponent>> results, MapId mapId, Box2Rotated worldBounds, bool approx = true)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckEnabled())
		{
			return;
		}
		foreach (var intersectingTree in GetIntersectingTrees(mapId, worldBounds))
		{
			EntityUid item = intersectingTree.Item1;
			TTreeComp item2 = intersectingTree.Item2;
			Box2 aabb = Matrix3Helpers.TransformBox(XformSystem.GetInvWorldMatrix(item), ref worldBounds);
			item2.Tree.QueryAabb<HashSet<Entity<TComp, TransformComponent>>>(ref results, delegate(ref HashSet<Entity<TComp, TransformComponent>> state, in ComponentTreeEntry<TComp> value)
			{
				state.Add(value);
				return true;
			}, aabb, approx);
		}
	}

	public void QueryAabb(List<Entity<TComp, TransformComponent>> results, MapId mapId, Box2 worldBounds, bool approx = true)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		this.QueryAabb(results, mapId, new Box2Rotated(worldBounds, default(Angle), default(Vector2)), approx);
	}

	public void QueryAabb(List<Entity<TComp, TransformComponent>> results, MapId mapId, Box2Rotated worldBounds, bool approx = true)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckEnabled())
		{
			return;
		}
		foreach (var intersectingTree in GetIntersectingTrees(mapId, worldBounds))
		{
			EntityUid item = intersectingTree.Item1;
			TTreeComp item2 = intersectingTree.Item2;
			Box2 aabb = Matrix3Helpers.TransformBox(XformSystem.GetInvWorldMatrix(item), ref worldBounds);
			item2.Tree.QueryAabb<List<Entity<TComp, TransformComponent>>>(ref results, delegate(ref List<Entity<TComp, TransformComponent>> state, in ComponentTreeEntry<TComp> value)
			{
				state.Add(value);
				return true;
			}, aabb, approx);
		}
	}

	public void QueryAabb<TState>(ref TState state, DynamicTree<ComponentTreeEntry<TComp>>.QueryCallbackDelegate<TState> callback, MapId mapId, Box2 worldBounds, bool approx = true)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		this.QueryAabb<TState>(ref state, callback, mapId, new Box2Rotated(worldBounds, default(Angle), default(Vector2)), approx);
	}

	public void QueryAabb<TState>(ref TState state, DynamicTree<ComponentTreeEntry<TComp>>.QueryCallbackDelegate<TState> callback, MapId mapId, Box2Rotated worldBounds, bool approx = true)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckEnabled())
		{
			return;
		}
		foreach (var intersectingTree in GetIntersectingTrees(mapId, worldBounds))
		{
			EntityUid item = intersectingTree.Item1;
			TTreeComp item2 = intersectingTree.Item2;
			Box2 aabb = Matrix3Helpers.TransformBox(XformSystem.GetInvWorldMatrix(item), ref worldBounds);
			item2.Tree.QueryAabb(ref state, callback, aabb, approx);
		}
	}

	[Obsolete("use IntersectRay")]
	public List<RayCastResults> IntersectRayWithPredicate<TState>(MapId mapId, in Ray ray, float maxLength, TState state, Func<EntityUid, TState, bool> predicate, bool returnOnFirstHit = true)
	{
		List<RayCastResults> list = new List<RayCastResults>();
		if (!returnOnFirstHit)
		{
			IntersectRay(list, mapId, in ray, maxLength, state, (Entity<TComp, TransformComponent> e, TState s) => predicate(e.Owner, s));
			return list;
		}
		RayCastResults? rayCastResults = IntersectRay(mapId, in ray, maxLength, state, (Entity<TComp, TransformComponent> e, TState s) => predicate(e.Owner, s));
		if (rayCastResults.HasValue)
		{
			list.Add(rayCastResults.Value);
		}
		return list;
	}

	public RayCastResults? IntersectRay(MapId mapId, in Ray ray, float length)
	{
		QueryState state = new QueryState(length);
		IntersectRayInternal(mapId, in ray, length, ref state, QueryCallback);
		return state.Result;
	}

	public void IntersectRay(List<RayCastResults> results, MapId mapId, in Ray ray, float maxLength)
	{
		results.Clear();
		QueryState state = new QueryState(maxLength, results);
		IntersectRayInternal(mapId, in ray, maxLength, ref state, QueryCallback);
	}

	public RayCastResults? IntersectRay<TState>(MapId mapId, in Ray ray, float length, TState predicateState, Func<Entity<TComp, TransformComponent>, TState, bool> ignore)
	{
		QueryState<TState> state = new QueryState<TState>(new QueryState(length), predicateState, ignore);
		IntersectRayInternal(mapId, in ray, length, ref state, PredicateQueryCallback);
		return state.Inner.Result;
	}

	public void IntersectRay<TState>(List<RayCastResults> results, MapId mapId, in Ray ray, float length, TState predicateState, Func<Entity<TComp, TransformComponent>, TState, bool> ignore)
	{
		QueryState<TState> state = new QueryState<TState>(new QueryState(length, results), predicateState, ignore);
		IntersectRayInternal(mapId, in ray, length, ref state, PredicateQueryCallback);
	}

	private void IntersectRayInternal<TState>(MapId mapId, in Ray ray, float maxLength, ref TState state, DynamicTree<ComponentTreeEntry<TComp>>.RayQueryCallbackDelegate<TState> callback) where TState : IDone
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (mapId == MapId.Nullspace || !CheckEnabled())
		{
			return;
		}
		Vector2 value = ray.Position + ray.Direction * maxLength;
		Unsafe.SkipInit(out Box2 worldAABB);
		((Box2)(ref worldAABB))._002Ector(Vector2.Min(ray.Position, value), Vector2.Max(ray.Position, value));
		foreach (var intersectingTree in GetIntersectingTrees(mapId, worldAABB))
		{
			EntityUid item = intersectingTree.Uid;
			TTreeComp item2 = intersectingTree.Comp;
			(Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 InvWorldMatrix) worldPositionRotationInvMatrix = XformSystem.GetWorldPositionRotationInvMatrix(item);
			Angle item3 = worldPositionRotationInvMatrix.WorldRotation;
			Matrix3x2 item4 = worldPositionRotationInvMatrix.InvWorldMatrix;
			Angle val = new Angle(0.0 - item3.Theta);
			Ray ray2 = new Ray(direction: ((Angle)(ref val)).RotateVec(ref ray.Direction), position: Vector2.Transform(ray.Position, item4));
			item2.Tree.QueryRay(ref state, callback, in ray2);
			if (state.Done)
			{
				break;
			}
		}
	}

	private static bool QueryCallback(ref QueryState state, in ComponentTreeEntry<TComp> value, in Vector2 point, float dist)
	{
		if (dist > state.MaxLength)
		{
			return true;
		}
		if (state.ReturnOnFirstHit)
		{
			state.Result = new RayCastResults(dist, point, value.Uid);
			return false;
		}
		state.List.Add(new RayCastResults(dist, point, value.Uid));
		return true;
	}

	private static bool PredicateQueryCallback<TState>(ref QueryState<TState> state, in ComponentTreeEntry<TComp> value, in Vector2 point, float dist)
	{
		if (dist > state.Inner.MaxLength)
		{
			return true;
		}
		if (state.Ignore(value, state.PredicateState))
		{
			return true;
		}
		if (state.Inner.ReturnOnFirstHit)
		{
			state.Inner.Result = new RayCastResults(dist, point, value.Uid);
			return false;
		}
		state.Inner.List.Add(new RayCastResults(dist, point, value.Uid));
		return true;
	}
}
