// Decompiled with JetBrains decompiler
// Type: Content.Shared.Interaction.SmartEquipSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Input;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Interaction;

public sealed class SmartEquipSystem : EntitySystem
{
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedStorageSystem _storage;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private ItemSlotsSystem _slots;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;

  public override void Initialize()
  {
    CommandBinds.Builder.Bind(ContentKeyFunctions.SmartEquipBackpack, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.HandleSmartEquipBackpack), handle: false, outsidePrediction: false)).Bind(ContentKeyFunctions.SmartEquipBelt, InputCmdHandler.FromDelegate(new StateInputCmdDelegate(this.HandleSmartEquipBelt), handle: false, outsidePrediction: false)).Register<SmartEquipSystem>();
  }

  public override void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<SmartEquipSystem>();
  }

  private void HandleSmartEquipBackpack(ICommonSession? session)
  {
    this.HandleSmartEquip(session, "back");
  }

  private void HandleSmartEquipBelt(ICommonSession? session)
  {
    this.HandleSmartEquip(session, "belt");
  }

  private void HandleSmartEquip(ICommonSession? session, string equipmentSlot)
  {
    if (session == null)
      return;
    EntityUid? nullable = session.AttachedEntity;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault1 = nullable.GetValueOrDefault();
    HandsComponent comp1;
    if (!valueOrDefault1.Valid || !this.Exists(valueOrDefault1) || !this.TryComp<HandsComponent>(valueOrDefault1, out comp1) || comp1.ActiveHandId == null)
      return;
    EntityUid? activeItem = this._hands.GetActiveItem((Entity<HandsComponent>) (valueOrDefault1, comp1));
    if (!this._actionBlocker.CanInteract(valueOrDefault1, activeItem))
      return;
    InventoryComponent comp2;
    if (!this.TryComp<InventoryComponent>(valueOrDefault1, out comp2) || !this._inventory.HasSlot(valueOrDefault1, equipmentSlot, comp2))
      this._popup.PopupClient(this.Loc.GetString("smart-equip-missing-equipment-slot", ("slotName", (object) equipmentSlot)), valueOrDefault1, new EntityUid?(valueOrDefault1));
    else if (activeItem.HasValue && !this._hands.CanDropHeld(valueOrDefault1, comp1.ActiveHandId))
    {
      this._popup.PopupClient(this.Loc.GetString("smart-equip-cant-drop"), valueOrDefault1, new EntityUid?(valueOrDefault1));
    }
    else
    {
      EntityUid? entityUid1;
      this._inventory.TryGetSlotEntity(valueOrDefault1, equipmentSlot, out entityUid1);
      string message = this.Loc.GetString("smart-equip-empty-equipment-slot", ("slotName", (object) equipmentSlot));
      if (entityUid1.HasValue)
      {
        EntityUid valueOrDefault2 = entityUid1.GetValueOrDefault();
        StorageComponent comp3;
        if (this.TryComp<StorageComponent>(valueOrDefault2, out comp3))
        {
          nullable = activeItem;
          if (!nullable.HasValue)
          {
            if (comp3.Container.ContainedEntities.Count == 0)
            {
              this._popup.PopupClient(message, valueOrDefault1, new EntityUid?(valueOrDefault1));
            }
            else
            {
              IReadOnlyList<EntityUid> containedEntities = comp3.Container.ContainedEntities;
              EntityUid entityUid2 = containedEntities[containedEntities.Count - 1];
              this._container.RemoveEntity(valueOrDefault2, entityUid2);
              this._hands.TryPickup(valueOrDefault1, entityUid2, handsComp: comp1);
            }
          }
          else
          {
            string reason1;
            if (!this._storage.CanInsert(valueOrDefault2, activeItem.Value, session.AttachedEntity, out reason1))
            {
              if (reason1 == null)
                return;
              this._popup.PopupClient(this.Loc.GetString(reason1), valueOrDefault1, new EntityUid?(valueOrDefault1));
            }
            else
            {
              this._hands.TryDrop((Entity<HandsComponent>) (valueOrDefault1, comp1), comp1.ActiveHandId);
              EntityUid? stackedEntity;
              string reason2;
              this._storage.Insert(valueOrDefault2, activeItem.Value, out stackedEntity, out reason2, new EntityUid?(valueOrDefault1));
              StackComponent comp4;
              if (!stackedEntity.HasValue || this._storage.CanInsert(valueOrDefault2, activeItem.Value, session.AttachedEntity, out reason2) || !this.TryComp<StackComponent>(activeItem.Value, out comp4) || comp4.Count <= 0)
                return;
              this._hands.TryPickup(valueOrDefault1, activeItem.Value, handsComp: comp1);
            }
          }
        }
        else
        {
          ItemSlotsComponent comp5;
          if (this.TryComp<ItemSlotsComponent>(valueOrDefault2, out comp5))
          {
            if (!activeItem.HasValue)
            {
              ItemSlot slot = (ItemSlot) null;
              foreach (ItemSlot itemSlot in comp5.Slots.Values)
              {
                if (itemSlot.HasItem && itemSlot.Priority > (slot != null ? slot.Priority : int.MinValue))
                  slot = itemSlot;
              }
              if (slot == null)
                this._popup.PopupClient(message, valueOrDefault1, new EntityUid?(valueOrDefault1));
              else
                this._slots.TryEjectToHands(valueOrDefault2, slot, new EntityUid?(valueOrDefault1), true);
            }
            else
            {
              ItemSlot slot = (ItemSlot) null;
              foreach (ItemSlot itemSlot in comp5.Slots.Values)
              {
                if (!itemSlot.HasItem && this._whitelistSystem.IsWhitelistPassOrNull(itemSlot.Whitelist, activeItem.Value) && itemSlot.Priority > (slot != null ? slot.Priority : int.MinValue))
                  slot = itemSlot;
              }
              if (slot == null)
                this._popup.PopupClient(this.Loc.GetString("smart-equip-no-valid-item-slot-insert", ("item", (object) activeItem.Value)), valueOrDefault1, new EntityUid?(valueOrDefault1));
              else
                this._slots.TryInsertFromHand(valueOrDefault2, slot, valueOrDefault1, comp1, true);
            }
          }
          else
          {
            if (activeItem.HasValue)
              return;
            string reason;
            if (!this._inventory.CanUnequip(valueOrDefault1, equipmentSlot, out reason))
            {
              this._popup.PopupClient(this.Loc.GetString(reason), valueOrDefault1, new EntityUid?(valueOrDefault1));
            }
            else
            {
              this._inventory.TryUnequip(valueOrDefault1, equipmentSlot, predicted: true, inventory: comp2, checkDoafter: true);
              this._hands.TryPickup(valueOrDefault1, valueOrDefault2, handsComp: comp1);
            }
          }
        }
      }
      else if (!activeItem.HasValue)
      {
        this._popup.PopupClient(message, valueOrDefault1, new EntityUid?(valueOrDefault1));
      }
      else
      {
        string reason;
        if (!this._inventory.CanEquip(valueOrDefault1, activeItem.Value, equipmentSlot, out reason))
        {
          this._popup.PopupClient(this.Loc.GetString(reason), valueOrDefault1, new EntityUid?(valueOrDefault1));
        }
        else
        {
          this._hands.TryDrop((Entity<HandsComponent>) (valueOrDefault1, comp1), comp1.ActiveHandId);
          this._inventory.TryEquip(valueOrDefault1, activeItem.Value, equipmentSlot, predicted: true, checkDoafter: true);
        }
      }
    }
  }
}
