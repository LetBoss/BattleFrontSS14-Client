// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Lunge.XenoLungeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Lunge;

public sealed class XenoLungeSystem : EntitySystem
{
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private ThrowingSystem _throwing;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private ThrownItemSystem _thrownItem;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private PullingSystem _pulling;
  [Dependency]
  private StatusEffectsSystem _statusEffects;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private SharedRMCLagCompensationSystem _rmcLagCompensation;
  [Dependency]
  private RMCObstacleSlammingSystem _rmcObstacleSlamming;
  [Dependency]
  private XenoLeapSystem _leap;
  [Dependency]
  private RMCSizeStunSystem _size;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;
  private Robust.Shared.GameObjects.EntityQuery<ThrownItemComponent> _thrownItemQuery;

  public override void Initialize()
  {
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this._thrownItemQuery = this.GetEntityQuery<ThrownItemComponent>();
    this.SubscribeAllEvent<XenoLungePredictedHitEvent>(new EntitySessionEventHandler<XenoLungePredictedHitEvent>(this.OnPredictedHit));
    this.SubscribeLocalEvent<XenoLungeComponent, XenoLungeActionEvent>(new EntityEventRefHandler<XenoLungeComponent, XenoLungeActionEvent>(this.OnXenoLungeAction));
    this.SubscribeLocalEvent<XenoLungeComponent, MeleeAttackAttemptEvent>(new EntityEventRefHandler<XenoLungeComponent, MeleeAttackAttemptEvent>(this.OnAttackAttempt));
    this.SubscribeLocalEvent<XenoActiveLungeComponent, ThrowDoHitEvent>(new EntityEventRefHandler<XenoActiveLungeComponent, ThrowDoHitEvent>(this.OnXenoLungeHit));
    this.SubscribeLocalEvent<XenoActiveLungeComponent, LandEvent>(new EntityEventRefHandler<XenoActiveLungeComponent, LandEvent>(this.OnXenoLungeLand));
    this.SubscribeLocalEvent<RMCLungeProtectionComponent, XenoLungeHitAttempt>(new EntityEventRefHandler<RMCLungeProtectionComponent, XenoLungeHitAttempt>(this.OnXenoLungeHitAttempt));
    this.SubscribeLocalEvent<XenoLungeStunnedComponent, PullStoppedMessage>(new EntityEventRefHandler<XenoLungeStunnedComponent, PullStoppedMessage>(this.OnXenoLungeStunnedPullStopped));
  }

  private void OnPredictedHit(XenoLungePredictedHitEvent msg, EntitySessionEventArgs args)
  {
    if (this._net.IsClient)
      return;
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    XenoActiveLungeComponent comp;
    if (!this.TryComp<XenoActiveLungeComponent>(valueOrDefault, out comp))
      return;
    EntityUid entity = this.GetEntity(msg.Target);
    if (!entity.Valid || !comp.Running || comp.Target != entity)
      return;
    this._rmcLagCompensation.SetLastRealTick(args.SenderSession.UserId, msg.LastRealTick);
    this.ApplyLungeHitEffects((Entity<XenoActiveLungeComponent>) (valueOrDefault, comp), entity, true, false);
  }

