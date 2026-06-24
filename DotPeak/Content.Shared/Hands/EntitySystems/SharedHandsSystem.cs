// Decompiled with JetBrains decompiler
// Type: Content.Shared.Hands.EntitySystems.SharedHandsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Inventory;
using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Atmos;
using Content.Shared.Camera;
using Content.Shared.Cuffs;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item;
using Content.Shared.Localizations;
using Content.Shared.Movement.Systems;
using Content.Shared.Projectiles;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Tag;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Wieldable;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Hands.EntitySystems;

public abstract class SharedHandsSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  protected SharedContainerSystem ContainerSystem;
  [Dependency]
  private SharedInteractionSystem _interactionSystem;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedStorageSystem _storage;
  [Dependency]
  protected SharedTransformSystem TransformSystem;
  [Dependency]
  private SharedVirtualItemSystem _virtualSystem;
  [Dependency]
  private TagSystem _tagSystem;
  private static readonly ProtoId<TagPrototype> BypassDropChecksTag = (ProtoId<TagPrototype>) "BypassDropChecks";
  [Dependency]
  private RMCHandsSystem _rmcHands;
  public const float MaxAnimationRange = 10f;

  public bool TrySelect(EntityUid uid, EntityUid? entity, HandsComponent? handsComp = null)
  {
    string inHand;
    if (!this.Resolve<HandsComponent>(uid, ref handsComp, false) || !this.IsHolding((Entity<HandsComponent>) (uid, handsComp), entity, out inHand))
      return false;
    this.SetActiveHand((Entity<HandsComponent>) (uid, handsComp), inHand);
    return true;
  }

  public bool TrySelect<TComponent>(
    EntityUid uid,
    [NotNullWhen(true)] out TComponent? component,
    HandsComponent? handsComp = null)
    where TComponent : Component
  {
    component = default (TComponent);
    if (!this.Resolve<HandsComponent>(uid, ref handsComp, false))
      return false;
    foreach (string key in handsComp.Hands.Keys)
    {
      EntityUid? held;
      if (this.TryGetHeldItem((Entity<HandsComponent>) (uid, handsComp), key, out held) && this.TryComp<TComponent>(held, out component))
        return true;
    }
    return false;
  }

  public bool TrySelectEmptyHand(EntityUid uid, HandsComponent? handsComp = null)
  {
    return this.TrySelect(uid, new EntityUid?(), handsComp);
  }

  public event Action<Entity<HandsComponent>, string, HandLocation>? OnPlayerAddHand;

  public event Action<Entity<HandsComponent>, string>? OnPlayerRemoveHand;

  protected event Action<Entity<HandsComponent>?>? OnHandSetActive;

  public override void Initialize()
  {
    base.Initialize();
    this.InitializeInteractions();
    this.InitializeDrop();
    this.InitializePickup();
    this.InitializeRelay();
    this.SubscribeLocalEvent<HandsComponent, ComponentInit>(new EntityEventRefHandler<HandsComponent, ComponentInit>(this.OnInit));
    this.SubscribeLocalEvent<HandsComponent, MapInitEvent>(new EntityEventRefHandler<HandsComponent, MapInitEvent>(this.OnMapInit));
  }

  public override void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<SharedHandsSystem>();
  }

  private void OnInit(Entity<HandsComponent> ent, ref ComponentInit args)
  {
    ContainerManagerComponent containerManager = this.EnsureComp<ContainerManagerComponent>((EntityUid) ent);
    foreach (string key in ent.Comp.Hands.Keys)
      this.ContainerSystem.EnsureContainer<ContainerSlot>((EntityUid) ent, key, containerManager);
  }

  private void OnMapInit(Entity<HandsComponent> ent, ref MapInitEvent args)
  {
    if (ent.Comp.ActiveHandId != null)
      return;
    this.SetActiveHand(ent.AsNullable(), ent.Comp.SortedHands.FirstOrDefault<string>());
  }

  public void AddHand(Entity<HandsComponent?> ent, string handName, HandLocation handLocation)
  {
    this.AddHand(ent, handName, new Hand(handLocation));
  }

  public void AddHand(Entity<HandsComponent?> ent, string handName, Hand hand)
  {
    if (!this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false) || ent.Comp.Hands.ContainsKey(handName))
      return;
    this.ContainerSystem.EnsureContainer<ContainerSlot>((EntityUid) ent, handName).OccludesLight = false;
    ent.Comp.Hands.Add(handName, hand);
    ent.Comp.SortedHands.Add(handName);
    this.Dirty<HandsComponent>(ent);
    Action<Entity<HandsComponent>, string, HandLocation> onPlayerAddHand = this.OnPlayerAddHand;
    if (onPlayerAddHand != null)
      onPlayerAddHand((Entity<HandsComponent>) ((EntityUid) ent, ent.Comp), handName, hand.Location);
    if (ent.Comp.ActiveHandId == null)
      this.SetActiveHand(ent, handName);
    this.RaiseLocalEvent<HandCountChangedEvent>((EntityUid) ent, new HandCountChangedEvent((EntityUid) ent));
  }

  public virtual void RemoveHand(Entity<HandsComponent?> ent, string handName)
  {
    if (!this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false))
      return;
    Action<Entity<HandsComponent>, string> playerRemoveHand = this.OnPlayerRemoveHand;
    if (playerRemoveHand != null)
      playerRemoveHand((Entity<HandsComponent>) ((EntityUid) ent, ent.Comp), handName);
    this.TryDrop(ent, handName, checkActionBlocker: false);
    if (!ent.Comp.Hands.Remove(handName))
      return;
    BaseContainer container;
    if (this.ContainerSystem.TryGetContainer((EntityUid) ent, handName, out container))
      this.ContainerSystem.ShutdownContainer(container);
    ent.Comp.SortedHands.Remove(handName);
    if (ent.Comp.ActiveHandId == handName)
      this.TrySetActiveHand(ent, ent.Comp.SortedHands.FirstOrDefault<string>());
    this.RaiseLocalEvent<HandCountChangedEvent>((EntityUid) ent, new HandCountChangedEvent((EntityUid) ent));
    this.Dirty<HandsComponent>(ent);
  }

  public void RemoveHands(Entity<HandsComponent?> ent)
  {
    if (!this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false))
      return;
    foreach (string handName in new List<string>((IEnumerable<string>) ent.Comp.Hands.Keys))
      this.RemoveHand(ent, handName);
  }

  private void HandleSetHand(RequestSetHandEvent msg, EntitySessionEventArgs eventArgs)
  {
    if (!eventArgs.SenderSession.AttachedEntity.HasValue)
      return;
    this.TrySetActiveHand((Entity<HandsComponent>) eventArgs.SenderSession.AttachedEntity.Value, msg.HandName);
  }

  public bool TryGetEmptyHand(Entity<HandsComponent?> ent, [NotNullWhen(true)] out string? emptyHand)
  {
    emptyHand = (string) null;
    if (!this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false))
      return false;
    foreach (string enumerateHand in this.EnumerateHands(ent))
    {
      if (this.HandIsEmpty(ent, enumerateHand))
      {
        emptyHand = enumerateHand;
        return true;
      }
    }
    return false;
  }

  public bool TryGetActiveItem(Entity<HandsComponent?> entity, [NotNullWhen(true)] out EntityUid? item)
  {
    item = new EntityUid?();
    EntityUid? held;
    if (!this.Resolve<HandsComponent>((EntityUid) entity, ref entity.Comp, false) || !this.TryGetHeldItem(entity, entity.Comp.ActiveHandId, out held))
      return false;
    item = held;
    return true;
  }

  public EntityUid GetActiveItemOrSelf(Entity<HandsComponent?> entity)
  {
    EntityUid? nullable;
    return !this.TryGetActiveItem(entity, out nullable) ? entity.Owner : nullable.Value;
  }

  public string? GetActiveHand(Entity<HandsComponent?> entity)
  {
    return !this.Resolve<HandsComponent>((EntityUid) entity, ref entity.Comp, false) ? (string) null : entity.Comp.ActiveHandId;
  }

  public EntityUid? GetActiveItem(Entity<HandsComponent?> entity)
  {
    return !this.Resolve<HandsComponent>((EntityUid) entity, ref entity.Comp, false) ? new EntityUid?() : this.GetHeldItem(entity, entity.Comp.ActiveHandId);
  }

  public bool ActiveHandIsEmpty(Entity<HandsComponent?> entity)
  {
    return !this.GetActiveItem(entity).HasValue;
  }

  public IEnumerable<string> EnumerateHands(Entity<HandsComponent?> ent)
  {
    if (this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false))
    {
      if (ent.Comp.ActiveHandId != null)
        yield return ent.Comp.ActiveHandId;
      foreach (string sortedHand in ent.Comp.SortedHands)
      {
        if (sortedHand != ent.Comp.ActiveHandId)
          yield return sortedHand;
      }
    }
  }

  public IEnumerable<EntityUid> EnumerateHeld(Entity<HandsComponent?> ent)
  {
    SharedHandsSystem sharedHandsSystem = this;
    if (sharedHandsSystem.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false))
    {
      EntityUid? nullable;
      if (sharedHandsSystem.TryGetActiveItem(ent, out nullable))
        yield return nullable.Value;
      foreach (string sortedHand in ent.Comp.SortedHands)
      {
        EntityUid? held;
        if (!(sortedHand == ent.Comp.ActiveHandId) && sharedHandsSystem.TryGetHeldItem(ent, sortedHand, out held))
          yield return held.Value;
      }
    }
  }

  public bool TrySetActiveHand(Entity<HandsComponent?> ent, string? name)
  {
    return this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false) && !(name == ent.Comp.ActiveHandId) && (name == null || ent.Comp.Hands.ContainsKey(name)) && this.SetActiveHand(ent, name);
  }

  public bool SetActiveHand(Entity<HandsComponent?> ent, string? handId)
  {
    if (!this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp) || handId == ent.Comp.ActiveHandId)
      return false;
    EntityUid? nullable;
    if (this.TryGetActiveItem(ent, out nullable))
      this.RaiseLocalEvent<HandDeselectedEvent>(nullable.Value, new HandDeselectedEvent((EntityUid) ent));
    if (handId == null)
    {
      ent.Comp.ActiveHandId = (string) null;
      return true;
    }
    ent.Comp.ActiveHandId = handId;
    Action<Entity<HandsComponent>?> onHandSetActive = this.OnHandSetActive;
    if (onHandSetActive != null)
      onHandSetActive(new Entity<HandsComponent>?((Entity<HandsComponent>) ((EntityUid) ent, ent.Comp)));
    EntityUid? held;
    if (this.TryGetHeldItem(ent, handId, out held))
      this.RaiseLocalEvent<HandSelectedEvent>(held.Value, new HandSelectedEvent((EntityUid) ent));
    this.Dirty<HandsComponent>(ent);
    return true;
  }

  public bool IsHolding(Entity<HandsComponent?> entity, [NotNullWhen(true)] EntityUid? item)
  {
    return this.IsHolding(entity, item, out string _);
  }

  public bool IsHolding(Entity<HandsComponent?> ent, [NotNullWhen(true)] EntityUid? entity, [NotNullWhen(true)] out string? inHand)
  {
    inHand = (string) null;
    if (!entity.HasValue || !this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false))
      return false;
    foreach (string key in ent.Comp.Hands.Keys)
    {
      EntityUid? heldItem = this.GetHeldItem(ent, key);
      EntityUid? nullable = entity;
      if ((heldItem.HasValue == nullable.HasValue ? (heldItem.HasValue ? (heldItem.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      {
        inHand = key;
        return true;
      }
    }
    return false;
  }

  public bool TryGetHand(Entity<HandsComponent?> ent, [NotNullWhen(true)] string? handId, [NotNullWhen(true)] out Hand? hand)
  {
    hand = new Hand?();
    Hand hand1;
    if (handId == null || !this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false) || !ent.Comp.Hands.TryGetValue(handId, out hand1))
      return false;
    hand = new Hand?(hand1);
    return true;
  }

  public EntityUid? GetHeldItem(Entity<HandsComponent?> ent, string? handId)
  {
    EntityUid? held;
    this.TryGetHeldItem(ent, handId, out held);
    return held;
  }

  public bool TryGetHeldItem(Entity<HandsComponent?> ent, string? handId, [NotNullWhen(true)] out EntityUid? held)
  {
    held = new EntityUid?();
    BaseContainer container;
    if (!this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false) || handId == null || !ent.Comp.Hands.ContainsKey(handId) || !this.ContainerSystem.TryGetContainer((EntityUid) ent, handId, out container))
      return false;
    held = container.ContainedEntities.FirstOrNull<EntityUid>();
    return held.HasValue;
  }

  public bool HandIsEmpty(Entity<HandsComponent?> ent, string handId)
  {
    return !this.GetHeldItem(ent, handId).HasValue;
  }

  public int GetHandCount(Entity<HandsComponent?> ent)
  {
    return !this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false) ? 0 : ent.Comp.Hands.Count;
  }

  public int CountFreeHands(Entity<HandsComponent?> ent)
  {
    if (!this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false))
      return 0;
    int num = 0;
    foreach (string key in ent.Comp.Hands.Keys)
    {
      if (this.HandIsEmpty(ent, key))
        ++num;
    }
    return num;
  }

  public int CountFreeableHands(Entity<HandsComponent> hands, EntityUid except)
  {
    int num = 0;
    foreach (string key in hands.Comp.Hands.Keys)
    {
      EntityUid? held;
      if (this.TryGetHeldItem(hands.AsNullable(), key, out held))
      {
        EntityUid? nullable = held;
        EntityUid entityUid = except;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
          continue;
      }
      if (this.HandIsEmpty(hands.AsNullable(), key) || this.CanDropHeld((EntityUid) hands, key))
        ++num;
    }
    return num;
  }

  private void InitializeDrop()
  {
    this.SubscribeLocalEvent<HandsComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<HandsComponent, EntRemovedFromContainerMessage>(this.HandleEntityRemoved));
  }

  protected virtual void HandleEntityRemoved(
    EntityUid uid,
    HandsComponent hands,
    EntRemovedFromContainerMessage args)
  {
    Hand? hand;
    if (!this.TryGetHand((Entity<HandsComponent>) uid, args.Container.ID, out hand))
      return;
    GotUnequippedHandEvent args1 = new GotUnequippedHandEvent(uid, args.Entity, hand.Value);
    this.RaiseLocalEvent<GotUnequippedHandEvent>(args.Entity, args1);
    DidUnequipHandEvent args2 = new DidUnequipHandEvent(uid, args.Entity, hand.Value);
    this.RaiseLocalEvent<DidUnequipHandEvent>(uid, args2);
    VirtualItemComponent comp;
    if (!this.TryComp<VirtualItemComponent>(args.Entity, out comp))
      return;
    this._virtualSystem.DeleteVirtualItem((Entity<VirtualItemComponent>) (args.Entity, comp), uid);
  }

  private bool ShouldIgnoreRestrictions(EntityUid user)
  {
    return !this._tagSystem.HasTag(user, SharedHandsSystem.BypassDropChecksTag);
  }

  public bool CanDrop(Entity<HandsComponent?> ent, EntityUid entity, bool checkActionBlocker = true)
  {
    string inHand;
    return this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false) && this.IsHolding(ent, new EntityUid?(entity), out inHand) && this.CanDropHeld((EntityUid) ent, inHand, checkActionBlocker);
  }

  public bool CanDropHeld(EntityUid uid, string handId, bool checkActionBlocker = true)
  {
    BaseContainer container;
    if (!this.ContainerSystem.TryGetContainer(uid, handId, out container))
      return false;
    EntityUid? nullable = container.ContainedEntities.FirstOrNull<EntityUid>();
    if (!nullable.HasValue)
      return false;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    return this.ContainerSystem.CanRemove(valueOrDefault, container) && (!checkActionBlocker || this._actionBlocker.CanDrop(uid, new EntityUid?(valueOrDefault)));
  }

  public bool TryDrop(
    Entity<HandsComponent?> ent,
    EntityCoordinates? targetDropLocation = null,
    bool checkActionBlocker = true,
    bool doDropInteraction = true)
  {
    return this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false) && ent.Comp.ActiveHandId != null && this.TryDrop(ent, ent.Comp.ActiveHandId, targetDropLocation, checkActionBlocker, doDropInteraction);
  }

  public bool TryDrop(
    Entity<HandsComponent?> ent,
    EntityUid entity,
    EntityCoordinates? targetDropLocation = null,
    bool checkActionBlocker = true,
    bool doDropInteraction = true)
  {
    string inHand;
    return this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false) && this.IsHolding(ent, new EntityUid?(entity), out inHand) && this.TryDrop(ent, inHand, targetDropLocation, checkActionBlocker, doDropInteraction);
  }

  public bool TryDrop(
    Entity<HandsComponent?> ent,
    string handId,
    EntityCoordinates? targetDropLocation = null,
    bool checkActionBlocker = true,
    bool doDropInteraction = true)
  {
    EntityUid? held;
    if (!this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false) || !this.CanDropHeld((EntityUid) ent, handId, checkActionBlocker) || !this.TryGetHeldItem(ent, handId, out held))
      return false;
    VirtualItemComponent comp;
    if (this.TryComp<VirtualItemComponent>(held, out comp))
      this._virtualSystem.DeleteVirtualItem((Entity<VirtualItemComponent>) (held.Value, comp), (EntityUid) ent);
    if (this.TerminatingOrDeleted(held))
      return true;
    TransformComponent transformComponent = this.Transform(held.Value);
    if (!transformComponent.MapUid.HasValue)
      return true;
    TransformComponent xform = this.Transform((EntityUid) ent);
    if (this.ContainerSystem.IsEntityOrParentInContainer((EntityUid) ent, xform: xform))
    {
      this.TransformSystem.DropNextTo((Entity<TransformComponent>) (held.Value, transformComponent), (Entity<TransformComponent>) ((EntityUid) ent, xform));
      RMCDroppedEvent args = new RMCDroppedEvent((EntityUid) ent);
      this.RaiseLocalEvent<RMCDroppedEvent>(held.Value, ref args, true);
      return true;
    }
    this.DoDrop(ent, handId, doDropInteraction);
    if (!targetDropLocation.HasValue)
      return true;
    (Vector2 vector2, Angle angle) = this.TransformSystem.GetWorldPositionRotation(held.Value);
    MapCoordinates origin = new MapCoordinates(vector2, transformComponent.MapID);
    MapCoordinates mapCoordinates = this.TransformSystem.ToMapCoordinates(targetDropLocation.Value);
    this.TransformSystem.SetWorldPositionRotation(held.Value, this.GetFinalDropCoordinates((EntityUid) ent, origin, mapCoordinates, held.Value), angle);
    return true;
  }

  public bool TryDropIntoContainer(
    Entity<HandsComponent?> ent,
    EntityUid entity,
    BaseContainer targetContainer,
    bool checkActionBlocker = true)
  {
    string inHand;
    if (!this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false) || !this.IsHolding(ent, new EntityUid?(entity), out inHand) || !this.CanDropHeld((EntityUid) ent, inHand, checkActionBlocker) || !this.ContainerSystem.CanInsert(entity, targetContainer))
      return false;
    this.DoDrop(ent, inHand, false);
    this.ContainerSystem.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) entity, targetContainer);
    return true;
  }

  private Vector2 GetFinalDropCoordinates(
    EntityUid user,
    MapCoordinates origin,
    MapCoordinates target,
    EntityUid held)
  {
    Vector2 vector2 = target.Position - origin.Position;
    float num1 = vector2.Length();
    float num2 = vector2.Length();
    if (this.ShouldIgnoreRestrictions(user))
    {
      if ((double) vector2.Length() > 1.5)
      {
        vector2 = Vector2Helpers.Normalized(vector2) * 1.5f;
        target = new MapCoordinates(origin.Position + vector2, target.MapId);
      }
      num2 = this._interactionSystem.UnobstructedDistance(origin, target, predicate: (SharedInteractionSystem.Ignored) (e => e == user || e == held));
    }
    return (double) num2 < (double) num1 ? origin.Position + Vector2Helpers.Normalized(vector2) * num2 : target.Position;
  }

  public virtual void DoDrop(
    Entity<HandsComponent?> ent,
    string handId,
    bool doDropInteraction = true,
    bool log = true)
  {
    BaseContainer container;
    EntityUid? held;
    if (!this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false) || !this.ContainerSystem.TryGetContainer((EntityUid) ent, handId, out container) || !this.TryGetHeldItem(ent, handId, out held) || this.TerminatingOrDeleted((EntityUid) ent) || this.TerminatingOrDeleted(held))
      return;
    if (!this.ContainerSystem.Remove((Entity<TransformComponent, MetaDataComponent>) held.Value, container))
    {
      this.Log.Error($"Failed to remove {this.ToPrettyString(held)} from users hand container when dropping. User: {this.ToPrettyString(new EntityUid?((EntityUid) ent))}. Hand: {handId}.");
    }
    else
    {
      this.Dirty<HandsComponent>(ent);
      if (doDropInteraction)
        this._interactionSystem.DroppedInteraction((EntityUid) ent, held.Value);
      if (log)
      {
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(9, 2);
        logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) ent)), "user", "ToPrettyString(ent)");
        logStringHandler.AppendLiteral(" dropped ");
        logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(held), "entity", "ToPrettyString(entity)");
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.Drop, LogImpact.Low, ref local);
      }
      if (!(handId == ent.Comp.ActiveHandId))
        return;
      this.RaiseLocalEvent<HandDeselectedEvent>(held.Value, new HandDeselectedEvent((EntityUid) ent));
    }
  }

  private void InitializeInteractions()
  {
    this.SubscribeAllEvent<RequestSetHandEvent>(new EntitySessionEventHandler<RequestSetHandEvent>(this.HandleSetHand));
    this.SubscribeAllEvent<RequestActivateInHandEvent>(new EntitySessionEventHandler<RequestActivateInHandEvent>(this.HandleActivateItemInHand));
    this.SubscribeAllEvent<RequestHandInteractUsingEvent>(new EntitySessionEventHandler<RequestHandInteractUsingEvent>(this.HandleInteractUsingInHand));
    this.SubscribeAllEvent<RequestUseInHandEvent>(new EntitySessionEventHandler<RequestUseInHandEvent>(this.HandleUseInHand));
    this.SubscribeAllEvent<RequestMoveHandItemEvent>(new EntitySessionEventHandler<RequestMoveHandItemEvent>(this.HandleMoveItemFromHand));
    this.SubscribeAllEvent<RequestHandAltInteractEvent>(new EntitySessionEventHandler<RequestHandAltInteractEvent>(this.HandleHandAltInteract));
    this.SubscribeLocalEvent<HandsComponent, GetUsedEntityEvent>(new ComponentEventRefHandler<HandsComponent, GetUsedEntityEvent>(this.OnGetUsedEntity));
    this.SubscribeLocalEvent<HandsComponent, ExaminedEvent>(new ComponentEventHandler<HandsComponent, ExaminedEvent>(this.HandleExamined));
    CommandBinds.Builder.Bind(ContentKeyFunctions.UseItemInHand, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.HandleUseItem), handle: false, outsidePrediction: false)).Bind(ContentKeyFunctions.AltUseItemInHand, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.HandleAltUseInHand), handle: false, outsidePrediction: false)).Bind(ContentKeyFunctions.SwapHands, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.SwapHandsPressed), handle: false, outsidePrediction: false)).Bind(ContentKeyFunctions.SwapHandsReverse, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.SwapHandsReversePressed), handle: false, outsidePrediction: false)).Bind(ContentKeyFunctions.Drop, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate(this.DropPressed))).Register<SharedHandsSystem>();
  }

  private void HandleAltUseInHand(ICommonSession? session)
  {
    if (session == null || !session.AttachedEntity.HasValue)
      return;
    this.TryUseItemInHand(session.AttachedEntity.Value, true);
  }

  private void HandleUseItem(ICommonSession? session)
  {
    if (session == null || !session.AttachedEntity.HasValue)
      return;
    this.TryUseItemInHand(session.AttachedEntity.Value);
  }

  private void HandleMoveItemFromHand(RequestMoveHandItemEvent msg, EntitySessionEventArgs args)
  {
    if (!args.SenderSession.AttachedEntity.HasValue)
      return;
    RMCHandsSystem rmcHands = this._rmcHands;
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    EntityUid user = attachedEntity.Value;
    string handName = msg.HandName;
    if (rmcHands.TryStorageEjectHand(user, handName))
      return;
    attachedEntity = args.SenderSession.AttachedEntity;
    this.TryMoveHeldEntityToActiveHand(attachedEntity.Value, msg.HandName);
  }

  private void HandleUseInHand(RequestUseInHandEvent msg, EntitySessionEventArgs args)
  {
    if (!args.SenderSession.AttachedEntity.HasValue)
      return;
    this.TryUseItemInHand(args.SenderSession.AttachedEntity.Value);
  }

  private void HandleActivateItemInHand(RequestActivateInHandEvent msg, EntitySessionEventArgs args)
  {
    if (!args.SenderSession.AttachedEntity.HasValue)
      return;
    this.TryActivateItemInHand(args.SenderSession.AttachedEntity.Value, handName: msg.HandName);
  }

  private void HandleInteractUsingInHand(
    RequestHandInteractUsingEvent msg,
    EntitySessionEventArgs args)
  {
    if (!args.SenderSession.AttachedEntity.HasValue)
      return;
    this.TryInteractHandWithActiveHand(args.SenderSession.AttachedEntity.Value, msg.HandName);
  }

  private void HandleHandAltInteract(RequestHandAltInteractEvent msg, EntitySessionEventArgs args)
  {
    if (!args.SenderSession.AttachedEntity.HasValue)
      return;
    this.TryUseItemInHand(args.SenderSession.AttachedEntity.Value, true, handName: msg.HandName);
  }

  private void SwapHandsPressed(ICommonSession? session) => this.SwapHands(session, false);

  private void SwapHandsReversePressed(ICommonSession? session) => this.SwapHands(session, true);

  private void SwapHands(ICommonSession? session, bool reverse)
  {
    HandsComponent comp;
    if (!this.TryComp<HandsComponent>((EntityUid?) session?.AttachedEntity, out comp) || comp.ActiveHandId == null || comp.Hands.Count < 2)
      return;
    int index = (comp.SortedHands.IndexOf(comp.ActiveHandId) + (reverse ? -1 : 1) + comp.Hands.Count) % comp.Hands.Count;
    string sortedHand = comp.SortedHands[index];
    this.TrySetActiveHand((Entity<HandsComponent>) (session.AttachedEntity.Value, comp), sortedHand);
  }

  private bool DropPressed(ICommonSession? session, EntityCoordinates coords, EntityUid netEntity)
  {
    HandsComponent comp;
    if (this.TryComp<HandsComponent>((EntityUid?) session?.AttachedEntity, out comp) && comp.ActiveHandId != null)
      this.TryDrop((Entity<HandsComponent>) (session.AttachedEntity.Value, comp), comp.ActiveHandId, new EntityCoordinates?(coords));
    return false;
  }

  public bool TryActivateItemInHand(EntityUid uid, HandsComponent? handsComp = null, string? handName = null)
  {
    if (!this.Resolve<HandsComponent>(uid, ref handsComp, false))
      return false;
    string handId = handName;
    if (!this.TryGetHand((Entity<HandsComponent>) uid, handId, out Hand? _))
      handId = handsComp.ActiveHandId;
    EntityUid? held;
    return this.TryGetHeldItem((Entity<HandsComponent>) (uid, handsComp), handId, out held) && this._interactionSystem.InteractionActivate(uid, held.Value);
  }

  public bool TryInteractHandWithActiveHand(
    EntityUid uid,
    string handName,
    HandsComponent? handsComp = null)
  {
    EntityUid? nullable;
    EntityUid? held;
    if (!this.Resolve<HandsComponent>(uid, ref handsComp, false) || !this.TryGetActiveItem((Entity<HandsComponent>) (uid, handsComp), out nullable) || !this.TryGetHeldItem((Entity<HandsComponent>) (uid, handsComp), handName, out held))
      return false;
    this._interactionSystem.InteractUsing(uid, nullable.Value, held.Value, this.Transform(held.Value).Coordinates);
    return true;
  }

  public bool TryUseItemInHand(
    EntityUid uid,
    bool altInteract = false,
    HandsComponent? handsComp = null,
    string? handName = null)
  {
    if (!this.Resolve<HandsComponent>(uid, ref handsComp, false))
      return false;
    string handId = handName;
    if (!this.TryGetHand((Entity<HandsComponent>) uid, handId, out Hand? _))
      handId = handsComp.ActiveHandId;
    EntityUid? held;
    if (!this.TryGetHeldItem((Entity<HandsComponent>) (uid, handsComp), handId, out held))
      return false;
    return altInteract ? this._interactionSystem.AltInteract(uid, held.Value) : this._interactionSystem.UseInHandInteraction(uid, held.Value);
  }

  public bool TryMoveHeldEntityToActiveHand(
    EntityUid uid,
    string handName,
    bool checkActionBlocker = true,
    HandsComponent? handsComp = null)
  {
    EntityUid? held;
    if (!this.Resolve<HandsComponent>(uid, ref handsComp) || handsComp.ActiveHandId == null || !this.HandIsEmpty((Entity<HandsComponent>) (uid, handsComp), handsComp.ActiveHandId) || !this.TryGetHeldItem((Entity<HandsComponent>) (uid, handsComp), handName, out held) || !this.CanDropHeld(uid, handName, checkActionBlocker) || !this.CanPickupToHand(uid, held.Value, handsComp.ActiveHandId, checkActionBlocker, handsComp))
      return false;
    this.DoDrop((Entity<HandsComponent>) uid, handName, false, false);
    this.DoPickup(uid, handsComp.ActiveHandId, held.Value, handsComp, false);
    return true;
  }

  private void OnGetUsedEntity(
    EntityUid uid,
    HandsComponent component,
    ref GetUsedEntityEvent args)
  {
    if (args.Handled)
      return;
    EntityUid? nullable;
    if (this.TryGetActiveItem((Entity<HandsComponent>) (uid, component), out nullable))
      this.RaiseLocalEvent<GetUsedEntityEvent>(nullable.Value, ref args);
    ref EntityUid? local = ref args.Used;
    if (local.HasValue)
      return;
    local = nullable;
  }

  private void HandleExamined(EntityUid examinedUid, HandsComponent handsComp, ExaminedEvent args)
  {
    List<string> list = this.EnumerateHeld((Entity<HandsComponent>) (examinedUid, handsComp)).Where<EntityUid>((Func<EntityUid, bool>) (entity => !this.HasComp<VirtualItemComponent>(entity))).Select<EntityUid, string>((Func<EntityUid, string>) (item => FormattedMessage.EscapeText((string) Identity.Name(item, (IEntityManager) this.EntityManager)))).Select<string, string>((Func<string, string>) (itemName => this.Loc.GetString("comp-hands-examine-wrapper", ("item", (object) itemName)))).ToList<string>();
    if (list.Count == 0 && !handsComp.ExamineShowEmpty)
      return;
    string str = list.Count != 0 ? "comp-hands-examine" : "comp-hands-examine-empty";
    (string, EntityUid) valueTuple1 = ("user", Identity.Entity(examinedUid, (IEntityManager) this.EntityManager));
    (string, string) valueTuple2 = ("items", ContentLocalizationManager.FormatList(list));
    using (args.PushGroup("HandsComponent"))
    {
      ExaminedEvent examinedEvent = args;
      ILocalizationManager loc = this.Loc;
      string messageId = str;
      (string, EntityUid) valueTuple3 = valueTuple1;
      (string, object) valueTuple4 = (valueTuple3.Item1, (object) valueTuple3.Item2);
      (string, string) valueTuple5 = valueTuple2;
      (string, object) valueTuple6 = (valueTuple5.Item1, (object) valueTuple5.Item2);
      string markup = loc.GetString(messageId, valueTuple4, valueTuple6);
      examinedEvent.PushMarkup(markup);
    }
  }

  private void InitializePickup()
  {
    this.SubscribeLocalEvent<HandsComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<HandsComponent, EntInsertedIntoContainerMessage>(this.HandleEntityInserted));
  }

  protected virtual void HandleEntityInserted(
    EntityUid uid,
    HandsComponent hands,
    EntInsertedIntoContainerMessage args)
  {
    Hand? hand;
    if (!this.TryGetHand((Entity<HandsComponent>) uid, args.Container.ID, out hand))
      return;
    DidEquipHandEvent args1 = new DidEquipHandEvent(uid, args.Entity, hand.Value);
    this.RaiseLocalEvent<DidEquipHandEvent>(uid, args1);
    GotEquippedHandEvent args2 = new GotEquippedHandEvent(uid, args.Entity, hand.Value);
    this.RaiseLocalEvent<GotEquippedHandEvent>(args.Entity, args2);
  }

  public bool TryPickupAnyHand(
    EntityUid uid,
    EntityUid entity,
    bool checkActionBlocker = true,
    bool animateUser = false,
    bool animate = true,
    HandsComponent? handsComp = null,
    ItemComponent? item = null)
  {
    string emptyHand;
    return this.Resolve<HandsComponent>(uid, ref handsComp, false) && this.TryGetEmptyHand((Entity<HandsComponent>) (uid, handsComp), out emptyHand) && this.TryPickup(uid, entity, emptyHand, checkActionBlocker, animateUser, animate, handsComp, item);
  }

  public bool TryPickup(
    EntityUid uid,
    EntityUid entity,
    string? handId = null,
    bool checkActionBlocker = true,
    bool animateUser = false,
    bool animate = true,
    HandsComponent? handsComp = null,
    ItemComponent? item = null)
  {
    if (!this.Resolve<HandsComponent>(uid, ref handsComp, false))
      return false;
    if (handId == null)
      handId = handsComp.ActiveHandId;
    if (handId == null || !this.Resolve<ItemComponent>(entity, ref item, false) || !this.CanPickupToHand(uid, entity, handId, checkActionBlocker, handsComp, item))
      return false;
    if (animate)
    {
      TransformComponent xform1 = this.Transform(uid);
      EntityUid entityUid = xform1.ParentUid.IsValid() ? xform1.ParentUid : uid;
      TransformComponent xform2 = this.Transform(entity);
      MapCoordinates mapCoordinates = this.TransformSystem.GetMapCoordinates(entity, xform2);
      if (mapCoordinates.MapId == xform1.MapID && (double) (mapCoordinates.Position - this.TransformSystem.GetMapCoordinates(uid, xform1).Position).Length() <= 10.0 && (int) this.MetaData(entity).VisibilityMask == (int) this.MetaData(uid).VisibilityMask)
      {
        EntityCoordinates coordinates = this.TransformSystem.ToCoordinates((Entity<TransformComponent>) entityUid, mapCoordinates);
        this._storage.PlayPickupAnimation(entity, coordinates, xform1.Coordinates, xform2.LocalRotation, new EntityUid?(uid));
      }
    }
    this.DoPickup(uid, handId, entity, handsComp);
    return true;
  }

  public bool TryForcePickup(
    Entity<HandsComponent?> ent,
    EntityUid entity,
    string hand,
    bool checkActionBlocker = true,
    bool animate = true,
    HandsComponent? handsComp = null,
    ItemComponent? item = null)
  {
    if (!this.Resolve<HandsComponent>((EntityUid) ent, ref ent.Comp, false))
      return false;
    Entity<HandsComponent> ent1 = ent;
    string handId = hand;
    bool flag = checkActionBlocker;
    EntityCoordinates? targetDropLocation = new EntityCoordinates?();
    int num = flag ? 1 : 0;
    this.TryDrop(ent1, handId, targetDropLocation, num != 0);
    return this.TryPickup((EntityUid) ent, entity, hand, checkActionBlocker, animate: animate, handsComp: handsComp, item: item);
  }

  public bool TryForcePickupAnyHand(
    EntityUid uid,
    EntityUid entity,
    bool checkActionBlocker = true,
    HandsComponent? handsComp = null,
    ItemComponent? item = null)
  {
    if (!this.Resolve<HandsComponent>(uid, ref handsComp, false))
      return false;
    if (this.TryPickupAnyHand(uid, entity, checkActionBlocker, handsComp: handsComp))
      return true;
    foreach (string key in handsComp.Hands.Keys)
    {
      Entity<HandsComponent> ent = (Entity<HandsComponent>) (uid, handsComp);
      string handId = key;
      bool flag = checkActionBlocker;
      EntityCoordinates? targetDropLocation = new EntityCoordinates?();
      int num = flag ? 1 : 0;
      if (this.TryDrop(ent, handId, targetDropLocation, num != 0) && this.TryPickup(uid, entity, key, checkActionBlocker, handsComp: handsComp))
        return true;
    }
    return false;
  }

  public bool CanPickupAnyHand(
    EntityUid uid,
    EntityUid entity,
    bool checkActionBlocker = true,
    HandsComponent? handsComp = null,
    ItemComponent? item = null)
  {
    string emptyHand;
    return this.Resolve<HandsComponent>(uid, ref handsComp, false) && this.TryGetEmptyHand((Entity<HandsComponent>) (uid, handsComp), out emptyHand) && this.CanPickupToHand(uid, entity, emptyHand, checkActionBlocker, handsComp, item);
  }

  public bool CanPickupToHand(
    EntityUid uid,
    EntityUid entity,
    string handId,
    bool checkActionBlocker = true,
    HandsComponent? handsComp = null,
    ItemComponent? item = null)
  {
    BaseContainer container1;
    PhysicsComponent comp;
    if (!this.Resolve<HandsComponent>(uid, ref handsComp, false) || !this.ContainerSystem.TryGetContainer(uid, handId, out container1) || container1.ContainedEntities.FirstOrNull<EntityUid>().HasValue || !this.Resolve<ItemComponent>(entity, ref item, false) || this.TryComp<PhysicsComponent>(entity, out comp) && comp.BodyType == BodyType.Static || checkActionBlocker && !this._actionBlocker.CanPickup(uid, entity))
      return false;
    BaseContainer container2;
    if (this.ContainerSystem.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (entity, (TransformComponent) null, (MetaDataComponent) null), out container2))
    {
      if (!this.ContainerSystem.CanRemove(entity, container2))
        return false;
      EntityUid? entityUid1;
      if (this._inventory.TryGetSlotEntity(uid, container2.ID, out entityUid1))
      {
        EntityUid? nullable = entityUid1;
        EntityUid entityUid2 = entity;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() == entityUid2 ? 1 : 0) : 0) != 0 && !this._inventory.CanUnequip(uid, container2.ID, out string _))
          return false;
      }
    }
    return this.ContainerSystem.CanInsert(entity, container1);
  }

  public void PickupOrDrop(
    EntityUid? uid,
    EntityUid entity,
    bool checkActionBlocker = true,
    bool animateUser = false,
    bool animate = true,
    bool dropNear = false,
    HandsComponent? handsComp = null,
    ItemComponent? item = null)
  {
    if (uid.HasValue && this.Resolve<HandsComponent>(uid.Value, ref handsComp, false) && this.TryPickupAnyHand(uid.Value, entity, checkActionBlocker, animateUser, animate, handsComp, item))
      return;
    this.ContainerSystem.AttachParentToContainerOrGrid((Entity<TransformComponent>) (entity, this.Transform(entity)));
    if (!dropNear || !uid.HasValue)
      return;
    this.TransformSystem.PlaceNextTo((Entity<TransformComponent>) entity, (Entity<TransformComponent>) uid.Value);
  }

  public virtual void DoPickup(
    EntityUid uid,
    string hand,
    EntityUid entity,
    HandsComponent? hands = null,
    bool log = true)
  {
    BaseContainer container;
    if (!this.Resolve<HandsComponent>(uid, ref hands) || !this.ContainerSystem.TryGetContainer(uid, hand, out container) || container.ContainedEntities.FirstOrNull<EntityUid>().HasValue)
      return;
    if (!this.ContainerSystem.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) entity, container))
    {
      this.Log.Error($"Failed to insert {this.ToPrettyString((Entity<MetaDataComponent>) entity)} into users hand container when picking up. User: {this.ToPrettyString((Entity<MetaDataComponent>) uid)}. Hand: {hand}.");
    }
    else
    {
      this._interactionSystem.DoContactInteraction(uid, new EntityUid?(entity));
      if (log)
      {
        ISharedAdminLogManager adminLogger = this._adminLogger;
        LogStringHandler logStringHandler = new LogStringHandler(11, 2);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) uid), "user", "ToPrettyString(uid)");
        logStringHandler.AppendLiteral(" picked up ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entity), nameof (entity), "ToPrettyString(entity)");
        ref LogStringHandler local = ref logStringHandler;
        adminLogger.Add(LogType.Pickup, LogImpact.Low, ref local);
      }
      this.Dirty(uid, (IComponent) hands);
      if (!(hand == hands.ActiveHandId))
        return;
      this.RaiseLocalEvent<HandSelectedEvent>(entity, new HandSelectedEvent(uid));
    }
  }

  private void InitializeRelay()
  {
    this.SubscribeLocalEvent<HandsComponent, GetEyeOffsetRelayedEvent>(new EntityEventRefHandler<HandsComponent, GetEyeOffsetRelayedEvent>(this.RelayEvent<GetEyeOffsetRelayedEvent>));
    this.SubscribeLocalEvent<HandsComponent, GetEyePvsScaleRelayedEvent>(new EntityEventRefHandler<HandsComponent, GetEyePvsScaleRelayedEvent>(this.RelayEvent<GetEyePvsScaleRelayedEvent>));
    this.SubscribeLocalEvent<HandsComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<HandsComponent, RefreshMovementSpeedModifiersEvent>(this.RelayEvent<RefreshMovementSpeedModifiersEvent>));
    this.SubscribeLocalEvent<HandsComponent, ExtinguishEvent>(new EntityEventRefHandler<HandsComponent, ExtinguishEvent>(this.RefRelayEvent<ExtinguishEvent>));
    this.SubscribeLocalEvent<HandsComponent, ProjectileReflectAttemptEvent>(new EntityEventRefHandler<HandsComponent, ProjectileReflectAttemptEvent>(this.RefRelayEvent<ProjectileReflectAttemptEvent>));
    this.SubscribeLocalEvent<HandsComponent, HitScanReflectAttemptEvent>(new EntityEventRefHandler<HandsComponent, HitScanReflectAttemptEvent>(this.RefRelayEvent<HitScanReflectAttemptEvent>));
    this.SubscribeLocalEvent<HandsComponent, WieldAttemptEvent>(new EntityEventRefHandler<HandsComponent, WieldAttemptEvent>(this.RefRelayEvent<WieldAttemptEvent>));
    this.SubscribeLocalEvent<HandsComponent, UnwieldAttemptEvent>(new EntityEventRefHandler<HandsComponent, UnwieldAttemptEvent>(this.RefRelayEvent<UnwieldAttemptEvent>));
    this.SubscribeLocalEvent<HandsComponent, TargetHandcuffedEvent>(new EntityEventRefHandler<HandsComponent, TargetHandcuffedEvent>(this.RefRelayEvent<TargetHandcuffedEvent>));
  }

  private void RelayEvent<T>(Entity<HandsComponent> entity, ref T args) where T : EntityEventArgs
  {
    this.CoreRelayEvent<T>(entity, ref args);
  }

  private void RefRelayEvent<T>(Entity<HandsComponent> entity, ref T args)
  {
    HeldRelayedEvent<T> heldRelayedEvent = this.CoreRelayEvent<T>(entity, ref args);
    args = heldRelayedEvent.Args;
  }

  private HeldRelayedEvent<T> CoreRelayEvent<T>(Entity<HandsComponent> entity, ref T args)
  {
    HeldRelayedEvent<T> args1 = new HeldRelayedEvent<T>(args);
    foreach (EntityUid uid in this.EnumerateHeld(entity.AsNullable()))
      this.RaiseLocalEvent<HeldRelayedEvent<T>>(uid, ref args1);
    return args1;
  }
}
