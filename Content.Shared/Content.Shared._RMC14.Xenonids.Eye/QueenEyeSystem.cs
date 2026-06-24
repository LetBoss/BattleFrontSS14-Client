using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Watch;
using Content.Shared.Coordinates;
using Content.Shared.Mind;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Prototypes;
using Robust.Shared.Threading;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Eye;

public sealed class QueenEyeSystem : EntitySystem
{
	private record struct SeedJob : IRobustJob
	{
		public required QueenEyeSystem System;

		public Entity<MapGridComponent> Grid;

		public Box2 ExpandedBounds;

		public void Execute()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			System._entityLookup.GetLocalEntitiesIntersecting<QueenEyeVisionComponent>(Grid.Owner, ExpandedBounds, System._seeds, (LookupFlags)111);
		}
	}

	private record struct ViewJob() : IParallelRobustJob, IParallelRangeRobustJob
	{
		public int BatchSize => 1;

		public required IEntityManager EntManager = null;

		public required SharedMapSystem Maps = null;

		public required QueenEyeSystem System = null;

		public Entity<MapGridComponent> Grid = default(Entity<MapGridComponent>);

		public List<Entity<QueenEyeVisionComponent>> Data = new List<Entity<QueenEyeVisionComponent>>();

		public required HashSet<Vector2i> VisibleTiles = null;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			Entity<QueenEyeVisionComponent> seed = Data[index];
			TransformComponent seedXform = EntManager.GetComponent<TransformComponent>(Entity<QueenEyeVisionComponent>.op_Implicit(seed));
			IEnumerable<TileRef> squircles = Maps.GetLocalTilesIntersecting(Grid.Owner, Grid.Comp, Box2.CenteredAround(System._transform.GetWorldPosition(seedXform), new Vector2(seed.Comp.Range)), false, (Predicate<TileRef>)null);
			lock (VisibleTiles)
			{
				foreach (TileRef tile in squircles)
				{
					VisibleTiles.Add(tile.GridIndices);
				}
			}
		}
	}

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedEyeSystem _eye;

	[Dependency]
	private SharedMapSystem _map;

	[Dependency]
	private SharedMindSystem _mind;

	[Dependency]
	private SharedMoverController _mover;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private IParallelManager _parallel;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedXenoWatchSystem _xenoWatch;

	private SeedJob _seedJob;

	private ViewJob _job;

	private readonly HashSet<Entity<QueenEyeVisionComponent>> _seeds = new HashSet<Entity<QueenEyeVisionComponent>>();

	private readonly HashSet<Vector2i> _singleTiles = new HashSet<Vector2i>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		_seedJob = new SeedJob
		{
			System = this
		};
		_job = new ViewJob
		{
			EntManager = (IEntityManager)(object)base.EntityManager,
			Maps = _map,
			System = this,
			VisibleTiles = _singleTiles
		};
		((EntitySystem)this).SubscribeLocalEvent<QueenEyeActionComponent, MapInitEvent>((EntityEventRefHandler<QueenEyeActionComponent, MapInitEvent>)OnQueenEyeActionMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<QueenEyeActionComponent, ComponentRemove>((EntityEventRefHandler<QueenEyeActionComponent, ComponentRemove>)OnQueenEyeActionRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<QueenEyeActionComponent, EntityTerminatingEvent>((EntityEventRefHandler<QueenEyeActionComponent, EntityTerminatingEvent>)OnQueenEyeActionTerminating, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<QueenEyeActionComponent, QueenEyeActionEvent>((EntityEventRefHandler<QueenEyeActionComponent, QueenEyeActionEvent>)OnQueenEyeAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<QueenEyeActionComponent, GetVisMaskEvent>((EntityEventRefHandler<QueenEyeActionComponent, GetVisMaskEvent>)OnQueenEyeActionGetVisMask, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<QueenEyeActionComponent, XenoWatchEvent>((EntityEventRefHandler<QueenEyeActionComponent, XenoWatchEvent>)OnQueenEyeActionWatch, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<QueenEyeActionComponent, XenoUnwatchEvent>((EntityEventRefHandler<QueenEyeActionComponent, XenoUnwatchEvent>)OnQueenEyeActionUnwatch, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<QueenEyeActionComponent, XenoOvipositorChangedEvent>((EntityEventRefHandler<QueenEyeActionComponent, XenoOvipositorChangedEvent>)OnQueenEyeOvipositorChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<QueenEyeComponent, XenoUnwatchEvent>((EntityEventRefHandler<QueenEyeComponent, XenoUnwatchEvent>)OnQueenEyeUnwatch, (Type[])null, (Type[])null);
	}

	private void OnQueenEyeActionMapInit(Entity<QueenEyeActionComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		_eye.RefreshVisibilityMask(Entity<EyeComponent>.op_Implicit(ent.Owner));
	}

	private void OnQueenEyeActionRemove(Entity<QueenEyeActionComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RemoveQueenEye(ent);
	}

	private void OnQueenEyeActionTerminating(Entity<QueenEyeActionComponent> ent, ref EntityTerminatingEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		RemoveQueenEye(ent);
	}

	private void OnQueenEyeAction(Entity<QueenEyeActionComponent> ent, ref QueenEyeActionEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		EyeComponent eye = default(EyeComponent);
		if (!RemoveQueenEye(ent) && !_net.IsClient && ((EntitySystem)this).TryComp<EyeComponent>(Entity<QueenEyeActionComponent>.op_Implicit(ent), ref eye))
		{
			ent.Comp.Eye = ((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(ent.Comp.Spawn), ent.Owner.ToCoordinates(), (ComponentRegistry)null);
			((EntitySystem)this).Dirty<QueenEyeActionComponent>(ent, (MetaDataComponent)null);
			QueenEyeComponent eyeComp = ((EntitySystem)this).EnsureComp<QueenEyeComponent>(ent.Comp.Eye.Value);
			eyeComp.Queen = Entity<QueenEyeActionComponent>.op_Implicit(ent);
			((EntitySystem)this).Dirty(ent.Comp.Eye.Value, (IComponent)(object)eyeComp, (MetaDataComponent)null);
			_eye.SetPvsScale(Entity<EyeComponent>.op_Implicit((Entity<QueenEyeActionComponent>.op_Implicit(ent), eye)), ent.Comp.EyePvsScale);
			_eye.SetTarget(Entity<QueenEyeActionComponent>.op_Implicit(ent), ent.Comp.Eye, eye);
			_eye.SetDrawFov(Entity<QueenEyeActionComponent>.op_Implicit(ent), false, (EyeComponent)null);
			_mover.SetRelay(Entity<QueenEyeActionComponent>.op_Implicit(ent), ent.Comp.Eye.Value);
		}
	}

	private void OnQueenEyeActionGetVisMask(Entity<QueenEyeActionComponent> ent, ref GetVisMaskEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (_mind.TryGetMind(ent.Owner, out EntityUid _, out MindComponent mind) && ((EntitySystem)this).HasComp<QueenEyeComponent>(mind.VisitingEntity))
		{
			args.VisibilityMask |= (int)ent.Comp.Visibility;
		}
	}

	private void OnQueenEyeActionWatch(Entity<QueenEyeActionComponent> ent, ref XenoWatchEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? eye = ent.Comp.Eye;
		if (eye.HasValue)
		{
			EntityUid eye2 = eye.GetValueOrDefault();
			_xenoWatch.SetWatching(Entity<XenoWatchingComponent>.op_Implicit(eye2), args.Watching);
		}
	}

	private void OnQueenEyeActionUnwatch(Entity<QueenEyeActionComponent> ent, ref XenoUnwatchEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? eye = ent.Comp.Eye;
		if (eye.HasValue)
		{
			EntityUid eye2 = eye.GetValueOrDefault();
			((EntitySystem)this).RemCompDeferred<XenoWatchingComponent>(eye2);
		}
	}

	private void OnQueenEyeOvipositorChanged(Entity<QueenEyeActionComponent> ent, ref XenoOvipositorChangedEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && !args.Attached)
		{
			RemoveQueenEye(ent);
		}
	}

	private void OnQueenEyeUnwatch(Entity<QueenEyeComponent> ent, ref XenoUnwatchEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? queen = ent.Comp.Queen;
		if (queen.HasValue)
		{
			EntityUid queen2 = queen.GetValueOrDefault();
			_eye.SetTarget(queen2, (EntityUid?)Entity<QueenEyeComponent>.op_Implicit(ent), (EyeComponent)null);
		}
	}

	public void GetView(Entity<BroadphaseComponent, MapGridComponent> grid, Box2Rotated worldBounds, HashSet<Vector2i> visibleTiles, float expansionSize = 29f)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		_seeds.Clear();
		_seedJob.Grid = Entity<MapGridComponent>.op_Implicit((grid.Owner, grid.Comp2));
		Matrix3x2 invWorldMatrix = _transform.GetInvWorldMatrix(Entity<BroadphaseComponent, MapGridComponent>.op_Implicit(grid));
		Box2Rotated val = ((Box2Rotated)(ref worldBounds)).Enlarged(expansionSize);
		Box2 enlargedLocalAabb = Matrix3Helpers.TransformBox(invWorldMatrix, ref val);
		_seedJob.ExpandedBounds = enlargedLocalAabb;
		_parallel.ProcessNow((IRobustJob)(object)_seedJob);
		_job.Data.Clear();
		foreach (Entity<QueenEyeVisionComponent> seed in _seeds)
		{
			_job.Data.Add(seed);
		}
		if (_seeds.Count != 0)
		{
			_job.Grid = Entity<MapGridComponent>.op_Implicit((grid.Owner, grid.Comp2));
			_job.VisibleTiles = visibleTiles;
			_parallel.ProcessNow((IParallelRobustJob)(object)_job, _job.Data.Count);
		}
	}

	private bool IsAccessible(Entity<BroadphaseComponent, MapGridComponent> grid, Vector2i tile, float expansionSize = 29f)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		_seeds.Clear();
		Box2 localBounds = _entityLookup.GetLocalBounds(tile, grid.Comp2.TileSize);
		Box2 expandedBounds = ((Box2)(ref localBounds)).Enlarged(expansionSize);
		_seedJob.Grid = Entity<MapGridComponent>.op_Implicit((grid.Owner, grid.Comp2));
		_seedJob.ExpandedBounds = expandedBounds;
		_parallel.ProcessNow((IRobustJob)(object)_seedJob);
		_job.Data.Clear();
		foreach (Entity<QueenEyeVisionComponent> seed in _seeds)
		{
			_job.Data.Add(seed);
		}
		if (_seeds.Count == 0)
		{
			return false;
		}
		_singleTiles.Clear();
		_job.Grid = Entity<MapGridComponent>.op_Implicit((grid.Owner, grid.Comp2));
		_job.VisibleTiles = _singleTiles;
		_parallel.ProcessNow((IParallelRobustJob)(object)_job, _job.Data.Count);
		return _job.VisibleTiles.Contains(tile);
	}

	private bool RemoveQueenEye(Entity<QueenEyeActionComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.Eye.HasValue)
		{
			return false;
		}
		_eye.SetTarget(Entity<QueenEyeActionComponent>.op_Implicit(ent), (EntityUid?)null, (EyeComponent)null);
		_eye.SetPvsScale(Entity<EyeComponent>.op_Implicit(ent.Owner), ent.Comp.PvsScale);
		_eye.SetDrawFov(Entity<QueenEyeActionComponent>.op_Implicit(ent), true, (EyeComponent)null);
		ent.Comp.Eye = null;
		((EntitySystem)this).Dirty<QueenEyeActionComponent>(ent, (MetaDataComponent)null);
		if (_net.IsServer && ((EntitySystem)this).HasComp<QueenEyeComponent>(ent.Comp.Eye))
		{
			((EntitySystem)this).QueueDel(ent.Comp.Eye);
		}
		((EntitySystem)this).RemComp<RelayInputMoverComponent>(Entity<QueenEyeActionComponent>.op_Implicit(ent));
		QueenEyeActionUpdated ev = new QueenEyeActionUpdated(ent);
		((EntitySystem)this).RaiseLocalEvent<QueenEyeActionUpdated>(Entity<QueenEyeActionComponent>.op_Implicit(ent), ref ev, false);
		return true;
	}

	public bool IsInQueenEye(Entity<QueenEyeActionComponent?> queen)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<QueenEyeActionComponent>(Entity<QueenEyeActionComponent>.op_Implicit(queen), ref queen.Comp, false))
		{
			return queen.Comp.Eye.HasValue;
		}
		return false;
	}

	public bool CanSeeTarget(Entity<QueenEyeActionComponent?> queen, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<QueenEyeActionComponent>(Entity<QueenEyeActionComponent>.op_Implicit(queen), ref queen.Comp, false) || !queen.Comp.Eye.HasValue)
		{
			return false;
		}
		TransformComponent targetTransform = ((EntitySystem)this).Transform(target);
		BroadphaseComponent broadphase = default(BroadphaseComponent);
		MapGridComponent grid = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<BroadphaseComponent>(targetTransform.GridUid, ref broadphase) || !((EntitySystem)this).TryComp<MapGridComponent>(targetTransform.GridUid, ref grid))
		{
			return false;
		}
		Vector2i targetTile = _map.LocalToTile(targetTransform.GridUid.Value, grid, targetTransform.Coordinates);
		return IsAccessible(Entity<BroadphaseComponent, MapGridComponent>.op_Implicit((targetTransform.GridUid.Value, broadphase, grid)), targetTile);
	}

	public bool CanSeeTarget(Entity<QueenEyeActionComponent?> queen, EntityCoordinates target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<QueenEyeActionComponent>(Entity<QueenEyeActionComponent>.op_Implicit(queen), ref queen.Comp, false) || !queen.Comp.Eye.HasValue)
		{
			return false;
		}
		EntityUid? grid = _transform.GetGrid(target);
		if (grid.HasValue)
		{
			EntityUid gridId = grid.GetValueOrDefault();
			BroadphaseComponent broadphase = default(BroadphaseComponent);
			MapGridComponent grid2 = default(MapGridComponent);
			if (((EntitySystem)this).TryComp<BroadphaseComponent>(gridId, ref broadphase) && ((EntitySystem)this).TryComp<MapGridComponent>(gridId, ref grid2))
			{
				Vector2i targetTile = _map.CoordinatesToTile(gridId, grid2, target);
				return IsAccessible(Entity<BroadphaseComponent, MapGridComponent>.op_Implicit((gridId, broadphase, grid2)), targetTile);
			}
		}
		return false;
	}
}
