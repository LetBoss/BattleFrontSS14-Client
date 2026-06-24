// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.TailSeize.XenoTailSeizeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Hook;
using Content.Shared._RMC14.Xenonids.Projectile;
using Content.Shared.ActionBlocker;
using Content.Shared.FixedPoint;
using Content.Shared.Projectiles;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.TailSeize;

public sealed class XenoTailSeizeSystem : EntitySystem
{
  [Dependency]
  private XenoHookSystem _hook;
  [Dependency]
  private XenoProjectileSystem _projectile;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private RMCPullingSystem _pulling;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private RMCSizeStunSystem _size;
  [Dependency]
  private RMCObstacleSlammingSystem _obstacleSlamming;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoTailSeizeComponent, XenoTailSeizeActionEvent>(new EntityEventRefHandler<XenoTailSeizeComponent, XenoTailSeizeActionEvent>(this.OnTailSeizeAction));
    this.SubscribeLocalEvent<VictimTailSeizedComponent, StopThrowEvent>(new EntityEventRefHandler<VictimTailSeizedComponent, StopThrowEvent>(this.OnSeizeEnd));
    this.SubscribeLocalEvent<XenoHookComponent, AmmoShotEvent>(new EntityEventRefHandler<XenoHookComponent, AmmoShotEvent>(this.OnHookMade));
    this.SubscribeLocalEvent<XenoHookOnHitComponent, ProjectileHitEvent>(new EntityEventRefHandler<XenoHookOnHitComponent, ProjectileHitEvent>(this.OnHookHit));
  }

  private void OnHookMade(Entity<XenoHookComponent> hook, ref AmmoShotEvent args)
  {
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
      this._hook.TryHookTarget(hook, firedProjectile);
  }

  private void OnHookHit(Entity<XenoHookOnHitComponent> hook, ref ProjectileHitEvent args)
  {
    if (this._net.IsClient)
      return;
    EntityUid? shooter = args.Shooter;
    if (!shooter.HasValue)
      return;
    XenoSystem xeno1 = this._xeno;
    shooter = args.Shooter;
    EntityUid xeno2 = shooter.Value;
    EntityUid target1 = args.Target;
    XenoHookComponent comp;
    if (!xeno1.CanAbilityAttackTarget(xeno2, target1) || !this.TryComp<XenoHookComponent>(args.Shooter, out comp))
      return;
    XenoHookSystem hook1 = this._hook;
    shooter = args.Shooter;
    Entity<XenoHookComponent> xeno3 = (Entity<XenoHookComponent>) (shooter.Value, comp);
    EntityUid target2 = args.Target;
    if (!hook1.TryHookTarget(xeno3, target2))
      return;
    this._pulling.TryStopAllPullsFromAndOn(args.Target);
    SharedTransformSystem transform1 = this._transform;
    shooter = args.Shooter;
    EntityUid uid = shooter.Value;
    EntityCoordinates moverCoordinates1 = transform1.GetMoverCoordinates(uid);
    SharedTransformSystem transform2 = this._transform;
    shooter = args.Shooter;
    EntityUid entity = shooter.Value;
    MapCoordinates mapCoordinates = transform2.GetMapCoordinates(entity);
    EntityCoordinates moverCoordinates2 = this._transform.GetMoverCoordinates(args.Target);
    float distance;
    if (!moverCoordinates1.TryDistance((IEntityManager) this.EntityManager, moverCoordinates2, out distance))
      return;
    float num = -Math.Max(distance - 2f, 0.5f);
    this._obstacleSlamming.MakeImmune(args.Target);
    this._size.KnockBack(args.Target, new MapCoordinates?(mapCoordinates), num, num, 10f);
    this.EnsureComp<VictimTailSeizedComponent>(args.Target);
  }

  private void OnSeizeEnd(Entity<VictimTailSeizedComponent> victim, ref StopThrowEvent args)
  {
    this._slow.TrySlowdown((EntityUid) victim, victim.Comp.SlowTime, ignoreDurationModifier: true);
    this._slow.TryRoot((EntityUid) victim, victim.Comp.RootTime);
    this.RemCompDeferred<VictimTailSeizedComponent>((EntityUid) victim);
  }

  private void OnTailSeizeAction(
    Entity<XenoTailSeizeComponent> xeno,
    ref XenoTailSeizeActionEvent args)
  {
    if (args.Handled || !this._actionBlocker.CanAttack((EntityUid) xeno))
      return;
    MeleeWeaponComponent comp;
    if (this.TryComp<MeleeWeaponComponent>((EntityUid) xeno, out comp))
    {
      if (this._timing.CurTime < comp.NextAttack)
        return;
      comp.NextAttack = this._timing.CurTime + TimeSpan.FromSeconds(1L);
      this.Dirty((EntityUid) xeno, (IComponent) comp);
    }
    XenoProjectileSystem projectile1 = this._projectile;
    EntityUid xeno1 = (EntityUid) xeno;
    EntityCoordinates target1 = args.Target;
    FixedPoint2 plasma = (FixedPoint2) 0;
    EntProtoId projectile2 = xeno.Comp.Projectile;
    Angle zero = Angle.Zero;
    double speed = (double) xeno.Comp.Speed;
    EntityUid? entity = args.Entity;
    float? stopAtDistance = new float?();
    EntityUid? target2 = entity;
    int? projectileHitLimit = new int?();
    projectile1.TryShoot(xeno1, target1, plasma, projectile2, (SoundSpecifier) null, 1, zero, (float) speed, stopAtDistance, target2, projectileHitLimit: projectileHitLimit);
    MeleeAttackEvent args1 = new MeleeAttackEvent((EntityUid) xeno);
    this.RaiseLocalEvent<MeleeAttackEvent>((EntityUid) xeno, ref args1);
    this._audio.PlayPredicted(xeno.Comp.Sound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Handled = true;
  }
}