  private void OnXenoLungeAction(Entity<XenoLungeComponent> xeno, ref XenoLungeActionEvent args)
  {
    EntityUid? entity = args.Entity;
    if (!entity.HasValue)
      return;
    EntityUid valueOrDefault = entity.GetValueOrDefault();
    if (!this._xeno.CanAbilityAttackTarget((EntityUid) xeno, valueOrDefault) || args.Handled)
      return;
    XenoLungeAttemptEvent args1 = new XenoLungeAttemptEvent();
    this.RaiseLocalEvent<XenoLungeAttemptEvent>((EntityUid) xeno, ref args1);
    if (args1.Cancelled)
      return;
    args.Handled = true;
    this._rmcPulling.TryStopAllPullsFromAndOn((EntityUid) xeno);
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
    EntityCoordinates coordinates = this._rmcLagCompensation.GetCoordinates(valueOrDefault, new EntityUid?((EntityUid) xeno));
    Vector2 direction = Vector2Helpers.Normalized(coordinates.Position - mapCoordinates.Position) * xeno.Comp.Range;
    XenoActiveLungeComponent activeLungeComponent = this.EnsureComp<XenoActiveLungeComponent>((EntityUid) xeno);
    activeLungeComponent.Origin = mapCoordinates;
    activeLungeComponent.Charge = direction;
    activeLungeComponent.Target = valueOrDefault;
    activeLungeComponent.TargetCoordinates = this._transform.ToMapCoordinates(coordinates);
    activeLungeComponent.Range = xeno.Comp.Range;
    activeLungeComponent.StunTime = xeno.Comp.StunTime;
    this.Dirty<XenoLungeComponent>(xeno);
    this._rmcObstacleSlamming.MakeImmune((EntityUid) xeno, 0.5f);
    this._throwing.TryThrow((EntityUid) xeno, direction, 30f, animated: false);
    PhysicsComponent component;
    if (!this._physicsQuery.TryGetComponent((EntityUid) xeno, out component))
      return;
    foreach (EntityUid contactingEntity in this._physics.GetContactingEntities(xeno.Owner, component))
    {
      if (!(contactingEntity != valueOrDefault) && this.ApplyLungeHitEffects((Entity<XenoActiveLungeComponent>) xeno.Owner, contactingEntity, true))
        break;
    }
  }

  private void OnAttackAttempt(Entity<XenoLungeComponent> ent, ref MeleeAttackAttemptEvent args)
  {
    NetEntity netEntity1 = this.GetNetEntity((EntityUid) ent);
    XenoLungeStunnedComponent comp;
    if (!this.TryComp<XenoLungeStunnedComponent>(this.GetEntity(args.Target), out comp))
      return;
    NetEntity netEntity2 = netEntity1;
    NetEntity? stunner = comp.Stunner;
    if ((stunner.HasValue ? (netEntity2 != stunner.GetValueOrDefault() ? 1 : 0) : 1) != 0 || !(args.Attack is DisarmAttackEvent attack))
      return;
    args.Attack = (AttackEvent) new LightAttackEvent(attack.Target, netEntity1, attack.Coordinates);
  }

  private void OnXenoLungeHit(Entity<XenoActiveLungeComponent> xeno, ref ThrowDoHitEvent args)
  {
    if (!this._mobState.IsAlive((EntityUid) xeno) || this.HasComp<StunnedComponent>((EntityUid) xeno))
      this.RemCompDeferred<XenoActiveLungeComponent>((EntityUid) xeno);
    else
      this.ApplyLungeHitEffects(xeno.AsNullable(), args.Target, true);
  }

  private void OnXenoLungeLand(Entity<XenoActiveLungeComponent> ent, ref LandEvent args)
  {
    if (!this._pulling.IsPulling((EntityUid) ent))
      this.ApplyLungeHitEffects(ent.AsNullable(), ent.Comp.Target, false);
    this.RemCompDeferred<XenoActiveLungeComponent>((EntityUid) ent);
  }

