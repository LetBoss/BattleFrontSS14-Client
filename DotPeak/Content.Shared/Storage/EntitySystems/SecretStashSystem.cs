// Decompiled with JetBrains decompiler
// Type: Content.Shared.Storage.EntitySystems.SecretStashSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Construction.EntitySystems;
using Content.Shared.Damage;
using Content.Shared.Destructible;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Materials;
using Content.Shared.Nutrition;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Tools.EntitySystems;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Storage.EntitySystems;

public sealed class SecretStashSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedHandsSystem _handsSystem;
  [Dependency]
  private SharedContainerSystem _containerSystem;
  [Dependency]
  private SharedItemSystem _item;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private ToolOpenableSystem _toolOpenableSystem;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  private DamageableSystem _damageableSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SecretStashComponent, ComponentInit>(new EntityEventRefHandler<SecretStashComponent, ComponentInit>(this.OnInit));
    this.SubscribeLocalEvent<SecretStashComponent, DestructionEventArgs>(new EntityEventRefHandler<SecretStashComponent, DestructionEventArgs>(this.OnDestroyed));
    this.SubscribeLocalEvent<SecretStashComponent, GotReclaimedEvent>(new EntityEventRefHandler<SecretStashComponent, GotReclaimedEvent>(this.OnReclaimed));
    this.SubscribeLocalEvent<SecretStashComponent, InteractUsingEvent>(new EntityEventRefHandler<SecretStashComponent, InteractUsingEvent>(this.OnInteractUsing), after: new Type[2]
    {
      typeof (ToolOpenableSystem),
      typeof (AnchorableSystem)
    });
    this.SubscribeLocalEvent<SecretStashComponent, AfterFullyEatenEvent>(new EntityEventRefHandler<SecretStashComponent, AfterFullyEatenEvent>(this.OnEaten));
    this.SubscribeLocalEvent<SecretStashComponent, InteractHandEvent>(new EntityEventRefHandler<SecretStashComponent, InteractHandEvent>(this.OnInteractHand));
    this.SubscribeLocalEvent<SecretStashComponent, GetVerbsEvent<InteractionVerb>>(new EntityEventRefHandler<SecretStashComponent, GetVerbsEvent<InteractionVerb>>(this.OnGetVerb));
  }

  private void OnInit(Entity<SecretStashComponent> entity, ref ComponentInit args)
  {
    entity.Comp.ItemContainer = this._containerSystem.EnsureContainer<ContainerSlot>((EntityUid) entity, "stash", out bool _);
  }

  private void OnDestroyed(Entity<SecretStashComponent> entity, ref DestructionEventArgs args)
  {
    this.DropContentsAndAlert(entity);
  }

  private void OnReclaimed(Entity<SecretStashComponent> entity, ref GotReclaimedEvent args)
  {
    this.DropContentsAndAlert(entity, new EntityCoordinates?(args.ReclaimerCoordinates));
  }

  private void OnEaten(Entity<SecretStashComponent> entity, ref AfterFullyEatenEvent args)
  {
    DamageSpecifier damageEatenItemInside = entity.Comp.DamageEatenItemInside;
    if (!this.HasItemInside(entity) || damageEatenItemInside == null)
      return;
    this._damageableSystem.TryChangeDamage(new EntityUid?(args.User), damageEatenItemInside, true);
  }

  private void OnInteractUsing(Entity<SecretStashComponent> entity, ref InteractUsingEvent args)
  {
    if (args.Handled || !this.IsStashOpen(entity))
      return;
    args.Handled = this.TryStashItem(entity, args.User, args.Used);
  }

  private void OnInteractHand(Entity<SecretStashComponent> entity, ref InteractHandEvent args)
  {
    if (args.Handled || !this.IsStashOpen(entity))
      return;
    args.Handled = this.TryGetItem(entity, args.User);
  }

  private bool TryStashItem(
    Entity<SecretStashComponent> entity,
    EntityUid userUid,
    EntityUid itemToHideUid)
  {
    ItemComponent comp;
    if (!this.TryComp<ItemComponent>(itemToHideUid, out comp))
      return false;
    this._audio.PlayPredicted(entity.Comp.TryInsertItemSound, (EntityUid) entity, new EntityUid?(userUid), new AudioParams?(AudioParams.Default.WithVariation(new float?(0.25f))));
    ContainerSlot itemContainer = entity.Comp.ItemContainer;
    if (this.HasItemInside(entity))
    {
      this._popupSystem.PopupClient(this.Loc.GetString("comp-secret-stash-action-hide-container-not-empty"), (EntityUid) entity, new EntityUid?(userUid));
      return false;
    }
    if (this._item.GetSizePrototype(comp.Size) > this._item.GetSizePrototype(entity.Comp.MaxItemSize) || this._whitelistSystem.IsBlacklistPass(entity.Comp.Blacklist, itemToHideUid))
    {
      this._popupSystem.PopupClient(this.Loc.GetString("comp-secret-stash-action-hide-item-too-big", ("item", (object) itemToHideUid), ("stashname", (object) this.GetStashName(entity))), (EntityUid) entity, new EntityUid?(userUid));
      return false;
    }
    if (!this._handsSystem.TryDropIntoContainer((Entity<HandsComponent>) userUid, itemToHideUid, (BaseContainer) itemContainer))
      return false;
    this._popupSystem.PopupClient(this.Loc.GetString("comp-secret-stash-action-hide-success", ("item", (object) itemToHideUid), ("stashname", (object) this.GetStashName(entity))), (EntityUid) entity, new EntityUid?(userUid));
    return true;
  }

  private bool TryGetItem(Entity<SecretStashComponent> entity, EntityUid userUid)
  {
    HandsComponent comp;
    if (!this.TryComp<HandsComponent>(userUid, out comp))
      return false;
    this._audio.PlayPredicted(entity.Comp.TryRemoveItemSound, (EntityUid) entity, new EntityUid?(userUid), new AudioParams?(AudioParams.Default.WithVariation(new float?(0.25f))));
    EntityUid? containedEntity = entity.Comp.ItemContainer.ContainedEntity;
    if (!containedEntity.HasValue)
      return false;
    this._handsSystem.PickupOrDrop(new EntityUid?(userUid), containedEntity.Value, handsComp: comp);
    this._popupSystem.PopupClient(this.Loc.GetString("comp-secret-stash-action-get-item-found-something", ("stashname", (object) this.GetStashName(entity))), (EntityUid) entity, new EntityUid?(userUid));
    return true;
  }

  private void OnGetVerb(
    Entity<SecretStashComponent> entity,
    ref GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanInteract || !args.CanAccess || !entity.Comp.HasVerbs)
      return;
    EntityUid user = args.User;
    EntityUid? item = args.Using;
    string stashName = this.GetStashName(entity);
    InteractionVerb interactionVerb = new InteractionVerb();
    if (!this.IsStashOpen(entity))
      return;
    if (item.HasValue)
    {
      interactionVerb.Text = this.Loc.GetString("comp-secret-stash-verb-insert-into-stash");
      if (this.HasItemInside(entity))
      {
        interactionVerb.Disabled = true;
        interactionVerb.Message = this.Loc.GetString("comp-secret-stash-verb-insert-message-item-already-inside", ("stashname", (object) stashName));
      }
      else
        interactionVerb.Message = this.Loc.GetString("comp-secret-stash-verb-insert-message-no-item", ("item", (object) item), ("stashname", (object) stashName));
      interactionVerb.Act = (Action) (() => this.TryStashItem(entity, user, item.Value));
    }
    else
    {
      interactionVerb.Text = this.Loc.GetString("comp-secret-stash-verb-take-out-item");
      interactionVerb.Message = this.Loc.GetString("comp-secret-stash-verb-take-out-message-something", ("stashname", (object) stashName));
      if (!this.HasItemInside(entity))
      {
        interactionVerb.Disabled = true;
        interactionVerb.Message = this.Loc.GetString("comp-secret-stash-verb-take-out-message-nothing", ("stashname", (object) stashName));
      }
      interactionVerb.Act = (Action) (() => this.TryGetItem(entity, user));
    }
    args.Verbs.Add(interactionVerb);
  }

  private string GetStashName(Entity<SecretStashComponent> entity)
  {
    return entity.Comp.SecretStashName == null ? (string) Identity.Name((EntityUid) entity, (IEntityManager) this.EntityManager) : this.Loc.GetString(entity.Comp.SecretStashName);
  }

  private bool IsStashOpen(Entity<SecretStashComponent> stash)
  {
    return this._toolOpenableSystem.IsOpen((EntityUid) stash);
  }

  private bool HasItemInside(Entity<SecretStashComponent> entity)
  {
    return entity.Comp.ItemContainer.ContainedEntity.HasValue;
  }

  private void DropContentsAndAlert(Entity<SecretStashComponent> entity, EntityCoordinates? cords = null)
  {
    List<EntityUid> entityUidList = this._containerSystem.EmptyContainer((BaseContainer) entity.Comp.ItemContainer, true, cords);
    if (entityUidList == null || entityUidList.Count < 1)
      return;
    this._popupSystem.PopupPredicted(this.Loc.GetString("comp-secret-stash-on-destroyed-popup", ("stashname", (object) this.GetStashName(entity))), entityUidList[0], new EntityUid?(), PopupType.MediumCaution);
  }
}
