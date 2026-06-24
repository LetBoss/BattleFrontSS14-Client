// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Storage.RMCStorageSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CrashLand;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Prototypes;
using Content.Shared._RMC14.Storage.Containers;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Lock;
using Content.Shared.ParaDrop;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Stunnable;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Storage;

public sealed class RMCStorageSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedCrashLandSystem _crashLand;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedEntityStorageSystem _entityStorage;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedItemSystem _item;
  [Dependency]
  private LockSystem _lock;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedStorageSystem _storage;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  private readonly List<EntityUid> _toRemove = new List<EntityUid>();
  private readonly List<EntityUid> _toClose = new List<EntityUid>();
  private Robust.Shared.GameObjects.EntityQuery<ItemComponent> _itemQuery;
  private Robust.Shared.GameObjects.EntityQuery<StorageComponent> _storageQuery;
  private readonly TimeSpan _stunStorage = TimeSpan.FromSeconds(4L);

  public override void Initialize()
  {
    this._itemQuery = this.GetEntityQuery<ItemComponent>();
    this._storageQuery = this.GetEntityQuery<StorageComponent>();
    this.SubscribeLocalEvent<StorageComponent, CMStorageItemFillEvent>(new EntityEventRefHandler<StorageComponent, CMStorageItemFillEvent>(this.OnStorageFillItem));
    this.SubscribeLocalEvent<StorageOpenDoAfterComponent, OpenStorageDoAfterEvent>(new EntityEventRefHandler<StorageOpenDoAfterComponent, OpenStorageDoAfterEvent>(this.OnStorageOpenDoAfter));
    this.SubscribeLocalEvent<StorageSkillRequiredComponent, StorageInteractAttemptEvent>(new EntityEventRefHandler<StorageSkillRequiredComponent, StorageInteractAttemptEvent>(this.OnStorageSkillOpenAttempt));
    this.SubscribeLocalEvent<StorageSkillRequiredComponent, DumpableDoAfterEvent>(new EntityEventRefHandler<StorageSkillRequiredComponent, DumpableDoAfterEvent>(this.OnDumpableDoAfter), new Type[1]
    {
      typeof (DumpableSystem)
    });
    this.SubscribeLocalEvent<StorageCloseOnMoveComponent, GotEquippedEvent>(new EntityEventRefHandler<StorageCloseOnMoveComponent, GotEquippedEvent>(this.OnStorageEquip));
    this.SubscribeLocalEvent<BlockEntityStorageComponent, InsertIntoEntityStorageAttemptEvent>(new EntityEventRefHandler<BlockEntityStorageComponent, InsertIntoEntityStorageAttemptEvent>(this.OnBlockInsertIntoEntityStorageAttempt));
    this.SubscribeLocalEvent<MarineComponent, EntGotRemovedFromContainerMessage>(new EntityEventRefHandler<MarineComponent, EntGotRemovedFromContainerMessage>(this.OnRemovedMarineFromContainer));
    this.SubscribeLocalEvent<StorageNestedOpenSkillRequiredComponent, StorageInteractAttemptEvent>(new EntityEventRefHandler<StorageNestedOpenSkillRequiredComponent, StorageInteractAttemptEvent>(this.OnNestedSkillRequiredInteractAttempt));
    this.SubscribeLocalEvent<SkyFallingComponent, StorageInteractAttemptEvent>(new EntityEventRefHandler<SkyFallingComponent, StorageInteractAttemptEvent>(this.OnSkyFalling));
    this.SubscribeLocalEvent<CrashLandingComponent, StorageInteractAttemptEvent>(new EntityEventRefHandler<CrashLandingComponent, StorageInteractAttemptEvent>(this.OnCrashLanding));
    this.SubscribeLocalEvent<ParaDroppingComponent, StorageInteractAttemptEvent>(new EntityEventRefHandler<ParaDroppingComponent, StorageInteractAttemptEvent>(this.OnParaDropping));
    this.SubscribeLocalEvent<RMCEntityStorageWhitelistComponent, ContainerIsInsertingAttemptEvent>(new EntityEventRefHandler<RMCEntityStorageWhitelistComponent, ContainerIsInsertingAttemptEvent>(this.OnEntityStorageWhitelistAttempt));
    this.SubscribeLocalEvent<EntityStorageCloseOnMapInitComponent, MapInitEvent>(new EntityEventRefHandler<EntityStorageCloseOnMapInitComponent, MapInitEvent>(this.OnEntityStorageClose));
    this.SubscribeLocalEvent<RMCContainerEmptyOnDestructionComponent, DestructionEventArgs>(new EntityEventRefHandler<RMCContainerEmptyOnDestructionComponent, DestructionEventArgs>(this.OnContainerEmptyDestroyed));
    this.SubscribeLocalEvent<RMCContainerEmptyOnDestructionComponent, EntityTerminatingEvent>(new EntityEventRefHandler<RMCContainerEmptyOnDestructionComponent, EntityTerminatingEvent>(this.OnContainerEmptyDeleted));
    this.Subs.BuiEvents<StorageCloseOnMoveComponent>((object) StorageComponent.StorageUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<StorageCloseOnMoveComponent>) (subs => subs.Event<BoundUIOpenedEvent>(new EntityEventRefHandler<StorageCloseOnMoveComponent, BoundUIOpenedEvent>(this.OnCloseOnMoveUIOpened))));
    this.Subs.BuiEvents<StorageOpenComponent>((object) StorageComponent.StorageUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<StorageOpenComponent>) (subs => subs.Event<BoundUIClosedEvent>(new EntityEventRefHandler<StorageOpenComponent, BoundUIClosedEvent>(this.OnCloseOnMoveUIClosed))));
    this.UpdatesAfter.Add(typeof (SharedStorageSystem));
  }

  private void OnDumpableDoAfter(
    Entity<StorageSkillRequiredComponent> ent,
    ref DumpableDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled || !this.TryCancel(args.User, ent))
      return;
    args.Handled = true;
  }

  private void OnStorageFillItem(Entity<StorageComponent> storage, ref CMStorageItemFillEvent args)
  {
    int num = 0;
    string reason;
    while (!this._storage.CanInsert((EntityUid) storage, (EntityUid) args.Item, new EntityUid?(), out reason, ignoreStacks: true) && reason == "comp-storage-insufficient-capacity" && num < 3)
    {
      ++num;
      if (CMPrototypeExtensions.FilterCM)
        this.Log.Warning($"Storage {this.ToPrettyString(new EntityUid?((EntityUid) storage))} can't fit {this.ToPrettyString(new EntityUid?((EntityUid) args.Item))}");
      foreach (Box2i box2i1 in (IEnumerable<Box2i>) this._item.GetItemShape((Entity<StorageComponent>) ((EntityUid) storage, args.Storage), (Entity<ItemComponent>) ((EntityUid) args.Item, (ItemComponent) args.Item)))
      {
        List<Box2i> grid = args.Storage.Grid;
        if (grid.Count == 0)
        {
          grid.Add(box2i1);
        }
        else
        {
          List<Box2i> box2iList1 = grid;
          Box2i box2i2 = box2iList1[box2iList1.Count - 1];
          Box2i box2i3;
          // ISSUE: explicit constructor call
          ((Box2i) ref box2i3).\u002Ector(box2i2.Left, box2i2.Bottom, box2i2.Right + ((Box2i) ref box2i1).Width + 1, box2i2.Top);
          if (box2i3.Top < box2i1.Top)
            box2i3.Top = box2i1.Top;
          List<Box2i> box2iList2 = grid;
          box2iList2[box2iList2.Count - 1] = box2i3;
        }
      }
    }
  }

  public bool IgnoreItemSize(Entity<StorageComponent> storage, EntityUid item)
  {
    IgnoreContentsSizeComponent comp;
    return this.TryComp<IgnoreContentsSizeComponent>((EntityUid) storage, out comp) && this._entityWhitelist.IsValid(comp.Items, item);
  }

  public bool OpenDoAfter(
    EntityUid uid,
    EntityUid entity,
    StorageComponent? storageComp = null,
    bool silent = false)
  {
    StorageOpenDoAfterComponent comp;
    if (!this.TryComp<StorageOpenDoAfterComponent>(uid, out comp) || comp.Duration == TimeSpan.Zero || comp.SkipInHand && this._hands.IsHolding((Entity<HandsComponent>) entity, new EntityUid?(uid)) || comp.SkipOnGround && !this._inventory.TryGetContainingSlot((Entity<TransformComponent, MetaDataComponent>) uid, out SlotDefinition _))
      return false;
    OpenStorageDoAfterEvent @event = new OpenStorageDoAfterEvent(this.GetNetEntity(uid), this.GetNetEntity(entity), silent);
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, entity, comp.Duration, (DoAfterEvent) @event, new EntityUid?(uid))
    {
      BreakOnMove = true
    });
    return true;
  }

  private void OnStorageOpenDoAfter(
    Entity<StorageOpenDoAfterComponent> ent,
    ref OpenStorageDoAfterEvent args)
  {
    EntityUid? entity1;
    EntityUid? entity2;
    StorageComponent comp;
    if (args.Cancelled || args.Handled || !this.TryGetEntity(args.Uid, out entity1) || !this.TryGetEntity(args.Entity, out entity2) || !this.TryComp<StorageComponent>(entity1, out comp))
      return;
    args.Handled = true;
    this._storage.OpenStorageUI(entity1.Value, entity2.Value, comp, args.Silent, false);
  }

  private void OnStorageSkillOpenAttempt(
    Entity<StorageSkillRequiredComponent> ent,
    ref StorageInteractAttemptEvent args)
  {
    if (args.Cancelled || !this.TryCancel(args.User, ent))
      return;
    args.Cancelled = true;
  }

  private void OnStorageEquip(Entity<StorageCloseOnMoveComponent> ent, ref GotEquippedEvent args)
  {
    this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) StorageComponent.StorageUiKey.Key, new EntityUid?(args.Equipee));
    StorageOpenComponent comp;
    if (!this.TryComp<StorageOpenComponent>((EntityUid) ent, out comp))
      return;
    comp.OpenedAt.Remove(args.Equipee);
  }

  private void OnBlockInsertIntoEntityStorageAttempt(
    Entity<BlockEntityStorageComponent> ent,
    ref InsertIntoEntityStorageAttemptEvent args)
  {
    if (!this._entityWhitelist.IsWhitelistPassOrNull(ent.Comp.Whitelist, args.Container))
      return;
    args.Cancelled = true;
  }

  private void OnRemovedMarineFromContainer(
    Entity<MarineComponent> ent,
    ref EntGotRemovedFromContainerMessage args)
  {
    if (this._timing.ApplyingState || this.TerminatingOrDeleted((EntityUid) ent))
      return;
    if (!this.HasComp<NoStunOnExitComponent>(args.Container.Owner))
      this._stun.TryStun((EntityUid) ent, this._stunStorage, true);
    if (!this.HasComp<SkyFallingComponent>(args.Container.Owner) && !this.HasComp<CrashLandingComponent>(args.Container.Owner))
      return;
    AttemptCrashLandEvent args1 = new AttemptCrashLandEvent();
    this.RaiseLocalEvent<AttemptCrashLandEvent>((EntityUid) ent, ref args1);
    if (args1.Cancelled)
      return;
    this._crashLand.TryCrashLand((Entity<CrashLandableComponent>) ent.Owner, true);
  }

  private void OnNestedSkillRequiredInteractAttempt(
    Entity<StorageNestedOpenSkillRequiredComponent> ent,
    ref StorageInteractAttemptEvent args)
  {
    BaseContainer container;
    StorageComponent comp;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null), out container) || !this.TryComp<StorageComponent>(container.Owner, out comp) || !comp.StoredItems.ContainsKey((EntityUid) ent) || this._skills.HasSkills((Entity<SkillsComponent>) args.User, ent.Comp.Skills))
      return;
    args.Cancelled = true;
    if (args.Silent)
      return;
    this._popup.PopupClient(this.Loc.GetString("rmc-storage-nested-unable", ("nested", (object) ent), ("parent", (object) container.Owner)), (EntityUid) ent, new EntityUid?(args.User), PopupType.SmallCaution);
  }

  private void OnEntityStorageWhitelistAttempt(
    Entity<RMCEntityStorageWhitelistComponent> ent,
    ref ContainerIsInsertingAttemptEvent args)
  {
    if (args.Cancelled || this._entityWhitelist.IsWhitelistPass(ent.Comp.Whitelist, args.EntityUid))
      return;
    args.Cancel();
  }

  private void OnEntityStorageClose(
    Entity<EntityStorageCloseOnMapInitComponent> ent,
    ref MapInitEvent args)
  {
    this._toClose.Add((EntityUid) ent);
  }

  private void OnCloseOnMoveUIOpened(
    Entity<StorageCloseOnMoveComponent> ent,
    ref BoundUIOpenedEvent args)
  {
    if (this._timing.ApplyingState || ent.Comp.SkipInHand && this._hands.IsHolding((Entity<HandsComponent>) args.Actor, new EntityUid?((EntityUid) ent)))
      return;
    EntityUid actor = args.Actor;
    NetCoordinates netCoordinates = this.GetNetCoordinates(this._transform.GetMoverCoordinates(actor));
    this.EnsureComp<StorageOpenComponent>((EntityUid) ent).OpenedAt[actor] = netCoordinates;
  }

  private void OnCloseOnMoveUIClosed(Entity<StorageOpenComponent> ent, ref BoundUIClosedEvent args)
  {
    ent.Comp.OpenedAt.Remove(args.Actor);
  }

  private void OnSkyFalling(Entity<SkyFallingComponent> ent, ref StorageInteractAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnParaDropping(
    Entity<ParaDroppingComponent> ent,
    ref StorageInteractAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnCrashLanding(
    Entity<CrashLandingComponent> ent,
    ref StorageInteractAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private bool TryCancel(EntityUid user, Entity<StorageSkillRequiredComponent> storage)
  {
    if (this._skills.HasAllSkills((Entity<SkillsComponent>) user, storage.Comp.Skills))
      return false;
    this._popup.PopupClient(this.Loc.GetString("cm-storage-unskilled"), (EntityUid) storage, new EntityUid?(user), PopupType.SmallCaution);
    return true;
  }

  private bool CanInsertStorageLimit(
    Entity<StorageComponent?, LimitedStorageComponent?> limited,
    EntityUid toInsert,
    out LocId popup)
  {
    popup = new LocId();
    if (!this.Resolve<LimitedStorageComponent>((EntityUid) limited, ref limited.Comp2, false) || !this._storageQuery.Resolve((EntityUid) limited, ref limited.Comp1, false))
      return true;
    foreach (LimitedStorageComponent.Limit limit in limited.Comp2.Limits)
    {
      if (this._entityWhitelist.IsWhitelistPassOrNull(limit.Whitelist, toInsert) && !this._entityWhitelist.IsBlacklistPass(limit.Blacklist, toInsert))
      {
        int num = 0;
        foreach (EntityUid key in limited.Comp1.StoredItems.Keys)
        {
          if (!(key == toInsert) && this._entityWhitelist.IsWhitelistPassOrNull(limit.Whitelist, key) && !this._entityWhitelist.IsBlacklistPass(limit.Blacklist, key))
          {
            ++num;
            if (num >= limit.Count)
              break;
          }
        }
        if (num >= limit.Count)
        {
          popup = limit.Popup == new LocId() ? (LocId) "rmc-storage-limit-cant-fit" : limit.Popup;
          return false;
        }
      }
    }
    return true;
  }

  public bool CanInsertStoreSkill(
    Entity<StorageComponent?, StorageStoreSkillRequiredComponent?> store,
    EntityUid toInsert,
    EntityUid? user,
    out LocId popup)
  {
    popup = new LocId();
    if (!user.HasValue || !this.Resolve<StorageStoreSkillRequiredComponent>((EntityUid) store, ref store.Comp2, false) || !this._storageQuery.Resolve((EntityUid) store, ref store.Comp1, false))
      return true;
    foreach (StorageStoreSkillRequiredComponent.Entry entry in store.Comp2.Entries)
    {
      if (!this._entityWhitelist.IsWhitelistFail(entry.Whitelist, toInsert) && !this._skills.HasSkills((Entity<SkillsComponent>) user.Value, entry.Skills))
      {
        popup = (LocId) "rmc-storage-store-skill-unable";
        return false;
      }
    }
    return true;
  }

  private bool CanEjectStoreSkill(
    Entity<StorageComponent?, StorageSkillRequiredComponent?> store,
    EntityUid? user,
    out LocId popup)
  {
    popup = new LocId();
    if (!user.HasValue || !this.Resolve<StorageSkillRequiredComponent>((EntityUid) store, ref store.Comp2, false) || !this._storageQuery.Resolve((EntityUid) store, ref store.Comp1, false) || this._skills.HasAllSkills((Entity<SkillsComponent>) user.Value, store.Comp2.Skills))
      return true;
    popup = (LocId) this.Loc.GetString("cm-storage-unskilled");
    return false;
  }

  public bool TryGetLastItem(Entity<StorageComponent?> storage, out EntityUid item)
  {
    item = new EntityUid();
    if (!this.Resolve<StorageComponent>((EntityUid) storage, ref storage.Comp, false))
      return false;
    ItemStorageLocation? nullable = new ItemStorageLocation?();
    foreach ((EntityUid key, ItemStorageLocation itemStorageLocation) in storage.Comp.StoredItems)
    {
      if (nullable.HasValue)
      {
        ItemStorageLocation valueOrDefault = nullable.GetValueOrDefault();
        if (valueOrDefault.Position.Y >= itemStorageLocation.Position.Y)
        {
          if (valueOrDefault.Position.Y == itemStorageLocation.Position.Y && valueOrDefault.Position.X > itemStorageLocation.Position.X)
          {
            item = key;
            nullable = new ItemStorageLocation?(itemStorageLocation);
            continue;
          }
          continue;
        }
      }
      item = key;
      nullable = new ItemStorageLocation?(itemStorageLocation);
    }
    return item != new EntityUid();
  }

  public bool TryGetFirstItem(Entity<StorageComponent?> storage, out EntityUid item)
  {
    item = new EntityUid();
    if (!this.Resolve<StorageComponent>((EntityUid) storage, ref storage.Comp, false))
      return false;
    ItemStorageLocation? nullable = new ItemStorageLocation?();
    foreach ((EntityUid key, ItemStorageLocation itemStorageLocation) in storage.Comp.StoredItems)
    {
      if (nullable.HasValue)
      {
        ItemStorageLocation valueOrDefault = nullable.GetValueOrDefault();
        if (valueOrDefault.Position.Y <= itemStorageLocation.Position.Y)
        {
          if (valueOrDefault.Position.Y == itemStorageLocation.Position.Y && valueOrDefault.Position.X < itemStorageLocation.Position.X)
          {
            item = key;
            nullable = new ItemStorageLocation?(itemStorageLocation);
            continue;
          }
          continue;
        }
      }
      item = key;
      nullable = new ItemStorageLocation?(itemStorageLocation);
    }
    return item != new EntityUid();
  }

  public bool CanInsert(
    Entity<StorageComponent?> storage,
    EntityUid toInsert,
    EntityUid? user,
    out LocId popup)
  {
    return this.CanInsertStorageLimit((Entity<StorageComponent, LimitedStorageComponent>) ((EntityUid) storage, (StorageComponent) storage, (LimitedStorageComponent) null), toInsert, out popup) && this.CanInsertStoreSkill((Entity<StorageComponent, StorageStoreSkillRequiredComponent>) ((EntityUid) storage, (StorageComponent) storage, (StorageStoreSkillRequiredComponent) null), toInsert, user, out popup);
  }

  public bool CanEject(EntityUid storage, EntityUid user, out LocId popup)
  {
    return this.CanEjectStoreSkill((Entity<StorageComponent, StorageSkillRequiredComponent>) storage, new EntityUid?(user), out popup);
  }

  private void OnContainerEmptyDestroyed(
    Entity<RMCContainerEmptyOnDestructionComponent> containerEnt,
    ref DestructionEventArgs args)
  {
    if (!containerEnt.Comp.OnDestruction)
      return;
    this.ContainerDestructionEmpty(containerEnt);
  }

  private void OnContainerEmptyDeleted(
    Entity<RMCContainerEmptyOnDestructionComponent> containerEnt,
    ref EntityTerminatingEvent args)
  {
    if (!containerEnt.Comp.OnDelete)
      return;
    this.ContainerDestructionEmpty(containerEnt);
  }

  private void ContainerDestructionEmpty(
    Entity<RMCContainerEmptyOnDestructionComponent> containerEnt)
  {
    TransformComponent comp1;
    ContainerManagerComponent comp2;
    if (!this.TryComp((EntityUid) containerEnt, out comp1) || this.TerminatingOrDeleted(comp1.GridUid) || !this.Exists((EntityUid) containerEnt) || !this.TryComp<ContainerManagerComponent>((EntityUid) containerEnt, out comp2))
      return;
    RMCContainerDestructionEmptyEvent args = new RMCContainerDestructionEmptyEvent();
    this.RaiseLocalEvent<RMCContainerDestructionEmptyEvent>((EntityUid) containerEnt, ref args);
    if (args.Handled)
      return;
    foreach (BaseContainer allContainer in this._container.GetAllContainers((EntityUid) containerEnt, comp2))
      this._container.EmptyContainer(allContainer);
  }

  public int EstimateFreeColumns(Entity<StorageComponent?> storage)
  {
    if (!this.Resolve<StorageComponent>((EntityUid) storage, ref storage.Comp, false))
      return 0;
    int num = 0;
    foreach (Box2i box2i in storage.Comp.Grid)
      num += (((Box2i) ref box2i).Width + 1) * ((Box2i) ref box2i).Height;
    foreach ((EntityUid entityUid, ItemStorageLocation _) in storage.Comp.StoredItems)
    {
      ItemComponent component;
      if (this._itemQuery.TryComp(entityUid, out component))
      {
        IReadOnlyList<Box2i> itemShape = this._item.GetItemShape(storage, (Entity<ItemComponent>) (entityUid, component));
        if (itemShape.Count != 0)
        {
          Box2i box2i = itemShape[0];
          num -= ((Box2i) ref box2i).Width;
        }
      }
    }
    return num;
  }

  public override void Update(float frameTime)
  {
    try
    {
      if (this._net.IsServer)
      {
        foreach (EntityUid entityUid in this._toClose)
        {
          int num = this._lock.IsLocked((Entity<LockComponent>) entityUid) ? 1 : 0;
          if (num != 0)
            this._lock.Unlock(entityUid, new EntityUid?());
          this._entityStorage.OpenStorage(entityUid);
          this._entityStorage.CloseStorage(entityUid);
          if (num != 0)
            this._lock.Lock(entityUid, new EntityUid?());
        }
      }
    }
    finally
    {
      this._toClose.Clear();
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<RemoveOnlyStorageComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<RemoveOnlyStorageComponent>();
    EntityUid uid1;
    RemoveOnlyStorageComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      StorageComponent comp;
      if (this.TryComp<StorageComponent>(uid1, out comp))
      {
        if (comp1_1.Blacklist != null)
          comp.Blacklist = comp1_1.Blacklist;
        comp.Whitelist = comp1_1.Whitelist;
        this.Dirty(uid1, (IComponent) comp);
      }
      this.RemCompDeferred<RemoveOnlyStorageComponent>(uid1);
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<StorageOpenComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<StorageOpenComponent>();
    EntityUid uid2;
    StorageOpenComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      this._toRemove.Clear();
      foreach ((EntityUid entityUid, NetCoordinates netEntity) in comp1_2.OpenedAt)
      {
        if (this.TerminatingOrDeleted(entityUid))
          this._toRemove.Add(entityUid);
        else if (!this._transform.InRange(this.GetCoordinates(netEntity), this._transform.GetMoverCoordinates(entityUid), 0.1f))
          this._toRemove.Add(entityUid);
      }
      foreach (EntityUid key in this._toRemove)
      {
        this._ui.CloseUi((Entity<UserInterfaceComponent>) uid2, (Enum) StorageComponent.StorageUiKey.Key, new EntityUid?(key));
        comp1_2.OpenedAt.Remove(key);
      }
      if (comp1_2.OpenedAt.Count == 0)
        this.RemCompDeferred<StorageOpenComponent>(uid2);
    }
  }
}
