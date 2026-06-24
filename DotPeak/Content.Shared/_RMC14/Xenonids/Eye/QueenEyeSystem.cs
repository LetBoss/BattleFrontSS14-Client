// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Eye.QueenEyeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Watch;
using Content.Shared.Coordinates;
using Content.Shared.Mind;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Threading;
using Robust.Shared.Timing;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Eye;

public sealed class QueenEyeSystem : EntitySystem
{
  [Robust.Shared.IoC.Dependency]
  private EntityLookupSystem _entityLookup;
  [Robust.Shared.IoC.Dependency]
  private SharedEyeSystem _eye;
  [Robust.Shared.IoC.Dependency]
  private SharedMapSystem _map;
  [Robust.Shared.IoC.Dependency]
  private SharedMindSystem _mind;
  [Robust.Shared.IoC.Dependency]
  private SharedMoverController _mover;
  [Robust.Shared.IoC.Dependency]
  private INetManager _net;
  [Robust.Shared.IoC.Dependency]
  private IParallelManager _parallel;
  [Robust.Shared.IoC.Dependency]
  private IGameTiming _timing;
  [Robust.Shared.IoC.Dependency]
  private SharedTransformSystem _transform;
  [Robust.Shared.IoC.Dependency]
  private SharedXenoWatchSystem _xenoWatch;
  private QueenEyeSystem.SeedJob _seedJob;
  private QueenEyeSystem.ViewJob _job;
  private readonly HashSet<Entity<QueenEyeVisionComponent>> _seeds = new HashSet<Entity<QueenEyeVisionComponent>>();
  private readonly HashSet<Vector2i> _singleTiles = new HashSet<Vector2i>();

  public override void Initialize()
  {
    base.Initialize();
    this._seedJob = new QueenEyeSystem.SeedJob()
    {
      System = this
    };
    this._job = new QueenEyeSystem.ViewJob()
    {
      EntManager = (IEntityManager) this.EntityManager,
      Maps = this._map,
      System = this,
      VisibleTiles = this._singleTiles
    };
    this.SubscribeLocalEvent<QueenEyeActionComponent, MapInitEvent>(new EntityEventRefHandler<QueenEyeActionComponent, MapInitEvent>(this.OnQueenEyeActionMapInit));
    this.SubscribeLocalEvent<QueenEyeActionComponent, ComponentRemove>(new EntityEventRefHandler<QueenEyeActionComponent, ComponentRemove>(this.OnQueenEyeActionRemove));
    this.SubscribeLocalEvent<QueenEyeActionComponent, EntityTerminatingEvent>(new EntityEventRefHandler<QueenEyeActionComponent, EntityTerminatingEvent>(this.OnQueenEyeActionTerminating));
    this.SubscribeLocalEvent<QueenEyeActionComponent, QueenEyeActionEvent>(new EntityEventRefHandler<QueenEyeActionComponent, QueenEyeActionEvent>(this.OnQueenEyeAction));
    this.SubscribeLocalEvent<QueenEyeActionComponent, GetVisMaskEvent>(new EntityEventRefHandler<QueenEyeActionComponent, GetVisMaskEvent>(this.OnQueenEyeActionGetVisMask));
    this.SubscribeLocalEvent<QueenEyeActionComponent, XenoWatchEvent>(new EntityEventRefHandler<QueenEyeActionComponent, XenoWatchEvent>(this.OnQueenEyeActionWatch));
    this.SubscribeLocalEvent<QueenEyeActionComponent, XenoUnwatchEvent>(new EntityEventRefHandler<QueenEyeActionComponent, XenoUnwatchEvent>(this.OnQueenEyeActionUnwatch));
    this.SubscribeLocalEvent<QueenEyeActionComponent, XenoOvipositorChangedEvent>(new EntityEventRefHandler<QueenEyeActionComponent, XenoOvipositorChangedEvent>(this.OnQueenEyeOvipositorChanged));
    this.SubscribeLocalEvent<QueenEyeComponent, XenoUnwatchEvent>(new EntityEventRefHandler<QueenEyeComponent, XenoUnwatchEvent>(this.OnQueenEyeUnwatch));
  }

  private void OnQueenEyeActionMapInit(Entity<QueenEyeActionComponent> ent, ref MapInitEvent args)
  {
    this._eye.RefreshVisibilityMask((Entity<EyeComponent>) ent.Owner);
  }

