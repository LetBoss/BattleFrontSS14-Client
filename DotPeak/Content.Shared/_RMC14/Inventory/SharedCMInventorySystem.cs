// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Inventory.SharedCMInventorySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Input;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared.Administration.Logs;
using Content.Shared.Clothing.Components;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Strip.Components;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Inventory;

public abstract class SharedCMInventorySystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private ItemSlotsSystem _itemSlots;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedStorageSystem _storage;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedItemSystem _item;
  [Dependency]
  private SkillsSystem _skills;
  private readonly SlotFlags[] _order = new SlotFlags[6]
  {
    SlotFlags.SUITSTORAGE,
    SlotFlags.BELT,
    SlotFlags.BACK,
    SlotFlags.POCKET,
    SlotFlags.INNERCLOTHING,
    SlotFlags.FEET
  };
  private readonly SlotFlags[] _quickEquipOrder = new SlotFlags[15]
  {
    SlotFlags.BACK,
    SlotFlags.IDCARD,
    SlotFlags.INNERCLOTHING,
    SlotFlags.OUTERCLOTHING,
    SlotFlags.HEAD,
    SlotFlags.FEET,
    SlotFlags.MASK,
    SlotFlags.GLOVES,
    SlotFlags.EARS,
    SlotFlags.EYES,
    SlotFlags.BELT,
    SlotFlags.SUITSTORAGE,
    SlotFlags.NECK,
    SlotFlags.POCKET,
    SlotFlags.LEGS
  };
  private Robust.Shared.GameObjects.EntityQuery<RMCPickupDroppedItemsComponent> _pickupDroppedItemsQuery;

  public override void Initialize()
  {
    this._pickupDroppedItemsQuery = this.GetEntityQuery<RMCPickupDroppedItemsComponent>();
    this.SubscribeLocalEvent<GunComponent, IsUnholsterableEvent>(new EntityEventRefHandler<GunComponent, IsUnholsterableEvent>(this.AllowUnholster<GunComponent>));
    this.SubscribeLocalEvent<MeleeWeaponComponent, IsUnholsterableEvent>(new EntityEventRefHandler<MeleeWeaponComponent, IsUnholsterableEvent>(this.AllowUnholster<MeleeWeaponComponent>));
    this.SubscribeLocalEvent<CMItemSlotsComponent, MapInitEvent>(new EntityEventRefHandler<CMItemSlotsComponent, MapInitEvent>(this.OnSlotsFillMapInit));
    this.SubscribeLocalEvent<CMItemSlotsComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<CMItemSlotsComponent, AfterAutoHandleStateEvent>(this.OnSlotsComponentHandleState));
    this.SubscribeLocalEvent<CMItemSlotsComponent, ActivateInWorldEvent>(new EntityEventRefHandler<CMItemSlotsComponent, ActivateInWorldEvent>(this.OnSlotsActivateInWorld));
    this.SubscribeLocalEvent<CMItemSlotsComponent, ItemSlotEjectAttemptEvent>(new EntityEventRefHandler<CMItemSlotsComponent, ItemSlotEjectAttemptEvent>(this.OnSlotsEjectAttempt));
    this.SubscribeLocalEvent<CMItemSlotsComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<CMItemSlotsComponent, EntInsertedIntoContainerMessage>(this.OnSlotsEntInsertedIntoContainer));
    this.SubscribeLocalEvent<CMItemSlotsComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<CMItemSlotsComponent, EntRemovedFromContainerMessage>(this.OnSlotsEntRemovedFromContainer));
    this.SubscribeLocalEvent<CMItemSlotsComponent, InteractUsingEvent>(new EntityEventRefHandler<CMItemSlotsComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<CMHolsterComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<CMHolsterComponent, GetVerbsEvent<AlternativeVerb>>(this.OnHolsterGetAltVerbs));
    this.SubscribeLocalEvent<CMHolsterComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<CMHolsterComponent, AfterAutoHandleStateEvent>(this.OnHolsterComponentHandleState));
    this.SubscribeLocalEvent<CMHolsterComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<CMHolsterComponent, EntInsertedIntoContainerMessage>(this.OnHolsterEntInsertedIntoContainer));
    this.SubscribeLocalEvent<CMHolsterComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<CMHolsterComponent, EntRemovedFromContainerMessage>(this.OnHolsterEntRemovedFromContainer));
    this.SubscribeLocalEvent<RMCItemPickupComponent, DroppedEvent>(new EntityEventRefHandler<RMCItemPickupComponent, DroppedEvent>(this.OnItemDropped));
    this.SubscribeLocalEvent<RMCItemPickupComponent, RMCDroppedEvent>(new EntityEventRefHandler<RMCItemPickupComponent, RMCDroppedEvent>(this.OnItemDropped));
    this.SubscribeLocalEvent<RMCStripTimeSkillComponent, BeforeStripEvent>(new EntityEventRefHandler<RMCStripTimeSkillComponent, BeforeStripEvent>(this.OnSkilledBeforeStrip));
    this.SubscribeLocalEvent<CMVirtualItemComponent, BeforeRangedInteractEvent>(new EntityEventRefHandler<CMVirtualItemComponent, BeforeRangedInteractEvent>(this.OnVirtualBeforeRangedInteract), after: new Type[1]
    {
      typeof (SharedVirtualItemSystem)
    });
    CommandBinds.Builder.Bind(CMKeyFunctions.CMHolsterPrimary, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
    {
      EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      this.OnHolster(attachedEntity.GetValueOrDefault(), 0);
    }), handle: false)).Bind(CMKeyFunctions.CMHolsterSecondary, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
    {
      EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      this.OnHolster(attachedEntity.GetValueOrDefault(), 1);
    }), handle: false)).Bind(CMKeyFunctions.CMHolsterTertiary, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
    {
      EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      this.OnHolster(attachedEntity.GetValueOrDefault(), 2);
    }), handle: false)).Bind(CMKeyFunctions.CMHolsterQuaternary, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
    {
      EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      this.OnHolster(attachedEntity.GetValueOrDefault(), 3, CMHolsterChoose.Last);
    }), handle: false)).Bind(CMKeyFunctions.RMCPickUpDroppedItems, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
    {
      EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
      if (!attachedEntity.HasValue)
        return;
      this.TryPickupDroppedItems(attachedEntity.GetValueOrDefault());
    }), handle: false)).Register<SharedCMInventorySystem>();
  }

  private void OnHolsterGetAltVerbs(
    EntityUid holster,
    CMHolsterComponent comp,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || comp.Contents.Count == 0 || this.HasComp<CMItemSlotsComponent>(holster))
      return;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = (Action) (() => this.Unholster(args.User, holster, out bool _));
    alternativeVerb1.Text = this.Loc.GetString("rmc-storage-holster-eject-verb");
    alternativeVerb1.IconEntity = new NetEntity?(this.GetNetEntity(comp.Contents[0]));
    alternativeVerb1.Priority = 5;
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
  }

  public override void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<SharedCMInventorySystem>();
  }

  private void AllowUnholster<T>(Entity<T> ent, ref IsUnholsterableEvent args) where T : IComponent?
  {
    args.Unholsterable = true;
  }

  private void OnSlotsFillMapInit(Entity<CMItemSlotsComponent> ent, ref MapInitEvent args)
  {
    ItemSlot slot1 = ent.Comp.Slot;
    if (slot1 == null)
      return;
    int? count = ent.Comp.Count;
    if (!count.HasValue)
      return;
    int valueOrDefault = count.GetValueOrDefault();
    List<EntProtoId> entProtoIdList = new List<EntProtoId>();
    EntProtoId? startingItem = ent.Comp.StartingItem;
    if (startingItem.HasValue)
    {
      entProtoIdList = Enumerable.Repeat<EntProtoId>(startingItem.GetValueOrDefault(), valueOrDefault).ToList<EntProtoId>();
    }
    else
    {
      List<EntProtoId> startingItems = ent.Comp.StartingItems;
      if (startingItems != null)
        entProtoIdList = startingItems;
    }
    ItemSlotsComponent itemSlotsComponent = this.EnsureComp<ItemSlotsComponent>((EntityUid) ent);
    EntityCoordinates coordinates = this.Transform((EntityUid) ent).Coordinates;
    for (int index = 0; index < valueOrDefault; ++index)
    {
      int num = index + 1;
      ItemSlot slot2 = new ItemSlot(slot1);
      slot2.Name = $"{slot2.Name} {num}";
      this._itemSlots.AddItemSlot((EntityUid) ent, $"{slot1.Name}{num}", slot2);
      if (entProtoIdList.Count > index)
      {
        EntProtoId prototype = entProtoIdList[index];
        ContainerSlot containerSlot = slot2.ContainerSlot;
        if (containerSlot != null)
          this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) this.Spawn((string) prototype, coordinates), (BaseContainer) containerSlot);
        else
          slot2.StartingItem = (string) prototype;
      }
    }
    this.ContentsUpdated(ent);
    this.Dirty((EntityUid) ent, (IComponent) itemSlotsComponent);
  }

  private void OnSlotsComponentHandleState(
    Entity<CMItemSlotsComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.ContentsUpdated(ent);
  }

  private void OnHolsterComponentHandleState(
    Entity<CMHolsterComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.ContentsUpdated(ent);
  }

  private void OnSlotsActivateInWorld(
    Entity<CMItemSlotsComponent> ent,
    ref ActivateInWorldEvent args)
  {
    if (this.HasComp<StorageComponent>((EntityUid) ent))
      return;
    this.PickupSlot(args.User, (EntityUid) ent);
  }

  private void OnSlotsEjectAttempt(
    Entity<CMItemSlotsComponent> ent,
    ref ItemSlotEjectAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    TimeSpan? cooldown = ent.Comp.Cooldown;
    if (!cooldown.HasValue)
      return;
    TimeSpan valueOrDefault = cooldown.GetValueOrDefault();
    if (!(this._timing.CurTime < ent.Comp.LastEjectAt + valueOrDefault))
      return;
    args.Cancelled = true;
  }

  private void OnInteractUsing(Entity<CMItemSlotsComponent> ent, ref InteractUsingEvent args)
  {
    ItemSlotsComponent comp1;
    ItemSlotsComponent comp2;
    if (!this.TryComp<ItemSlotsComponent>(args.Used, out comp1) || !this.TryComp<ItemSlotsComponent>((EntityUid) ent, out comp2) || args.Handled)
      return;
    SoundSpecifier sound = (SoundSpecifier) null;
    foreach (KeyValuePair<string, ItemSlot> slot1 in comp1.Slots)
    {
      BaseContainer container1;
      if (this._container.TryGetContainer(args.Used, slot1.Key, out container1))
      {
        foreach (KeyValuePair<string, ItemSlot> slot2 in comp2.Slots)
        {
          BaseContainer container2;
          if (this._container.TryGetContainer((EntityUid) ent, slot2.Key, out container2))
          {
            if (this._itemSlots.CanInsert(ent.Owner, args.Used, new EntityUid?(args.User), slot2.Value) && this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) args.Used, container2))
            {
              sound = slot2.Value.InsertSound;
              args.Handled = true;
              break;
            }
            foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container1.ContainedEntities)
            {
              if (this._itemSlots.CanInsert(ent.Owner, containedEntity, new EntityUid?(args.User), slot2.Value) && this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) containedEntity, container2))
              {
                sound = slot2.Value.InsertSound;
                args.Handled = true;
              }
            }
          }
        }
      }
    }
    this._audio.PlayPredicted(sound, (EntityUid) ent, new EntityUid?(args.User));
  }

  protected void OnSlotsEntInsertedIntoContainer(
    Entity<CMItemSlotsComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    this.ContentsUpdated(ent);
  }

  protected void OnSlotsEntRemovedFromContainer(
    Entity<CMItemSlotsComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    if (!this._timing.ApplyingState)
    {
      ent.Comp.LastEjectAt = this._timing.CurTime;
      this.Dirty<CMItemSlotsComponent>(ent);
    }
    this.ContentsUpdated(ent);
  }

  protected void OnHolsterEntInsertedIntoContainer(
    Entity<CMHolsterComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    EntityUid entity = args.Entity;
    IsUnholsterableEvent args1 = new IsUnholsterableEvent();
    this.RaiseLocalEvent<IsUnholsterableEvent>(entity, ref args1);
    if (args1.Unholsterable && !ent.Comp.Contents.Contains(entity))
    {
      EntityWhitelist whitelist = ent.Comp.Whitelist;
      if (whitelist == null || this._whitelist.IsWhitelistPass(whitelist, entity))
        ent.Comp.Contents.Add(entity);
    }
    this.ContentsUpdated(ent);
  }

  protected void OnHolsterEntRemovedFromContainer(
    Entity<CMHolsterComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    if (!this._timing.ApplyingState)
    {
      ent.Comp.LastEjectAt = this._timing.CurTime;
      this.Dirty<CMHolsterComponent>(ent);
    }
    EntityUid entity = args.Entity;
    ent.Comp.Contents.Remove(entity);
    this.ContentsUpdated(ent);
  }

  protected void OnItemDropped(Entity<RMCItemPickupComponent> ent, ref DroppedEvent args)
  {
    this.HandleDroppedItem(ent, args.User);
  }

  protected void OnItemDropped(Entity<RMCItemPickupComponent> ent, ref RMCDroppedEvent args)
  {
    this.HandleDroppedItem(ent, args.User);
  }

  protected void HandleDroppedItem(Entity<RMCItemPickupComponent> item, EntityUid user)
  {
    RMCPickupDroppedItemsComponent component;
    if (!this._pickupDroppedItemsQuery.TryComp(user, out component))
      return;
    component.DroppedItems.Add(item.Owner);
  }

  protected void TryPickupDroppedItems(EntityUid user)
  {
    RMCPickupDroppedItemsComponent component;
    if (!this._pickupDroppedItemsQuery.TryComp(user, out component) || this.HasComp<DevouredComponent>(user))
      return;
    foreach (EntityUid entityUid in component.DroppedItems.OrderByDescending<EntityUid, bool>((Func<EntityUid, bool>) (item => this.HasComp<GunComponent>(item))).ThenByDescending<EntityUid, bool>((Func<EntityUid, bool>) (item => this.HasComp<MeleeWeaponComponent>(item))).ToList<EntityUid>().Distinct<EntityUid>())
    {
      if (!this._container.IsEntityInContainer(entityUid) && this._interaction.InRangeUnobstructed((Entity<TransformComponent>) user, (Entity<TransformComponent>) entityUid) && this._hands.TryPickupAnyHand(user, entityUid))
      {
        component.DroppedItems.Remove(entityUid);
        break;
      }
    }
  }

  protected void OnSkilledBeforeStrip(
    Entity<RMCStripTimeSkillComponent> ent,
    ref BeforeStripEvent args)
  {
    args.Multiplier = this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) ent.Owner, ent.Comp.Skill);
  }

  private void OnVirtualBeforeRangedInteract(
    Entity<CMVirtualItemComponent> ent,
    ref BeforeRangedInteractEvent args)
  {
    VirtualItemComponent comp;
    if (!this.TryComp<VirtualItemComponent>((EntityUid) ent, out comp))
      return;
    ShouldHandleVirtualItemInteractEvent args1 = new ShouldHandleVirtualItemInteractEvent(args);
    this.RaiseLocalEvent<ShouldHandleVirtualItemInteractEvent>(comp.BlockingEntity, ref args1);
    if (!args1.Handle)
      return;
    this.RaiseLocalEvent<BeforeRangedInteractEvent>(comp.BlockingEntity, args);
  }

  protected virtual void ContentsUpdated(Entity<CMItemSlotsComponent> ent)
  {
    (int Filled, int Total) = this.GetItemSlotsFilled((Entity<ItemSlotsComponent>) ent.Owner);
    CMItemSlotsVisuals itemSlotsVisuals = Total != 0 ? (Filled < Total ? ((double) Filled < (double) Total * 0.66600000858306885 ? ((double) Filled < (double) Total * 0.33300000429153442 ? (Filled <= 0 ? CMItemSlotsVisuals.Empty : CMItemSlotsVisuals.Low) : CMItemSlotsVisuals.Medium) : CMItemSlotsVisuals.High) : CMItemSlotsVisuals.Full) : CMItemSlotsVisuals.Empty;
    this._appearance.SetData((EntityUid) ent, (Enum) CMItemSlotsLayers.Fill, (object) itemSlotsVisuals);
  }

  protected virtual void ContentsUpdated(Entity<CMHolsterComponent> ent)
  {
    CMHolsterVisuals cmHolsterVisuals = CMHolsterVisuals.Empty;
    int num = 0;
    if (ent.Comp.Contents.Count != 0)
    {
      cmHolsterVisuals = CMHolsterVisuals.Full;
      foreach (EntityUid content in ent.Comp.Contents)
      {
        ItemComponent comp;
        if (this.TryComp<ItemComponent>(content, out comp))
          num += this._item.GetItemShape(comp).GetArea();
      }
    }
    this._appearance.SetData((EntityUid) ent, (Enum) CMHolsterLayers.Fill, (object) cmHolsterVisuals);
  }

  private bool SlotCanInteract(EntityUid user, EntityUid holster, [NotNullWhen(true)] out ItemSlotsComponent? itemSlots)
  {
    if (!this.TryComp<ItemSlotsComponent>(holster, out itemSlots))
      return false;
    BaseContainer container;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (holster, (TransformComponent) null), out container) || !(container.Owner != user) || !this._inventory.HasSlot(container.Owner, container.ID))
      return true;
    itemSlots = (ItemSlotsComponent) null;
    return false;
  }

  private bool PickupSlot(EntityUid user, EntityUid holster, EntityWhitelist? whitelist = null)
  {
    ItemSlotsComponent itemSlots;
    if (!this.SlotCanInteract(user, holster, out itemSlots))
      return false;
    foreach (ItemSlot slot in (IEnumerable<ItemSlot>) itemSlots.Slots.Values.OrderBy<ItemSlot, int>((Func<ItemSlot, int>) (s => s.Priority)))
    {
      EntityUid? containedEntity = (EntityUid?) slot.ContainerSlot?.ContainedEntity;
      if ((!containedEntity.HasValue || !this._whitelist.IsWhitelistFail(whitelist, containedEntity.Value)) && this._itemSlots.TryEjectToHands(holster, slot, new EntityUid?(user), true))
      {
        if (containedEntity.HasValue)
        {
          ISharedAdminLogManager adminLog = this._adminLog;
          LogStringHandler logStringHandler = new LogStringHandler(13, 2);
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
          logStringHandler.AppendLiteral(" unholstered ");
          logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(containedEntity), "ToPrettyString(item)");
          ref LogStringHandler local = ref logStringHandler;
          adminLog.Add(LogType.RMCHolster, ref local);
        }
        return true;
      }
    }
    return false;
  }

  private void OnHolster(EntityUid user, int startIndex, CMHolsterChoose choose = CMHolsterChoose.First)
  {
    EntityUid? nullable;
    if (this._hands.TryGetActiveItem((Entity<HandsComponent>) user, out nullable))
      this.Holster(user, nullable.Value);
    else
      this.Unholster(user, startIndex, choose);
  }

  public bool Holster(EntityUid user, EntityUid item, bool act = true)
  {
    List<SharedCMInventorySystem.HolsterSlot> holsterSlotList = new List<SharedCMInventorySystem.HolsterSlot>();
    int Priority = 0;
    EntityUid? stackedEntity;
    string reason;
    foreach (SlotFlags flags in this._quickEquipOrder)
    {
      InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) user, flags);
      ContainerSlot container;
      while (slotEnumerator.MoveNext(out container))
      {
        stackedEntity = container.ContainedEntity;
        if (stackedEntity.HasValue)
        {
          EntityUid valueOrDefault = stackedEntity.GetValueOrDefault();
          CMHolsterComponent comp;
          if (this.TryComp<CMHolsterComponent>(valueOrDefault, out comp))
          {
            EntityWhitelist whitelist = comp.Whitelist;
            if (whitelist == null || this._whitelist.IsWhitelistPass(whitelist, item))
            {
              ItemSlotsComponent itemSlots;
              ItemSlot itemSlot;
              if (this.HasComp<CMItemSlotsComponent>(valueOrDefault) && this.SlotCanInteract(user, valueOrDefault, out itemSlots) && this.TryGetAvailableSlot((Entity<ItemSlotsComponent>) (valueOrDefault, itemSlots), item, new Entity<HandsComponent>?((Entity<HandsComponent>) user), out itemSlot, true) && itemSlot.ContainerSlot != null)
                holsterSlotList.Add(new SharedCMInventorySystem.HolsterSlot(Priority, true, (ContainerSlot) null, valueOrDefault, itemSlot));
              else if (this.HasComp<StorageComponent>(valueOrDefault) && this._storage.CanInsert(valueOrDefault, item, new EntityUid?(user), out reason))
                holsterSlotList.Add(new SharedCMInventorySystem.HolsterSlot(Priority, true, (ContainerSlot) null, valueOrDefault, (ItemSlot) null));
            }
          }
        }
        else if (this._inventory.CanEquip(user, item, container.ID, out reason))
          holsterSlotList.Add(new SharedCMInventorySystem.HolsterSlot(Priority, false, container, user, (ItemSlot) null));
      }
      ++Priority;
    }
    holsterSlotList.Sort();
    foreach (SharedCMInventorySystem.HolsterSlot holsterSlot in holsterSlotList)
    {
      if (!holsterSlot.IsHolster && holsterSlot.Slot != null && this._inventory.CanEquip(user, item, holsterSlot.Slot.ID, out reason))
      {
        if (act)
          this._inventory.TryEquip(user, item, holsterSlot.Slot.ID, true, checkDoafter: true);
        return true;
      }
      if (holsterSlot.ItemSlot != null && this._itemSlots.CanInsert(holsterSlot.Ent, item, new EntityUid?(user), holsterSlot.ItemSlot))
      {
        if (act)
        {
          this._itemSlots.TryInsert(holsterSlot.Ent, holsterSlot.ItemSlot, item, new EntityUid?(user), true);
          ISharedAdminLogManager adminLog = this._adminLog;
          LogStringHandler logStringHandler = new LogStringHandler(11, 2);
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
          logStringHandler.AppendLiteral(" holstered ");
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) item), "ToPrettyString(item)");
          ref LogStringHandler local = ref logStringHandler;
          adminLog.Add(LogType.RMCHolster, ref local);
        }
        return true;
      }
      StorageComponent comp1;
      CMHolsterComponent comp2;
      if (holsterSlot.ItemSlot == null && this.TryComp<StorageComponent>(holsterSlot.Ent, out comp1) && this.TryComp<CMHolsterComponent>(holsterSlot.Ent, out comp2) && !comp2.Contents.Contains(item) && this._hands.CanDrop((Entity<HandsComponent>) user, item) && this._storage.CanInsert(holsterSlot.Ent, item, new EntityUid?(user), out reason, comp1))
      {
        if (act && this._hands.TryDrop((Entity<HandsComponent>) user, item))
        {
          this._storage.Insert(holsterSlot.Ent, item, out stackedEntity, new EntityUid?(user), comp1, false);
          this._audio.PlayPredicted(comp2.InsertSound, item, new EntityUid?(user));
          ISharedAdminLogManager adminLog = this._adminLog;
          LogStringHandler logStringHandler = new LogStringHandler(11, 2);
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
          logStringHandler.AppendLiteral(" holstered ");
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) item), "ToPrettyString(item)");
          ref LogStringHandler local = ref logStringHandler;
          adminLog.Add(LogType.RMCHolster, ref local);
        }
        return true;
      }
    }
    this._popup.PopupClient(this.Loc.GetString("cm-inventory-unable-equip"), user, new EntityUid?(user), PopupType.SmallCaution);
    return false;
  }

  public bool CanHolster(EntityUid user, EntityUid item) => this.Holster(user, item, false);

  private bool TryGetAvailableSlot(
    Entity<ItemSlotsComponent?> ent,
    EntityUid item,
    Entity<HandsComponent?>? userEnt,
    [NotNullWhen(true)] out ItemSlot? itemSlot,
    bool emptyOnly = false)
  {
    itemSlot = (ItemSlot) null;
    if (userEnt.HasValue)
    {
      Entity<HandsComponent> valueOrDefault = userEnt.GetValueOrDefault();
      if (this.Resolve<HandsComponent>((EntityUid) valueOrDefault, ref valueOrDefault.Comp) && this._hands.IsHolding(valueOrDefault, new EntityUid?(item)) && !this._hands.CanDrop(valueOrDefault, item))
        return false;
    }
    if (!this.Resolve<ItemSlotsComponent>((EntityUid) ent, ref ent.Comp, false))
      return false;
    List<ItemSlot> itemSlotList = new List<ItemSlot>();
    foreach (ItemSlot itemSlot1 in ent.Comp.Slots.Values)
    {
      EntityUid? nullable1;
      if (emptyOnly)
      {
        ContainerSlot containerSlot = itemSlot1.ContainerSlot;
        int num;
        if (containerSlot == null)
        {
          num = 0;
        }
        else
        {
          nullable1 = containerSlot.ContainedEntity;
          num = nullable1.HasValue ? 1 : 0;
        }
        if (num != 0)
          continue;
      }
      ItemSlotsSystem itemSlots = this._itemSlots;
      EntityUid uid = (EntityUid) ent;
      EntityUid usedUid = item;
      Entity<HandsComponent>? nullable2 = userEnt;
      EntityUid? user;
      if (!nullable2.HasValue)
      {
        nullable1 = new EntityUid?();
        user = nullable1;
      }
      else
        user = new EntityUid?((EntityUid) nullable2.GetValueOrDefault());
      ItemSlot slot = itemSlot1;
      if (itemSlots.CanInsert(uid, usedUid, user, slot))
        itemSlotList.Add(itemSlot1);
    }
    if (itemSlotList.Count == 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    itemSlotList.Sort(SharedCMInventorySystem.\u003C\u003EO.\u003C0\u003E__SortEmpty ?? (SharedCMInventorySystem.\u003C\u003EO.\u003C0\u003E__SortEmpty = new Comparison<ItemSlot>(ItemSlotsSystem.SortEmpty)));
    itemSlot = itemSlotList[0];
    return true;
  }

  private bool TryGetLastInserted(Entity<CMHolsterComponent?> holster, out EntityUid item)
  {
    item = new EntityUid();
    if (!this.Resolve<CMHolsterComponent>((EntityUid) holster, ref holster.Comp))
      return false;
    List<EntityUid> contents = holster.Comp.Contents;
    if (contents.Count == 0)
      return false;
    item = contents[contents.Count - 1];
    return true;
  }

  private void Unholster(EntityUid user, int startIndex, CMHolsterChoose choose)
  {
    if (this._order.Length == 0)
      return;
    if (startIndex >= this._order.Length)
      startIndex = this._order.Length - 1;
    for (int index = startIndex; index < this._order.Length; ++index)
    {
      bool stop;
      if (this.Unholster(user, this._order[index], choose, out stop) | stop)
        return;
    }
    int index1 = 0;
    bool stop1;
    while (index1 < startIndex && !(this.Unholster(user, this._order[index1], choose, out stop1) | stop1))
      ++index1;
  }

  private bool Unholster(EntityUid user, SlotFlags flag, CMHolsterChoose choose, out bool stop)
  {
    stop = false;
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) user, flag);
    if (choose == CMHolsterChoose.Last)
    {
      List<EntityUid> entityUidList = new List<EntityUid>();
      EntityUid entityUid1;
      while (slotEnumerator.NextItem(out entityUid1))
        entityUidList.Add(entityUid1);
      entityUidList.Reverse();
      foreach (EntityUid entityUid2 in entityUidList)
      {
        if (this.Unholster(user, entityUid2, out stop))
          return true;
      }
    }
    EntityUid entityUid;
    while (slotEnumerator.NextItem(out entityUid))
    {
      if (this.Unholster(user, entityUid, out stop))
        return true;
    }
    return false;
  }

  private bool Unholster(EntityUid user, EntityUid item, out bool stop)
  {
    stop = false;
    CMHolsterComponent comp;
    if (this.TryComp<CMHolsterComponent>(item, out comp))
    {
      TimeSpan? cooldown = comp.Cooldown;
      if (cooldown.HasValue)
      {
        TimeSpan valueOrDefault = cooldown.GetValueOrDefault();
        if (this._timing.CurTime < comp.LastEjectAt + valueOrDefault)
        {
          stop = true;
          this._popup.PopupPredicted(comp.CooldownPopup, user, new EntityUid?(user), PopupType.SmallCaution);
          return false;
        }
      }
      EntityUid entity;
      if (this.TryComp<StorageComponent>(item, out StorageComponent _) && this.TryGetLastInserted((Entity<CMHolsterComponent>) (item, comp), out entity))
      {
        if (!this._hands.TryPickup(user, entity))
          return false;
        comp.Contents.Remove(entity);
        this._audio.PlayPredicted(comp.EjectSound, item, new EntityUid?(user));
        stop = true;
        return true;
      }
      if (this.PickupSlot(user, item, comp.Whitelist))
      {
        ISharedAdminLogManager adminLog = this._adminLog;
        LogStringHandler logStringHandler = new LogStringHandler(13, 2);
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
        logStringHandler.AppendLiteral(" unholstered ");
        logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) item), "ToPrettyString(item)");
        ref LogStringHandler local = ref logStringHandler;
        adminLog.Add(LogType.RMCHolster, ref local);
        return true;
      }
    }
    IsUnholsterableEvent args = new IsUnholsterableEvent();
    this.RaiseLocalEvent<IsUnholsterableEvent>(item, ref args);
    if (!args.Unholsterable)
      return false;
    ISharedAdminLogManager adminLog1 = this._adminLog;
    LogStringHandler logStringHandler1 = new LogStringHandler(13, 2);
    logStringHandler1.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user), "ToPrettyString(user)");
    logStringHandler1.AppendLiteral(" unholstered ");
    logStringHandler1.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) item), "ToPrettyString(item)");
    ref LogStringHandler local1 = ref logStringHandler1;
    adminLog1.Add(LogType.RMCHolster, ref local1);
    return this._hands.TryPickup(user, item);
  }

  public bool TryEquipClothing(
    EntityUid user,
    Entity<ClothingComponent> clothing,
    bool doRangeCheck = true)
  {
    foreach (SlotFlags slotFlags in this._quickEquipOrder)
    {
      InventorySystem.InventorySlotEnumerator containerSlotEnumerator;
      if ((clothing.Comp.Slots & slotFlags) != SlotFlags.NONE && this._inventory.TryGetContainerSlotEnumerator((Entity<InventoryComponent>) user, out containerSlotEnumerator, clothing.Comp.Slots))
      {
        ContainerSlot container;
        while (containerSlotEnumerator.MoveNext(out container))
        {
          if (this._inventory.TryEquip(user, (EntityUid) clothing, container.ID, doRangeCheck: doRangeCheck))
            return true;
        }
      }
    }
    return false;
  }

  public (int Filled, int Total) GetItemSlotsFilled(Entity<ItemSlotsComponent?> slots)
  {
    if (!this.Resolve<ItemSlotsComponent>((EntityUid) slots, ref slots.Comp, false))
      return (0, 0);
    if (slots.Comp.Slots.Count == 0)
      return (0, 0);
    int num = 0;
    foreach ((string _, ItemSlot itemSlot) in slots.Comp.Slots)
    {
      EntityUid? containedEntity = (EntityUid?) itemSlot.ContainerSlot?.ContainedEntity;
      if (containedEntity.HasValue && !this.TerminatingOrDeleted(containedEntity.GetValueOrDefault()))
        ++num;
    }
    return (num, slots.Comp.Slots.Count);
  }

  public bool TryGetUserHoldingOrStoringItem(EntityUid item, out EntityUid user)
  {
    user = new EntityUid();
    BaseContainer container;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (item, (TransformComponent) null), out container))
      return false;
    if (IsUser(this, container.Owner))
    {
      user = container.Owner;
      return true;
    }
    StorageComponent comp;
    if (!this.TryComp<StorageComponent>(container.Owner, out comp) || comp.Container != container || !this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (container.Owner, (TransformComponent) null), out container) || !IsUser(this, container.Owner))
      return false;
    user = container.Owner;
    return true;

    static bool IsUser(SharedCMInventorySystem system, EntityUid user)
    {
      return system.HasComp<InventoryComponent>(user) || system.HasComp<HandsComponent>(user);
    }
  }

  private readonly record struct HolsterSlot(
    int Priority,
    bool IsHolster,
    ContainerSlot? Slot,
    EntityUid Ent,
    ItemSlot? ItemSlot) : IComparable<SharedCMInventorySystem.HolsterSlot>
  {
    public int CompareTo(SharedCMInventorySystem.HolsterSlot other)
    {
      if (this.IsHolster && other.IsHolster)
        return this.Priority.CompareTo(other.Priority);
      return this.IsHolster ? -1 : 1;
    }
  }
}
