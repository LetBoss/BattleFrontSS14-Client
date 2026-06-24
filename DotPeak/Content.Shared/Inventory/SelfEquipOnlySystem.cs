// Decompiled with JetBrains decompiler
// Type: Content.Shared.Inventory.SelfEquipOnlySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Inventory;

public sealed class SelfEquipOnlySystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlocker;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<SelfEquipOnlyComponent, BeingEquippedAttemptEvent>(new EntityEventRefHandler<SelfEquipOnlyComponent, BeingEquippedAttemptEvent>(this.OnBeingEquipped));
    this.SubscribeLocalEvent<SelfEquipOnlyComponent, BeingUnequippedAttemptEvent>(new EntityEventRefHandler<SelfEquipOnlyComponent, BeingUnequippedAttemptEvent>(this.OnBeingUnequipped));
  }

  private void OnBeingEquipped(
    Entity<SelfEquipOnlyComponent> ent,
    ref BeingEquippedAttemptEvent args)
  {
    ClothingComponent comp;
    if (args.Cancelled || this.TryComp<ClothingComponent>((EntityUid) ent, out comp) && (comp.Slots & args.SlotFlags) == SlotFlags.NONE || !(args.Equipee != args.EquipTarget))
      return;
    args.Cancel();
  }

  private void OnBeingUnequipped(
    Entity<SelfEquipOnlyComponent> ent,
    ref BeingUnequippedAttemptEvent args)
  {
    ClothingComponent comp;
    if (args.Cancelled || args.Unequipee == args.UnEquipTarget || this.TryComp<ClothingComponent>((EntityUid) ent, out comp) && (comp.Slots & args.SlotFlags) == SlotFlags.NONE || ent.Comp.UnequipRequireConscious && !this._actionBlocker.CanConsciouslyPerformAction(args.UnEquipTarget))
      return;
    args.Cancel();
  }
}