  private void OnQueenEyeActionRemove(Entity<QueenEyeActionComponent> ent, ref ComponentRemove args)
  {
    this.RemoveQueenEye(ent);
  }

  private void OnQueenEyeActionTerminating(
    Entity<QueenEyeActionComponent> ent,
    ref EntityTerminatingEvent args)
  {
    this.RemoveQueenEye(ent);
  }

  private void OnQueenEyeAction(Entity<QueenEyeActionComponent> ent, ref QueenEyeActionEvent args)
  {
    EyeComponent comp;
    if (this.RemoveQueenEye(ent) || this._net.IsClient || !this.TryComp<EyeComponent>((EntityUid) ent, out comp))
      return;
    ent.Comp.Eye = new EntityUid?(this.SpawnAtPosition((string) ent.Comp.Spawn, ent.Owner.ToCoordinates()));
    this.Dirty<QueenEyeActionComponent>(ent);
    QueenEyeComponent queenEyeComponent = this.EnsureComp<QueenEyeComponent>(ent.Comp.Eye.Value);
    queenEyeComponent.Queen = new EntityUid?((EntityUid) ent);
    this.Dirty(ent.Comp.Eye.Value, (IComponent) queenEyeComponent);
    this._eye.SetPvsScale((Entity<EyeComponent>) ((EntityUid) ent, comp), ent.Comp.EyePvsScale);
    this._eye.SetTarget((EntityUid) ent, ent.Comp.Eye, comp);
    this._eye.SetDrawFov((EntityUid) ent, false);
    this._mover.SetRelay((EntityUid) ent, ent.Comp.Eye.Value);
  }

  private void OnQueenEyeActionGetVisMask(
    Entity<QueenEyeActionComponent> ent,
    ref GetVisMaskEvent args)
  {
    MindComponent mind;
    if (!this._mind.TryGetMind(ent.Owner, out EntityUid _, out mind) || !this.HasComp<QueenEyeComponent>(mind.VisitingEntity))
      return;
    args.VisibilityMask |= (int) ent.Comp.Visibility;
  }

  private void OnQueenEyeActionWatch(Entity<QueenEyeActionComponent> ent, ref XenoWatchEvent args)
  {
    EntityUid? eye = ent.Comp.Eye;
    if (!eye.HasValue)
      return;
    this._xenoWatch.SetWatching((Entity<XenoWatchingComponent>) eye.GetValueOrDefault(), args.Watching);
  }

  private void OnQueenEyeActionUnwatch(
    Entity<QueenEyeActionComponent> ent,
    ref XenoUnwatchEvent args)
  {
    EntityUid? eye = ent.Comp.Eye;
    if (!eye.HasValue)
      return;
    this.RemCompDeferred<XenoWatchingComponent>(eye.GetValueOrDefault());
  }

  private void OnQueenEyeOvipositorChanged(
    Entity<QueenEyeActionComponent> ent,
    ref XenoOvipositorChangedEvent args)
  {
    if (this._timing.ApplyingState || args.Attached)
      return;
    this.RemoveQueenEye(ent);
  }

  private void OnQueenEyeUnwatch(Entity<QueenEyeComponent> ent, ref XenoUnwatchEvent args)
  {
    EntityUid? queen = ent.Comp.Queen;
    if (!queen.HasValue)
      return;
    this._eye.SetTarget(queen.GetValueOrDefault(), new EntityUid?((EntityUid) ent));
  }

  public void GetView(
    Entity<BroadphaseComponent, MapGridComponent> grid,
    Box2Rotated worldBounds,
    HashSet<Vector2i> visibleTiles,
    float expansionSize = 29f)
  {
    this._seeds.Clear();
    this._seedJob.Grid = (Entity<MapGridComponent>) (grid.Owner, grid.Comp2);
    Matrix3x2 invWorldMatrix = this._transform.GetInvWorldMatrix((EntityUid) grid);
    Box2Rotated box2Rotated = ((Box2Rotated) ref worldBounds).Enlarged(expansionSize);
    ref Box2Rotated local = ref box2Rotated;
    this._seedJob.ExpandedBounds = Matrix3Helpers.TransformBox(invWorldMatrix, ref local);
    this._parallel.ProcessNow((IRobustJob) this._seedJob);
    this._job.Data.Clear();
    foreach (Entity<QueenEyeVisionComponent> seed in this._seeds)
      this._job.Data.Add(seed);
    if (this._seeds.Count == 0)
      return;
    this._job.Grid = (Entity<MapGridComponent>) (grid.Owner, grid.Comp2);
    this._job.VisibleTiles = visibleTiles;
    this._parallel.ProcessNow((IParallelRobustJob) this._job, this._job.Data.Count);
  }

