// Decompiled with JetBrains decompiler
// Type: Content.Shared.Containers.ItemSlots.ItemSlotsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Lock;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Containers.ItemSlots;

public sealed class ItemSlotsSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLogger;
  [Dependency]
  private ActionBlockerSystem _actionBlockerSystem;
  [Dependency]
  private SharedContainerSystem _containers;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedHandsSystem _handsSystem;
  [Dependency]
  private SharedAudioSystem _audioSystem;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    this.InitializeLock();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemSlotsComponent, MapInitEvent>(new ComponentEventHandler<ItemSlotsComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemSlotsComponent, ComponentInit>(new ComponentEventHandler<ItemSlotsComponent, ComponentInit>((object) this, __methodptr(Oninitialize)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemSlotsComponent, InteractUsingEvent>(new ComponentEventHandler<ItemSlotsComponent, InteractUsingEvent>((object) this, __methodptr(OnInteractUsing)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemSlotsComponent, InteractHandEvent>(new ComponentEventHandler<ItemSlotsComponent, InteractHandEvent>((object) this, __methodptr(OnInteractHand)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemSlotsComponent, UseInHandEvent>(new ComponentEventHandler<ItemSlotsComponent, UseInHandEvent>((object) this, __methodptr(OnUseInHand)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemSlotsComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<ItemSlotsComponent, GetVerbsEvent<AlternativeVerb>>((object) this, __methodptr(AddAlternativeVerbs)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemSlotsComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<ItemSlotsComponent, GetVerbsEvent<InteractionVerb>>((object) this, __methodptr(AddInteractionVerbsVerbs)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemSlotsComponent, BreakageEventArgs>(new ComponentEventHandler<ItemSlotsComponent, BreakageEventArgs>((object) this, __methodptr(OnBreak)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemSlotsComponent, DestructionEventArgs>(new ComponentEventHandler<ItemSlotsComponent, DestructionEventArgs>((object) this, __methodptr(OnBreak)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemSlotsComponent, ComponentGetState>(new ComponentEventRefHandler<ItemSlotsComponent, ComponentGetState>((object) this, __methodptr(GetItemSlotsState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemSlotsComponent, ComponentHandleState>(new ComponentEventRefHandler<ItemSlotsComponent, ComponentHandleState>((object) this, __methodptr(HandleItemSlotsState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemSlotsComponent, ItemSlotButtonPressedEvent>(new ComponentEventHandler<ItemSlotsComponent, ItemSlotButtonPressedEvent>((object) this, __methodptr(HandleButtonPressed)), (Type[]) null, (Type[]) null);
  }

  private void OnMapInit(EntityUid uid, ItemSlotsComponent itemSlots, MapInitEvent args)
  {
    foreach (ItemSlot itemSlot in itemSlots.Slots.Values)
    {
      if (!itemSlot.HasItem && !string.IsNullOrEmpty(itemSlot.StartingItem))
      {
        EntityUid entityUid = this.Spawn(itemSlot.StartingItem, this.Transform(uid).Coordinates);
        if (itemSlot.ContainerSlot != null)
          this._containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(entityUid), (BaseContainer) itemSlot.ContainerSlot, (TransformComponent) null, false);
      }
    }
  }

  private void Oninitialize(EntityUid uid, ItemSlotsComponent itemSlots, ComponentInit args)
  {
    foreach ((string key, ItemSlot itemSlot) in itemSlots.Slots)
      itemSlot.ContainerSlot = this._containers.EnsureContainer<ContainerSlot>(uid, key, (ContainerManagerComponent) null);
  }

  public void AddItemSlot(EntityUid uid, string id, ItemSlot slot, ItemSlotsComponent? itemSlots = null)
  {
    if (itemSlots == null)
      itemSlots = this.EnsureComp<ItemSlotsComponent>(uid);
    ItemSlot other;
    if (itemSlots.Slots.TryGetValue(id, out other))
    {
      if (other.Local)
        this.Log.Error($"Duplicate item slot key. Entity: {this.Comp<MetaDataComponent>(uid).EntityName} ({uid}), key: {id}");
      else
        slot.CopyFrom(other);
    }
    slot.ContainerSlot = this._containers.EnsureContainer<ContainerSlot>(uid, id, (ContainerManagerComponent) null);
    itemSlots.Slots[id] = slot;
    this.Dirty(uid, (IComponent) itemSlots, (MetaDataComponent) null);
  }

  public void RemoveItemSlot(EntityUid uid, ItemSlot slot, ItemSlotsComponent? itemSlots = null)
  {
    if (this.Terminating(uid, (MetaDataComponent) null) || slot.ContainerSlot == null)
      return;
    this._containers.ShutdownContainer((BaseContainer) slot.ContainerSlot);
    if (!this.Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
      return;
    itemSlots.Slots.Remove(((BaseContainer) slot.ContainerSlot).ID);
    if (itemSlots.Slots.Count == 0)
      this.RemComp(uid, (IComponent) itemSlots);
    else
      this.Dirty(uid, (IComponent) itemSlots, (MetaDataComponent) null);
  }

  public bool TryGetSlot(
    EntityUid uid,
    string slotId,
    [NotNullWhen(true)] out ItemSlot? itemSlot,
    ItemSlotsComponent? component = null)
  {
    itemSlot = (ItemSlot) null;
    return this.Resolve<ItemSlotsComponent>(uid, ref component, true) && component.Slots.TryGetValue(slotId, out itemSlot);
  }

  public void SetEjectFlags(
    EntityUid uid,
    ItemSlot slot,
    bool disableEject,
    bool ejectOnInteract = false,
    bool ejectOnUse = false,
    ItemSlotsComponent? itemSlots = null)
  {
    if (!this.Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
      return;
    slot.DisableEject = disableEject;
    slot.EjectOnInteract = ejectOnInteract;
    slot.EjectOnUse = ejectOnUse;
    this.Dirty(uid, (IComponent) itemSlots, (MetaDataComponent) null);
  }

  private void OnInteractHand(EntityUid uid, ItemSlotsComponent itemSlots, InteractHandEvent args)
  {
    if (args.Handled)
      return;
    foreach (ItemSlot slot in itemSlots.Slots.Values)
    {
      if (slot.EjectOnInteract && slot.Item.HasValue && this.CanEject(uid, new EntityUid?(args.User), slot, new EntityUid?(args.User)))
      {
        args.Handled = true;
        this.TryEjectToHands(uid, slot, new EntityUid?(args.User), true);
        break;
      }
    }
  }

  private void OnUseInHand(EntityUid uid, ItemSlotsComponent itemSlots, UseInHandEvent args)
  {
    if (args.Handled)
      return;
    foreach (ItemSlot slot in itemSlots.Slots.Values)
    {
      if (slot.EjectOnUse && slot.Item.HasValue && this.CanEject(uid, new EntityUid?(args.User), slot, new EntityUid?(args.User)))
      {
        args.Handled = true;
        this.TryEjectToHands(uid, slot, new EntityUid?(args.User), true);
        break;
      }
    }
  }

  private void OnInteractUsing(
    EntityUid uid,
    ItemSlotsComponent itemSlots,
    InteractUsingEvent args)
  {
    HandsComponent handsComp;
    if (args.Handled || !this.TryComp<HandsComponent>(args.User, ref handsComp) || itemSlots.Slots.Count == 0)
      return;
    List<ItemSlot> itemSlotList = new List<ItemSlot>();
    string str1 = (string) null;
    string str2 = (string) null;
    foreach (ItemSlot slot in itemSlots.Slots.Values)
    {
      if (slot.InsertOnInteract)
      {
        if (this.CanInsert(uid, args.Used, new EntityUid?(args.User), slot, slot.Swap))
        {
          itemSlotList.Add(slot);
        }
        else
        {
          bool flag = this.CanInsertWhitelist(args.Used, slot);
          LocId? nullable;
          if (((str2 != null ? 0 : (slot.LockedFailPopup.HasValue ? 1 : 0)) & (flag ? 1 : 0)) != 0 && slot.Locked)
          {
            nullable = slot.LockedFailPopup;
            str2 = nullable.HasValue ? LocId.op_Implicit(nullable.GetValueOrDefault()) : (string) null;
          }
          if (str1 == null && slot.WhitelistFailPopup.HasValue)
          {
            nullable = slot.WhitelistFailPopup;
            str1 = nullable.HasValue ? LocId.op_Implicit(nullable.GetValueOrDefault()) : (string) null;
          }
        }
      }
    }
    if (itemSlotList.Count == 0)
    {
      if (str2 != null)
      {
        this._popupSystem.PopupClient(this.Loc.GetString(str2), uid, new EntityUid?(args.User));
      }
      else
      {
        if (str1 == null)
          return;
        this._popupSystem.PopupClient(this.Loc.GetString(str1), uid, new EntityUid?(args.User));
      }
    }
    else
    {
      if (!this._handsSystem.TryDrop(Entity<HandsComponent>.op_Implicit(args.User), args.Used))
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      itemSlotList.Sort(ItemSlotsSystem.\u003C\u003EO.\u003C0\u003E__SortEmpty ?? (ItemSlotsSystem.\u003C\u003EO.\u003C0\u003E__SortEmpty = new Comparison<ItemSlot>(ItemSlotsSystem.SortEmpty)));
      using (List<ItemSlot>.Enumerator enumerator = itemSlotList.GetEnumerator())
      {
        if (!enumerator.MoveNext())
          return;
        ItemSlot current = enumerator.Current;
        if (current.Item.HasValue)
          this._handsSystem.TryPickupAnyHand(args.User, current.Item.Value, handsComp: handsComp);
        this.Insert(uid, current, args.Used, new EntityUid?(args.User), true);
        if (current.InsertSuccessPopup.HasValue)
        {
          SharedPopupSystem popupSystem = this._popupSystem;
          ILocalizationManager loc = this.Loc;
          LocId? insertSuccessPopup = current.InsertSuccessPopup;
          string str3 = insertSuccessPopup.HasValue ? LocId.op_Implicit(insertSuccessPopup.GetValueOrDefault()) : (string) null;
          string message = loc.GetString(str3);
          EntityUid uid1 = uid;
          EntityUid? recipient = new EntityUid?(args.User);
          popupSystem.PopupClient(message, uid1, recipient);
        }
        args.Handled = true;
      }
    }
  }

  private void Insert(
    EntityUid uid,
    ItemSlot slot,
    EntityUid item,
    EntityUid? user,
    bool excludeUserAudio = false)
  {
    bool? nullable = slot.ContainerSlot != null ? new bool?(this._containers.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(item), (BaseContainer) slot.ContainerSlot, (TransformComponent) null, false)) : new bool?();
    if (nullable.HasValue && nullable.Value && user.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(16 /*0x10*/, 4);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user.Value)), "ToPrettyString(user.Value)");
      logStringHandler.AppendLiteral(" inserted ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(item)), "ToPrettyString(item)");
      logStringHandler.AppendLiteral(" into ");
      logStringHandler.AppendFormatted(((BaseContainer) slot.ContainerSlot)?.ID + " slot of ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
    }
    this._audioSystem.PlayPredicted(slot.InsertSound, uid, excludeUserAudio ? user : new EntityUid?(), new AudioParams?());
  }

  public bool CanInsert(
    EntityUid uid,
    EntityUid usedUid,
    EntityUid? user,
    ItemSlot slot,
    bool swap = false)
  {
    if (slot.ContainerSlot == null || slot.HasItem && (!swap || swap && !this.CanEject(uid, user, slot)) || !this.CanInsertWhitelist(usedUid, slot) || slot.Locked)
      return false;
    ItemSlotInsertAttemptEvent insertAttemptEvent = new ItemSlotInsertAttemptEvent(uid, usedUid, user, slot);
    this.RaiseLocalEvent<ItemSlotInsertAttemptEvent>(uid, ref insertAttemptEvent, false);
    this.RaiseLocalEvent<ItemSlotInsertAttemptEvent>(usedUid, ref insertAttemptEvent, false);
    return !insertAttemptEvent.Cancelled && this._containers.CanInsert(usedUid, (BaseContainer) slot.ContainerSlot, swap, (TransformComponent) null);
  }

  private bool CanInsertWhitelist(EntityUid usedUid, ItemSlot slot)
  {
    return !this._whitelistSystem.IsWhitelistFail(slot.Whitelist, usedUid) && !this._whitelistSystem.IsBlacklistPass(slot.Blacklist, usedUid);
  }

  public bool TryInsert(
    EntityUid uid,
    string id,
    EntityUid item,
    EntityUid? user,
    ItemSlotsComponent? itemSlots = null,
    bool excludeUserAudio = false)
  {
    ItemSlot slot;
    return this.Resolve<ItemSlotsComponent>(uid, ref itemSlots, true) && itemSlots.Slots.TryGetValue(id, out slot) && this.TryInsert(uid, slot, item, user, excludeUserAudio);
  }

  public bool TryInsert(
    EntityUid uid,
    ItemSlot slot,
    EntityUid item,
    EntityUid? user,
    bool excludeUserAudio = false)
  {
    if (!this.CanInsert(uid, item, user, slot))
      return false;
    this.Insert(uid, slot, item, user, excludeUserAudio);
    return true;
  }

  public bool TryInsertFromHand(
    EntityUid uid,
    ItemSlot slot,
    EntityUid user,
    HandsComponent? hands = null,
    bool excludeUserAudio = false)
  {
    EntityUid? nullable;
    if (!this.Resolve<HandsComponent>(user, ref hands, false) || !this._handsSystem.TryGetActiveItem(Entity<HandsComponent>.op_Implicit((user, hands)), out nullable) || !this.CanInsert(uid, nullable.Value, new EntityUid?(user), slot) || !this._handsSystem.TryDrop(Entity<HandsComponent>.op_Implicit(user), hands.ActiveHandId))
      return false;
    this.Insert(uid, slot, nullable.Value, new EntityUid?(user), excludeUserAudio);
    return true;
  }

  public bool TryInsertEmpty(
    Entity<ItemSlotsComponent?> ent,
    EntityUid item,
    EntityUid? user,
    bool excludeUserAudio = false)
  {
    if (!this.Resolve<ItemSlotsComponent>(Entity<ItemSlotsComponent>.op_Implicit(ent), ref ent.Comp, false))
      return false;
    Entity<ItemSlotsComponent> ent1 = ent;
    EntityUid entityUid = item;
    EntityUid? nullable = user;
    Entity<HandsComponent>? userEnt = nullable.HasValue ? new Entity<HandsComponent>?(Entity<HandsComponent>.op_Implicit(nullable.GetValueOrDefault())) : new Entity<HandsComponent>?();
    ItemSlot slot;
    ref ItemSlot local = ref slot;
    if (!this.TryGetAvailableSlot(ent1, entityUid, userEnt, out local, true) || user.HasValue && !this._handsSystem.TryDrop(Entity<HandsComponent>.op_Implicit(user.Value), item))
      return false;
    this.Insert(Entity<ItemSlotsComponent>.op_Implicit(ent), slot, item, user, excludeUserAudio);
    return true;
  }

  public bool TryGetAvailableSlot(
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
      if (this.Resolve<HandsComponent>(Entity<HandsComponent>.op_Implicit(valueOrDefault), ref valueOrDefault.Comp, true) && this._handsSystem.IsHolding(valueOrDefault, new EntityUid?(item)) && !this._handsSystem.CanDrop(valueOrDefault, item))
        return false;
    }
    if (!this.Resolve<ItemSlotsComponent>(Entity<ItemSlotsComponent>.op_Implicit(ent), ref ent.Comp, false))
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
      EntityUid uid = Entity<ItemSlotsComponent>.op_Implicit(ent);
      EntityUid usedUid = item;
      Entity<HandsComponent>? nullable2 = userEnt;
      EntityUid? user;
      if (!nullable2.HasValue)
      {
        nullable1 = new EntityUid?();
        user = nullable1;
      }
      else
        user = new EntityUid?(Entity<HandsComponent>.op_Implicit(nullable2.GetValueOrDefault()));
      ItemSlot slot = itemSlot1;
      if (this.CanInsert(uid, usedUid, user, slot))
        itemSlotList.Add(itemSlot1);
    }
    if (itemSlotList.Count == 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    itemSlotList.Sort(ItemSlotsSystem.\u003C\u003EO.\u003C0\u003E__SortEmpty ?? (ItemSlotsSystem.\u003C\u003EO.\u003C0\u003E__SortEmpty = new Comparison<ItemSlot>(ItemSlotsSystem.SortEmpty)));
    itemSlot = itemSlotList[0];
    return true;
  }

  public static int SortEmpty(ItemSlot a, ItemSlot b)
  {
    EntityUid? containedEntity1 = (EntityUid?) a.ContainerSlot?.ContainedEntity;
    EntityUid? containedEntity2 = (EntityUid?) b.ContainerSlot?.ContainedEntity;
    if (!containedEntity1.HasValue && !containedEntity2.HasValue)
      return a.Priority.CompareTo(b.Priority);
    return !containedEntity1.HasValue ? -1 : 1;
  }

  public bool CanEject(EntityUid uid, EntityUid? user, ItemSlot slot, EntityUid? popup = null)
  {
    if (slot.Locked)
    {
      if (popup.HasValue && slot.LockedFailPopup.HasValue)
      {
        SharedPopupSystem popupSystem = this._popupSystem;
        ILocalizationManager loc = this.Loc;
        LocId? lockedFailPopup = slot.LockedFailPopup;
        string str = lockedFailPopup.HasValue ? LocId.op_Implicit(lockedFailPopup.GetValueOrDefault()) : (string) null;
        string message = loc.GetString(str);
        EntityUid uid1 = uid;
        EntityUid? recipient = new EntityUid?(popup.Value);
        popupSystem.PopupClient(message, uid1, recipient);
      }
      return false;
    }
    EntityUid? containedEntity = (EntityUid?) slot.ContainerSlot?.ContainedEntity;
    if (!containedEntity.HasValue)
      return false;
    EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
    ItemSlotEjectAttemptEvent ejectAttemptEvent = new ItemSlotEjectAttemptEvent(uid, valueOrDefault, user, slot);
    this.RaiseLocalEvent<ItemSlotEjectAttemptEvent>(uid, ref ejectAttemptEvent, false);
    this.RaiseLocalEvent<ItemSlotEjectAttemptEvent>(valueOrDefault, ref ejectAttemptEvent, false);
    return !ejectAttemptEvent.Cancelled && this._containers.CanRemove(valueOrDefault, (BaseContainer) slot.ContainerSlot);
  }

  private void Eject(
    EntityUid uid,
    ItemSlot slot,
    EntityUid item,
    EntityUid? user,
    bool excludeUserAudio = false)
  {
    bool? nullable = slot.ContainerSlot != null ? new bool?(this._containers.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(item), (BaseContainer) slot.ContainerSlot, true, false, new EntityCoordinates?(), new Angle?())) : new bool?();
    if (nullable.HasValue && nullable.Value && user.HasValue)
    {
      ISharedAdminLogManager adminLogger = this._adminLogger;
      LogStringHandler logStringHandler = new LogStringHandler(15, 4);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(user.Value)), "ToPrettyString(user.Value)");
      logStringHandler.AppendLiteral(" ejected ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(item)), "ToPrettyString(item)");
      logStringHandler.AppendLiteral(" from ");
      logStringHandler.AppendFormatted(((BaseContainer) slot.ContainerSlot)?.ID + " slot of ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
      ref LogStringHandler local = ref logStringHandler;
      adminLogger.Add(LogType.Action, LogImpact.Low, ref local);
    }
    this._audioSystem.PlayPredicted(slot.EjectSound, uid, excludeUserAudio ? user : new EntityUid?(), new AudioParams?());
  }

  public bool TryEject(
    EntityUid uid,
    ItemSlot slot,
    EntityUid? user,
    [NotNullWhen(true)] out EntityUid? item,
    bool excludeUserAudio = false)
  {
    item = new EntityUid?();
    if (!this.CanEject(uid, user, slot))
      return false;
    item = slot.Item;
    if (user.HasValue && item.HasValue && !this._actionBlockerSystem.CanPickup(user.Value, item.Value))
      return false;
    this.Eject(uid, slot, item.Value, user, excludeUserAudio);
    return true;
  }

  public bool TryEject(
    EntityUid uid,
    string id,
    EntityUid? user,
    [NotNullWhen(true)] out EntityUid? item,
    ItemSlotsComponent? itemSlots = null,
    bool excludeUserAudio = false)
  {
    item = new EntityUid?();
    ItemSlot slot;
    return this.Resolve<ItemSlotsComponent>(uid, ref itemSlots, true) && itemSlots.Slots.TryGetValue(id, out slot) && this.TryEject(uid, slot, user, out item, excludeUserAudio);
  }

  public bool TryEjectToHands(
    EntityUid uid,
    ItemSlot slot,
    EntityUid? user,
    bool excludeUserAudio = false)
  {
    EntityUid? nullable;
    if (!this.TryEject(uid, slot, user, out nullable, excludeUserAudio))
      return false;
    if (user.HasValue)
      this._handsSystem.PickupOrDrop(new EntityUid?(user.Value), nullable.Value);
    return true;
  }

  private void AddAlternativeVerbs(
    EntityUid uid,
    ItemSlotsComponent itemSlots,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (args.Hands == null || !args.CanAccess || !args.CanInteract)
      return;
    if (args.Using.HasValue && this._actionBlockerSystem.CanDrop(args.User))
    {
      bool flag = false;
      foreach (ItemSlot itemSlot in itemSlots.Slots.Values)
      {
        ItemSlot slot = itemSlot;
        if (!slot.InsertOnInteract && this.CanInsert(uid, args.Using.Value, new EntityUid?(args.User), slot))
        {
          string str = slot.Name != string.Empty ? this.Loc.GetString(slot.Name) : this.Name(args.Using.Value, (MetaDataComponent) null);
          AlternativeVerb alternativeVerb1 = new AlternativeVerb();
          alternativeVerb1.IconEntity = this.GetNetEntity(args.Using, (MetaDataComponent) null);
          alternativeVerb1.Act = (Action) (() => this.Insert(uid, slot, args.Using.Value, new EntityUid?(args.User), true));
          AlternativeVerb alternativeVerb2 = alternativeVerb1;
          if (slot.InsertVerbText != null)
          {
            alternativeVerb2.Text = this.Loc.GetString(slot.InsertVerbText);
            alternativeVerb2.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/insert.svg.192dpi.png"));
          }
          else if (slot.EjectOnInteract)
          {
            alternativeVerb2.Text = this.Loc.GetString("place-item-verb-text", ("subject", (object) str));
            alternativeVerb2.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/drop.svg.192dpi.png"));
          }
          else
          {
            alternativeVerb2.Category = VerbCategory.Insert;
            alternativeVerb2.Text = str;
          }
          alternativeVerb2.Priority = slot.Priority;
          args.Verbs.Add(alternativeVerb2);
          flag = true;
        }
      }
      if (flag)
        return;
    }
    foreach (ItemSlot itemSlot in itemSlots.Slots.Values)
    {
      ItemSlot slot = itemSlot;
      if (!slot.EjectOnInteract && !slot.DisableEject)
      {
        EntityUid uid1 = uid;
        EntityUid? user1 = new EntityUid?(args.User);
        ItemSlot slot1 = slot;
        EntityUid? nullable = new EntityUid?();
        EntityUid? popup = nullable;
        if (this.CanEject(uid1, user1, slot1, popup))
        {
          ActionBlockerSystem actionBlockerSystem = this._actionBlockerSystem;
          EntityUid user2 = args.User;
          nullable = slot.Item;
          EntityUid entityUid = nullable.Value;
          if (actionBlockerSystem.CanPickup(user2, entityUid))
          {
            string str1;
            if (!(slot.Name != string.Empty))
            {
              nullable = slot.Item;
              str1 = this.Comp<MetaDataComponent>(nullable.Value).EntityName ?? string.Empty;
            }
            else
              str1 = this.Loc.GetString(slot.Name);
            string str2 = str1;
            AlternativeVerb alternativeVerb3 = new AlternativeVerb();
            alternativeVerb3.IconEntity = this.GetNetEntity(slot.Item, (MetaDataComponent) null);
            alternativeVerb3.Act = (Action) (() => this.TryEjectToHands(uid, slot, new EntityUid?(args.User), true));
            AlternativeVerb alternativeVerb4 = alternativeVerb3;
            if (slot.EjectVerbText == null)
            {
              alternativeVerb4.Text = str2;
              alternativeVerb4.Category = VerbCategory.Eject;
            }
            else
              alternativeVerb4.Text = this.Loc.GetString(slot.EjectVerbText);
            alternativeVerb4.Priority = slot.Priority;
            args.Verbs.Add(alternativeVerb4);
          }
        }
      }
    }
  }

  private void AddInteractionVerbsVerbs(
    EntityUid uid,
    ItemSlotsComponent itemSlots,
    GetVerbsEvent<InteractionVerb> args)
  {
    if (args.Hands == null || !args.CanAccess || !args.CanInteract)
      return;
    foreach (ItemSlot itemSlot in itemSlots.Slots.Values)
    {
      ItemSlot slot = itemSlot;
      if (slot.EjectOnInteract)
      {
        EntityUid uid1 = uid;
        EntityUid? user1 = new EntityUid?(args.User);
        ItemSlot slot1 = slot;
        EntityUid? nullable = new EntityUid?();
        EntityUid? popup = nullable;
        if (this.CanEject(uid1, user1, slot1, popup))
        {
          ActionBlockerSystem actionBlockerSystem = this._actionBlockerSystem;
          EntityUid user2 = args.User;
          nullable = slot.Item;
          EntityUid entityUid = nullable.Value;
          if (actionBlockerSystem.CanPickup(user2, entityUid))
          {
            string str1;
            if (!(slot.Name != string.Empty))
            {
              nullable = slot.Item;
              str1 = this.Name(nullable.Value, (MetaDataComponent) null);
            }
            else
              str1 = this.Loc.GetString(slot.Name);
            string str2 = str1;
            InteractionVerb interactionVerb1 = new InteractionVerb();
            interactionVerb1.IconEntity = this.GetNetEntity(slot.Item, (MetaDataComponent) null);
            interactionVerb1.Act = (Action) (() => this.TryEjectToHands(uid, slot, new EntityUid?(args.User), true));
            InteractionVerb interactionVerb2 = interactionVerb1;
            if (slot.EjectVerbText == null)
              interactionVerb2.Text = this.Loc.GetString("take-item-verb-text", ("subject", (object) str2));
            else
              interactionVerb2.Text = this.Loc.GetString(slot.EjectVerbText);
            interactionVerb2.Priority = slot.Priority;
            args.Verbs.Add(interactionVerb2);
          }
        }
      }
    }
    if (!args.Using.HasValue || !this._actionBlockerSystem.CanDrop(args.User))
      return;
    foreach (ItemSlot itemSlot in itemSlots.Slots.Values)
    {
      ItemSlot slot = itemSlot;
      if (slot.InsertOnInteract && this.CanInsert(uid, args.Using.Value, new EntityUid?(args.User), slot))
      {
        string str = slot.Name != string.Empty ? this.Loc.GetString(slot.Name) : this.Name(args.Using.Value, (MetaDataComponent) null);
        InteractionVerb interactionVerb3 = new InteractionVerb();
        interactionVerb3.IconEntity = this.GetNetEntity(args.Using, (MetaDataComponent) null);
        interactionVerb3.Act = (Action) (() => this.Insert(uid, slot, args.Using.Value, new EntityUid?(args.User), true));
        InteractionVerb interactionVerb4 = interactionVerb3;
        if (slot.InsertVerbText != null)
        {
          interactionVerb4.Text = this.Loc.GetString(slot.InsertVerbText);
          interactionVerb4.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/insert.svg.192dpi.png"));
        }
        else if (slot.EjectOnInteract)
        {
          interactionVerb4.Text = this.Loc.GetString("place-item-verb-text", ("subject", (object) str));
          interactionVerb4.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/drop.svg.192dpi.png"));
        }
        else
        {
          interactionVerb4.Category = VerbCategory.Insert;
          interactionVerb4.Text = str;
        }
        interactionVerb4.Priority = slot.Priority;
        args.Verbs.Add(interactionVerb4);
      }
    }
  }

  private void HandleButtonPressed(
    EntityUid uid,
    ItemSlotsComponent component,
    ItemSlotButtonPressedEvent args)
  {
    ItemSlot slot;
    if (!component.Slots.TryGetValue(args.SlotId, out slot))
      return;
    if (args.TryEject && slot.HasItem)
    {
      this.TryEjectToHands(uid, slot, new EntityUid?(((BaseBoundUserInterfaceEvent) args).Actor), true);
    }
    else
    {
      if (!args.TryInsert || slot.HasItem)
        return;
      this.TryInsertFromHand(uid, slot, ((BaseBoundUserInterfaceEvent) args).Actor);
    }
  }

  private void OnBreak(EntityUid uid, ItemSlotsComponent component, EntityEventArgs args)
  {
    foreach (ItemSlot slot in component.Slots.Values)
    {
      if (slot.EjectOnBreak && slot.HasItem)
      {
        this.SetLock(uid, slot, false, component);
        this.TryEject(uid, slot, new EntityUid?(), out EntityUid? _);
      }
    }
  }

  public EntityUid? GetItemOrNull(EntityUid uid, string id, ItemSlotsComponent? itemSlots = null)
  {
    if (!this.Resolve<ItemSlotsComponent>(uid, ref itemSlots, false))
      return new EntityUid?();
    return itemSlots.Slots.GetValueOrDefault<string, ItemSlot>(id)?.Item;
  }

  public void SetLock(EntityUid uid, string id, bool locked, ItemSlotsComponent? itemSlots = null)
  {
    ItemSlot slot;
    if (!this.Resolve<ItemSlotsComponent>(uid, ref itemSlots, true) || !itemSlots.Slots.TryGetValue(id, out slot))
      return;
    this.SetLock(uid, slot, locked, itemSlots);
  }

  public void SetLock(EntityUid uid, ItemSlot slot, bool locked, ItemSlotsComponent? itemSlots = null)
  {
    if (!this.Resolve<ItemSlotsComponent>(uid, ref itemSlots, true))
      return;
    slot.Locked = locked;
    this.Dirty(uid, (IComponent) itemSlots, (MetaDataComponent) null);
  }

  private void HandleItemSlotsState(
    EntityUid uid,
    ItemSlotsComponent component,
    ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is ItemSlotsComponentState current))
      return;
    foreach ((string key, ItemSlot itemSlot1) in component.Slots)
    {
      string key2 = key;
      ItemSlot slot = itemSlot1;
      if (!current.Slots.ContainsKey(key2))
        this.RemoveItemSlot(uid, slot, component);
    }
    foreach ((key, itemSlot1) in current.Slots)
    {
      string str = key;
      ItemSlot other = itemSlot1;
      ItemSlot itemSlot2;
      if (component.Slots.TryGetValue(str, out itemSlot2))
      {
        itemSlot2.CopyFrom(other);
        itemSlot2.ContainerSlot = this._containers.EnsureContainer<ContainerSlot>(uid, str, (ContainerManagerComponent) null);
      }
      else
        this.AddItemSlot(uid, str, new ItemSlot(other)
        {
          Local = false
        });
    }
  }

  private void GetItemSlotsState(
    EntityUid uid,
    ItemSlotsComponent component,
    ref ComponentGetState args)
  {
    ((ComponentGetState) ref args).State = (IComponentState) new ItemSlotsComponentState(component.Slots);
  }

  private void InitializeLock()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemSlotsLockComponent, MapInitEvent>(new EntityEventRefHandler<ItemSlotsLockComponent, MapInitEvent>((object) this, __methodptr(OnLockMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ItemSlotsLockComponent, LockToggledEvent>(new EntityEventRefHandler<ItemSlotsLockComponent, LockToggledEvent>((object) this, __methodptr(OnLockToggled)), (Type[]) null, (Type[]) null);
  }

  private void OnLockMapInit(Entity<ItemSlotsLockComponent> ent, ref MapInitEvent args)
  {
    LockComponent lockComponent;
    if (!this.TryComp<LockComponent>(ent.Owner, ref lockComponent))
      return;
    this.UpdateLocks(ent, lockComponent.Locked);
  }

  private void OnLockToggled(Entity<ItemSlotsLockComponent> ent, ref LockToggledEvent args)
  {
    this.UpdateLocks(ent, args.Locked);
  }

  private void UpdateLocks(Entity<ItemSlotsLockComponent> ent, bool value)
  {
    foreach (string slot in ent.Comp.Slots)
    {
      ItemSlot itemSlot;
      if (this.TryGetSlot(ent.Owner, slot, out itemSlot))
        this.SetLock(ent.Owner, itemSlot, value);
    }
  }
}
