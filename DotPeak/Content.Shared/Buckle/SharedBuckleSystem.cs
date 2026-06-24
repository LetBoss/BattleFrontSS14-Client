// Decompiled with JetBrains decompiler
// Type: Content.Shared.Buckle.SharedBuckleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Buckle;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Standing;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Buckle.Components;
using Content.Shared.Construction;
using Content.Shared.Cuffs.Components;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Foldable;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Popups;
using Content.Shared.Pulling.Events;
using Content.Shared.Rotation;
using Content.Shared.Standing;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
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
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Buckle;

public abstract class SharedBuckleSystem : EntitySystem
{
  public static ProtoId<AlertCategoryPrototype> BuckledAlertCategory = ProtoId<AlertCategoryPrototype>.op_Implicit("Buckled");
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  private RMCBuckleSystem _rmcBuckle;
  [Dependency]
  private RMCMovementSystem _rmcMovement;
  [Dependency]
  private TagSystem _tags;
  private static readonly ProtoId<TagPrototype> WallTag = ProtoId<TagPrototype>.op_Implicit("Wall");
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private ISharedPlayerManager _playerManager;
  [Dependency]
  protected ActionBlockerSystem ActionBlocker;
  [Dependency]
  protected SharedAppearanceSystem Appearance;
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedJointSystem _joints;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedRotationVisualsSystem _rotationVisuals;
  [Dependency]
  private SharedDoAfterSystem _doAfter;

