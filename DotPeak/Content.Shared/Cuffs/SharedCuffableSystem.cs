// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cuffs.SharedCuffableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Buckle.Components;
using Content.Shared.CombatMode;
using Content.Shared.Cuffs.Components;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Popups;
using Content.Shared.Pulling.Events;
using Content.Shared.Rejuvenate;
using Content.Shared.Stunnable;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Cuffs;

public abstract class SharedCuffableSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedVirtualItemSystem _virtualItem;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private UseDelaySystem _delay;
  [Dependency]
  private SharedCombatModeSystem _combatMode;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, HandCountChangedEvent>(new EntityEventRefHandler<CuffableComponent, HandCountChangedEvent>((object) this, __methodptr(OnHandCountChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<UncuffAttemptEvent>(new EntityEventRefHandler<UncuffAttemptEvent>((object) this, __methodptr(OnUncuffAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<CuffableComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnCuffsRemovedFromContainer)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<CuffableComponent, EntInsertedIntoContainerMessage>((object) this, __methodptr(OnCuffsInsertedIntoContainer)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, RejuvenateEvent>(new ComponentEventHandler<CuffableComponent, RejuvenateEvent>((object) this, __methodptr(OnRejuvenate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, ComponentInit>(new ComponentEventHandler<CuffableComponent, ComponentInit>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, AttemptStopPullingEvent>(new ComponentEventHandler<CuffableComponent, AttemptStopPullingEvent>((object) this, __methodptr(HandleStopPull)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, RemoveCuffsAlertEvent>(new EntityEventRefHandler<CuffableComponent, RemoveCuffsAlertEvent>((object) this, __methodptr(OnRemoveCuffsAlert)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, UpdateCanMoveEvent>(new ComponentEventHandler<CuffableComponent, UpdateCanMoveEvent>((object) this, __methodptr(HandleMoveAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, IsEquippingAttemptEvent>(new ComponentEventHandler<CuffableComponent, IsEquippingAttemptEvent>((object) this, __methodptr(OnEquipAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, IsUnequippingAttemptEvent>(new ComponentEventHandler<CuffableComponent, IsUnequippingAttemptEvent>((object) this, __methodptr(OnUnequipAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, BeingPulledAttemptEvent>(new ComponentEventHandler<CuffableComponent, BeingPulledAttemptEvent>((object) this, __methodptr(OnBeingPulledAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, BuckleAttemptEvent>(new EntityEventRefHandler<CuffableComponent, BuckleAttemptEvent>((object) this, __methodptr(OnBuckleAttemptEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, UnbuckleAttemptEvent>(new EntityEventRefHandler<CuffableComponent, UnbuckleAttemptEvent>((object) this, __methodptr(OnUnbuckleAttemptEvent)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<CuffableComponent, GetVerbsEvent<Verb>>((object) this, __methodptr(AddUncuffVerb)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, UnCuffDoAfterEvent>(new ComponentEventHandler<CuffableComponent, UnCuffDoAfterEvent>((object) this, __methodptr(OnCuffableDoAfter)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, PullStartedMessage>(new ComponentEventHandler<CuffableComponent, PullStartedMessage>((object) this, __methodptr(OnPull)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, PullStoppedMessage>(new ComponentEventHandler<CuffableComponent, PullStoppedMessage>((object) this, __methodptr(OnPull)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, DropAttemptEvent>(new ComponentEventHandler<CuffableComponent, DropAttemptEvent>((object) this, __methodptr(CheckAct)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, PickupAttemptEvent>(new ComponentEventHandler<CuffableComponent, PickupAttemptEvent>((object) this, __methodptr(CheckAct)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, AttackAttemptEvent>(new ComponentEventHandler<CuffableComponent, AttackAttemptEvent>((object) this, __methodptr(CheckAct)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, UseAttemptEvent>(new ComponentEventHandler<CuffableComponent, UseAttemptEvent>((object) this, __methodptr(CheckAct)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CuffableComponent, InteractionAttemptEvent>(new EntityEventRefHandler<CuffableComponent, InteractionAttemptEvent>((object) this, __methodptr(CheckInteract)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HandcuffComponent, AfterInteractEvent>(new ComponentEventHandler<HandcuffComponent, AfterInteractEvent>((object) this, __methodptr(OnCuffAfterInteract)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HandcuffComponent, MeleeHitEvent>(new ComponentEventHandler<HandcuffComponent, MeleeHitEvent>((object) this, __methodptr(OnCuffMeleeHit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HandcuffComponent, AddCuffDoAfterEvent>(new ComponentEventHandler<HandcuffComponent, AddCuffDoAfterEvent>((object) this, __methodptr(OnAddCuffDoAfter)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HandcuffComponent, VirtualItemDeletedEvent>(new ComponentEventHandler<HandcuffComponent, VirtualItemDeletedEvent>((object) this, __methodptr(OnCuffVirtualItemDeleted)), (Type[]) null, (Type[]) null);
  }

  private void CheckInteract(Entity<CuffableComponent> ent, ref InteractionAttemptEvent args)
  {
    if (ent.Comp.CanStillInteract)
      return;
    args.Cancelled = true;
  }

  private void OnUncuffAttempt(ref UncuffAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    if (!this.Exists(args.User) || this.Deleted(args.User, (MetaDataComponent) null))
    {
      args.Cancelled = true;
    }
    else
    {
      if (EntityUid.op_Equality(args.User, args.Target))
      {
        CuffableComponent cuffableComponent;
        if (!this.TryComp<CuffableComponent>(args.User, ref cuffableComponent))
          return;
        cuffableComponent.CanStillInteract = true;
        this.Dirty(args.User, (IComponent) cuffableComponent, (MetaDataComponent) null);
        if (!this._actionBlocker.CanInteract(args.User, new EntityUid?(args.User)))
          args.Cancelled = true;
        cuffableComponent.CanStillInteract = false;
        this.Dirty(args.User, (IComponent) cuffableComponent, (MetaDataComponent) null);
      }
      else if (!this._actionBlocker.CanInteract(args.User, new EntityUid?(args.Target)))
        args.Cancelled = true;
      if (!args.Cancelled)
        return;
      this._popup.PopupClient(this.Loc.GetString("cuffable-component-cannot-interact-message"), args.Target, new EntityUid?(args.User));
    }
  }

  private void OnStartup(EntityUid uid, CuffableComponent component, ComponentInit args)
  {
    component.Container = this._container.EnsureContainer<Container>(uid, this.Factory.GetComponentName(((object) component).GetType()), (ContainerManagerComponent) null);
  }

  private void OnRejuvenate(EntityUid uid, CuffableComponent component, RejuvenateEvent args)
  {
    this._container.EmptyContainer((BaseContainer) component.Container, true, new EntityCoordinates?(), true);
  }

  private void OnCuffsRemovedFromContainer(
    EntityUid uid,
    CuffableComponent component,
    EntRemovedFromContainerMessage args)
  {
    if (((ContainerModifiedMessage) args).Container.ID != ((BaseContainer) component.Container)?.ID)
      return;
    this._virtualItem.DeleteInHandsMatching(uid, ((ContainerModifiedMessage) args).Entity);
    this.UpdateCuffState(uid, component);
  }

  private void OnCuffsInsertedIntoContainer(
    EntityUid uid,
    CuffableComponent component,
    ContainerModifiedMessage args)
  {
    if (args.Container != component.Container)
      return;
    this.UpdateCuffState(uid, component);
  }

  public void UpdateCuffState(EntityUid uid, CuffableComponent component)
  {
    HandsComponent handsComponent;
    bool flag = this.TryComp<HandsComponent>(uid, ref handsComponent) && handsComponent.Hands.Count > component.CuffedHandCount;
    if (flag == component.CanStillInteract)
      return;
    component.CanStillInteract = flag;
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    this._actionBlocker.UpdateCanMove(uid);
    if (component.CanStillInteract)
      this._alerts.ClearAlert(uid, component.CuffedAlert);
    else
      this._alerts.ShowAlert(uid, component.CuffedAlert);
    CuffedStateChangeEvent stateChangeEvent = new CuffedStateChangeEvent();
    this.RaiseLocalEvent<CuffedStateChangeEvent>(uid, ref stateChangeEvent, false);
  }

  private void OnBeingPulledAttempt(
    EntityUid uid,
    CuffableComponent component,
    BeingPulledAttemptEvent args)
  {
    PullableComponent pullableComponent;
    if (!this.TryComp<PullableComponent>(uid, ref pullableComponent) || !pullableComponent.Puller.HasValue || component.CanStillInteract)
      return;
    args.Cancel();
  }

  private void OnBuckleAttempt(
    Entity<CuffableComponent> ent,
    EntityUid? user,
    ref bool cancelled,
    bool buckling,
    bool popup)
  {
    if (cancelled)
      return;
    EntityUid? nullable = user;
    EntityUid owner = ent.Owner;
    HandsComponent handsComponent;
    if ((nullable.HasValue ? (EntityUid.op_Inequality(nullable.GetValueOrDefault(), owner) ? 1 : 0) : 1) != 0 || !this.TryComp<HandsComponent>(Entity<CuffableComponent>.op_Implicit(ent), ref handsComponent) || ent.Comp.CuffedHandCount < handsComponent.Count)
      return;
    cancelled = true;
    if (!popup)
      return;
    this._popup.PopupClient(buckling ? this.Loc.GetString("handcuff-component-cuff-interrupt-buckled-message") : this.Loc.GetString("handcuff-component-cuff-interrupt-unbuckled-message"), Entity<CuffableComponent>.op_Implicit(ent), user);
  }

  private void OnBuckleAttemptEvent(Entity<CuffableComponent> ent, ref BuckleAttemptEvent args)
  {
    this.OnBuckleAttempt(ent, args.User, ref args.Cancelled, true, args.Popup);
  }

  private void OnUnbuckleAttemptEvent(Entity<CuffableComponent> ent, ref UnbuckleAttemptEvent args)
  {
    this.OnBuckleAttempt(ent, args.User, ref args.Cancelled, false, args.Popup);
  }

  private void OnPull(EntityUid uid, CuffableComponent component, PullMessage args)
  {
    if (component.CanStillInteract)
      return;
    this._actionBlocker.UpdateCanMove(uid);
  }

  private void HandleMoveAttempt(
    EntityUid uid,
    CuffableComponent component,
    UpdateCanMoveEvent args)
  {
    PullableComponent pullableComponent;
    if (component.CanStillInteract || !this.TryComp<PullableComponent>(uid, ref pullableComponent) || !pullableComponent.BeingPulled)
      return;
    args.Cancel();
  }

  private void HandleStopPull(
    EntityUid uid,
    CuffableComponent component,
    AttemptStopPullingEvent args)
  {
    if (!args.User.HasValue || !this.Exists(args.User.Value) || !EntityUid.op_Equality(args.User.Value, uid) || component.CanStillInteract)
      return;
    args.Cancelled = true;
  }

  private void OnRemoveCuffsAlert(Entity<CuffableComponent> ent, ref RemoveCuffsAlertEvent args)
  {
    if (args.Handled)
      return;
    EntityUid target = Entity<CuffableComponent>.op_Implicit(ent);
    EntityUid user = Entity<CuffableComponent>.op_Implicit(ent);
    CuffableComponent comp = ent.Comp;
    EntityUid? cuffsToRemove = new EntityUid?();
    CuffableComponent cuffable = comp;
    this.TryUncuff(target, user, cuffsToRemove, cuffable);
    args.Handled = true;
  }

  private void AddUncuffVerb(EntityUid uid, CuffableComponent component, GetVerbsEvent<Verb> args)
  {
    if (!args.CanAccess || component.CuffedHandCount == 0 || args.Hands == null || EntityUid.op_Inequality(args.User, args.Target) && !args.CanInteract)
      return;
    Verb verb = new Verb()
    {
      Act = (Action) (() =>
      {
        EntityUid target = uid;
        EntityUid user = args.User;
        CuffableComponent cuffableComponent = component;
        EntityUid? cuffsToRemove = new EntityUid?();
        CuffableComponent cuffable = cuffableComponent;
        this.TryUncuff(target, user, cuffsToRemove, cuffable);
      }),
      DoContactInteraction = new bool?(true),
      Text = this.Loc.GetString("uncuff-verb-get-data-text")
    };
    args.Verbs.Add(verb);
  }

  private void OnCuffableDoAfter(
    EntityUid uid,
    CuffableComponent component,
    UnCuffDoAfterEvent args)
  {
    EntityUid? target = args.Args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault1 = target.GetValueOrDefault();
    EntityUid? used = args.Args.Used;
    if (!used.HasValue)
      return;
    EntityUid valueOrDefault2 = used.GetValueOrDefault();
    if (args.Handled)
      return;
    args.Handled = true;
    EntityUid user = args.Args.User;
    if (!args.Cancelled)
      this.Uncuff(valueOrDefault1, new EntityUid?(user), valueOrDefault2, component);
    else
      this._popup.PopupClient(this.Loc.GetString("cuffable-component-remove-cuffs-fail-message"), user, new EntityUid?(user));
  }

  private void OnCuffAfterInteract(
    EntityUid uid,
    HandcuffComponent component,
    AfterInteractEvent args)
  {
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    if (!((EntityUid) ref valueOrDefault).Valid)
      return;
    if (!args.CanReach)
    {
      this._popup.PopupClient(this.Loc.GetString("handcuff-component-too-far-away-error"), args.User, new EntityUid?(args.User));
    }
    else
    {
      bool flag = this.TryCuffing(args.User, valueOrDefault, uid, component);
      args.Handled = flag;
    }
  }

  private void OnCuffMeleeHit(EntityUid uid, HandcuffComponent component, MeleeHitEvent args)
  {
    if (!args.HitEntities.Any<EntityUid>())
      return;
    this.TryCuffing(args.User, args.HitEntities.First<EntityUid>(), uid, component);
    args.Handled = true;
  }

  private void OnAddCuffDoAfter(
    EntityUid uid,
    HandcuffComponent component,
    AddCuffDoAfterEvent args)
  {
    EntityUid user = args.Args.User;
    CuffableComponent component1;
    if (!this.TryComp<CuffableComponent>(args.Args.Target, ref component1))
      return;
    EntityUid target = args.Args.Target.Value;
    if (args.Handled)
      return;
    args.Handled = true;
    if (!args.Cancelled && this.TryAddNewCuffs(target, user, uid, component1))
    {
      component.Used = true;
      this._audio.PlayPredicted(component.EndCuffSound, uid, new EntityUid?(user), new AudioParams?());
      this._popup.PopupEntity(this.Loc.GetString(EntityUid.op_Equality(user, target) ? "handcuff-component-cuff-self-observer-success-message" : "handcuff-component-cuff-observer-success-message", ("user", (object) Identity.Name(user, (IEntityManager) this.EntityManager)), ("target", (object) Identity.Entity(target, (IEntityManager) this.EntityManager))), target, Filter.Pvs(target, 2f, (IEntityManager) this.EntityManager, (ISharedPlayerManager) null, (IConfigurationManager) null).RemoveWhere((Predicate<ICommonSession>) (e =>
      {
        EntityUid? attachedEntity1 = e.AttachedEntity;
        EntityUid entityUid1 = target;
        if ((attachedEntity1.HasValue ? (EntityUid.op_Equality(attachedEntity1.GetValueOrDefault(), entityUid1) ? 1 : 0) : 0) != 0)
          return true;
        EntityUid? attachedEntity2 = e.AttachedEntity;
        EntityUid entityUid2 = user;
        return attachedEntity2.HasValue && EntityUid.op_Equality(attachedEntity2.GetValueOrDefault(), entityUid2);
      })), true);
      if (EntityUid.op_Equality(target, user))
      {
        this._popup.PopupClient(this.Loc.GetString("handcuff-component-cuff-self-success-message"), user, new EntityUid?(user));
        ISharedAdminLogManager adminLog = this._adminLog;
        LogStringHandler logStringHandler = new LogStringHandler(19, 1);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "player", "ToPrettyString(user)");
        logStringHandler.AppendLiteral(" has cuffed himself");
        ref LogStringHandler local = ref logStringHandler;
        adminLog.Add(LogType.Action, LogImpact.Medium, ref local);
      }
      else
      {
        this._popup.PopupClient(this.Loc.GetString("handcuff-component-cuff-other-success-message", ("otherName", (object) Identity.Name(target, (IEntityManager) this.EntityManager, new EntityUid?(user)))), user, new EntityUid?(user));
        this._popup.PopupClient(this.Loc.GetString("handcuff-component-cuff-by-other-success-message", ("otherName", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?(target)))), target, new EntityUid?(target));
        ISharedAdminLogManager adminLog = this._adminLog;
        LogStringHandler logStringHandler = new LogStringHandler(12, 2);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "player", "ToPrettyString(user)");
        logStringHandler.AppendLiteral(" has cuffed ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "player", "ToPrettyString(target)");
        ref LogStringHandler local = ref logStringHandler;
        adminLog.Add(LogType.Action, LogImpact.High, ref local);
      }
    }
    else if (EntityUid.op_Equality(target, user))
    {
      this._popup.PopupClient(this.Loc.GetString("handcuff-component-cuff-interrupt-self-message"), user, new EntityUid?(user));
    }
    else
    {
      this._popup.PopupClient(this.Loc.GetString("handcuff-component-cuff-interrupt-message", ("targetName", (object) Identity.Name(target, (IEntityManager) this.EntityManager, new EntityUid?(user)))), user, new EntityUid?(user));
      this._popup.PopupClient(this.Loc.GetString("handcuff-component-cuff-interrupt-other-message", ("otherName", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?(target))), ("otherEnt", (object) user)), target, new EntityUid?(target));
    }
  }

  private void OnCuffVirtualItemDeleted(
    EntityUid uid,
    HandcuffComponent component,
    VirtualItemDeletedEvent args)
  {
    this.Uncuff(args.User, new EntityUid?(), uid, cuff: component);
  }

  private void OnHandCountChanged(Entity<CuffableComponent> ent, ref HandCountChangedEvent message)
  {
    if (ent.Comp.Container == null)
      return;
    bool flag = false;
    HandsComponent handsComponent = this.CompOrNull<HandsComponent>(ent.Owner);
    int count = handsComponent != null ? handsComponent.Count : 0;
    while (ent.Comp.CuffedHandCount > count && ent.Comp.CuffedHandCount > 0)
    {
      flag = true;
      IReadOnlyList<EntityUid> containedEntities = ((BaseContainer) ent.Comp.Container).ContainedEntities;
      this._transform.PlaceNextTo(Entity<TransformComponent>.op_Implicit(containedEntities[containedEntities.Count - 1]), Entity<TransformComponent>.op_Implicit(ent.Owner));
    }
    if (!flag)
      return;
    this.UpdateCuffState(ent.Owner, ent.Comp);
  }

  private void UpdateHeldItems(EntityUid uid, EntityUid handcuff, CuffableComponent? component = null)
  {
    HandsComponent handsComponent;
    if (!this.Resolve<CuffableComponent>(uid, ref component, true) || !this.TryComp<HandsComponent>(uid, ref handsComponent))
      return;
    int num = 0;
    foreach (string enumerateHand in this._hands.EnumerateHands(Entity<HandsComponent>.op_Implicit((uid, handsComponent))))
    {
      EntityUid? held;
      if (!this._hands.TryGetHeldItem(Entity<HandsComponent>.op_Implicit((uid, handsComponent)), enumerateHand, out held))
        ++num;
      else if (!this.HasComp<UnremoveableComponent>(held))
      {
        this._hands.DoDrop(Entity<HandsComponent>.op_Implicit(uid), enumerateHand);
        ++num;
        if (num == 2)
          break;
      }
    }
    EntityUid? virtualItem1;
    if (this._virtualItem.TrySpawnVirtualItemInHand(handcuff, uid, out virtualItem1))
      this.EnsureComp<UnremoveableComponent>(virtualItem1.Value);
    EntityUid? virtualItem2;
    if (!this._virtualItem.TrySpawnVirtualItemInHand(handcuff, uid, out virtualItem2))
      return;
    this.EnsureComp<UnremoveableComponent>(virtualItem2.Value);
  }

  public bool TryAddNewCuffs(
    EntityUid target,
    EntityUid user,
    EntityUid handcuff,
    CuffableComponent? component = null,
    HandcuffComponent? cuff = null)
  {
    HandsComponent handsComponent;
    if (!this.Resolve<CuffableComponent>(target, ref component, true) || !this.Resolve<HandcuffComponent>(handcuff, ref cuff, true) || !this._interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(handcuff), Entity<TransformComponent>.op_Implicit(target)) || this.TryComp<HandsComponent>(target, ref handsComponent) && handsComponent.Count <= component.CuffedHandCount)
      return false;
    TargetHandcuffedEvent targetHandcuffedEvent = new TargetHandcuffedEvent();
    this.RaiseLocalEvent<TargetHandcuffedEvent>(target, ref targetHandcuffedEvent, false);
    this._hands.TryDrop(Entity<HandsComponent>.op_Implicit(user), handcuff);
    this._container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(handcuff), (BaseContainer) component.Container, (TransformComponent) null, false);
    this.UpdateHeldItems(target, handcuff, component);
    return true;
  }

  public bool TryCuffing(
    EntityUid user,
    EntityUid target,
    EntityUid handcuff,
    HandcuffComponent? handcuffComponent = null,
    CuffableComponent? cuffable = null)
  {
    if (!this.Resolve<HandcuffComponent>(handcuff, ref handcuffComponent, true) || !this.Resolve<CuffableComponent>(target, ref cuffable, false))
      return false;
    HandsComponent handsComponent;
    if (!this.TryComp<HandsComponent>(target, ref handsComponent))
    {
      this._popup.PopupClient(this.Loc.GetString("handcuff-component-target-has-no-hands-error", ("targetName", (object) Identity.Name(target, (IEntityManager) this.EntityManager, new EntityUid?(user)))), user, new EntityUid?(user));
      return true;
    }
    if (cuffable.CuffedHandCount >= handsComponent.Count)
    {
      this._popup.PopupClient(this.Loc.GetString("handcuff-component-target-has-no-free-hands-error", ("targetName", (object) Identity.Name(target, (IEntityManager) this.EntityManager, new EntityUid?(user)))), user, new EntityUid?(user));
      return true;
    }
    if (!this._hands.CanDrop(Entity<HandsComponent>.op_Implicit(user), handcuff))
    {
      this._popup.PopupClient(this.Loc.GetString("handcuff-component-cannot-drop-cuffs", (nameof (target), (object) Identity.Name(target, (IEntityManager) this.EntityManager, new EntityUid?(user)))), user, new EntityUid?(user));
      return false;
    }
    float seconds = handcuffComponent.CuffTime;
    if (this.HasComp<StunnedComponent>(target))
      seconds = MathF.Max(0.1f, seconds - handcuffComponent.StunBonus);
    if (this.HasComp<DisarmProneComponent>(target))
      seconds = 0.0f;
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, seconds, (DoAfterEvent) new AddCuffDoAfterEvent(), new EntityUid?(handcuff), new EntityUid?(target), new EntityUid?(handcuff))
    {
      BreakOnMove = true,
      BreakOnWeightlessMove = false,
      BreakOnDamage = true,
      NeedHand = true,
      DistanceThreshold = new float?(1f),
      ForceVisible = EntityUid.op_Inequality(user, target)
    }))
      return true;
    this._popup.PopupEntity(this.Loc.GetString(EntityUid.op_Equality(user, target) ? "handcuff-component-start-cuffing-self-observer" : "handcuff-component-start-cuffing-observer", (nameof (user), (object) Identity.Name(user, (IEntityManager) this.EntityManager)), (nameof (target), (object) Identity.Entity(target, (IEntityManager) this.EntityManager))), target, Filter.Pvs(target, 2f, (IEntityManager) this.EntityManager, (ISharedPlayerManager) null, (IConfigurationManager) null).RemoveWhere((Predicate<ICommonSession>) (e =>
    {
      EntityUid? attachedEntity1 = e.AttachedEntity;
      EntityUid entityUid1 = target;
      if ((attachedEntity1.HasValue ? (EntityUid.op_Equality(attachedEntity1.GetValueOrDefault(), entityUid1) ? 1 : 0) : 0) != 0)
        return true;
      EntityUid? attachedEntity2 = e.AttachedEntity;
      EntityUid entityUid2 = user;
      return attachedEntity2.HasValue && EntityUid.op_Equality(attachedEntity2.GetValueOrDefault(), entityUid2);
    })), true);
    if (EntityUid.op_Equality(target, user))
    {
      this._popup.PopupClient(this.Loc.GetString("handcuff-component-target-self"), user, new EntityUid?(user));
    }
    else
    {
      this._popup.PopupClient(this.Loc.GetString("handcuff-component-start-cuffing-target-message", ("targetName", (object) Identity.Name(target, (IEntityManager) this.EntityManager, new EntityUid?(user)))), user, new EntityUid?(user));
      this._popup.PopupEntity(this.Loc.GetString("handcuff-component-start-cuffing-by-other-message", ("otherName", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?(target)))), target, target);
    }
    this._audio.PlayPredicted(handcuffComponent.StartCuffSound, handcuff, new EntityUid?(user), new AudioParams?());
    return true;
  }

  public bool IsCuffed(Entity<CuffableComponent> target, bool requireFullyCuffed = true)
  {
    HandsComponent handsComponent;
    return this.TryComp<HandsComponent>(Entity<CuffableComponent>.op_Implicit(target), ref handsComponent) && target.Comp.CuffedHandCount > 0 && (!requireFullyCuffed || handsComponent.Count <= target.Comp.CuffedHandCount);
  }

  public void TryUncuff(
    EntityUid target,
    EntityUid user,
    EntityUid? cuffsToRemove = null,
    CuffableComponent? cuffable = null,
    HandcuffComponent? cuff = null)
  {
    if (!this.Resolve<CuffableComponent>(target, ref cuffable, true))
      return;
    bool flag = EntityUid.op_Equality(user, target);
    if (!cuffsToRemove.HasValue)
    {
      if (((BaseContainer) cuffable.Container).ContainedEntities.Count == 0)
        return;
      cuffsToRemove = new EntityUid?(cuffable.LastAddedCuffs);
    }
    else if (!((BaseContainer) cuffable.Container).ContainedEntities.Contains<EntityUid>(cuffsToRemove.Value))
      this.Log.Warning("A user is trying to remove handcuffs that aren't in the owner's container. This should never happen!");
    if (!this.Resolve<HandcuffComponent>(cuffsToRemove.Value, ref cuff, true))
      return;
    UncuffAttemptEvent uncuffAttemptEvent = new UncuffAttemptEvent(user, target);
    this.RaiseLocalEvent<UncuffAttemptEvent>(user, ref uncuffAttemptEvent, true);
    if (uncuffAttemptEvent.Cancelled)
      return;
    if (!flag && !this._interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target)))
    {
      this._popup.PopupClient(this.Loc.GetString("cuffable-component-cannot-remove-cuffs-too-far-message"), user, new EntityUid?(user));
    }
    else
    {
      ModifyUncuffDurationEvent uncuffDurationEvent = new ModifyUncuffDurationEvent(user, target, flag ? cuff.BreakoutTime : cuff.UncuffTime);
      this.RaiseLocalEvent<ModifyUncuffDurationEvent>(user, ref uncuffDurationEvent, false);
      float duration = uncuffDurationEvent.Duration;
      UseDelayComponent useDelayComponent;
      if (flag && (!this.TryComp<UseDelayComponent>(cuffsToRemove.Value, ref useDelayComponent) || !this._delay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((cuffsToRemove.Value, useDelayComponent)), true)))
        return;
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, duration, (DoAfterEvent) new UnCuffDoAfterEvent(), new EntityUid?(target), new EntityUid?(target), cuffsToRemove)
      {
        BreakOnMove = true,
        BreakOnWeightlessMove = false,
        BreakOnDamage = true,
        NeedHand = true,
        RequireCanInteract = false,
        DistanceThreshold = new float?(1f),
        ForceVisible = EntityUid.op_Inequality(user, target)
      }))
        return;
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(21, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user)), "player", "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" is trying to uncuff ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "subject", "ToPrettyString(target)");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.Action, LogImpact.High, ref local);
      this._popup.PopupEntity(this.Loc.GetString(EntityUid.op_Equality(user, target) ? "cuffable-component-start-uncuffing-self-observer" : "cuffable-component-start-uncuffing-observer", (nameof (user), (object) Identity.Name(user, (IEntityManager) this.EntityManager)), (nameof (target), (object) Identity.Entity(target, (IEntityManager) this.EntityManager))), target, Filter.Pvs(target, 2f, (IEntityManager) this.EntityManager, (ISharedPlayerManager) null, (IConfigurationManager) null).RemoveWhere((Predicate<ICommonSession>) (e =>
      {
        EntityUid? attachedEntity1 = e.AttachedEntity;
        EntityUid entityUid1 = target;
        if ((attachedEntity1.HasValue ? (EntityUid.op_Equality(attachedEntity1.GetValueOrDefault(), entityUid1) ? 1 : 0) : 0) != 0)
          return true;
        EntityUid? attachedEntity2 = e.AttachedEntity;
        EntityUid entityUid2 = user;
        return attachedEntity2.HasValue && EntityUid.op_Equality(attachedEntity2.GetValueOrDefault(), entityUid2);
      })), true);
      if (EntityUid.op_Equality(target, user))
      {
        this._popup.PopupClient(this.Loc.GetString("cuffable-component-start-uncuffing-self"), user, new EntityUid?(user));
      }
      else
      {
        this._popup.PopupClient(this.Loc.GetString("cuffable-component-start-uncuffing-target-message", ("targetName", (object) Identity.Name(target, (IEntityManager) this.EntityManager, new EntityUid?(user)))), user, new EntityUid?(user));
        this._popup.PopupEntity(this.Loc.GetString("cuffable-component-start-uncuffing-by-other-message", ("otherName", (object) Identity.Name(user, (IEntityManager) this.EntityManager, new EntityUid?(target)))), target, target);
      }
      this._audio.PlayPredicted(flag ? cuff.StartBreakoutSound : cuff.StartUncuffSound, target, new EntityUid?(user), new AudioParams?());
    }
  }

  public void Uncuff(
    EntityUid target,
    EntityUid? user,
    EntityUid cuffsToRemove,
    CuffableComponent? cuffable = null,
    HandcuffComponent? cuff = null)
  {
    if (!this.Resolve<CuffableComponent>(target, ref cuffable, true) || !this.Resolve<HandcuffComponent>(cuffsToRemove, ref cuff, true) || !cuff.Used || cuff.Removing || this.TerminatingOrDeleted(cuffsToRemove, (MetaDataComponent) null) || this.TerminatingOrDeleted(target, (MetaDataComponent) null))
      return;
    if (user.HasValue)
    {
      UncuffAttemptEvent uncuffAttemptEvent = new UncuffAttemptEvent(user.Value, target);
      this.RaiseLocalEvent<UncuffAttemptEvent>(user.Value, ref uncuffAttemptEvent, false);
      if (uncuffAttemptEvent.Cancelled)
        return;
    }
    cuff.Removing = true;
    cuff.Used = false;
    this._audio.PlayPredicted(cuff.EndUncuffSound, target, user, new AudioParams?());
    this._container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(cuffsToRemove), (BaseContainer) cuffable.Container, true, false, new EntityCoordinates?(), new Angle?());
    if (this._net.IsServer)
    {
      if (cuff.BreakOnRemove)
      {
        this.QueueDel(new EntityUid?(cuffsToRemove));
        EntProtoId? brokenPrototype = cuff.BrokenPrototype;
        EntityUid entity = this.Spawn(brokenPrototype.HasValue ? EntProtoId.op_Implicit(brokenPrototype.GetValueOrDefault()) : (string) null, this.Transform(cuffsToRemove).Coordinates);
        this._hands.PickupOrDrop(user, entity);
      }
      else
        this._hands.PickupOrDrop(user, cuffsToRemove);
    }
    bool flag = false;
    EntityUid? nullable;
    if (this._combatMode.IsInCombatMode(user))
    {
      EntityUid entityUid = target;
      nullable = user;
      if ((nullable.HasValue ? (EntityUid.op_Inequality(entityUid, nullable.GetValueOrDefault()) ? 1 : 0) : 1) != 0 && user.HasValue)
      {
        DisarmedEvent disarmedEvent = new DisarmedEvent(target, user.Value, 1f);
        this.RaiseLocalEvent<DisarmedEvent>(target, ref disarmedEvent, false);
        flag = true;
      }
    }
    if (cuffable.CuffedHandCount == 0)
    {
      if (user.HasValue)
      {
        if (flag)
          this._popup.PopupClient(this.Loc.GetString("cuffable-component-remove-cuffs-push-success-message", ("otherName", (object) Identity.Name(user.Value, (IEntityManager) this.EntityManager, user))), user.Value, new EntityUid?(user.Value));
        else
          this._popup.PopupClient(this.Loc.GetString("cuffable-component-remove-cuffs-success-message"), user.Value, new EntityUid?(user.Value));
      }
      EntityUid entityUid = target;
      nullable = user;
      if ((nullable.HasValue ? (EntityUid.op_Inequality(entityUid, nullable.GetValueOrDefault()) ? 1 : 0) : 1) != 0 && user.HasValue)
      {
        this._popup.PopupEntity(this.Loc.GetString("cuffable-component-remove-cuffs-by-other-success-message", ("otherName", (object) Identity.Name(user.Value, (IEntityManager) this.EntityManager, user))), target, target);
        ISharedAdminLogManager adminLog = this._adminLog;
        LogStringHandler logStringHandler = new LogStringHandler(27, 2);
        logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(user, (MetaDataComponent) null), "player", "ToPrettyString(user)");
        logStringHandler.AppendLiteral(" has successfully uncuffed ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(target)), "player", "ToPrettyString(target)");
        ref LogStringHandler local = ref logStringHandler;
        adminLog.Add(LogType.Action, LogImpact.High, ref local);
      }
      else
      {
        ISharedAdminLogManager adminLog = this._adminLog;
        LogStringHandler logStringHandler = new LogStringHandler(37, 1);
        logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(user, (MetaDataComponent) null), "player", "ToPrettyString(user)");
        logStringHandler.AppendLiteral(" has successfully uncuffed themselves");
        ref LogStringHandler local = ref logStringHandler;
        adminLog.Add(LogType.Action, LogImpact.High, ref local);
      }
    }
    else if (user.HasValue)
    {
      nullable = user;
      EntityUid entityUid = target;
      if ((nullable.HasValue ? (EntityUid.op_Inequality(nullable.GetValueOrDefault(), entityUid) ? 1 : 0) : 1) != 0)
      {
        this._popup.PopupClient(this.Loc.GetString("cuffable-component-remove-cuffs-partial-success-message", ("cuffedHandCount", (object) cuffable.CuffedHandCount), ("otherName", (object) Identity.Name(user.Value, (IEntityManager) this.EntityManager, new EntityUid?(user.Value)))), user.Value, new EntityUid?(user.Value));
        this._popup.PopupEntity(this.Loc.GetString("cuffable-component-remove-cuffs-by-other-partial-success-message", ("otherName", (object) Identity.Name(user.Value, (IEntityManager) this.EntityManager, new EntityUid?(user.Value))), ("cuffedHandCount", (object) cuffable.CuffedHandCount)), target, target);
      }
      else
        this._popup.PopupClient(this.Loc.GetString("cuffable-component-remove-cuffs-partial-success-message", ("cuffedHandCount", (object) cuffable.CuffedHandCount)), user.Value, new EntityUid?(user.Value));
    }
    cuff.Removing = false;
  }

  private void CheckAct(
    EntityUid uid,
    CuffableComponent component,
    CancellableEntityEventArgs args)
  {
    if (component.CanStillInteract)
      return;
    args.Cancel();
  }

  private void OnEquipAttempt(
    EntityUid uid,
    CuffableComponent component,
    IsEquippingAttemptEvent args)
  {
    if (!EntityUid.op_Equality(args.Equipee, uid))
      return;
    this.CheckAct(uid, component, (CancellableEntityEventArgs) args);
  }

  private void OnUnequipAttempt(
    EntityUid uid,
    CuffableComponent component,
    IsUnequippingAttemptEvent args)
  {
    if (!EntityUid.op_Equality(args.Unequipee, uid))
      return;
    this.CheckAct(uid, component, (CancellableEntityEventArgs) args);
  }

  public IReadOnlyList<EntityUid> GetAllCuffs(CuffableComponent component)
  {
    return ((BaseContainer) component.Container).ContainedEntities;
  }
}
