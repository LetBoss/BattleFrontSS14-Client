// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Webbing.SharedWebbingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Verbs;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Webbing;

public abstract class SharedWebbingSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedItemSystem _item;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private MobStateSystem _mob;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedStorageSystem _storage;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<ClothingBlockWebbingComponent, BeingEquippedAttemptEvent>(new EntityEventRefHandler<ClothingBlockWebbingComponent, BeingEquippedAttemptEvent>(this.OnBlockWebbingBeingEquippedAttempt));
    this.SubscribeLocalEvent<WebbingClothingComponent, MapInitEvent>(new EntityEventRefHandler<WebbingClothingComponent, MapInitEvent>(this.OnWebbingClothingMapInit));
    this.SubscribeLocalEvent<WebbingClothingComponent, InteractUsingEvent>(new EntityEventRefHandler<WebbingClothingComponent, InteractUsingEvent>(this.OnWebbingClothingInteractUsing));
    this.SubscribeLocalEvent<WebbingClothingComponent, InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>>(new ComponentEventHandler<WebbingClothingComponent, InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>>(this.GetRelayedVerbs));
    this.SubscribeLocalEvent<WebbingClothingComponent, GetVerbsEvent<EquipmentVerb>>(new EntityEventRefHandler<WebbingClothingComponent, GetVerbsEvent<EquipmentVerb>>(this.OnWebbingClothingGetEquipmentVerbs));
    this.SubscribeLocalEvent<WebbingClothingComponent, GetVerbsEvent<InteractionVerb>>(new EntityEventRefHandler<WebbingClothingComponent, GetVerbsEvent<InteractionVerb>>(this.OnWebbingClothingGetInteractionVerbs));
    this.SubscribeLocalEvent<WebbingClothingComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<WebbingClothingComponent, EntInsertedIntoContainerMessage>(this.OnClothingInserted));
    this.SubscribeLocalEvent<WebbingClothingComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<WebbingClothingComponent, EntRemovedFromContainerMessage>(this.OnClothingRemoved));
    this.SubscribeLocalEvent<WebbingClothingComponent, BeingEquippedAttemptEvent>(new EntityEventRefHandler<WebbingClothingComponent, BeingEquippedAttemptEvent>(this.OnClothingBeingEquippedAttempt));
  }

  private void OnBlockWebbingBeingEquippedAttempt(
    Entity<ClothingBlockWebbingComponent> ent,
    ref BeingEquippedAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) args.EquipTarget);
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      WebbingClothingComponent comp;
      if (this.TryComp<WebbingClothingComponent>(container.ContainedEntity, out comp) && comp.Webbing.HasValue)
      {
        args.Reason = "rmc-webbing-cannot-wear-with-webbing";
        args.Cancel();
      }
    }
  }

  private void OnWebbingClothingMapInit(Entity<WebbingClothingComponent> ent, ref MapInitEvent args)
  {
    EntProtoId<WebbingComponent>? startingWebbing = ent.Comp.StartingWebbing;
    if (!startingWebbing.HasValue)
      return;
    EntityUid webbing = this.Spawn((string) startingWebbing.GetValueOrDefault(), MapCoordinates.Nullspace, rotation: new Angle());
    this.Attach(ent, webbing, new EntityUid?(), out bool _);
  }

  private void OnWebbingClothingInteractUsing(
    Entity<WebbingClothingComponent> clothing,
    ref InteractUsingEvent args)
  {
    bool handled;
    this.Attach(clothing, args.Used, new EntityUid?(args.User), out handled);
    args.Handled = handled;
  }

  private void OnWebbingClothingGetInteractionVerbs(
    Entity<WebbingClothingComponent> clothing,
    ref GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || this.HasComp<XenoComponent>(args.User) || !this.HasWebbing((Entity<WebbingClothingComponent>) ((EntityUid) clothing, (WebbingClothingComponent) clothing), out Entity<WebbingComponent> _))
      return;
    EntityUid user = args.User;
    SortedSet<InteractionVerb> verbs = args.Verbs;
    InteractionVerb interactionVerb = new InteractionVerb();
    interactionVerb.Text = this.Loc.GetString("rmc-storage-webbing-remove-verb");
    interactionVerb.Act = (Action) (() => this.Detach(clothing, user));
    interactionVerb.IconEntity = new NetEntity?(this.GetNetEntity(clothing.Owner));
    verbs.Add(interactionVerb);
  }

  private void GetRelayedVerbs(
    EntityUid uid,
    WebbingClothingComponent component,
    InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>> args)
  {
    this.OnWebbingClothingGetEquipmentVerbs((Entity<WebbingClothingComponent>) (uid, component), ref args.Args);
  }

  private void OnWebbingClothingGetEquipmentVerbs(
    Entity<WebbingClothingComponent> clothing,
    ref GetVerbsEvent<EquipmentVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || this.HasComp<XenoComponent>(args.User) || !this.HasWebbing((Entity<WebbingClothingComponent>) ((EntityUid) clothing, (WebbingClothingComponent) clothing), out Entity<WebbingComponent> _))
      return;
    EntityUid parentUid = this.Transform((EntityUid) clothing).ParentUid;
    EntityUid user = args.User;
    if (user == parentUid || !this._mob.IsDead(parentUid))
      return;
    SortedSet<EquipmentVerb> verbs = args.Verbs;
    EquipmentVerb equipmentVerb = new EquipmentVerb();
    equipmentVerb.Text = this.Loc.GetString("rmc-storage-webbing-remove-verb");
    equipmentVerb.Act = (Action) (() => this.Detach(clothing, user));
    equipmentVerb.IconEntity = new NetEntity?(this.GetNetEntity(clothing.Owner));
    verbs.Add(equipmentVerb);
  }

  public bool HasWebbing(
    Entity<WebbingClothingComponent?> clothing,
    out Entity<WebbingComponent> webbing)
  {
    webbing = new Entity<WebbingComponent>();
    BaseContainer container;
    if (!this.Resolve<WebbingClothingComponent>((EntityUid) clothing, ref clothing.Comp, false) || !this._container.TryGetContainer((EntityUid) clothing, clothing.Comp.Container, out container) || container.Count <= 0)
      return false;
    EntityUid containedEntity = container.ContainedEntities[0];
    WebbingComponent comp;
    if (!this.TryComp<WebbingComponent>(containedEntity, out comp))
      return false;
    webbing = (Entity<WebbingComponent>) (containedEntity, comp);
    return true;
  }

  protected virtual void OnClothingInserted(
    Entity<WebbingClothingComponent> clothing,
    ref EntInsertedIntoContainerMessage args)
  {
    if (clothing.Comp.Container != args.Container.ID)
      return;
    clothing.Comp.Webbing = new EntityUid?(args.Entity);
    this.Dirty<WebbingClothingComponent>(clothing);
    this._item.VisualsChanged((EntityUid) clothing);
  }

  protected virtual void OnClothingRemoved(
    Entity<WebbingClothingComponent> clothing,
    ref EntRemovedFromContainerMessage args)
  {
    if (clothing.Comp.Container != args.Container.ID)
      return;
    clothing.Comp.Webbing = new EntityUid?();
    this.Dirty<WebbingClothingComponent>(clothing);
    this._item.VisualsChanged((EntityUid) clothing);
  }

  private void OnClothingBeingEquippedAttempt(
    Entity<WebbingClothingComponent> ent,
    ref BeingEquippedAttemptEvent args)
  {
    if (!ent.Comp.Webbing.HasValue)
      return;
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) args.EquipTarget);
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      if (this.HasComp<ClothingBlockWebbingComponent>(container.ContainedEntity))
      {
        args.Reason = "rmc-webbing-cannot-wear-with-webbing";
        args.Cancel();
      }
    }
  }

  public bool Attach(
    Entity<WebbingClothingComponent> clothing,
    EntityUid webbing,
    EntityUid? user,
    out bool handled)
  {
    handled = false;
    WebbingComponent comp1;
    ItemComponent comp2;
    ItemComponent comp3;
    if (!this.TryComp<WebbingComponent>(webbing, out comp1) || this.HasComp<StorageComponent>((EntityUid) clothing) || !this.HasComp<StorageComponent>(webbing) || !this.TryComp<ItemComponent>((EntityUid) clothing, out comp2) || !this.TryComp<ItemComponent>(webbing, out comp3))
      return false;
    BaseContainer container1;
    if (this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) clothing, (TransformComponent) null), out container1))
    {
      StorageComponent comp4;
      if (this.TryComp<StorageComponent>(container1.Owner, out comp4) && comp4.StoredItems.ContainsKey((EntityUid) clothing))
      {
        handled = true;
        if (user.HasValue)
          this._popup.PopupClient(this.Loc.GetString("rmc-webbing-cannot-in-storage"), user, PopupType.LargeCaution);
        return false;
      }
      InventoryComponent comp5;
      if (this.TryComp<InventoryComponent>(container1.Owner, out comp5))
      {
        InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) (container1.Owner, comp5));
        ContainerSlot container2;
        while (slotEnumerator.MoveNext(out container2))
        {
          if (this.HasComp<ClothingBlockWebbingComponent>(container2.ContainedEntity))
          {
            handled = true;
            if (user.HasValue)
              this._popup.PopupClient(this.Loc.GetString("rmc-webbing-cannot-wear-with-webbing"), webbing, user, PopupType.SmallCaution);
            return false;
          }
        }
      }
    }
    ContainerSlot container3 = this._container.EnsureContainer<ContainerSlot>((EntityUid) clothing, clothing.Comp.Container);
    if (container3.Count > 0 || !this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) webbing, (BaseContainer) container3))
      return false;
    this.EntityManager.AddComponents((EntityUid) clothing, comp1.Components, true);
    WebbingTransferComponent transferComponent = this.EnsureComp<WebbingTransferComponent>(webbing);
    transferComponent.Clothing = new EntityUid?((EntityUid) clothing);
    transferComponent.Transfer = WebbingTransferComponent.TransferType.ToClothing;
    this.Dirty(webbing, (IComponent) transferComponent);
    clothing.Comp.UnequippedSize = new ProtoId<ItemSizePrototype>?(comp2.Size);
    this._item.SetSize((EntityUid) clothing, comp3.Size);
    handled = true;
    return true;
  }

  private void Detach(Entity<WebbingClothingComponent> clothing, EntityUid user)
  {
    Entity<WebbingComponent> webbing;
    if (this.TerminatingOrDeleted((EntityUid) clothing) || !clothing.Comp.Running || !this.HasWebbing((Entity<WebbingClothingComponent>) ((EntityUid) clothing, (WebbingClothingComponent) clothing), out webbing))
      return;
    this._container.TryRemoveFromContainer((Entity<TransformComponent, MetaDataComponent>) webbing.Owner);
    this._hands.TryPickupAnyHand(user, (EntityUid) webbing);
    this.EntityManager.AddComponents((EntityUid) webbing, webbing.Comp.Components, true);
    WebbingTransferComponent transferComponent = this.EnsureComp<WebbingTransferComponent>((EntityUid) webbing);
    transferComponent.Clothing = new EntityUid?((EntityUid) clothing);
    transferComponent.Transfer = WebbingTransferComponent.TransferType.ToWebbing;
    this.Dirty((EntityUid) webbing, (IComponent) transferComponent);
    ProtoId<ItemSizePrototype>? unequippedSize = clothing.Comp.UnequippedSize;
    if (!unequippedSize.HasValue)
      return;
    ProtoId<ItemSizePrototype> valueOrDefault = unequippedSize.GetValueOrDefault();
    clothing.Comp.UnequippedSize = new ProtoId<ItemSizePrototype>?();
    this._item.SetSize((EntityUid) clothing, valueOrDefault);
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<WebbingTransferComponent, WebbingComponent> entityQueryEnumerator = this.EntityQueryEnumerator<WebbingTransferComponent, WebbingComponent>();
    EntityUid uid;
    WebbingTransferComponent comp1;
    WebbingComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (comp1.Defer)
      {
        comp1.Defer = false;
      }
      else
      {
        this.RemCompDeferred<WebbingTransferComponent>(uid);
        EntityUid? stackedEntity;
        switch (comp1.Transfer)
        {
          case WebbingTransferComponent.TransferType.ToClothing:
            StorageComponent comp3;
            if (this.TryComp<StorageComponent>(uid, out comp3))
            {
              stackedEntity = comp1.Clothing;
              if (stackedEntity.HasValue)
              {
                EntityUid valueOrDefault = stackedEntity.GetValueOrDefault();
                foreach (EntityUid insertEnt in comp3.Container.ContainedEntities.ToArray<EntityUid>())
                  this._storage.Insert(valueOrDefault, insertEnt, out stackedEntity, playSound: false);
                continue;
              }
              continue;
            }
            continue;
          case WebbingTransferComponent.TransferType.ToWebbing:
            stackedEntity = comp1.Clothing;
            if (stackedEntity.HasValue)
            {
              EntityUid valueOrDefault = stackedEntity.GetValueOrDefault();
              StorageComponent comp4;
              if (this.TryComp<StorageComponent>(valueOrDefault, out comp4))
              {
                foreach (EntityUid insertEnt in comp4.Container.ContainedEntities.ToArray<EntityUid>())
                  this._storage.Insert(uid, insertEnt, out stackedEntity, playSound: false);
              }
              using (Dictionary<string, EntityPrototype.ComponentRegistryEntry>.ValueCollection.Enumerator enumerator = comp2.Components.Values.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  EntityPrototype.ComponentRegistryEntry current = enumerator.Current;
                  this.RemComp(valueOrDefault, current.Component.GetType());
                }
                continue;
              }
            }
            continue;
          default:
            continue;
        }
      }
    }
  }
}