  private bool IsAccessible(
    Entity<BroadphaseComponent, MapGridComponent> grid,
    Vector2i tile,
    float expansionSize = 29f)
  {
    this._seeds.Clear();
    Box2 localBounds = this._entityLookup.GetLocalBounds(tile, grid.Comp2.TileSize);
    Box2 box2 = ((Box2) ref localBounds).Enlarged(expansionSize);
    this._seedJob.Grid = (Entity<MapGridComponent>) (grid.Owner, grid.Comp2);
    this._seedJob.ExpandedBounds = box2;
    this._parallel.ProcessNow((IRobustJob) this._seedJob);
    this._job.Data.Clear();
    foreach (Entity<QueenEyeVisionComponent> seed in this._seeds)
      this._job.Data.Add(seed);
    if (this._seeds.Count == 0)
      return false;
    this._singleTiles.Clear();
    this._job.Grid = (Entity<MapGridComponent>) (grid.Owner, grid.Comp2);
    this._job.VisibleTiles = this._singleTiles;
    this._parallel.ProcessNow((IParallelRobustJob) this._job, this._job.Data.Count);
    return this._job.VisibleTiles.Contains(tile);
  }

  private bool RemoveQueenEye(Entity<QueenEyeActionComponent> ent)
  {
    if (!ent.Comp.Eye.HasValue)
      return false;
    this._eye.SetTarget((EntityUid) ent, new EntityUid?());
    this._eye.SetPvsScale((Entity<EyeComponent>) ent.Owner, ent.Comp.PvsScale);
    this._eye.SetDrawFov((EntityUid) ent, true);
    ent.Comp.Eye = new EntityUid?();
    this.Dirty<QueenEyeActionComponent>(ent);
    if (this._net.IsServer && this.HasComp<QueenEyeComponent>(ent.Comp.Eye))
      this.QueueDel(ent.Comp.Eye);
    this.RemComp<RelayInputMoverComponent>((EntityUid) ent);
    QueenEyeActionUpdated args = new QueenEyeActionUpdated(ent);
    this.RaiseLocalEvent<QueenEyeActionUpdated>((EntityUid) ent, ref args);
    return true;
  }

  public bool IsInQueenEye(Entity<QueenEyeActionComponent?> queen)
  {
    return this.Resolve<QueenEyeActionComponent>((EntityUid) queen, ref queen.Comp, false) && queen.Comp.Eye.HasValue;
  }

  public bool CanSeeTarget(Entity<QueenEyeActionComponent?> queen, EntityUid target)
  {
    if (!this.Resolve<QueenEyeActionComponent>((EntityUid) queen, ref queen.Comp, false) || !queen.Comp.Eye.HasValue)
      return false;
    TransformComponent transformComponent = this.Transform(target);
    BroadphaseComponent comp1;
    MapGridComponent comp2;
    if (!this.TryComp<BroadphaseComponent>(transformComponent.GridUid, out comp1) || !this.TryComp<MapGridComponent>(transformComponent.GridUid, out comp2))
      return false;
    Vector2i tile = this._map.LocalToTile(transformComponent.GridUid.Value, comp2, transformComponent.Coordinates);
    return this.IsAccessible((Entity<BroadphaseComponent, MapGridComponent>) (transformComponent.GridUid.Value, comp1, comp2), tile);
  }

  public bool CanSeeTarget(Entity<QueenEyeActionComponent?> queen, EntityCoordinates target)
  {
    if (!this.Resolve<QueenEyeActionComponent>((EntityUid) queen, ref queen.Comp, false) || !queen.Comp.Eye.HasValue)
      return false;
    EntityUid? grid = this._transform.GetGrid(target);
    if (grid.HasValue)
    {
      EntityUid valueOrDefault = grid.GetValueOrDefault();
      BroadphaseComponent comp1;
      MapGridComponent comp2;
      if (this.TryComp<BroadphaseComponent>(valueOrDefault, out comp1) && this.TryComp<MapGridComponent>(valueOrDefault, out comp2))
      {
        Vector2i tile = this._map.CoordinatesToTile(valueOrDefault, comp2, target);
        return this.IsAccessible((Entity<BroadphaseComponent, MapGridComponent>) (valueOrDefault, comp1, comp2), tile);
      }
    }
    return false;
  }