  private void InitializeBuckle()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, ComponentShutdown>(new EntityEventRefHandler<BuckleComponent, ComponentShutdown>((object) this, __methodptr(OnBuckleComponentShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, MoveEvent>(new EntityEventRefHandler<BuckleComponent, MoveEvent>((object) this, __methodptr(OnBuckleMove)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, EntParentChangedMessage>(new EntityEventRefHandler<BuckleComponent, EntParentChangedMessage>((object) this, __methodptr(OnParentChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, EntGotInsertedIntoContainerMessage>(new EntityEventRefHandler<BuckleComponent, EntGotInsertedIntoContainerMessage>((object) this, __methodptr(OnInserted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, StartPullAttemptEvent>(new EntityEventRefHandler<BuckleComponent, StartPullAttemptEvent>((object) this, __methodptr(OnPullAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, BeingPulledAttemptEvent>(new EntityEventRefHandler<BuckleComponent, BeingPulledAttemptEvent>((object) this, __methodptr(OnBeingPulledAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, PullStartedMessage>(new EntityEventRefHandler<BuckleComponent, PullStartedMessage>((object) this, __methodptr(OnPullStarted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, UnbuckleAlertEvent>(new EntityEventRefHandler<BuckleComponent, UnbuckleAlertEvent>((object) this, __methodptr(OnUnbuckleAlert)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, InsertIntoEntityStorageAttemptEvent>(new ComponentEventRefHandler<BuckleComponent, InsertIntoEntityStorageAttemptEvent>((object) this, __methodptr(OnBuckleInsertIntoEntityStorageAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, PreventCollideEvent>(new ComponentEventRefHandler<BuckleComponent, PreventCollideEvent>((object) this, __methodptr(OnBucklePreventCollide)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, DownAttemptEvent>(new ComponentEventHandler<BuckleComponent, DownAttemptEvent>((object) this, __methodptr(OnBuckleDownAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, StandAttemptEvent>(new ComponentEventHandler<BuckleComponent, StandAttemptEvent>((object) this, __methodptr(OnBuckleStandAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, ThrowPushbackAttemptEvent>(new ComponentEventHandler<BuckleComponent, ThrowPushbackAttemptEvent>((object) this, __methodptr(OnBuckleThrowPushbackAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, UpdateCanMoveEvent>(new ComponentEventHandler<BuckleComponent, UpdateCanMoveEvent>((object) this, __methodptr(OnBuckleUpdateCanMove)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, BuckleDoAfterEvent>(new EntityEventRefHandler<BuckleComponent, BuckleDoAfterEvent>((object) this, __methodptr(OnBuckleDoafter)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, DoAfterAttemptEvent<BuckleDoAfterEvent>>(new ComponentEventHandler<BuckleComponent, DoAfterAttemptEvent<BuckleDoAfterEvent>>((object) this, __methodptr(\u003CInitializeBuckle\u003Eb__6_0)), (Type[]) null, (Type[]) null);
  }

  private void OnBuckleComponentShutdown(Entity<BuckleComponent> ent, ref ComponentShutdown args)
  {
    this.Unbuckle(ent, new EntityUid?());
  }

  private void OnPullAttempt(Entity<BuckleComponent> ent, ref StartPullAttemptEvent args)
  {
    EntityUid? buckledTo = ent.Comp.BuckledTo;
    EntityUid pulled = args.Pulled;
    if ((buckledTo.HasValue ? (EntityUid.op_Equality(buckledTo.GetValueOrDefault(), pulled) ? 1 : 0) : 0) == 0 || ent.Comp.PullStrap)
      return;
    args.Cancel();
  }

  private void OnBeingPulledAttempt(Entity<BuckleComponent> ent, ref BeingPulledAttemptEvent args)
  {
    if (args.Cancelled || !ent.Comp.Buckled || this.CanUnbuckle(ent, args.Puller, false))
      return;
    args.Cancel();
  }

  private void OnPullStarted(Entity<BuckleComponent> ent, ref PullStartedMessage args)
  {
    this.Unbuckle(ent, new EntityUid?(args.PullerUid));
  }

  private void OnUnbuckleAlert(Entity<BuckleComponent> ent, ref UnbuckleAlertEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = this.TryUnbuckle(Entity<BuckleComponent>.op_Implicit(ent), new EntityUid?(Entity<BuckleComponent>.op_Implicit(ent)), Entity<BuckleComponent>.op_Implicit(ent));
  }

  private void OnParentChanged(Entity<BuckleComponent> ent, ref EntParentChangedMessage args)
  {
    this.BuckleTransformCheck(ent, ((EntParentChangedMessage) ref args).Transform);
  }

  private void OnInserted(Entity<BuckleComponent> ent, ref EntGotInsertedIntoContainerMessage args)
  {
    this.BuckleTransformCheck(ent, this.Transform(Entity<BuckleComponent>.op_Implicit(ent)));
  }

  private void OnBuckleMove(Entity<BuckleComponent> ent, ref MoveEvent ev)
  {
    this.BuckleTransformCheck(ent, ((MoveEvent) ref ev).Component);
  }

  private void BuckleTransformCheck(Entity<BuckleComponent> buckle, TransformComponent xform)
  {
    if (this._gameTiming.ApplyingState)
      return;
    EntityUid? nullable = buckle.Comp.BuckledTo;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    StrapComponent strapComponent;
    if (!this.TryComp<StrapComponent>(valueOrDefault, ref strapComponent))
    {
      this.Log.Error($"Encountered buckle entity {this.ToPrettyString(new EntityUid?(Entity<BuckleComponent>.op_Implicit(buckle)), (MetaDataComponent) null)} without a valid strap entity {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(valueOrDefault))}");
      this.SetBuckledTo(buckle, new Entity<StrapComponent>?());
    }
    else if (EntityUid.op_Inequality(xform.ParentUid, valueOrDefault) || this._container.IsEntityInContainer(Entity<BuckleComponent>.op_Implicit(buckle), (MetaDataComponent) null))
    {
      Entity<BuckleComponent> buckle1 = buckle;
      Entity<StrapComponent> strap = Entity<StrapComponent>.op_Implicit((valueOrDefault, strapComponent));
      nullable = new EntityUid?();
      EntityUid? user = nullable;
      this.Unbuckle(buckle1, strap, user);
    }
    else
    {
      if ((double) (xform.LocalPosition - strapComponent.BuckleOffset - this._rmcBuckle.GetOffset(Entity<RMCBuckleOffsetComponent>.op_Implicit(buckle.Owner))).LengthSquared() <= 1E-05)
        return;
      Entity<BuckleComponent> buckle2 = buckle;
      Entity<StrapComponent> strap = Entity<StrapComponent>.op_Implicit((valueOrDefault, strapComponent));
      nullable = new EntityUid?();
      EntityUid? user = nullable;
      this.Unbuckle(buckle2, strap, user);
    }
  }

  public void SetBuckleOffset(Entity<StrapComponent?> strap, Vector2 offset)
  {
    if (!this.Resolve<StrapComponent>(Entity<StrapComponent>.op_Implicit(strap), ref strap.Comp, false))
      return;
    strap.Comp.BuckleOffset = offset;
    this.Dirty(Entity<StrapComponent>.op_Implicit(strap), (IComponent) strap.Comp, (MetaDataComponent) null);
  }

  private void OnBuckleInsertIntoEntityStorageAttempt(
    EntityUid uid,
    BuckleComponent component,
    ref InsertIntoEntityStorageAttemptEvent args)
  {
    if (!component.Buckled)
      return;
    args.Cancelled = true;
  }

  private void OnBucklePreventCollide(
    EntityUid uid,
    BuckleComponent component,
    ref PreventCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    EntityUid? buckledTo = component.BuckledTo;
    if ((buckledTo.HasValue ? (EntityUid.op_Equality(otherEntity, buckledTo.GetValueOrDefault()) ? 1 : 0) : 0) != 0 && component.DontCollide)
      args.Cancelled = true;
    if (!component.Buckled || !this._tags.HasTag(args.OtherEntity, SharedBuckleSystem.WallTag))
      return;
    args.Cancelled = true;
  }

  private void OnBuckleDownAttempt(EntityUid uid, BuckleComponent component, DownAttemptEvent args)
  {
    if (!component.Buckled)
      return;
    args.Cancel();
  }

  private void OnBuckleStandAttempt(
    EntityUid uid,
    BuckleComponent component,
    StandAttemptEvent args)
  {
    if (!component.Buckled)
      return;
    args.Cancel();
  }

  private void OnBuckleThrowPushbackAttempt(
    EntityUid uid,
    BuckleComponent component,
    ThrowPushbackAttemptEvent args)
  {
    if (!component.Buckled)
      return;
    args.Cancel();
  }

  private void OnBuckleUpdateCanMove(
    EntityUid uid,
    BuckleComponent component,
    UpdateCanMoveEvent args)
  {
    if (this.HasComp<RMCAllowStrapMovementComponent>(component.BuckledTo) || !component.Buckled)
      return;
    args.Cancel();
  }

  public bool IsBuckled(EntityUid uid, BuckleComponent? component = null)
  {
    return this.Resolve<BuckleComponent>(uid, ref component, false) && component.Buckled;
  }

  protected void SetBuckledTo(Entity<BuckleComponent> buckle, Entity<StrapComponent?>? strap)
  {
    StrapComponent strapComponent;
    if (this.TryComp<StrapComponent>(buckle.Comp.BuckledTo, ref strapComponent))
    {
      strapComponent.BuckledEntities.Remove(Entity<BuckleComponent>.op_Implicit(buckle));
      this.Dirty(buckle.Comp.BuckledTo.Value, (IComponent) strapComponent, (MetaDataComponent) null);
    }
    if (strap.HasValue)
    {
      Entity<StrapComponent> valueOrDefault = strap.GetValueOrDefault();
      if (this.Resolve<StrapComponent>(valueOrDefault.Owner, ref valueOrDefault.Comp, true))
      {
        valueOrDefault.Comp.BuckledEntities.Add(Entity<BuckleComponent>.op_Implicit(buckle));
        this.Dirty<StrapComponent>(valueOrDefault, (MetaDataComponent) null);
        if (valueOrDefault.Comp.BuckledAlertType.HasValue)
        {
          this._alerts.ShowAlert(Entity<BuckleComponent>.op_Implicit(buckle), valueOrDefault.Comp.BuckledAlertType.Value);
          goto label_7;
        }
        goto label_7;
      }
    }
    this._alerts.ClearAlertCategory(Entity<BuckleComponent>.op_Implicit(buckle), SharedBuckleSystem.BuckledAlertCategory);
label_7:
    BuckleComponent comp = buckle.Comp;
    Entity<StrapComponent>? nullable1 = strap;
    EntityUid? nullable2 = nullable1.HasValue ? new EntityUid?(Entity<StrapComponent>.op_Implicit(nullable1.GetValueOrDefault())) : new EntityUid?();
    comp.BuckledTo = nullable2;
    buckle.Comp.BuckleTime = new TimeSpan?(this._gameTiming.CurTime);
    this.ActionBlocker.UpdateCanMove(Entity<BuckleComponent>.op_Implicit(buckle));
    this.Appearance.SetData(Entity<BuckleComponent>.op_Implicit(buckle), (Enum) StrapVisuals.State, (object) buckle.Comp.Buckled, (AppearanceComponent) null);
    this.Dirty<BuckleComponent>(buckle, (MetaDataComponent) null);
  }

  private bool CanBuckle(
    EntityUid buckleUid,
    EntityUid? user,
    EntityUid strapUid,
    bool popup,
    [NotNullWhen(true)] out StrapComponent? strapComp,
    BuckleComponent buckleComp)
  {
    strapComp = (StrapComponent) null;
    if (!this.Resolve<StrapComponent>(strapUid, ref strapComp, false) || !strapComp.Enabled || !this._rmcMovement.CanClimbOver(user, buckleUid, strapUid, false))
      return false;
    if (this._whitelistSystem.IsWhitelistFail(strapComp.Whitelist, buckleUid) || this._whitelistSystem.IsBlacklistPass(strapComp.Blacklist, buckleUid))
    {
      if (popup)
        this._popup.PopupClient(this.Loc.GetString("buckle-component-cannot-fit-message"), user, PopupType.Medium);
      return false;
    }
    if (!this._interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(buckleUid), Entity<TransformComponent>.op_Implicit(strapUid), buckleComp.Range, predicate: (SharedInteractionSystem.Ignored) (entity =>
    {
      if (!EntityUid.op_Equality(entity, buckleUid))
      {
        EntityUid entityUid = entity;
        EntityUid? nullable = user;
        if ((nullable.HasValue ? (EntityUid.op_Equality(entityUid, nullable.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
          return EntityUid.op_Equality(entity, strapUid);
      }
      return true;
    }), popup: true) || !this._container.IsInSameOrNoContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((buckleUid, (TransformComponent) null, (MetaDataComponent) null)), Entity<TransformComponent, MetaDataComponent>.op_Implicit((strapUid, (TransformComponent) null, (MetaDataComponent) null))))
      return false;
    if (user.HasValue && !this.HasComp<HandsComponent>(user))
    {
      if (popup)
        this._popup.PopupClient(this.Loc.GetString("buckle-component-no-hands-message"), user);
      return false;
    }
    if (buckleComp.Buckled && !this.TryUnbuckle(buckleUid, user, buckleComp))
    {
      if (popup)
      {
        ILocalizationManager loc = this.Loc;
        EntityUid entityUid = buckleUid;
        EntityUid? nullable = user;
        string str = (nullable.HasValue ? (EntityUid.op_Equality(entityUid, nullable.GetValueOrDefault()) ? 1 : 0) : 0) != 0 ? "buckle-component-already-buckled-message" : "buckle-component-other-already-buckled-message";
        EntityUid uid = buckleUid;
        EntityManager entityManager = this.EntityManager;
        nullable = new EntityUid?();
        EntityUid? viewer = nullable;
        (string, object) valueTuple = ("owner", (object) Identity.Entity(uid, (IEntityManager) entityManager, viewer));
        this._popup.PopupClient(loc.GetString(str, valueTuple), user);
      }
      return false;
    }
    for (EntityUid parentUid = this.Transform(strapUid).ParentUid; ((EntityUid) ref parentUid).IsValid(); parentUid = this.Transform(parentUid).ParentUid)
    {
      if (!EntityUid.op_Inequality(parentUid, buckleUid))
      {
        if (popup)
        {
          ILocalizationManager loc = this.Loc;
          EntityUid entityUid = buckleUid;
          EntityUid? nullable = user;
          string str = (nullable.HasValue ? (EntityUid.op_Equality(entityUid, nullable.GetValueOrDefault()) ? 1 : 0) : 0) != 0 ? "buckle-component-cannot-buckle-message" : "buckle-component-other-cannot-buckle-message";
          EntityUid uid = buckleUid;
          EntityManager entityManager = this.EntityManager;
          nullable = new EntityUid?();
          EntityUid? viewer = nullable;
          (string, object) valueTuple = ("owner", (object) Identity.Entity(uid, (IEntityManager) entityManager, viewer));
          this._popup.PopupClient(loc.GetString(str, valueTuple), user);
        }
        return false;
      }
    }
    if (!this.StrapHasSpace(strapUid, buckleComp, strapComp))
    {
      if (popup)
      {
        ILocalizationManager loc = this.Loc;
        EntityUid entityUid = buckleUid;
        EntityUid? nullable = user;
        string str = (nullable.HasValue ? (EntityUid.op_Equality(entityUid, nullable.GetValueOrDefault()) ? 1 : 0) : 0) != 0 ? "buckle-component-cannot-buckle-message" : "buckle-component-other-cannot-buckle-message";
        EntityUid uid = buckleUid;
        EntityManager entityManager = this.EntityManager;
        nullable = new EntityUid?();
        EntityUid? viewer = nullable;
        (string, object) valueTuple = ("owner", (object) Identity.Entity(uid, (IEntityManager) entityManager, viewer));
        this._popup.PopupClient(loc.GetString(str, valueTuple), user);
      }
      return false;
    }
    if (!this._rmcBuckle.CanBuckle(user, buckleUid, popup))
      return false;
    BuckleAttemptEvent buckleAttemptEvent = new BuckleAttemptEvent(Entity<StrapComponent>.op_Implicit((strapUid, strapComp)), Entity<BuckleComponent>.op_Implicit((buckleUid, buckleComp)), user, popup);
    this.RaiseLocalEvent<BuckleAttemptEvent>(buckleUid, ref buckleAttemptEvent, false);
    if (buckleAttemptEvent.Cancelled)
      return false;
    StrapAttemptEvent strapAttemptEvent = new StrapAttemptEvent(Entity<StrapComponent>.op_Implicit((strapUid, strapComp)), Entity<BuckleComponent>.op_Implicit((buckleUid, buckleComp)), user, popup);
    this.RaiseLocalEvent<StrapAttemptEvent>(strapUid, ref strapAttemptEvent, false);
    return !strapAttemptEvent.Cancelled;
  }

  public bool TryBuckle(
    EntityUid buckle,
    EntityUid? user,
    EntityUid strap,
    BuckleComponent? buckleComp = null,
    bool popup = true)
  {
    StrapComponent strapComp;
    if (!this.Resolve<BuckleComponent>(buckle, ref buckleComp, false) || !this.CanBuckle(buckle, user, strap, popup, out strapComp, buckleComp))
      return false;
    this.Buckle(Entity<BuckleComponent>.op_Implicit((buckle, buckleComp)), Entity<StrapComponent>.op_Implicit((strap, strapComp)), user);
    return true;
  }

  private void Buckle(
    Entity<BuckleComponent> buckle,
    Entity<StrapComponent> strap,
    EntityUid? user)
  {
    EntityUid? nullable = user;
    EntityUid owner = buckle.Owner;
    if ((nullable.HasValue ? (EntityUid.op_Equality(nullable.GetValueOrDefault(), owner) ? 1 : 0) : 0) != 0)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(23, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(user, (MetaDataComponent) null), "player", "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" buckled themselves to ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?(Entity<StrapComponent>.op_Implicit(strap)), (MetaDataComponent) null), "ToPrettyString(strap)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
    }
    else if (user.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(13, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(user, (MetaDataComponent) null), "player", "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" buckled ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?(Entity<BuckleComponent>.op_Implicit(buckle)), (MetaDataComponent) null), "ToPrettyString(buckle)");
      logStringHandler.AppendLiteral(" to ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?(Entity<StrapComponent>.op_Implicit(strap)), (MetaDataComponent) null), "ToPrettyString(strap)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
    }
    this._audio.PlayPredicted(strap.Comp.BuckleSound, Entity<StrapComponent>.op_Implicit(strap), user, new AudioParams?());
    this.SetBuckledTo(buckle, new Entity<StrapComponent>?(strap));
    this.Appearance.SetData(Entity<StrapComponent>.op_Implicit(strap), (Enum) StrapVisuals.State, (object) true, (AppearanceComponent) null);
    this.Appearance.SetData(Entity<BuckleComponent>.op_Implicit(buckle), (Enum) BuckleVisuals.Buckled, (object) true, (AppearanceComponent) null);
    this._rotationVisuals.SetHorizontalAngle(Entity<RotationVisualsComponent>.op_Implicit(buckle.Owner), strap.Comp.Rotation);
    TransformComponent transformComponent = this.Transform(Entity<BuckleComponent>.op_Implicit(buckle));
    EntityCoordinates entityCoordinates;
    // ISSUE: explicit constructor call
    ((EntityCoordinates) ref entityCoordinates).\u002Ector(Entity<StrapComponent>.op_Implicit(strap), strap.Comp.BuckleOffset + this._rmcBuckle.GetOffset(Entity<RMCBuckleOffsetComponent>.op_Implicit(buckle.Owner)));
    this._transform.SetCoordinates(Entity<BuckleComponent>.op_Implicit(buckle), transformComponent, entityCoordinates, new Angle?(Angle.Zero), true, (TransformComponent) null, (TransformComponent) null);
    this._joints.SetRelay(Entity<BuckleComponent>.op_Implicit(buckle), new EntityUid?(Entity<StrapComponent>.op_Implicit(strap)), (JointComponent) null);
    switch (strap.Comp.Position)
    {
      case StrapPosition.Stand:
        this._standing.Stand(Entity<BuckleComponent>.op_Implicit(buckle), force: true);
        break;
      case StrapPosition.Down:
        this._standing.Down(Entity<BuckleComponent>.op_Implicit(buckle), false, false, true, true);
        break;
    }
    StrappedEvent strappedEvent = new StrappedEvent(strap, buckle);
    this.RaiseLocalEvent<StrappedEvent>(Entity<StrapComponent>.op_Implicit(strap), ref strappedEvent, false);
    BuckledEvent buckledEvent = new BuckledEvent(strap, buckle);
    this.RaiseLocalEvent<BuckledEvent>(Entity<BuckleComponent>.op_Implicit(buckle), ref buckledEvent, true);
    PhysicsComponent physicsComponent;
    if (!this.TryComp<PhysicsComponent>(Entity<BuckleComponent>.op_Implicit(buckle), ref physicsComponent))
      return;
    this._physics.ResetDynamics(Entity<BuckleComponent>.op_Implicit(buckle), physicsComponent, true);
  }

  public bool TryUnbuckle(
    EntityUid buckleUid,
    EntityUid? user,
    BuckleComponent? buckleComp = null,
    bool popup = true)
  {
    return this.TryUnbuckle(Entity<BuckleComponent>.op_Implicit((buckleUid, buckleComp)), user, popup);
  }

  public bool TryUnbuckle(Entity<BuckleComponent?> buckle, EntityUid? user, bool popup)
  {
    Entity<StrapComponent> strap;
    if (!this.Resolve<BuckleComponent>(buckle.Owner, ref buckle.Comp, false) || !this.CanUnbuckle(buckle, user, popup, out strap))
      return false;
    this.Unbuckle(buckle, strap, user);
    return true;
  }

  public void Unbuckle(Entity<BuckleComponent?> buckle, EntityUid? user)
  {
    if (!this.Resolve<BuckleComponent>(buckle.Owner, ref buckle.Comp, false))
      return;
    EntityUid? buckledTo = buckle.Comp.BuckledTo;
    if (!buckledTo.HasValue)
      return;
    EntityUid valueOrDefault = buckledTo.GetValueOrDefault();
    StrapComponent strapComponent;
    if (!this.TryComp<StrapComponent>(valueOrDefault, ref strapComponent))
    {
      this.Log.Error($"Encountered buckle {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(buckle.Owner))} with invalid strap entity {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(valueOrDefault))}");
      this.SetBuckledTo(buckle, new Entity<StrapComponent>?());
    }
    else
      this.Unbuckle(buckle, Entity<StrapComponent>.op_Implicit((valueOrDefault, strapComponent)), user);
  }

  private void Unbuckle(
    Entity<BuckleComponent> buckle,
    Entity<StrapComponent> strap,
    EntityUid? user)
  {
    EntityUid? nullable = user;
    EntityUid owner = buckle.Owner;
    if ((nullable.HasValue ? (EntityUid.op_Equality(nullable.GetValueOrDefault(), owner) ? 1 : 0) : 0) != 0)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(27, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(user, (MetaDataComponent) null), nameof (user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" unbuckled themselves from ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?(Entity<StrapComponent>.op_Implicit(strap)), (MetaDataComponent) null), nameof (strap), "ToPrettyString(strap)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
    }
    else if (user.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(17, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(user, (MetaDataComponent) null), nameof (user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" unbuckled ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?(Entity<BuckleComponent>.op_Implicit(buckle)), (MetaDataComponent) null), "target", "ToPrettyString(buckle)");
      logStringHandler.AppendLiteral(" from ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?(Entity<StrapComponent>.op_Implicit(strap)), (MetaDataComponent) null), nameof (strap), "ToPrettyString(strap)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
    }
    this._audio.PlayPredicted(strap.Comp.UnbuckleSound, Entity<StrapComponent>.op_Implicit(strap), user, new AudioParams?());
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(Entity<BuckleComponent>.op_Implicit(buckle));
    this.SetBuckledTo(buckle, new Entity<StrapComponent>?());
    TransformComponent transformComponent1 = this.Transform(Entity<BuckleComponent>.op_Implicit(buckle));
    TransformComponent transformComponent2 = this.Transform(Entity<StrapComponent>.op_Implicit(strap));
    if (EntityUid.op_Equality(transformComponent1.ParentUid, strap.Owner) && !this.Terminating(transformComponent2.ParentUid, (MetaDataComponent) null))
    {
      this._transform.PlaceNextTo(Entity<TransformComponent>.op_Implicit((Entity<BuckleComponent>.op_Implicit(buckle), transformComponent1)), Entity<TransformComponent>.op_Implicit((strap.Owner, transformComponent2)));
      transformComponent1.ActivelyLerping = false;
      Angle worldRotation = this._transform.GetWorldRotation(Entity<StrapComponent>.op_Implicit(strap));
      this._transform.SetWorldRotationNoLerp(Entity<TransformComponent>.op_Implicit((Entity<BuckleComponent>.op_Implicit(buckle), transformComponent1)), worldRotation);
      if (strap.Comp.BuckleOffset + this._rmcBuckle.GetOffset(Entity<RMCBuckleOffsetComponent>.op_Implicit(buckle.Owner)) != Vector2.Zero)
        this._transform.SetCoordinates(Entity<BuckleComponent>.op_Implicit(buckle), moverCoordinates);
    }
    this._rotationVisuals.ResetHorizontalAngle(Entity<RotationVisualsComponent>.op_Implicit(buckle.Owner));
    this.Appearance.SetData(Entity<StrapComponent>.op_Implicit(strap), (Enum) StrapVisuals.State, (object) (strap.Comp.BuckledEntities.Count != 0), (AppearanceComponent) null);
    this.Appearance.SetData(Entity<BuckleComponent>.op_Implicit(buckle), (Enum) BuckleVisuals.Buckled, (object) false, (AppearanceComponent) null);
    RMCRestComponent rmcRestComponent;
    if (this.HasComp<KnockedDownComponent>(Entity<BuckleComponent>.op_Implicit(buckle)) || this._mobState.IsIncapacitated(Entity<BuckleComponent>.op_Implicit(buckle)) || this.TryComp<RMCRestComponent>(Entity<BuckleComponent>.op_Implicit(buckle), ref rmcRestComponent) && rmcRestComponent.Resting)
      this._standing.Down(Entity<BuckleComponent>.op_Implicit(buckle), false, changeCollision: true);
    else
      this._standing.Stand(Entity<BuckleComponent>.op_Implicit(buckle));
    this._joints.RefreshRelay(Entity<BuckleComponent>.op_Implicit(buckle), (JointComponent) null);
    UnbuckledEvent unbuckledEvent = new UnbuckledEvent(strap, buckle);
    this.RaiseLocalEvent<UnbuckledEvent>(Entity<BuckleComponent>.op_Implicit(buckle), ref unbuckledEvent, true);
    UnstrappedEvent unstrappedEvent = new UnstrappedEvent(strap, buckle);
    this.RaiseLocalEvent<UnstrappedEvent>(Entity<StrapComponent>.op_Implicit(strap), ref unstrappedEvent, false);
  }

  public bool CanUnbuckle(Entity<BuckleComponent?> buckle, EntityUid user, bool popup)
  {
    return this.CanUnbuckle(buckle, new EntityUid?(user), popup, out Entity<StrapComponent> _);
  }

  private bool CanUnbuckle(
    Entity<BuckleComponent?> buckle,
    EntityUid? user,
    bool popup,
    out Entity<StrapComponent> strap)
  {
    strap = new Entity<StrapComponent>();
    if (!this.Resolve<BuckleComponent>(buckle.Owner, ref buckle.Comp, true))
      return false;
    EntityUid? nullable1 = buckle.Comp.BuckledTo;
    if (!nullable1.HasValue)
      return false;
    EntityUid valueOrDefault = nullable1.GetValueOrDefault();
    StrapComponent strapComponent;
    if (!this.TryComp<StrapComponent>(valueOrDefault, ref strapComponent))
    {
      this.Log.Error($"Encountered buckle {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(buckle.Owner))} with invalid strap entity {this.ToPrettyString(new EntityUid?(Entity<StrapComponent>.op_Implicit(strap)), (MetaDataComponent) null)}");
      this.SetBuckledTo(buckle, new Entity<StrapComponent>?());
      return false;
    }
    strap = Entity<StrapComponent>.op_Implicit((valueOrDefault, strapComponent));
    TimeSpan curTime = this._gameTiming.CurTime;
    TimeSpan? buckleTime = buckle.Comp.BuckleTime;
    TimeSpan delay = buckle.Comp.Delay;
    TimeSpan? nullable2 = buckleTime.HasValue ? new TimeSpan?(buckleTime.GetValueOrDefault() + delay) : new TimeSpan?();
    if ((nullable2.HasValue ? (curTime < nullable2.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      return false;
    if (user.HasValue)
    {
      SharedInteractionSystem interaction = this._interaction;
      Entity<TransformComponent> origin = Entity<TransformComponent>.op_Implicit(user.Value);
      Entity<TransformComponent> other = Entity<TransformComponent>.op_Implicit(strap.Owner);
      double range = (double) buckle.Comp.Range;
      int num = popup ? 1 : 0;
      nullable1 = new EntityUid?();
      EntityUid? user1 = nullable1;
      if (!interaction.InRangeUnobstructed(origin, other, (float) range, popup: num != 0, user: user1) || EntityUid.op_Inequality(user.Value, buckle.Owner) && !this.ActionBlocker.CanComplexInteract(user.Value))
        return false;
    }
    UnbuckleAttemptEvent unbuckleAttemptEvent = new UnbuckleAttemptEvent(strap, buckle, user, popup);
    this.RaiseLocalEvent<UnbuckleAttemptEvent>(Entity<BuckleComponent>.op_Implicit(buckle), ref unbuckleAttemptEvent, false);
    if (unbuckleAttemptEvent.Cancelled)
      return false;
    UnstrapAttemptEvent unstrapAttemptEvent = new UnstrapAttemptEvent(strap, buckle, user, popup);
    this.RaiseLocalEvent<UnstrapAttemptEvent>(Entity<StrapComponent>.op_Implicit(strap), ref unstrapAttemptEvent, false);
    return !unstrapAttemptEvent.Cancelled;
  }

  private void OnBuckleDoafter(Entity<BuckleComponent> entity, ref BuckleDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? nullable = args.Target;
    if (!nullable.HasValue)
      return;
    nullable = args.Used;
    if (!nullable.HasValue)
      return;
    BuckleDoAfterEvent buckleDoAfterEvent = args;
    nullable = args.Target;
    EntityUid buckle = nullable.Value;
    EntityUid? user = new EntityUid?(args.User);
    nullable = args.Used;
    EntityUid strap = nullable.Value;
    int num = this.TryBuckle(buckle, user, strap, popup: false) ? 1 : 0;
    buckleDoAfterEvent.Handled = num != 0;
  }

  private void BuckleDoafterEarly(
    Entity<BuckleComponent> entity,
    BuckleDoAfterEvent args,
    CancellableEntityEventArgs ev)
  {
    if (!args.Target.HasValue || !args.Used.HasValue)
      return;
    CuffableComponent cuffableComponent;
    EntityUid? nullable;
    if (!this.TryComp<CuffableComponent>(args.Target, ref cuffableComponent) || cuffableComponent.CuffedHandCount <= 0)
    {
      MobStateSystem mobState = this._mobState;
      nullable = args.Target;
      EntityUid target = nullable.Value;
      if (!mobState.IsIncapacitated(target))
        return;
    }
    ev.Cancel();
    nullable = args.Target;
    EntityUid buckle = nullable.Value;
    EntityUid? user = new EntityUid?(args.User);
    nullable = args.Used;
    EntityUid strap = nullable.Value;
    this.TryBuckle(buckle, user, strap, popup: false);
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this.UpdatesAfter.Add(typeof (SharedInteractionSystem));
    this.UpdatesAfter.Add(typeof (SharedInputSystem));
    this.InitializeBuckle();
    this.InitializeStrap();
    this.InitializeInteraction();
  }

  private void InitializeInteraction()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrapComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<StrapComponent, GetVerbsEvent<InteractionVerb>>((object) this, __methodptr(AddStrapVerbs)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrapComponent, InteractHandEvent>(new ComponentEventHandler<StrapComponent, InteractHandEvent>((object) this, __methodptr(OnStrapInteractHand)), new Type[1]
    {
      typeof (InteractionPopupSystem)
    }, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrapComponent, DragDropTargetEvent>(new ComponentEventRefHandler<StrapComponent, DragDropTargetEvent>((object) this, __methodptr(OnStrapDragDropTarget)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrapComponent, CanDropTargetEvent>(new ComponentEventRefHandler<StrapComponent, CanDropTargetEvent>((object) this, __methodptr(OnCanDropTarget)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, InteractHandEvent>(new EntityEventRefHandler<BuckleComponent, InteractHandEvent>((object) this, __methodptr(OnBuckleInteractHand)), new Type[1]
    {
      typeof (InteractionPopupSystem)
    }, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BuckleComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<BuckleComponent, GetVerbsEvent<InteractionVerb>>((object) this, __methodptr(AddUnbuckleVerb)), (Type[]) null, (Type[]) null);
  }

  private void OnCanDropTarget(
    EntityUid uid,
    StrapComponent component,
    ref CanDropTargetEvent args)
  {
    args.CanDrop = this.StrapCanDragDropOn(uid, args.User, uid, args.Dragged, component);
    args.Handled = true;
  }

  private void OnStrapDragDropTarget(
    EntityUid uid,
    StrapComponent component,
    ref DragDropTargetEvent args)
  {
    if (!this.StrapCanDragDropOn(uid, args.User, uid, args.Dragged, component) || !this._rmcBuckle.CanBuckle(new EntityUid?(args.User), args.Dragged))
      return;
    if (EntityUid.op_Equality(args.Dragged, args.User))
    {
      BuckleComponent buckleComp;
      if (!this.TryComp<BuckleComponent>(args.User, ref buckleComp))
        return;
      args.Handled = this.TryBuckle(args.User, new EntityUid?(args.User), uid, buckleComp);
    }
    else
    {
      BuckleComponent buckleComp;
      if (!this.TryComp<BuckleComponent>(args.Dragged, ref buckleComp) || !this.CanBuckle(args.Dragged, new EntityUid?(args.User), uid, true, out StrapComponent _, buckleComp))
        return;
      float buckleDoafterTime = component.BuckleDoafterTime;
      if (buckleComp.BuckleDelay.HasValue)
        buckleDoafterTime = buckleComp.BuckleDelay.Value;
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, buckleDoafterTime, (DoAfterEvent) new BuckleDoAfterEvent(), new EntityUid?(args.Dragged), new EntityUid?(args.Dragged), new EntityUid?(uid))
      {
        BreakOnMove = true,
        BreakOnDamage = true,
        AttemptFrequency = AttemptFrequency.EveryTick
      });
    }
  }

  private bool StrapCanDragDropOn(
    EntityUid strapUid,
    EntityUid userUid,
    EntityUid targetUid,
    EntityUid buckleUid,
    StrapComponent? strapComp = null,
    BuckleComponent? buckleComp = null)
  {
    return this.Resolve<StrapComponent>(strapUid, ref strapComp, false) && this.Resolve<BuckleComponent>(buckleUid, ref buckleComp, false) && strapComp.Enabled && this._interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(targetUid), Entity<TransformComponent>.op_Implicit(buckleUid), buckleComp.Range, predicate: new SharedInteractionSystem.Ignored(Ignored));

    bool Ignored(EntityUid entity)
    {
      return EntityUid.op_Equality(entity, userUid) || EntityUid.op_Equality(entity, buckleUid) || EntityUid.op_Equality(entity, targetUid);
    }
  }

  private void OnStrapInteractHand(EntityUid uid, StrapComponent component, InteractHandEvent args)
  {
    BuckleComponent buckleComp;
    if (args.Handled || !component.Enabled || !this.TryComp<BuckleComponent>(args.User, ref buckleComp))
      return;
    if (!buckleComp.BuckledTo.HasValue && component.BuckleOnInteractHand && this.StrapHasSpace(uid, buckleComp, component))
    {
      if (!this._rmcBuckle.CanBuckle(new EntityUid?(args.User), args.User, false))
        return;
      this.TryBuckle(args.User, new EntityUid?(args.User), uid, buckleComp);
      args.Handled = true;
    }
    else
    {
      EntityUid? buckledTo = buckleComp.BuckledTo;
      EntityUid entityUid = uid;
      if ((buckledTo.HasValue ? (EntityUid.op_Equality(buckledTo.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) != 0 && this.TryUnbuckle(args.User, new EntityUid?(args.User), buckleComp))
      {
        args.Handled = true;
      }
      else
      {
        EntityUid? nullable;
        if (!Extensions.TryFirstOrNull<EntityUid>((IEnumerable<EntityUid>) component.BuckledEntities, ref nullable) || !this.TryUnbuckle(nullable.Value, new EntityUid?(args.User)))
          return;
        args.Handled = true;
      }
    }
  }

  private void OnBuckleInteractHand(Entity<BuckleComponent> ent, ref InteractHandEvent args)
  {
    if (args.Handled || !ent.Comp.ClickUnbuckle || !ent.Comp.BuckledTo.HasValue)
      return;
    args.Handled = this.TryUnbuckle(ent, new EntityUid?(args.User), true);
  }

  private void AddStrapVerbs(
    EntityUid uid,
    StrapComponent component,
    GetVerbsEvent<InteractionVerb> args)
  {
    if (args.Hands == null || !args.CanAccess || !args.CanInteract || !component.Enabled)
      return;
    foreach (EntityUid buckledEntity in component.BuckledEntities)
    {
      EntityUid entity = buckledEntity;
      BuckleComponent buckledComp = this.Comp<BuckleComponent>(entity);
      if (this._interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(args.User), Entity<TransformComponent>.op_Implicit(args.Target), buckledComp.Range))
      {
        InteractionVerb interactionVerb1 = new InteractionVerb();
        interactionVerb1.Act = (Action) (() => this.TryUnbuckle(entity, new EntityUid?(args.User), buckledComp));
        interactionVerb1.Category = VerbCategory.Unbuckle;
        interactionVerb1.Text = EntityUid.op_Equality(entity, args.User) ? this.Loc.GetString("verb-self-target-pronoun") : (string) Identity.Name(entity, (IEntityManager) this.EntityManager);
        InteractionVerb interactionVerb2 = interactionVerb1;
        args.Verbs.Add(interactionVerb2);
      }
    }
    BuckleComponent buckle;
    if (this.TryComp<BuckleComponent>(args.User, ref buckle))
    {
      EntityUid? buckledTo = buckle.BuckledTo;
      EntityUid entityUid = uid;
      if ((buckledTo.HasValue ? (EntityUid.op_Inequality(buckledTo.GetValueOrDefault(), entityUid) ? 1 : 0) : 1) != 0 && EntityUid.op_Inequality(args.User, uid) && this.StrapHasSpace(uid, buckle, component) && this._interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(args.User), Entity<TransformComponent>.op_Implicit(args.Target), buckle.Range))
      {
        InteractionVerb interactionVerb3 = new InteractionVerb();
        interactionVerb3.Act = (Action) (() => this.TryBuckle(args.User, new EntityUid?(args.User), args.Target, buckle));
        interactionVerb3.Category = VerbCategory.Buckle;
        interactionVerb3.Text = this.Loc.GetString("verb-self-target-pronoun");
        InteractionVerb interactionVerb4 = interactionVerb3;
        args.Verbs.Add(interactionVerb4);
      }
    }
    EntityUid? nullable = args.Using;
    if (!nullable.HasValue)
      return;
    EntityUid @using = nullable.GetValueOrDefault();
    BuckleComponent usingBuckle;
    if (!((EntityUid) ref @using).Valid || !this.TryComp<BuckleComponent>(@using, ref usingBuckle) || !this.StrapHasSpace(uid, usingBuckle, component))
      return;
    SharedInteractionSystem interaction1 = this._interaction;
    Entity<TransformComponent> origin1 = Entity<TransformComponent>.op_Implicit(@using);
    Entity<TransformComponent> other1 = Entity<TransformComponent>.op_Implicit(args.Target);
    double range1 = (double) usingBuckle.Range;
    nullable = new EntityUid?();
    EntityUid? user1 = nullable;
    if (!interaction1.InRangeUnobstructed(origin1, other1, (float) range1, user: user1))
      return;
    SharedInteractionSystem interaction2 = this._interaction;
    Entity<TransformComponent> origin2 = Entity<TransformComponent>.op_Implicit(@using);
    Entity<TransformComponent> other2 = Entity<TransformComponent>.op_Implicit(args.Target);
    double range2 = (double) usingBuckle.Range;
    SharedInteractionSystem.Ignored predicate = new SharedInteractionSystem.Ignored(Ignored);
    nullable = new EntityUid?();
    EntityUid? user2 = nullable;
    if (!interaction2.InRangeUnobstructed(origin2, other2, (float) range2, predicate: predicate, user: user2))
      return;
    ICommonSession icommonSession;
    bool sessionByEntity = this._playerManager.TryGetSessionByEntity(@using, ref icommonSession);
    InteractionVerb interactionVerb5 = new InteractionVerb();
    interactionVerb5.Act = (Action) (() => this.TryBuckle(@using, new EntityUid?(args.User), args.Target, usingBuckle));
    interactionVerb5.Category = VerbCategory.Buckle;
    EntityUid uid1 = @using;
    EntityManager entityManager = this.EntityManager;
    nullable = new EntityUid?();
    EntityUid? viewer = nullable;
    interactionVerb5.Text = (string) Identity.Name(uid1, (IEntityManager) entityManager, viewer);
    interactionVerb5.Priority = sessionByEntity ? 1 : -1;
    InteractionVerb interactionVerb6 = interactionVerb5;
    args.Verbs.Add(interactionVerb6);

    bool Ignored(EntityUid entity)
    {
      return EntityUid.op_Equality(entity, args.User) || EntityUid.op_Equality(entity, args.Target) || EntityUid.op_Equality(entity, @using);
    }
  }

  private void AddUnbuckleVerb(
    EntityUid uid,
    BuckleComponent component,
    GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || !component.Buckled || !this.CanUnbuckle(Entity<BuckleComponent>.op_Implicit((uid, component)), args.User, false))
      return;
    InteractionVerb interactionVerb1 = new InteractionVerb();
    interactionVerb1.Act = (Action) (() => this.TryUnbuckle(uid, new EntityUid?(args.User), component));
    interactionVerb1.Text = this.Loc.GetString("verb-categories-unbuckle");
    interactionVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/unbuckle.svg.192dpi.png"));
    InteractionVerb interactionVerb2 = interactionVerb1;
    if (EntityUid.op_Equality(args.Target, args.User) && !args.Using.HasValue)
      interactionVerb2.Priority = 1;
    args.Verbs.Add(interactionVerb2);
  }

  private void InitializeStrap()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrapComponent, ComponentStartup>(new ComponentEventHandler<StrapComponent, ComponentStartup>((object) this, __methodptr(OnStrapStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrapComponent, ComponentShutdown>(new ComponentEventHandler<StrapComponent, ComponentShutdown>((object) this, __methodptr(OnStrapShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrapComponent, EntityTerminatingEvent>(new EntityEventRefHandler<StrapComponent, EntityTerminatingEvent>((object) this, __methodptr(OnStrapTerminating)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrapComponent, ComponentRemove>(new ComponentEventHandler<StrapComponent, ComponentRemove>((object) this, __methodptr(\u003CInitializeStrap\u003Eb__62_0)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrapComponent, ContainerGettingInsertedAttemptEvent>(new ComponentEventHandler<StrapComponent, ContainerGettingInsertedAttemptEvent>((object) this, __methodptr(OnStrapContainerGettingInsertedAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrapComponent, DestructionEventArgs>(new ComponentEventHandler<StrapComponent, DestructionEventArgs>((object) this, __methodptr(\u003CInitializeStrap\u003Eb__62_1)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrapComponent, BreakageEventArgs>(new ComponentEventHandler<StrapComponent, BreakageEventArgs>((object) this, __methodptr(\u003CInitializeStrap\u003Eb__62_2)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrapComponent, FoldAttemptEvent>(new ComponentEventRefHandler<StrapComponent, FoldAttemptEvent>((object) this, __methodptr(OnAttemptFold)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StrapComponent, MachineDeconstructedEvent>(new ComponentEventHandler<StrapComponent, MachineDeconstructedEvent>((object) this, __methodptr(\u003CInitializeStrap\u003Eb__62_3)), (Type[]) null, (Type[]) null);
  }

  private void OnStrapStartup(EntityUid uid, StrapComponent component, ComponentStartup args)
  {
    this.Appearance.SetData(uid, (Enum) StrapVisuals.State, (object) (component.BuckledEntities.Count != 0), (AppearanceComponent) null);
  }

  private void OnStrapShutdown(EntityUid uid, StrapComponent component, ComponentShutdown args)
  {
    if (this.TerminatingOrDeleted(uid, (MetaDataComponent) null))
      return;
    this.StrapRemoveAll(uid, component);
  }

  private void OnStrapTerminating(Entity<StrapComponent> entity, ref EntityTerminatingEvent args)
  {
    this.StrapRemoveAll(Entity<StrapComponent>.op_Implicit(entity), entity.Comp);
  }

  private void OnStrapContainerGettingInsertedAttempt(
    EntityUid uid,
    StrapComponent component,
    ContainerGettingInsertedAttemptEvent args)
  {
    if (!(((ContainerAttemptEventBase) args).Container.ID == StorageComponent.ContainerId) || component.BuckledEntities.Count == 0)
      return;
    ((CancellableEntityEventArgs) args).Cancel();
  }

  private void OnAttemptFold(EntityUid uid, StrapComponent component, ref FoldAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    args.Cancelled = component.BuckledEntities.Count != 0;
  }

  private void StrapRemoveAll(EntityUid uid, StrapComponent strapComp)
  {
    foreach (EntityUid entityUid in strapComp.BuckledEntities.ToArray<EntityUid>())
      this.Unbuckle(Entity<BuckleComponent>.op_Implicit(entityUid), new EntityUid?(entityUid));
  }

  private bool StrapHasSpace(
    EntityUid strapUid,
    BuckleComponent buckleComp,
    StrapComponent? strapComp = null)
  {
    if (!this.Resolve<StrapComponent>(strapUid, ref strapComp, false))
      return false;
    int num1 = strapComp.Size;
    foreach (EntityUid buckledEntity in strapComp.BuckledEntities)
    {
      int num2 = num1;
      BuckleComponent buckleComponent = this.CompOrNull<BuckleComponent>(buckledEntity);
      int size = buckleComponent != null ? buckleComponent.Size : 0;
      num1 = num2 - size;
    }
    return num1 >= buckleComp.Size;
  }

  public void StrapSetEnabled(EntityUid strapUid, bool enabled, StrapComponent? strapComp = null)
  {
    if (!this.Resolve<StrapComponent>(strapUid, ref strapComp, false) || strapComp.Enabled == enabled)
      return;
    strapComp.Enabled = enabled;
    this.Dirty(strapUid, (IComponent) strapComp, (MetaDataComponent) null);
    if (enabled)
      return;
    this.StrapRemoveAll(strapUid, strapComp);
  }
}
