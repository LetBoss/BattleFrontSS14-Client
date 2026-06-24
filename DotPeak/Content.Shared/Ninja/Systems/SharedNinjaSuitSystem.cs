// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Systems.SharedNinjaSuitSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Clothing;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory.Events;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Ninja.Components;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Ninja.Systems;

public abstract class SharedNinjaSuitSystem : EntitySystem
{
  [Dependency]
  private ActionContainerSystem _actionContainer;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private ItemToggleSystem _toggle;
  [Dependency]
  protected SharedPopupSystem Popup;
  [Dependency]
  private SharedSpaceNinjaSystem _ninja;
  [Dependency]
  private UseDelaySystem _useDelay;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<NinjaSuitComponent, MapInitEvent>(new EntityEventRefHandler<NinjaSuitComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<NinjaSuitComponent, ClothingGotEquippedEvent>(new EntityEventRefHandler<NinjaSuitComponent, ClothingGotEquippedEvent>(this.OnEquipped));
    this.SubscribeLocalEvent<NinjaSuitComponent, GetItemActionsEvent>(new EntityEventRefHandler<NinjaSuitComponent, GetItemActionsEvent>(this.OnGetItemActions));
    this.SubscribeLocalEvent<NinjaSuitComponent, ToggleClothingCheckEvent>(new EntityEventRefHandler<NinjaSuitComponent, ToggleClothingCheckEvent>(this.OnCloakCheck));
    this.SubscribeLocalEvent<NinjaSuitComponent, CheckItemCreatorEvent>(new EntityEventRefHandler<NinjaSuitComponent, CheckItemCreatorEvent>(this.OnStarCheck));
    this.SubscribeLocalEvent<NinjaSuitComponent, CreateItemAttemptEvent>(new EntityEventRefHandler<NinjaSuitComponent, CreateItemAttemptEvent>(this.OnCreateStarAttempt));
    this.SubscribeLocalEvent<NinjaSuitComponent, ItemToggleActivateAttemptEvent>(new EntityEventRefHandler<NinjaSuitComponent, ItemToggleActivateAttemptEvent>(this.OnActivateAttempt));
    this.SubscribeLocalEvent<NinjaSuitComponent, GotUnequippedEvent>(new EntityEventRefHandler<NinjaSuitComponent, GotUnequippedEvent>(this.OnUnequipped));
  }

  private void OnEquipped(Entity<NinjaSuitComponent> ent, ref ClothingGotEquippedEvent args)
  {
    EntityUid wearer = args.Wearer;
    SpaceNinjaComponent component;
    if (!this._ninja.NinjaQuery.TryComp(wearer, out component))
      return;
    this.NinjaEquipped(ent, (Entity<SpaceNinjaComponent>) (wearer, component));
  }

  protected virtual void NinjaEquipped(
    Entity<NinjaSuitComponent> ent,
    Entity<SpaceNinjaComponent> user)
  {
    this._ninja.AssignSuit(user, new EntityUid?((EntityUid) ent));
  }

  private void OnMapInit(Entity<NinjaSuitComponent> ent, ref MapInitEvent args)
  {
    (EntityUid entityUid, NinjaSuitComponent comp) = ent;
    this._actionContainer.EnsureAction(entityUid, ref comp.RecallKatanaActionEntity, (string) comp.RecallKatanaAction);
    this._actionContainer.EnsureAction(entityUid, ref comp.EmpActionEntity, (string) comp.EmpAction);
    this.Dirty(entityUid, (IComponent) comp);
  }

  private void OnGetItemActions(Entity<NinjaSuitComponent> ent, ref GetItemActionsEvent args)
  {
    if (!this._ninja.IsNinja(new EntityUid?(args.User)))
      return;
    NinjaSuitComponent comp = ent.Comp;
    args.AddAction(ref comp.RecallKatanaActionEntity, (string) comp.RecallKatanaAction);
    args.AddAction(ref comp.EmpActionEntity, (string) comp.EmpAction);
  }

