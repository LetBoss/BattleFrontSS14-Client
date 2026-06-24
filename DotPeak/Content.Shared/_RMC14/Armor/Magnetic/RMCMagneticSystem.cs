// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Armor.Magnetic.RMCMagneticSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Inventory;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;

#nullable enable
namespace Content.Shared._RMC14.Armor.Magnetic;

public sealed class RMCMagneticSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private ItemSlotsSystem _slots;
  [Dependency]
  private ThrownItemSystem _thrownItem;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCMagneticItemComponent, DroppedEvent>(new EntityEventRefHandler<RMCMagneticItemComponent, DroppedEvent>(this.OnMagneticItemDropped));
    this.SubscribeLocalEvent<RMCMagneticItemComponent, RMCDroppedEvent>(new EntityEventRefHandler<RMCMagneticItemComponent, RMCDroppedEvent>(this.OnMagneticItemRMCDropped));
    this.SubscribeLocalEvent<RMCMagneticItemComponent, ThrownEvent>(new EntityEventRefHandler<RMCMagneticItemComponent, ThrownEvent>(this.OnMagneticItemThrown));
    this.SubscribeLocalEvent<RMCMagneticItemComponent, DropAttemptEvent>(new EntityEventRefHandler<RMCMagneticItemComponent, DropAttemptEvent>(this.OnMagneticItemDropAttempt));
    this.SubscribeLocalEvent<RMCMagneticArmorComponent, InventoryRelayedEvent<RMCMagnetizeItemEvent>>(new EntityEventRefHandler<RMCMagneticArmorComponent, InventoryRelayedEvent<RMCMagnetizeItemEvent>>(this.OnMagnetizeItem));
    this.SubscribeLocalEvent<InventoryComponent, RMCMagnetizeItemEvent>(new EntityEventRefHandler<InventoryComponent, RMCMagnetizeItemEvent>(this._inventory.RelayEvent<RMCMagnetizeItemEvent>));
  }

  private void OnMagneticItemDropped(Entity<RMCMagneticItemComponent> ent, ref DroppedEvent args)
  {
    this.TryReturn(ent, args.User);
  }

  private void OnMagneticItemRMCDropped(
    Entity<RMCMagneticItemComponent> ent,
    ref RMCDroppedEvent args)
  {
    this.TryReturn(ent, args.User);
  }

  private void OnMagneticItemThrown(Entity<RMCMagneticItemComponent> ent, ref ThrownEvent args)
  {
    EntityUid? user = args.User;
    if (!user.HasValue)
      return;
    EntityUid valueOrDefault = user.GetValueOrDefault();
    ThrownItemComponent comp;
    if (!this.TryReturn(ent, valueOrDefault) || !this.TryComp<ThrownItemComponent>((EntityUid) ent, out comp))
      return;
    this._thrownItem.StopThrow((EntityUid) ent, comp);
  }

  private void OnMagneticItemDropAttempt(
    Entity<RMCMagneticItemComponent> ent,
    ref DropAttemptEvent args)
  {
    if (!this.CanReturn(ent, args.Uid, out EntityUid _, out EntityUid? _, out string _))
      return;
    args.Cancel();
  }

  private void OnMagnetizeItem(
    Entity<RMCMagneticArmorComponent> ent,
    ref InventoryRelayedEvent<RMCMagnetizeItemEvent> args)
  {
    InventorySystem.InventorySlotEnumerator slotEnumerator1 = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) args.Args.User);
    ContainerSlot container1;
    while (slotEnumerator1.MoveNext(out container1))
    {
      EntityUid? containedEntity = container1.ContainedEntity;
      if (containedEntity.HasValue)
      {
        EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
        ItemSlotsComponent comp;
        if (this.HasComp<RMCMagneticItemReceiverComponent>(valueOrDefault) && this.TryComp<ItemSlotsComponent>(valueOrDefault, out comp))
        {
          foreach (BaseContainer allContainer in this._container.GetAllContainers(valueOrDefault))
          {
            ItemSlot itemSlot;
            if (this._slots.TryGetSlot(valueOrDefault, allContainer.ID, out itemSlot, comp) && this._slots.CanInsert((EntityUid) ent, args.Args.Item, new EntityUid?(args.Args.User), itemSlot))
            {
              args.Args.Magnetizer = new EntityUid?((EntityUid) ent);
              args.Args.ReceivingItem = new EntityUid?(valueOrDefault);
              args.Args.ReceivingContainer = allContainer.ID;
              return;
            }
          }
        }
      }
    }
    if ((ent.Comp.AllowMagnetizeToSlots & args.Args.MagnetizeToSlots) == SlotFlags.NONE)
      return;
    InventorySystem.InventorySlotEnumerator slotEnumerator2 = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) args.Args.User, ent.Comp.AllowMagnetizeToSlots & args.Args.MagnetizeToSlots);
    ContainerSlot container2;
    while (slotEnumerator2.MoveNext(out container2))
    {
      if (container2.Count <= 0)
      {
        args.Args.Magnetizer = new EntityUid?((EntityUid) ent);
        break;
      }
    }
  }

  private bool CanReturn(
    Entity<RMCMagneticItemComponent> ent,
    EntityUid user,
    out EntityUid magnetizer,
    out EntityUid? receivingItem,
    out string receivingContainer)
  {
    RMCMagnetizeItemEvent args = new RMCMagnetizeItemEvent(user, ent.Owner, ent.Comp.MagnetizeToSlots, SlotFlags.OUTERCLOTHING);
    this.RaiseLocalEvent<RMCMagnetizeItemEvent>(user, ref args);
    magnetizer = args.Magnetizer.GetValueOrDefault();
    receivingItem = args.ReceivingItem;
    receivingContainer = args.ReceivingContainer;
    return magnetizer != new EntityUid();
  }

  private bool TryReturn(Entity<RMCMagneticItemComponent> ent, EntityUid user)
  {
    EntityUid magnetizer;
    EntityUid? receivingItem;
    string receivingContainer;
    if (!this.CanReturn(ent, user, out magnetizer, out receivingItem, out receivingContainer))
      return false;
    RMCReturnToInventoryComponent inventoryComponent = this.EnsureComp<RMCReturnToInventoryComponent>((EntityUid) ent);
    inventoryComponent.User = user;
    inventoryComponent.Magnetizer = magnetizer;
    inventoryComponent.ReceivingItem = receivingItem;
    inventoryComponent.ReceivingContainer = receivingContainer;
    this.Dirty((EntityUid) ent, (IComponent) inventoryComponent);
    return true;
  }

  public void SetMagnetizeToSlots(Entity<RMCMagneticItemComponent> ent, SlotFlags slots)
  {
    ent.Comp.MagnetizeToSlots = slots;
    this.Dirty<RMCMagneticItemComponent>(ent);
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCReturnToInventoryComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCReturnToInventoryComponent>();
    EntityUid uid;
    RMCReturnToInventoryComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!comp1.Returned)
      {
        EntityUid user = comp1.User;
        EntityUid magnetizer = comp1.Magnetizer;
        if (!this.TerminatingOrDeleted(user) && !this.TerminatingOrDeleted(magnetizer))
        {
          EntityUid? receivingItem = comp1.ReceivingItem;
          if (receivingItem.HasValue)
          {
            EntityUid valueOrDefault = receivingItem.GetValueOrDefault();
            BaseContainer container;
            if (this._container.TryGetContainer(valueOrDefault, comp1.ReceivingContainer, out container) && this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) uid, container, force: true))
            {
              this._popup.PopupClient(this.Loc.GetString("rmc-magnetize-return", ("item", (object) uid), ("magnetizer", (object) valueOrDefault)), user, new EntityUid?(user), PopupType.Medium);
              comp1.Returned = true;
              this.Dirty(uid, (IComponent) comp1);
            }
          }
          else
          {
            InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) user, SlotFlags.SUITSTORAGE);
            ContainerSlot container;
            while (slotEnumerator.MoveNext(out container))
            {
              if (this._inventory.TryEquip(user, uid, container.ID, force: true))
              {
                this._popup.PopupClient(this.Loc.GetString("rmc-magnetize-return", ("item", (object) uid), ("magnetizer", (object) magnetizer)), user, new EntityUid?(user), PopupType.Medium);
                comp1.Returned = true;
                this.Dirty(uid, (IComponent) comp1);
                break;
              }
            }
          }
        }
        this.RemCompDeferred<RMCReturnToInventoryComponent>(uid);
      }
    }
  }
}
