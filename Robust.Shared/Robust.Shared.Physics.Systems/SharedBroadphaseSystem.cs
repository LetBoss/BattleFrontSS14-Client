using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Physics.Dynamics.Contacts;
using Robust.Shared.Threading;

namespace Robust.Shared.Physics.Systems;

public abstract class SharedBroadphaseSystem : EntitySystem
{
	internal delegate void BroadphaseCallback(Entity<BroadphaseComponent> entity);

	internal delegate void BroadphaseCallback<TState>(Entity<BroadphaseComponent> entity, ref TState state);

	private record struct BroadphaseContactJob() : IParallelRobustJob, IParallelRangeRobustJob
	{
		public int BatchSize => 16;

		public SharedBroadphaseSystem System = null;

		public SharedTransformSystem TransformSys = null;

		public IMapManager MapManager = null;

		public EntityQuery<TransformComponent> XformQuery = default(EntityQuery<TransformComponent>);

		public readonly List<FixtureProxy> MoveBuffer = new List<FixtureProxy>();

		public List<(FixtureProxy, FixtureProxy, PairFlag)> Pairs = new List<(FixtureProxy, FixtureProxy, PairFlag)>(64);

		public float FrameTime = 0f;

		public void Execute(int index)
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			FixtureProxy fixtureProxy = MoveBuffer[index];
			EntityUid? entityUid = XformQuery.GetComponent(fixtureProxy.Entity).Broadphase?.Uid;
			Box2 val = Matrix3Helpers.TransformBox(TransformSys.GetWorldMatrix(entityUid.Value), ref fixtureProxy.AABB);
			EntityUid entityUid2 = XformQuery.GetComponent(fixtureProxy.Entity).MapUid ?? EntityUid.Invalid;
			float broadphaseExpand = System.GetBroadphaseExpand(fixtureProxy.Body, FrameTime);
			_ = fixtureProxy.Body;
			(SharedBroadphaseSystem, FixtureProxy, Box2, List<(FixtureProxy, FixtureProxy, PairFlag)>) state = (System, fixtureProxy, val, Pairs);
			MapManager.FindGridsIntersecting<(SharedBroadphaseSystem, FixtureProxy, Box2, List<(FixtureProxy, FixtureProxy, PairFlag)>)>(entityUid2, ((Box2)(ref val)).Enlarged(broadphaseExpand), ref state, delegate(EntityUid uid, MapGridComponent _, ref (SharedBroadphaseSystem system, FixtureProxy proxy, Box2 worldAABB, List<(FixtureProxy, FixtureProxy, PairFlag)> pairBuffer) tuple)
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				ref List<(FixtureProxy, FixtureProxy, PairFlag)> item = ref tuple.pairBuffer;
				tuple.system.FindPairs(tuple.proxy, tuple.worldAABB, uid, item);
				return true;
			}, approx: true, includeMap: false);
			System.FindPairs(fixtureProxy, val, entityUid2, Pairs);
		}

		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			builder.Append("System = ");
			builder.Append(System);
			builder.Append(", TransformSys = ");
			builder.Append(TransformSys);
			builder.Append(", MapManager = ");
			builder.Append(MapManager);
			builder.Append(", XformQuery = ");
			builder.Append(XformQuery.ToString());
			builder.Append(", MoveBuffer = ");
			builder.Append(MoveBuffer);
			builder.Append(", Pairs = ");
			builder.Append(Pairs);
			builder.Append(", FrameTime = ");
			builder.Append(FrameTime.ToString());
			builder.Append(", BatchSize = ");
			builder.Append(BatchSize.ToString());
			return true;
		}
	}

	[Flags]
	private enum PairFlag : byte
	{
		None = 0,
		Wake = 1,
		Grid = 2
	}

	[Robust.Shared.IoC.Dependency]
	private readonly IConfigurationManager _cfg;

	[Robust.Shared.IoC.Dependency]
	private readonly IMapManagerInternal _mapManager;

	[Robust.Shared.IoC.Dependency]
	private readonly IParallelManager _parallel;

	[Robust.Shared.IoC.Dependency]
	private readonly EntityLookupSystem _lookup;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedGridTraversalSystem _traversal;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedMapSystem _map;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedPhysicsSystem _physicsSystem;

	[Robust.Shared.IoC.Dependency]
	private readonly SharedTransformSystem _transform;

	private EntityQuery<BroadphaseComponent> _broadphaseQuery;

	private EntityQuery<FixturesComponent> _fixturesQuery;

	private EntityQuery<MapGridComponent> _gridQuery;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	private EntityQuery<TransformComponent> _xformQuery;

	private readonly HashSet<FixtureProxy> _gridMoveBuffer = new HashSet<FixtureProxy>();

	private float _frameTime;

	private BroadphaseContactJob _contactJob;

	public override void Initialize()
	{
		base.Initialize();
		_contactJob = new BroadphaseContactJob
		{
			MapManager = _mapManager,
			System = this,
			TransformSys = EntityManager.System<SharedTransformSystem>(),
			XformQuery = GetEntityQuery<TransformComponent>()
		};
		_broadphaseQuery = GetEntityQuery<BroadphaseComponent>();
		_fixturesQuery = GetEntityQuery<FixturesComponent>();
		_gridQuery = GetEntityQuery<MapGridComponent>();
		_physicsQuery = GetEntityQuery<PhysicsComponent>();
		_xformQuery = GetEntityQuery<TransformComponent>();
		base.UpdatesOutsidePrediction = true;
		base.UpdatesAfter.Add(typeof(SharedTransformSystem));
		base.Subs.CVar(_cfg, CVars.TargetMinimumTickrate, delegate(int val)
		{
			_frameTime = 1f / (float)val;
		}, invokeImmediately: true);
	}

	public void Rebuild(BroadphaseComponent component, bool fullBuild)
	{
		component.StaticTree.Rebuild(fullBuild);
		component.DynamicTree.Rebuild(fullBuild);
		component.SundriesTree._b2Tree.Rebuild(fullBuild);
		component.StaticSundriesTree._b2Tree.Rebuild(fullBuild);
	}

	public void RebuildBottomUp(BroadphaseComponent component)
	{
		component.StaticTree.RebuildBottomUp();
		component.DynamicTree.RebuildBottomUp();
		component.SundriesTree._b2Tree.RebuildBottomUp();
		component.StaticSundriesTree._b2Tree.RebuildBottomUp();
	}

	private void FindGridContacts(HashSet<EntityUid> movedGrids)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (movedGrids.Count == 0)
		{
			return;
		}
		HashSet<FixtureProxy> moveBuffer = _physicsSystem.MoveBuffer;
		_gridMoveBuffer.Clear();
		foreach (EntityUid movedGrid in movedGrids)
		{
			MapGridComponent component = _gridQuery.GetComponent(movedGrid);
			TransformComponent component2 = _xformQuery.GetComponent(movedGrid);
			if (_broadphaseQuery.TryComp(component2.MapUid, out BroadphaseComponent component3))
			{
				Matrix3x2 worldMatrix = _transform.GetWorldMatrix(component2);
				Box2 localAABB = component.LocalAABB;
				Box2 val = Matrix3Helpers.TransformBox(worldMatrix, ref localAABB);
				Box2 enlargedAABB = ((Box2)(ref val)).Enlarged(GetBroadphaseExpand(_physicsQuery.GetComponent(movedGrid), _frameTime));
				(HashSet<FixtureProxy>, HashSet<FixtureProxy>) state = (moveBuffer, _gridMoveBuffer);
				QueryMapBroadphase(component3.DynamicTree, ref state, enlargedAABB);
				QueryMapBroadphase(component3.StaticTree, ref state, enlargedAABB);
			}
		}
		foreach (FixtureProxy item in _gridMoveBuffer)
		{
			moveBuffer.Add(item);
			_traversal.CheckTraverse((Owner: item.Entity, Comp: _xformQuery.GetComponent(item.Entity)));
		}
	}

	private float GetBroadphaseExpand(PhysicsComponent body, float frameTime)
	{
		return body.LinearVelocity.Length() * 1.2f * frameTime;
	}

	private void QueryMapBroadphase(IBroadPhase broadPhase, ref (HashSet<FixtureProxy>, HashSet<FixtureProxy>) state, Box2 enlargedAABB)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		broadPhase.QueryAabb<(HashSet<FixtureProxy>, HashSet<FixtureProxy>)>(ref state, delegate(ref (HashSet<FixtureProxy> moveBuffer, HashSet<FixtureProxy> gridMoveBuffer) tuple, in FixtureProxy value)
		{
			if (tuple.moveBuffer.Contains(value))
			{
				return true;
			}
			tuple.gridMoveBuffer.Add(value);
			return true;
		}, enlargedAABB, approx: true);
	}

	internal void FindNewContacts()
	{
		_contactJob.FrameTime = _frameTime;
		_contactJob.Pairs.Clear();
		HashSet<FixtureProxy> moveBuffer = _physicsSystem.MoveBuffer;
		HashSet<EntityUid> movedGrids = _physicsSystem.MovedGrids;
		FindGridContacts(movedGrids);
		HandleGridCollisions(movedGrids);
		if (moveBuffer.Count == 0)
		{
			return;
		}
		_contactJob.MoveBuffer.Clear();
		foreach (FixtureProxy item2 in moveBuffer)
		{
			_contactJob.MoveBuffer.Add(item2);
		}
		int count = moveBuffer.Count;
		_parallel.ProcessNow(_contactJob, count);
		foreach (var pair in _contactJob.Pairs)
		{
			FixtureProxy proxyA = pair.Item1;
			FixtureProxy proxyB = pair.Item2;
			PairFlag item = pair.Item3;
			PhysicsComponent body = proxyB.Body;
			ContactFlags contactFlags = ContactFlags.None;
			if ((item & PairFlag.Wake) == PairFlag.Wake)
			{
				_physicsSystem.WakeBody(proxyA.Entity, force: true, null, proxyA.Body);
				_physicsSystem.WakeBody(proxyB.Entity, force: true, null, body);
			}
			if ((PairFlag.Grid & item) == PairFlag.Grid)
			{
				contactFlags |= ContactFlags.Grid;
			}
			_physicsSystem.AddPair(proxyA.FixtureId, proxyB.FixtureId, in proxyA, in proxyB, contactFlags);
		}
		moveBuffer.Clear();
		movedGrids.Clear();
	}

	private void HandleGridCollisions(HashSet<EntityUid> movedGrids)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid movedGrid in movedGrids)
		{
			MapGridComponent component = _gridQuery.GetComponent(movedGrid);
			TransformComponent component2 = _xformQuery.GetComponent(movedGrid);
			if (component2.MapID == MapId.Nullspace)
			{
				continue;
			}
			(Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) worldPositionRotationMatrixWithInv = _transform.GetWorldPositionRotationMatrixWithInv(component2);
			Vector2 item = worldPositionRotationMatrixWithInv.WorldPosition;
			Angle item2 = worldPositionRotationMatrixWithInv.WorldRotation;
			Matrix3x2 item3 = worldPositionRotationMatrixWithInv.WorldMatrix;
			Matrix3x2 item4 = worldPositionRotationMatrixWithInv.InvWorldMatrix;
			Box2Rotated val = new Box2Rotated(component.LocalAABB, item2);
			Box2 val2 = ((Box2Rotated)(ref val)).CalcBoundingBox();
			Box2 worldAABB = ((Box2)(ref val2)).Translated(item);
			FixturesComponent comp = _fixturesQuery.Comp(movedGrid);
			PhysicsComponent comp2 = _physicsQuery.Comp(movedGrid);
			Transform physicsTransform = _physicsSystem.GetPhysicsTransform(movedGrid);
			(Entity<FixturesComponent, MapGridComponent, PhysicsComponent, TransformComponent>, Transform, Matrix3x2, Matrix3x2, SharedMapSystem, SharedPhysicsSystem, SharedTransformSystem, EntityQuery<FixturesComponent>, EntityQuery<PhysicsComponent>, EntityQuery<TransformComponent>) state = (new Entity<FixturesComponent, MapGridComponent, PhysicsComponent, TransformComponent>(movedGrid, comp, component, comp2, component2), physicsTransform, item3, item4, _map, _physicsSystem, _transform, _fixturesQuery, _physicsQuery, _xformQuery);
			_mapManager.FindGridsIntersecting<(Entity<FixturesComponent, MapGridComponent, PhysicsComponent, TransformComponent>, Transform, Matrix3x2, Matrix3x2, SharedMapSystem, SharedPhysicsSystem, SharedTransformSystem, EntityQuery<FixturesComponent>, EntityQuery<PhysicsComponent>, EntityQuery<TransformComponent>)>(component2.MapID, worldAABB, ref state, delegate(EntityUid uid, MapGridComponent mapGridComponent, ref (Entity<FixturesComponent, MapGridComponent, PhysicsComponent, TransformComponent> grid, Transform transform, Matrix3x2 worldMatrix, Matrix3x2 invWorldMatrix, SharedMapSystem _map, SharedPhysicsSystem _physicsSystem, SharedTransformSystem xformSystem, EntityQuery<FixturesComponent> fixturesQuery, EntityQuery<PhysicsComponent> physicsQuery, EntityQuery<TransformComponent> xformQuery) tuple)
			{
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_012a: Unknown result type (might be due to invalid IL or missing references)
				//IL_012f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0135: Unknown result type (might be due to invalid IL or missing references)
				//IL_014a: Unknown result type (might be due to invalid IL or missing references)
				//IL_014f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0154: Unknown result type (might be due to invalid IL or missing references)
				//IL_0159: Unknown result type (might be due to invalid IL or missing references)
				//IL_015d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0162: Unknown result type (might be due to invalid IL or missing references)
				//IL_0167: Unknown result type (might be due to invalid IL or missing references)
				//IL_016c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0176: Unknown result type (might be due to invalid IL or missing references)
				//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
				//IL_01db: Unknown result type (might be due to invalid IL or missing references)
				//IL_022e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0233: Unknown result type (might be due to invalid IL or missing references)
				if (tuple.grid.Owner == uid || !tuple.xformQuery.TryGetComponent(uid, out TransformComponent component3))
				{
					return true;
				}
				if (tuple.grid.Owner.Id > uid.Id && tuple._physicsSystem.MovedGrids.Contains(uid))
				{
					return true;
				}
				(Vector2 WorldPosition, Angle WorldRotation, Matrix3x2 WorldMatrix, Matrix3x2 InvWorldMatrix) worldPositionRotationMatrixWithInv2 = tuple.xformSystem.GetWorldPositionRotationMatrixWithInv(component3);
				Matrix3x2 item5 = worldPositionRotationMatrixWithInv2.WorldMatrix;
				Matrix3x2 item6 = worldPositionRotationMatrixWithInv2.InvWorldMatrix;
				Box2 localAABB = mapGridComponent.LocalAABB;
				Box2 val3 = Matrix3Helpers.TransformBox(item5, ref localAABB);
				Transform physicsTransform2 = tuple._physicsSystem.GetPhysicsTransform(uid);
				localAABB = tuple.grid.Comp2.LocalAABB;
				Box2 val4 = Matrix3Helpers.TransformBox(tuple.invWorldMatrix, ref val3);
				Box2 localAABB2 = ((Box2)(ref localAABB)).Intersect(ref val4);
				ChunkEnumerator localMapChunks = tuple._map.GetLocalMapChunks(tuple.grid.Owner, tuple.grid, localAABB2);
				PhysicsComponent comp3 = tuple.grid.Comp3;
				PhysicsComponent component4 = tuple.physicsQuery.GetComponent(uid);
				FixturesComponent fixturesComponent = tuple.fixturesQuery.Comp(uid);
				MapChunk chunk;
				while (localMapChunks.MoveNext(out chunk))
				{
					Matrix3x2 item7 = tuple.worldMatrix;
					Box2i cachedBounds = chunk.CachedBounds;
					val4 = Box2i.op_Implicit(((Box2i)(ref cachedBounds)).Translated(chunk.Indices * (int)tuple.grid.Comp2.ChunkSize));
					Box2 val5 = Matrix3Helpers.TransformBox(item7, ref val4);
					Box2 localAABB3 = Matrix3Helpers.TransformBox(item6, ref val5);
					ChunkEnumerator localMapChunks2 = tuple._map.GetLocalMapChunks(uid, mapGridComponent, localAABB3);
					MapChunk chunk2;
					while (localMapChunks2.MoveNext(out chunk2))
					{
						foreach (string fixture3 in chunk.Fixtures)
						{
							Fixture fixture = tuple.grid.Comp1.Fixtures[fixture3];
							for (int i = 0; i < fixture.Shape.ChildCount; i++)
							{
								Box2 val6 = fixture.Shape.ComputeAABB(tuple.transform, i);
								foreach (string fixture4 in chunk2.Fixtures)
								{
									Fixture fixture2 = fixturesComponent.Fixtures[fixture4];
									if (fixture.Contacts.ContainsKey(fixture2))
									{
										break;
									}
									for (int j = 0; j < fixture2.Shape.ChildCount; j++)
									{
										Box2 val7 = fixture2.Shape.ComputeAABB(physicsTransform2, j);
										if (((Box2)(ref val6)).Intersects(ref val7))
										{
											tuple._physicsSystem.AddPair((Owner: tuple.grid.Owner, Comp1: tuple.grid.Comp3, Comp2: tuple.grid.Comp4), (Owner: uid, Comp1: component4, Comp2: component3), fixture3, fixture4, fixture, i, fixture2, j, comp3, component4, ContactFlags.Grid);
											break;
										}
									}
								}
							}
						}
					}
				}
				return true;
			}, approx: true, includeMap: false);
		}
	}

	private void FindPairs(FixtureProxy proxy, Box2 worldAABB, EntityUid broadphase, List<(FixtureProxy, FixtureProxy, PairFlag)> pairBuffer)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		if (proxy.Entity == broadphase || !_xformQuery.TryGetComponent(proxy.Entity, out TransformComponent component))
		{
			return;
		}
		if (!_lookup.TryGetCurrentBroadphase(component, out BroadphaseComponent broadphase2))
		{
			base.Log.Error($"Found null broadphase for {ToPrettyString(proxy.Entity)}");
			return;
		}
		Box2 aabb = ((!(broadphase2.Owner == broadphase)) ? Matrix3Helpers.TransformBox(_transform.GetInvWorldMatrix(broadphase), ref worldAABB) : proxy.AABB);
		BroadphaseComponent component2 = _broadphaseQuery.GetComponent(broadphase);
		(List<(FixtureProxy, FixtureProxy, PairFlag)>, HashSet<FixtureProxy>, SharedBroadphaseSystem, SharedPhysicsSystem, FixtureProxy) state = (pairBuffer, _physicsSystem.MoveBuffer, this, _physicsSystem, proxy);
		QueryBroadphase(component2.DynamicTree, state, aabb);
		if ((proxy.Body.BodyType & BodyType.Static) == 0)
		{
			QueryBroadphase(component2.StaticTree, state, aabb);
		}
	}

	private void QueryBroadphase(IBroadPhase broadPhase, (List<(FixtureProxy, FixtureProxy, PairFlag)>, HashSet<FixtureProxy> MoveBuffer, SharedBroadphaseSystem Broadphase, SharedPhysicsSystem PhysicsSystem, FixtureProxy) state, Box2 aabb)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		broadPhase.QueryAabb<(List<(FixtureProxy, FixtureProxy, PairFlag)>, HashSet<FixtureProxy>, SharedBroadphaseSystem, SharedPhysicsSystem, FixtureProxy)>(ref state, delegate(ref (List<(FixtureProxy, FixtureProxy, PairFlag)> pairs, HashSet<FixtureProxy> moveBuffer, SharedBroadphaseSystem broadphase, SharedPhysicsSystem physicsSystem, FixtureProxy proxy) tuple, in FixtureProxy other)
		{
			if (tuple.proxy.Entity == other.Entity || !SharedPhysicsSystem.ShouldCollide(tuple.proxy.Fixture, other.Fixture))
			{
				return true;
			}
			if (tuple.proxy.Entity.Id > other.Entity.Id && tuple.moveBuffer.Contains(other))
			{
				return true;
			}
			if (tuple.proxy.Fixture.Contacts.ContainsKey(other.Fixture))
			{
				return true;
			}
			if (!tuple.physicsSystem.ShouldCollideJoints(tuple.proxy.Entity, other.Entity))
			{
				return true;
			}
			PairFlag pairFlag = PairFlag.None;
			if (tuple.proxy.Fixture.Hard && other.Fixture.Hard && (tuple.broadphase._gridMoveBuffer.Contains(tuple.proxy) || tuple.broadphase._gridMoveBuffer.Contains(other)))
			{
				pairFlag |= PairFlag.Wake;
			}
			lock (tuple.pairs)
			{
				tuple.pairs.Add((tuple.proxy, other, pairFlag));
			}
			return true;
		}, aabb, approx: true);
	}

	[Obsolete("Use Entity<T> variant")]
	public void RegenerateContacts(EntityUid uid, PhysicsComponent body, FixturesComponent? fixtures = null, TransformComponent? xform = null)
	{
		RegenerateContacts((Owner: uid, Comp1: body, Comp2: fixtures, Comp3: xform));
	}

	public void RegenerateContacts(Entity<PhysicsComponent?, FixturesComponent?, TransformComponent?> entity)
	{
		if (!Resolve(entity.Owner, ref entity.Comp1))
		{
			return;
		}
		_physicsSystem.DestroyContacts(entity.Comp1);
		if (!Resolve(entity.Owner, ref entity.Comp2, ref entity.Comp3) || !entity.Comp3.MapUid.HasValue || !_xformQuery.TryGetComponent(entity.Comp3.Broadphase?.Uid, out TransformComponent _))
		{
			return;
		}
		_physicsSystem.SetAwake(entity, value: true);
		foreach (Fixture value in entity.Comp2.Fixtures.Values)
		{
			TouchProxies(value);
		}
	}

	internal void TouchProxies(Fixture fixture)
	{
		FixtureProxy[] proxies = fixture.Proxies;
		foreach (FixtureProxy proxy in proxies)
		{
			AddToMoveBuffer(proxy);
		}
	}

	private void AddToMoveBuffer(FixtureProxy proxy)
	{
		_physicsSystem.MoveBuffer.Add(proxy);
	}

	public void Refilter(EntityUid uid, Fixture fixture, TransformComponent? xform = null)
	{
		foreach (Contact value in fixture.Contacts.Values)
		{
			value.Flags |= ContactFlags.Filter;
		}
		if (Resolve(uid, ref xform) && xform.MapUid.HasValue && _xformQuery.TryGetComponent(xform.Broadphase?.Uid, out TransformComponent _))
		{
			TouchProxies(fixture);
		}
	}

	internal void GetBroadphases(MapId mapId, Box2 aabb, BroadphaseCallback callback)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		(BroadphaseCallback, EntityQuery<BroadphaseComponent>) state = (callback, _broadphaseQuery);
		if (!_map.TryGetMap(mapId, out var uid))
		{
			return;
		}
		if (_broadphaseQuery.TryGetComponent(uid.Value, out BroadphaseComponent component))
		{
			callback((Owner: uid.Value, Comp: component));
		}
		_mapManager.FindGridsIntersecting<(BroadphaseCallback, EntityQuery<BroadphaseComponent>)>(uid.Value, aabb, ref state, delegate(EntityUid entityUid, MapGridComponent _, ref (BroadphaseCallback callback, EntityQuery<BroadphaseComponent> _broadphaseQuery) tuple)
		{
			if (tuple._broadphaseQuery.TryComp(entityUid, out BroadphaseComponent component2))
			{
				tuple.callback((Owner: entityUid, Comp: component2));
			}
			return true;
		}, approx: true, includeMap: false);
	}

	internal void GetBroadphases<TState>(MapId mapId, Box2 aabb, ref TState state, BroadphaseCallback<TState> callback)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		(TState, BroadphaseCallback<TState>, EntityQuery<BroadphaseComponent>) state2 = (state, callback, _broadphaseQuery);
		if (!_map.TryGetMap(mapId, out var uid))
		{
			return;
		}
		if (_broadphaseQuery.TryGetComponent(uid.Value, out BroadphaseComponent component))
		{
			callback((Owner: uid.Value, Comp: component), ref state);
		}
		_mapManager.FindGridsIntersecting<(TState, BroadphaseCallback<TState>, EntityQuery<BroadphaseComponent>)>(uid.Value, aabb, ref state2, delegate(EntityUid entityUid, MapGridComponent _, ref (TState state, BroadphaseCallback<TState> callback, EntityQuery<BroadphaseComponent> _broadphaseQuery) reference)
		{
			if (reference._broadphaseQuery.TryComp(entityUid, out BroadphaseComponent component2))
			{
				reference.callback((Owner: entityUid, Comp: component2), ref reference.state);
			}
			return true;
		}, approx: true, includeMap: false);
		(state, _, _) = state2;
	}
}
