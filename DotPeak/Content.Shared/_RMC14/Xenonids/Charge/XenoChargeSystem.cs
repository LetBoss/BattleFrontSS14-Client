// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Charge.XenoChargeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Animation;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.HiveLeader;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.CCVar;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Charge;

public sealed class XenoChargeSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private SharedMoverController _moverController;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private SharedRMCDamageableSystem _rmcDamageable;
  [Dependency]
  private SharedRMCEmoteSystem _rmcEmote;
  [Dependency]
  private RMCObstacleSlammingSystem _rmcObstacleSlamming;
  [Dependency]
  private SharedStunSystem _stun;
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
  [Dependency]
  private SharedXenoHiveSystem _xenoHive;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private SharedDestructibleSystem _destruct;
  [Dependency]
  private RMCSizeStunSystem _sizeStun;
  private readonly ProtoId<DamageTypePrototype> _blunt = (ProtoId<DamageTypePrototype>) "Blunt";
  private Robust.Shared.GameObjects.EntityQuery<InputMoverComponent> _inputMoverQuery;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;
  private Robust.Shared.GameObjects.EntityQuery<ThrownItemComponent> _thrownItemQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoToggleChargingComponent> _xenoToggleChargingQuery;
  private Robust.Shared.GameObjects.EntityQuery<ActiveXenoToggleChargingComponent> _activeXenoToggleChargingQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoToggleChargingRecentlyHitComponent> _xenoToggleChargingRecentlyHitQuery;
  private bool _relativeMovement;
  private readonly HashSet<(Entity<ActiveXenoToggleChargingComponent> Crusher, EntityUid Target)> _hit = new HashSet<(Entity<ActiveXenoToggleChargingComponent>, EntityUid)>();

  public override void Initialize()
  {
    this._inputMoverQuery = this.GetEntityQuery<InputMoverComponent>();
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this._thrownItemQuery = this.GetEntityQuery<ThrownItemComponent>();
    this._xenoToggleChargingQuery = this.GetEntityQuery<XenoToggleChargingComponent>();
    this._activeXenoToggleChargingQuery = this.GetEntityQuery<ActiveXenoToggleChargingComponent>();
    this._xenoToggleChargingRecentlyHitQuery = this.GetEntityQuery<XenoToggleChargingRecentlyHitComponent>();
    this.SubscribeLocalEvent<XenoChargeComponent, XenoChargeActionEvent>(new EntityEventRefHandler<XenoChargeComponent, XenoChargeActionEvent>(this.OnXenoChargeAction));
    this.SubscribeLocalEvent<XenoChargeComponent, ThrowDoHitEvent>(new EntityEventRefHandler<XenoChargeComponent, ThrowDoHitEvent>(this.OnXenoChargeHit));
    this.SubscribeLocalEvent<XenoChargeComponent, XenoChargeDoAfterEvent>(new EntityEventRefHandler<XenoChargeComponent, XenoChargeDoAfterEvent>(this.OnXenoChargeDoAfterEvent));
    this.SubscribeLocalEvent<XenoChargeComponent, StopThrowEvent>(new EntityEventRefHandler<XenoChargeComponent, StopThrowEvent>(this.OnXenoChargeStop));
    this.SubscribeLocalEvent<XenoToggleChargingComponent, XenoToggleChargingActionEvent>(new EntityEventRefHandler<XenoToggleChargingComponent, XenoToggleChargingActionEvent>(this.OnXenoToggleChargingAction));
    this.SubscribeLocalEvent<ActiveXenoToggleChargingComponent, MapInitEvent>(new EntityEventRefHandler<ActiveXenoToggleChargingComponent, MapInitEvent>(this.OnActiveToggleChargingMapInit));
    this.SubscribeLocalEvent<ActiveXenoToggleChargingComponent, ComponentRemove>(new EntityEventRefHandler<ActiveXenoToggleChargingComponent, ComponentRemove>(this.OnActiveToggleChargingRemove));
    this.SubscribeLocalEvent<ActiveXenoToggleChargingComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<ActiveXenoToggleChargingComponent, RefreshMovementSpeedModifiersEvent>(this.OnActiveToggleChargingSpeed));
    this.SubscribeLocalEvent<ActiveXenoToggleChargingComponent, MoveInputEvent>(new EntityEventRefHandler<ActiveXenoToggleChargingComponent, MoveInputEvent>(this.OnActiveToggleChargingMoveInput));
    this.SubscribeLocalEvent<ActiveXenoToggleChargingComponent, MoveEvent>(new EntityEventRefHandler<ActiveXenoToggleChargingComponent, MoveEvent>(this.OnActiveToggleChargingMove));
    this.SubscribeLocalEvent<ActiveXenoToggleChargingComponent, StartCollideEvent>(new EntityEventRefHandler<ActiveXenoToggleChargingComponent, StartCollideEvent>(this.OnActiveToggleChargingCollide));
    this.SubscribeLocalEvent<ActiveXenoToggleChargingComponent, MobStateChangedEvent>(new EntityEventRefHandler<ActiveXenoToggleChargingComponent, MobStateChangedEvent>(this.OnActiveToggleChargingMobStateChanged));
    this.SubscribeLocalEvent<XenoToggleChargingDamageComponent, XenoToggleChargingCollideEvent>(new EntityEventRefHandler<XenoToggleChargingDamageComponent, XenoToggleChargingCollideEvent>(this.OnChargingDamageCollide));
    this.SubscribeLocalEvent<XenoToggleChargingKnockbackComponent, XenoToggleChargingCollideEvent>(new EntityEventRefHandler<XenoToggleChargingKnockbackComponent, XenoToggleChargingCollideEvent>(this.OnChargingKnockbackCollide));
    this.SubscribeLocalEvent<XenoToggleChargingKnockbackComponent, AttemptMobTargetCollideEvent>(new EntityEventRefHandler<XenoToggleChargingKnockbackComponent, AttemptMobTargetCollideEvent>(this.OnChargingKnockbackAttemptCollide));
    this.SubscribeLocalEvent<XenoToggleChargingParalyzeComponent, XenoToggleChargingCollideEvent>(new EntityEventRefHandler<XenoToggleChargingParalyzeComponent, XenoToggleChargingCollideEvent>(this.OnChargingParalyzeCollide));
    this.SubscribeLocalEvent<XenoToggleChargingStopComponent, XenoToggleChargingCollideEvent>(new EntityEventRefHandler<XenoToggleChargingStopComponent, XenoToggleChargingCollideEvent>(this.OnChargingStopCollide));
    this.SubscribeLocalEvent<HiveLeaderComponent, XenoToggleChargingCollideEvent>(new EntityEventRefHandler<HiveLeaderComponent, XenoToggleChargingCollideEvent>(this.OnLeaderCollide));
    this.Subs.CVar<bool>(this._config, CCVars.RelativeMovement, (Action<bool>) (v => this._relativeMovement = v), true);
  }

  private void OnChargingDamageCollide(
    Entity<XenoToggleChargingDamageComponent> damage,
    ref XenoToggleChargingCollideEvent args)
  {
    args.Handled = true;
    Entity<ActiveXenoToggleChargingComponent> charger = args.Charger;
    if (charger.Comp.Stage < damage.Comp.MinimumStage)
      return;
    if (this._net.IsServer)
      this._audio.PlayPvs(damage.Comp.Sound, this._transform.GetMoverCoordinates((EntityUid) damage));
    DamageableComponent damageable1 = this.CompOrNull<DamageableComponent>((EntityUid) damage);
    if (damage.Comp.Destroy)
    {
      if (this._net.IsServer)
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-charge-plow-through", ("xeno", (object) charger), ("target", (object) damage)), (EntityUid) damage, PopupType.SmallCaution);
      if (this._net.IsClient)
        this._transform.DetachEntity((EntityUid) damage, this.Transform((EntityUid) damage));
      else
        this.QueueDel(new EntityUid?((EntityUid) damage));
    }
    else
    {
      if (this._net.IsServer)
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-charge-smashes", ("xeno", (object) charger), ("target", (object) damage)), (EntityUid) damage, PopupType.SmallCaution);
      int key = charger.Comp.Stage;
      int num;
      if (damage.Comp.StageMultipliers != null && damage.Comp.StageMultipliers.TryGetValue(key, out num))
        key = num * num;
      else if (damage.Comp.DefaultMultiplier != 0)
        key = damage.Comp.DefaultMultiplier * damage.Comp.DefaultMultiplier;
      else if (key < damage.Comp.MinimumStage)
        key = damage.Comp.MinimumStage;
      if (damage.Comp.Damage != null)
        this._damageable.TryChangeDamage(new EntityUid?((EntityUid) damage), damage.Comp.Damage * (float) key, damageable: damageable1);
      if (damage.Comp.ArmorPiercingDamage != null)
      {
        DamageableSystem damageable2 = this._damageable;
        EntityUid? uid = new EntityUid?((EntityUid) damage);
        DamageSpecifier damage1 = damage.Comp.ArmorPiercingDamage * (float) key;
        DamageableComponent damageable3 = damageable1;
        int armorPiercing1 = damage.Comp.ArmorPiercing;
        EntityUid? origin = new EntityUid?();
        EntityUid? tool = new EntityUid?();
        int armorPiercing2 = armorPiercing1;
        damageable2.TryChangeDamage(uid, damage1, damageable: damageable3, origin: origin, tool: tool, armorPiercing: armorPiercing2);
      }
      FixedPoint2? destroyed;
      if (damage.Comp.PercentageDamage > FixedPoint2.Zero && this._rmcDamageable.TryGetDestroyedAt((EntityUid) charger, out destroyed))
        this._damageable.TryChangeDamage(new EntityUid?((EntityUid) damage), new DamageSpecifier()
        {
          DamageDict = {
            [(string) this._blunt] = destroyed.Value * damage.Comp.PercentageDamage * key
          }
        }, damageable: damageable1);
    }
    if (this._net.IsClient && damageable1 != null && damage.Comp.DestroyDamage > FixedPoint2.Zero && damageable1.TotalDamage >= damage.Comp.DestroyDamage)
      this._transform.DetachEntity((EntityUid) damage, this.Transform((EntityUid) damage));
    if (damage.Comp.Unanchor && !this.TerminatingOrDeleted((EntityUid) damage) && !this.EntityManager.IsQueuedForDeletion((EntityUid) damage))
      this._transform.Unanchor((EntityUid) damage);
    if (damage.Comp.Stop)
    {
      this.ResetCharging(charger, false);
    }
    else
    {
      if (damage.Comp.StageLoss <= 0)
        return;
      this.IncrementStages(charger, -damage.Comp.StageLoss);
    }
  }

  private void OnChargingKnockbackCollide(
    Entity<XenoToggleChargingKnockbackComponent> ent,
    ref XenoToggleChargingCollideEvent args)
  {
    args.Handled = true;
    TransformComponent comp;
    if (this.TryComp((EntityUid) ent, out comp) && comp.Anchored)
      this.ResetStage(args.Charger);
    else if (!ent.Comp.Enabled || args.Charger.Comp.Stage == 0)
    {
      this.ResetStage(args.Charger);
    }
    else
    {
      this._rmcPulling.TryStopAllPullsFromAndOn((EntityUid) ent);
      if (this._xenoHive.FromSameHive((Entity<HiveMemberComponent>) ent.Owner, (Entity<HiveMemberComponent>) args.Charger.Owner))
        this._rmcObstacleSlamming.MakeImmune((EntityUid) ent);
      DirectionFlag direction1 = args.Charger.Comp.Direction;
      if (direction1 == null)
        return;
      (Direction First, Direction Second) perpendiculars = DirectionExtensions.AsDir(direction1).GetPerpendiculars();
      Vector2 direction2 = Vector2Helpers.Normalized(DirectionExtensions.ToVec(this._random.Prob(0.5f) ? perpendiculars.First : perpendiculars.Second));
      this._throwing.TryThrow((EntityUid) ent, direction2, compensateFriction: true);
      this.IncrementStages(args.Charger, -1);
      if (!this._net.IsServer)
        return;
      this._audio.PlayPvs(ent.Comp.Sound, (EntityUid) ent);
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-charge-knockback-others", ("user", (object) args.Charger), ("target", (object) ent)), (EntityUid) ent, PopupType.MediumCaution);
    }
  }

  private void OnChargingKnockbackAttemptCollide(
    Entity<XenoToggleChargingKnockbackComponent> ent,
    ref AttemptMobTargetCollideEvent args)
  {
    ActiveXenoToggleChargingComponent component;
    if (!this._activeXenoToggleChargingQuery.TryComp(args.Entity, out component) || component.Stage <= 0)
      return;
    args.Cancelled = true;
  }

  private void OnChargingParalyzeCollide(
    Entity<XenoToggleChargingParalyzeComponent> ent,
    ref XenoToggleChargingCollideEvent args)
  {
    args.Handled = true;
    int stage = args.Charger.Comp.Stage;
    XenoToggleChargingComponent component;
    if (stage <= 0 || !this._xenoToggleChargingQuery.TryComp((EntityUid) args.Charger, out component))
      return;
    TimeSpan time = stage >= component.MaxStage ? ent.Comp.MaxStageDuration : ent.Comp.Duration;
    this._stun.TryParalyze((EntityUid) ent, time, false);
  }

  private void OnChargingStopCollide(
    Entity<XenoToggleChargingStopComponent> ent,
    ref XenoToggleChargingCollideEvent args)
  {
    args.Handled = true;
    this.ResetStage(args.Charger);
  }

  private void OnLeaderCollide(
    Entity<HiveLeaderComponent> ent,
    ref XenoToggleChargingCollideEvent args)
  {
    args.Handled = true;
    this.ResetStage(args.Charger);
  }

  private void OnXenoChargeAction(Entity<XenoChargeComponent> xeno, ref XenoChargeActionEvent args)
  {
    if (args.Handled)
      return;
    XenoChargeAttemptEvent args1 = new XenoChargeAttemptEvent();
    this.RaiseLocalEvent<XenoChargeAttemptEvent>((EntityUid) xeno, ref args1);
    if (args1.Cancelled || !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.PlasmaCost))
      return;
    args.Handled = true;
    XenoChargeDoAfterEvent @event = new XenoChargeDoAfterEvent(this.GetNetCoordinates(args.Target));
    DoAfterArgs args2 = new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, xeno.Comp.ChargeDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) xeno))
    {
      BreakOnMove = true,
      Hidden = true
    };
    this._stun.TrySlowdown((EntityUid) xeno, TimeSpan.FromSeconds(1.75), false, 0.0f, 0.0f);
    this._doAfter.TryStartDoAfter(args2);
  }

  private void StopCrusherCharge(Entity<XenoChargeComponent> xeno)
  {
    PhysicsComponent component1;
    ThrownItemComponent component2;
    if (this._physicsQuery.TryGetComponent((EntityUid) xeno, out component1) && this._thrownItemQuery.TryGetComponent((EntityUid) xeno, out component2))
    {
      this._thrownItem.LandComponent((EntityUid) xeno, component2, component1, true);
      this._thrownItem.StopThrow((EntityUid) xeno, component2);
    }
    if (!this._timing.IsFirstTimePredicted)
      return;
    Vector2? charge = xeno.Comp.Charge;
    if (!charge.HasValue)
      return;
    Vector2 valueOrDefault = charge.GetValueOrDefault();
    xeno.Comp.Charge = new Vector2?();
    this._xenoAnimations.PlayLungeAnimationEvent((EntityUid) xeno, valueOrDefault);
  }

  private void OnXenoChargeHit(Entity<XenoChargeComponent> xeno, ref ThrowDoHitEvent args)
  {
    EntityUid target = args.Target;
    if (this._mobState.IsDead(target))
      return;
    this.StopCrusherCharge(xeno);
    XenoCrusherChargableComponent comp1 = (XenoCrusherChargableComponent) null;
    bool flag = false;
    if (!this._xeno.CanAbilityAttackTarget((EntityUid) xeno, target) && !this.TryComp<XenoCrusherChargableComponent>(target, out comp1))
      return;
    if (this._net.IsServer)
      this._audio.PlayPvs(xeno.Comp.Sound, (EntityUid) xeno);
    DamageSpecifier baseDamage = xeno.Comp.Damage;
    if (comp1 != null)
    {
      if (comp1.SetDamage != null)
        baseDamage = comp1.SetDamage;
      if (comp1.InstantDestroy)
      {
        if (this._net.IsClient & flag)
        {
          this._transform.DetachEntity(target, this.Transform(target));
          return;
        }
        if (!this._net.IsServer)
          return;
        this._destruct.DestroyEntity(target);
        return;
      }
    }
    DamageSpecifier damageSpecifier = this._damageable.TryChangeDamage(new EntityUid?(target), this._xeno.TryApplyXenoSlashDamageMultiplier(target, baseDamage), origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno));
    FixedPoint2? total = damageSpecifier?.GetTotal();
    FixedPoint2 zero1 = FixedPoint2.Zero;
    if ((total.HasValue ? (total.GetValueOrDefault() > zero1 ? 1 : 0) : 0) != 0)
    {
      Filter filter1 = Filter.Pvs(target, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno.Owner));
      SharedColorFlashEffectSystem colorFlash = this._colorFlash;
      Color red = Color.Red;
      List<EntityUid> entities = new List<EntityUid>();
      entities.Add(target);
      Filter filter2 = filter1;
      colorFlash.RaiseEffect(red, entities, filter2);
    }
    DamageableComponent comp2;
    if (comp1 != null && comp1.DestroyDamage.HasValue && this.TryComp<DamageableComponent>(target, out comp2) && damageSpecifier != null && comp1.PassOnDestroy)
    {
      FixedPoint2? destroyDamage = comp1.DestroyDamage;
      FixedPoint2 zero2 = FixedPoint2.Zero;
      if ((destroyDamage.HasValue ? (destroyDamage.GetValueOrDefault() > zero2 ? 1 : 0) : 0) != 0)
      {
        FixedPoint2 totalDamage = comp2.TotalDamage;
        destroyDamage = comp1.DestroyDamage;
        if ((destroyDamage.HasValue ? (totalDamage >= destroyDamage.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        {
          if (this._net.IsClient)
            this._transform.DetachEntity(target, this.Transform(target));
        }
      }
    }
    double range = (double) xeno.Comp.Range;
    if (comp1 != null && comp1.ThrowRange.HasValue)
    {
      double num = (double) comp1.ThrowRange.Value;
    }
    this._rmcPulling.TryStopAllPullsFromAndOn(target);
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
    this._stun.TryParalyze(target, xeno.Comp.StunTime, true);
    this._sizeStun.KnockBack(target, new MapCoordinates?(mapCoordinates), 2f, 2f, 10f);
  }

  private void OnXenoChargeDoAfterEvent(
    Entity<XenoChargeComponent> xeno,
    ref XenoChargeDoAfterEvent args)
  {
    if (args.Cancelled)
      return;
    this._rmcPulling.TryStopAllPullsFromAndOn((EntityUid) xeno);
    EntityCoordinates coordinates = this.GetCoordinates(args.Coordinates);
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
    Vector2 direction = Vector2Helpers.Normalized(this._transform.ToMapCoordinates(coordinates).Position - mapCoordinates.Position) * xeno.Comp.Range;
    xeno.Comp.Charge = new Vector2?(direction);
    this.Dirty<XenoChargeComponent>(xeno);
    this._rmcObstacleSlamming.MakeImmune((EntityUid) xeno);
    this._throwing.TryThrow((EntityUid) xeno, direction, xeno.Comp.Strength, animated: false);
  }

  private void OnXenoChargeStop(Entity<XenoChargeComponent> xeno, ref StopThrowEvent args)
  {
    if (!xeno.Comp.Charge.HasValue)
      return;
    foreach (Entity<MobStateComponent> entity in this._lookup.GetEntitiesInRange<MobStateComponent>(this._transform.GetMapCoordinates((EntityUid) xeno), xeno.Comp.SlowRange))
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) entity))
        this._slow.TrySlowdown((EntityUid) entity, xeno.Comp.SlowTime, ignoreDurationModifier: true);
    }
  }

  private void OnXenoToggleChargingAction(
    Entity<XenoToggleChargingComponent> ent,
    ref XenoToggleChargingActionEvent args)
  {
    InputMoverComponent comp;
    if (this._timing.ApplyingState || this.RemComp<ActiveXenoToggleChargingComponent>((EntityUid) ent) || !this.TryComp<InputMoverComponent>((EntityUid) ent, out comp))
      return;
    DirectionFlag heldButton = this.GetHeldButton((EntityUid) ent, comp.HeldMoveButtons);
    ActiveXenoToggleChargingComponent component = new ActiveXenoToggleChargingComponent();
    this.AddComp<ActiveXenoToggleChargingComponent>((EntityUid) ent, component, true);
    if ((heldButton & (int) (sbyte) (heldButton - 1)) != null)
      return;
    component.Direction = heldButton;
    this.Dirty((EntityUid) ent, (IComponent) component);
  }

  private void OnActiveToggleChargingMapInit(
    Entity<ActiveXenoToggleChargingComponent> ent,
    ref MapInitEvent args)
  {
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoToggleChargingActionEvent>((EntityUid) ent))
      this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) ((EntityUid) entity, (ActionComponent) entity)), true);
  }

  private void OnActiveToggleChargingRemove(
    Entity<ActiveXenoToggleChargingComponent> ent,
    ref ComponentRemove args)
  {
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoToggleChargingActionEvent>((EntityUid) ent))
      this._actions.SetToggled(new Entity<ActionComponent>?((Entity<ActionComponent>) ((EntityUid) entity, (ActionComponent) entity)), false);
  }

  private void OnActiveToggleChargingSpeed(
    Entity<ActiveXenoToggleChargingComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    XenoToggleChargingComponent component;
    if (ent.Comp.Stage == 0 || !this._xenoToggleChargingQuery.TryComp((EntityUid) ent, out component))
      return;
    float num = (float) (1.0 + (double) ent.Comp.Stage * (double) component.SpeedPerStage);
    args.ModifySpeed(num, num);
  }

  private void OnActiveToggleChargingMoveInput(
    Entity<ActiveXenoToggleChargingComponent> ent,
    ref MoveInputEvent args)
  {
    DirectionFlag heldButton = this.GetHeldButton((EntityUid) ent, args.Entity.Comp.HeldMoveButtons & MoveButtons.AnyDirection);
    if (heldButton != null && (ent.Comp.Direction & heldButton) == heldButton && (heldButton & (int) (sbyte) (heldButton - 1)) == null)
      return;
    if (ent.Comp.Direction != null)
    {
      (Direction First, Direction Second) perpendiculars = DirectionExtensions.AsDir(ent.Comp.Direction).GetPerpendiculars();
      if ((ent.Comp.Direction == DirectionExtensions.AsFlag(perpendiculars.First) ? 1 : (ent.Comp.Direction == DirectionExtensions.AsFlag(perpendiculars.Second) ? 1 : 0)) != 0 && (ent.Comp.Deviated == null || ent.Comp.Deviated == heldButton))
      {
        ent.Comp.Deviated = heldButton;
        return;
      }
    }
    this.ResetCharging(ent);
    ent.Comp.Direction = heldButton;
  }

  private void OnActiveToggleChargingMove(
    Entity<ActiveXenoToggleChargingComponent> ent,
    ref MoveEvent args)
  {
    XenoToggleChargingComponent component1;
    float distance;
    if (!this._xenoToggleChargingQuery.TryComp((EntityUid) ent, out component1) || this._rmcPulling.IsBeingPulled((Entity<PullableComponent>) ent.Owner, out EntityUid _) || !args.OldPosition.TryDistance((IEntityManager) this.EntityManager, this._transform, args.NewPosition, out distance))
      return;
    float num = Math.Abs(distance);
    ent.Comp.Distance += num;
    ent.Comp.LastMovedAt = this._timing.CurTime;
    this.Dirty<ActiveXenoToggleChargingComponent>(ent);
    InputMoverComponent component2;
    if (this._inputMoverQuery.TryComp((EntityUid) ent, out component2))
    {
      Angle? relativeRotation1 = ent.Comp.LastRelativeRotation;
      ent.Comp.LastRelativeRotation = new Angle?(component2.RelativeRotation);
      Angle? relativeRotation2 = ent.Comp.LastRelativeRotation;
      Angle? nullable = relativeRotation1;
      if ((relativeRotation2.HasValue == nullable.HasValue ? (relativeRotation2.HasValue ? (Angle.op_Inequality(relativeRotation2.GetValueOrDefault(), nullable.GetValueOrDefault()) ? 1 : 0) : 0) : 1) != 0)
      {
        this.ResetStage(ent);
        return;
      }
    }
    if (ent.Comp.Deviated != null)
    {
      ent.Comp.DeviatedDistance += num;
      if ((double) ent.Comp.DeviatedDistance >= (double) component1.MaxDeviation)
      {
        this.ResetCharging(ent);
        return;
      }
    }
    if ((double) ent.Comp.Distance < (double) component1.StepIncrement)
      return;
    ent.Comp.Steps += component1.StepIncrement;
    ent.Comp.Distance -= component1.StepIncrement;
    if ((double) ent.Comp.Steps < (double) component1.MinimumSteps)
      return;
    if (!this._xenoPlasma.TryRemovePlasma((Entity<XenoPlasmaComponent>) ent.Owner, component1.PlasmaPerStep))
    {
      this.ResetCharging(ent, false);
    }
    else
    {
      this._rmcPulling.TryStopAllPullsFromAndOn((EntityUid) ent);
      if (ent.Comp.Stage == component1.MaxStage - 1)
      {
        ProtoId<EmotePrototype>? emote = component1.Emote;
        if (emote.HasValue)
        {
          ProtoId<EmotePrototype> valueOrDefault = emote.GetValueOrDefault();
          this._rmcEmote.TryEmoteWithChat((EntityUid) ent, valueOrDefault, cooldown: component1.EmoteCooldown);
        }
      }
      ent.Comp.Stage = Math.Min(component1.MaxStage, ent.Comp.Stage + 1);
      ent.Comp.SoundSteps += component1.StepIncrement;
      if (ent.Comp.Stage == 1 || (double) ent.Comp.SoundSteps >= (double) component1.SoundEvery)
      {
        ent.Comp.SoundSteps = 0.0f;
        if (this._timing.InSimulation)
          this._audio.PlayPredicted(component1.Sound, (EntityUid) ent, new EntityUid?((EntityUid) ent));
      }
      this.Dirty<ActiveXenoToggleChargingComponent>(ent);
      this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
    }
  }

  private void OnActiveToggleChargingCollide(
    Entity<ActiveXenoToggleChargingComponent> ent,
    ref StartCollideEvent args)
  {
    if ((double) Math.Abs(ent.Comp.Steps - 1f) < 0.001)
      return;
    this._hit.Add((ent, args.OtherEntity));
  }

  private void OnActiveToggleChargingMobStateChanged(
    Entity<ActiveXenoToggleChargingComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState == MobState.Alive)
      return;
    this.ResetCharging(ent);
    if (this._timing.ApplyingState)
      return;
    this.RemComp<ActiveXenoToggleChargingComponent>((EntityUid) ent);
  }

  private void ResetCharging(Entity<ActiveXenoToggleChargingComponent> xeno, bool resetInput = true)
  {
    this.ResetStage(xeno);
    xeno.Comp.DeviatedDistance = 0.0f;
    if (resetInput)
      xeno.Comp.Direction = (DirectionFlag) 0;
    this.Dirty<ActiveXenoToggleChargingComponent>(xeno);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
  }

  private void ResetStage(Entity<ActiveXenoToggleChargingComponent> xeno)
  {
    xeno.Comp.Steps = 0.0f;
    xeno.Comp.SoundSteps = 0.0f;
    xeno.Comp.Stage = 0;
    this.Dirty<ActiveXenoToggleChargingComponent>(xeno);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
  }

  private void IncrementStages(Entity<ActiveXenoToggleChargingComponent> ent, int increment)
  {
    ent.Comp.Stage = Math.Max(0, ent.Comp.Stage + increment);
    XenoToggleChargingComponent component;
    if (this._xenoToggleChargingQuery.TryComp((EntityUid) ent, out component))
      ent.Comp.Stage = Math.Min(component.MaxStage, ent.Comp.Stage);
    this.Dirty<ActiveXenoToggleChargingComponent>(ent);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
  }

  private DirectionFlag GetHeldButton(EntityUid mover, MoveButtons button)
  {
    InputMoverComponent comp;
    if (!this.TryComp<InputMoverComponent>(mover, out comp))
      return (DirectionFlag) 0;
    Angle parentGridAngle = this._moverController.GetParentGridAngle(comp);
    Vector2 vector2 = this._moverController.DirVecForButtons(button);
    return DirectionExtensions.AsFlag(DirectionExtensions.GetDir(this._relativeMovement ? ((Angle) ref parentGridAngle).RotateVec(ref vector2) : vector2));
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    try
    {
      foreach ((Entity<ActiveXenoToggleChargingComponent> Crusher, EntityUid Target) tuple in this._hit)
      {
        if (!this.TerminatingOrDeleted((EntityUid) tuple.Crusher) && !this.TerminatingOrDeleted(tuple.Target))
        {
          XenoToggleChargingRecentlyHitComponent component;
          if (this._xenoToggleChargingRecentlyHitQuery.TryComp(tuple.Target, out component) && curTime < component.LastHitAt + component.Cooldown)
            return;
          XenoToggleChargingCollideEvent args = new XenoToggleChargingCollideEvent(tuple.Crusher);
          this.RaiseLocalEvent<XenoToggleChargingCollideEvent>(tuple.Target, ref args);
          if (args.Handled)
          {
            component = this.EnsureComp<XenoToggleChargingRecentlyHitComponent>(tuple.Target);
            component.LastHitAt = curTime;
            this.Dirty(tuple.Target, (IComponent) component);
            if (tuple.Crusher.Comp.Stage == 0)
            {
              tuple.Crusher.Comp.Steps = 0.0f;
              this.Dirty<ActiveXenoToggleChargingComponent>(tuple.Crusher);
            }
          }
        }
      }
    }
    finally
    {
      this._hit.Clear();
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActiveXenoToggleChargingComponent, XenoToggleChargingComponent, PhysicsComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActiveXenoToggleChargingComponent, XenoToggleChargingComponent, PhysicsComponent>();
    EntityUid uid;
    ActiveXenoToggleChargingComponent comp1;
    XenoToggleChargingComponent comp2;
    PhysicsComponent comp3;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2, out comp3))
    {
      if (comp3.BodyStatus == BodyStatus.InAir)
        this.ResetCharging((Entity<ActiveXenoToggleChargingComponent>) (uid, comp1));
      else if (curTime >= comp1.LastMovedAt + comp2.LastMovedGrace)
        this.ResetCharging((Entity<ActiveXenoToggleChargingComponent>) (uid, comp1), false);
    }
  }
}
