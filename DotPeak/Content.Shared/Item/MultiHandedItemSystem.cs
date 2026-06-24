// Decompiled with JetBrains decompiler
// Type: Content.Shared.Item.MultiHandedItemSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared.Item;

public sealed class MultiHandedItemSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedVirtualItemSystem _virtualItem;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MultiHandedItemComponent, GettingPickedUpAttemptEvent>(new EntityEventRefHandler<MultiHandedItemComponent, GettingPickedUpAttemptEvent>(this.OnAttemptPickup));
    this.SubscribeLocalEvent<MultiHandedItemComponent, VirtualItemDeletedEvent>(new EntityEventRefHandler<MultiHandedItemComponent, VirtualItemDeletedEvent>(this.OnVirtualItemDeleted));
    this.SubscribeLocalEvent<MultiHandedItemComponent, GotEquippedHandEvent>(new EntityEventRefHandler<MultiHandedItemComponent, GotEquippedHandEvent>(this.OnEquipped));
    this.SubscribeLocalEvent<MultiHandedItemComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<MultiHandedItemComponent, GotUnequippedHandEvent>(this.OnUnequipped));
  }

  private void OnEquipped(Entity<MultiHandedItemComponent> ent, ref GotEquippedHandEvent args)
  {
    for (int index = 0; index < ent.Comp.HandsNeeded - 1; ++index)
      this._virtualItem.TrySpawnVirtualItemInHand(ent.Owner, args.User);
  }

  private void OnUnequipped(Entity<MultiHandedItemComponent> ent, ref GotUnequippedHandEvent args)
  {
    this._virtualItem.DeleteInHandsMatching(args.User, ent.Owner);
  }

  private void OnAttemptPickup(
    Entity<MultiHandedItemComponent> ent,
    ref GettingPickedUpAttemptEvent args)
  {
    if (this._hands.CountFreeHands((Entity<HandsComponent>) args.User) >= ent.Comp.HandsNeeded)
      return;
    args.Cancel();
    this._popup.PopupPredictedCursor(this.Loc.GetString("multi-handed-item-pick-up-fail", ("number", (object) (ent.Comp.HandsNeeded - 1)), ("item", (object) ent.Owner)), args.User);
  }

  private void OnVirtualItemDeleted(
    Entity<MultiHandedItemComponent> ent,
    ref VirtualItemDeletedEvent args)
  {
    if (args.BlockingEntity != ent.Owner || this._timing.ApplyingState)
      return;
    this._hands.TryDrop((Entity<HandsComponent>) args.User, ent.Owner);
  }
}
