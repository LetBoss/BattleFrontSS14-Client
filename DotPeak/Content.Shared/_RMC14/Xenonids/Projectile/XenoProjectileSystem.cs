// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Projectile.XenoProjectileSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Light;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Random;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Weapons.Ranged.Prediction;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Mobs.Components;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Projectile;

public sealed class XenoProjectileSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedGunSystem _gun;
  [Dependency]
  private SharedGunPredictionSystem _gunPrediction;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedProjectileSystem _projectile;
  [Dependency]
  private SharedRMCLagCompensationSystem _rmcLagCompensation;
  [Dependency]
  private CMPoweredLightSystem _rmcPoweredLight;
  [Dependency]
  private RMCPseudoRandomSystem _rmcPseudoRandom;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  private Robust.Shared.GameObjects.EntityQuery<ProjectileComponent> _projectileQuery;
  private Robust.Shared.GameObjects.EntityQuery<PreventAttackLightOffComponent> _preventAttackLightOffQuery;
  private int _limitHitsId;

  public override void Initialize()
  {
    this._projectileQuery = this.GetEntityQuery<ProjectileComponent>();
    this._preventAttackLightOffQuery = this.GetEntityQuery<PreventAttackLightOffComponent>();
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestartCleanup));
    this.SubscribeNetworkEvent<XenoProjectilePredictedHitEvent>(new EntitySessionEventHandler<XenoProjectilePredictedHitEvent>(this.OnPredictedHit));
    this.SubscribeLocalEvent<XenoProjectileShooterComponent, ComponentRemove>(new EntityEventRefHandler<XenoProjectileShooterComponent, ComponentRemove>(this.OnShooterRemove<ComponentRemove>));
    this.SubscribeLocalEvent<XenoProjectileShooterComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoProjectileShooterComponent, EntityTerminatingEvent>(this.OnShooterRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<XenoProjectileShotComponent, ComponentRemove>(new EntityEventRefHandler<XenoProjectileShotComponent, ComponentRemove>(this.OnShotRemove<ComponentRemove>));
    this.SubscribeLocalEvent<XenoProjectileShotComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoProjectileShotComponent, EntityTerminatingEvent>(this.OnShotRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<XenoClientProjectileShotComponent, StartCollideEvent>(new EntityEventRefHandler<XenoClientProjectileShotComponent, StartCollideEvent>(this.OnShotCollide));
    this.SubscribeLocalEvent<XenoProjectileComponent, PreventCollideEvent>(new EntityEventRefHandler<XenoProjectileComponent, PreventCollideEvent>(this.OnPreventCollide));
    this.SubscribeLocalEvent<XenoProjectileComponent, ProjectileHitEvent>(new EntityEventRefHandler<XenoProjectileComponent, ProjectileHitEvent>(this.OnProjectileHit));
    this.SubscribeLocalEvent<XenoProjectileComponent, CMClusterSpawnedEvent>(new EntityEventRefHandler<XenoProjectileComponent, CMClusterSpawnedEvent>(this.OnClusterSpawned));
  }

  private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev) => this._limitHitsId = 0;

  private void OnPredictedHit(XenoProjectilePredictedHitEvent msg, EntitySessionEventArgs args)
  {
    if (this._net.IsClient || !this._gunPrediction.GunPrediction)
      return;
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    EntityUid entity = this.GetEntity(msg.Target);
    XenoProjectileShooterComponent comp1;
    EntityUid? element;
    if (!entity.Valid || !this.TryComp<XenoProjectileShooterComponent>(valueOrDefault, out comp1) || comp1.Shot.Count == 0 || !comp1.Shot.TryFirstOrNull<EntityUid>((Func<EntityUid, bool>) (e =>
    {
      int? id1 = this.CompOrNull<XenoProjectileShotComponent>(e)?.Id;
      int id2 = msg.Id;
      return id1.GetValueOrDefault() == id2 & id1.HasValue;
    }), out element) || this.TerminatingOrDeleted(element))
      return;
    this._rmcLagCompensation.SetLastRealTick(args.SenderSession.UserId, msg.LastRealTick);
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(this._rmcLagCompensation.GetCoordinates(entity, args.SenderSession));
    ProjectileComponent comp2;
    PhysicsComponent comp3;
    if (!this.TryComp<ProjectileComponent>(element, out comp2) || !this.TryComp<PhysicsComponent>(element, out comp3) || !this._rmcLagCompensation.Collides((Entity<FixturesComponent>) entity, (Entity<PhysicsComponent>) (element.Value, comp3), mapCoordinates, new Angle()))
      return;
    this._projectile.ProjectileCollide((Entity<ProjectileComponent, PhysicsComponent>) (element.Value, comp2, comp3), entity, true);
  }

  private void OnShooterRemove<T>(Entity<XenoProjectileShooterComponent> ent, ref T args)
  {
    if (this._timing.ApplyingState)
      return;
    foreach (EntityUid uid in ent.Comp.Shot)
      this.RemCompDeferred<XenoProjectileShotComponent>(uid);
    ent.Comp.Shot.Clear();
    this.Dirty<XenoProjectileShooterComponent>(ent);
  }

  private void OnShotRemove<T>(Entity<XenoProjectileShotComponent> ent, ref T args)
  {
    EntityUid? shooterEnt = ent.Comp.ShooterEnt;
    if (!shooterEnt.HasValue)
      return;
    EntityUid valueOrDefault = shooterEnt.GetValueOrDefault();
    XenoProjectileShooterComponent comp;
    if (!this.TryComp<XenoProjectileShooterComponent>(valueOrDefault, out comp) || !comp.Shot.Remove((EntityUid) ent))
      return;
    this.Dirty(valueOrDefault, (IComponent) comp);
  }

  private void OnShotCollide(
    Entity<XenoClientProjectileShotComponent> ent,
    ref StartCollideEvent args)
  {
    XenoProjectileShotComponent comp;
    if (this._net.IsServer || !this.IsClientSide((EntityUid) ent) || !this.TryComp<XenoProjectileShotComponent>((EntityUid) ent, out comp))
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new XenoProjectilePredictedHitEvent(comp.Id, this.GetNetEntity(args.OtherEntity), this._rmcLagCompensation.GetLastRealTick(new NetUserId?())));
  }

  private void OnPreventCollide(Entity<XenoProjectileComponent> ent, ref PreventCollideEvent args)
  {
    if (args.Cancelled)
      return;
    if (this._preventAttackLightOffQuery.HasComp(args.OtherEntity) && this._rmcPoweredLight.IsOff(args.OtherEntity))
    {
      args.Cancelled = true;
    }
    else
    {
      if (ent.Comp.DeleteOnFriendlyXeno || !this._hive.FromSameHive((Entity<HiveMemberComponent>) ent.Owner, (Entity<HiveMemberComponent>) args.OtherEntity) || !this.HasComp<XenoComponent>(args.OtherEntity) && !this.HasComp<HiveCoreComponent>(args.OtherEntity))
        return;
      args.Cancelled = true;
    }
  }

  private void OnProjectileHit(Entity<XenoProjectileComponent> ent, ref ProjectileHitEvent args)
  {
    if (this._hive.FromSameHive((Entity<HiveMemberComponent>) ent.Owner, (Entity<HiveMemberComponent>) args.Target))
    {
      args.Handled = true;
      if (!this._net.IsServer && !this.IsClientSide((EntityUid) ent))
        return;
      this.QueueDel(new EntityUid?((EntityUid) ent));
    }
    else
    {
      if (this.HasComp<XenoComponent>(args.Target))
        args.Damage = this._xeno.TryApplyXenoProjectileDamageMultiplier(args.Target, args.Damage);
      ProjectileComponent component;
      if (!this._projectileQuery.TryComp((EntityUid) ent, out component))
        return;
      EntityUid? shooter = component.Shooter;
      if (!shooter.HasValue)
        return;
      EntityUid valueOrDefault = shooter.GetValueOrDefault();
      XenoProjectileHitUserEvent args1 = new XenoProjectileHitUserEvent(args.Target);
      this.RaiseLocalEvent<XenoProjectileHitUserEvent>(valueOrDefault, ref args1);
    }
  }

  private void OnClusterSpawned(Entity<XenoProjectileComponent> ent, ref CMClusterSpawnedEvent args)
  {
    Entity<HiveComponent>? hive = this._hive.GetHive((Entity<HiveMemberComponent>) ent.Owner);
    if (!hive.HasValue)
      return;
    Entity<HiveComponent> valueOrDefault = hive.GetValueOrDefault();
    foreach (EntityUid member in args.Spawned)
      this._hive.SetHive((Entity<HiveMemberComponent>) member, new EntityUid?((EntityUid) valueOrDefault));
  }

  public bool TryShoot(
    EntityUid xeno,
    EntityCoordinates targetCoords,
    FixedPoint2 plasma,
    EntProtoId projectileId,
    SoundSpecifier? sound,
    int shots,
    Angle deviation,
    float speed,
    float? stopAtDistance = null,
    EntityUid? target = null,
    bool predicted = true,
    int? projectileHitLimit = null)
  {
    if (!predicted && this._net.IsClient)
      return false;
    MapCoordinates mapCoordinates1 = this._transform.GetMapCoordinates(xeno);
    MapCoordinates mapCoordinates2 = this._transform.ToMapCoordinates(targetCoords);
    if (mapCoordinates1.MapId != mapCoordinates2.MapId || mapCoordinates1.Position == mapCoordinates2.Position || !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno, plasma))
      return false;
    this._audio.PlayPredicted(sound, xeno, new EntityUid?(xeno));
    if (this._net.IsClient && !this._gunPrediction.GunPrediction || !this._timing.IsFirstTimePredicted)
      return true;
    AmmoShotEvent args = new AmmoShotEvent()
    {
      FiredProjectiles = new List<EntityUid>(shots)
    };
    if (target.HasValue && this.HasComp<MobStateComponent>(target) && !this._xeno.CanAbilityAttackTarget(xeno, target.Value))
      target = new EntityUid?();
    XenoProjectileShooterComponent shooterComponent = (XenoProjectileShooterComponent) null;
    ICommonSession playerSession = this.CompOrNull<ActorComponent>(xeno)?.PlayerSession;
    Xoroshiro64S xoroshiro64S = this._rmcPseudoRandom.GetXoroshiro64S(xeno);
    Vector2 vector2 = mapCoordinates2.Position - mapCoordinates1.Position;
    double num = Angle.op_Implicit(deviation) / 2.0;
    if (projectileHitLimit.HasValue)
      ++this._limitHitsId;
    for (int index = 0; index < shots; ++index)
    {
      Angle angle = Angle.Zero;
      if (index > 0 && Angle.op_Inequality(deviation, Angle.Zero))
        angle = this._rmcPseudoRandom.NextAngle(ref xoroshiro64S, Angle.op_Implicit(-num), Angle.op_Implicit(num));
      Vector2 direction = new MapCoordinates(mapCoordinates1.Position + ((Angle) ref angle).RotateVec(ref vector2), mapCoordinates2.MapId).Position - mapCoordinates1.Position;
      EntityUid entityUid = this.Spawn((string) projectileId, mapCoordinates1, rotation: new Angle());
      direction *= speed / direction.Length();
      this._gun.ShootProjectile(entityUid, direction, Vector2.Zero, new EntityUid?(xeno), new EntityUid?(xeno), speed);
      args.FiredProjectiles.Add(entityUid);
      this.EnsureComp<XenoProjectileComponent>(entityUid);
      this._hive.SetSameHive((Entity<HiveMemberComponent>) xeno, (Entity<HiveMemberComponent>) entityUid);
      if (stopAtDistance.HasValue)
      {
        ProjectileFixedDistanceComponent distanceComponent = this.EnsureComp<ProjectileFixedDistanceComponent>(entityUid);
        distanceComponent.FlyEndTime = this._timing.CurTime + TimeSpan.FromSeconds((double) stopAtDistance.Value / (double) speed);
        this.Dirty(entityUid, (IComponent) distanceComponent);
      }
      if (target.HasValue)
      {
        TargetedProjectileComponent projectileComponent = this.EnsureComp<TargetedProjectileComponent>(entityUid);
        projectileComponent.Target = target.Value;
        this.Dirty(entityUid, (IComponent) projectileComponent);
      }
      if (projectileHitLimit.HasValue)
      {
        ProjectileLimitHitsComponent limitHitsComponent = this.EnsureComp<ProjectileLimitHitsComponent>(entityUid);
        limitHitsComponent.Limit = projectileHitLimit.Value;
        limitHitsComponent.OriginEntity = xeno;
        limitHitsComponent.ExtraId = new int?(this._limitHitsId);
        this.Dirty(entityUid, (IComponent) limitHitsComponent);
      }
      if (predicted)
      {
        if (shooterComponent == null)
          shooterComponent = this.EnsureComp<XenoProjectileShooterComponent>(xeno);
        shooterComponent.Shot.Add(entityUid);
        this.Dirty(xeno, (IComponent) shooterComponent);
        XenoProjectileShotComponent projectileShotComponent = this.EnsureComp<XenoProjectileShotComponent>(entityUid);
        projectileShotComponent.Id = shooterComponent.NextId++;
        projectileShotComponent.Shooter = playerSession;
        projectileShotComponent.ShooterEnt = new EntityUid?(xeno);
        this.Dirty(entityUid, (IComponent) projectileShotComponent);
      }
      if (!this._net.IsServer)
      {
        this.EnsureComp<XenoClientProjectileShotComponent>(entityUid);
        this._physics.UpdateIsPredicted(new EntityUid?(entityUid));
      }
    }
    this.RaiseLocalEvent<AmmoShotEvent>(xeno, args);
    return true;
  }
}
