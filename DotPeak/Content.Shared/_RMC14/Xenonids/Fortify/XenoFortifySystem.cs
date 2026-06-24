// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fortify.XenoFortifySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Crest;
using Content.Shared._RMC14.Xenonids.Headbutt;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Explosion;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffectNew;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Fortify;

public sealed class XenoFortifySystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private CMArmorSystem _armor;
  [Dependency]
  private SharedRMCExplosionSystem _explode;
  [Dependency]
  private FixtureSystem _fixtures;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private MovementSpeedModifierSystem _speed;
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoFortifyComponent, XenoFortifyActionEvent>(new EntityEventRefHandler<XenoFortifyComponent, XenoFortifyActionEvent>(this.OnXenoFortifyAction));
    this.SubscribeLocalEvent<XenoFortifyComponent, CMGetArmorEvent>(new EntityEventRefHandler<XenoFortifyComponent, CMGetArmorEvent>(this.OnXenoFortifyGetArmor));
    this.SubscribeLocalEvent<XenoFortifyComponent, BeforeStatusEffectAddedEvent>(new EntityEventRefHandler<XenoFortifyComponent, BeforeStatusEffectAddedEvent>(this.OnXenoFortifyBeforeStatusAdded));
    this.SubscribeLocalEvent<XenoFortifyComponent, GetExplosionResistanceEvent>(new EntityEventRefHandler<XenoFortifyComponent, GetExplosionResistanceEvent>(this.OnXenoFortifyGetExplosionResistance));
    this.SubscribeLocalEvent<XenoFortifyComponent, ChangeDirectionAttemptEvent>(new EntityEventRefHandler<XenoFortifyComponent, ChangeDirectionAttemptEvent>(this.OnXenoFortifyCancel<ChangeDirectionAttemptEvent>));
    this.SubscribeLocalEvent<XenoFortifyComponent, UpdateCanMoveEvent>(new EntityEventRefHandler<XenoFortifyComponent, UpdateCanMoveEvent>(this.OnXenoFortifyCancel<UpdateCanMoveEvent>));
    this.SubscribeLocalEvent<XenoFortifyComponent, AttackAttemptEvent>(new EntityEventRefHandler<XenoFortifyComponent, AttackAttemptEvent>(this.OnXenoFortifyAttack));
    this.SubscribeLocalEvent<XenoFortifyComponent, XenoHeadbuttAttemptEvent>(new EntityEventRefHandler<XenoFortifyComponent, XenoHeadbuttAttemptEvent>(this.OnXenoFortifyHeadbuttAttempt));
    this.SubscribeLocalEvent<XenoFortifyComponent, XenoRestAttemptEvent>(new EntityEventRefHandler<XenoFortifyComponent, XenoRestAttemptEvent>(this.OnXenoFortifyRestAttempt));
    this.SubscribeLocalEvent<XenoFortifyComponent, XenoTailSweepAttemptEvent>(new EntityEventRefHandler<XenoFortifyComponent, XenoTailSweepAttemptEvent>(this.OnXenoFortifyTailSweepAttempt));
    this.SubscribeLocalEvent<XenoFortifyComponent, XenoToggleCrestAttemptEvent>(new EntityEventRefHandler<XenoFortifyComponent, XenoToggleCrestAttemptEvent>(this.OnXenoFortifyToggleCrestAttempt));
    this.SubscribeLocalEvent<XenoFortifyComponent, MobStateChangedEvent>(new EntityEventRefHandler<XenoFortifyComponent, MobStateChangedEvent>(this.OnXenoFortifyMobStateChanged));
    this.SubscribeLocalEvent<XenoFortifyComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<XenoFortifyComponent, RefreshMovementSpeedModifiersEvent>(this.OnXenoFortifyRefreshSpeed));
    this.SubscribeLocalEvent<XenoFortifyComponent, GetMeleeDamageEvent>(new EntityEventRefHandler<XenoFortifyComponent, GetMeleeDamageEvent>(this.OnXenoFortifyGetMeleeDamage));
  }

  private void OnXenoFortifyAction(
    Entity<XenoFortifyComponent> xeno,
    ref XenoFortifyActionEvent args)
  {
    if (args.Handled)
      return;
    XenoFortifyAttemptEvent args1 = new XenoFortifyAttemptEvent();
    this.RaiseLocalEvent<XenoFortifyAttemptEvent>((EntityUid) xeno, ref args1);
    if (args1.Cancelled)
      return;
    args.Handled = true;
    this._audio.PlayPredicted(xeno.Comp.FortifySound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    if (xeno.Comp.Fortified)
      this.Unfortify(xeno);
    else
      this.Fortify(xeno);
  }

  private void OnXenoFortifyGetArmor(Entity<XenoFortifyComponent> xeno, ref CMGetArmorEvent args)
  {
    if (!xeno.Comp.Fortified)
      return;
    args.XenoArmor += xeno.Comp.Armor;
    args.FrontalArmor += xeno.Comp.FrontalArmor;
  }

  private void OnXenoFortifyBeforeStatusAdded(
    Entity<XenoFortifyComponent> xeno,
    ref BeforeStatusEffectAddedEvent args)
  {
    if (!xeno.Comp.Fortified || !((IEnumerable<string>) xeno.Comp.ImmuneToStatuses).Contains<string>(args.Effect.Id))
      return;
    args.Cancelled = true;
  }

  private void OnXenoFortifyGetExplosionResistance(
    Entity<XenoFortifyComponent> xeno,
    ref GetExplosionResistanceEvent args)
  {
    if (!xeno.Comp.Fortified)
      return;
    int explosionArmor = xeno.Comp.ExplosionArmor;
    if (explosionArmor <= 0)
      return;
    float num = (float) Math.Pow(1.1, (double) explosionArmor / 5.0);
    args.DamageCoefficient /= num;
  }

  private void OnXenoFortifyCancel<T>(Entity<XenoFortifyComponent> xeno, ref T args) where T : CancellableEntityEventArgs
  {
    if (!xeno.Comp.Fortified || xeno.Comp.CanMoveFortified)
      return;
    args.Cancel();
  }

  private void OnXenoFortifyAttack(Entity<XenoFortifyComponent> xeno, ref AttackAttemptEvent args)
  {
    if (!xeno.Comp.Fortified)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue || !this.HasComp<MobStateComponent>(target.GetValueOrDefault()))
      return;
    args.Cancel();
  }

  private void OnXenoFortifyHeadbuttAttempt(
    Entity<XenoFortifyComponent> xeno,
    ref XenoHeadbuttAttemptEvent args)
  {
    if (xeno.Comp.CanHeadbuttFortified || !xeno.Comp.Fortified)
      return;
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-fortify-cant-headbutt"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoFortifyRestAttempt(
    Entity<XenoFortifyComponent> xeno,
    ref XenoRestAttemptEvent args)
  {
    if (!xeno.Comp.Fortified)
      return;
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-fortify-cant-rest"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoFortifyTailSweepAttempt(
    Entity<XenoFortifyComponent> xeno,
    ref XenoTailSweepAttemptEvent args)
  {
    if (!xeno.Comp.Fortified)
      return;
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-fortify-cant-tail-sweep"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoFortifyToggleCrestAttempt(
    Entity<XenoFortifyComponent> xeno,
    ref XenoToggleCrestAttemptEvent args)
  {
    if (!xeno.Comp.Fortified)
      return;
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-fortify-cant-toggle-crest"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    args.Cancelled = true;
  }

  private void OnXenoFortifyMobStateChanged(
    Entity<XenoFortifyComponent> xeno,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState != MobState.Critical && args.NewMobState != MobState.Dead)
      return;
    this.Unfortify(xeno);
  }

  private void OnXenoFortifyRefreshSpeed(
    Entity<XenoFortifyComponent> xeno,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    if (!xeno.Comp.CanMoveFortified || !xeno.Comp.Fortified)
      return;
    float num = xeno.Comp.MoveSpeedModifier.Float();
    args.ModifySpeed(num, num);
  }

  private void OnXenoFortifyGetMeleeDamage(
    Entity<XenoFortifyComponent> xeno,
    ref GetMeleeDamageEvent args)
  {
    if (!xeno.Comp.Fortified)
      return;
    args.Damage.ExclusiveAdd(xeno.Comp.DamageAddedFortified);
  }

  private void Fortify(Entity<XenoFortifyComponent> xeno)
  {
    xeno.Comp.Fortified = true;
    RMCSizeComponent comp1;
    if (this.TryComp<RMCSizeComponent>((EntityUid) xeno, out comp1))
    {
      xeno.Comp.OriginalSize = new RMCSizes?(comp1.Size);
      comp1.Size = xeno.Comp.FortifySize;
      this.Dirty(xeno.Owner, (IComponent) comp1);
    }
    StunOnExplosionReceivedComponent comp2;
    if (xeno.Comp.ChangeExplosionWeakness && this.TryComp<StunOnExplosionReceivedComponent>((EntityUid) xeno, out comp2))
      this._explode.ChangeExplosionStunResistance((EntityUid) xeno, comp2, false);
    if (!xeno.Comp.CanMoveFortified)
    {
      this._fixtures.TryCreateFixture((EntityUid) xeno, xeno.Comp.Shape, "cm-xeno-fortify", collisionLayer: 223);
      this._transform.AnchorEntity((Entity<TransformComponent>) ((EntityUid) xeno, this.Transform((EntityUid) xeno)));
    }
    else
      this._speed.RefreshMovementSpeedModifiers((EntityUid) xeno);
    this.FortifyUpdated(xeno);
  }

  private void Unfortify(Entity<XenoFortifyComponent> xeno)
  {
    xeno.Comp.Fortified = false;
    RMCSizeComponent comp1;
    if (this.TryComp<RMCSizeComponent>((EntityUid) xeno, out comp1))
    {
      comp1.Size = xeno.Comp.OriginalSize ?? RMCSizes.Xeno;
      this.Dirty(xeno.Owner, (IComponent) comp1);
    }
    StunOnExplosionReceivedComponent comp2;
    if (xeno.Comp.ChangeExplosionWeakness && this.TryComp<StunOnExplosionReceivedComponent>((EntityUid) xeno, out comp2))
      this._explode.ChangeExplosionStunResistance((EntityUid) xeno, comp2, xeno.Comp.BaseWeakToExplosionStuns);
    if (!xeno.Comp.CanMoveFortified)
    {
      this._fixtures.DestroyFixture((EntityUid) xeno, "cm-xeno-fortify");
      this._transform.Unanchor((EntityUid) xeno, this.Transform((EntityUid) xeno));
      this._physics.TrySetBodyType((EntityUid) xeno, BodyType.KinematicController);
    }
    else
      this._speed.RefreshMovementSpeedModifiers((EntityUid) xeno);
    this.FortifyUpdated(xeno);
  }

  private void FortifyUpdated(Entity<XenoFortifyComponent> xeno)
  {
    this._actionBlocker.UpdateCanMove((EntityUid) xeno);
    this._appearance.SetData((EntityUid) xeno, (Enum) XenoVisualLayers.Fortify, (object) xeno.Comp.Fortified);
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoFortifyActionEvent>((EntityUid) xeno))
      this._actions.SetToggled(new Entity<ActionComponent>?(entity.AsNullable()), xeno.Comp.Fortified);
    this._armor.UpdateArmorValue((Entity<CMArmorComponent>) ((EntityUid) xeno, (CMArmorComponent) null));
    this.Dirty<XenoFortifyComponent>(xeno);
    XenoFortifiedEvent args = new XenoFortifiedEvent(xeno.Comp.Fortified);
    this.RaiseLocalEvent<XenoFortifiedEvent>((EntityUid) xeno, ref args);
  }
}
