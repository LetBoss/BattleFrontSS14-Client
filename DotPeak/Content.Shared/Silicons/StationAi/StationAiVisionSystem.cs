// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.StationAi.StationAiVisionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.StationAi;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Threading;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Silicons.StationAi;

public sealed class StationAiVisionSystem : EntitySystem
{
  [Robust.Shared.IoC.Dependency]
  private IParallelManager _parallel;
  [Robust.Shared.IoC.Dependency]
  private EntityLookupSystem _lookup;
  [Robust.Shared.IoC.Dependency]
  private SharedMapSystem _maps;
  [Robust.Shared.IoC.Dependency]
  private SharedTransformSystem _xforms;
  [Robust.Shared.IoC.Dependency]
  private SharedPowerReceiverSystem _power;
  private StationAiVisionSystem.SeedJob _seedJob;
  private StationAiVisionSystem.ViewJob _job;
  private readonly HashSet<Entity<OccluderComponent>> _occluders = new HashSet<Entity<OccluderComponent>>();
  private readonly HashSet<Entity<StationAiVisionComponent>> _seeds = new HashSet<Entity<StationAiVisionComponent>>();
  private readonly HashSet<Vector2i> _viewportTiles = new HashSet<Vector2i>();
  private Robust.Shared.GameObjects.EntityQuery<OccluderComponent> _occluderQuery;
  private readonly HashSet<Vector2i> _singleTiles = new HashSet<Vector2i>();
  private readonly HashSet<Vector2i> _opaque = new HashSet<Vector2i>();
  private bool FastPath;

  public override void Initialize()
  {
    base.Initialize();
    this._occluderQuery = this.GetEntityQuery<OccluderComponent>();
    this._seedJob = new StationAiVisionSystem.SeedJob()
    {
      System = this
    };
    this._job = new StationAiVisionSystem.ViewJob()
    {
      EntManager = (IEntityManager) this.EntityManager,
      Maps = this._maps,
      System = this,
      VisibleTiles = this._singleTiles
    };
  }

  public bool IsAccessible(
    Entity<BroadphaseComponent, MapGridComponent> grid,
    Vector2i tile,
    float expansionSize = 8.5f,
    bool fastPath = false)
  {
    this._viewportTiles.Clear();
    this._opaque.Clear();
    this._seeds.Clear();
    this._viewportTiles.Add(tile);
    Box2 localBounds = this._lookup.GetLocalBounds(tile, grid.Comp2.TileSize);
    Box2 aabb = ((Box2) ref localBounds).Enlarged(expansionSize);
    this._seedJob.Grid = (Entity<MapGridComponent>) (grid.Owner, grid.Comp2);
    this._seedJob.ExpandedBounds = aabb;
    this._parallel.ProcessNow((IRobustJob) this._seedJob);
    this._job.Data.Clear();
    this.FastPath = fastPath;
    foreach (Entity<StationAiVisionComponent> seed in this._seeds)
    {
      if (seed.Comp.Enabled && (!seed.Comp.NeedsPower || this._power.IsPowered((Entity<SharedApcPowerReceiverComponent>) seed.Owner)) && (!seed.Comp.NeedsAnchoring || this.Transform(seed.Owner).Anchored))
        this._job.Data.Add(seed);
    }
    if (this._seeds.Count == 0)
      return false;
    if (!fastPath)
    {
      SharedMapSystem.TilesEnumerator localTilesEnumerator = this._maps.GetLocalTilesEnumerator((EntityUid) grid, (MapGridComponent) grid, aabb, false);
      TileRef tile1;
      while (localTilesEnumerator.MoveNext(out tile1))
      {
        if (this.IsOccluded(grid, tile1.GridIndices))
          this._opaque.Add(tile1.GridIndices);
      }
    }
    for (int count = this._job.Vis1.Count; count < this._job.Data.Count; ++count)
    {
      this._job.Vis1.Add(new Dictionary<Vector2i, int>());
      this._job.Vis2.Add(new Dictionary<Vector2i, int>());
      this._job.SeedTiles.Add(new HashSet<Vector2i>());
      this._job.BoundaryTiles.Add(new HashSet<Vector2i>());
    }
    this._singleTiles.Clear();
    this._job.Grid = (Entity<MapGridComponent>) (grid.Owner, grid.Comp2);
    this._job.VisibleTiles = this._singleTiles;
    this._parallel.ProcessNow((IParallelRobustJob) this._job, this._job.Data.Count);
    return this._job.VisibleTiles.Contains(tile);
  }

