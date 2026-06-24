// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Hands.RMCHandsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Storage;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Hands;

public abstract class RMCHandsSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private RMCStorageSystem _rmcStorage;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private SharedStorageSystem _storage;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<GiveHandsComponent, MapInitEvent>(new EntityEventRefHandler<GiveHandsComponent, MapInitEvent>(this.OnXenoHandsMapInit));
    this.SubscribeLocalEvent<WhitelistPickupByComponent, GettingPickedUpAttemptEvent>(new EntityEventRefHandler<WhitelistPickupByComponent, GettingPickedUpAttemptEvent>(this.OnWhitelistGettingPickedUpAttempt));
    this.SubscribeLocalEvent<CartridgeAmmoComponent, GettingPickedUpAttemptEvent>(new EntityEventRefHandler<CartridgeAmmoComponent, GettingPickedUpAttemptEvent>(this.OnSpentCasingPickup));
    this.SubscribeLocalEvent<WhitelistPickupComponent, PickupAttemptEvent>(new EntityEventRefHandler<WhitelistPickupComponent, PickupAttemptEvent>(this.OnWhitelistPickUpAttempt));
    this.SubscribeLocalEvent<DropHeldOnIncapacitateComponent, MobStateChangedEvent>(new EntityEventRefHandler<DropHeldOnIncapacitateComponent, MobStateChangedEvent>(this.OnDropMobStateChanged));
    this.SubscribeLocalEvent<RMCStorageEjectHandComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<RMCStorageEjectHandComponent, GetVerbsEvent<AlternativeVerb>>(this.OnStorageEjectHandVerbs));
    this.SubscribeLocalEvent<DropOnUseInHandComponent, UseInHandEvent>(new EntityEventRefHandler<DropOnUseInHandComponent, UseInHandEvent>(this.OnDropOnUseInHand));
  }

  private void OnXenoHandsMapInit(Entity<GiveHandsComponent> ent, ref MapInitEvent args)
  {
    foreach (GivenHand hand in ent.Comp.Hands)
      this._hands.AddHand((Entity<HandsComponent>) ent.Owner, hand.Name, hand.Location);
  }

  private void OnWhitelistGettingPickedUpAttempt(
    Entity<WhitelistPickupByComponent> ent,
    ref GettingPickedUpAttemptEvent args)
  {
    if (args.Cancelled || this._whitelist.IsValid(ent.Comp.Whitelist, args.User))
      return;
    args.Cancel();
  }

  private void OnSpentCasingPickup(
    Entity<CartridgeAmmoComponent> ent,
    ref GettingPickedUpAttemptEvent args)
  {
    if (args.Cancelled || !ent.Comp.Spent)
      return;
    args.Cancel();
  }

  private void OnWhitelistPickUpAttempt(
    Entity<WhitelistPickupComponent> ent,
    ref PickupAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    if (!this._whitelist.IsValid(ent.Comp.Whitelist, args.Item))
    {
      args.Cancel();
    }
    else
    {
      if (ent.Comp.AllowDead || !this._mobState.IsDead(args.Item))
        return;
      args.Cancel();
    }
  }

  private void OnDropMobStateChanged(
    Entity<DropHeldOnIncapacitateComponent> ent,
    ref MobStateChangedEvent args)
  {
    HandsComponent comp;
    if (args.OldMobState != MobState.Alive || args.NewMobState <= MobState.Alive || !this.TryComp<HandsComponent>((EntityUid) ent, out comp))
      return;
    foreach (string key in comp.Hands.Keys)
      this._hands.TryDrop((Entity<HandsComponent>) ((EntityUid) ent, comp), key, checkActionBlocker: false);
  }

  private void OnStorageEjectHandVerbs(
    Entity<RMCStorageEjectHandComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanInteract)
      return;
    EntityUid user = args.User;
    if (!ent.Comp.CanToggleStorage || this._container.GetContainingContainers((Entity<TransformComponent>) ent.Owner).All<BaseContainer>((Func<BaseContainer, bool>) (c => c.Owner != user)))
      return;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Text = this.Loc.GetString("rmc-storage-hand-switch");
    alternativeVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/flip.svg.192dpi.png"));
    alternativeVerb1.Priority = -2;
    alternativeVerb1.Act = (Action) (() =>
    {
      ent.Comp.State = RMCHandsSystem.GetNextState(ent.Comp.State);
      this.Dirty<RMCStorageEjectHandComponent>(ent);
      string messageId;
      switch (ent.Comp.State)
      {
        case RMCStorageEjectState.Last:
          messageId = "rmc-storage-hand-eject-last-item";
          break;
        case RMCStorageEjectState.First:
          messageId = "rmc-storage-hand-eject-first-item";
          break;
        case RMCStorageEjectState.Unequip:
          messageId = "rmc-storage-hand-eject-unequips";
          break;
        case RMCStorageEjectState.Open:
          messageId = "rmc-storage-hand-eject-open";
          break;
        default:
          messageId = string.Empty;
          break;
      }
      this._popup.PopupClient(this.Loc.GetString(messageId, ("storage", (object) ent.Owner)), user, new EntityUid?(user), PopupType.Medium);
    });
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
    BaseContainer container;
    SlotDefinition slot;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null), out container) || container.Owner != user || !this._inventory.TryGetContainingSlot((Entity<TransformComponent, MetaDataComponent>) ent.Owner, out slot))
      return;
    AlternativeVerb alternativeVerb3 = new AlternativeVerb();
    alternativeVerb3.Text = "Unequip";
    alternativeVerb3.Act = (Action) (() =>
    {
      if (!this._inventory.TryGetContainingSlot((Entity<TransformComponent, MetaDataComponent>) ent.Owner, out slot) || !this._inventory.TryUnequip(user, user, slot.Name, checkDoafter: true))
        return;
      this._hands.TryPickupAnyHand(user, ent.Owner);
    });
    AlternativeVerb alternativeVerb4 = alternativeVerb3;
    args.Verbs.Add(alternativeVerb4);
  }

  private static RMCStorageEjectState GetNextState(RMCStorageEjectState current)
  {
    return (RMCStorageEjectState) ((uint) (current + (byte) 1) % (uint) Enum.GetValues<RMCStorageEjectState>().Length);
  }

  private void OnDropOnUseInHand(Entity<DropOnUseInHandComponent> ent, ref UseInHandEvent args)
  {
    this._hands.TryDrop((Entity<HandsComponent>) args.User, (EntityUid) ent);
  }

  public bool IsPickupByAllowed(
    Entity<WhitelistPickupByComponent?> item,
    Entity<WhitelistPickupComponent?> user)
  {
    this.Resolve<WhitelistPickupByComponent>((EntityUid) item, ref item.Comp, false);
    this.Resolve<WhitelistPickupComponent>((EntityUid) user, ref user.Comp, false);
    if (item.Comp != null && !this._whitelist.IsValid(item.Comp.Whitelist, (EntityUid) user) || user.Comp != null && !this._whitelist.IsValid(user.Comp.Whitelist, item.Owner))
      return false;
    WhitelistPickupComponent comp = user.Comp;
    return comp == null || comp.AllowDead || !this._mobState.IsDead((EntityUid) item);
  }

  public bool TryGetHolder(EntityUid item, out EntityUid user)
  {
    user = new EntityUid();
    BaseContainer container;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (item, (TransformComponent) null), out container) || !this._hands.IsHolding((Entity<HandsComponent>) container.Owner, new EntityUid?(item)))
      return false;
    user = container.Owner;
    return true;
  }

  public bool TryGetNestedStorageParent(EntityUid item, out EntityUid user)
  {
    user = new EntityUid();
    BaseContainer container;
    StorageComponent comp;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (item, (TransformComponent) null), out container) || !this.TryComp<StorageComponent>(container.Owner, out comp) || !comp.StoredItems.ContainsKey(item))
      return false;
    user = container.Owner;
    return true;
  }

  public bool TryStorageEjectHand(EntityUid user, string handName)
  {
    if (this._hands.TryGetHand((Entity<HandsComponent>) user, handName, out Hand? _))
    {
      EntityUid? heldItem = this._hands.GetHeldItem((Entity<HandsComponent>) user, handName);
      if (heldItem.HasValue)
      {
        EntityUid valueOrDefault = heldItem.GetValueOrDefault();
        return this.HasComp<InteractionActivateOnClickComponent>(valueOrDefault) && this._interaction.InteractionActivate(user, valueOrDefault) || this.TryStorageEjectHand(user, valueOrDefault);
      }
    }
    return false;
  }

  public bool TryStorageEjectHand(EntityUid user, EntityUid item)
  {
    RMCStorageEjectHandItemEvent args = new RMCStorageEjectHandItemEvent(user);
    this.RaiseLocalEvent<RMCStorageEjectHandItemEvent>(item, ref args);
    if (args.Handled)
      return true;
    RMCStorageEjectHandComponent comp1;
    StorageComponent comp2;
    EntityUid user1;
    if (!this.TryComp<RMCStorageEjectHandComponent>(item, out comp1) || !this.TryComp<StorageComponent>(item, out comp2) || comp1.NestedWhitelist != null && (!this.TryGetNestedStorageParent(item, out user1) || !this._whitelist.IsWhitelistPass(comp1.NestedWhitelist, user1)))
      return false;
    switch (comp1.State)
    {
      case RMCStorageEjectState.Unequip:
        return false;
      case RMCStorageEjectState.Open:
        this._storage.OpenStorageUI(item, user, comp2, false);
        return true;
      default:
        LocId popup;
        if (!this._rmcStorage.CanEject(item, user, out popup))
        {
          this._popup.PopupClient((string) popup, user, new EntityUid?(user), PopupType.SmallCaution);
          return false;
        }
        if (comp1.Whitelist != null)
        {
          foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) comp2.Container.ContainedEntities)
          {
            if (this._whitelist.IsWhitelistPass(comp1.Whitelist, containedEntity))
            {
              this._hands.TryPickupAnyHand(user, containedEntity);
              return true;
            }
          }
        }
        EntityUid? nullable = new EntityUid?();
        switch (comp1.State)
        {
          case RMCStorageEjectState.Last:
            EntityUid entityUid1;
            if (this._rmcStorage.TryGetLastItem((Entity<StorageComponent>) (item, comp2), out entityUid1))
            {
              nullable = new EntityUid?(entityUid1);
              break;
            }
            if (comp1.EjectWhenEmpty)
              return false;
            this._popup.PopupClient(this.Loc.GetString("rmc-storage-nothing-left", ("storage", (object) item)), user, new EntityUid?(user));
            return true;
          case RMCStorageEjectState.First:
            EntityUid entityUid2;
            if (this._rmcStorage.TryGetFirstItem((Entity<StorageComponent>) (item, comp2), out entityUid2))
            {
              nullable = new EntityUid?(entityUid2);
              break;
            }
            if (comp1.EjectWhenEmpty)
              return false;
            this._popup.PopupClient(this.Loc.GetString("rmc-storage-nothing-left", ("storage", (object) item)), user, new EntityUid?(user));
            return true;
        }
        if (!nullable.HasValue)
          return false;
        this._hands.TryPickupAnyHand(user, nullable.Value);
        return true;
    }
  }

  public virtual void ThrowHeldItem(
    EntityUid player,
    EntityCoordinates coordinates,
    float minDistance = 0.1f)
  {
  }
}
