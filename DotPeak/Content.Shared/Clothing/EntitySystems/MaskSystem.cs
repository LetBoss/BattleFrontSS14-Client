// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.EntitySystems.MaskSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Clothing.Components;
using Content.Shared.Foldable;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Clothing.EntitySystems;

public sealed class MaskSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actionSystem;
  [Dependency]
  private InventorySystem _inventorySystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private ClothingSystem _clothing;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MaskComponent, ToggleMaskEvent>(new EntityEventRefHandler<MaskComponent, ToggleMaskEvent>((object) this, __methodptr(OnToggleMask)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MaskComponent, GetItemActionsEvent>(new ComponentEventHandler<MaskComponent, GetItemActionsEvent>((object) this, __methodptr(OnGetActions)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MaskComponent, GotUnequippedEvent>(new ComponentEventHandler<MaskComponent, GotUnequippedEvent>((object) this, __methodptr(OnGotUnequipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MaskComponent, FoldedEvent>(new EntityEventRefHandler<MaskComponent, FoldedEvent>((object) this, __methodptr(OnFolded)), (Type[]) null, (Type[]) null);
  }

  private void OnGetActions(EntityUid uid, MaskComponent component, GetItemActionsEvent args)
  {
    if (!this._inventorySystem.InSlotWithFlags(Entity<TransformComponent, MetaDataComponent>.op_Implicit(uid), SlotFlags.MASK))
      return;
    args.AddAction(ref component.ToggleActionEntity, EntProtoId.op_Implicit(component.ToggleAction));
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
  }

  private void OnToggleMask(Entity<MaskComponent> ent, ref ToggleMaskEvent args)
  {
    EntityUid entityUid1;
    MaskComponent maskComponent1;
    ent.Deconstruct(ref entityUid1, ref maskComponent1);
    EntityUid entityUid2 = entityUid1;
    MaskComponent maskComponent2 = maskComponent1;
    ClothingComponent clothingComponent;
    if (!maskComponent2.ToggleActionEntity.HasValue || !maskComponent2.IsToggleable || !this.TryComp<ClothingComponent>(Entity<MaskComponent>.op_Implicit(ent), ref clothingComponent))
      return;
    SlotFlags? inSlotFlag = clothingComponent.InSlotFlag;
    if (!inSlotFlag.HasValue)
      return;
    SlotFlags valueOrDefault = inSlotFlag.GetValueOrDefault();
    if (!clothingComponent.Slots.HasFlag((Enum) valueOrDefault))
      return;
    this.SetToggled(Entity<MaskComponent>.op_Implicit((entityUid2, maskComponent2)), !maskComponent2.IsToggled);
    this._popupSystem.PopupClient(this.Loc.GetString($"action-mask-pull-{(maskComponent2.IsToggled ? "down" : "up")}-popup-message", ("mask", (object) entityUid2)), args.Performer, new EntityUid?(args.Performer));
  }

  private void OnGotUnequipped(EntityUid uid, MaskComponent mask, GotUnequippedEvent args)
  {
    if (!mask.IsToggled || !mask.IsToggleable)
      return;
    mask.IsToggled = false;
    this.ToggleMaskComponents(uid, mask, args.Equipee, mask.EquippedPrefix, true);
  }

  private void ToggleMaskComponents(
    EntityUid uid,
    MaskComponent mask,
    EntityUid wearer,
    string? equippedPrefix = null,
    bool isEquip = false)
  {
    this.Dirty(uid, (IComponent) mask, (MetaDataComponent) null);
    EntityUid? toggleActionEntity = mask.ToggleActionEntity;
    if (toggleActionEntity.HasValue)
      this._actionSystem.SetToggled(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleActionEntity.GetValueOrDefault())), mask.IsToggled);
    ItemMaskToggledEvent maskToggledEvent1 = new ItemMaskToggledEvent(Entity<MaskComponent>.op_Implicit((uid, mask)), new EntityUid?(wearer));
    this.RaiseLocalEvent<ItemMaskToggledEvent>(uid, ref maskToggledEvent1, false);
    WearerMaskToggledEvent maskToggledEvent2 = new WearerMaskToggledEvent(Entity<MaskComponent>.op_Implicit((uid, mask)));
    this.RaiseLocalEvent<WearerMaskToggledEvent>(wearer, ref maskToggledEvent2, false);
  }

  private void OnFolded(Entity<MaskComponent> ent, ref FoldedEvent args)
  {
    if (!ent.Comp.DisableOnFolded)
      return;
    this.SetToggled(ent, args.IsFolded, true);
    this.SetToggleable(ent, !args.IsFolded);
  }

  public void SetToggled(Entity<MaskComponent?> mask, bool toggled, bool force = false)
  {
    if (this._timing.ApplyingState || !this.Resolve<MaskComponent>(mask.Owner, ref mask.Comp, true) || !force && !mask.Comp.IsToggleable || mask.Comp.IsToggled == toggled)
      return;
    mask.Comp.IsToggled = toggled;
    EntityUid? toggleActionEntity = mask.Comp.ToggleActionEntity;
    if (toggleActionEntity.HasValue)
      this._actionSystem.SetToggled(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleActionEntity.GetValueOrDefault())), mask.Comp.IsToggled);
    string equippedPrefix = mask.Comp.IsToggled ? mask.Comp.EquippedPrefix : (string) null;
    this._clothing.SetEquippedPrefix(Entity<MaskComponent>.op_Implicit(mask), equippedPrefix);
    EntityUid? Wearer = new EntityUid?();
    ClothingComponent clothingComponent;
    if (this.TryComp<ClothingComponent>(Entity<MaskComponent>.op_Implicit(mask), ref clothingComponent))
    {
      SlotFlags? inSlotFlag = clothingComponent.InSlotFlag;
      if (inSlotFlag.HasValue)
      {
        SlotFlags valueOrDefault = inSlotFlag.GetValueOrDefault();
        if (clothingComponent.Slots.HasFlag((Enum) valueOrDefault))
          Wearer = new EntityUid?(this.Transform(Entity<MaskComponent>.op_Implicit(mask)).ParentUid);
      }
    }
    ItemMaskToggledEvent maskToggledEvent1 = new ItemMaskToggledEvent(mask, Wearer);
    this.RaiseLocalEvent<ItemMaskToggledEvent>(Entity<MaskComponent>.op_Implicit(mask), ref maskToggledEvent1, false);
    if (Wearer.HasValue)
    {
      WearerMaskToggledEvent maskToggledEvent2 = new WearerMaskToggledEvent(mask);
      this.RaiseLocalEvent<WearerMaskToggledEvent>(Wearer.Value, ref maskToggledEvent2, false);
    }
    this.Dirty<MaskComponent>(mask, (MetaDataComponent) null);
  }

  public void SetToggleable(Entity<MaskComponent?> mask, bool toggleable)
  {
    if (this._timing.ApplyingState || !this.Resolve<MaskComponent>(mask.Owner, ref mask.Comp, true) || mask.Comp.IsToggleable == toggleable)
      return;
    EntityUid? toggleActionEntity = mask.Comp.ToggleActionEntity;
    if (toggleActionEntity.HasValue)
      this._actionSystem.SetEnabled(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleActionEntity.GetValueOrDefault())), mask.Comp.IsToggleable);
    mask.Comp.IsToggleable = toggleable;
    this.Dirty<MaskComponent>(mask, (MetaDataComponent) null);
  }
}