  private bool IsOccluded(Entity<BroadphaseComponent, MapGridComponent> grid, Vector2i tile)
  {
    Box2 localBounds = this._lookup.GetLocalBounds(tile, grid.Comp2.TileSize);
    Box2 localAABB = ((Box2) ref localBounds).Enlarged(-0.05f);
    this._occluders.Clear();
    this._lookup.GetLocalEntitiesIntersecting<OccluderComponent>((Entity<BroadphaseComponent>) (grid.Owner, grid.Comp1), localAABB, this._occluders, this._occluderQuery, LookupFlags.Approximate | LookupFlags.Static);
    bool flag = false;
    foreach (Entity<OccluderComponent> occluder in this._occluders)
    {
      if (occluder.Comp.Enabled)
      {
        flag = true;
        break;
      }
    }
    return flag;
  }

  public void GetView(
    Entity<BroadphaseComponent, MapGridComponent> grid,
    Box2Rotated worldBounds,
    HashSet<Vector2i> visibleTiles,
    float expansionSize = 8.5f)
  {
    this._viewportTiles.Clear();
    this._opaque.Clear();
    this._seeds.Clear();
    this._seedJob.Grid = (Entity<MapGridComponent>) (grid.Owner, grid.Comp2);
    Matrix3x2 invWorldMatrix = this._xforms.GetInvWorldMatrix((EntityUid) grid);
    Box2 aabb1 = Matrix3Helpers.TransformBox(invWorldMatrix, ref worldBounds);
    Box2Rotated box2Rotated = ((Box2Rotated) ref worldBounds).Enlarged(expansionSize);
    Box2 aabb2 = Matrix3Helpers.TransformBox(invWorldMatrix, ref box2Rotated);
    this._seedJob.ExpandedBounds = aabb2;
    this._parallel.ProcessNow((IRobustJob) this._seedJob);
    this._job.Data.Clear();
    this.FastPath = false;
    foreach (Entity<StationAiVisionComponent> seed in this._seeds)
    {
      if (seed.Comp.Enabled && (!seed.Comp.NeedsPower || this._power.IsPowered((Entity<SharedApcPowerReceiverComponent>) seed.Owner)) && (!seed.Comp.NeedsAnchoring || this.Transform(seed.Owner).Anchored))
        this._job.Data.Add(seed);
    }
    if (this._seeds.Count == 0)
      return;
    SharedMapSystem.TilesEnumerator localTilesEnumerator1 = this._maps.GetLocalTilesEnumerator((EntityUid) grid, (MapGridComponent) grid, aabb1, false);
    TileRef tile1;
    while (localTilesEnumerator1.MoveNext(out tile1))
    {
      if (this.IsOccluded(grid, tile1.GridIndices))
        this._opaque.Add(tile1.GridIndices);
      this._viewportTiles.Add(tile1.GridIndices);
    }
    SharedMapSystem.TilesEnumerator localTilesEnumerator2 = this._maps.GetLocalTilesEnumerator((EntityUid) grid, (MapGridComponent) grid, aabb2, false);
    TileRef tile2;
    while (localTilesEnumerator2.MoveNext(out tile2))
    {
      if (!this._viewportTiles.Contains(tile2.GridIndices) && this.IsOccluded(grid, tile2.GridIndices))
        this._opaque.Add(tile2.GridIndices);
    }
    for (int count = this._job.Vis1.Count; count < this._job.Data.Count; ++count)
    {
      this._job.Vis1.Add(new Dictionary<Vector2i, int>());
      this._job.Vis2.Add(new Dictionary<Vector2i, int>());
      this._job.SeedTiles.Add(new HashSet<Vector2i>());
      this._job.BoundaryTiles.Add(new HashSet<Vector2i>());
    }
    this._job.Grid = (Entity<MapGridComponent>) (grid.Owner, grid.Comp2);
    this._job.VisibleTiles = visibleTiles;
    this._parallel.ProcessNow((IParallelRobustJob) this._job, this._job.Data.Count);
  }

