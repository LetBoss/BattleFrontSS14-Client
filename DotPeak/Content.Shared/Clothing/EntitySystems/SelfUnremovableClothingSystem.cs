// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.EntitySystems.SelfUnremovableClothingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.Components;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.Clothing.EntitySystems;

public sealed class SelfUnremovableClothingSystem : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SelfUnremovableClothingComponent, BeingUnequippedAttemptEvent>(new EntityEventRefHandler<SelfUnremovableClothingComponent, BeingUnequippedAttemptEvent>((object) this, __methodptr(OnUnequip)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SelfUnremovableClothingComponent, ExaminedEvent>(new EntityEventRefHandler<SelfUnremovableClothingComponent, ExaminedEvent>((object) this, __methodptr(OnUnequipMarkup)), (Type[]) null, (Type[]) null);
  }

  private void OnUnequip(
    Entity<SelfUnremovableClothingComponent> selfUnremovableClothing,
    ref BeingUnequippedAttemptEvent args)
  {
    ClothingComponent clothingComponent;
    if (this.TryComp<ClothingComponent>(Entity<SelfUnremovableClothingComponent>.op_Implicit(selfUnremovableClothing), ref clothingComponent) && (clothingComponent.Slots & args.SlotFlags) == SlotFlags.NONE || !EntityUid.op_Equality(args.UnEquipTarget, args.Unequipee))
      return;
    args.Cancel();
  }

  private void OnUnequipMarkup(
    Entity<SelfUnremovableClothingComponent> selfUnremovableClothing,
    ref ExaminedEvent args)
  {
    args.PushMarkup(this.Loc.GetString("comp-self-unremovable-clothing"));
  }
}
