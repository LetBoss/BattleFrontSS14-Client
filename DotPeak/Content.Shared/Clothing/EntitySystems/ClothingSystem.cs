// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.EntitySystems.ClothingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Strip.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Shared.Clothing.EntitySystems;

public abstract class ClothingSystem : EntitySystem
{
  [Dependency]
  private SharedItemSystem _itemSys;
  [Dependency]
  private InventorySystem _invSystem;
  [Dependency]
  private SharedHandsSystem _handsSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingComponent, UseInHandEvent>(new EntityEventRefHandler<ClothingComponent, UseInHandEvent>((object) this, __methodptr(OnUseInHand)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<ClothingComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(AfterAutoHandleState)), (Type[]) null, (Type[]) null);
    ClothingSystem clothingSystem1 = this;
    // ISSUE: virtual method pointer
    this.SubscribeLocalEvent<ClothingComponent, GotEquippedEvent>(new ComponentEventHandler<ClothingComponent, GotEquippedEvent>((object) clothingSystem1, __vmethodptr(clothingSystem1, OnGotEquipped)), (Type[]) null, (Type[]) null);
    ClothingSystem clothingSystem2 = this;
    // ISSUE: virtual method pointer
    this.SubscribeLocalEvent<ClothingComponent, GotUnequippedEvent>(new ComponentEventHandler<ClothingComponent, GotUnequippedEvent>((object) clothingSystem2, __vmethodptr(clothingSystem2, OnGotUnequipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingComponent, ClothingEquipDoAfterEvent>(new EntityEventRefHandler<ClothingComponent, ClothingEquipDoAfterEvent>((object) this, __methodptr(OnEquipDoAfter)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingComponent, ClothingUnequipDoAfterEvent>(new EntityEventRefHandler<ClothingComponent, ClothingUnequipDoAfterEvent>((object) this, __methodptr(OnUnequipDoAfter)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingComponent, BeforeItemStrippedEvent>(new EntityEventRefHandler<ClothingComponent, BeforeItemStrippedEvent>((object) this, __methodptr(OnItemStripped)), (Type[]) null, (Type[]) null);
  }

  private void OnUseInHand(Entity<ClothingComponent> ent, ref UseInHandEvent args)
  {
    if (args.Handled || !ent.Comp.QuickEquip)
      return;
    EntityUid user = args.User;
    InventoryComponent inventoryComponent;
    HandsComponent handsComponent;
    if (!this.TryComp<InventoryComponent>(user, ref inventoryComponent) || !this.TryComp<HandsComponent>(user, ref handsComponent))
      return;
    this.QuickEquip(ent, Entity<InventoryComponent, HandsComponent>.op_Implicit((user, inventoryComponent, handsComponent)));
    args.Handled = true;
    args.ApplyDelay = false;
  }

  private void QuickEquip(
    Entity<ClothingComponent> toEquipEnt,
    Entity<InventoryComponent, HandsComponent> userEnt)
  {
    foreach (SlotDefinition slot in userEnt.Comp1.Slots)
    {
      if (this._invSystem.CanEquip(Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), Entity<ClothingComponent>.op_Implicit(toEquipEnt), slot.Name, out string _, slot, Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), Entity<ClothingComponent>.op_Implicit(toEquipEnt)))
      {
        EntityUid? entityUid;
        if (this._invSystem.TryGetSlotEntity(Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), slot.Name, out entityUid, Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt)))
        {
          ClothingComponent clothingComponent;
          if ((!this.TryComp<ClothingComponent>(entityUid, ref clothingComponent) || clothingComponent.QuickEquip) && this._invSystem.TryUnequip(Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), slot.Name, true, inventory: Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), checkDoafter: true) && this._invSystem.TryEquip(Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), Entity<ClothingComponent>.op_Implicit(toEquipEnt), slot.Name, inventory: Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), clothing: Entity<ClothingComponent>.op_Implicit(toEquipEnt), checkDoafter: true, triggerHandContact: true))
          {
            this._handsSystem.PickupOrDrop(new EntityUid?(Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt)), entityUid.Value, handsComp: Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt));
            break;
          }
        }
        else if (this._invSystem.TryEquip(Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), Entity<ClothingComponent>.op_Implicit(toEquipEnt), slot.Name, inventory: Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), clothing: Entity<ClothingComponent>.op_Implicit(toEquipEnt), checkDoafter: true, triggerHandContact: true))
          break;
      }
    }
  }

  protected virtual void OnGotEquipped(
    EntityUid uid,
    ClothingComponent component,
    GotEquippedEvent args)
  {
    component.InSlot = args.Slot;
    component.InSlotFlag = new SlotFlags?(args.SlotFlags);
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    if ((component.Slots & args.SlotFlags) == SlotFlags.NONE)
      return;
    ClothingGotEquippedEvent gotEquippedEvent = new ClothingGotEquippedEvent(args.Equipee, component);
    this.RaiseLocalEvent<ClothingGotEquippedEvent>(uid, ref gotEquippedEvent, false);
    ClothingDidEquippedEvent didEquippedEvent = new ClothingDidEquippedEvent(Entity<ClothingComponent>.op_Implicit((uid, component)));
    this.RaiseLocalEvent<ClothingDidEquippedEvent>(args.Equipee, ref didEquippedEvent, false);
  }

  protected virtual void OnGotUnequipped(
    EntityUid uid,
    ClothingComponent component,
    GotUnequippedEvent args)
  {
    if ((component.Slots & args.SlotFlags) != SlotFlags.NONE)
    {
      ClothingGotUnequippedEvent gotUnequippedEvent = new ClothingGotUnequippedEvent(args.Equipee, component);
      this.RaiseLocalEvent<ClothingGotUnequippedEvent>(uid, ref gotUnequippedEvent, false);
      ClothingDidUnequippedEvent didUnequippedEvent = new ClothingDidUnequippedEvent(Entity<ClothingComponent>.op_Implicit((uid, component)));
      this.RaiseLocalEvent<ClothingDidUnequippedEvent>(args.Equipee, ref didUnequippedEvent, false);
    }
    component.InSlot = (string) null;
    component.InSlotFlag = new SlotFlags?();
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
  }

  private void AfterAutoHandleState(
    Entity<ClothingComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this._itemSys.VisualsChanged(ent.Owner);
  }

  private void OnEquipDoAfter(Entity<ClothingComponent> ent, ref ClothingEquipDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    args.Handled = this._invSystem.TryEquip(args.User, valueOrDefault, Entity<ClothingComponent>.op_Implicit(ent), args.Slot, predicted: true, clothing: ent.Comp);
  }

  private void OnUnequipDoAfter(Entity<ClothingComponent> ent, ref ClothingUnequipDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    args.Handled = this._invSystem.TryUnequip(args.User, valueOrDefault, args.Slot, predicted: true, clothing: ent.Comp, triggerHandContact: true);
    if (!args.Handled)
      return;
    this._handsSystem.TryPickup(args.User, Entity<ClothingComponent>.op_Implicit(ent));
  }

  private void OnItemStripped(Entity<ClothingComponent> ent, ref BeforeItemStrippedEvent args)
  {
    BeforeItemStrippedEvent itemStrippedEvent = args;
    itemStrippedEvent.Additive = itemStrippedEvent.Additive + ent.Comp.StripDelay;
  }

  public void SetEquippedPrefix(EntityUid uid, string? prefix, ClothingComponent? clothing = null)
  {
    if (!this.Resolve<ClothingComponent>(uid, ref clothing, false) || clothing.EquippedPrefix == prefix)
      return;
    clothing.EquippedPrefix = prefix;
    this._itemSys.VisualsChanged(uid);
    this.Dirty(uid, (IComponent) clothing, (MetaDataComponent) null);
  }

  public void SetSlots(EntityUid uid, SlotFlags slots, ClothingComponent? clothing = null)
  {
    if (!this.Resolve<ClothingComponent>(uid, ref clothing, true))
      return;
    clothing.Slots = slots;
    this.Dirty(uid, (IComponent) clothing, (MetaDataComponent) null);
  }

  public void CopyVisuals(
    EntityUid uid,
    ClothingComponent otherClothing,
    ClothingComponent? clothing = null)
  {
    if (!this.Resolve<ClothingComponent>(uid, ref clothing, true))
      return;
    clothing.ClothingVisuals = otherClothing.ClothingVisuals;
    clothing.EquippedPrefix = otherClothing.EquippedPrefix;
    clothing.RsiPath = otherClothing.RsiPath;
    this._itemSys.VisualsChanged(uid);
    this.Dirty(uid, (IComponent) clothing, (MetaDataComponent) null);
  }

  public void SetLayerColor(ClothingComponent clothing, string slot, string mapKey, Color? color)
  {
    foreach (PrototypeLayerData prototypeLayerData in clothing.ClothingVisuals[slot])
    {
      if (prototypeLayerData.MapKeys == null)
        break;
      if (prototypeLayerData.MapKeys.Contains(mapKey))
        prototypeLayerData.Color = color;
    }
  }

  public void SetLayerState(ClothingComponent clothing, string slot, string mapKey, string state)
  {
    foreach (PrototypeLayerData prototypeLayerData in clothing.ClothingVisuals[slot])
    {
      if (prototypeLayerData.MapKeys == null)
        break;
      if (prototypeLayerData.MapKeys.Contains(mapKey))
        prototypeLayerData.State = state;
    }
  }
}