  private int GetMaxDelta(Vector2i tile, Vector2i center)
  {
    Vector2i vector2i = Vector2i.op_Subtraction(tile, center);
    return Math.Max(Math.Abs(vector2i.X), Math.Abs(vector2i.Y));
  }

  private int GetSumDelta(Vector2i tile, Vector2i center)
  {
    Vector2i vector2i = Vector2i.op_Subtraction(tile, center);
    return Math.Abs(vector2i.X) + Math.Abs(vector2i.Y);
  }

  private bool CheckNeighborsVis(Dictionary<Vector2i, int> vis, Vector2i index, int d)
  {
    for (int index1 = -1; index1 <= 1; ++index1)
    {
      for (int index2 = -1; index2 <= 1; ++index2)
      {
        if (index1 != 0 || index2 != 0)
        {
          Vector2i key = Vector2i.op_Addition(index, new Vector2i(index1, index2));
          if (vis.GetValueOrDefault<Vector2i, int>(key) == d)
            return true;
        }
      }
    }
    return false;
  }

  private bool IsCorner(
    HashSet<Vector2i> tiles,
    HashSet<Vector2i> blocked,
    Dictionary<Vector2i, int> vis1,
    Vector2i index,
    Vector2i delta)
  {
    Vector2i equalValue = Vector2i.op_Addition(index, delta);
    Vector2i actualValue;
    if (!tiles.TryGetValue(equalValue, out actualValue))
      return false;
    Vector2i key1;
    // ISSUE: explicit constructor call
    ((Vector2i) ref key1).\u002Ector(index.X, actualValue.Y);
    Vector2i key2;
    // ISSUE: explicit constructor call
    ((Vector2i) ref key2).\u002Ector(actualValue.X, index.Y);
    return vis1.GetValueOrDefault<Vector2i, int>(actualValue) != 0 && vis1.GetValueOrDefault<Vector2i, int>(key1) != 0 && vis1.GetValueOrDefault<Vector2i, int>(key2) != 0 && blocked.Contains(key1) && blocked.Contains(key2) && !blocked.Contains(actualValue);
  }

  private record struct SeedJob : IRobustJob
  {
    public required StationAiVisionSystem System;
    public Entity<MapGridComponent> Grid;
    public Box2 ExpandedBounds;

    public SeedJob()
    {
      this.System = (StationAiVisionSystem) null;
      this.Grid = new Entity<MapGridComponent>();
      this.ExpandedBounds = new Box2();
    }

    public void Execute()
    {
      this.System._lookup.GetLocalEntitiesIntersecting<StationAiVisionComponent>(this.Grid.Owner, this.ExpandedBounds, this.System._seeds, LookupFlags.All | LookupFlags.Approximate);
    }

    [CompilerGenerated]
    public override readonly int GetHashCode()
    {
      return (EqualityComparer<StationAiVisionSystem>.Default.GetHashCode(this.System) * -1521134295 + EqualityComparer<Entity<MapGridComponent>>.Default.GetHashCode(this.Grid)) * -1521134295 + EqualityComparer<Box2>.Default.GetHashCode(this.ExpandedBounds);
    }

    [CompilerGenerated]
    public readonly bool Equals(StationAiVisionSystem.SeedJob other)
    {
      return EqualityComparer<StationAiVisionSystem>.Default.Equals(this.System, other.System) && EqualityComparer<Entity<MapGridComponent>>.Default.Equals(this.Grid, other.Grid) && EqualityComparer<Box2>.Default.Equals(this.ExpandedBounds, other.ExpandedBounds);
    }
  }