  private bool ApplyLungeHitEffects(
    Entity<XenoActiveLungeComponent?> xeno,
    EntityUid targetId,
    bool stopThrow,
    bool predicted = true)
  {
    if (!this.Resolve<XenoActiveLungeComponent>((EntityUid) xeno, ref xeno.Comp, false) || this._mobState.IsDead(targetId))
      return false;
    PhysicsComponent component1;
    ThrownItemComponent component2;
    if (this._physicsQuery.TryGetComponent((EntityUid) xeno, out component1) && this._thrownItemQuery.TryGetComponent((EntityUid) xeno, out component2))
    {
      this._thrownItem.LandComponent((EntityUid) xeno, component2, component1, true);
      if (stopThrow)
        this._thrownItem.StopThrow((EntityUid) xeno, component2);
    }
    XenoLungeHitAttempt args = new XenoLungeHitAttempt((EntityUid) xeno);
    this.RaiseLocalEvent<XenoLungeHitAttempt>(targetId, ref args);
    RMCSizes size;
    XenoComponent comp1;
    if (args.Cancelled || !this._xeno.CanAbilityAttackTarget((EntityUid) xeno, targetId) || this._size.TryGetSize(targetId, out size) && size >= RMCSizes.Big || this.TryComp<XenoComponent>(targetId, out comp1) && comp1.Tier >= 2)
      return true;
    if (this._net.IsServer)
    {
      TimeSpan time = this._xeno.TryApplyXenoDebuffMultiplier(targetId, xeno.Comp.StunTime);
      this._stun.TryParalyze(targetId, time, true);
      XenoLungeStunnedComponent stunnedComponent = this.EnsureComp<XenoLungeStunnedComponent>(targetId);
      stunnedComponent.ExpireAt = this._timing.CurTime + time;
      stunnedComponent.Stunner = new NetEntity?(this.GetNetEntity((EntityUid) xeno));
      this.Dirty(targetId, (IComponent) stunnedComponent);
    }
    MeleeWeaponComponent comp2;
    if (this.TryComp<MeleeWeaponComponent>((EntityUid) xeno, out comp2))
    {
      comp2.NextAttack = this._timing.CurTime;
      this.Dirty((EntityUid) xeno, (IComponent) comp2);
    }
    if (this._net.IsClient & predicted)
    {
      XenoLungePredictedHitEvent predictedHitEvent = new XenoLungePredictedHitEvent(this.GetNetEntity(targetId), this._rmcLagCompensation.GetLastRealTick(new NetUserId?()));
      this.RaiseNetworkEvent((EntityEventArgs) predictedHitEvent);
      if (this._timing.InPrediction && this._timing.IsFirstTimePredicted)
        this.RaisePredictiveEvent<XenoLungePredictedHitEvent>(predictedHitEvent);
    }
    this.StopLunge((EntityUid) xeno);
    this._transform.SetMapCoordinates(targetId, xeno.Comp.TargetCoordinates);
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
    if (xeno.Comp.TargetCoordinates.MapId == mapCoordinates.MapId && !xeno.Comp.TargetCoordinates.InRange(mapCoordinates, 1.25f))
    {
      Vector2 vector2 = xeno.Comp.TargetCoordinates.Position - mapCoordinates.Position;
      float num = vector2.Length();
      MapCoordinates coordinates = mapCoordinates.Offset((num - 1.25f) / num * vector2);
      this._transform.SetMapCoordinates((EntityUid) xeno, coordinates);
    }
    this._pulling.TryStartPull((EntityUid) xeno, targetId);
    this.RemCompDeferred<XenoActiveLungeComponent>((EntityUid) xeno);
    return true;
  }

  private void OnXenoLungeStunnedPullStopped(
    Entity<XenoLungeStunnedComponent> ent,
    ref PullStoppedMessage args)
  {
    if (args.PulledUid != ent.Owner)
      return;
    foreach (ProtoId<StatusEffectPrototype> effect in ent.Comp.Effects)
      this._statusEffects.TryRemoveStatusEffect((EntityUid) ent, (string) effect);
    this.RemCompDeferred<XenoLungeStunnedComponent>(ent.Owner);
  }

  private void OnXenoLungeHitAttempt(
    Entity<RMCLungeProtectionComponent> ent,
    ref XenoLungeHitAttempt args)
  {
    XenoActiveLungeComponent comp;
    if (args.Cancelled || !this.TryComp<XenoActiveLungeComponent>(args.Lunging, out comp))
      return;
    args.Cancelled = this._leap.AttemptBlockLeap(ent.Owner, ent.Comp.StunDuration, ent.Comp.BlockSound, args.Lunging, this._transform.ToCoordinates(comp.Origin), ent.Comp.FullProtection);
  }

  private void StopLunge(EntityUid lunging)
  {
    PhysicsComponent component;
    if (!this._physicsQuery.TryGetComponent(lunging, out component))
      return;
    this._physics.SetLinearVelocity(lunging, Vector2.Zero, body: component);
    this._physics.SetBodyStatus(lunging, component, BodyStatus.OnGround);
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoLungeStunnedComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoLungeStunnedComponent>();
    EntityUid uid;
    XenoLungeStunnedComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(curTime < comp1.ExpireAt))
        this.RemCompDeferred<XenoLungeStunnedComponent>(uid);
    }
  }
}
