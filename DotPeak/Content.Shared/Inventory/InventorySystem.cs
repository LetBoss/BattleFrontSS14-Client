// Decompiled with JetBrains decompiler
// Type: Content.Shared.Inventory.InventorySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Hands;
using Content.Shared.Armor;
using Content.Shared.Atmos;
using Content.Shared.Chat;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Hypospray.Events;
using Content.Shared.Climbing.Events;
using Content.Shared.Clothing.Components;
using Content.Shared.Contraband;
using Content.Shared.Damage;
using Content.Shared.Damage.Events;
using Content.Shared.DoAfter;
using Content.Shared.Electrocution;
using Content.Shared.Explosion;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.Flash;
using Content.Shared.Gravity;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Implants;
using Content.Shared.Interaction;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Overlays;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Radio;
using Content.Shared.Slippery;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Strip;
using Content.Shared.Strip.Components;
using Content.Shared.Temperature;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Whitelist;
using Content.Shared.Wieldable;
using Content.Shared.Zombies;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared.Inventory;

public abstract class InventorySystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private SharedInteractionSystem _interactionSystem;
  [Dependency]
  private SharedItemSystem _item;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _containerSystem;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _handsSystem;
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  private SharedStrippableSystem _strippable;
  [Dependency]
  private RMCHandsSystem _rmcHands;
  private static readonly ProtoId<ItemSizePrototype> PocketableItemSize = (ProtoId<ItemSizePrototype>) "Small";
  [Dependency]
  private SharedStorageSystem _storageSystem;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IViewVariablesManager _vvm;

  public override void Initialize()
  {
    base.Initialize();
    this.InitializeEquip();
    this.InitializeRelay();
    this.InitializeSlots();
  }

  public override void Shutdown()
  {
    base.Shutdown();
    this.ShutdownSlots();
  }

  private void InitializeEquip()
  {
    this.SubscribeLocalEvent<InventoryComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<InventoryComponent, EntInsertedIntoContainerMessage>(this.OnEntInserted));
    this.SubscribeLocalEvent<InventoryComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<InventoryComponent, EntRemovedFromContainerMessage>(this.OnEntRemoved));
    this.SubscribeAllEvent<UseSlotNetworkMessage>(new EntitySessionEventHandler<UseSlotNetworkMessage>(this.OnUseSlot));
  }

  private void OnEntRemoved(
    EntityUid uid,
    InventoryComponent component,
    EntRemovedFromContainerMessage args)
  {
    SlotDefinition slotDefinition;
    if (!this.TryGetSlot(uid, args.Container.ID, out slotDefinition, component))
      return;
    DidUnequipEvent args1 = new DidUnequipEvent(uid, args.Entity, slotDefinition);
    this.RaiseLocalEvent<DidUnequipEvent>(uid, args1, true);
    GotUnequippedEvent args2 = new GotUnequippedEvent(uid, args.Entity, slotDefinition);
    this.RaiseLocalEvent<GotUnequippedEvent>(args.Entity, args2, true);
  }

  private void OnEntInserted(
    EntityUid uid,
    InventoryComponent component,
    EntInsertedIntoContainerMessage args)
  {
    SlotDefinition slotDefinition;
    if (!this.TryGetSlot(uid, args.Container.ID, out slotDefinition, component))
      return;
    DidEquipEvent args1 = new DidEquipEvent(uid, args.Entity, slotDefinition);
    this.RaiseLocalEvent<DidEquipEvent>(uid, args1, true);
    GotEquippedEvent args2 = new GotEquippedEvent(uid, args.Entity, slotDefinition);
    this.RaiseLocalEvent<GotEquippedEvent>(args.Entity, args2, true);
  }

  private void OnUseSlot(UseSlotNetworkMessage ev, EntitySessionEventArgs eventArgs)
  {
    EntityUid? attachedEntity = eventArgs.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    InventoryComponent comp1;
    HandsComponent comp2;
    if (!valueOrDefault.Valid || !this.TryComp<InventoryComponent>(valueOrDefault, out comp1) || !this.TryComp<HandsComponent>(valueOrDefault, out comp2))
      return;
    EntityUid? activeItem = this._handsSystem.GetActiveItem((Entity<HandsComponent>) (valueOrDefault, comp2));
    EntityUid? entityUid;
    this.TryGetSlotEntity(valueOrDefault, ev.Slot, out entityUid, comp1);
    if (activeItem.HasValue && entityUid.HasValue)
      this._interactionSystem.InteractUsing(valueOrDefault, activeItem.Value, entityUid.Value, this.Transform(entityUid.Value).Coordinates);
    else if (entityUid.HasValue)
    {
      EntityUid? removedItem;
      if (this._rmcHands.TryStorageEjectHand(valueOrDefault, entityUid.Value) || !this.TryUnequip(valueOrDefault, ev.Slot, out removedItem, predicted: true, inventory: comp1, checkDoafter: true, triggerHandContact: true))
        return;
      this._handsSystem.PickupOrDrop(new EntityUid?(valueOrDefault), removedItem.Value);
    }
    else
    {
      if (!activeItem.HasValue)
        return;
      string reason;
      if (!this.CanEquip(valueOrDefault, activeItem.Value, ev.Slot, out reason))
      {
        this._popup.PopupCursor(this.Loc.GetString(reason));
      }
      else
      {
        if (!this._handsSystem.CanDropHeld(valueOrDefault, comp2.ActiveHandId, false))
          return;
        this.RaiseLocalEvent<HandDeselectedEvent>(activeItem.Value, new HandDeselectedEvent(valueOrDefault));
        this.TryEquip(valueOrDefault, valueOrDefault, activeItem.Value, ev.Slot, force: true, predicted: true, inventory: comp1, checkDoafter: true, triggerHandContact: true);
      }
    }
  }

  public bool TryEquip(
    EntityUid uid,
    EntityUid itemUid,
    string slot,
    bool silent = false,
    bool force = false,
    bool predicted = false,
    InventoryComponent? inventory = null,
    ClothingComponent? clothing = null,
    bool checkDoafter = false,
    bool triggerHandContact = false,
    bool doRangeCheck = true)
  {
    return this.TryEquip(uid, uid, itemUid, slot, silent, force, predicted, inventory, clothing, checkDoafter, triggerHandContact, doRangeCheck);
  }

  public bool TryEquip(
    EntityUid actor,
    EntityUid target,
    EntityUid itemUid,
    string slot,
    bool silent = false,
    bool force = false,
    bool predicted = false,
    InventoryComponent? inventory = null,
    ClothingComponent? clothing = null,
    bool checkDoafter = false,
    bool triggerHandContact = false,
    bool doRangeCheck = true)
  {
    if (!this.Resolve<InventoryComponent>(target, ref inventory, false))
    {
      if (!silent)
        this._popup.PopupCursor(this.Loc.GetString("inventory-component-can-equip-cannot"));
      return false;
    }
    this.Resolve<ClothingComponent>(itemUid, ref clothing, false);
    ContainerSlot containerSlot;
    SlotDefinition slotDefinition;
    if (!this.TryGetSlotContainer(target, slot, out containerSlot, out slotDefinition, inventory))
    {
      if (!silent)
        this._popup.PopupCursor(this.Loc.GetString("inventory-component-can-equip-cannot"));
      return false;
    }
    string reason;
    if (!force && !this.CanEquip(actor, target, itemUid, slot, out reason, slotDefinition, inventory, clothing, doRangeCheck: doRangeCheck))
    {
      if (!silent)
        this._popup.PopupCursor(this.Loc.GetString(reason));
      return false;
    }
    if (checkDoafter && clothing != null && clothing.EquipDelay > TimeSpan.Zero && (clothing.Slots & slotDefinition.SlotFlags) != SlotFlags.NONE && this._containerSystem.CanInsert(itemUid, (BaseContainer) containerSlot))
    {
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, actor, clothing.EquipDelay, (DoAfterEvent) new ClothingEquipDoAfterEvent(slot), new EntityUid?(itemUid), new EntityUid?(target), new EntityUid?(itemUid))
      {
        BreakOnMove = true,
        NeedHand = true
      });
      return false;
    }
    if (!this._containerSystem.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) itemUid, (BaseContainer) containerSlot))
    {
      if (!silent)
        this._popup.PopupCursor(this.Loc.GetString("inventory-component-can-unequip-cannot"));
      return false;
    }
    if (!silent && clothing != null)
      this._audio.PlayPredicted(clothing.EquipSound, target, new EntityUid?(actor));
    if (triggerHandContact && (slotDefinition.SlotFlags & SlotFlags.GLOVES) != SlotFlags.NONE)
      this.TriggerHandContactInteraction(target);
    this.Dirty(target, (IComponent) inventory);
    this._movementSpeed.RefreshMovementSpeedModifiers(target);
    return true;
  }

  public bool CanAccess(EntityUid actor, EntityUid target, EntityUid itemUid, bool doRangeCheck = true)
  {
    AttachedClothingComponent comp;
    if (this.TryComp<AttachedClothingComponent>(itemUid, out comp))
      itemUid = comp.AttachedUid;
    if (actor != target && (!doRangeCheck || !this._interactionSystem.InRangeUnobstructed((Entity<TransformComponent>) actor, (Entity<TransformComponent>) target) || !this._containerSystem.IsInSameOrParentContainer((Entity<TransformComponent, MetaDataComponent>) actor, (Entity<TransformComponent, MetaDataComponent>) target)))
      return false;
    if (this._interactionSystem.InRangeAndAccessible((Entity<TransformComponent>) actor, (Entity<TransformComponent>) itemUid))
      return true;
    return actor != target && this.HasComp<StrippableComponent>(target) && this.HasComp<StrippingComponent>(actor) && this.HasComp<HandsComponent>(actor);
  }

  public bool CanEquip(
    EntityUid uid,
    EntityUid itemUid,
    string slot,
    [NotNullWhen(false)] out string? reason,
    SlotDefinition? slotDefinition = null,
    InventoryComponent? inventory = null,
    ClothingComponent? clothing = null,
    ItemComponent? item = null,
    bool doRangeCheck = true)
  {
    return this.CanEquip(uid, uid, itemUid, slot, out reason, slotDefinition, inventory, clothing, item, doRangeCheck);
  }

  public bool CanEquip(
    EntityUid actor,
    EntityUid target,
    EntityUid itemUid,
    string slot,
    [NotNullWhen(false)] out string? reason,
    SlotDefinition? slotDefinition = null,
    InventoryComponent? inventory = null,
    ClothingComponent? clothing = null,
    ItemComponent? item = null,
    bool doRangeCheck = true)
  {
    reason = "inventory-component-can-equip-cannot";
    if (!this.Resolve<InventoryComponent>(target, ref inventory, false))
      return false;
    this.Resolve<ClothingComponent, ItemComponent>(itemUid, ref clothing, ref item, false);
    if (slotDefinition == null && !this.TryGetSlot(target, slot, out slotDefinition, inventory))
      return false;
    if (slotDefinition.DependsOn != null)
    {
      EntityUid? entityUid;
      if (!this.TryGetSlotEntity(target, slotDefinition.DependsOn, out entityUid, inventory))
        return false;
      ComponentRegistry dependsOnComponents = slotDefinition.DependsOnComponents;
      if (dependsOnComponents != null)
      {
        foreach ((string _, EntityPrototype.ComponentRegistryEntry componentRegistryEntry) in (Dictionary<string, EntityPrototype.ComponentRegistryEntry>) dependsOnComponents)
        {
          AllowSuitStorageComponent comp;
          if (!this.HasComp(entityUid, componentRegistryEntry.Component.GetType()) || this.TryComp<AllowSuitStorageComponent>(entityUid, out comp) && this._whitelistSystem.IsWhitelistFailOrNull(comp.Whitelist, itemUid))
            return false;
        }
      }
    }
    bool flag = slotDefinition.SlotFlags.HasFlag((Enum) SlotFlags.POCKET) && item != null && this._item.GetSizePrototype(item.Size) <= this._item.GetSizePrototype(InventorySystem.PocketableItemSize);
    if (clothing == null && !flag || clothing != null && !clothing.Slots.HasFlag((Enum) slotDefinition.SlotFlags) && !flag)
    {
      reason = "inventory-component-can-equip-does-not-fit";
      return false;
    }
    if (!this.CanAccess(actor, target, itemUid, doRangeCheck))
    {
      reason = "interaction-system-user-interaction-cannot-reach";
      return false;
    }
    if (this._whitelistSystem.IsWhitelistFail(slotDefinition.Whitelist, itemUid) || this._whitelistSystem.IsBlacklistPass(slotDefinition.Blacklist, itemUid))
    {
      reason = "inventory-component-can-equip-does-not-fit";
      return false;
    }
    IsEquippingAttemptEvent args1 = new IsEquippingAttemptEvent(actor, target, itemUid, slotDefinition);
    this.RaiseLocalEvent<IsEquippingAttemptEvent>(actor, args1, true);
    if (args1.Cancelled)
    {
      reason = args1.Reason ?? reason;
      return false;
    }
    IsEquippingTargetAttemptEvent args2 = new IsEquippingTargetAttemptEvent(actor, target, itemUid, slotDefinition);
    this.RaiseLocalEvent<IsEquippingTargetAttemptEvent>(target, args2, true);
    if (args2.Cancelled)
    {
      reason = args2.Reason ?? reason;
      return false;
    }
    BeingEquippedAttemptEvent args3 = new BeingEquippedAttemptEvent(actor, target, itemUid, slotDefinition);
    this.RaiseLocalEvent<BeingEquippedAttemptEvent>(itemUid, args3, true);
    if (!args3.Cancelled)
      return true;
    reason = args3.Reason ?? reason;
    return false;
  }

  public bool TryUnequip(
    EntityUid uid,
    string slot,
    bool silent = false,
    bool force = false,
    bool predicted = false,
    InventoryComponent? inventory = null,
    ClothingComponent? clothing = null,
    bool reparent = true,
    bool checkDoafter = false,
    bool triggerHandContact = false)
  {
    return this.TryUnequip(uid, uid, slot, silent, force, predicted, inventory, clothing, reparent, checkDoafter, triggerHandContact);
  }

  public bool TryUnequip(
    EntityUid actor,
    EntityUid target,
    string slot,
    bool silent = false,
    bool force = false,
    bool predicted = false,
    InventoryComponent? inventory = null,
    ClothingComponent? clothing = null,
    bool reparent = true,
    bool checkDoafter = false,
    bool triggerHandContact = false)
  {
    return this.TryUnequip(actor, target, slot, out EntityUid? _, silent, force, predicted, inventory, clothing, reparent, checkDoafter, triggerHandContact);
  }

  public bool TryUnequip(
    EntityUid uid,
    string slot,
    [NotNullWhen(true)] out EntityUid? removedItem,
    bool silent = false,
    bool force = false,
    bool predicted = false,
    InventoryComponent? inventory = null,
    ClothingComponent? clothing = null,
    bool reparent = true,
    bool checkDoafter = false,
    bool triggerHandContact = false)
  {
    return this.TryUnequip(uid, uid, slot, out removedItem, silent, force, predicted, inventory, clothing, reparent, checkDoafter, triggerHandContact);
  }

  public bool TryUnequip(
    EntityUid actor,
    EntityUid target,
    string slot,
    [NotNullWhen(true)] out EntityUid? removedItem,
    bool silent = false,
    bool force = false,
    bool predicted = false,
    InventoryComponent? inventory = null,
    ClothingComponent? clothing = null,
    bool reparent = true,
    bool checkDoafter = false,
    bool triggerHandContact = false)
  {
    int itemsDropped = 0;
    return this.TryUnequip(actor, target, slot, out removedItem, ref itemsDropped, silent, force, predicted, inventory, clothing, reparent, checkDoafter);
  }

  private bool TryUnequip(
    EntityUid actor,
    EntityUid target,
    string slot,
    [NotNullWhen(true)] out EntityUid? removedItem,
    ref int itemsDropped,
    bool silent = false,
    bool force = false,
    bool predicted = false,
    InventoryComponent? inventory = null,
    ClothingComponent? clothing = null,
    bool reparent = true,
    bool checkDoafter = false,
    bool triggerHandContact = false)
  {
    removedItem = new EntityUid?();
    if (this.TerminatingOrDeleted(target))
      return false;
    if (!this.Resolve<InventoryComponent>(target, ref inventory, false))
    {
      if (!silent)
        this._popup.PopupCursor(this.Loc.GetString("inventory-component-can-unequip-cannot"));
      return false;
    }
    ContainerSlot containerSlot;
    SlotDefinition slotDefinition;
    if (!this.TryGetSlotContainer(target, slot, out containerSlot, out slotDefinition, inventory))
    {
      if (!silent)
        this._popup.PopupCursor(this.Loc.GetString("inventory-component-can-unequip-cannot"));
      return false;
    }
    removedItem = containerSlot.ContainedEntity;
    if (!removedItem.HasValue || this.TerminatingOrDeleted(removedItem.Value))
      return false;
    string reason;
    if (!force && !this.CanUnequip(actor, target, slot, out reason, containerSlot, slotDefinition, inventory))
    {
      if (!silent)
        this._popup.PopupCursor(this.Loc.GetString(reason));
      return false;
    }
    if (!force && !this._containerSystem.CanRemove(removedItem.Value, (BaseContainer) containerSlot))
      return false;
    if (checkDoafter && this.Resolve<ClothingComponent>(removedItem.Value, ref clothing, false) && (clothing.Slots & slotDefinition.SlotFlags) != SlotFlags.NONE && clothing.UnequipDelay > TimeSpan.Zero)
    {
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, actor, clothing.UnequipDelay, (DoAfterEvent) new ClothingUnequipDoAfterEvent(slot), new EntityUid?(removedItem.Value), new EntityUid?(target), new EntityUid?(removedItem.Value))
      {
        BreakOnMove = true,
        NeedHand = true
      });
      return false;
    }
    SharedContainerSystem containerSystem = this._containerSystem;
    Entity<TransformComponent, MetaDataComponent> toRemove = (Entity<TransformComponent, MetaDataComponent>) removedItem.Value;
    ContainerSlot container = containerSlot;
    bool flag1 = force;
    int num1 = reparent ? 1 : 0;
    int num2 = flag1 ? 1 : 0;
    EntityCoordinates? destination = new EntityCoordinates?();
    Angle? localRotation = new Angle?();
    if (!containerSystem.Remove(toRemove, (BaseContainer) container, num1 != 0, num2 != 0, destination, localRotation))
      return false;
    bool flag2 = itemsDropped == 0;
    ++itemsDropped;
    foreach (SlotDefinition slot1 in inventory.Slots)
    {
      if (slot1 != slotDefinition && slot1.DependsOn == slotDefinition.Name)
        this.TryUnequip(actor, target, slot1.Name, out EntityUid? _, ref itemsDropped, true, true, predicted, inventory, reparent: reparent);
    }
    if (((silent ? 0 : (this._gameTiming.IsFirstTimePredicted ? 1 : 0)) & (flag2 ? 1 : 0)) != 0 && itemsDropped > 1)
      this._popup.PopupClient(this.Loc.GetString("inventory-component-dropped-from-unequip", ("items", (object) (itemsDropped - 1))), target, new EntityUid?(target));
    if (!this._containerSystem.IsEntityInContainer(removedItem.Value))
      this._transform.DropNextTo((Entity<TransformComponent>) removedItem.Value, (Entity<TransformComponent>) target);
    if (!silent && this.Resolve<ClothingComponent>(removedItem.Value, ref clothing, false) && clothing.UnequipSound != null)
      this._audio.PlayPredicted(clothing.UnequipSound, target, new EntityUid?(actor));
    if (triggerHandContact && (slotDefinition.SlotFlags & SlotFlags.GLOVES) != SlotFlags.NONE)
      this.TriggerHandContactInteraction(target);
    this.Dirty(target, (IComponent) inventory);
    this._movementSpeed.RefreshMovementSpeedModifiers(target);
    return true;
  }

  public bool CanUnequip(
    EntityUid uid,
    string slot,
    [NotNullWhen(false)] out string? reason,
    ContainerSlot? containerSlot = null,
    SlotDefinition? slotDefinition = null,
    InventoryComponent? inventory = null)
  {
    return this.CanUnequip(uid, uid, slot, out reason, containerSlot, slotDefinition, inventory);
  }

  public bool CanUnequip(
    EntityUid actor,
    EntityUid target,
    string slot,
    [NotNullWhen(false)] out string? reason,
    ContainerSlot? containerSlot = null,
    SlotDefinition? slotDefinition = null,
    InventoryComponent? inventory = null)
  {
    reason = "inventory-component-can-unequip-cannot";
    if (!this.Resolve<InventoryComponent>(target, ref inventory, false) || (containerSlot == null || slotDefinition == null) && !this.TryGetSlotContainer(target, slot, out containerSlot, out slotDefinition, inventory))
      return false;
    EntityUid? containedEntity = containerSlot.ContainedEntity;
    if (!containedEntity.HasValue)
      return false;
    EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
    if (!this._containerSystem.CanRemove(valueOrDefault, (BaseContainer) containerSlot))
      return false;
    if (!this.CanAccess(actor, target, valueOrDefault))
    {
      reason = "interaction-system-user-interaction-cannot-reach";
      return false;
    }
    IsUnequippingAttemptEvent args1 = new IsUnequippingAttemptEvent(actor, target, valueOrDefault, slotDefinition);
    this.RaiseLocalEvent<IsUnequippingAttemptEvent>(actor, args1, true);
    if (args1.Cancelled)
    {
      reason = args1.Reason ?? reason;
      return false;
    }
    IsUnequippingTargetAttemptEvent args2 = new IsUnequippingTargetAttemptEvent(actor, target, valueOrDefault, slotDefinition);
    this.RaiseLocalEvent<IsUnequippingTargetAttemptEvent>(target, args2, true);
    if (args2.Cancelled)
    {
      reason = args2.Reason ?? reason;
      return false;
    }
    BeingUnequippedAttemptEvent args3 = new BeingUnequippedAttemptEvent(actor, target, valueOrDefault, slotDefinition);
    this.RaiseLocalEvent<BeingUnequippedAttemptEvent>(valueOrDefault, args3, true);
    if (!args3.Cancelled)
      return true;
    reason = args3.Reason ?? reason;
    return false;
  }

  public bool TryGetSlotEntity(
    EntityUid uid,
    string slot,
    [NotNullWhen(true)] out EntityUid? entityUid,
    InventoryComponent? inventoryComponent = null,
    ContainerManagerComponent? containerManagerComponent = null)
  {
    entityUid = new EntityUid?();
    ContainerSlot containerSlot;
    if (!this.Resolve<InventoryComponent, ContainerManagerComponent>(uid, ref inventoryComponent, ref containerManagerComponent, false) || !this.TryGetSlotContainer(uid, slot, out containerSlot, out SlotDefinition _, inventoryComponent, containerManagerComponent))
      return false;
    entityUid = containerSlot.ContainedEntity;
    return entityUid.HasValue;
  }

  public void TriggerHandContactInteraction(EntityUid uid)
  {
    foreach (EntityUid entityUid in this._handsSystem.EnumerateHeld((Entity<HandsComponent>) uid))
      this._interactionSystem.DoContactInteraction(uid, new EntityUid?(entityUid));
  }

  public IEnumerable<EntityUid> GetHandOrInventoryEntities(
    Entity<HandsComponent?, InventoryComponent?> user,
    SlotFlags flags = SlotFlags.All)
  {
    InventorySystem inventorySystem = this;
    if (inventorySystem.Resolve<HandsComponent>(user.Owner, ref user.Comp1, false))
    {
      foreach (EntityUid orInventoryEntity in inventorySystem._handsSystem.EnumerateHeld((Entity<HandsComponent>) user))
        yield return orInventoryEntity;
    }
    if (inventorySystem.Resolve<InventoryComponent>(user.Owner, ref user.Comp2, false))
    {
      InventorySystem.InventorySlotEnumerator slotEnumerator = new InventorySystem.InventorySlotEnumerator(user.Comp2, flags);
      EntityUid orInventoryEntity;
      while (slotEnumerator.NextItem(out orInventoryEntity))
        yield return orInventoryEntity;
    }
  }

  public bool TryGetContainingSlot(
    Entity<TransformComponent?, MetaDataComponent?> entity,
    [NotNullWhen(true)] out SlotDefinition? slot)
  {
    BaseContainer container;
    if (this._containerSystem.TryGetContainingContainer(entity, out container))
      return this.TryGetSlot(container.Owner, container.ID, out slot);
    slot = (SlotDefinition) null;
    return false;
  }

  public bool InSlotWithFlags(
    Entity<TransformComponent?, MetaDataComponent?> entity,
    SlotFlags flags)
  {
    SlotDefinition slot;
    return this.TryGetContainingSlot(entity, out slot) && (slot.SlotFlags & flags) == flags;
  }

  public bool SpawnItemInSlot(
    EntityUid uid,
    string slot,
    string prototype,
    bool silent = false,
    bool force = false,
    InventoryComponent? inventory = null)
  {
    if (!this.Resolve<InventoryComponent>(uid, ref inventory, false) || this.Deleted(uid) || !this.HasSlot(uid, slot) || this.TryGetSlotEntity(uid, slot, out EntityUid? _, inventory) || !this._prototypeManager.HasIndex<EntityPrototype>(prototype))
      return false;
    EntityUid item = this.Spawn(prototype, this.Transform(uid).Coordinates);
    return this.TryEquip(uid, item, slot, silent, force) || DeleteItem();

    bool DeleteItem()
    {
      this.Del(new EntityUid?(item));
      return false;
    }
  }

  public void SpawnItemsOnEntity(EntityUid entity, List<string> items)
  {
    foreach (string str in items)
      this.SpawnItemOnEntity(entity, (EntProtoId) str);
  }

  public void SpawnItemOnEntity(EntityUid entity, EntProtoId item)
  {
    if (!this.HasComp<TransformComponent>(entity))
      return;
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(this.Transform(entity));
    EntityUid entityUid = this.Spawn((string) item, mapCoordinates, rotation: new Angle());
    ContainerSlot containerSlot1;
    SlotDefinition slotDefinition;
    if (this.TryGetSlotContainer(entity, "back", out containerSlot1, out slotDefinition))
    {
      EntityUid? stackedEntity = containerSlot1.ContainedEntity;
      if (stackedEntity.HasValue && this._storageSystem.Insert(containerSlot1.ContainedEntity.Value, entityUid, out stackedEntity))
        return;
    }
    ContainerSlot containerSlot2;
    ContainerSlot containerSlot3;
    ContainerSlot containerSlot4;
    if (this.TryGetSlotContainer(entity, "pocket1", out containerSlot2, out slotDefinition) && this._containerSystem.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) entityUid, (BaseContainer) containerSlot2) || this.TryGetSlotContainer(entity, "pocket2", out containerSlot3, out slotDefinition) && this._containerSystem.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) entityUid, (BaseContainer) containerSlot3) || this.TryGetSlotContainer(entity, "belt", out containerSlot4, out slotDefinition) && this._containerSystem.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) entityUid, (BaseContainer) containerSlot4))
      return;
    this._handsSystem.PickupOrDrop(new EntityUid?(entity), entityUid, false);
  }

  public void InitializeRelay()
  {
    this.SubscribeLocalEvent<InventoryComponent, DamageModifyEvent>(new ComponentEventHandler<InventoryComponent, DamageModifyEvent>(this.RelayInventoryEvent<DamageModifyEvent>));
    this.SubscribeLocalEvent<InventoryComponent, ElectrocutionAttemptEvent>(new ComponentEventHandler<InventoryComponent, ElectrocutionAttemptEvent>(this.RelayInventoryEvent<ElectrocutionAttemptEvent>));
    this.SubscribeLocalEvent<InventoryComponent, SlipAttemptEvent>(new ComponentEventHandler<InventoryComponent, SlipAttemptEvent>(this.RelayInventoryEvent<SlipAttemptEvent>));
    this.SubscribeLocalEvent<InventoryComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<InventoryComponent, RefreshMovementSpeedModifiersEvent>(this.RelayInventoryEvent<RefreshMovementSpeedModifiersEvent>));
    this.SubscribeLocalEvent<InventoryComponent, BeforeStripEvent>(new ComponentEventHandler<InventoryComponent, BeforeStripEvent>(this.RelayInventoryEvent<BeforeStripEvent>));
    this.SubscribeLocalEvent<InventoryComponent, SeeIdentityAttemptEvent>(new ComponentEventHandler<InventoryComponent, SeeIdentityAttemptEvent>(this.RelayInventoryEvent<SeeIdentityAttemptEvent>));
    this.SubscribeLocalEvent<InventoryComponent, ModifyChangedTemperatureEvent>(new ComponentEventHandler<InventoryComponent, ModifyChangedTemperatureEvent>(this.RelayInventoryEvent<ModifyChangedTemperatureEvent>));
    this.SubscribeLocalEvent<InventoryComponent, GetDefaultRadioChannelEvent>(new ComponentEventHandler<InventoryComponent, GetDefaultRadioChannelEvent>(this.RelayInventoryEvent<GetDefaultRadioChannelEvent>));
    this.SubscribeLocalEvent<InventoryComponent, RefreshNameModifiersEvent>(new ComponentEventHandler<InventoryComponent, RefreshNameModifiersEvent>(this.RelayInventoryEvent<RefreshNameModifiersEvent>));
    this.SubscribeLocalEvent<InventoryComponent, TransformSpeakerNameEvent>(new ComponentEventHandler<InventoryComponent, TransformSpeakerNameEvent>(this.RelayInventoryEvent<TransformSpeakerNameEvent>));
    this.SubscribeLocalEvent<InventoryComponent, SelfBeforeHyposprayInjectsEvent>(new ComponentEventHandler<InventoryComponent, SelfBeforeHyposprayInjectsEvent>(this.RelayInventoryEvent<SelfBeforeHyposprayInjectsEvent>));
    this.SubscribeLocalEvent<InventoryComponent, TargetBeforeHyposprayInjectsEvent>(new ComponentEventHandler<InventoryComponent, TargetBeforeHyposprayInjectsEvent>(this.RelayInventoryEvent<TargetBeforeHyposprayInjectsEvent>));
    this.SubscribeLocalEvent<InventoryComponent, SelfBeforeGunShotEvent>(new ComponentEventHandler<InventoryComponent, SelfBeforeGunShotEvent>(this.RelayInventoryEvent<SelfBeforeGunShotEvent>));
    this.SubscribeLocalEvent<InventoryComponent, SelfBeforeClimbEvent>(new ComponentEventHandler<InventoryComponent, SelfBeforeClimbEvent>(this.RelayInventoryEvent<SelfBeforeClimbEvent>));
    this.SubscribeLocalEvent<InventoryComponent, CoefficientQueryEvent>(new ComponentEventHandler<InventoryComponent, CoefficientQueryEvent>(this.RelayInventoryEvent<CoefficientQueryEvent>));
    this.SubscribeLocalEvent<InventoryComponent, ZombificationResistanceQueryEvent>(new ComponentEventHandler<InventoryComponent, ZombificationResistanceQueryEvent>(this.RelayInventoryEvent<ZombificationResistanceQueryEvent>));
    this.SubscribeLocalEvent<InventoryComponent, IsEquippingTargetAttemptEvent>(new ComponentEventHandler<InventoryComponent, IsEquippingTargetAttemptEvent>(this.RelayInventoryEvent<IsEquippingTargetAttemptEvent>));
    this.SubscribeLocalEvent<InventoryComponent, IsUnequippingTargetAttemptEvent>(new ComponentEventHandler<InventoryComponent, IsUnequippingTargetAttemptEvent>(this.RelayInventoryEvent<IsUnequippingTargetAttemptEvent>));
    this.SubscribeLocalEvent<InventoryComponent, ChameleonControllerOutfitSelectedEvent>(new ComponentEventHandler<InventoryComponent, ChameleonControllerOutfitSelectedEvent>(this.RelayInventoryEvent<ChameleonControllerOutfitSelectedEvent>));
    this.SubscribeLocalEvent<InventoryComponent, RefreshFrictionModifiersEvent>(new ComponentEventRefHandler<InventoryComponent, RefreshFrictionModifiersEvent>(this.RefRelayInventoryEvent<RefreshFrictionModifiersEvent>));
    this.SubscribeLocalEvent<InventoryComponent, BeforeStaminaDamageEvent>(new ComponentEventRefHandler<InventoryComponent, BeforeStaminaDamageEvent>(this.RefRelayInventoryEvent<BeforeStaminaDamageEvent>));
    this.SubscribeLocalEvent<InventoryComponent, GetExplosionResistanceEvent>(new ComponentEventRefHandler<InventoryComponent, GetExplosionResistanceEvent>(this.RefRelayInventoryEvent<GetExplosionResistanceEvent>));
    this.SubscribeLocalEvent<InventoryComponent, IsWeightlessEvent>(new ComponentEventRefHandler<InventoryComponent, IsWeightlessEvent>(this.RefRelayInventoryEvent<IsWeightlessEvent>));
    this.SubscribeLocalEvent<InventoryComponent, GetSpeedModifierContactCapEvent>(new ComponentEventRefHandler<InventoryComponent, GetSpeedModifierContactCapEvent>(this.RefRelayInventoryEvent<GetSpeedModifierContactCapEvent>));
    this.SubscribeLocalEvent<InventoryComponent, GetSlowedOverSlipperyModifierEvent>(new ComponentEventRefHandler<InventoryComponent, GetSlowedOverSlipperyModifierEvent>(this.RefRelayInventoryEvent<GetSlowedOverSlipperyModifierEvent>));
    this.SubscribeLocalEvent<InventoryComponent, ModifySlowOnDamageSpeedEvent>(new ComponentEventRefHandler<InventoryComponent, ModifySlowOnDamageSpeedEvent>(this.RefRelayInventoryEvent<ModifySlowOnDamageSpeedEvent>));
    this.SubscribeLocalEvent<InventoryComponent, ExtinguishEvent>(new ComponentEventRefHandler<InventoryComponent, ExtinguishEvent>(this.RefRelayInventoryEvent<ExtinguishEvent>));
    this.SubscribeLocalEvent<InventoryComponent, ProjectileReflectAttemptEvent>(new ComponentEventRefHandler<InventoryComponent, ProjectileReflectAttemptEvent>(this.RefRelayInventoryEvent<ProjectileReflectAttemptEvent>));
    this.SubscribeLocalEvent<InventoryComponent, HitScanReflectAttemptEvent>(new ComponentEventRefHandler<InventoryComponent, HitScanReflectAttemptEvent>(this.RefRelayInventoryEvent<HitScanReflectAttemptEvent>));
    this.SubscribeLocalEvent<InventoryComponent, GetContrabandDetailsEvent>(new ComponentEventRefHandler<InventoryComponent, GetContrabandDetailsEvent>(this.RefRelayInventoryEvent<GetContrabandDetailsEvent>));
    this.SubscribeLocalEvent<InventoryComponent, FlashAttemptEvent>(new ComponentEventRefHandler<InventoryComponent, FlashAttemptEvent>(this.RefRelayInventoryEvent<FlashAttemptEvent>));
    this.SubscribeLocalEvent<InventoryComponent, WieldAttemptEvent>(new ComponentEventRefHandler<InventoryComponent, WieldAttemptEvent>(this.RefRelayInventoryEvent<WieldAttemptEvent>));
    this.SubscribeLocalEvent<InventoryComponent, UnwieldAttemptEvent>(new ComponentEventRefHandler<InventoryComponent, UnwieldAttemptEvent>(this.RefRelayInventoryEvent<UnwieldAttemptEvent>));
    this.SubscribeLocalEvent<InventoryComponent, CanSeeAttemptEvent>(new ComponentEventHandler<InventoryComponent, CanSeeAttemptEvent>(this.RelayInventoryEvent<CanSeeAttemptEvent>));
    this.SubscribeLocalEvent<InventoryComponent, GetEyeProtectionEvent>(new ComponentEventHandler<InventoryComponent, GetEyeProtectionEvent>(this.RelayInventoryEvent<GetEyeProtectionEvent>));
    this.SubscribeLocalEvent<InventoryComponent, GetBlurEvent>(new ComponentEventHandler<InventoryComponent, GetBlurEvent>(this.RelayInventoryEvent<GetBlurEvent>));
    this.SubscribeLocalEvent<InventoryComponent, SolutionScanEvent>(new ComponentEventHandler<InventoryComponent, SolutionScanEvent>(this.RelayInventoryEvent<SolutionScanEvent>));
    this.SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowJobIconsComponent>>(new ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowJobIconsComponent>>(this.RefRelayInventoryEvent<RefreshEquipmentHudEvent<ShowJobIconsComponent>>));
    this.SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowHealthBarsComponent>>(new ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowHealthBarsComponent>>(this.RefRelayInventoryEvent<RefreshEquipmentHudEvent<ShowHealthBarsComponent>>));
    this.SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowHealthIconsComponent>>(new ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowHealthIconsComponent>>(this.RefRelayInventoryEvent<RefreshEquipmentHudEvent<ShowHealthIconsComponent>>));
    this.SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowHungerIconsComponent>>(new ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowHungerIconsComponent>>(this.RefRelayInventoryEvent<RefreshEquipmentHudEvent<ShowHungerIconsComponent>>));
    this.SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowThirstIconsComponent>>(new ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowThirstIconsComponent>>(this.RefRelayInventoryEvent<RefreshEquipmentHudEvent<ShowThirstIconsComponent>>));
    this.SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowMindShieldIconsComponent>>(new ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowMindShieldIconsComponent>>(this.RefRelayInventoryEvent<RefreshEquipmentHudEvent<ShowMindShieldIconsComponent>>));
    this.SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowSyndicateIconsComponent>>(new ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowSyndicateIconsComponent>>(this.RefRelayInventoryEvent<RefreshEquipmentHudEvent<ShowSyndicateIconsComponent>>));
    this.SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowCriminalRecordIconsComponent>>(new ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowCriminalRecordIconsComponent>>(this.RefRelayInventoryEvent<RefreshEquipmentHudEvent<ShowCriminalRecordIconsComponent>>));
    this.SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<BlackAndWhiteOverlayComponent>>(new ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<BlackAndWhiteOverlayComponent>>(this.RefRelayInventoryEvent<RefreshEquipmentHudEvent<BlackAndWhiteOverlayComponent>>));
    this.SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<NoirOverlayComponent>>(new ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<NoirOverlayComponent>>(this.RefRelayInventoryEvent<RefreshEquipmentHudEvent<NoirOverlayComponent>>));
    this.SubscribeLocalEvent<InventoryComponent, GetVerbsEvent<EquipmentVerb>>(new ComponentEventHandler<InventoryComponent, GetVerbsEvent<EquipmentVerb>>(this.OnGetEquipmentVerbs));
    this.SubscribeLocalEvent<InventoryComponent, GetVerbsEvent<InnateVerb>>(new ComponentEventHandler<InventoryComponent, GetVerbsEvent<InnateVerb>>(this.OnGetInnateVerbs));
  }

  protected void RefRelayInventoryEvent<T>(EntityUid uid, InventoryComponent component, ref T args) where T : IInventoryRelayEvent
  {
    this.RelayEvent<T>((Entity<InventoryComponent>) (uid, component), ref args);
  }

  protected void RelayInventoryEvent<T>(EntityUid uid, InventoryComponent component, T args) where T : IInventoryRelayEvent
  {
    this.RelayEvent<T>((Entity<InventoryComponent>) (uid, component), args);
  }

  public void RelayEvent<T>(Entity<InventoryComponent> inventory, ref T args) where T : IInventoryRelayEvent
  {
    if (args.TargetSlots == SlotFlags.NONE)
      return;
    InventoryRelayedEvent<T> args1 = new InventoryRelayedEvent<T>(args);
    InventorySystem.InventorySlotEnumerator inventorySlotEnumerator = new InventorySystem.InventorySlotEnumerator((InventoryComponent) inventory, args.TargetSlots);
    EntityUid uid;
    while (inventorySlotEnumerator.NextItem(out uid))
      this.RaiseLocalEvent<InventoryRelayedEvent<T>>(uid, args1);
    args = args1.Args;
  }

  public void RelayEvent<T>(Entity<InventoryComponent> inventory, T args) where T : IInventoryRelayEvent
  {
    if (args.TargetSlots == SlotFlags.NONE)
      return;
    InventoryRelayedEvent<T> args1 = new InventoryRelayedEvent<T>(args);
    InventorySystem.InventorySlotEnumerator inventorySlotEnumerator = new InventorySystem.InventorySlotEnumerator((InventoryComponent) inventory, args.TargetSlots);
    EntityUid uid;
    while (inventorySlotEnumerator.NextItem(out uid))
      this.RaiseLocalEvent<InventoryRelayedEvent<T>>(uid, args1);
  }

  private void OnGetEquipmentVerbs(
    EntityUid uid,
    InventoryComponent component,
    GetVerbsEvent<EquipmentVerb> args)
  {
    InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>> args1 = new InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>(args);
    InventorySystem.InventorySlotEnumerator inventorySlotEnumerator = new InventorySystem.InventorySlotEnumerator(component);
    EntityUid uid1;
    SlotDefinition slot;
    while (inventorySlotEnumerator.NextItem(out uid1, out slot))
    {
      if (!this._strippable.IsStripHidden(slot, new EntityUid?(args.User)) || args.User == uid)
        this.RaiseLocalEvent<InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>>(uid1, args1);
    }
  }

  private void OnGetInnateVerbs(
    EntityUid uid,
    InventoryComponent component,
    GetVerbsEvent<InnateVerb> args)
  {
    InventoryRelayedEvent<GetVerbsEvent<InnateVerb>> args1 = new InventoryRelayedEvent<GetVerbsEvent<InnateVerb>>(args);
    InventorySystem.InventorySlotEnumerator inventorySlotEnumerator = new InventorySystem.InventorySlotEnumerator(component, SlotFlags.WITHOUT_POCKET);
    EntityUid uid1;
    while (inventorySlotEnumerator.NextItem(out uid1))
      this.RaiseLocalEvent<InventoryRelayedEvent<GetVerbsEvent<InnateVerb>>>(uid1, args1);
  }

  private void InitializeSlots()
  {
    this.SubscribeLocalEvent<InventoryComponent, ComponentInit>(new ComponentEventHandler<InventoryComponent, ComponentInit>(this.OnInit));
    this.SubscribeAllEvent<OpenSlotStorageNetworkMessage>(new EntitySessionEventHandler<OpenSlotStorageNetworkMessage>(this.OnOpenSlotStorage));
    this._vvm.GetTypeHandler<InventoryComponent>().AddHandler(new HandleTypePathComponent<InventoryComponent>(this.HandleViewVariablesSlots), new ListTypeCustomPathsComponent<InventoryComponent>(this.ListViewVariablesSlots));
    this.SubscribeLocalEvent<InventoryComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<InventoryComponent, AfterAutoHandleStateEvent>(this.AfterAutoState));
  }

  private void ShutdownSlots()
  {
    this._vvm.GetTypeHandler<InventoryComponent>().RemoveHandler(new HandleTypePathComponent<InventoryComponent>(this.HandleViewVariablesSlots), new ListTypeCustomPathsComponent<InventoryComponent>(this.ListViewVariablesSlots));
  }

  public bool TryGetInventoryEntity<T>(Entity<InventoryComponent?> entity, out Entity<T?> target) where T : IComponent, IClothingSlots
  {
    InventorySystem.InventorySlotEnumerator containerSlotEnumerator;
    if (this.TryGetContainerSlotEnumerator((Entity<InventoryComponent>) entity.Owner, out containerSlotEnumerator))
    {
      EntityUid uid;
      SlotDefinition slot;
      while (containerSlotEnumerator.NextItem(out uid, out slot))
      {
        T comp;
        if (this.TryComp<T>(uid, out comp) && (comp.Slots & slot.SlotFlags) != SlotFlags.NONE)
        {
          target = (Entity<T>) (uid, comp);
          return true;
        }
      }
    }
    target = (Entity<T>) EntityUid.Invalid;
    return false;
  }

  protected virtual void OnInit(EntityUid uid, InventoryComponent component, ComponentInit args)
  {
    InventoryTemplatePrototype prototype;
    if (!this._prototypeManager.TryIndex<InventoryTemplatePrototype>(component.TemplateId, out prototype))
      return;
    component.Slots = prototype.Slots;
    component.Containers = new ContainerSlot[component.Slots.Length];
    for (int index = 0; index < component.Containers.Length; ++index)
    {
      SlotDefinition slot = component.Slots[index];
      ContainerSlot containerSlot = this._containerSystem.EnsureContainer<ContainerSlot>(uid, slot.Name);
      containerSlot.OccludesLight = false;
      component.Containers[index] = containerSlot;
    }
  }

  private void AfterAutoState(Entity<InventoryComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    this.UpdateInventoryTemplate(ent);
  }

  protected virtual void UpdateInventoryTemplate(Entity<InventoryComponent> ent)
  {
    InventoryTemplatePrototype prototype;
    if (ent.Comp.LifeStage < ComponentLifeStage.Initialized || !this._prototypeManager.TryIndex<InventoryTemplatePrototype>(ent.Comp.TemplateId, out prototype))
      return;
    ent.Comp.Slots = prototype.Slots;
    InventoryTemplateUpdated args = new InventoryTemplateUpdated();
    this.RaiseLocalEvent<InventoryTemplateUpdated>((EntityUid) ent, ref args);
  }

  public void RefreshInventoryTemplate(Entity<InventoryComponent> ent)
  {
    this.UpdateInventoryTemplate(ent);
  }

  private void OnOpenSlotStorage(OpenSlotStorageNetworkMessage ev, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    EntityUid? entityUid;
    StorageComponent comp;
    if (!valueOrDefault.Valid || !this.TryGetSlotEntity(valueOrDefault, ev.Slot, out entityUid) || !this.TryComp<StorageComponent>(entityUid, out comp))
      return;
    this._storageSystem.OpenStorageUI(entityUid.Value, valueOrDefault, comp, false);
  }

  public bool TryGetSlotContainer(
    EntityUid uid,
    string slot,
    [NotNullWhen(true)] out ContainerSlot? containerSlot,
    [NotNullWhen(true)] out SlotDefinition? slotDefinition,
    InventoryComponent? inventory = null,
    ContainerManagerComponent? containerComp = null)
  {
    containerSlot = (ContainerSlot) null;
    slotDefinition = (SlotDefinition) null;
    if (!this.Resolve<InventoryComponent, ContainerManagerComponent>(uid, ref inventory, ref containerComp, false) || !this.TryGetSlot(uid, slot, out slotDefinition, inventory))
      return false;
    BaseContainer container;
    if (!this._containerSystem.TryGetContainer(uid, slotDefinition.Name, out container, containerComp))
    {
      if (inventory.LifeStage >= ComponentLifeStage.Initialized)
        this.Log.Error($"Missing inventory container {slot} on entity {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
      return false;
    }
    if (!(container is ContainerSlot containerSlot1))
      return false;
    containerSlot = containerSlot1;
    return true;
  }

  public bool HasSlot(EntityUid uid, string slot, InventoryComponent? component = null)
  {
    return this.TryGetSlot(uid, slot, out SlotDefinition _, component);
  }

  public bool TryGetSlot(
    EntityUid uid,
    string slot,
    [NotNullWhen(true)] out SlotDefinition? slotDefinition,
    InventoryComponent? inventory = null)
  {
    slotDefinition = (SlotDefinition) null;
    if (!this.Resolve<InventoryComponent>(uid, ref inventory, false))
      return false;
    foreach (SlotDefinition slot1 in inventory.Slots)
    {
      if (slot1.Name.Equals(slot))
      {
        slotDefinition = slot1;
        return true;
      }
    }
    return false;
  }

  public bool TryGetContainerSlotEnumerator(
    Entity<InventoryComponent?> entity,
    out InventorySystem.InventorySlotEnumerator containerSlotEnumerator,
    SlotFlags flags = SlotFlags.All)
  {
    if (!this.Resolve<InventoryComponent>(entity.Owner, ref entity.Comp, false))
    {
      containerSlotEnumerator = new InventorySystem.InventorySlotEnumerator();
      return false;
    }
    containerSlotEnumerator = new InventorySystem.InventorySlotEnumerator(entity.Comp, flags);
    return true;
  }

  public InventorySystem.InventorySlotEnumerator GetSlotEnumerator(
    Entity<InventoryComponent?> entity,
    SlotFlags flags = SlotFlags.All)
  {
    return !this.Resolve<InventoryComponent>(entity.Owner, ref entity.Comp, false) ? InventorySystem.InventorySlotEnumerator.Empty : new InventorySystem.InventorySlotEnumerator(entity.Comp, flags);
  }

  public bool TryGetSlots(EntityUid uid, [NotNullWhen(true)] out SlotDefinition[]? slotDefinitions)
  {
    InventoryComponent comp;
    if (!this.TryComp<InventoryComponent>(uid, out comp))
    {
      slotDefinitions = (SlotDefinition[]) null;
      return false;
    }
    slotDefinitions = comp.Slots;
    return true;
  }

  private ViewVariablesPath? HandleViewVariablesSlots(
    EntityUid uid,
    InventoryComponent comp,
    string relativePath)
  {
    EntityUid? entityUid;
    return !this.TryGetSlotEntity(uid, relativePath, out entityUid, comp) ? (ViewVariablesPath) null : (ViewVariablesPath) ViewVariablesPath.FromObject((object) entityUid);
  }

  private IEnumerable<string> ListViewVariablesSlots(EntityUid uid, InventoryComponent comp)
  {
    SlotDefinition[] slotDefinitionArray = comp.Slots;
    for (int index = 0; index < slotDefinitionArray.Length; ++index)
      yield return slotDefinitionArray[index].Name;
    slotDefinitionArray = (SlotDefinition[]) null;
  }

  public void SetTemplateId(
    Entity<InventoryComponent> ent,
    ProtoId<InventoryTemplatePrototype> newTemplate)
  {
    if (!((IEnumerable<SlotDefinition>) this._prototypeManager.Index<InventoryTemplatePrototype>(newTemplate).Slots).Select<SlotDefinition, string>((Func<SlotDefinition, string>) (x => x.Name)).SequenceEqual<string>(((IEnumerable<SlotDefinition>) ent.Comp.Slots).Select<SlotDefinition, string>((Func<SlotDefinition, string>) (x => x.Name))))
      throw new ArgumentException("Incompatible inventory template!");
    ent.Comp.TemplateId = (string) newTemplate;
    this.Dirty<InventoryComponent>(ent);
  }

  public bool ForceSetTemplateId(
    Entity<InventoryComponent?> ent,
    ProtoId<InventoryTemplatePrototype> newTemplate)
  {
    InventoryComponent comp = ent.Comp;
    if (!this.Resolve<InventoryComponent>(ent.Owner, ref comp, false) || !this._prototypeManager.TryIndex<InventoryTemplatePrototype>(newTemplate, out InventoryTemplatePrototype _))
      return false;
    int num = (ProtoId<InventoryTemplatePrototype>) comp.TemplateId != newTemplate ? 1 : 0;
    if (num != 0)
      comp.TemplateId = (string) newTemplate;
    bool flag = this.EnsureTemplateSlots((Entity<InventoryComponent>) (ent.Owner, comp));
    if ((num | (flag ? 1 : 0)) != 0)
      this.Dirty(ent.Owner, (IComponent) comp);
    return (num | (flag ? 1 : 0)) != 0;
  }

  public bool EnsureTemplateSlots(Entity<InventoryComponent?> ent)
  {
    InventoryComponent comp = ent.Comp;
    InventoryTemplatePrototype prototype;
    if (!this.Resolve<InventoryComponent>(ent.Owner, ref comp, false) || !this._prototypeManager.TryIndex<InventoryTemplatePrototype>(comp.TemplateId, out prototype))
      return false;
    if (comp.Slots.Length == prototype.Slots.Length)
    {
      bool flag = true;
      for (int index = 0; index < prototype.Slots.Length; ++index)
      {
        SlotDefinition slot1 = comp.Slots[index];
        SlotDefinition slot2 = prototype.Slots[index];
        if (!(slot1.Name == slot2.Name) || slot1.SlotFlags != slot2.SlotFlags || !(slot1.SlotGroup == slot2.SlotGroup) || slot1.ShowInWindow != slot2.ShowInWindow || !Vector2i.op_Equality(slot1.UIWindowPosition, slot2.UIWindowPosition) || !Vector2i.op_Equality(slot1.StrippingWindowPos, slot2.StrippingWindowPos))
        {
          flag = false;
          break;
        }
      }
      if (flag)
        return false;
    }
    Dictionary<string, ContainerSlot> dictionary = new Dictionary<string, ContainerSlot>(comp.Slots.Length);
    for (int index = 0; index < comp.Slots.Length && index < comp.Containers.Length; ++index)
      dictionary[comp.Slots[index].Name] = comp.Containers[index];
    ContainerSlot[] containerSlotArray = new ContainerSlot[prototype.Slots.Length];
    for (int index = 0; index < prototype.Slots.Length; ++index)
    {
      SlotDefinition slot = prototype.Slots[index];
      ContainerSlot containerSlot1;
      if (dictionary.TryGetValue(slot.Name, out containerSlot1))
      {
        containerSlotArray[index] = containerSlot1;
      }
      else
      {
        ContainerSlot containerSlot2 = this._containerSystem.EnsureContainer<ContainerSlot>(ent.Owner, slot.Name);
        containerSlot2.OccludesLight = false;
        containerSlotArray[index] = containerSlot2;
      }
    }
    comp.Slots = prototype.Slots;
    comp.Containers = containerSlotArray;
    this.Dirty(ent.Owner, (IComponent) comp);
    return true;
  }

  public struct InventorySlotEnumerator(
    SlotDefinition[] slots,
    ContainerSlot[] containers,
    SlotFlags flags = SlotFlags.All)
  {
    private readonly SlotDefinition[] _slots = slots;
    private readonly ContainerSlot[] _containers = containers;
    private readonly SlotFlags _flags = flags;
    private int _nextIdx = 0;
    public static InventorySystem.InventorySlotEnumerator Empty = new InventorySystem.InventorySlotEnumerator(Array.Empty<SlotDefinition>(), Array.Empty<ContainerSlot>());

    public InventorySlotEnumerator(InventoryComponent inventory, SlotFlags flags = SlotFlags.All)
      : this(inventory.Slots, inventory.Containers, flags)
    {
    }

    public bool MoveNext([NotNullWhen(true)] out ContainerSlot? container)
    {
      while (this._nextIdx < this._slots.Length)
      {
        int index = this._nextIdx++;
        if ((this._slots[index].SlotFlags & this._flags) != SlotFlags.NONE)
        {
          container = this._containers[index];
          return true;
        }
      }
      container = (ContainerSlot) null;
      return false;
    }

    public bool NextItem(out EntityUid item)
    {
      while (this._nextIdx < this._slots.Length)
      {
        int index = this._nextIdx++;
        if ((this._slots[index].SlotFlags & this._flags) != SlotFlags.NONE)
        {
          EntityUid? containedEntity = this._containers[index].ContainedEntity;
          if (containedEntity.HasValue)
          {
            EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
            item = valueOrDefault;
            return true;
          }
        }
      }
      item = new EntityUid();
      return false;
    }

    public bool NextItem(out EntityUid item, [NotNullWhen(true)] out SlotDefinition? slot)
    {
      while (this._nextIdx < this._slots.Length)
      {
        int index = this._nextIdx++;
        slot = this._slots[index];
        if ((slot.SlotFlags & this._flags) != SlotFlags.NONE)
        {
          EntityUid? containedEntity = this._containers[index].ContainedEntity;
          if (containedEntity.HasValue)
          {
            EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
            item = valueOrDefault;
            return true;
          }
        }
      }
      item = new EntityUid();
      slot = (SlotDefinition) null;
      return false;
    }
  }
}