  private void OnCloakCheck(Entity<NinjaSuitComponent> ent, ref ToggleClothingCheckEvent args)
  {
    if (this._ninja.IsNinja(new EntityUid?(args.User)))
      return;
    args.Cancelled = true;
  }

  private void OnStarCheck(Entity<NinjaSuitComponent> ent, ref CheckItemCreatorEvent args)
  {
    if (this._ninja.IsNinja(new EntityUid?(args.User)))
      return;
    args.Cancelled = true;
  }

  private void OnCreateStarAttempt(Entity<NinjaSuitComponent> ent, ref CreateItemAttemptEvent args)
  {
    if (!this.CheckDisabled(ent, args.User))
      return;
    args.Cancelled = true;
  }

  private void OnUnequipped(Entity<NinjaSuitComponent> ent, ref GotUnequippedEvent args)
  {
    EntityUid equipee = args.Equipee;
    SpaceNinjaComponent component;
    if (!this._ninja.NinjaQuery.TryComp(equipee, out component))
      return;
    this.UserUnequippedSuit(ent, (Entity<SpaceNinjaComponent>) (equipee, component));
  }

  public void RevealNinja(Entity<NinjaSuitComponent?> ent, EntityUid user, bool disable = true)
  {
    if (!this.Resolve<NinjaSuitComponent>((EntityUid) ent, ref ent.Comp))
      return;
    EntityUid owner = ent.Owner;
    NinjaSuitComponent comp = ent.Comp;
    if (this._toggle.TryDeactivate((Entity<ItemToggleComponent>) owner, new EntityUid?(user)) || !disable)
      return;
    this._audio.PlayPredicted(comp.RevealSound, owner, new EntityUid?(user));
    this.Popup.PopupClient(this.Loc.GetString("ninja-revealed"), user, new EntityUid?(user), PopupType.MediumCaution);
    this._useDelay.TryResetDelay(owner, id: comp.DisableDelayId);
  }

  private void OnActivateAttempt(
    Entity<NinjaSuitComponent> ent,
    ref ItemToggleActivateAttemptEvent args)
  {
    if (!this._ninja.IsNinja(args.User))
    {
      args.Cancelled = true;
    }
    else
    {
      if (!this.IsDisabled((Entity<NinjaSuitComponent, UseDelayComponent>) ((EntityUid) ent, ent.Comp, (UseDelayComponent) null)))
        return;
      args.Cancelled = true;
      args.Popup = this.Loc.GetString("ninja-suit-cooldown");
    }
  }

  public bool IsDisabled(Entity<NinjaSuitComponent?, UseDelayComponent?> ent)
  {
    return this.Resolve<NinjaSuitComponent, UseDelayComponent>((EntityUid) ent, ref ent.Comp1, ref ent.Comp2) && this._useDelay.IsDelayed((Entity<UseDelayComponent>) ((EntityUid) ent, ent.Comp2), ent.Comp1.DisableDelayId);
  }

  protected bool CheckDisabled(Entity<NinjaSuitComponent> ent, EntityUid user)
  {
    if (!this.IsDisabled((Entity<NinjaSuitComponent, UseDelayComponent>) ((EntityUid) ent, ent.Comp, (UseDelayComponent) null)))
      return false;
    this.Popup.PopupEntity(this.Loc.GetString("ninja-suit-cooldown"), user, user, PopupType.Medium);
    return true;
  }

  protected virtual void UserUnequippedSuit(
    Entity<NinjaSuitComponent> ent,
    Entity<SpaceNinjaComponent> user)
  {
    this._ninja.AssignSuit(user, new EntityUid?());
    EntityUid? gloves = user.Comp.Gloves;
    if (!gloves.HasValue)
      return;
    this._toggle.TryDeactivate((Entity<ItemToggleComponent>) gloves.GetValueOrDefault(), new EntityUid?((EntityUid) user));
  }
}