  private record struct ViewJob : IParallelRobustJob, IParallelRangeRobustJob
  {
    public required IEntityManager EntManager;
    public required SharedMapSystem Maps;
    public required StationAiVisionSystem System;
    public Entity<MapGridComponent> Grid;
    public List<Entity<StationAiVisionComponent>> Data;
    public required HashSet<Vector2i> VisibleTiles;
    public readonly List<Dictionary<Vector2i, int>> Vis1;
    public readonly List<Dictionary<Vector2i, int>> Vis2;
    public readonly List<HashSet<Vector2i>> SeedTiles;
    public readonly List<HashSet<Vector2i>> BoundaryTiles;

    public ViewJob()
    {
      this.EntManager = (IEntityManager) null;
      this.Maps = (SharedMapSystem) null;
      this.System = (StationAiVisionSystem) null;
      this.Grid = new Entity<MapGridComponent>();
      this.VisibleTiles = (HashSet<Vector2i>) null;
      this.Data = new List<Entity<StationAiVisionComponent>>();
      this.Vis1 = new List<Dictionary<Vector2i, int>>();
      this.Vis2 = new List<Dictionary<Vector2i, int>>();
      this.SeedTiles = new List<HashSet<Vector2i>>();
      this.BoundaryTiles = new List<HashSet<Vector2i>>();
    }

    public int BatchSize => 1;

    public void Execute(int index)
    {
      Entity<StationAiVisionComponent> uid = this.Data[index];
      TransformComponent component = this.EntManager.GetComponent<TransformComponent>((EntityUid) uid);
      if (!uid.Comp.Occluded || this.System.FastPath)
      {
        IEnumerable<TileRef> tilesIntersecting = this.Maps.GetLocalTilesIntersecting(this.Grid.Owner, this.Grid.Comp, new Circle(this.System._xforms.GetWorldPosition(component), uid.Comp.Range), false);
        lock (this.VisibleTiles)
        {
          foreach (TileRef tileRef in tilesIntersecting)
            this.VisibleTiles.Add(tileRef.GridIndices);
        }
      }
      else
      {
        float range = uid.Comp.Range;
        Dictionary<Vector2i, int> dictionary1 = this.Vis1[index];
        Dictionary<Vector2i, int> dictionary2 = this.Vis2[index];
        HashSet<Vector2i> seedTile = this.SeedTiles[index];
        HashSet<Vector2i> boundaryTile = this.BoundaryTiles[index];
        dictionary1.Clear();
        dictionary2.Clear();
        seedTile.Clear();
        boundaryTile.Clear();
        int val1_1 = 0;
        int val1_2 = 0;
        Vector2i gridIndices = this.Maps.GetTileRef(this.Grid.Owner, (MapGridComponent) this.Grid, component.Coordinates).GridIndices;
        for (double num1 = Math.Floor((double) gridIndices.X - (double) range); num1 <= (double) gridIndices.X + (double) range; ++num1)
        {
          for (double num2 = Math.Floor((double) gridIndices.Y - (double) range); num2 <= (double) gridIndices.Y + (double) range; ++num2)
          {
            Vector2i vector2i1;
            // ISSUE: explicit constructor call
            ((Vector2i) ref vector2i1).\u002Ector((int) num1, (int) num2);
            Vector2i vector2i2 = Vector2i.op_Subtraction(vector2i1, gridIndices);
            int val1_3 = Math.Abs(vector2i2.X);
            int val2_1 = Math.Abs(vector2i2.Y);
            int val2_2 = val1_3 + val2_1;
            val1_1 = Math.Max(val1_1, Math.Max(val1_3, val2_1));
            val1_2 = Math.Max(val1_2, val2_2);
            seedTile.Add(vector2i1);
          }
        }
        for (int d = 0; d < val1_1; ++d)
        {
          foreach (Vector2i vector2i in seedTile)
          {
            if (this.System.GetMaxDelta(vector2i, gridIndices) == d + 1 && this.System.CheckNeighborsVis(dictionary2, vector2i, d))
              dictionary2[vector2i] = this.System._opaque.Contains(vector2i) ? -1 : d + 1;
          }
        }
        for (int d = 0; d < val1_2; ++d)
        {
          foreach (Vector2i vector2i in seedTile)
          {
            if (this.System.GetSumDelta(vector2i, gridIndices) == d + 1 && this.System.CheckNeighborsVis(dictionary1, vector2i, d))
            {
              if (this.System._opaque.Contains(vector2i))
                dictionary1[vector2i] = -1;
              else if (dictionary2.GetValueOrDefault<Vector2i, int>(vector2i) != 0)
                dictionary1[vector2i] = d + 1;
            }
          }
        }
        dictionary1[gridIndices] = 1;
        foreach (Vector2i key in seedTile)
          dictionary2[key] = dictionary1.GetValueOrDefault<Vector2i, int>(key, 0);
        foreach (Vector2i vector2i in seedTile)
        {
          if (this.System._opaque.Contains(vector2i) && dictionary1.GetValueOrDefault<Vector2i, int>(vector2i) == 0 && (this.System.IsCorner(seedTile, this.System._opaque, dictionary1, vector2i, Vector2i.UpRight) || this.System.IsCorner(seedTile, this.System._opaque, dictionary1, vector2i, Vector2i.UpLeft) || this.System.IsCorner(seedTile, this.System._opaque, dictionary1, vector2i, Vector2i.DownLeft) || this.System.IsCorner(seedTile, this.System._opaque, dictionary1, vector2i, Vector2i.DownRight)))
            boundaryTile.Add(vector2i);
        }
        foreach (Vector2i key in boundaryTile)
          dictionary1[key] = -1;
        foreach (Vector2i key in seedTile)
        {
          if (this.System._viewportTiles.Contains(key) && dictionary1.GetValueOrDefault<Vector2i, int>(key, 0) != 0)
          {
            lock (this.VisibleTiles)
              this.VisibleTiles.Add(key);
          }
        }
      }
    }

