// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.EntitySystems.FoldableClothingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.Components;
using Content.Shared.Foldable;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Clothing.EntitySystems;

public sealed class FoldableClothingSystem : EntitySystem
{
  [Dependency]
  private ClothingSystem _clothingSystem;
  [Dependency]
  private InventorySystem _inventorySystem;
  [Dependency]
  private SharedItemSystem _itemSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FoldableClothingComponent, FoldAttemptEvent>(new EntityEventRefHandler<FoldableClothingComponent, FoldAttemptEvent>((object) this, __methodptr(OnFoldAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FoldableClothingComponent, FoldedEvent>(new EntityEventRefHandler<FoldableClothingComponent, FoldedEvent>((object) this, __methodptr(OnFolded)), (Type[]) null, new Type[1]
    {
      typeof (MaskSystem)
    });
  }

  private void OnFoldAttempt(Entity<FoldableClothingComponent> ent, ref FoldAttemptEvent args)
  {
    SlotDefinition slot;
    if (args.Cancelled || !this._inventorySystem.TryGetContainingSlot(Entity<TransformComponent, MetaDataComponent>.op_Implicit(ent.Owner), out slot))
      return;
    SlotFlags? nullable = args.Comp.IsFolded ? ent.Comp.UnfoldedSlots : ent.Comp.FoldedSlots;
    if (nullable.HasValue && (nullable.Value & slot.SlotFlags) != slot.SlotFlags)
    {
      args.Cancelled = true;
    }
    else
    {
      if (ent.Comp.FoldedHideLayers.Count == 0 && ent.Comp.UnfoldedHideLayers.Count == 0)
        return;
      args.Cancelled = true;
    }
  }

  private void OnFolded(Entity<FoldableClothingComponent> ent, ref FoldedEvent args)
  {
    ClothingComponent clothing;
    ItemComponent component;
    if (!this.TryComp<ClothingComponent>(ent.Owner, ref clothing) || !this.TryComp<ItemComponent>(ent.Owner, ref component))
      return;
    if (args.IsFolded)
    {
      if (ent.Comp.FoldedSlots.HasValue)
        this._clothingSystem.SetSlots(ent.Owner, ent.Comp.FoldedSlots.Value, clothing);
      if (ent.Comp.FoldedEquippedPrefix != null)
        this._clothingSystem.SetEquippedPrefix(ent.Owner, ent.Comp.FoldedEquippedPrefix, clothing);
      if (ent.Comp.FoldedHeldPrefix != null)
        this._itemSystem.SetHeldPrefix(ent.Owner, ent.Comp.FoldedHeldPrefix, component: component);
      HideLayerClothingComponent clothingComponent;
      if (ent.Comp.FoldedHideLayers.Count == 0 || !this.TryComp<HideLayerClothingComponent>(ent.Owner, ref clothingComponent))
        return;
      clothingComponent.Slots = ent.Comp.FoldedHideLayers;
    }
    else
    {
      if (ent.Comp.UnfoldedSlots.HasValue)
        this._clothingSystem.SetSlots(ent.Owner, ent.Comp.UnfoldedSlots.Value, clothing);
      if (ent.Comp.FoldedEquippedPrefix != null)
        this._clothingSystem.SetEquippedPrefix(ent.Owner, (string) null, clothing);
      if (ent.Comp.FoldedHeldPrefix != null)
        this._itemSystem.SetHeldPrefix(ent.Owner, (string) null, component: component);
      HideLayerClothingComponent clothingComponent;
      if (ent.Comp.UnfoldedHideLayers.Count == 0 || !this.TryComp<HideLayerClothingComponent>(ent.Owner, ref clothingComponent))
        return;
      clothingComponent.Slots = ent.Comp.UnfoldedHideLayers;
    }
  }
}
