// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Headbutt.XenoHeadbuttSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Animation;
using Content.Shared._RMC14.Xenonids.Crest;
using Content.Shared._RMC14.Xenonids.Fortify;
using Content.Shared.Actions;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Headbutt;

public sealed class XenoHeadbuttSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private RMCObstacleSlammingSystem _rmcObstacleSlamming;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private RMCSizeStunSystem _sizeStun;
  [Dependency]
  private ThrowingSystem _throwing;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private ThrownItemSystem _thrownItem;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private XenoAnimationsSystem _xenoAnimations;
  [Dependency]
  private XenoSystem _xeno;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;
  private Robust.Shared.GameObjects.EntityQuery<ThrownItemComponent> _thrownItemQuery;

  public override void Initialize()
  {
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this._thrownItemQuery = this.GetEntityQuery<ThrownItemComponent>();
    this.SubscribeLocalEvent<XenoHeadbuttComponent, XenoHeadbuttActionEvent>(new EntityEventRefHandler<XenoHeadbuttComponent, XenoHeadbuttActionEvent>(this.OnXenoHeadbuttAction));
    this.SubscribeLocalEvent<XenoHeadbuttComponent, ThrowDoHitEvent>(new EntityEventRefHandler<XenoHeadbuttComponent, ThrowDoHitEvent>(this.OnXenoHeadbuttHit));
  }

  private void OnXenoHeadbuttAction(
    Entity<XenoHeadbuttComponent> xeno,
    ref XenoHeadbuttActionEvent args)
  {
    if (args.Handled)
      return;
    XenoCrestComponent comp;
    if (this.TryComp<XenoCrestComponent>((EntityUid) xeno, out comp) && comp.Lowered && !this._interaction.InRangeUnobstructed((Entity<TransformComponent>) xeno.Owner, (Entity<TransformComponent>) args.Target))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-headbutt-too-far"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    }
    else
    {
      XenoHeadbuttAttemptEvent args1 = new XenoHeadbuttAttemptEvent();
      this.RaiseLocalEvent<XenoHeadbuttAttemptEvent>((EntityUid) xeno, ref args1);
      if (args1.Cancelled || !this._rmcActions.TryUseAction((EntityTargetActionEvent) args))
        return;
      this._rmcPulling.TryStopAllPullsFromAndOn((EntityUid) xeno);
      args.Handled = true;
      MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
      Vector2 direction = Vector2Helpers.Normalized(this._transform.GetMapCoordinates(args.Target).Position - mapCoordinates.Position) * xeno.Comp.Range;
      xeno.Comp.Charge = new Vector2?(direction);
      this.Dirty<XenoHeadbuttComponent>(xeno);
      this._rmcObstacleSlamming.MakeImmune((EntityUid) xeno);
      this._throwing.TryThrow((EntityUid) xeno, direction);
    }
  }

  private void OnXenoHeadbuttHit(Entity<XenoHeadbuttComponent> xeno, ref ThrowDoHitEvent args)
  {
    EntityUid target = args.Target;
    if (!this._xeno.CanAbilityAttackTarget((EntityUid) xeno, target))
      return;
    PhysicsComponent component1;
    ThrownItemComponent component2;
    if (this._physicsQuery.TryGetComponent((EntityUid) xeno, out component1) && this._thrownItemQuery.TryGetComponent((EntityUid) xeno, out component2))
    {
      this._thrownItem.LandComponent((EntityUid) xeno, component2, component1, true);
      this._thrownItem.StopThrow((EntityUid) xeno, component2);
    }
    if (this._timing.IsFirstTimePredicted)
    {
      Vector2? charge = xeno.Comp.Charge;
      if (charge.HasValue)
      {
        Vector2 valueOrDefault = charge.GetValueOrDefault();
        xeno.Comp.Charge = new Vector2?();
        this._xenoAnimations.PlayLungeAnimationEvent((EntityUid) xeno, valueOrDefault);
      }
    }
    if (this._net.IsServer)
      this._audio.PlayPvs(xeno.Comp.Sound, (EntityUid) xeno);
    DamageSpecifier damage1 = xeno.Comp.Damage;
    XenoCrestComponent comp1;
    if (this.TryComp<XenoCrestComponent>((EntityUid) xeno, out comp1) && comp1.Lowered)
      damage1.ExclusiveAdd(xeno.Comp.CrestedDamageReduction);
    DamageableSystem damageable = this._damageable;
    EntityUid? uid = new EntityUid?(target);
    DamageSpecifier damage2 = this._xeno.TryApplyXenoSlashDamageMultiplier(target, damage1);
    int ap = xeno.Comp.AP;
    EntityUid? origin = new EntityUid?((EntityUid) xeno);
    EntityUid? tool = new EntityUid?((EntityUid) xeno);
    int armorPiercing = ap;
    FixedPoint2? total = damageable.TryChangeDamage(uid, damage2, origin: origin, tool: tool, armorPiercing: armorPiercing)?.GetTotal();
    FixedPoint2 zero = FixedPoint2.Zero;
    if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
    {
      Filter filter1 = Filter.Pvs(target, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno.Owner));
      SharedColorFlashEffectSystem colorFlash = this._colorFlash;
      Color red = Color.Red;
      List<EntityUid> entities = new List<EntityUid>();
      entities.Add(target);
      Filter filter2 = filter1;
      colorFlash.RaiseEffect(red, entities, filter2);
    }
    XenoCrestComponent comp2;
    XenoFortifyComponent comp3;
    float num = xeno.Comp.ThrowForce + (this.TryComp<XenoCrestComponent>((EntityUid) xeno, out comp2) && comp2.Lowered || this.TryComp<XenoFortifyComponent>((EntityUid) xeno, out comp3) && comp3.Fortified ? xeno.Comp.CrestFortifiedThrowAdd : 0.0f);
    this._rmcPulling.TryStopAllPullsFromAndOn(target);
    this.StopHeadbutt((EntityUid) xeno);
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
    this._sizeStun.KnockBack(target, new MapCoordinates?(mapCoordinates), num, num, 10f, true);
    if (!this._net.IsServer)
      return;
    this.SpawnAttachedTo((string) xeno.Comp.Effect, target.ToCoordinates(), rotation: new Angle());
  }

  private void StopHeadbutt(EntityUid xeno)
  {
    PhysicsComponent component;
    if (!this._physicsQuery.TryGetComponent(xeno, out component))
      return;
    this._physics.SetLinearVelocity(xeno, Vector2.Zero, body: component);
    this._physics.SetBodyStatus(xeno, component, BodyStatus.OnGround);
  }
}
