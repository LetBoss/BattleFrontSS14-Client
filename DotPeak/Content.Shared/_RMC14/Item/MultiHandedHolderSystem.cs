// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Item.MultiHandedHolderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared._RMC14.Item;

public sealed class MultiHandedHolderSystem : EntitySystem
{
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedVirtualItemSystem _virtualItem;
  [Dependency]
  private EntityWhitelistSystem _whitelist;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MultiHandedHolderComponent, PickupAttemptEvent>(new EntityEventRefHandler<MultiHandedHolderComponent, PickupAttemptEvent>(this.OnPickupAttempt));
    this.SubscribeLocalEvent<MultiHandedHolderComponent, VirtualItemDeletedEvent>(new EntityEventRefHandler<MultiHandedHolderComponent, VirtualItemDeletedEvent>(this.OnVirtualItemDeleted));
    this.SubscribeLocalEvent<MultiHandedHolderComponent, DidEquipHandEvent>(new EntityEventRefHandler<MultiHandedHolderComponent, DidEquipHandEvent>(this.OnEquipped));
    this.SubscribeLocalEvent<MultiHandedHolderComponent, DidUnequipHandEvent>(new EntityEventRefHandler<MultiHandedHolderComponent, DidUnequipHandEvent>(this.OnUnequipped));
  }

  private void OnPickupAttempt(
    Entity<MultiHandedHolderComponent> holder,
    ref PickupAttemptEvent args)
  {
    int? handsNeeded = this.GetHandsNeeded(holder, args.Item);
    if (!handsNeeded.HasValue)
      return;
    int valueOrDefault = handsNeeded.GetValueOrDefault();
    if (this.HasComp<HandsComponent>(args.User) && this._hands.CountFreeHands((Entity<HandsComponent>) args.User) >= valueOrDefault)
      return;
    args.Cancel();
    if (!this._timing.IsFirstTimePredicted)
      return;
    this._popup.PopupCursor(this.Loc.GetString("multi-handed-item-pick-up-fail", ("number", (object) (valueOrDefault - 1)), ("item", (object) args.Item)), args.User);
  }

  private void OnVirtualItemDeleted(
    Entity<MultiHandedHolderComponent> ent,
    ref VirtualItemDeletedEvent args)
  {
    if (args.User != ent.Owner)
      return;
    this._hands.TryDrop((Entity<HandsComponent>) args.User, args.BlockingEntity);
  }

  private void OnEquipped(Entity<MultiHandedHolderComponent> holder, ref DidEquipHandEvent args)
  {
    int? handsNeeded = this.GetHandsNeeded(holder, args.Equipped);
    if (!handsNeeded.HasValue)
      return;
    int valueOrDefault = handsNeeded.GetValueOrDefault();
    for (int index = 0; index < valueOrDefault - 1; ++index)
      this._virtualItem.TrySpawnVirtualItemInHand(args.Equipped, args.User);
  }

  private void OnUnequipped(Entity<MultiHandedHolderComponent> holder, ref DidUnequipHandEvent args)
  {
    this._virtualItem.DeleteInHandsMatching(args.User, args.Unequipped);
  }

  private int? GetHandsNeeded(Entity<MultiHandedHolderComponent> holder, EntityUid item)
  {
    foreach ((int Hands, EntityWhitelist entityWhitelist) in holder.Comp.Items)
    {
      if (this._whitelist.IsValid(entityWhitelist, item))
        return new int?(Hands);
    }
    return new int?();
  }
}