  private record struct SeedJob : IRobustJob
  {
    public required QueenEyeSystem System;
    public Entity<MapGridComponent> Grid;
    public Box2 ExpandedBounds;

    public void Execute()
    {
      this.System._entityLookup.GetLocalEntitiesIntersecting<QueenEyeVisionComponent>(this.Grid.Owner, this.ExpandedBounds, this.System._seeds, LookupFlags.All | LookupFlags.Approximate);
    }

    [CompilerGenerated]
    public override readonly int GetHashCode()
    {
      return (EqualityComparer<QueenEyeSystem>.Default.GetHashCode(this.System) * -1521134295 + EqualityComparer<Entity<MapGridComponent>>.Default.GetHashCode(this.Grid)) * -1521134295 + EqualityComparer<Box2>.Default.GetHashCode(this.ExpandedBounds);
    }

    [CompilerGenerated]
    public readonly bool Equals(QueenEyeSystem.SeedJob other)
    {
      return EqualityComparer<QueenEyeSystem>.Default.Equals(this.System, other.System) && EqualityComparer<Entity<MapGridComponent>>.Default.Equals(this.Grid, other.Grid) && EqualityComparer<Box2>.Default.Equals(this.ExpandedBounds, other.ExpandedBounds);
    }
  }

  private record struct ViewJob : IParallelRobustJob, IParallelRangeRobustJob
  {
    public required IEntityManager EntManager;
    public required SharedMapSystem Maps;
    public required QueenEyeSystem System;
    public Entity<MapGridComponent> Grid;
    public List<Entity<QueenEyeVisionComponent>> Data;
    public required HashSet<Vector2i> VisibleTiles;

    public ViewJob()
    {
      this.EntManager = (IEntityManager) null;
      this.Maps = (SharedMapSystem) null;
      this.System = (QueenEyeSystem) null;
      this.Grid = new Entity<MapGridComponent>();
      this.VisibleTiles = (HashSet<Vector2i>) null;
      this.Data = new List<Entity<QueenEyeVisionComponent>>();
    }

    public int BatchSize => 1;

    public void Execute(int index)
    {
      Entity<QueenEyeVisionComponent> uid = this.Data[index];
      IEnumerable<TileRef> tilesIntersecting = this.Maps.GetLocalTilesIntersecting(this.Grid.Owner, this.Grid.Comp, Box2.CenteredAround(this.System._transform.GetWorldPosition(this.EntManager.GetComponent<TransformComponent>((EntityUid) uid)), new Vector2(uid.Comp.Range)), false);
      lock (this.VisibleTiles)
      {
        foreach (TileRef tileRef in tilesIntersecting)
          this.VisibleTiles.Add(tileRef.GridIndices);
      }
    }

    [CompilerGenerated]
    public override readonly int GetHashCode()
    {
      return ((((EqualityComparer<IEntityManager>.Default.GetHashCode(this.EntManager) * -1521134295 + EqualityComparer<SharedMapSystem>.Default.GetHashCode(this.Maps)) * -1521134295 + EqualityComparer<QueenEyeSystem>.Default.GetHashCode(this.System)) * -1521134295 + EqualityComparer<Entity<MapGridComponent>>.Default.GetHashCode(this.Grid)) * -1521134295 + EqualityComparer<List<Entity<QueenEyeVisionComponent>>>.Default.GetHashCode(this.Data)) * -1521134295 + EqualityComparer<HashSet<Vector2i>>.Default.GetHashCode(this.VisibleTiles);
    }

    [CompilerGenerated]
    public readonly bool Equals(QueenEyeSystem.ViewJob other)
    {
      return EqualityComparer<IEntityManager>.Default.Equals(this.EntManager, other.EntManager) && EqualityComparer<SharedMapSystem>.Default.Equals(this.Maps, other.Maps) && EqualityComparer<QueenEyeSystem>.Default.Equals(this.System, other.System) && EqualityComparer<Entity<MapGridComponent>>.Default.Equals(this.Grid, other.Grid) && EqualityComparer<List<Entity<QueenEyeVisionComponent>>>.Default.Equals(this.Data, other.Data) && EqualityComparer<HashSet<Vector2i>>.Default.Equals(this.VisibleTiles, other.VisibleTiles);
    }
  }
}
