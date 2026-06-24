// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Pulling.Systems.PullingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Fireman;
using Content.Shared._RMC14.Pulling;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Buckle.Components;
using Content.Shared.Cuffs.Components;
using Content.Shared.Database;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Pulling.Events;
using Content.Shared.Standing;
using Content.Shared.Verbs;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Dynamics.Joints;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared.Movement.Pulling.Systems;

public sealed class PullingSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private ActionBlockerSystem _blocker;
  [Dependency]
  private AlertsSystem _alertsSystem;
  [Dependency]
  private MovementSpeedModifierSystem _modifierSystem;
  [Dependency]
  private SharedJointSystem _joints;
  [Dependency]
  private SharedContainerSystem _containerSystem;
  [Dependency]
  private SharedHandsSystem _handsSystem;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private HeldSpeedModifierSystem _clothingMoveSpeed;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedVirtualItemSystem _virtual;
  [Dependency]
  private RMCPullingSystem _rmcPulling;

  public override void Initialize()
  {
    base.Initialize();
    this.UpdatesAfter.Add(typeof (SharedPhysicsSystem));
    this.UpdatesOutsidePrediction = true;
    this.SubscribeLocalEvent<PullableComponent, MoveInputEvent>(new ComponentEventRefHandler<PullableComponent, MoveInputEvent>(this.OnPullableMoveInput));
    this.SubscribeLocalEvent<PullableComponent, CollisionChangeEvent>(new ComponentEventRefHandler<PullableComponent, CollisionChangeEvent>(this.OnPullableCollisionChange));
    this.SubscribeLocalEvent<PullableComponent, JointRemovedEvent>(new ComponentEventHandler<PullableComponent, JointRemovedEvent>(this.OnJointRemoved));
    this.SubscribeLocalEvent<PullableComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<PullableComponent, GetVerbsEvent<Verb>>(this.AddPullVerbs));
    this.SubscribeLocalEvent<PullableComponent, EntGotInsertedIntoContainerMessage>(new EntityEventRefHandler<PullableComponent, EntGotInsertedIntoContainerMessage>(this.OnPullableContainerInsert));
    this.SubscribeLocalEvent<PullableComponent, ModifyUncuffDurationEvent>(new EntityEventRefHandler<PullableComponent, ModifyUncuffDurationEvent>(this.OnModifyUncuffDuration));
    this.SubscribeLocalEvent<PullableComponent, StopBeingPulledAlertEvent>(new EntityEventRefHandler<PullableComponent, StopBeingPulledAlertEvent>(this.OnStopBeingPulledAlert));
    this.SubscribeLocalEvent<PullerComponent, UpdateMobStateEvent>(new ComponentEventRefHandler<PullerComponent, UpdateMobStateEvent>(this.OnStateChanged), after: new Type[1]
    {
      typeof (MobThresholdSystem)
    });
    this.SubscribeLocalEvent<PullerComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<PullerComponent, AfterAutoHandleStateEvent>(this.OnAfterState));
    this.SubscribeLocalEvent<PullerComponent, EntGotInsertedIntoContainerMessage>(new EntityEventRefHandler<PullerComponent, EntGotInsertedIntoContainerMessage>(this.OnPullerContainerInsert));
    this.SubscribeLocalEvent<PullerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<PullerComponent, EntityUnpausedEvent>(this.OnPullerUnpaused));
    this.SubscribeLocalEvent<PullerComponent, VirtualItemDeletedEvent>(new ComponentEventHandler<PullerComponent, VirtualItemDeletedEvent>(this.OnVirtualItemDeleted));
    this.SubscribeLocalEvent<PullerComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<PullerComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshMovespeed));
    this.SubscribeLocalEvent<PullerComponent, DropHandItemsEvent>(new ComponentEventHandler<PullerComponent, DropHandItemsEvent>(this.OnDropHandItems));
    this.SubscribeLocalEvent<PullerComponent, StopPullingAlertEvent>(new EntityEventRefHandler<PullerComponent, StopPullingAlertEvent>(this.OnStopPullingAlert));
    this.SubscribeLocalEvent<HandsComponent, PullStartedMessage>(new ComponentEventHandler<HandsComponent, PullStartedMessage>(this.HandlePullStarted));
    this.SubscribeLocalEvent<HandsComponent, PullStoppedMessage>(new ComponentEventHandler<HandsComponent, PullStoppedMessage>(this.HandlePullStopped));
    this.SubscribeLocalEvent<PullableComponent, StrappedEvent>(new EntityEventRefHandler<PullableComponent, StrappedEvent>(this.OnBuckled));
    this.SubscribeLocalEvent<PullableComponent, BuckledEvent>(new EntityEventRefHandler<PullableComponent, BuckledEvent>(this.OnGotBuckled));
    CommandBinds.Builder.Bind(ContentKeyFunctions.ReleasePulledObject, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.OnReleasePulledObject), handle: false)).Register<PullingSystem>();
  }

  private void HandlePullStarted(EntityUid uid, HandsComponent component, PullStartedMessage args)
  {
    PullerComponent comp;
    if (args.PullerUid != uid || this.TryComp<PullerComponent>(args.PullerUid, out comp) && !comp.NeedsHands)
      return;
    this._virtual.TrySpawnVirtualItemInHand(args.PulledUid, uid);
  }

  private void HandlePullStopped(EntityUid uid, HandsComponent component, PullStoppedMessage args)
  {
    if (args.PullerUid != uid)
      return;
    foreach (EntityUid entityUid in this._handsSystem.EnumerateHeld((Entity<HandsComponent>) (uid, component)))
    {
      VirtualItemComponent comp;
      if (this.TryComp<VirtualItemComponent>(entityUid, out comp) && !(comp.BlockingEntity != args.PulledUid))
      {
        this._handsSystem.TryDrop((Entity<HandsComponent>) (args.PullerUid, component), entityUid);
        break;
      }
    }
  }

  private void OnStateChanged(
    EntityUid uid,
    PullerComponent component,
    ref UpdateMobStateEvent args)
  {
    PullableComponent comp;
    if (!component.Pulling.HasValue || !this.TryComp<PullableComponent>(component.Pulling, out comp) || args.State != MobState.Critical && args.State != MobState.Dead)
      return;
    this.TryStopPull(component.Pulling.Value, comp);
  }

  private void OnBuckled(Entity<PullableComponent> ent, ref StrappedEvent args)
  {
    EntityUid? puller = ent.Comp.Puller;
    EntityUid owner = args.Buckle.Owner;
    if ((puller.HasValue ? (puller.GetValueOrDefault() == owner ? 1 : 0) : 0) == 0 || args.Buckle.Comp.PullStrap)
      return;
    this.StopPulling((EntityUid) ent, (PullableComponent) ent);
  }

  private void OnGotBuckled(Entity<PullableComponent> ent, ref BuckledEvent args)
  {
    EntityUid? puller = ent.Comp.Puller;
    if (puller.HasValue)
    {
      EntityUid valueOrDefault1 = puller.GetValueOrDefault();
      EntityUid? nullable = this._rmcPulling.TryRetargetPull(valueOrDefault1, (EntityUid) ent);
      if (nullable.HasValue)
      {
        EntityUid valueOrDefault2 = nullable.GetValueOrDefault();
        this.TryStartPull(valueOrDefault1, valueOrDefault2);
        return;
      }
    }
    this.StopPulling((EntityUid) ent, (PullableComponent) ent);
  }

  private void OnAfterState(Entity<PullerComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    if (!ent.Comp.Pulling.HasValue)
      this.RemComp<ActivePullerComponent>(ent.Owner);
    else
      this.EnsureComp<ActivePullerComponent>(ent.Owner);
  }

  private void OnDropHandItems(EntityUid uid, PullerComponent pullerComp, DropHandItemsEvent args)
  {
    PullableComponent comp;
    if (!pullerComp.Pulling.HasValue || pullerComp.NeedsHands || !this.TryComp<PullableComponent>(pullerComp.Pulling, out comp))
      return;
    this.TryStopPull(pullerComp.Pulling.Value, comp, new EntityUid?(uid));
  }

  private void OnStopPullingAlert(Entity<PullerComponent> ent, ref StopPullingAlertEvent args)
  {
    PullableComponent comp;
    if (args.Handled || !this.TryComp<PullableComponent>(ent.Comp.Pulling, out comp))
      return;
    args.Handled = this.TryStopPull(ent.Comp.Pulling.Value, comp, new EntityUid?((EntityUid) ent));
  }

  private void OnPullerContainerInsert(
    Entity<PullerComponent> ent,
    ref EntGotInsertedIntoContainerMessage args)
  {
    PullableComponent comp;
    if (!ent.Comp.Pulling.HasValue || !this.TryComp<PullableComponent>(ent.Comp.Pulling.Value, out comp))
      return;
    this.TryStopPull(ent.Comp.Pulling.Value, comp, new EntityUid?(ent.Owner));
  }

  private void OnPullableContainerInsert(
    Entity<PullableComponent> ent,
    ref EntGotInsertedIntoContainerMessage args)
  {
    this.TryStopPull(ent.Owner, ent.Comp);
  }

  private void OnModifyUncuffDuration(
    Entity<PullableComponent> ent,
    ref ModifyUncuffDurationEvent args)
  {
    if (!ent.Comp.BeingPulled || args.User != args.Target)
      return;
    args.Duration *= 2f;
  }

  private void OnStopBeingPulledAlert(
    Entity<PullableComponent> ent,
    ref StopBeingPulledAlertEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = this.TryStopPull((EntityUid) ent, (PullableComponent) ent, new EntityUid?((EntityUid) ent));
  }

  public override void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<PullingSystem>();
  }

  private void OnPullerUnpaused(
    EntityUid uid,
    PullerComponent component,
    ref EntityUnpausedEvent args)
  {
    component.NextThrow += args.PausedTime;
  }

  private void OnVirtualItemDeleted(
    EntityUid uid,
    PullerComponent component,
    VirtualItemDeletedEvent args)
  {
    if (this._timing.ApplyingState || !component.Pulling.HasValue)
      return;
    EntityUid? pulling = component.Pulling;
    EntityUid blockingEntity = args.BlockingEntity;
    PullableComponent comp;
    if ((pulling.HasValue ? (pulling.GetValueOrDefault() != blockingEntity ? 1 : 0) : 1) != 0 || !this.TryComp<PullableComponent>(args.BlockingEntity, out comp))
      return;
    this.TryStopPull(args.BlockingEntity, comp);
  }

  private void AddPullVerbs(EntityUid uid, PullableComponent component, GetVerbsEvent<Verb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.User == args.Target)
      return;
    EntityUid? puller = component.Puller;
    EntityUid user = args.User;
    if ((puller.HasValue ? (puller.GetValueOrDefault() == user ? 1 : 0) : 0) != 0)
    {
      Verb verb = new Verb()
      {
        Text = this.Loc.GetString("pulling-verb-get-data-text-stop-pulling"),
        Act = (Action) (() => this.TryStopPull(uid, component, new EntityUid?(args.User))),
        DoContactInteraction = new bool?(false)
      };
      args.Verbs.Add(verb);
    }
    else
    {
      if (!this.CanPull(args.User, args.Target))
        return;
      Verb verb = new Verb()
      {
        Text = this.Loc.GetString("pulling-verb-get-data-text"),
        Act = (Action) (() => this.TryStartPull(args.User, args.Target)),
        DoContactInteraction = new bool?(false)
      };
      args.Verbs.Add(verb);
    }
  }

  private void OnRefreshMovespeed(
    EntityUid uid,
    PullerComponent component,
    RefreshMovementSpeedModifiersEvent args)
  {
    HeldSpeedModifierComponent comp;
    if (this.TryComp<HeldSpeedModifierComponent>(component.Pulling, out comp) && component.Pulling.HasValue)
    {
      (float walk, float sprint) = this._clothingMoveSpeed.GetHeldMovementSpeedModifiers(component.Pulling.Value, comp);
      args.ModifySpeed(walk, sprint);
    }
    else
      args.ModifySpeed(component.WalkSpeedModifier, component.SprintSpeedModifier);
  }

  private void OnPullableMoveInput(
    EntityUid uid,
    PullableComponent component,
    ref MoveInputEvent args)
  {
    if (!component.BeingPulled || !this._blocker.CanMove((EntityUid) args.Entity))
      return;
    this.TryStopPull(uid, component, new EntityUid?(uid));
  }

  private void OnPullableCollisionChange(
    EntityUid uid,
    PullableComponent component,
    ref CollisionChangeEvent args)
  {
    if (this._timing.ApplyingState || component.PullJointId == null || args.CanCollide)
      return;
    this._joints.RemoveJoint(uid, component.PullJointId);
  }

  private void OnJointRemoved(EntityUid uid, PullableComponent component, JointRemovedEvent args)
  {
    EntityUid? puller = component.Puller;
    EntityUid otherEntity = args.OtherEntity;
    if ((puller.HasValue ? (puller.GetValueOrDefault() != otherEntity ? 1 : 0) : 1) != 0 || args.Joint.ID != component.PullJointId || this._timing.ApplyingState || args.Joint.ID != component.PullJointId || !component.Puller.HasValue)
      return;
    this.StopPulling(uid, component);
  }

  private void StopPulling(EntityUid pullableUid, PullableComponent pullableComp)
  {
    if (!pullableComp.Puller.HasValue)
      return;
    if (!this._timing.ApplyingState)
    {
      if (pullableComp.PullJointId != null)
      {
        this._joints.RemoveJoint(pullableUid, pullableComp.PullJointId);
        pullableComp.PullJointId = (string) null;
      }
      PhysicsComponent comp;
      if (this.TryComp<PhysicsComponent>(pullableUid, out comp))
        this._physics.SetFixedRotation(pullableUid, pullableComp.PrevFixedRotation, body: comp);
    }
    EntityUid? puller = pullableComp.Puller;
    if (puller.HasValue)
      this.RemComp<ActivePullerComponent>(puller.Value);
    pullableComp.PullJointId = (string) null;
    pullableComp.Puller = new EntityUid?();
    this.Dirty(pullableUid, (IComponent) pullableComp);
    PullerComponent comp1;
    if (this.TryComp<PullerComponent>(puller, out comp1))
    {
      EntityUid entityUid = puller.Value;
      this._alertsSystem.ClearAlert(entityUid, comp1.PullingAlert);
      comp1.Pulling = new EntityUid?();
      this.Dirty(puller.Value, (IComponent) comp1);
      PullStoppedMessage args = new PullStoppedMessage(entityUid, pullableUid);
      this._modifierSystem.RefreshMovementSpeedModifiers(entityUid);
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(17, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entityUid), "user", "ToPrettyString(pullerUid)");
      logStringHandler.AppendLiteral(" stopped pulling ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) pullableUid), "target", "ToPrettyString(pullableUid)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
      this.RaiseLocalEvent<PullStoppedMessage>(entityUid, args);
      this.RaiseLocalEvent<PullStoppedMessage>(pullableUid, args);
    }
    this._alertsSystem.ClearAlert(pullableUid, pullableComp.PulledAlert);
  }

  public bool IsPulled(EntityUid uid, PullableComponent? component = null)
  {
    return this.Resolve<PullableComponent>(uid, ref component, false) && component.BeingPulled;
  }

  public bool IsPulling(EntityUid puller, PullerComponent? component = null)
  {
    return this.Resolve<PullerComponent>(puller, ref component, false) && component.Pulling.HasValue;
  }

  public EntityUid? GetPuller(EntityUid puller, PullableComponent? component = null)
  {
    return this.Resolve<PullableComponent>(puller, ref component, false) ? component.Puller : new EntityUid?();
  }

  public EntityUid? GetPulling(EntityUid puller, PullerComponent? component = null)
  {
    return this.Resolve<PullerComponent>(puller, ref component, false) ? component.Pulling : new EntityUid?();
  }

  private void OnReleasePulledObject(ICommonSession? session)
  {
    EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    PullerComponent comp1;
    PullableComponent comp2;
    if (!valueOrDefault.Valid || !this.TryComp<PullerComponent>(valueOrDefault, out comp1) || !this.TryComp<PullableComponent>(comp1.Pulling, out comp2))
      return;
    this.TryStopPull(comp1.Pulling.Value, comp2, new EntityUid?(valueOrDefault));
  }

  public bool CanPull(EntityUid puller, EntityUid pullableUid, PullerComponent? pullerComp = null)
  {
    PhysicsComponent comp;
    if (!this.Resolve<PullerComponent>(puller, ref pullerComp, false) || pullerComp.NeedsHands && !this._handsSystem.TryGetEmptyHand((Entity<HandsComponent>) puller, out string _) && !pullerComp.Pulling.HasValue || !this._blocker.CanInteract(puller, new EntityUid?(pullableUid)) || !this.TryComp<PhysicsComponent>(pullableUid, out comp) || comp.BodyType == BodyType.Static || puller == pullableUid || !this._containerSystem.IsInSameOrNoContainer((Entity<TransformComponent, MetaDataComponent>) puller, (Entity<TransformComponent, MetaDataComponent>) pullableUid))
      return false;
    BeingPulledAttemptEvent args1 = new BeingPulledAttemptEvent(puller, pullableUid);
    this.RaiseLocalEvent<BeingPulledAttemptEvent>(pullableUid, args1, true);
    StartPullAttemptEvent args2 = new StartPullAttemptEvent(puller, pullableUid);
    this.RaiseLocalEvent<StartPullAttemptEvent>(puller, args2, true);
    return !args2.Cancelled && !args1.Cancelled;
  }

  public bool TogglePull(Entity<PullableComponent?> pullable, EntityUid pullerUid)
  {
    if (!this.Resolve<PullableComponent>((EntityUid) pullable, ref pullable.Comp, false))
      return false;
    EntityUid? nullable = pullable.Comp.Puller;
    EntityUid entityUid = pullerUid;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() == entityUid ? 1 : 0) : 0) == 0)
      return this.TryStartPull(pullerUid, (EntityUid) pullable, pullableComp: (PullableComponent) pullable);
    RMCPullToggleEvent args = new RMCPullToggleEvent();
    this.RaiseLocalEvent<RMCPullToggleEvent>(pullerUid, ref args);
    if (args.Handled)
      return true;
    EntityUid pullableUid = (EntityUid) pullable;
    PullableComponent comp = pullable.Comp;
    nullable = new EntityUid?();
    EntityUid? user = nullable;
    return this.TryStopPull(pullableUid, comp, user);
  }

  public bool TogglePull(EntityUid pullerUid, PullerComponent puller)
  {
    PullableComponent comp;
    return this.TryComp<PullableComponent>(puller.Pulling, out comp) && this.TogglePull((Entity<PullableComponent>) (puller.Pulling.Value, comp), pullerUid);
  }

  public bool TryStartPull(
    EntityUid pullerUid,
    EntityUid pullableUid,
    PullerComponent? pullerComp = null,
    PullableComponent? pullableComp = null)
  {
    RMCGetPullTargetEvent args1 = new RMCGetPullTargetEvent(pullerUid, pullableUid);
    this.RaiseLocalEvent<RMCGetPullTargetEvent>(pullableUid, ref args1);
    if (pullableUid != args1.Target)
    {
      pullableUid = args1.Target;
      pullableComp = this.CompOrNull<PullableComponent>(pullableUid);
    }
    if (!this.Resolve<PullerComponent>(pullerUid, ref pullerComp, false) || !this.Resolve<PullableComponent>(pullableUid, ref pullableComp, false))
      return false;
    EntityUid? nullable = pullerComp.Pulling;
    EntityUid entityUid1 = pullableUid;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() == entityUid1 ? 1 : 0) : 0) != 0)
      return true;
    PhysicsComponent comp1;
    PhysicsComponent comp2;
    PullableComponent comp3;
    if (!this.CanPull(pullerUid, pullableUid) || !this.TryComp<PhysicsComponent>(pullerUid, out comp1) || !this.TryComp<PhysicsComponent>(pullableUid, out comp2) || this.TryComp<PullableComponent>(pullerComp.Pulling, out comp3) && !this.TryStopPull(pullerComp.Pulling.Value, comp3, new EntityUid?(pullerUid)))
      return false;
    if (pullableComp.Puller.HasValue)
    {
      nullable = pullableComp.Puller;
      EntityUid entityUid2 = pullerUid;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() == entityUid2 ? 1 : 0) : 0) != 0 || !this.TryStopPull(pullableUid, pullableComp, pullableComp.Puller))
        return false;
    }
    PullAttemptEvent args2 = new PullAttemptEvent(pullerUid, pullableUid);
    this.RaiseLocalEvent<PullAttemptEvent>(pullerUid, args2);
    if (args2.Cancelled)
      return false;
    this.RaiseLocalEvent<PullAttemptEvent>(pullableUid, args2);
    if (args2.Cancelled)
      return false;
    this._interaction.DoContactInteraction(pullableUid, new EntityUid?(pullerUid));
    pullableComp.PullJointId = $"pull-joint-{this.GetNetEntity(pullableUid)}";
    this.EnsureComp<ActivePullerComponent>(pullerUid);
    pullerComp.Pulling = new EntityUid?(pullableUid);
    pullableComp.Puller = new EntityUid?(pullerUid);
    pullableComp.PrevFixedRotation = comp2.FixedRotation;
    if (!this._timing.ApplyingState)
    {
      DistanceJoint distanceJoint = this._joints.CreateDistanceJoint(pullableUid, pullerUid, new Vector2?(comp2.LocalCenter), new Vector2?(comp1.LocalCenter), pullableComp.PullJointId, minimumDistance: new float?(1f));
      distanceJoint.CollideConnected = false;
      distanceJoint.MaxLength = distanceJoint.Length + 0.15f;
      distanceJoint.MinLength = 0.0f;
      distanceJoint.Stiffness = 0.0f;
      this._physics.SetFixedRotation(pullableUid, pullableComp.FixedRotationOnPull, body: comp2);
    }
    PullStartedMessage args3 = new PullStartedMessage(pullerUid, pullableUid);
    this._modifierSystem.RefreshMovementSpeedModifiers(pullerUid);
    this._alertsSystem.ShowAlert(pullerUid, pullerComp.PullingAlert);
    this._alertsSystem.ShowAlert(pullableUid, pullableComp.PulledAlert);
    this.RaiseLocalEvent<PullStartedMessage>(pullerUid, args3);
    this.RaiseLocalEvent<PullStartedMessage>(pullableUid, args3);
    this.Dirty(pullerUid, (IComponent) pullerComp);
    this.Dirty(pullableUid, (IComponent) pullableComp);
    ILocalizationManager loc = this.Loc;
    EntityUid uid = pullerUid;
    EntityManager entityManager = this.EntityManager;
    nullable = new EntityUid?();
    EntityUid? viewer = nullable;
    (string, object) valueTuple = ("puller", (object) Identity.Entity(uid, (IEntityManager) entityManager, viewer));
    this._popup.PopupEntity(loc.GetString("getting-pulled-popup", valueTuple), pullableUid, pullableUid);
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(17, 2);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) pullerUid), "user", "ToPrettyString(pullerUid)");
    logStringHandler.AppendLiteral(" started pulling ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) pullableUid), "target", "ToPrettyString(pullableUid)");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
    return true;
  }

  public bool TryStopPull(EntityUid pullableUid, PullableComponent pullable, EntityUid? user = null)
  {
    if (!pullable.Puller.HasValue)
      return true;
    if (user.HasValue && !this._blocker.CanInteract(user.Value, new EntityUid?(pullableUid)))
      return false;
    AttemptStopPullingEvent args = new AttemptStopPullingEvent(user);
    this.RaiseLocalEvent<AttemptStopPullingEvent>(pullableUid, args, true);
    if (args.Cancelled)
      return false;
    this.StopPulling(pullableUid, pullable);
    return true;
  }
}
