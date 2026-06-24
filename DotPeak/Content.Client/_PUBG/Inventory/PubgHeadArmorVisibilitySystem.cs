// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Inventory.PubgHeadArmorVisibilitySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Clothing;
using Content.Client.Inventory;
using Content.Shared._PUBG.Armor;
using Content.Shared.Armor;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.Inventory;

public sealed class PubgHeadArmorVisibilitySystem : EntitySystem
{
  private const string HeadSlot = "head";
  private const string HeadArmorSlot = "headArmor";
  private const string OuterClothingSlot = "outerClothing";
  private const string OuterArmorSlot = "outerArmor";
  private const string OuterArmorLayerAnchor = "enum.SquadArmorLayers.Armor";
  private const string OuterArmorFallbackAnchor = "pubg-outer-armor-anchor";
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private ClientClothingSystem _clothing;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InventoryComponent, DidEquipEvent>(new ComponentEventHandler<InventoryComponent, DidEquipEvent>((object) this, __methodptr(OnDidEquip)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InventoryComponent, DidUnequipEvent>(new ComponentEventHandler<InventoryComponent, DidUnequipEvent>((object) this, __methodptr(OnDidUnequip)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<InventoryComponent, InventoryTemplateUpdated>(new ComponentEventRefHandler<InventoryComponent, InventoryTemplateUpdated>((object) this, __methodptr(OnInventoryTemplateUpdated)), (Type[]) null, new Type[1]
    {
      typeof (ClientClothingSystem)
    });
  }

  private void OnDidEquip(EntityUid uid, InventoryComponent component, DidEquipEvent args)
  {
    if (!this.IsTrackedSlot(args.Slot))
      return;
    this.UpdateLayerVisibility(uid, component);
  }

  private void OnDidUnequip(EntityUid uid, InventoryComponent component, DidUnequipEvent args)
  {
    if (!this.IsTrackedSlot(args.Slot))
      return;
    this.UpdateLayerVisibility(uid, component);
  }

  private void OnInventoryTemplateUpdated(
    EntityUid uid,
    InventoryComponent component,
    ref InventoryTemplateUpdated args)
  {
    this.UpdateLayerVisibility(uid, component);
  }

  private void UpdateLayerVisibility(EntityUid uid, InventoryComponent inventory)
  {
    SpriteComponent sprite;
    InventorySlotsComponent inventorySlots;
    if (!this.TryComp<SpriteComponent>(uid, ref sprite) || !this.TryComp<InventorySlotsComponent>(uid, ref inventorySlots))
      return;
    this.EnsureOuterArmorLayerAlias(uid, sprite, inventory);
    this._clothing.InitClothing(uid, inventory);
    if (!this.HasArmorHelmet(uid, inventory))
      return;
    this.HideHeadSlotLayers(uid, sprite, inventorySlots);
  }

  private bool HasArmorHelmet(EntityUid uid, InventoryComponent inventory)
  {
    EntityUid? entityUid;
    if (!this._inventory.TryGetSlotEntity(uid, "headArmor", out entityUid, inventory))
      return false;
    PubgArmorComponent pubgArmorComponent;
    ArmorComponent armorComponent;
    return this.TryComp<PubgArmorComponent>(entityUid.Value, ref pubgArmorComponent) || this.TryComp<ArmorComponent>(entityUid.Value, ref armorComponent);
  }

  private bool IsTrackedSlot(string slot)
  {
    return slot == "head" || slot == "headArmor" || slot == "outerClothing" || slot == "outerArmor";
  }

  private void EnsureOuterArmorLayerAlias(
    EntityUid uid,
    SpriteComponent sprite,
    InventoryComponent inventory)
  {
    int armorLayer;
    int num;
    if (!PubgHeadArmorVisibilitySystem.HasSlot(inventory, "outerArmor") || !this.TryGetOuterArmorAnchor(uid, sprite, out armorLayer) || this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), "outerArmor", ref num, false) && num == armorLayer)
      return;
    this._sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), "outerArmor", armorLayer);
  }

  private bool TryGetOuterArmorAnchor(EntityUid uid, SpriteComponent sprite, out int armorLayer)
  {
    if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), "enum.SquadArmorLayers.Armor", ref armorLayer, false) || this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), "pubg-outer-armor-anchor", ref armorLayer, false))
      return true;
    int num;
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), "outerClothing", ref num, false))
    {
      armorLayer = 0;
      return false;
    }
    armorLayer = num + 1;
    this._sprite.AddBlankLayer(Entity<SpriteComponent>.op_Implicit((uid, sprite)), new int?(armorLayer));
    this._sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), "pubg-outer-armor-anchor", armorLayer);
    return true;
  }

  private static bool HasSlot(InventoryComponent inventory, string slotName)
  {
    foreach (SlotDefinition slot in inventory.Slots)
    {
      if (slot.Name == slotName)
        return true;
    }
    return false;
  }

  private void HideHeadSlotLayers(
    EntityUid uid,
    SpriteComponent sprite,
    InventorySlotsComponent inventorySlots)
  {
    HashSet<string> stringSet;
    if (!inventorySlots.VisualLayerKeys.TryGetValue("head", out stringSet) || stringSet.Count == 0)
      return;
    foreach (string str in stringSet)
    {
      try
      {
        this._sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, sprite)), str, true);
      }
      catch
      {
      }
    }
    stringSet.Clear();
  }
}
