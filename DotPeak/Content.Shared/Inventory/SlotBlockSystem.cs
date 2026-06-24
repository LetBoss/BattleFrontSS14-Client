// Decompiled with JetBrains decompiler
// Type: Content.Shared.Inventory.SlotBlockSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Inventory;

public sealed class SlotBlockSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SlotBlockComponent, InventoryRelayedEvent<IsEquippingTargetAttemptEvent>>(new EntityEventRefHandler<SlotBlockComponent, InventoryRelayedEvent<IsEquippingTargetAttemptEvent>>(this.OnEquipAttempt));
    this.SubscribeLocalEvent<SlotBlockComponent, InventoryRelayedEvent<IsUnequippingTargetAttemptEvent>>(new EntityEventRefHandler<SlotBlockComponent, InventoryRelayedEvent<IsUnequippingTargetAttemptEvent>>(this.OnUnequipAttempt));
  }

  private void OnEquipAttempt(
    Entity<SlotBlockComponent> ent,
    ref InventoryRelayedEvent<IsEquippingTargetAttemptEvent> args)
  {
    if (args.Args.Cancelled || (args.Args.SlotFlags & ent.Comp.Slots) == SlotFlags.NONE)
      return;
    args.Args.Reason = this.Loc.GetString("slot-block-component-blocked", ("item", (object) ent));
    args.Args.Cancel();
  }

  private void OnUnequipAttempt(
    Entity<SlotBlockComponent> ent,
    ref InventoryRelayedEvent<IsUnequippingTargetAttemptEvent> args)
  {
    if (args.Args.Cancelled || (args.Args.SlotFlags & ent.Comp.Slots) == SlotFlags.NONE)
      return;
    args.Args.Reason = this.Loc.GetString("slot-block-component-blocked", ("item", (object) ent));
    args.Args.Cancel();
  }
}
