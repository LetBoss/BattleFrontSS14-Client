// Decompiled with JetBrains decompiler
// Type: Content.Shared.Disposal.Unit.SharedDisposalUnitSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Body.Components;
using Content.Shared.Climbing.Systems;
using Content.Shared.Containers;
using Content.Shared.Database;
using Content.Shared.Disposal.Components;
using Content.Shared.Disposal.Tube;
using Content.Shared.Disposal.Unit.Events;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Emag.Systems;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Content.Shared.Popups;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.Throwing;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Disposal.Unit;

public abstract class SharedDisposalUnitSystem : EntitySystem
{
  [Dependency]
  protected ActionBlockerSystem ActionBlockerSystem;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  protected MetaDataSystem Metadata;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  protected SharedAudioSystem Audio;
  [Dependency]
  protected IGameTiming GameTiming;
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private ClimbSystem _climb;
  [Dependency]
  protected SharedContainerSystem Containers;
  [Dependency]
  protected SharedJointSystem Joints;
  [Dependency]
  private SharedPowerReceiverSystem _power;
  [Dependency]
  private SharedDisposalTubeSystem _disposalTubeSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedDoAfterSystem _doAfterSystem;
  [Dependency]
  private SharedHandsSystem _handsSystem;
  [Dependency]
  protected SharedTransformSystem TransformSystem;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private SharedMapSystem _map;
  protected static TimeSpan ExitAttemptDelay = TimeSpan.FromSeconds(0.5);
  public const float PressurePerSecond = 0.05f;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, PreventCollideEvent>(new ComponentEventRefHandler<DisposalUnitComponent, PreventCollideEvent>((object) this, __methodptr(OnPreventCollide)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, CanDropTargetEvent>(new ComponentEventRefHandler<DisposalUnitComponent, CanDropTargetEvent>((object) this, __methodptr(OnCanDragDropOn)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<DisposalUnitComponent, GetVerbsEvent<InteractionVerb>>((object) this, __methodptr(AddInsertVerb)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<DisposalUnitComponent, GetVerbsEvent<AlternativeVerb>>((object) this, __methodptr(AddDisposalAltVerbs)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<DisposalUnitComponent, GetVerbsEvent<Verb>>((object) this, __methodptr(AddClimbInsideVerb)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, DisposalDoAfterEvent>(new ComponentEventHandler<DisposalUnitComponent, DisposalDoAfterEvent>((object) this, __methodptr(OnDoAfter)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, BeforeThrowInsertEvent>(new EntityEventRefHandler<DisposalUnitComponent, BeforeThrowInsertEvent>((object) this, __methodptr(OnThrowInsert)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, DisposalUnitComponent.UiButtonPressedMessage>(new ComponentEventHandler<DisposalUnitComponent, DisposalUnitComponent.UiButtonPressedMessage>((object) this, __methodptr(OnUiButtonPressed)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, GotEmaggedEvent>(new ComponentEventRefHandler<DisposalUnitComponent, GotEmaggedEvent>((object) this, __methodptr(OnEmagged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<DisposalUnitComponent, AnchorStateChangedEvent>((object) this, __methodptr(OnAnchorChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, PowerChangedEvent>(new ComponentEventRefHandler<DisposalUnitComponent, PowerChangedEvent>((object) this, __methodptr(OnPowerChange)), (Type[]) null, (Type[]) null);
    SharedDisposalUnitSystem disposalUnitSystem = this;
    // ISSUE: virtual method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, ComponentInit>(new EntityEventRefHandler<DisposalUnitComponent, ComponentInit>((object) disposalUnitSystem, __vmethodptr(disposalUnitSystem, OnDisposalInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, ActivateInWorldEvent>(new ComponentEventHandler<DisposalUnitComponent, ActivateInWorldEvent>((object) this, __methodptr(OnActivate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, AfterInteractUsingEvent>(new ComponentEventHandler<DisposalUnitComponent, AfterInteractUsingEvent>((object) this, __methodptr(OnAfterInteractUsing)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, DragDropTargetEvent>(new ComponentEventRefHandler<DisposalUnitComponent, DragDropTargetEvent>((object) this, __methodptr(OnDragDropOn)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, ContainerRelayMovementEntityEvent>(new ComponentEventRefHandler<DisposalUnitComponent, ContainerRelayMovementEntityEvent>((object) this, __methodptr(OnMovement)), (Type[]) null, (Type[]) null);
  }

  private void AddDisposalAltVerbs(
    Entity<DisposalUnitComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    EntityUid uid = ent.Owner;
    DisposalUnitComponent component = ent.Comp;
    if (((BaseContainer) component.Container).ContainedEntities.Count <= 0)
      return;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = (Action) (() => this.ManualEngage(uid, component));
    alternativeVerb1.Text = this.Loc.GetString("disposal-flush-verb-get-data-text");
    alternativeVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/delete_transparent.svg.192dpi.png"));
    alternativeVerb1.Priority = 1;
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
    AlternativeVerb alternativeVerb3 = new AlternativeVerb();
    alternativeVerb3.Act = (Action) (() => this.TryEjectContents(uid, component));
    alternativeVerb3.Category = VerbCategory.Eject;
    alternativeVerb3.Text = this.Loc.GetString("disposal-eject-verb-get-data-text");
    AlternativeVerb alternativeVerb4 = alternativeVerb3;
    args.Verbs.Add(alternativeVerb4);
  }

  private void AddInsertVerb(
    EntityUid uid,
    DisposalUnitComponent component,
    GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null || !args.Using.HasValue || !this.ActionBlockerSystem.CanDrop(args.User) || !this.CanInsert(uid, component, args.Using.Value))
      return;
    InteractionVerb interactionVerb1 = new InteractionVerb();
    interactionVerb1.Text = this.Name(args.Using.Value, (MetaDataComponent) null);
    interactionVerb1.Category = VerbCategory.Insert;
    interactionVerb1.Act = (Action) (() =>
    {
      this._handsSystem.TryDropIntoContainer(Entity<HandsComponent>.op_Implicit((args.User, args.Hands)), args.Using.Value, (BaseContainer) component.Container, false);
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(16 /*0x10*/, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "player", "ToPrettyString(args.User)");
      logStringHandler.AppendLiteral(" inserted ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.Using.Value)), "ToPrettyString(args.Using.Value)");
      logStringHandler.AppendLiteral(" into ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.Action, LogImpact.Medium, ref local);
      this.AfterInsert(uid, component, args.Using.Value, new EntityUid?(args.User));
    });
    InteractionVerb interactionVerb2 = interactionVerb1;
    args.Verbs.Add(interactionVerb2);
  }

  private void OnDoAfter(EntityUid uid, DisposalUnitComponent component, DoAfterEvent args)
  {
    if (args.Handled || args.Cancelled || !args.Args.Target.HasValue || !args.Args.Used.HasValue)
      return;
    this.AfterInsert(uid, component, args.Args.Target.Value, new EntityUid?(args.Args.User), true);
    args.Handled = true;
  }

  private void OnThrowInsert(Entity<DisposalUnitComponent> ent, ref BeforeThrowInsertEvent args)
  {
    if (this.CanInsert(Entity<DisposalUnitComponent>.op_Implicit(ent), Entity<DisposalUnitComponent>.op_Implicit(ent), args.ThrownEntity))
      return;
    args.Cancelled = true;
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<DisposalUnitComponent, MetaDataComponent> entityQueryEnumerator = this.EntityQueryEnumerator<DisposalUnitComponent, MetaDataComponent>();
    EntityUid uid;
    DisposalUnitComponent component;
    MetaDataComponent metadata;
    while (entityQueryEnumerator.MoveNext(ref uid, ref component, ref metadata))
      this.Update(uid, component, metadata);
  }

  private void OnMovement(
    EntityUid uid,
    DisposalUnitComponent component,
    ref ContainerRelayMovementEntityEvent args)
  {
    TimeSpan curTime = this.GameTiming.CurTime;
    HandsComponent handsComponent;
    if (!this.ActionBlockerSystem.CanMove(args.Entity) || !this.TryComp<HandsComponent>(args.Entity, ref handsComponent) || handsComponent.Count == 0 || curTime < component.LastExitAttempt + SharedDisposalUnitSystem.ExitAttemptDelay)
      return;
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    component.LastExitAttempt = curTime;
    this.Remove(uid, component, args.Entity);
    this.UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
  }

  private void OnActivate(
    EntityUid uid,
    DisposalUnitComponent component,
    ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex)
      return;
    args.Handled = true;
    this._ui.TryToggleUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum) DisposalUnitComponent.DisposalUnitUiKey.Key, args.User);
  }

  private void OnAfterInteractUsing(
    EntityUid uid,
    DisposalUnitComponent component,
    AfterInteractUsingEvent args)
  {
    if (args.Handled || !args.CanReach || !this.HasComp<HandsComponent>(args.User) || !this.CanInsert(uid, component, args.Used) || !this._handsSystem.TryDropIntoContainer(Entity<HandsComponent>.op_Implicit(args.User), args.Used, (BaseContainer) component.Container))
      return;
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(16 /*0x10*/, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.User)), "player", "ToPrettyString(args.User)");
    logStringHandler.AppendLiteral(" inserted ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(args.Used)), "ToPrettyString(args.Used)");
    logStringHandler.AppendLiteral(" into ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.Action, LogImpact.Medium, ref local);
    this.AfterInsert(uid, component, args.Used, new EntityUid?(args.User));
    args.Handled = true;
  }

  protected virtual void OnDisposalInit(Entity<DisposalUnitComponent> ent, ref ComponentInit args)
  {
    ent.Comp.Container = this.Containers.EnsureContainer<Container>(Entity<DisposalUnitComponent>.op_Implicit(ent), "disposals", (ContainerManagerComponent) null);
  }

  private void OnPowerChange(
    EntityUid uid,
    DisposalUnitComponent component,
    ref PowerChangedEvent args)
  {
    if (!component.Running)
      return;
    this.UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
    this.UpdateVisualState(uid, component);
    if (!args.Powered)
    {
      component.NextFlush = new TimeSpan?();
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
    else
    {
      if (!component.Engaged)
        return;
      this.ManualEngage(uid, component);
    }
  }

  private void OnAnchorChanged(
    EntityUid uid,
    DisposalUnitComponent component,
    ref AnchorStateChangedEvent args)
  {
    if (this.Terminating(uid, (MetaDataComponent) null))
      return;
    this.UpdateVisualState(uid, component);
    if (((AnchorStateChangedEvent) ref args).Anchored)
      return;
    this.TryEjectContents(uid, component);
  }

  private void OnDragDropOn(
    EntityUid uid,
    DisposalUnitComponent component,
    ref DragDropTargetEvent args)
  {
    args.Handled = this.TryInsert(uid, args.Dragged, new EntityUid?(args.User));
  }

  protected virtual void UpdateUI(Entity<DisposalUnitComponent> entity)
  {
  }

  public TimeSpan EstimatedFullPressure(EntityUid uid, DisposalUnitComponent component)
  {
    return component.NextPressurized < this.GameTiming.CurTime ? TimeSpan.Zero : component.NextPressurized;
  }

  public bool CanFlush(EntityUid unit, DisposalUnitComponent component)
  {
    return this.GetState(unit, component) == DisposalsPressureState.Ready && this._power.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(unit)) && this.Comp<TransformComponent>(unit).Anchored;
  }

  public void Remove(EntityUid uid, DisposalUnitComponent component, EntityUid toRemove)
  {
    if (this.GameTiming.ApplyingState || !this.Containers.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(toRemove), (BaseContainer) component.Container, true, false, new EntityCoordinates?(), new Angle?()))
      return;
    if (((BaseContainer) component.Container).ContainedEntities.Count == 0 && !component.Engaged)
    {
      component.NextFlush = new TimeSpan?();
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
      this.UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
    }
    this._climb.Climb(toRemove, toRemove, uid, true);
    this.UpdateVisualState(uid, component);
  }

  public void UpdateVisualState(EntityUid uid, DisposalUnitComponent component, bool flush = false)
  {
    AppearanceComponent appearanceComponent;
    if (!this.TryComp<AppearanceComponent>(uid, ref appearanceComponent))
      return;
    if (!this.Transform(uid).Anchored)
    {
      this._appearance.SetData(uid, (Enum) DisposalUnitComponent.Visuals.VisualState, (object) DisposalUnitComponent.VisualState.UnAnchored, appearanceComponent);
      this._appearance.SetData(uid, (Enum) DisposalUnitComponent.Visuals.Handle, (object) DisposalUnitComponent.HandleState.Normal, appearanceComponent);
      this._appearance.SetData(uid, (Enum) DisposalUnitComponent.Visuals.Light, (object) DisposalUnitComponent.LightStates.Off, appearanceComponent);
    }
    else
    {
      DisposalsPressureState state = this.GetState(uid, component);
      switch (state)
      {
        case DisposalsPressureState.Ready:
          this._appearance.SetData(uid, (Enum) DisposalUnitComponent.Visuals.VisualState, (object) DisposalUnitComponent.VisualState.Anchored, appearanceComponent);
          break;
        case DisposalsPressureState.Flushed:
          this._appearance.SetData(uid, (Enum) DisposalUnitComponent.Visuals.VisualState, (object) DisposalUnitComponent.VisualState.OverlayFlushing, appearanceComponent);
          break;
        case DisposalsPressureState.Pressurizing:
          this._appearance.SetData(uid, (Enum) DisposalUnitComponent.Visuals.VisualState, (object) DisposalUnitComponent.VisualState.OverlayCharging, appearanceComponent);
          break;
      }
      this._appearance.SetData(uid, (Enum) DisposalUnitComponent.Visuals.Handle, (object) (DisposalUnitComponent.HandleState) (component.Engaged ? 1 : 0), appearanceComponent);
      if (!this._power.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(uid)))
      {
        this._appearance.SetData(uid, (Enum) DisposalUnitComponent.Visuals.Light, (object) DisposalUnitComponent.LightStates.Off, appearanceComponent);
      }
      else
      {
        DisposalUnitComponent.LightStates lightStates1 = DisposalUnitComponent.LightStates.Off;
        if (((BaseContainer) component.Container).ContainedEntities.Count > 0)
          lightStates1 |= DisposalUnitComponent.LightStates.Full;
        bool flag;
        switch (state)
        {
          case DisposalsPressureState.Flushed:
          case DisposalsPressureState.Pressurizing:
            flag = true;
            break;
          default:
            flag = false;
            break;
        }
        DisposalUnitComponent.LightStates lightStates2 = !flag ? lightStates1 | DisposalUnitComponent.LightStates.Ready : lightStates1 | DisposalUnitComponent.LightStates.Charging;
        this._appearance.SetData(uid, (Enum) DisposalUnitComponent.Visuals.Light, (object) lightStates2, appearanceComponent);
      }
    }
  }

  public DisposalsPressureState GetState(
    EntityUid uid,
    DisposalUnitComponent component,
    MetaDataComponent? metadata = null)
  {
    TimeSpan timeSpan = this.Metadata.GetPauseTime(uid, metadata) + component.NextPressurized - this.GameTiming.CurTime;
    double num = 20.0 - component.FlushDelay.TotalSeconds;
    if (timeSpan.TotalSeconds > num)
      return DisposalsPressureState.Flushed;
    return timeSpan > TimeSpan.Zero ? DisposalsPressureState.Pressurizing : DisposalsPressureState.Ready;
  }

  public float GetPressure(
    EntityUid uid,
    DisposalUnitComponent component,
    MetaDataComponent? metadata = null)
  {
    return !this.Resolve(uid, ref metadata, true) ? 0.0f : MathF.Min(1f, (float) ((this.GameTiming.CurTime - this.Metadata.GetPauseTime(uid, metadata) - component.NextPressurized).TotalSeconds / 0.05000000074505806));
  }

  protected void OnPreventCollide(
    EntityUid uid,
    DisposalUnitComponent component,
    ref PreventCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    if (!this.HasComp<ItemComponent>(otherEntity) || this.HasComp<ThrownItemComponent>(otherEntity))
      return;
    args.Cancelled = true;
  }

  protected void OnCanDragDropOn(
    EntityUid uid,
    DisposalUnitComponent component,
    ref CanDropTargetEvent args)
  {
    if (args.Handled)
      return;
    args.CanDrop = this.CanInsert(uid, component, args.Dragged);
    args.Handled = true;
  }

  protected void OnEmagged(
    EntityUid uid,
    DisposalUnitComponent component,
    ref GotEmaggedEvent args)
  {
    component.DisablePressure = true;
    args.Handled = true;
  }

  public virtual bool CanInsert(EntityUid uid, DisposalUnitComponent component, EntityUid entity)
  {
    if (!this.Containers.CanInsert(entity, (BaseContainer) component.Container, false, (TransformComponent) null) || !this.Transform(uid).Anchored)
      return false;
    bool flag = this.HasComp<ItemComponent>(entity);
    PhysicsComponent physicsComponent;
    return (flag || this.HasComp<BodyComponent>(entity)) && !this._whitelistSystem.IsBlacklistPass(component.Blacklist, entity) && !this._whitelistSystem.IsWhitelistFail(component.Whitelist, entity) && ((!this.TryComp<PhysicsComponent>(entity, ref physicsComponent) ? 0 : (physicsComponent.CanCollide ? 1 : 0)) | (flag ? 1 : 0)) != 0;
  }

  public void DoInsertDisposalUnit(
    EntityUid uid,
    EntityUid toInsert,
    EntityUid user,
    DisposalUnitComponent? disposal = null)
  {
    if (!this.Resolve<DisposalUnitComponent>(uid, ref disposal, true) || !this.Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(toInsert), (BaseContainer) disposal.Container, (TransformComponent) null, false))
      return;
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(16 /*0x10*/, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "player", "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" inserted ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(toInsert)), "ToPrettyString(toInsert)");
    logStringHandler.AppendLiteral(" into ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.Action, LogImpact.Medium, ref local);
    this.AfterInsert(uid, disposal, toInsert, new EntityUid?(user));
  }

  public virtual void AfterInsert(
    EntityUid uid,
    DisposalUnitComponent component,
    EntityUid inserted,
    EntityUid? user = null,
    bool doInsert = false)
  {
    this.Audio.PlayPredicted(component.InsertSound, uid, user, new AudioParams?());
    if (doInsert && !this.Containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(inserted), (BaseContainer) component.Container, (TransformComponent) null, false))
      return;
    EntityUid? nullable = user;
    EntityUid entityUid = inserted;
    if ((nullable.HasValue ? (EntityUid.op_Inequality(nullable.GetValueOrDefault(), entityUid) ? 1 : 0) : 1) != 0 && user.HasValue)
    {
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(16 /*0x10*/, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user.Value)), "player", "ToPrettyString(user.Value)");
      logStringHandler.AppendLiteral(" inserted ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(inserted)), "ToPrettyString(inserted)");
      logStringHandler.AppendLiteral(" into ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.Action, LogImpact.Medium, ref local);
    }
    this.QueueAutomaticEngage(uid, component);
    this._ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum) DisposalUnitComponent.DisposalUnitUiKey.Key, new EntityUid?(inserted), false);
    this.Joints.RecursiveClearJoints(inserted, (TransformComponent) null, (JointComponent) null, (JointRelayTargetComponent) null);
    this.UpdateVisualState(uid, component);
  }

  public bool TryInsert(
    EntityUid unitId,
    EntityUid toInsertId,
    EntityUid? userId,
    DisposalUnitComponent? unit = null)
  {
    if (!this.Resolve<DisposalUnitComponent>(unitId, ref unit, true))
      return false;
    EntityUid? nullable;
    if (userId.HasValue && !this.HasComp<HandsComponent>(userId))
    {
      EntityUid entityUid = toInsertId;
      nullable = userId;
      if ((nullable.HasValue ? (EntityUid.op_Inequality(entityUid, nullable.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      {
        this._popupSystem.PopupEntity(this.Loc.GetString("disposal-unit-no-hands"), userId.Value, userId.Value, PopupType.SmallCaution);
        return false;
      }
    }
    if (!this.CanInsert(unitId, unit, toInsertId))
      return false;
    nullable = userId;
    EntityUid entityUid1 = toInsertId;
    bool flag = nullable.HasValue && EntityUid.op_Equality(nullable.GetValueOrDefault(), entityUid1);
    float seconds = flag ? unit.EntryDelay : unit.DraggedEntryDelay;
    if (userId.HasValue && !flag)
    {
      SharedPopupSystem popupSystem = this._popupSystem;
      ILocalizationManager loc = this.Loc;
      EntityUid uid1 = userId.Value;
      EntityManager entityManager = this.EntityManager;
      nullable = new EntityUid?();
      EntityUid? viewer = nullable;
      (string, object) valueTuple = ("user", (object) Identity.Entity(uid1, (IEntityManager) entityManager, viewer));
      string message = loc.GetString("disposal-unit-being-inserted", valueTuple);
      EntityUid uid2 = toInsertId;
      EntityUid recipient = toInsertId;
      popupSystem.PopupEntity(message, uid2, recipient, PopupType.Large);
    }
    if ((double) seconds <= 0.0 || !userId.HasValue)
    {
      this.AfterInsert(unitId, unit, toInsertId, userId, true);
      return true;
    }
    this._doAfterSystem.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, userId.Value, seconds, (DoAfterEvent) new DisposalDoAfterEvent(), new EntityUid?(unitId), new EntityUid?(toInsertId), new EntityUid?(unitId))
    {
      BreakOnDamage = true,
      BreakOnMove = true,
      NeedHand = false
    });
    return true;
  }

  private void UpdateState(
    EntityUid uid,
    DisposalsPressureState state,
    DisposalUnitComponent component,
    MetaDataComponent metadata)
  {
    if (component.State == state)
      return;
    component.State = state;
    this.UpdateVisualState(uid, component);
    this.Dirty(uid, (IComponent) component, metadata);
    if (state != DisposalsPressureState.Ready)
      return;
    component.NextPressurized = TimeSpan.Zero;
    if (component.Engaged)
      component.NextFlush = new TimeSpan?(this.GameTiming.CurTime + component.ManualFlushTime);
    else if (((BaseContainer) component.Container).ContainedEntities.Count > 0)
      component.NextFlush = new TimeSpan?(this.GameTiming.CurTime + component.AutomaticEngageTime);
    else
      component.NextFlush = new TimeSpan?();
  }

  private void Update(EntityUid uid, DisposalUnitComponent component, MetaDataComponent metadata)
  {
    DisposalsPressureState state = this.GetState(uid, component, metadata);
    if (component.NextPressurized > this.GameTiming.CurTime)
    {
      this.UpdateState(uid, state, component, metadata);
    }
    else
    {
      if (component.NextFlush.HasValue && component.NextFlush.Value < this.GameTiming.CurTime)
        this.TryFlush(uid, component);
      this.UpdateState(uid, state, component, metadata);
    }
  }

  public bool TryFlush(EntityUid uid, DisposalUnitComponent component)
  {
    if (!this.CanFlush(uid, component))
      return false;
    if (component.NextFlush.HasValue)
      component.NextFlush = new TimeSpan?(component.NextFlush.Value + component.AutomaticEngageTime);
    BeforeDisposalFlushEvent disposalFlushEvent = new BeforeDisposalFlushEvent();
    this.RaiseLocalEvent<BeforeDisposalFlushEvent>(uid, disposalFlushEvent, false);
    if (disposalFlushEvent.Cancelled)
    {
      this.Disengage(uid, component);
      return false;
    }
    TransformComponent xform = this.Transform(uid);
    MapGridComponent mapGridComponent;
    if (!this.TryComp<MapGridComponent>(xform.GridUid, ref mapGridComponent))
      return false;
    EntityCoordinates coordinates = xform.Coordinates;
    EntityUid uid1 = this._map.GetLocal(xform.GridUid.Value, mapGridComponent, coordinates).FirstOrDefault<EntityUid>(new Func<EntityUid, bool>(((EntitySystem) this).HasComp<DisposalEntryComponent>));
    if (!EntityUid.op_Equality(uid1, new EntityUid()) && component != null)
    {
      DisposalUnitComponent disposalUnitComponent = component;
      this.HandleAir(uid, disposalUnitComponent, xform);
      this._disposalTubeSystem.TryInsert(uid1, disposalUnitComponent, (IEnumerable<string>) disposalFlushEvent.Tags);
      component.NextPressurized = this.GameTiming.CurTime;
      if (!component.DisablePressure)
        component.NextPressurized += TimeSpan.FromSeconds(20.0);
      component.Engaged = false;
      component.NextFlush = new TimeSpan?();
      this.UpdateVisualState(uid, component, true);
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
      this.UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
      return true;
    }
    component.Engaged = false;
    this.UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    return false;
  }

  protected virtual void HandleAir(
    EntityUid uid,
    DisposalUnitComponent component,
    TransformComponent xform)
  {
  }

  public void ManualEngage(
    EntityUid uid,
    DisposalUnitComponent component,
    MetaDataComponent? metadata = null)
  {
    component.Engaged = true;
    this.UpdateVisualState(uid, component);
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    this.UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
    if (!this.CanFlush(uid, component) || !this.Resolve(uid, ref metadata, true))
      return;
    TimeSpan timeSpan = this.GameTiming.CurTime - this.Metadata.GetPauseTime(uid, metadata) + component.ManualFlushTime;
    component.NextFlush = new TimeSpan?(TimeSpan.FromSeconds(Math.Min((component.NextFlush ?? TimeSpan.MaxValue).TotalSeconds, timeSpan.TotalSeconds)));
  }

  public void Disengage(EntityUid uid, DisposalUnitComponent component)
  {
    component.Engaged = false;
    if (((BaseContainer) component.Container).ContainedEntities.Count == 0)
      component.NextFlush = new TimeSpan?();
    this.UpdateVisualState(uid, component);
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    this.UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
  }

  public void TryEjectContents(EntityUid uid, DisposalUnitComponent component)
  {
    foreach (EntityUid toRemove in ((BaseContainer) component.Container).ContainedEntities.ToArray<EntityUid>())
      this.Remove(uid, component, toRemove);
    if (component.Engaged)
      return;
    component.NextFlush = new TimeSpan?();
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    this.UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
  }

  public void QueueAutomaticEngage(
    EntityUid uid,
    DisposalUnitComponent component,
    MetaDataComponent? metadata = null)
  {
    if (component.Deleted || !component.AutomaticEngage || !this._power.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(uid)) && ((BaseContainer) component.Container).ContainedEntities.Count == 0)
      return;
    TimeSpan pauseTime = this.Metadata.GetPauseTime(uid, metadata);
    TimeSpan timeSpan1 = this.GameTiming.CurTime + component.AutomaticEngageTime - pauseTime;
    TimeSpan timeSpan2 = TimeSpan.FromSeconds(Math.Min((component.NextFlush ?? TimeSpan.MaxValue).TotalSeconds, timeSpan1.TotalSeconds));
    component.NextFlush = new TimeSpan?(timeSpan2);
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    this.UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
  }

  private void OnUiButtonPressed(
    EntityUid uid,
    DisposalUnitComponent component,
    DisposalUnitComponent.UiButtonPressedMessage args)
  {
    EntityUid actor = ((BaseBoundUserInterfaceEvent) args).Actor;
    if (!((EntityUid) ref actor).Valid)
      return;
    switch (args.Button)
    {
      case DisposalUnitComponent.UiButton.Eject:
        this.TryEjectContents(uid, component);
        ISharedAdminLogManager adminLog1 = this._adminLog;
        LogStringHandler logStringHandler1 = new LogStringHandler(21, 2);
        logStringHandler1.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor)), "player", "ToPrettyString(player)");
        logStringHandler1.AppendLiteral(" hit eject button on ");
        logStringHandler1.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
        ref LogStringHandler local1 = ref logStringHandler1;
        adminLog1.Add(LogType.Action, LogImpact.Low, ref local1);
        break;
      case DisposalUnitComponent.UiButton.Engage:
        this.ToggleEngage(uid, component);
        ISharedAdminLogManager adminLog2 = this._adminLog;
        LogStringHandler logStringHandler2 = new LogStringHandler(32 /*0x20*/, 3);
        logStringHandler2.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor)), "player", "ToPrettyString(player)");
        logStringHandler2.AppendLiteral(" hit flush button on ");
        logStringHandler2.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
        logStringHandler2.AppendLiteral(", it's now ");
        logStringHandler2.AppendFormatted(component.Engaged ? "on" : "off");
        ref LogStringHandler local2 = ref logStringHandler2;
        adminLog2.Add(LogType.Action, LogImpact.Low, ref local2);
        break;
      case DisposalUnitComponent.UiButton.Power:
        this._power.TogglePower(uid, user: new EntityUid?(((BaseBoundUserInterfaceEvent) args).Actor));
        break;
      default:
        throw new ArgumentOutOfRangeException($"{this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actor)):player} attempted to hit a nonexistant button on {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
    }
  }

  public void ToggleEngage(EntityUid uid, DisposalUnitComponent component)
  {
    DisposalUnitComponent disposalUnitComponent = component;
    disposalUnitComponent.Engaged = !disposalUnitComponent.Engaged;
    if (component.Engaged)
      this.ManualEngage(uid, component);
    else
      this.Disengage(uid, component);
  }

  private void AddClimbInsideVerb(
    EntityUid uid,
    DisposalUnitComponent component,
    GetVerbsEvent<Verb> args)
  {
    if (!args.CanAccess || !args.CanInteract || ((BaseContainer) component.Container).ContainedEntities.Contains<EntityUid>(args.User) || !this.ActionBlockerSystem.CanMove(args.User) || !this.CanInsert(uid, component, args.User))
      return;
    Verb verb = new Verb()
    {
      Act = (Action) (() => this.TryInsert(uid, args.User, new EntityUid?(args.User))),
      DoContactInteraction = new bool?(true),
      Text = this.Loc.GetString("disposal-self-insert-verb-get-data-text")
    };
    args.Verbs.Add(verb);
  }
}