    [CompilerGenerated]
    public override readonly int GetHashCode()
    {
      return ((((((((EqualityComparer<IEntityManager>.Default.GetHashCode(this.EntManager) * -1521134295 + EqualityComparer<SharedMapSystem>.Default.GetHashCode(this.Maps)) * -1521134295 + EqualityComparer<StationAiVisionSystem>.Default.GetHashCode(this.System)) * -1521134295 + EqualityComparer<Entity<MapGridComponent>>.Default.GetHashCode(this.Grid)) * -1521134295 + EqualityComparer<List<Entity<StationAiVisionComponent>>>.Default.GetHashCode(this.Data)) * -1521134295 + EqualityComparer<HashSet<Vector2i>>.Default.GetHashCode(this.VisibleTiles)) * -1521134295 + EqualityComparer<List<Dictionary<Vector2i, int>>>.Default.GetHashCode(this.Vis1)) * -1521134295 + EqualityComparer<List<Dictionary<Vector2i, int>>>.Default.GetHashCode(this.Vis2)) * -1521134295 + EqualityComparer<List<HashSet<Vector2i>>>.Default.GetHashCode(this.SeedTiles)) * -1521134295 + EqualityComparer<List<HashSet<Vector2i>>>.Default.GetHashCode(this.BoundaryTiles);
    }

    [CompilerGenerated]
    public readonly bool Equals(StationAiVisionSystem.ViewJob other)
    {
      return EqualityComparer<IEntityManager>.Default.Equals(this.EntManager, other.EntManager) && EqualityComparer<SharedMapSystem>.Default.Equals(this.Maps, other.Maps) && EqualityComparer<StationAiVisionSystem>.Default.Equals(this.System, other.System) && EqualityComparer<Entity<MapGridComponent>>.Default.Equals(this.Grid, other.Grid) && EqualityComparer<List<Entity<StationAiVisionComponent>>>.Default.Equals(this.Data, other.Data) && EqualityComparer<HashSet<Vector2i>>.Default.Equals(this.VisibleTiles, other.VisibleTiles) && EqualityComparer<List<Dictionary<Vector2i, int>>>.Default.Equals(this.Vis1, other.Vis1) && EqualityComparer<List<Dictionary<Vector2i, int>>>.Default.Equals(this.Vis2, other.Vis2) && EqualityComparer<List<HashSet<Vector2i>>>.Default.Equals(this.SeedTiles, other.SeedTiles) && EqualityComparer<List<HashSet<Vector2i>>>.Default.Equals(this.BoundaryTiles, other.BoundaryTiles);
    }
  }
}
