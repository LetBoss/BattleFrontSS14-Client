// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.SharedJetpackSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Gravity;
using Content.Shared.Interaction.Events;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using System;

#nullable enable
namespace Content.Shared.Movement.Systems;

public abstract class SharedJetpackSystem : EntitySystem
{
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeedModifier;
  [Dependency]
  protected SharedAppearanceSystem Appearance;
  [Dependency]
  protected SharedContainerSystem Container;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private ActionContainerSystem _actionContainer;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<JetpackComponent, GetItemActionsEvent>(new ComponentEventHandler<JetpackComponent, GetItemActionsEvent>(this.OnJetpackGetAction));
    this.SubscribeLocalEvent<JetpackComponent, DroppedEvent>(new ComponentEventHandler<JetpackComponent, DroppedEvent>(this.OnJetpackDropped));
    this.SubscribeLocalEvent<JetpackComponent, ToggleJetpackEvent>(new ComponentEventHandler<JetpackComponent, ToggleJetpackEvent>(this.OnJetpackToggle));
    this.SubscribeLocalEvent<JetpackUserComponent, RefreshWeightlessModifiersEvent>(new EntityEventRefHandler<JetpackUserComponent, RefreshWeightlessModifiersEvent>(this.OnJetpackUserWeightlessMovement));
    this.SubscribeLocalEvent<JetpackUserComponent, CanWeightlessMoveEvent>(new ComponentEventRefHandler<JetpackUserComponent, CanWeightlessMoveEvent>(this.OnJetpackUserCanWeightless));
    this.SubscribeLocalEvent<JetpackUserComponent, EntParentChangedMessage>(new ComponentEventRefHandler<JetpackUserComponent, EntParentChangedMessage>(this.OnJetpackUserEntParentChanged));
    this.SubscribeLocalEvent<JetpackComponent, EntGotInsertedIntoContainerMessage>(new EntityEventRefHandler<JetpackComponent, EntGotInsertedIntoContainerMessage>(this.OnJetpackMoved));
    this.SubscribeLocalEvent<GravityChangedEvent>(new EntityEventRefHandler<GravityChangedEvent>(this.OnJetpackUserGravityChanged));
    this.SubscribeLocalEvent<JetpackComponent, MapInitEvent>(new ComponentEventHandler<JetpackComponent, MapInitEvent>(this.OnMapInit));
  }

  private void OnJetpackUserWeightlessMovement(
    Entity<JetpackUserComponent> ent,
    ref RefreshWeightlessModifiersEvent args)
  {
    args.WeightlessAcceleration = ent.Comp.WeightlessAcceleration;
    args.WeightlessModifier = ent.Comp.WeightlessModifier;
    args.WeightlessFriction = ent.Comp.WeightlessFriction;
    args.WeightlessFrictionNoInput = ent.Comp.WeightlessFrictionNoInput;
  }

  private void OnMapInit(EntityUid uid, JetpackComponent component, MapInitEvent args)
  {
    this._actionContainer.EnsureAction(uid, ref component.ToggleActionEntity, (string) component.ToggleAction);
    this.Dirty(uid, (IComponent) component);
  }

  private void OnJetpackUserGravityChanged(ref GravityChangedEvent ev)
  {
    EntityUid changedGridIndex = ev.ChangedGridIndex;
    Robust.Shared.GameObjects.EntityQuery<JetpackComponent> entityQuery = this.GetEntityQuery<JetpackComponent>();
    Robust.Shared.GameObjects.EntityQueryEnumerator<JetpackUserComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<JetpackUserComponent, TransformComponent>();
    EntityUid uid;
    JetpackUserComponent comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      EntityUid? gridUid = comp2.GridUid;
      EntityUid entityUid = changedGridIndex;
      JetpackComponent component;
      if ((gridUid.HasValue ? (gridUid.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0 && ev.HasGravity && entityQuery.TryGetComponent(comp1.Jetpack, out component))
      {
        this._popup.PopupClient(this.Loc.GetString("jetpack-to-grid"), uid, new EntityUid?(uid));
        this.SetEnabled(comp1.Jetpack, component, false, new EntityUid?(uid));
      }
    }
  }

  private void OnJetpackDropped(EntityUid uid, JetpackComponent component, DroppedEvent args)
  {
    this.SetEnabled(uid, component, false, new EntityUid?(args.User));
  }

  private void OnJetpackMoved(
    Entity<JetpackComponent> ent,
    ref EntGotInsertedIntoContainerMessage args)
  {
    EntityUid owner = args.Container.Owner;
    EntityUid? jetpackUser = ent.Comp.JetpackUser;
    if ((jetpackUser.HasValue ? (owner != jetpackUser.GetValueOrDefault() ? 1 : 0) : 1) == 0)
      return;
    this.SetEnabled((EntityUid) ent, ent.Comp, false, ent.Comp.JetpackUser);
  }

  private void OnJetpackUserCanWeightless(
    EntityUid uid,
    JetpackUserComponent component,
    ref CanWeightlessMoveEvent args)
  {
    args.CanMove = true;
  }

  private void OnJetpackUserEntParentChanged(
    EntityUid uid,
    JetpackUserComponent component,
    ref EntParentChangedMessage args)
  {
    JetpackComponent comp;
    if (!this.TryComp<JetpackComponent>(component.Jetpack, out comp) || this.CanEnableOnGrid(args.Transform.GridUid))
      return;
    this.SetEnabled(component.Jetpack, comp, false, new EntityUid?(uid));
    this._popup.PopupClient(this.Loc.GetString("jetpack-to-grid"), uid, new EntityUid?(uid));
  }

  private void SetupUser(EntityUid user, EntityUid jetpackUid, JetpackComponent component)
  {
    JetpackUserComponent comp1;
    this.EnsureComp<JetpackUserComponent>(user, out comp1);
    component.JetpackUser = new EntityUid?(user);
    PhysicsComponent comp2;
    if (this.TryComp<PhysicsComponent>(user, out comp2))
      this._physics.SetBodyStatus(user, comp2, BodyStatus.InAir);
    comp1.Jetpack = jetpackUid;
    comp1.WeightlessAcceleration = component.Acceleration;
    comp1.WeightlessModifier = component.WeightlessModifier;
    comp1.WeightlessFriction = component.Friction;
    comp1.WeightlessFrictionNoInput = component.Friction;
    this._movementSpeedModifier.RefreshWeightlessModifiers(user);
  }

  private void RemoveUser(EntityUid uid, JetpackComponent component)
  {
    if (!this.RemComp<JetpackUserComponent>(uid))
      return;
    component.JetpackUser = new EntityUid?();
    PhysicsComponent comp;
    if (this.TryComp<PhysicsComponent>(uid, out comp))
      this._physics.SetBodyStatus(uid, comp, BodyStatus.OnGround);
    this._movementSpeedModifier.RefreshWeightlessModifiers(uid);
  }

  private void OnJetpackToggle(EntityUid uid, JetpackComponent component, ToggleJetpackEvent args)
  {
    if (args.Handled)
      return;
    TransformComponent comp;
    if (this.TryComp(uid, out comp) && !this.CanEnableOnGrid(comp.GridUid))
      this._popup.PopupClient(this.Loc.GetString("jetpack-no-station"), uid, new EntityUid?(args.Performer));
    else
      this.SetEnabled(uid, component, !this.IsEnabled(uid));
  }

  private bool CanEnableOnGrid(EntityUid? gridUid)
  {
    return !gridUid.HasValue || !this.HasComp<GravityComponent>(gridUid);
  }

  private void OnJetpackGetAction(
    EntityUid uid,
    JetpackComponent component,
    GetItemActionsEvent args)
  {
    args.AddAction(ref component.ToggleActionEntity, (string) component.ToggleAction);
  }

  private bool IsEnabled(EntityUid uid) => this.HasComp<ActiveJetpackComponent>(uid);

  public void SetEnabled(EntityUid uid, JetpackComponent component, bool enabled, EntityUid? user = null)
  {
    if (this.IsEnabled(uid) == enabled || enabled && !this.CanEnable(uid, component))
      return;
    if (!user.HasValue)
    {
      BaseContainer container;
      if (!this.Container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (uid, (TransformComponent) null, (MetaDataComponent) null), out container))
        return;
      user = new EntityUid?(container.Owner);
    }
    if (enabled)
    {
      this.SetupUser(user.Value, uid, component);
      this.EnsureComp<ActiveJetpackComponent>(uid);
    }
    else
    {
      this.RemoveUser(user.Value, component);
      this.RemComp<ActiveJetpackComponent>(uid);
    }
    this.Appearance.SetData(uid, (Enum) JetpackVisuals.Enabled, (object) enabled);
    this.Dirty(uid, (IComponent) component);
  }

  public bool IsUserFlying(EntityUid uid) => this.HasComp<JetpackUserComponent>(uid);

  protected virtual bool CanEnable(EntityUid uid, JetpackComponent component) => true;
}
