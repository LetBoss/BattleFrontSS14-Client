// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Weapons.Ranged.Prediction.GunPredictionSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Projectiles;
using Content.Shared._RMC14.Weapons.Ranged.Prediction;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Client.GameObjects;
using Robust.Client.Physics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._RMC14.Weapons.Ranged.Prediction;

public sealed class GunPredictionSystem : SharedGunPredictionSystem
{
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private ProjectileSystem _projectile;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  private EntityQuery<IgnorePredictionHideComponent> _ignorePredictionHideQuery;
  private EntityQuery<IgnorePredictionHitComponent> _ignorePredictionHitQuery;
  private EntityQuery<SpriteComponent> _spriteQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._ignorePredictionHideQuery = this.GetEntityQuery<IgnorePredictionHideComponent>();
    this._ignorePredictionHitQuery = this.GetEntityQuery<IgnorePredictionHitComponent>();
    this._spriteQuery = this.GetEntityQuery<SpriteComponent>();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PhysicsUpdateBeforeSolveEvent>(new EntityEventRefHandler<PhysicsUpdateBeforeSolveEvent>((object) this, __methodptr(OnBeforeSolve)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PhysicsUpdateAfterSolveEvent>(new EntityEventRefHandler<PhysicsUpdateAfterSolveEvent>((object) this, __methodptr(OnAfterSolve)), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<RequestShootEvent>(new EntitySessionEventHandler<RequestShootEvent>(this.OnShootRequest), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PredictedProjectileClientComponent, UpdateIsPredictedEvent>(new EntityEventRefHandler<PredictedProjectileClientComponent, UpdateIsPredictedEvent>((object) this, __methodptr(OnClientProjectileUpdateIsPredicted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PredictedProjectileClientComponent, StartCollideEvent>(new EntityEventRefHandler<PredictedProjectileClientComponent, StartCollideEvent>((object) this, __methodptr(OnClientProjectileStartCollide)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PredictedProjectileServerComponent, ComponentStartup>(new EntityEventRefHandler<PredictedProjectileServerComponent, ComponentStartup>((object) this, __methodptr(OnServerProjectileStartup)), (Type[]) null, (Type[]) null);
    this.UpdatesBefore.Add(typeof (TransformSystem));
  }

  private void OnBeforeSolve(ref PhysicsUpdateBeforeSolveEvent ev)
  {
    EntityQueryEnumerator<PredictedProjectileClientComponent> entityQueryEnumerator = this.EntityQueryEnumerator<PredictedProjectileClientComponent>();
    EntityUid entityUid;
    PredictedProjectileClientComponent projectileClientComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref projectileClientComponent))
      projectileClientComponent.Coordinates = new EntityCoordinates?(this.Transform(entityUid).Coordinates);
  }

  private void OnAfterSolve(ref PhysicsUpdateAfterSolveEvent ev)
  {
    if (this._timing.IsFirstTimePredicted)
      return;
    EntityQueryEnumerator<PredictedProjectileClientComponent> entityQueryEnumerator = this.EntityQueryEnumerator<PredictedProjectileClientComponent>();
    EntityUid entityUid;
    PredictedProjectileClientComponent projectileClientComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref projectileClientComponent))
    {
      EntityCoordinates? coordinates = projectileClientComponent.Coordinates;
      if (coordinates.HasValue)
      {
        EntityCoordinates valueOrDefault = coordinates.GetValueOrDefault();
        this._transform.SetCoordinates(entityUid, valueOrDefault);
      }
      projectileClientComponent.Coordinates = new EntityCoordinates?();
    }
  }

  private void OnShootRequest(RequestShootEvent ev, EntitySessionEventArgs args)
  {
    this.ShootRequested(ev.Gun, ev.Coordinates, ev.Target, (List<int>) null, ((EntitySessionEventArgs) ref args).SenderSession);
  }

  private void OnClientProjectileUpdateIsPredicted(
    Entity<PredictedProjectileClientComponent> ent,
    ref UpdateIsPredictedEvent args)
  {
    args.IsPredicted = true;
  }

  private void OnClientProjectileStartCollide(
    Entity<PredictedProjectileClientComponent> ent,
    ref StartCollideEvent args)
  {
    ProjectileComponent projectileComponent;
    PhysicsComponent physicsComponent;
    if (ent.Comp.Hit || !this.TryComp<ProjectileComponent>(Entity<PredictedProjectileClientComponent>.op_Implicit(ent), ref projectileComponent) || !this.TryComp<PhysicsComponent>(Entity<PredictedProjectileClientComponent>.op_Implicit(ent), ref physicsComponent) || this._ignorePredictionHitQuery.HasComp(args.OtherEntity))
      return;
    HashSet<(NetEntity, MapCoordinates)> hit = new HashSet<(NetEntity, MapCoordinates)>()
    {
      (this.GetNetEntity(args.OtherEntity, (MetaDataComponent) null), this._transform.GetMapCoordinates(args.OtherEntity, (TransformComponent) null))
    };
    PredictedProjectileHitEvent projectileHitEvent = new PredictedProjectileHitEvent(ent.Owner.Id, hit);
    ent.Comp.Hit = true;
    this.RaiseNetworkEvent((EntityEventArgs) projectileHitEvent);
    this._projectile.ProjectileCollide(Entity<ProjectileComponent, PhysicsComponent>.op_Implicit((Entity<PredictedProjectileClientComponent>.op_Implicit(ent), projectileComponent, physicsComponent)), args.OtherEntity);
  }

  private void OnServerProjectileStartup(
    Entity<PredictedProjectileServerComponent> ent,
    ref ComponentStartup args)
  {
    if (!this.GunPrediction)
      return;
    EntityUid? clientEnt = ent.Comp.ClientEnt;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    SpriteComponent spriteComponent;
    if ((clientEnt.HasValue == localEntity.HasValue ? (clientEnt.HasValue ? (EntityUid.op_Inequality(clientEnt.GetValueOrDefault(), localEntity.GetValueOrDefault()) ? 1 : 0) : 0) : 1) != 0 || this._ignorePredictionHideQuery.HasComp(Entity<PredictedProjectileServerComponent>.op_Implicit(ent)) || !this._spriteQuery.TryComp(Entity<PredictedProjectileServerComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((Entity<PredictedProjectileServerComponent>.op_Implicit(ent), spriteComponent)), false);
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this._timing.IsFirstTimePredicted)
      return;
    EntityQueryEnumerator<PredictedProjectileClientComponent, ProjectileComponent, PhysicsComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<PredictedProjectileClientComponent, ProjectileComponent, PhysicsComponent>();
    EntityUid entityUid1;
    PredictedProjectileClientComponent projectileClientComponent;
    ProjectileComponent projectileComponent;
    PhysicsComponent physicsComponent;
    while (entityQueryEnumerator1.MoveNext(ref entityUid1, ref projectileClientComponent, ref projectileComponent, ref physicsComponent))
    {
      if (!projectileClientComponent.Hit)
      {
        HashSet<EntityUid> contactingEntities = this._physics.GetContactingEntities(entityUid1, physicsComponent, true);
        if (contactingEntities.Count != 0)
        {
          HashSet<(NetEntity, MapCoordinates)> hit = new HashSet<(NetEntity, MapCoordinates)>();
          foreach (EntityUid entityUid2 in contactingEntities)
          {
            if (!this._ignorePredictionHitQuery.HasComp(entityUid2))
            {
              NetEntity netEntity = this.GetNetEntity(entityUid2, (MetaDataComponent) null);
              MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(entityUid2, (TransformComponent) null);
              hit.Add((netEntity, mapCoordinates));
            }
          }
          if (hit.Count != 0)
          {
            PredictedProjectileHitEvent projectileHitEvent = new PredictedProjectileHitEvent(entityUid1.Id, hit);
            projectileClientComponent.Hit = true;
            this.RaiseNetworkEvent((EntityEventArgs) projectileHitEvent);
            this._projectile.ProjectileCollide(Entity<ProjectileComponent, PhysicsComponent>.op_Implicit((entityUid1, projectileComponent, physicsComponent)), contactingEntities.First<EntityUid>());
          }
        }
      }
    }
    EntityQueryEnumerator<PredictedProjectileHitComponent, SpriteComponent, TransformComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<PredictedProjectileHitComponent, SpriteComponent, TransformComponent>();
    EntityUid entityUid3;
    PredictedProjectileHitComponent projectileHitComponent;
    SpriteComponent spriteComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator2.MoveNext(ref entityUid3, ref projectileHitComponent, ref spriteComponent, ref transformComponent))
    {
      EntityCoordinates origin = projectileHitComponent.Origin;
      EntityCoordinates coordinates = transformComponent.Coordinates;
      float num;
      if (!((EntityCoordinates) ref origin).TryDistance((IEntityManager) this.EntityManager, this._transform, coordinates, ref num) || (double) num >= (double) projectileHitComponent.Distance)
        this._sprite.SetVisible(Entity<SpriteComponent>.op_Implicit((entityUid3, spriteComponent)), false);
    }
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    EntityQueryEnumerator<PredictedProjectileClientComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<PredictedProjectileClientComponent, TransformComponent>();
    PredictedProjectileClientComponent projectileClientComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref projectileClientComponent, ref transformComponent))
      transformComponent.ActivelyLerping = false;
  }
}
