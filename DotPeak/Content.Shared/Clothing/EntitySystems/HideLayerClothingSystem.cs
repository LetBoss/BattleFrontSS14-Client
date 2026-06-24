// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.EntitySystems.HideLayerClothingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.Components;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Clothing.EntitySystems;

public sealed class HideLayerClothingSystem : EntitySystem
{
  [Dependency]
  private SharedHumanoidAppearanceSystem _humanoid;
  [Dependency]
  private IGameTiming _timing;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HideLayerClothingComponent, ClothingGotUnequippedEvent>(new EntityEventRefHandler<HideLayerClothingComponent, ClothingGotUnequippedEvent>((object) this, __methodptr(OnHideGotUnequipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HideLayerClothingComponent, ClothingGotEquippedEvent>(new EntityEventRefHandler<HideLayerClothingComponent, ClothingGotEquippedEvent>((object) this, __methodptr(OnHideGotEquipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HideLayerClothingComponent, ItemMaskToggledEvent>(new EntityEventRefHandler<HideLayerClothingComponent, ItemMaskToggledEvent>((object) this, __methodptr(OnHideToggled)), (Type[]) null, (Type[]) null);
  }

  private void OnHideToggled(Entity<HideLayerClothingComponent> ent, ref ItemMaskToggledEvent args)
  {
    if (!args.Wearer.HasValue)
      return;
    this.SetLayerVisibility(Entity<HideLayerClothingComponent, ClothingComponent>.op_Implicit(ent), Entity<HumanoidAppearanceComponent>.op_Implicit(args.Wearer.Value), true);
  }

  private void OnHideGotEquipped(
    Entity<HideLayerClothingComponent> ent,
    ref ClothingGotEquippedEvent args)
  {
    this.SetLayerVisibility(Entity<HideLayerClothingComponent, ClothingComponent>.op_Implicit(ent), Entity<HumanoidAppearanceComponent>.op_Implicit(args.Wearer), true);
  }

  private void OnHideGotUnequipped(
    Entity<HideLayerClothingComponent> ent,
    ref ClothingGotUnequippedEvent args)
  {
    this.SetLayerVisibility(Entity<HideLayerClothingComponent, ClothingComponent>.op_Implicit(ent), Entity<HumanoidAppearanceComponent>.op_Implicit(args.Wearer), false);
  }

  private void SetLayerVisibility(
    Entity<HideLayerClothingComponent?, ClothingComponent?> clothing,
    Entity<HumanoidAppearanceComponent?> user,
    bool hideLayers)
  {
    if (this._timing.ApplyingState || !this.Resolve<HideLayerClothingComponent, ClothingComponent>(clothing.Owner, ref clothing.Comp1, ref clothing.Comp2, true) || !this.Resolve<HumanoidAppearanceComponent>(user.Owner, ref user.Comp, false))
      return;
    hideLayers &= this.IsEnabled(clothing);
    HashSet<HumanoidVisualLayers> hideLayersOnEquip = user.Comp.HideLayersOnEquip;
    SlotFlags valueOrDefault = clothing.Comp2.InSlotFlag.GetValueOrDefault();
    bool dirty = false;
    foreach ((HumanoidVisualLayers humanoidVisualLayers, SlotFlags slotFlags) in clothing.Comp1.Layers)
    {
      if (hideLayersOnEquip.Contains(humanoidVisualLayers) && slotFlags.HasFlag((Enum) valueOrDefault))
        this._humanoid.SetLayerVisibility(user, humanoidVisualLayers, !hideLayers, new SlotFlags?(valueOrDefault), ref dirty);
    }
    HashSet<HumanoidVisualLayers> slots = clothing.Comp1.Slots;
    if (slots != null && clothing.Comp2.Slots.HasFlag((Enum) valueOrDefault))
    {
      foreach (HumanoidVisualLayers layer in slots)
      {
        if (hideLayersOnEquip.Contains(layer))
          this._humanoid.SetLayerVisibility(user, layer, !hideLayers, new SlotFlags?(valueOrDefault), ref dirty);
      }
    }
    if (!dirty)
      return;
    this.Dirty<HumanoidAppearanceComponent>(user, (MetaDataComponent) null);
  }

  private bool IsEnabled(
    Entity<HideLayerClothingComponent, ClothingComponent> clothing)
  {
    MaskComponent maskComponent;
    return !clothing.Comp1.HideOnToggle || !this.TryComp<MaskComponent>(Entity<HideLayerClothingComponent, ClothingComponent>.op_Implicit(clothing), ref maskComponent) || !maskComponent.IsToggled;
  }
}
