// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ensnaring.SharedEnsnareableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Ensnaring.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.StepTrigger.Systems;
using Content.Shared.Strip.Components;
using Content.Shared.Throwing;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Ensnaring;

public abstract class SharedEnsnareableSystem : EntitySystem
{
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private MovementSpeedModifierSystem _speedModifier;
  [Dependency]
  protected SharedAppearanceSystem Appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  protected SharedContainerSystem Container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  protected SharedPopupSystem Popup;
  [Dependency]
  private SharedStaminaSystem _stamina;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EnsnareableComponent, ComponentInit>(new EntityEventRefHandler<EnsnareableComponent, ComponentInit>(this.OnEnsnareInit));
    this.SubscribeLocalEvent<EnsnareableComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<EnsnareableComponent, RefreshMovementSpeedModifiersEvent>(this.MovementSpeedModify));
    this.SubscribeLocalEvent<EnsnareableComponent, EnsnareEvent>(new ComponentEventHandler<EnsnareableComponent, EnsnareEvent>(this.OnEnsnare));
    this.SubscribeLocalEvent<EnsnareableComponent, EnsnareRemoveEvent>(new ComponentEventHandler<EnsnareableComponent, EnsnareRemoveEvent>(this.OnEnsnareRemove));
    this.SubscribeLocalEvent<EnsnareableComponent, EnsnaredChangedEvent>(new ComponentEventHandler<EnsnareableComponent, EnsnaredChangedEvent>(this.OnEnsnareChange));
    this.SubscribeLocalEvent<EnsnareableComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<EnsnareableComponent, AfterAutoHandleStateEvent>(this.OnHandleState));
    this.SubscribeLocalEvent<EnsnareableComponent, StrippingEnsnareButtonPressed>(new ComponentEventHandler<EnsnareableComponent, StrippingEnsnareButtonPressed>(this.OnStripEnsnareMessage));
    this.SubscribeLocalEvent<EnsnareableComponent, RemoveEnsnareAlertEvent>(new EntityEventRefHandler<EnsnareableComponent, RemoveEnsnareAlertEvent>(this.OnRemoveEnsnareAlert));
    this.SubscribeLocalEvent<EnsnareableComponent, EnsnareableDoAfterEvent>(new ComponentEventHandler<EnsnareableComponent, EnsnareableDoAfterEvent>(this.OnDoAfter));
    this.SubscribeLocalEvent<EnsnaringComponent, ComponentRemove>(new ComponentEventHandler<EnsnaringComponent, ComponentRemove>(this.OnComponentRemove));
    this.SubscribeLocalEvent<EnsnaringComponent, StepTriggerAttemptEvent>(new ComponentEventRefHandler<EnsnaringComponent, StepTriggerAttemptEvent>(this.AttemptStepTrigger));
    this.SubscribeLocalEvent<EnsnaringComponent, StepTriggeredOffEvent>(new ComponentEventRefHandler<EnsnaringComponent, StepTriggeredOffEvent>(this.OnStepTrigger));
    this.SubscribeLocalEvent<EnsnaringComponent, ThrowDoHitEvent>(new ComponentEventHandler<EnsnaringComponent, ThrowDoHitEvent>(this.OnThrowHit));
  }

  protected virtual void OnEnsnareInit(Entity<EnsnareableComponent> ent, ref ComponentInit args)
  {
    ent.Comp.Container = this.Container.EnsureContainer<Robust.Shared.Containers.Container>(ent.Owner, "ensnare");
  }

  private void OnHandleState(
    EntityUid uid,
    EnsnareableComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    this.RaiseLocalEvent<EnsnaredChangedEvent>(uid, new EnsnaredChangedEvent(component.IsEnsnared));
  }

  private void OnDoAfter(EntityUid uid, EnsnareableComponent component, DoAfterEvent args)
  {
    EnsnaringComponent comp;
    if (!args.Args.Target.HasValue || args.Handled || !this.TryComp<EnsnaringComponent>(args.Args.Used, out comp))
      return;
    if (args.Cancelled || !this.Container.Remove((Entity<TransformComponent, MetaDataComponent>) args.Args.Used.Value, (BaseContainer) component.Container))
    {
      EntityUid user = args.User;
      EntityUid? target = args.Target;
      if ((target.HasValue ? (user == target.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      {
        this.Popup.PopupPredicted(this.Loc.GetString("ensnare-component-try-free-fail", ("ensnare", (object) args.Args.Used)), uid, new EntityUid?(args.User), PopupType.MediumCaution);
      }
      else
      {
        target = args.Target;
        if (!target.HasValue)
          return;
        this.Popup.PopupPredicted(this.Loc.GetString("ensnare-component-try-free-fail-other", ("ensnare", (object) args.Args.Used), ("user", (object) args.Target)), uid, new EntityUid?(args.User), PopupType.MediumCaution);
      }
    }
    else
    {
      component.IsEnsnared = component.Container.ContainedEntities.Count > 0;
      this.Dirty(uid, (IComponent) component);
      comp.Ensnared = new EntityUid?();
      this._hands.PickupOrDrop(new EntityUid?(args.Args.User), args.Args.Used.Value);
      EntityUid user = args.User;
      EntityUid? target = args.Target;
      if ((target.HasValue ? (user == target.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      {
        this.Popup.PopupPredicted(this.Loc.GetString("ensnare-component-try-free-complete", ("ensnare", (object) args.Args.Used)), uid, new EntityUid?(args.User), PopupType.Medium);
      }
      else
      {
        target = args.Target;
        if (target.HasValue)
          this.Popup.PopupPredicted(this.Loc.GetString("ensnare-component-try-free-complete-other", ("ensnare", (object) args.Args.Used), ("user", (object) args.Target)), uid, new EntityUid?(args.User), PopupType.Medium);
      }
      this.UpdateAlert(args.Args.Target.Value, component);
      EnsnareRemoveEvent args1 = new EnsnareRemoveEvent(comp.WalkSpeed, comp.SprintSpeed);
      this.RaiseLocalEvent<EnsnareRemoveEvent>(uid, args1);
      args.Handled = true;
    }
  }

  private void OnEnsnare(EntityUid uid, EnsnareableComponent component, EnsnareEvent args)
  {
    component.WalkSpeed *= args.WalkSpeed;
    component.SprintSpeed *= args.SprintSpeed;
    this._speedModifier.RefreshMovementSpeedModifiers(uid);
    EnsnaredChangedEvent args1 = new EnsnaredChangedEvent(component.IsEnsnared);
    this.RaiseLocalEvent<EnsnaredChangedEvent>(uid, args1);
  }

  private void OnEnsnareRemove(
    EntityUid uid,
    EnsnareableComponent component,
    EnsnareRemoveEvent args)
  {
    component.WalkSpeed /= args.WalkSpeed;
    component.SprintSpeed /= args.SprintSpeed;
    this._speedModifier.RefreshMovementSpeedModifiers(uid);
    EnsnaredChangedEvent args1 = new EnsnaredChangedEvent(component.IsEnsnared);
    this.RaiseLocalEvent<EnsnaredChangedEvent>(uid, args1);
  }

  private void OnEnsnareChange(
    EntityUid uid,
    EnsnareableComponent component,
    EnsnaredChangedEvent args)
  {
    this.UpdateAppearance(uid, component);
  }

  private void UpdateAppearance(
    EntityUid uid,
    EnsnareableComponent component,
    AppearanceComponent? appearance = null)
  {
    this.Appearance.SetData(uid, (Enum) EnsnareableVisuals.IsEnsnared, (object) component.IsEnsnared, appearance);
  }

  private void MovementSpeedModify(
    EntityUid uid,
    EnsnareableComponent component,
    RefreshMovementSpeedModifiersEvent args)
  {
    if (!component.IsEnsnared)
      return;
    args.ModifySpeed(component.WalkSpeed, component.SprintSpeed);
  }

  public void TryFree(
    EntityUid target,
    EntityUid user,
    EntityUid ensnare,
    EnsnaringComponent component)
  {
    if (!this.HasComp<EnsnareableComponent>(target))
      return;
    float seconds = user == target ? component.BreakoutTime : component.FreeTime;
    bool flag = !component.CanMoveBreakout;
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, seconds, (DoAfterEvent) new EnsnareableDoAfterEvent(), new EntityUid?(target), new EntityUid?(target), new EntityUid?(ensnare))
    {
      BreakOnMove = flag,
      BreakOnDamage = false,
      NeedHand = true,
      BreakOnDropItem = false
    }))
      return;
    if (user == target)
      this.Popup.PopupPredicted(this.Loc.GetString("ensnare-component-try-free", (nameof (ensnare), (object) ensnare)), target, new EntityUid?(target));
    else
      this.Popup.PopupPredicted(this.Loc.GetString("ensnare-component-try-free-other", (nameof (ensnare), (object) ensnare), (nameof (user), (object) Identity.Entity(target, (IEntityManager) this.EntityManager))), user, new EntityUid?(user));
  }

  private void OnStripEnsnareMessage(
    EntityUid uid,
    EnsnareableComponent component,
    StrippingEnsnareButtonPressed args)
  {
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) component.Container.ContainedEntities)
    {
      EnsnaringComponent comp;
      if (this.TryComp<EnsnaringComponent>(containedEntity, out comp))
      {
        this.TryFree(uid, args.Actor, containedEntity, comp);
        break;
      }
    }
  }

  private void OnRemoveEnsnareAlert(
    Entity<EnsnareableComponent> ent,
    ref RemoveEnsnareAlertEvent args)
  {
    if (args.Handled)
      return;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) ent.Comp.Container.ContainedEntities)
    {
      EnsnaringComponent comp;
      if (this.TryComp<EnsnaringComponent>(containedEntity, out comp))
      {
        this.TryFree((EntityUid) ent, (EntityUid) ent, containedEntity, comp);
        args.Handled = true;
        break;
      }
    }
  }

  private void OnComponentRemove(EntityUid uid, EnsnaringComponent component, ComponentRemove args)
  {
    EnsnareableComponent comp;
    if (!this.TryComp<EnsnareableComponent>(component.Ensnared, out comp) || !comp.IsEnsnared)
      return;
    this.ForceFree(uid, component);
  }

  private void AttemptStepTrigger(
    EntityUid uid,
    EnsnaringComponent component,
    ref StepTriggerAttemptEvent args)
  {
    args.Continue = true;
  }

  private void OnStepTrigger(
    EntityUid uid,
    EnsnaringComponent component,
    ref StepTriggeredOffEvent args)
  {
    this.TryEnsnare(args.Tripper, uid, component);
  }

  private void OnThrowHit(EntityUid uid, EnsnaringComponent component, ThrowDoHitEvent args)
  {
    if (!component.CanThrowTrigger || !this.TryEnsnare(args.Target, uid, component))
      return;
    this._audio.PlayPvs(component.EnsnareSound, uid);
  }

  public bool TryEnsnare(EntityUid target, EntityUid ensnare, EnsnaringComponent component)
  {
    EnsnareableComponent comp1;
    if (!this.TryComp<EnsnareableComponent>(target, out comp1) || (double) comp1.Container.ContainedEntities.Count >= (double) component.MaxEnsnares)
      return false;
    this.Container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) ensnare, (BaseContainer) comp1.Container);
    StaminaComponent comp2;
    if (this.TryComp<StaminaComponent>(target, out comp2))
    {
      SharedStaminaSystem stamina = this._stamina;
      EntityUid uid = target;
      double staminaDamage = (double) component.StaminaDamage;
      EntityUid? nullable = new EntityUid?(ensnare);
      StaminaComponent component1 = comp2;
      EntityUid? source = new EntityUid?();
      EntityUid? with = nullable;
      stamina.TakeStaminaDamage(uid, (float) staminaDamage, component1, source, with);
    }
    component.Ensnared = new EntityUid?(target);
    comp1.IsEnsnared = true;
    this.Dirty(target, (IComponent) comp1);
    this.UpdateAlert(target, comp1);
    EnsnareEvent args = new EnsnareEvent(component.WalkSpeed, component.SprintSpeed);
    this.RaiseLocalEvent<EnsnareEvent>(target, args);
    return true;
  }

  public void ForceFree(EntityUid ensnare, EnsnaringComponent component)
  {
    EnsnareableComponent comp;
    if (!component.Ensnared.HasValue || !this.TryComp<EnsnareableComponent>(component.Ensnared, out comp))
      return;
    EntityUid target = component.Ensnared.Value;
    this.Container.Remove((Entity<TransformComponent, MetaDataComponent>) ensnare, (BaseContainer) comp.Container, force: true);
    comp.IsEnsnared = comp.Container.ContainedEntities.Count > 0;
    this.Dirty(component.Ensnared.Value, (IComponent) comp);
    component.Ensnared = new EntityUid?();
    this.UpdateAlert(target, comp);
    EnsnareRemoveEvent args = new EnsnareRemoveEvent(component.WalkSpeed, component.SprintSpeed);
    this.RaiseLocalEvent<EnsnareRemoveEvent>(ensnare, args);
  }

  public void UpdateAlert(EntityUid target, EnsnareableComponent component)
  {
    if (!component.IsEnsnared)
      this._alerts.ClearAlert(target, component.EnsnaredAlert);
    else
      this._alerts.ShowAlert(target, component.EnsnaredAlert);
  }
}
