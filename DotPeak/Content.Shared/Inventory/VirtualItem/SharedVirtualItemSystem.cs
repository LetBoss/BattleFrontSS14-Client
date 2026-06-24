// Decompiled with JetBrains decompiler
// Type: Content.Shared.Inventory.VirtualItem.SharedVirtualItemSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Popups;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Inventory.VirtualItem;

public abstract class SharedVirtualItemSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _containerSystem;
  [Dependency]
  private SharedItemSystem _itemSystem;
  [Dependency]
  private InventorySystem _inventorySystem;
  [Dependency]
  private SharedHandsSystem _handsSystem;
  [Dependency]
  private SharedPopupSystem _popup;
  private static readonly EntProtoId VirtualItem = (EntProtoId) nameof (VirtualItem);

  public override void Initialize()
  {
    this.SubscribeLocalEvent<VirtualItemComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<VirtualItemComponent, AfterAutoHandleStateEvent>(this.OnAfterAutoHandleState));
    this.SubscribeLocalEvent<VirtualItemComponent, BeingEquippedAttemptEvent>(new EntityEventRefHandler<VirtualItemComponent, BeingEquippedAttemptEvent>(this.OnBeingEquippedAttempt));
    this.SubscribeLocalEvent<VirtualItemComponent, BeingUnequippedAttemptEvent>(new EntityEventRefHandler<VirtualItemComponent, BeingUnequippedAttemptEvent>(this.OnBeingUnequippedAttempt));
    this.SubscribeLocalEvent<VirtualItemComponent, BeforeRangedInteractEvent>(new EntityEventRefHandler<VirtualItemComponent, BeforeRangedInteractEvent>(this.OnBeforeRangedInteract));
    this.SubscribeLocalEvent<VirtualItemComponent, GettingInteractedWithAttemptEvent>(new EntityEventRefHandler<VirtualItemComponent, GettingInteractedWithAttemptEvent>(this.OnGettingInteractedWithAttemptEvent));
    this.SubscribeLocalEvent<VirtualItemComponent, GetUsedEntityEvent>(new EntityEventRefHandler<VirtualItemComponent, GetUsedEntityEvent>(this.OnGetUsedEntity));
  }

  private void OnAfterAutoHandleState(
    Entity<VirtualItemComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    if (!this._containerSystem.IsEntityInContainer((EntityUid) ent))
      return;
    this._itemSystem.VisualsChanged((EntityUid) ent);
  }

  private void OnBeingEquippedAttempt(
    Entity<VirtualItemComponent> ent,
    ref BeingEquippedAttemptEvent args)
  {
    args.Cancel();
  }

  private void OnBeingUnequippedAttempt(
    Entity<VirtualItemComponent> ent,
    ref BeingUnequippedAttemptEvent args)
  {
    args.Cancel();
  }

  private void OnBeforeRangedInteract(
    Entity<VirtualItemComponent> ent,
    ref BeforeRangedInteractEvent args)
  {
    args.Handled = true;
  }

  private void OnGettingInteractedWithAttemptEvent(
    Entity<VirtualItemComponent> ent,
    ref GettingInteractedWithAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnGetUsedEntity(Entity<VirtualItemComponent> ent, ref GetUsedEntityEvent args)
  {
    if (args.Handled)
      return;
    foreach (EntityUid entityUid in this._handsSystem.EnumerateHeld((Entity<HandsComponent>) args.User))
    {
      if (entityUid == ent.Comp.BlockingEntity)
      {
        args.Used = new EntityUid?(ent.Comp.BlockingEntity);
        break;
      }
    }
  }

  public bool TrySpawnVirtualItemInHand(EntityUid blockingEnt, EntityUid user, bool dropOthers = false)
  {
    return this.TrySpawnVirtualItemInHand(blockingEnt, user, out EntityUid? _, dropOthers);
  }

  public bool TrySpawnVirtualItemInHand(
    EntityUid blockingEnt,
    EntityUid user,
    [NotNullWhen(true)] out EntityUid? virtualItem,
    bool dropOthers = false,
    string? empty = null)
  {
    virtualItem = new EntityUid?();
    if (empty == null && !this._handsSystem.TryGetEmptyHand((Entity<HandsComponent>) user, out empty))
    {
      if (!dropOthers)
        return false;
      foreach (string enumerateHand in this._handsSystem.EnumerateHands((Entity<HandsComponent>) user))
      {
        EntityUid? held;
        if (this._handsSystem.TryGetHeldItem((Entity<HandsComponent>) user, enumerateHand, out held))
        {
          EntityUid? nullable = held;
          EntityUid entityUid = blockingEnt;
          if ((nullable.HasValue ? (nullable.GetValueOrDefault() == entityUid ? 1 : 0) : 0) == 0 && this._handsSystem.TryDrop((Entity<HandsComponent>) user, enumerateHand))
          {
            if (!this.TerminatingOrDeleted(held))
              this._popup.PopupClient(this.Loc.GetString("virtual-item-dropped-other", ("dropped", (object) held)), user, new EntityUid?(user));
            empty = enumerateHand;
            break;
          }
        }
      }
    }
    if (empty == null || !this.TrySpawnVirtualItem(blockingEnt, user, out virtualItem))
      return false;
    this._handsSystem.DoPickup(user, empty, virtualItem.Value);
    return true;
  }

  public void DeleteInHandsMatching(EntityUid user, EntityUid matching)
  {
    foreach (EntityUid uid in this._handsSystem.EnumerateHeld((Entity<HandsComponent>) user))
    {
      VirtualItemComponent comp;
      if (this.TryComp<VirtualItemComponent>(uid, out comp) && comp.BlockingEntity == matching)
        this.DeleteVirtualItem((Entity<VirtualItemComponent>) (uid, comp), user);
    }
  }

  public bool TrySpawnVirtualItemInInventory(
    EntityUid blockingEnt,
    EntityUid user,
    string slot,
    bool force = false)
  {
    return this.TrySpawnVirtualItemInInventory(blockingEnt, user, slot, force, out EntityUid? _);
  }

  public bool TrySpawnVirtualItemInInventory(
    EntityUid blockingEnt,
    EntityUid user,
    string slot,
    bool force,
    [NotNullWhen(true)] out EntityUid? virtualItem)
  {
    if (!this.TrySpawnVirtualItem(blockingEnt, user, out virtualItem))
      return false;
    this._inventorySystem.TryEquip(user, virtualItem.Value, slot, force: force);
    return true;
  }

  public void DeleteInSlotMatching(EntityUid user, EntityUid matching, string? slotName = null)
  {
    if (slotName != null)
    {
      EntityUid? entityUid;
      VirtualItemComponent comp;
      if (!this._inventorySystem.TryGetSlotEntity(user, slotName, out entityUid) || !this.TryComp<VirtualItemComponent>(entityUid, out comp) || !(comp.BlockingEntity == matching))
        return;
      this.DeleteVirtualItem((Entity<VirtualItemComponent>) (entityUid.Value, comp), user);
    }
    else
    {
      SlotDefinition[] slotDefinitions;
      if (!this._inventorySystem.TryGetSlots(user, out slotDefinitions))
        return;
      foreach (SlotDefinition slotDefinition in slotDefinitions)
      {
        EntityUid? entityUid;
        VirtualItemComponent comp;
        if (this._inventorySystem.TryGetSlotEntity(user, slotDefinition.Name, out entityUid) && this.TryComp<VirtualItemComponent>(entityUid, out comp) && comp.BlockingEntity == matching)
          this.DeleteVirtualItem((Entity<VirtualItemComponent>) (entityUid.Value, comp), user);
      }
    }
  }

  public bool TrySpawnVirtualItem(
    EntityUid blockingEnt,
    EntityUid user,
    [NotNullWhen(true)] out EntityUid? virtualItem)
  {
    EntityCoordinates coordinates = this.Transform(user).Coordinates;
    virtualItem = new EntityUid?(this.PredictedSpawnAttachedTo((string) SharedVirtualItemSystem.VirtualItem, coordinates, rotation: new Angle()));
    VirtualItemComponent virtualItemComponent = this.Comp<VirtualItemComponent>(virtualItem.Value);
    virtualItemComponent.BlockingEntity = blockingEnt;
    this.Dirty(virtualItem.Value, (IComponent) virtualItemComponent);
    return true;
  }

  public void DeleteVirtualItem(Entity<VirtualItemComponent> item, EntityUid user)
  {
    VirtualItemDeletedEvent args1 = new VirtualItemDeletedEvent(item.Comp.BlockingEntity, user);
    this.RaiseLocalEvent<VirtualItemDeletedEvent>(user, args1);
    VirtualItemDeletedEvent args2 = new VirtualItemDeletedEvent(item.Comp.BlockingEntity, user);
    this.RaiseLocalEvent<VirtualItemDeletedEvent>(item.Comp.BlockingEntity, args2);
    if (this.TerminatingOrDeleted((EntityUid) item))
      return;
    this.PredictedQueueDel(item.Owner);
  }
}
