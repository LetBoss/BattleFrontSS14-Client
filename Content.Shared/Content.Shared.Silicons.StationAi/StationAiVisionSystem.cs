using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.StationAi;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Threading;

namespace Content.Shared.Silicons.StationAi;

public sealed class StationAiVisionSystem : EntitySystem
{
	private record struct SeedJob() : IRobustJob
	{
		public required StationAiVisionSystem System = null;

		public Entity<MapGridComponent> Grid = default(Entity<MapGridComponent>);

		public Box2 ExpandedBounds = default(Box2);

		public void Execute()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			System._lookup.GetLocalEntitiesIntersecting<StationAiVisionComponent>(Grid.Owner, ExpandedBounds, System._seeds, (LookupFlags)111);
		}
	}

	private record struct ViewJob() : IParallelRobustJob, IParallelRangeRobustJob
	{
		public int BatchSize => 1;

		public required IEntityManager EntManager = null;

		public required SharedMapSystem Maps = null;

		public required StationAiVisionSystem System = null;

		public Entity<MapGridComponent> Grid = default(Entity<MapGridComponent>);

		public List<Entity<StationAiVisionComponent>> Data = new List<Entity<StationAiVisionComponent>>();

		public required HashSet<Vector2i> VisibleTiles = null;

		public readonly List<Dictionary<Vector2i, int>> Vis1 = new List<Dictionary<Vector2i, int>>();

		public readonly List<Dictionary<Vector2i, int>> Vis2 = new List<Dictionary<Vector2i, int>>();

		public readonly List<HashSet<Vector2i>> SeedTiles = new List<HashSet<Vector2i>>();

		public readonly List<HashSet<Vector2i>> BoundaryTiles = new List<HashSet<Vector2i>>();

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
			//IL_027b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_0441: Unknown result type (might be due to invalid IL or missing references)
			//IL_0443: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_0465: Unknown result type (might be due to invalid IL or missing references)
			//IL_04da: Unknown result type (might be due to invalid IL or missing references)
			//IL_04df: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_051a: Unknown result type (might be due to invalid IL or missing references)
			Entity<StationAiVisionComponent> seed = Data[index];
			TransformComponent seedXform = EntManager.GetComponent<TransformComponent>(Entity<StationAiVisionComponent>.op_Implicit(seed));
			if (!seed.Comp.Occluded || System.FastPath)
			{
				IEnumerable<TileRef> squircles = Maps.GetLocalTilesIntersecting(Grid.Owner, Grid.Comp, new Circle(System._xforms.GetWorldPosition(seedXform), seed.Comp.Range), false, (Predicate<TileRef>)null);
				lock (VisibleTiles)
				{
					foreach (TileRef tile in squircles)
					{
						VisibleTiles.Add(tile.GridIndices);
					}
					return;
				}
			}
			float range = seed.Comp.Range;
			Dictionary<Vector2i, int> vis1 = Vis1[index];
			Dictionary<Vector2i, int> vis2 = Vis2[index];
			HashSet<Vector2i> seedTiles = SeedTiles[index];
			HashSet<Vector2i> boundary = BoundaryTiles[index];
			vis1.Clear();
			vis2.Clear();
			seedTiles.Clear();
			boundary.Clear();
			int maxDepthMax = 0;
			int sumDepthMax = 0;
			Vector2i eyePos = Maps.GetTileRef(Grid.Owner, Entity<MapGridComponent>.op_Implicit(Grid), seedXform.Coordinates).GridIndices;
			Vector2i tile2 = default(Vector2i);
			for (double x = Math.Floor((float)eyePos.X - range); x <= (double)((float)eyePos.X + range); x += 1.0)
			{
				for (double y = Math.Floor((float)eyePos.Y - range); y <= (double)((float)eyePos.Y + range); y += 1.0)
				{
					((Vector2i)(ref tile2))._002Ector((int)x, (int)y);
					Vector2i val = tile2 - eyePos;
					int xDelta = Math.Abs(val.X);
					int yDelta = Math.Abs(val.Y);
					int deltaSum = xDelta + yDelta;
					maxDepthMax = Math.Max(maxDepthMax, Math.Max(xDelta, yDelta));
					sumDepthMax = Math.Max(sumDepthMax, deltaSum);
					seedTiles.Add(tile2);
				}
			}
			for (int d = 0; d < maxDepthMax; d++)
			{
				foreach (Vector2i tile3 in seedTiles)
				{
					if (System.GetMaxDelta(tile3, eyePos) == d + 1 && System.CheckNeighborsVis(vis2, tile3, d))
					{
						vis2[tile3] = (System._opaque.Contains(tile3) ? (-1) : (d + 1));
					}
				}
			}
			for (int i = 0; i < sumDepthMax; i++)
			{
				foreach (Vector2i tile4 in seedTiles)
				{
					if (System.GetSumDelta(tile4, eyePos) == i + 1 && System.CheckNeighborsVis(vis1, tile4, i))
					{
						if (System._opaque.Contains(tile4))
						{
							vis1[tile4] = -1;
						}
						else if (vis2.GetValueOrDefault(tile4) != 0)
						{
							vis1[tile4] = i + 1;
						}
					}
				}
			}
			vis1[eyePos] = 1;
			foreach (Vector2i tile5 in seedTiles)
			{
				vis2[tile5] = vis1.GetValueOrDefault(tile5, 0);
			}
			foreach (Vector2i tile6 in seedTiles)
			{
				if (System._opaque.Contains(tile6) && vis1.GetValueOrDefault(tile6) == 0 && (System.IsCorner(seedTiles, System._opaque, vis1, tile6, Vector2i.UpRight) || System.IsCorner(seedTiles, System._opaque, vis1, tile6, Vector2i.UpLeft) || System.IsCorner(seedTiles, System._opaque, vis1, tile6, Vector2i.DownLeft) || System.IsCorner(seedTiles, System._opaque, vis1, tile6, Vector2i.DownRight)))
				{
					boundary.Add(tile6);
				}
			}
			foreach (Vector2i tile7 in boundary)
			{
				vis1[tile7] = -1;
			}
			foreach (Vector2i tile8 in seedTiles)
			{
				if (System._viewportTiles.Contains(tile8) && vis1.GetValueOrDefault(tile8, 0) != 0)
				{
					lock (VisibleTiles)
					{
						VisibleTiles.Add(tile8);
					}
				}
			}
		}
	}

	[Dependency]
	private IParallelManager _parallel;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private SharedMapSystem _maps;

	[Dependency]
	private SharedTransformSystem _xforms;

	[Dependency]
	private SharedPowerReceiverSystem _power;

	private SeedJob _seedJob;

	private ViewJob _job;

	private readonly HashSet<Entity<OccluderComponent>> _occluders = new HashSet<Entity<OccluderComponent>>();

	private readonly HashSet<Entity<StationAiVisionComponent>> _seeds = new HashSet<Entity<StationAiVisionComponent>>();

	private readonly HashSet<Vector2i> _viewportTiles = new HashSet<Vector2i>();

	private EntityQuery<OccluderComponent> _occluderQuery;

	private readonly HashSet<Vector2i> _singleTiles = new HashSet<Vector2i>();

	private readonly HashSet<Vector2i> _opaque = new HashSet<Vector2i>();

	private bool FastPath;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_occluderQuery = ((EntitySystem)this).GetEntityQuery<OccluderComponent>();
		_seedJob = new SeedJob
		{
			System = this
		};
		_job = new ViewJob
		{
			EntManager = (IEntityManager)(object)base.EntityManager,
			Maps = _maps,
			System = this,
			VisibleTiles = _singleTiles
		};
	}

	public bool IsAccessible(Entity<BroadphaseComponent, MapGridComponent> grid, Vector2i tile, float expansionSize = 8.5f, bool fastPath = false)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		_viewportTiles.Clear();
		_opaque.Clear();
		_seeds.Clear();
		_viewportTiles.Add(tile);
		Box2 localBounds = _lookup.GetLocalBounds(tile, grid.Comp2.TileSize);
		Box2 expandedBounds = ((Box2)(ref localBounds)).Enlarged(expansionSize);
		_seedJob.Grid = Entity<MapGridComponent>.op_Implicit((grid.Owner, grid.Comp2));
		_seedJob.ExpandedBounds = expandedBounds;
		_parallel.ProcessNow((IRobustJob)(object)_seedJob);
		_job.Data.Clear();
		FastPath = fastPath;
		foreach (Entity<StationAiVisionComponent> seed in _seeds)
		{
			if (seed.Comp.Enabled && (!seed.Comp.NeedsPower || _power.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(seed.Owner))) && (!seed.Comp.NeedsAnchoring || ((EntitySystem)this).Transform(seed.Owner).Anchored))
			{
				_job.Data.Add(seed);
			}
		}
		if (_seeds.Count == 0)
		{
			return false;
		}
		if (!fastPath)
		{
			TilesEnumerator tileEnumerator = _maps.GetLocalTilesEnumerator(Entity<BroadphaseComponent, MapGridComponent>.op_Implicit(grid), Entity<BroadphaseComponent, MapGridComponent>.op_Implicit(grid), expandedBounds, false, (Predicate<TileRef>)null);
			TileRef tileRef = default(TileRef);
			while (((TilesEnumerator)(ref tileEnumerator)).MoveNext(ref tileRef))
			{
				if (IsOccluded(grid, tileRef.GridIndices))
				{
					_opaque.Add(tileRef.GridIndices);
				}
			}
		}
		for (int i = _job.Vis1.Count; i < _job.Data.Count; i++)
		{
			_job.Vis1.Add(new Dictionary<Vector2i, int>());
			_job.Vis2.Add(new Dictionary<Vector2i, int>());
			_job.SeedTiles.Add(new HashSet<Vector2i>());
			_job.BoundaryTiles.Add(new HashSet<Vector2i>());
		}
		_singleTiles.Clear();
		_job.Grid = Entity<MapGridComponent>.op_Implicit((grid.Owner, grid.Comp2));
		_job.VisibleTiles = _singleTiles;
		_parallel.ProcessNow((IParallelRobustJob)(object)_job, _job.Data.Count);
		return _job.VisibleTiles.Contains(tile);
	}

	private bool IsOccluded(Entity<BroadphaseComponent, MapGridComponent> grid, Vector2i tile)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		Box2 localBounds = _lookup.GetLocalBounds(tile, grid.Comp2.TileSize);
		Box2 tileBounds = ((Box2)(ref localBounds)).Enlarged(-0.05f);
		_occluders.Clear();
		_lookup.GetLocalEntitiesIntersecting<OccluderComponent>(Entity<BroadphaseComponent>.op_Implicit((grid.Owner, grid.Comp1)), tileBounds, _occluders, _occluderQuery, (LookupFlags)5);
		bool anyOccluders = false;
		foreach (Entity<OccluderComponent> occluder in _occluders)
		{
			if (occluder.Comp.Enabled)
			{
				anyOccluders = true;
				break;
			}
		}
		return anyOccluders;
	}

	public void GetView(Entity<BroadphaseComponent, MapGridComponent> grid, Box2Rotated worldBounds, HashSet<Vector2i> visibleTiles, float expansionSize = 8.5f)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		_viewportTiles.Clear();
		_opaque.Clear();
		_seeds.Clear();
		_seedJob.Grid = Entity<MapGridComponent>.op_Implicit((grid.Owner, grid.Comp2));
		Matrix3x2 invWorldMatrix = _xforms.GetInvWorldMatrix(Entity<BroadphaseComponent, MapGridComponent>.op_Implicit(grid));
		Box2 localAabb = Matrix3Helpers.TransformBox(invWorldMatrix, ref worldBounds);
		Box2Rotated val = ((Box2Rotated)(ref worldBounds)).Enlarged(expansionSize);
		Box2 enlargedLocalAabb = Matrix3Helpers.TransformBox(invWorldMatrix, ref val);
		_seedJob.ExpandedBounds = enlargedLocalAabb;
		_parallel.ProcessNow((IRobustJob)(object)_seedJob);
		_job.Data.Clear();
		FastPath = false;
		foreach (Entity<StationAiVisionComponent> seed in _seeds)
		{
			if (seed.Comp.Enabled && (!seed.Comp.NeedsPower || _power.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(seed.Owner))) && (!seed.Comp.NeedsAnchoring || ((EntitySystem)this).Transform(seed.Owner).Anchored))
			{
				_job.Data.Add(seed);
			}
		}
		if (_seeds.Count == 0)
		{
			return;
		}
		TilesEnumerator tileEnumerator = _maps.GetLocalTilesEnumerator(Entity<BroadphaseComponent, MapGridComponent>.op_Implicit(grid), Entity<BroadphaseComponent, MapGridComponent>.op_Implicit(grid), localAabb, false, (Predicate<TileRef>)null);
		TileRef tileRef = default(TileRef);
		while (((TilesEnumerator)(ref tileEnumerator)).MoveNext(ref tileRef))
		{
			if (IsOccluded(grid, tileRef.GridIndices))
			{
				_opaque.Add(tileRef.GridIndices);
			}
			_viewportTiles.Add(tileRef.GridIndices);
		}
		tileEnumerator = _maps.GetLocalTilesEnumerator(Entity<BroadphaseComponent, MapGridComponent>.op_Implicit(grid), Entity<BroadphaseComponent, MapGridComponent>.op_Implicit(grid), enlargedLocalAabb, false, (Predicate<TileRef>)null);
		TileRef tileRef2 = default(TileRef);
		while (((TilesEnumerator)(ref tileEnumerator)).MoveNext(ref tileRef2))
		{
			if (!_viewportTiles.Contains(tileRef2.GridIndices) && IsOccluded(grid, tileRef2.GridIndices))
			{
				_opaque.Add(tileRef2.GridIndices);
			}
		}
		for (int i = _job.Vis1.Count; i < _job.Data.Count; i++)
		{
			_job.Vis1.Add(new Dictionary<Vector2i, int>());
			_job.Vis2.Add(new Dictionary<Vector2i, int>());
			_job.SeedTiles.Add(new HashSet<Vector2i>());
			_job.BoundaryTiles.Add(new HashSet<Vector2i>());
		}
		_job.Grid = Entity<MapGridComponent>.op_Implicit((grid.Owner, grid.Comp2));
		_job.VisibleTiles = visibleTiles;
		_parallel.ProcessNow((IParallelRobustJob)(object)_job, _job.Data.Count);
	}

	private int GetMaxDelta(Vector2i tile, Vector2i center)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Vector2i delta = tile - center;
		return Math.Max(Math.Abs(delta.X), Math.Abs(delta.Y));
	}

	private int GetSumDelta(Vector2i tile, Vector2i center)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Vector2i delta = tile - center;
		return Math.Abs(delta.X) + Math.Abs(delta.Y);
	}

	private bool CheckNeighborsVis(Dictionary<Vector2i, int> vis, Vector2i index, int d)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				if (x != 0 || y != 0)
				{
					Vector2i neighbor = index + new Vector2i(x, y);
					if (vis.GetValueOrDefault(neighbor) == d)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool IsCorner(HashSet<Vector2i> tiles, HashSet<Vector2i> blocked, Dictionary<Vector2i, int> vis1, Vector2i index, Vector2i delta)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		Vector2i diagonalIndex = index + delta;
		if (!tiles.TryGetValue(diagonalIndex, out var diagonal))
		{
			return false;
		}
		Vector2i cardinal1 = default(Vector2i);
		((Vector2i)(ref cardinal1))._002Ector(index.X, diagonal.Y);
		Vector2i cardinal2 = default(Vector2i);
		((Vector2i)(ref cardinal2))._002Ector(diagonal.X, index.Y);
		if (vis1.GetValueOrDefault(diagonal) != 0 && vis1.GetValueOrDefault(cardinal1) != 0 && vis1.GetValueOrDefault(cardinal2) != 0 && blocked.Contains(cardinal1) && blocked.Contains(cardinal2))
		{
			return !blocked.Contains(diagonal);
		}
		return false;
	}
}
