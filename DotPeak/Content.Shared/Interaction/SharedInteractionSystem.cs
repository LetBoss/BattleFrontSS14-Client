// Decompiled with JetBrains decompiler
// Type: Content.Shared.Interaction.SharedInteractionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CombatMode;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Storage;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.CCVar;
using Content.Shared.Chat;
using Content.Shared.CombatMode;
using Content.Shared.Database;
using Content.Shared.Ghost;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Input;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Movement.Systems;
using Content.Shared.Physics;
using Content.Shared.Players.RateLimiting;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Strip;
using Content.Shared.Tag;
using Content.Shared.Timing;
using Content.Shared.UserInterface;
using Content.Shared.Verbs;
using Content.Shared.Wall;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Interaction;

public abstract class SharedInteractionSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private ISharedChatManager _chat;
  [Dependency]
  private ActionBlockerSystem _actionBlockerSystem;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private PullingSystem _pullSystem;
  [Dependency]
  private RotateToFaceSystem _rotateToFaceSystem;
  [Dependency]
  private SharedContainerSystem _containerSystem;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private SharedPhysicsSystem _broadphase;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedVerbSystem _verbSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private SharedStrippableSystem _strippable;
  [Dependency]
  private SharedPlayerRateLimitManager _rateLimit;
  [Dependency]
  private TagSystem _tagSystem;
  [Dependency]
  private UseDelaySystem _useDelay;
  private Robust.Shared.GameObjects.EntityQuery<IgnoreUIRangeComponent> _ignoreUiRangeQuery;
  private Robust.Shared.GameObjects.EntityQuery<FixturesComponent> _fixtureQuery;
  private Robust.Shared.GameObjects.EntityQuery<ItemComponent> _itemQuery;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;
  private Robust.Shared.GameObjects.EntityQuery<HandsComponent> _handsQuery;
  private Robust.Shared.GameObjects.EntityQuery<InteractionRelayComponent> _relayQuery;
  private Robust.Shared.GameObjects.EntityQuery<CombatModeComponent> _combatQuery;
  private Robust.Shared.GameObjects.EntityQuery<WallMountComponent> _wallMountQuery;
  private Robust.Shared.GameObjects.EntityQuery<UseDelayComponent> _delayQuery;
  private Robust.Shared.GameObjects.EntityQuery<ActivatableUIComponent> _uiQuery;
  private const CollisionGroup InRangeUnobstructedMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable;
  public const float InteractionRange = 1.5f;
  public const float InteractionRangeSquared = 2.25f;
  public const float MaxRaycastRange = 100f;
  public const string RateLimitKey = "Interaction";
  private static readonly ProtoId<TagPrototype> BypassInteractionRangeChecksTag = (ProtoId<TagPrototype>) "BypassInteractionRangeChecks";
  [Dependency]
  private SharedRMCLagCompensationSystem _rmcLagCompensation;
  [Dependency]
  private INetManager _net;

  private void InitializeBlocking()
  {
    this.SubscribeLocalEvent<BlockMovementComponent, UpdateCanMoveEvent>(new ComponentEventHandler<BlockMovementComponent, UpdateCanMoveEvent>(this.OnMoveAttempt));
    this.SubscribeLocalEvent<BlockMovementComponent, UseAttemptEvent>(new ComponentEventHandler<BlockMovementComponent, UseAttemptEvent>(this.CancelEvent));
    this.SubscribeLocalEvent<BlockMovementComponent, InteractionAttemptEvent>(new EntityEventRefHandler<BlockMovementComponent, InteractionAttemptEvent>(this.CancelInteractEvent));
    this.SubscribeLocalEvent<BlockMovementComponent, DropAttemptEvent>(new ComponentEventHandler<BlockMovementComponent, DropAttemptEvent>(this.CancelEvent));
    this.SubscribeLocalEvent<BlockMovementComponent, PickupAttemptEvent>(new ComponentEventHandler<BlockMovementComponent, PickupAttemptEvent>(this.CancelEvent));
    this.SubscribeLocalEvent<BlockMovementComponent, ChangeDirectionAttemptEvent>(new ComponentEventHandler<BlockMovementComponent, ChangeDirectionAttemptEvent>(this.CancelEvent));
    this.SubscribeLocalEvent<BlockMovementComponent, ComponentStartup>(new ComponentEventHandler<BlockMovementComponent, ComponentStartup>(this.OnBlockingStartup));
    this.SubscribeLocalEvent<BlockMovementComponent, ComponentShutdown>(new ComponentEventHandler<BlockMovementComponent, ComponentShutdown>(this.OnBlockingShutdown));
  }

  private void CancelInteractEvent(
    Entity<BlockMovementComponent> ent,
    ref InteractionAttemptEvent args)
  {
    if (!ent.Comp.BlockInteraction)
      return;
    args.Cancelled = true;
  }

  private void OnMoveAttempt(
    EntityUid uid,
    BlockMovementComponent component,
    UpdateCanMoveEvent args)
  {
    if (this.HasComp<RelayInputMoverComponent>(uid))
      return;
    args.Cancel();
  }

  private void CancelEvent(
    EntityUid uid,
    BlockMovementComponent component,
    CancellableEntityEventArgs args)
  {
    args.Cancel();
  }

  private void OnBlockingStartup(
    EntityUid uid,
    BlockMovementComponent component,
    ComponentStartup args)
  {
    this._actionBlockerSystem.UpdateCanMove(uid);
  }

  private void OnBlockingShutdown(
    EntityUid uid,
    BlockMovementComponent component,
    ComponentShutdown args)
  {
    this._actionBlockerSystem.UpdateCanMove(uid);
  }

  public override void Initialize()
  {
    this._ignoreUiRangeQuery = this.GetEntityQuery<IgnoreUIRangeComponent>();
    this._fixtureQuery = this.GetEntityQuery<FixturesComponent>();
    this._itemQuery = this.GetEntityQuery<ItemComponent>();
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this._handsQuery = this.GetEntityQuery<HandsComponent>();
    this._relayQuery = this.GetEntityQuery<InteractionRelayComponent>();
    this._combatQuery = this.GetEntityQuery<CombatModeComponent>();
    this._wallMountQuery = this.GetEntityQuery<WallMountComponent>();
    this._delayQuery = this.GetEntityQuery<UseDelayComponent>();
    this._uiQuery = this.GetEntityQuery<ActivatableUIComponent>();
    this.SubscribeLocalEvent<BoundUserInterfaceCheckRangeEvent>(new EntityEventRefHandler<BoundUserInterfaceCheckRangeEvent>(this.HandleUserInterfaceRangeCheck));
    this.SubscribeLocalEvent<UserInterfaceComponent, BoundUserInterfaceMessageAttempt>(new EntityEventRefHandler<UserInterfaceComponent, BoundUserInterfaceMessageAttempt>(this.OnBoundInterfaceInteractAttempt));
    this.SubscribeAllEvent<InteractInventorySlotEvent>(new EntitySessionEventHandler<InteractInventorySlotEvent>(this.HandleInteractInventorySlotEvent));
    this.SubscribeLocalEvent<UnremoveableComponent, ContainerGettingRemovedAttemptEvent>(new ComponentEventHandler<UnremoveableComponent, ContainerGettingRemovedAttemptEvent>(this.OnRemoveAttempt));
    this.SubscribeLocalEvent<UnremoveableComponent, GotUnequippedEvent>(new ComponentEventHandler<UnremoveableComponent, GotUnequippedEvent>(this.OnUnequip));
    this.SubscribeLocalEvent<UnremoveableComponent, GotUnequippedHandEvent>(new ComponentEventHandler<UnremoveableComponent, GotUnequippedHandEvent>(this.OnUnequipHand));
    this.SubscribeLocalEvent<UnremoveableComponent, DroppedEvent>(new ComponentEventHandler<UnremoveableComponent, DroppedEvent>(this.OnDropped));
    CommandBinds.Builder.Bind(ContentKeyFunctions.AltActivateItemInWorld, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate(this.HandleAltUseInteraction))).Bind(EngineKeyFunctions.Use, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate(this.HandleUseInteraction))).Bind(ContentKeyFunctions.ActivateItemInWorld, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate(this.HandleActivateItemInWorld))).Bind(ContentKeyFunctions.TryPullObject, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate(this.HandleTryPullObject))).Register<SharedInteractionSystem>();
    this._rateLimit.Register("Interaction", new RateLimitRegistration(CCVars.InteractionRateLimitPeriod, CCVars.InteractionRateLimitCount, (Action<ICommonSession>) null, CCVars.InteractionRateLimitAnnounceAdminsDelay, new Action<ICommonSession>(this.RateLimitAlertAdmins)));
    this.InitializeBlocking();
  }

  private void RateLimitAlertAdmins(ICommonSession session)
  {
    this._chat.SendAdminAlert(this.Loc.GetString("interaction-rate-limit-admin-announcement", ("player", (object) session.Name)));
  }

  public override void Shutdown()
  {
    CommandBinds.Unregister<SharedInteractionSystem>();
    base.Shutdown();
  }

  private void OnBoundInterfaceInteractAttempt(
    Entity<UserInterfaceComponent> ent,
    ref BoundUserInterfaceMessageAttempt ev)
  {
    ActivatableUIComponent component;
    this._uiQuery.TryComp(ev.Target, out component);
    if (!this._actionBlockerSystem.CanInteract(ev.Actor, new EntityUid?(ev.Target)) && (!(ev.Message is OpenBoundInterfaceMessage) || !this.HasComp<GhostComponent>(ev.Actor) || component != null && component.BlockSpectators))
      ev.Cancel();
    else if ((double) this._ui.GetUiRange((Entity<UserInterfaceComponent>) ev.Target, ev.UiKey) <= 0.0 && !this.IsAccessible((Entity<TransformComponent>) ev.Actor, (Entity<TransformComponent>) ev.Target))
    {
      ev.Cancel();
    }
    else
    {
      if (component == null)
        return;
      if (component.SingleUser && component.CurrentSingleUser.HasValue)
      {
        EntityUid? currentSingleUser = component.CurrentSingleUser;
        EntityUid actor = ev.Actor;
        if ((currentSingleUser.HasValue ? (currentSingleUser.GetValueOrDefault() != actor ? 1 : 0) : 1) != 0)
        {
          ev.Cancel();
          return;
        }
      }
      if (!component.RequiresComplex || this._actionBlockerSystem.CanComplexInteract(ev.Actor))
        return;
      ev.Cancel();
    }
  }

  private bool UiRangeCheck(
    Entity<TransformComponent?> user,
    Entity<TransformComponent?> target,
    float range)
  {
    if (!this.Resolve((EntityUid) target, ref target.Comp))
      return false;
    return user.Owner == target.Owner || target.Comp.ParentUid == user.Owner || this.InRangeAndAccessible(user, target, range) || this._ignoreUiRangeQuery.HasComp((EntityUid) user);
  }

  private void OnRemoveAttempt(
    EntityUid uid,
    UnremoveableComponent item,
    ContainerGettingRemovedAttemptEvent args)
  {
    if (this._gameTiming.ApplyingState)
      return;
    args.Cancel();
  }

  private void OnUnequip(EntityUid uid, UnremoveableComponent item, GotUnequippedEvent args)
  {
    if (!item.DeleteOnDrop)
      this.RemCompDeferred<UnremoveableComponent>(uid);
    else
      this.PredictedQueueDel(uid);
  }

  private void OnUnequipHand(
    EntityUid uid,
    UnremoveableComponent item,
    GotUnequippedHandEvent args)
  {
    if (!item.DeleteOnDrop)
      this.RemCompDeferred<UnremoveableComponent>(uid);
    else
      this.PredictedQueueDel(uid);
  }

  private void OnDropped(EntityUid uid, UnremoveableComponent item, DroppedEvent args)
  {
    if (!item.DeleteOnDrop)
      this.RemCompDeferred<UnremoveableComponent>(uid);
    else
      this.PredictedQueueDel(uid);
  }

  private bool HandleTryPullObject(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
  {
    this._rmcLagCompensation.SendLastRealTick();
    EntityUid? userEntity;
    if (!this.ValidateClientInput(session, coords, uid, out userEntity))
    {
      this.Log.Info("TryPullObject input validation failed");
      return true;
    }
    if (userEntity.Value == uid || this.Deleted(uid) || !this.InRangeUnobstructed((Entity<TransformComponent>) userEntity.Value, (Entity<TransformComponent>) uid, popup: true))
      return false;
    this._pullSystem.TogglePull((Entity<PullableComponent>) uid, userEntity.Value);
    return false;
  }

  private void HandleInteractInventorySlotEvent(
    InteractInventorySlotEvent msg,
    EntitySessionEventArgs args)
  {
    EntityUid entity = this.GetEntity(msg.ItemUid);
    TransformComponent comp;
    EntityUid? userEntity;
    if (!this.TryComp(entity, out comp) || !this.ValidateClientInput(args.SenderSession, comp.Coordinates, entity, out userEntity))
      this.Log.Info($"Inventory interaction validation failed.  Session={args.SenderSession}");
    else if (msg.AltInteract)
      this.UserInteraction(userEntity.Value, comp.Coordinates, new EntityUid?(entity), msg.AltInteract);
    else
      this.InteractionActivate(userEntity.Value, entity);
  }

  public bool HandleAltUseInteraction(
    ICommonSession? session,
    EntityCoordinates coords,
    EntityUid uid)
  {
    this._rmcLagCompensation.SendLastRealTick();
    EntityUid? userEntity;
    if (!this.ValidateClientInput(session, coords, uid, out userEntity))
    {
      this.Log.Info("Alt-use input validation failed");
      return true;
    }
    this.UserInteraction(userEntity.Value, coords, new EntityUid?(uid), true, checkAccess: this.ShouldCheckAccess(userEntity.Value));
    return false;
  }

  public bool HandleUseInteraction(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
  {
    this._rmcLagCompensation.SendLastRealTick();
    EntityUid? userEntity;
    if (!this.ValidateClientInput(session, coords, uid, out userEntity))
    {
      this.Log.Info("Use input validation failed");
      return true;
    }
    this.UserInteraction(userEntity.Value, coords, !this.Deleted(uid) ? new EntityUid?(uid) : new EntityUid?(), checkAccess: this.ShouldCheckAccess(userEntity.Value));
    return false;
  }

  private bool ShouldCheckAccess(EntityUid user)
  {
    return !this._tagSystem.HasTag(user, SharedInteractionSystem.BypassInteractionRangeChecksTag);
  }

  public bool CombatModeCanHandInteract(EntityUid user, EntityUid? target)
  {
    RMCCombatModeInteractOverrideUserEvent args1 = new RMCCombatModeInteractOverrideUserEvent(target);
    this.RaiseLocalEvent<RMCCombatModeInteractOverrideUserEvent>(user, ref args1);
    if (args1.Handled)
      return args1.CanInteract;
    HandsComponent component;
    if (!target.HasValue || !this._handsQuery.TryComp(user, out component) || this._hands.GetActiveItem((Entity<HandsComponent>) (user, component)).HasValue || !this._itemQuery.HasComp(target))
      return false;
    CombatModeShouldHandInteractEvent args2 = new CombatModeShouldHandInteractEvent(user);
    this.RaiseLocalEvent<CombatModeShouldHandInteractEvent>(target.Value, ref args2);
    return !args2.Cancelled;
  }

  public void UserInteraction(
    EntityUid user,
    EntityCoordinates coordinates,
    EntityUid? target,
    bool altInteract = false,
    bool checkCanInteract = true,
    bool checkAccess = true,
    bool checkCanUse = true)
  {
    InteractionRelayComponent component1;
    if (this._relayQuery.TryComp(user, out component1) && component1.RelayEntity.HasValue && this._actionBlockerSystem.CanInteract(user, target))
    {
      this.UserInteraction(component1.RelayEntity.Value, coordinates, target, altInteract, checkCanInteract, checkAccess, checkCanUse);
    }
    else
    {
      CombatModeComponent component2;
      if (target.HasValue && this.Deleted(target.Value) || !altInteract && this._combatQuery.TryComp(user, out component2) && component2.IsInCombatMode && !this.CombatModeCanHandInteract(user, target) || !this.ValidateInteractAndFace(user, coordinates))
        return;
      if (altInteract && target.HasValue)
      {
        this.AltInteract(user, target.Value);
      }
      else
      {
        if (checkCanInteract && !this._actionBlockerSystem.CanInteract(user, target) || checkAccess && target.HasValue && !this.IsAccessible((Entity<TransformComponent>) user, (Entity<TransformComponent>) target.Value))
          return;
        bool inRangeUnobstructed = !target.HasValue ? !checkAccess || this.InRangeUnobstructed(user, coordinates) : !checkAccess || this.InRangeUnobstructed((Entity<TransformComponent>) user, (Entity<TransformComponent>) target.Value);
        EntityUid? used;
        if (!this.TryGetUsedEntity(user, out used, checkCanUse))
        {
          if (!inRangeUnobstructed || !target.HasValue)
            return;
          this.InteractHand(user, target.Value);
        }
        else
        {
          EntityUid? nullable1 = target;
          EntityUid? nullable2 = used;
          if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
            this.UseInHandInteraction(user, target.Value, false, false);
          else if (inRangeUnobstructed && target.HasValue)
            this.InteractUsing(user, used.Value, target.Value, coordinates, false, false);
          else
            this.InteractUsingRanged(user, used.Value, target, coordinates, inRangeUnobstructed);
        }
      }
    }
  }

  private bool IsDeleted(EntityUid uid)
  {
    return this.TerminatingOrDeleted(uid) || this.EntityManager.IsQueuedForDeletion(uid);
  }

  private bool IsDeleted(EntityUid? uid) => uid.HasValue && this.IsDeleted(uid.Value);

  public void InteractHand(EntityUid user, EntityUid target)
  {
    if (this.IsDeleted(user) || this.IsDeleted(target))
      return;
    bool flag = this._actionBlockerSystem.CanComplexInteract(user);
    if (!flag)
    {
      this.InteractionActivate(user, target, false, checkAccess: false, complexInteractions: new bool?(flag), checkDeletion: false);
    }
    else
    {
      BeforeInteractHandEvent args1 = new BeforeInteractHandEvent(target);
      this.RaiseLocalEvent<BeforeInteractHandEvent>(user, args1);
      if (args1.Handled)
      {
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(55, 2);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
        logStringHandler.AppendLiteral(" interacted with ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), nameof (target), "ToPrettyString(target)");
        logStringHandler.AppendLiteral(", but it was handled by another system");
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.InteractHand, LogImpact.Low, ref local);
      }
      else
      {
        InteractHandEvent args2 = new InteractHandEvent(user, target);
        this.RaiseLocalEvent<InteractHandEvent>(target, args2, true);
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(17, 2);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
        logStringHandler.AppendLiteral(" interacted with ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), nameof (target), "ToPrettyString(target)");
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.InteractHand, LogImpact.Low, ref local);
        this.DoContactInteraction(user, new EntityUid?(target), (HandledEntityEventArgs) args2);
        if (args2.Handled)
          return;
        this.InteractionActivate(user, target, false, checkAccess: false, complexInteractions: new bool?(flag), checkDeletion: false);
      }
    }
  }

  public void InteractUsingRanged(
    EntityUid user,
    EntityUid used,
    EntityUid? target,
    EntityCoordinates clickLocation,
    bool inRangeUnobstructed)
  {
    if (this.IsDeleted(user) || this.IsDeleted(used) || this.IsDeleted(target))
      return;
    if (target.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(24, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" interacted with ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(target), nameof (target), "ToPrettyString(target)");
      logStringHandler.AppendLiteral(" using ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) used), nameof (used), "ToPrettyString(used)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.InteractUsing, LogImpact.Low, ref local);
    }
    else
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(33, 2);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
      logStringHandler.AppendLiteral(" interacted with *nothing* using ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) used), nameof (used), "ToPrettyString(used)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.InteractUsing, LogImpact.Low, ref local);
    }
    if (this.RangedInteractDoBefore(user, used, target, clickLocation, inRangeUnobstructed, false))
      return;
    if (target.HasValue)
    {
      RangedInteractEvent args = new RangedInteractEvent(user, used, target.Value, clickLocation);
      this.RaiseLocalEvent<RangedInteractEvent>(target.Value, args, true);
      this.DoContactInteraction(user, new EntityUid?(used), (HandledEntityEventArgs) args);
      if (args.Handled)
        return;
    }
    this.InteractDoAfter(user, used, target, clickLocation, inRangeUnobstructed, false);
  }

  protected bool ValidateInteractAndFace(EntityUid user, EntityCoordinates coordinates)
  {
    if (this._transform.GetMapId(coordinates) != this.Transform(user).MapID)
      return false;
    InputMoverComponent comp;
    if (!this.HasComp<NoRotateOnInteractComponent>(user) && (!this.TryComp<InputMoverComponent>(user, out comp) || (comp.HeldMoveButtons & MoveButtons.AnyDirection) == MoveButtons.None))
      this._rotateToFaceSystem.TryFaceCoordinates(user, this._transform.ToMapCoordinates(coordinates).Position);
    return true;
  }

  public float UnobstructedDistance(
    MapCoordinates origin,
    MapCoordinates other,
    int collisionMask = 130,
    SharedInteractionSystem.Ignored? predicate = null)
  {
    Vector2 vector2 = other.Position - origin.Position;
    if (vector2.LengthSquared().Equals(0.0f))
      return 0.0f;
    if (predicate == null)
      predicate = (SharedInteractionSystem.Ignored) (_ => false);
    CollisionRay ray = new CollisionRay(origin.Position, Vector2Helpers.Normalized(vector2), collisionMask);
    List<RayCastResults> list = this._broadphase.IntersectRayWithPredicate(origin.MapId, ray, vector2.Length(), new Func<EntityUid, bool>(predicate.Invoke), false).ToList<RayCastResults>();
    return list.Count == 0 ? vector2.Length() : (list[0].HitPos - origin.Position).Length();
  }

  public bool InRangeUnobstructed(
    MapCoordinates origin,
    MapCoordinates other,
    float range = 1.5f,
    CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable,
    SharedInteractionSystem.Ignored? predicate = null,
    bool checkAccess = true)
  {
    if (other.MapId != origin.MapId)
      return false;
    if (!checkAccess)
      return true;
    Vector2 vector2 = other.Position - origin.Position;
    float maxLength = vector2.Length();
    if ((double) range > 0.0 && (double) maxLength > (double) range)
      return false;
    if (MathHelper.CloseTo(maxLength, 0.0f, 1E-07f))
      return true;
    if (predicate == null)
      predicate = (SharedInteractionSystem.Ignored) (_ => false);
    if ((double) maxLength > 100.0)
    {
      this.Log.Warning("InRangeUnobstructed check performed over extreme range. Limiting CollisionRay size.");
      maxLength = 100f;
    }
    CollisionRay ray = new CollisionRay(origin.Position, Vector2Helpers.Normalized(vector2), (int) collisionMask);
    return this._broadphase.IntersectRayWithPredicate(origin.MapId, ray, maxLength, new Func<EntityUid, bool>(predicate.Invoke), false).ToList<RayCastResults>().Count == 0;
  }

  public bool InRangeUnobstructed(
    Entity<TransformComponent?> origin,
    Entity<TransformComponent?> other,
    float range = 1.5f,
    CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable,
    SharedInteractionSystem.Ignored? predicate = null,
    bool popup = false,
    bool overlapCheck = true,
    EntityUid? user = null,
    bool lagCompensate = true)
  {
    if (!this.Resolve((EntityUid) other, ref other.Comp))
      return false;
    InRangeOverrideEvent args = new InRangeOverrideEvent((EntityUid) origin, (EntityUid) other);
    this.RaiseLocalEvent<InRangeOverrideEvent>((EntityUid) origin, ref args);
    if (args.Handled)
      return args.InRange;
    EntityCoordinates coordinates = other.Comp.Coordinates;
    Angle localRotation = other.Comp.LocalRotation;
    ActorComponent comp;
    if (lagCompensate && this.TryComp<ActorComponent>(user ?? (EntityUid) origin, out comp))
      (coordinates, localRotation) = this._rmcLagCompensation.GetCoordinatesAngle((EntityUid) other, comp.PlayerSession);
    return this.InRangeUnobstructed(origin, other, coordinates, localRotation, range, collisionMask, predicate, popup, overlapCheck);
  }

  public bool InRangeUnobstructed(
    Entity<TransformComponent?> origin,
    Entity<TransformComponent?> other,
    EntityCoordinates otherCoordinates,
    Angle otherAngle,
    float range = 1.5f,
    CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable,
    SharedInteractionSystem.Ignored? predicate = null,
    bool popup = false,
    bool overlapCheck = true)
  {
    if (this._net.IsServer)
      range += this._rmcLagCompensation.MarginTiles;
    if (origin.Owner == other.Owner && this.Resolve((EntityUid) other, ref other.Comp, false))
    {
      otherCoordinates = other.Comp.Coordinates;
      otherAngle = other.Comp.LocalRotation;
    }
    SharedInteractionSystem.Ignored predicate1 = (SharedInteractionSystem.Ignored) (e =>
    {
      if (e == origin.Owner)
        return true;
      SharedInteractionSystem.Ignored ignored = predicate;
      return ignored != null && ignored(e);
    });
    bool flag = true;
    MapCoordinates mapCoordinates1 = new MapCoordinates();
    MapCoordinates mapCoordinates2 = this._transform.ToMapCoordinates(otherCoordinates);
    Angle angle = Angle.op_Addition(this._transform.GetWorldRotation(otherCoordinates.EntityId), otherAngle);
    FixturesComponent component1;
    FixturesComponent component2;
    if ((double) range > 0.0 && this._fixtureQuery.TryComp((EntityUid) origin, out component1) && component1.FixtureCount > 0 && this._fixtureQuery.TryComp((EntityUid) other, out component2) && component2.FixtureCount > 0 && this.Resolve((EntityUid) origin, ref origin.Comp))
    {
      (Vector2 WorldPosition, Angle WorldRotation) positionRotation = this._transform.GetWorldPositionRotation(origin.Comp);
      Robust.Shared.Physics.Transform xfA = new Robust.Shared.Physics.Transform(positionRotation.WorldPosition, positionRotation.WorldRotation);
      Robust.Shared.Physics.Transform xfB = new Robust.Shared.Physics.Transform(mapCoordinates2.Position, angle);
      float distance;
      if (!this._broadphase.TryGetNearest((EntityUid) origin, (EntityUid) other, out Vector2 _, out Vector2 _, out distance, xfA, xfB, component1, component2))
      {
        flag = false;
      }
      else
      {
        if (overlapCheck && distance.Equals(0.0f) || !this.ShouldCheckAccess((EntityUid) origin))
          return true;
        if ((double) distance > (double) range)
        {
          mapCoordinates1 = this._transform.GetMapCoordinates((EntityUid) origin, origin.Comp);
        }
        else
        {
          mapCoordinates1 = this._transform.GetMapCoordinates((EntityUid) origin, origin.Comp);
          range = (mapCoordinates1.Position - mapCoordinates2.Position).Length();
        }
      }
    }
    else
      mapCoordinates1 = this._transform.GetMapCoordinates((EntityUid) origin, (TransformComponent) origin);
    if (flag)
    {
      SharedInteractionSystem.Ignored predicate2 = this.GetPredicate(mapCoordinates1, (EntityUid) other, mapCoordinates2, angle, collisionMask, predicate1);
      flag = this.InRangeUnobstructed(mapCoordinates1, mapCoordinates2, range, collisionMask, predicate2, this.ShouldCheckAccess((EntityUid) origin));
    }
    if (!flag & popup && this._gameTiming.IsFirstTimePredicted)
      this._popupSystem.PopupClient(this.Loc.GetString("interaction-system-user-interaction-cannot-reach"), (EntityUid) origin, new EntityUid?((EntityUid) origin));
    return flag;
  }

  public bool InRangeUnobstructed(
    MapCoordinates origin,
    EntityUid target,
    float range = 1.5f,
    CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable,
    SharedInteractionSystem.Ignored? predicate = null)
  {
    TransformComponent component = this.Transform(target);
    (Vector2 vector2, Angle angle) = this._transform.GetWorldPositionRotation(component);
    MapCoordinates mapCoordinates = new MapCoordinates(vector2, component.MapID);
    SharedInteractionSystem.Ignored predicate1 = this.GetPredicate(origin, target, mapCoordinates, angle, collisionMask, predicate);
    return this.InRangeUnobstructed(origin, mapCoordinates, range, collisionMask, predicate1);
  }

  private SharedInteractionSystem.Ignored GetPredicate(
    MapCoordinates originCoords,
    EntityUid target,
    MapCoordinates targetCoords,
    Angle targetRotation,
    CollisionGroup collisionMask,
    SharedInteractionSystem.Ignored? predicate = null)
  {
    HashSet<EntityUid> ignored = new HashSet<EntityUid>();
    PhysicsComponent component1;
    if (this._itemQuery.HasComp(target) && this._physicsQuery.TryComp(target, out component1) && component1.CanCollide)
    {
      foreach (EntityUid uid in this._lookup.GetEntitiesInRange(target, 0.01f, LookupFlags.Static))
      {
        PhysicsComponent component2;
        if (!(target == uid) && this._physicsQuery.TryComp(uid, out component2) && component2.CanCollide && (collisionMask & (CollisionGroup) component2.CollisionLayer) != CollisionGroup.None)
          ignored.Add(uid);
      }
    }
    else
    {
      WallMountComponent component3;
      if (this._wallMountQuery.TryComp(target, out component3))
      {
        bool flag;
        if (Angle.op_Implicit(component3.Arc) >= 2.0 * Math.PI)
        {
          flag = true;
        }
        else
        {
          Angle angle1 = Angle.FromWorldVec(originCoords.Position - targetCoords.Position);
          Angle angle2 = Angle.op_Subtraction(Angle.op_Addition(component3.Direction, targetRotation), angle1);
          Angle angle3 = ((Angle) ref angle2).Reduced();
          Angle angle4 = ((Angle) ref angle3).FlipPositive();
          flag = Angle.op_Implicit(angle4) < Angle.op_Implicit(component3.Arc) / 2.0 || Angle.op_Implicit(Angle.op_Subtraction(Angle.op_Implicit(2.0 * Math.PI), angle4)) < Angle.op_Implicit(component3.Arc) / 2.0;
        }
        EntityUid uid1;
        MapGridComponent grid;
        if (flag && this._mapManager.TryFindGridAt(targetCoords, out uid1, out grid))
        {
          ignored.UnionWith(this._map.GetAnchoredEntities((Entity<MapGridComponent>) (uid1, grid), targetCoords));
          foreach (EntityUid uid2 in this._lookup.GetEntitiesInRange(targetCoords, 0.2f))
          {
            TransformComponent comp;
            if (this.TryComp(uid2, out comp) && comp.Anchored)
              ignored.Add(uid2);
          }
        }
      }
    }
    return (SharedInteractionSystem.Ignored) (e =>
    {
      if (!(e == target))
      {
        SharedInteractionSystem.Ignored ignored1 = predicate;
        if ((ignored1 != null ? (ignored1(e) ? 1 : 0) : 0) == 0)
          return ignored.Contains(e);
      }
      return true;
    });
  }

  public bool InRangeUnobstructed(
    EntityUid origin,
    EntityCoordinates other,
    float range = 1.5f,
    CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable,
    SharedInteractionSystem.Ignored? predicate = null,
    bool popup = false)
  {
    return this.InRangeUnobstructed(origin, this._transform.ToMapCoordinates(other), range, collisionMask, predicate, popup);
  }

  public bool InRangeUnobstructed(
    EntityUid origin,
    MapCoordinates other,
    float range = 1.5f,
    CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable,
    SharedInteractionSystem.Ignored? predicate = null,
    bool popup = false)
  {
    SharedInteractionSystem.Ignored predicate1 = (SharedInteractionSystem.Ignored) (e =>
    {
      if (e == origin)
        return true;
      SharedInteractionSystem.Ignored ignored = predicate;
      return ignored != null && ignored(e);
    });
    int num = this.InRangeUnobstructed(this._transform.GetMapCoordinates(origin), other, range, collisionMask, predicate1, this.ShouldCheckAccess(origin)) ? 1 : 0;
    if (!(num == 0 & popup))
      return num != 0;
    if (!this._gameTiming.IsFirstTimePredicted)
      return num != 0;
    this._popupSystem.PopupEntity(this.Loc.GetString("interaction-system-user-interaction-cannot-reach"), origin, origin);
    return num != 0;
  }

  public bool RangedInteractDoBefore(
    EntityUid user,
    EntityUid used,
    EntityUid? target,
    EntityCoordinates clickLocation,
    bool canReach,
    bool checkDeletion = true)
  {
    if (checkDeletion && (this.IsDeleted(user) || this.IsDeleted(used) || this.IsDeleted(target)))
      return false;
    BeforeRangedInteractEvent args = new BeforeRangedInteractEvent(user, used, target, clickLocation, canReach);
    this.RaiseLocalEvent<BeforeRangedInteractEvent>(used, args);
    if (!args.Handled)
      return false;
    this.DoContactInteraction(user, new EntityUid?(used), (HandledEntityEventArgs) args);
    return args.Handled;
  }

  public bool InteractUsing(
    EntityUid user,
    EntityUid used,
    EntityUid target,
    EntityCoordinates clickLocation,
    bool checkCanInteract = true,
    bool checkCanUse = true)
  {
    if (this.IsDeleted(user) || this.IsDeleted(used) || this.IsDeleted(target) || checkCanInteract && !this._actionBlockerSystem.CanInteract(user, new EntityUid?(target)) || checkCanUse && !this._actionBlockerSystem.CanUseHeldEntity(user, used))
      return false;
    ISharedAdminLogManager adminLogger = this._adminLogger;
    LogStringHandler logStringHandler = new LogStringHandler(24, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
    logStringHandler.AppendLiteral(" interacted with ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) target), nameof (target), "ToPrettyString(target)");
    logStringHandler.AppendLiteral(" using ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) used), nameof (used), "ToPrettyString(used)");
    ref LogStringHandler local = ref logStringHandler;
    adminLogger.Add(LogType.InteractUsing, LogImpact.Low, ref local);
    if (this.RangedInteractDoBefore(user, used, new EntityUid?(target), clickLocation, true, false))
      return true;
    InteractUsingEvent args = new InteractUsingEvent(user, used, target, clickLocation);
    this.RaiseLocalEvent<InteractUsingEvent>(target, args, true);
    this.DoContactInteraction(user, new EntityUid?(used), (HandledEntityEventArgs) args);
    this.DoContactInteraction(user, new EntityUid?(target), (HandledEntityEventArgs) args);
    return args.Handled || this.InteractDoAfter(user, used, new EntityUid?(target), clickLocation, true, false);
  }

  public bool InteractDoAfter(
    EntityUid user,
    EntityUid used,
    EntityUid? target,
    EntityCoordinates clickLocation,
    bool canReach,
    bool checkDeletion = true)
  {
    if (target.HasValue && !target.GetValueOrDefault().Valid)
      target = new EntityUid?();
    if (checkDeletion && (this.IsDeleted(user) || this.IsDeleted(used) || this.IsDeleted(target)))
      return false;
    AfterInteractEvent args1 = new AfterInteractEvent(user, used, target, clickLocation, canReach);
    this.RaiseLocalEvent<AfterInteractEvent>(used, args1);
    this.DoContactInteraction(user, new EntityUid?(used), (HandledEntityEventArgs) args1);
    if (canReach)
      this.DoContactInteraction(user, target, (HandledEntityEventArgs) args1);
    if (args1.Handled)
      return true;
    if (!target.HasValue)
      return false;
    AfterInteractUsingEvent args2 = new AfterInteractUsingEvent(user, used, target, clickLocation, canReach);
    this.RaiseLocalEvent<AfterInteractUsingEvent>(target.Value, args2);
    this.DoContactInteraction(user, new EntityUid?(used), (HandledEntityEventArgs) args2);
    if (canReach)
      this.DoContactInteraction(user, target, (HandledEntityEventArgs) args2);
    return args2.Handled;
  }

  private bool HandleActivateItemInWorld(
    ICommonSession? session,
    EntityCoordinates coords,
    EntityUid uid)
  {
    this._rmcLagCompensation.SendLastRealTick();
    EntityUid? userEntity;
    if (!this.ValidateClientInput(session, coords, uid, out userEntity))
    {
      this.Log.Info("ActivateItemInWorld input validation failed");
      return false;
    }
    if (this.Deleted(uid))
      return false;
    this.InteractionActivate(userEntity.Value, uid, checkAccess: this.ShouldCheckAccess(userEntity.Value));
    return false;
  }

  public bool InteractionActivate(
    EntityUid user,
    EntityUid used,
    bool checkCanInteract = true,
    bool checkUseDelay = true,
    bool checkAccess = true,
    bool? complexInteractions = null,
    bool checkDeletion = true)
  {
    if (checkDeletion && (this.IsDeleted(user) || this.IsDeleted(used)))
      return false;
    UseDelayComponent component;
    this._delayQuery.TryComp(used, out component);
    if (checkUseDelay && component != null && this._useDelay.IsDelayed((Entity<UseDelayComponent>) (used, component)) || checkCanInteract && !this._actionBlockerSystem.CanInteract(user, new EntityUid?(used)) || checkAccess && !this.InRangeUnobstructed((Entity<TransformComponent>) user, (Entity<TransformComponent>) used) || checkAccess && !this.IsAccessible((Entity<TransformComponent>) user, (Entity<TransformComponent>) used))
      return false;
    complexInteractions.GetValueOrDefault();
    if (!complexInteractions.HasValue)
      complexInteractions = new bool?(this._actionBlockerSystem.CanComplexInteract(user));
    ActivateInWorldEvent args1 = new ActivateInWorldEvent(user, used, complexInteractions.Value);
    this.RaiseLocalEvent<ActivateInWorldEvent>(used, args1, true);
    if (args1.Handled)
    {
      this.DoContactInteraction(user, new EntityUid?(used));
      if (!args1.WasLogged)
      {
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(11, 2);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
        logStringHandler.AppendLiteral(" activated ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) used), nameof (used), "ToPrettyString(used)");
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.InteractActivate, LogImpact.Low, ref local);
      }
      if (component != null)
        this._useDelay.TryResetDelay(used, component: component);
      return true;
    }
    UserActivateInWorldEvent args2 = new UserActivateInWorldEvent(user, used, complexInteractions.Value);
    this.RaiseLocalEvent<UserActivateInWorldEvent>(user, args2, true);
    if (!args2.Handled)
      return false;
    this.DoContactInteraction(user, new EntityUid?(used));
    if (component != null)
      this._useDelay.TryResetDelay(used, component: component);
    ISharedAdminLogManager adminLogger1 = this._adminLogger;
    LogStringHandler logStringHandler1 = new LogStringHandler(11, 2);
    logStringHandler1.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), nameof (user), "ToPrettyString(user)");
    logStringHandler1.AppendLiteral(" activated ");
    logStringHandler1.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) used), nameof (used), "ToPrettyString(used)");
    ref LogStringHandler local1 = ref logStringHandler1;
    adminLogger1.Add(LogType.InteractActivate, LogImpact.Low, ref local1);
    return true;
  }

  public bool UseInHandInteraction(
    EntityUid user,
    EntityUid used,
    bool checkCanUse = true,
    bool checkCanInteract = true,
    bool checkUseDelay = true)
  {
    if (this.IsDeleted(user) || this.IsDeleted(used))
      return false;
    UseDelayComponent component;
    this._delayQuery.TryComp(used, out component);
    if (checkUseDelay && component != null && this._useDelay.IsDelayed((Entity<UseDelayComponent>) (used, component)))
      return true;
    if (checkCanInteract && !this._actionBlockerSystem.CanInteract(user, new EntityUid?(used)) || checkCanUse && !this._actionBlockerSystem.CanUseHeldEntity(user, used))
      return false;
    UseInHandEvent args = new UseInHandEvent(user);
    this.RaiseLocalEvent<UseInHandEvent>(used, args, true);
    if (!args.Handled)
      return this.InteractionActivate(user, used, false, false, false, checkDeletion: false);
    this.DoContactInteraction(user, new EntityUid?(used), (HandledEntityEventArgs) args);
    if (component != null && args.ApplyDelay)
      this._useDelay.TryResetDelay((Entity<UseDelayComponent>) (used, component));
    return true;
  }

  public bool AltInteract(EntityUid user, EntityUid target)
  {
    SortedSet<Verb> localVerbs = this._verbSystem.GetLocalVerbs(target, user, typeof (AlternativeVerb));
    if (localVerbs.Count == 0)
      return false;
    this._verbSystem.ExecuteVerb(localVerbs.First<Verb>(), user, target);
    return true;
  }

  public void DroppedInteraction(EntityUid user, EntityUid item)
  {
    if (this.IsDeleted(user) || this.IsDeleted(item))
      return;
    DroppedEvent args = new DroppedEvent(user);
    this.RaiseLocalEvent<DroppedEvent>(item, args, true);
    Angle angle = Angle.Zero;
    InputMoverComponent comp;
    if (this.TryComp<InputMoverComponent>(user, out comp))
      angle = comp.TargetRelativeRotation;
    this.Transform(item).LocalRotation = angle;
  }

  public bool InRangeAndAccessible(
    Entity<TransformComponent?> user,
    Entity<TransformComponent?> target,
    float range = 1.5f,
    CollisionGroup collisionMask = CollisionGroup.Impassable | CollisionGroup.InteractImpassable,
    SharedInteractionSystem.Ignored? predicate = null,
    bool lagCompensated = false)
  {
    if (user == target)
      return true;
    return this.Resolve((EntityUid) user, ref user.Comp) && this.Resolve((EntityUid) target, ref target.Comp) && this.IsAccessible(user, target) && this.InRangeUnobstructed(user, target, range, collisionMask, predicate);
  }

  public bool IsAccessible(Entity<TransformComponent?> user, Entity<TransformComponent?> target)
  {
    AccessibleOverrideEvent args = new AccessibleOverrideEvent((EntityUid) user, (EntityUid) target);
    this.RaiseLocalEvent<AccessibleOverrideEvent>((EntityUid) user, ref args);
    if (args.Handled)
      return args.Accessible;
    BaseContainer otherContainer;
    if (this._containerSystem.IsInSameOrParentContainer((Entity<TransformComponent, MetaDataComponent>) user, (Entity<TransformComponent, MetaDataComponent>) target, out BaseContainer _, out otherContainer))
      return true;
    return otherContainer != null && this.CanAccessViaStorage((EntityUid) user, (EntityUid) target, otherContainer);
  }

  public bool CanAccessViaStorage(EntityUid user, EntityUid target)
  {
    BaseContainer container;
    return this._containerSystem.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (target, (TransformComponent) null, (MetaDataComponent) null), out container) && this.CanAccessViaStorage(user, target, container);
  }

  public bool CanAccessViaStorage(EntityUid user, EntityUid target, BaseContainer container)
  {
    if (StorageComponent.ContainerId != container.ID)
      return false;
    return this._ui.IsUiOpen((Entity<UserInterfaceComponent>) container.Owner, (Enum) StorageComponent.StorageUiKey.Key, user) || this.HasComp<RMCItemKeepUIOpenOnStorageClosedComponent>(target);
  }

  public bool CanAccessEquipment(EntityUid user, EntityUid target)
  {
    BaseContainer container;
    if (this.Deleted(target) || !this._containerSystem.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) target, out container))
      return false;
    EntityUid owner = container.Owner;
    SlotDefinition slotDefinition;
    if (!this._inventory.TryGetSlot(owner, container.ID, out slotDefinition))
      return false;
    if (owner == user)
      return true;
    return !this._strippable.IsStripHidden(slotDefinition, new EntityUid?(user)) && this.InRangeUnobstructed((Entity<TransformComponent>) user, (Entity<TransformComponent>) owner) && this._containerSystem.IsInSameOrParentContainer((Entity<TransformComponent, MetaDataComponent>) user, (Entity<TransformComponent, MetaDataComponent>) owner);
  }

  protected bool ValidateClientInput(
    ICommonSession? session,
    EntityCoordinates coords,
    EntityUid uid,
    [NotNullWhen(true)] out EntityUid? userEntity)
  {
    userEntity = new EntityUid?();
    if (!coords.IsValid((IEntityManager) this.EntityManager))
    {
      this.Log.Info($"Invalid Coordinates: client={session}, coords={coords}");
      return false;
    }
    if (this.IsClientSide(uid))
    {
      this.Log.Warning($"Client sent interaction with client-side entity. Session={session}, Uid={uid}");
      return false;
    }
    userEntity = (EntityUid?) session?.AttachedEntity;
    if (!userEntity.HasValue || !userEntity.Value.Valid)
    {
      this.Log.Warning($"Client sent interaction with no attached entity. Session={session}");
      return false;
    }
    if (this.Exists(userEntity))
      return this._rateLimit.CountAction(session, "Interaction") == RateLimitStatus.Allowed;
    this.Log.Warning($"Client attempted interaction with a non-existent attached entity. Session={session},  entity={userEntity}");
    return false;
  }

  public void DoContactInteraction(EntityUid uidA, EntityUid? uidB, HandledEntityEventArgs? args = null)
  {
    MetaDataComponent comp1;
    MetaDataComponent comp2;
    if (!uidB.HasValue || args != null && !args.Handled || uidA == uidB.Value || !this.TryComp(uidA, out comp1) || comp1.EntityPaused || !this.TryComp(uidB, out comp2) || comp2.EntityPaused)
      return;
    ContactInteractionEvent args1 = new ContactInteractionEvent(uidB.Value);
    this.RaiseLocalEvent<ContactInteractionEvent>(uidA, args1);
    args1.Other = uidA;
    this.RaiseLocalEvent<ContactInteractionEvent>(uidB.Value, args1);
  }

  private void HandleUserInterfaceRangeCheck(ref BoundUserInterfaceCheckRangeEvent ev)
  {
    if (ev.Result == BoundUserInterfaceRangeResult.Fail)
      return;
    ev.Result = this.UiRangeCheck(ev.Actor, (Entity<TransformComponent>) ev.Target, ev.Data.InteractionRange) ? BoundUserInterfaceRangeResult.Pass : BoundUserInterfaceRangeResult.Fail;
  }

  public bool TryGetUsedEntity(EntityUid user, [NotNullWhen(true)] out EntityUid? used, bool checkCanUse = true)
  {
    GetUsedEntityEvent args = new GetUsedEntityEvent(user);
    this.RaiseLocalEvent<GetUsedEntityEvent>(user, ref args);
    used = args.Used;
    if (!args.Handled)
      return false;
    if (!checkCanUse || this._actionBlockerSystem.CanUseHeldEntity(user, args.Used.Value))
      return args.Handled;
    used = new EntityUid?();
    return false;
  }

  [Obsolete("Use ActionBlockerSystem")]
  public bool SupportsComplexInteractions(EntityUid user)
  {
    return this._actionBlockerSystem.CanComplexInteract(user);
  }

  public void SetRelay(EntityUid uid, EntityUid? relayEntity, InteractionRelayComponent? component = null)
  {
    if (!this.Resolve<InteractionRelayComponent>(uid, ref component))
      return;
    component.RelayEntity = relayEntity;
    this.Dirty(uid, (IComponent) component);
  }

  public delegate bool Ignored(EntityUid entity);
}
