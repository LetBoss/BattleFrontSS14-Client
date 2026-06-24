// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Clothing.RMCClothingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.UniformAccessories;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared._RMC14.Clothing;

public sealed class RMCClothingSystem : EntitySystem
{
  [Dependency]
  private ClothingSystem _clothing;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedUniformAccessorySystem _uniformAccessories;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  private Robust.Shared.GameObjects.EntityQuery<ClothingLimitComponent> _clothingLimitQuery;

  public override void Initialize()
  {
    this._clothingLimitQuery = this.GetEntityQuery<ClothingLimitComponent>();
    this.SubscribeLocalEvent<ClothingLimitComponent, BeingEquippedAttemptEvent>(new EntityEventRefHandler<ClothingLimitComponent, BeingEquippedAttemptEvent>(this.OnClothingLimitBeingEquippedAttempt));
    this.SubscribeLocalEvent<ClothingRequireEquippedComponent, BeingEquippedAttemptEvent>(new EntityEventRefHandler<ClothingRequireEquippedComponent, BeingEquippedAttemptEvent>(this.OnRequireEquippedBeingEquippedAttempt));
    this.SubscribeLocalEvent<ClothingComponent, DroppedEvent>(new EntityEventRefHandler<ClothingComponent, DroppedEvent>(this.OnDropped));
    this.SubscribeLocalEvent<NoClothingSlowdownComponent, ComponentStartup>(new EntityEventRefHandler<NoClothingSlowdownComponent, ComponentStartup>(this.OnNoClothingSlowUpdate<ComponentStartup>));
    this.SubscribeLocalEvent<NoClothingSlowdownComponent, DidEquipEvent>(new EntityEventRefHandler<NoClothingSlowdownComponent, DidEquipEvent>(this.OnNoClothingSlowUpdate<DidEquipEvent>));
    this.SubscribeLocalEvent<NoClothingSlowdownComponent, DidUnequipEvent>(new EntityEventRefHandler<NoClothingSlowdownComponent, DidUnequipEvent>(this.OnNoClothingSlowUpdate<DidUnequipEvent>));
    this.SubscribeLocalEvent<NoClothingSlowdownComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<NoClothingSlowdownComponent, RefreshMovementSpeedModifiersEvent>(this.OnNoClothingSlowRefresh));
    this.SubscribeLocalEvent<RMCClothingFoldableComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<RMCClothingFoldableComponent, GetVerbsEvent<AlternativeVerb>>(this.AddFoldVerb));
  }

  private void OnClothingLimitBeingEquippedAttempt(
    Entity<ClothingLimitComponent> ent,
    ref BeingEquippedAttemptEvent args)
  {
    if (args.Cancelled || (args.SlotFlags & ent.Comp.Slot) == SlotFlags.NONE)
      return;
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) args.EquipTarget, ent.Comp.Slot);
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      ClothingLimitComponent component;
      if (this._clothingLimitQuery.TryComp(container.ContainedEntity, out component) && component.Id == ent.Comp.Id)
      {
        args.Reason = "rmc-clothing-limit";
        args.Cancel();
      }
    }
  }

  private void OnNoClothingSlowUpdate<T>(Entity<NoClothingSlowdownComponent> ent, ref T args) where T : EntityEventArgs
  {
    ent.Comp.Active = !this._inventory.TryGetSlotEntity((EntityUid) ent, ent.Comp.Slot, out EntityUid? _);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) ent);
  }

  private void OnNoClothingSlowRefresh(
    Entity<NoClothingSlowdownComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    if (!ent.Comp.Active)
      return;
    args.ModifySpeed(ent.Comp.WalkModifier, ent.Comp.SprintModifier);
  }

  private void OnRequireEquippedBeingEquippedAttempt(
    Entity<ClothingRequireEquippedComponent> ent,
    ref BeingEquippedAttemptEvent args)
  {
    if (args.Cancelled || this.HasEquippedItemsWithinWhitelist(args.EquipTarget, ent.Comp.Whitelist))
      return;
    args.Cancel();
    this._popup.PopupClient(this.Loc.GetString(ent.Comp.DenyReason), args.EquipTarget, new EntityUid?(args.EquipTarget), PopupType.SmallCaution);
  }

  private void OnDropped(Entity<ClothingComponent> ent, ref DroppedEvent args)
  {
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) args.User);
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      EntityUid? containedEntity = container.ContainedEntity;
      ClothingRequireEquippedComponent comp;
      if (containedEntity.HasValue && this.TryComp<ClothingRequireEquippedComponent>(containedEntity.GetValueOrDefault(), out comp) && comp.AutoUnequip && this._whitelist.IsWhitelistPassOrNull(comp.Whitelist, ent.Owner) && !this.HasEquippedItemsWithinWhitelist(args.User, comp.Whitelist))
        this._inventory.TryUnequip(args.User, container.ID);
    }
  }

  private bool HasEquippedItemsWithinWhitelist(EntityUid uid, EntityWhitelist? whitelist)
  {
    foreach (EntityUid uid1 in this._hands.EnumerateHeld((Entity<HandsComponent>) uid))
    {
      if (this._whitelist.IsWhitelistPassOrNull(whitelist, uid1))
        return true;
    }
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) uid);
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      EntityUid? containedEntity = container.ContainedEntity;
      if (containedEntity.HasValue)
      {
        EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
        if (this._whitelist.IsWhitelistPassOrNull(whitelist, valueOrDefault))
          return true;
      }
    }
    return false;
  }

  private void AddFoldVerb(
    Entity<RMCClothingFoldableComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null)
      return;
    EntityUid user = args.User;
    foreach (FoldableType type1 in ent.Comp.Types)
    {
      FoldableType type = type1;
      AlternativeVerb alternativeVerb1 = new AlternativeVerb();
      alternativeVerb1.Act = (Action) (() => this.TryToggleFold(ent, type, new EntityUid?(user)));
      alternativeVerb1.Text = this.Loc.GetString((string) type.Name);
      alternativeVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/fold.svg.192dpi.png"));
      alternativeVerb1.Priority = type.Priority;
      AlternativeVerb alternativeVerb2 = alternativeVerb1;
      args.Verbs.Add(alternativeVerb2);
    }
  }

  public void TryToggleFold(
    Entity<RMCClothingFoldableComponent> ent,
    FoldableType type,
    EntityUid? user)
  {
    if (type.Prefix == ent.Comp.ActivatedPrefix)
      this.SetPrefix(ent, (string) null, false);
    else if (type.BlacklistedPrefix == ent.Comp.ActivatedPrefix && ent.Comp.ActivatedPrefix != null)
    {
      if (!type.BlacklistPopup.HasValue || !user.HasValue)
        return;
      ILocalizationManager loc = this.Loc;
      LocId? blacklistPopup = type.BlacklistPopup;
      string valueOrDefault = blacklistPopup.HasValue ? (string) blacklistPopup.GetValueOrDefault() : (string) null;
      this._popup.PopupClient(loc.GetString(valueOrDefault), user.Value, new EntityUid?(user.Value), PopupType.SmallCaution);
    }
    else
      this.SetPrefix(ent, type.Prefix, type.HideAccessories);
  }

  public void SetPrefix(
    Entity<RMCClothingFoldableComponent> ent,
    string? prefix,
    bool hideAccessories)
  {
    ent.Comp.ActivatedPrefix = prefix;
    this.Dirty<RMCClothingFoldableComponent>(ent);
    this._clothing.SetEquippedPrefix(ent.Owner, ent.Comp.ActivatedPrefix);
    this._uniformAccessories.SetAccessoriesHidden(ent.Owner, hideAccessories);
  }
}
