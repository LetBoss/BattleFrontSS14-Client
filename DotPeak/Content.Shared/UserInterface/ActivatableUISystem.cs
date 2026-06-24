// Decompiled with JetBrains decompiler
// Type: Content.Shared.UserInterface.ActivatableUISystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Administration.Managers;
using Content.Shared.Ghost;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Popups;
using Content.Shared.PowerCell;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.UserInterface;

public sealed class ActivatableUISystem : EntitySystem
{
  [Dependency]
  private ISharedAdminManager _adminManager;
  [Dependency]
  private ActionBlockerSystem _blockerSystem;
  [Dependency]
  private SharedUserInterfaceSystem _uiSystem;
  [Dependency]
  private SharedPopupSystem _popupSystem;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  private ItemToggleSystem _toggle;
  [Dependency]
  private SharedPowerCellSystem _cell;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ActivatableUIComponent, ComponentStartup>(new EntityEventRefHandler<ActivatableUIComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<ActivatableUIComponent, UseInHandEvent>(new ComponentEventHandler<ActivatableUIComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<ActivatableUIComponent, ActivateInWorldEvent>(new ComponentEventHandler<ActivatableUIComponent, ActivateInWorldEvent>(this.OnActivate));
    this.SubscribeLocalEvent<ActivatableUIComponent, InteractUsingEvent>(new ComponentEventHandler<ActivatableUIComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<ActivatableUIComponent, HandDeselectedEvent>(new EntityEventRefHandler<ActivatableUIComponent, HandDeselectedEvent>(this.OnHandDeselected));
    this.SubscribeLocalEvent<ActivatableUIComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<ActivatableUIComponent, GotUnequippedHandEvent>(this.OnHandUnequipped));
    this.SubscribeLocalEvent<ActivatableUIComponent, BoundUIClosedEvent>(new ComponentEventHandler<ActivatableUIComponent, BoundUIClosedEvent>(this.OnUIClose));
    this.SubscribeLocalEvent<ActivatableUIComponent, GetVerbsEvent<ActivationVerb>>(new ComponentEventHandler<ActivatableUIComponent, GetVerbsEvent<ActivationVerb>>(this.GetActivationVerb));
    this.SubscribeLocalEvent<ActivatableUIComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<ActivatableUIComponent, GetVerbsEvent<Verb>>(this.GetVerb));
    this.SubscribeLocalEvent<UserInterfaceComponent, OpenUiActionEvent>(new ComponentEventHandler<UserInterfaceComponent, OpenUiActionEvent>(this.OnActionPerform));
    this.InitializePower();
  }

  private void OnStartup(Entity<ActivatableUIComponent> ent, ref ComponentStartup args)
  {
    if (ent.Comp.Key != null)
      return;
    this.Log.Error($"Missing UI Key for entity: {this.ToPrettyString(new EntityUid?((EntityUid) ent))}");
  }

  private void OnActionPerform(
    EntityUid uid,
    UserInterfaceComponent component,
    OpenUiActionEvent args)
  {
    if (args.Handled || args.Key == null)
      return;
    args.Handled = this._uiSystem.TryToggleUi((Entity<UserInterfaceComponent>) uid, args.Key, args.Performer);
  }

  private void GetActivationVerb(
    EntityUid uid,
    ActivatableUIComponent component,
    GetVerbsEvent<ActivationVerb> args)
  {
    if (component.VerbOnly || !this.ShouldAddVerb<ActivationVerb>(uid, component, args))
      return;
    SortedSet<ActivationVerb> verbs = args.Verbs;
    ActivationVerb activationVerb = new ActivationVerb();
    activationVerb.Act = (Action) (() => this.InteractUI(args.User, uid, component));
    activationVerb.Text = this.Loc.GetString((string) component.VerbText);
    activationVerb.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/settings.svg.192dpi.png"));
    verbs.Add(activationVerb);
  }

  private void GetVerb(EntityUid uid, ActivatableUIComponent component, GetVerbsEvent<Verb> args)
  {
    if (!component.VerbOnly || !this.ShouldAddVerb<Verb>(uid, component, args))
      return;
    args.Verbs.Add(new Verb()
    {
      Act = (Action) (() => this.InteractUI(args.User, uid, component)),
      Text = this.Loc.GetString((string) component.VerbText),
      Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/settings.svg.192dpi.png"))
    });
  }

  private bool ShouldAddVerb<T>(
    EntityUid uid,
    ActivatableUIComponent component,
    GetVerbsEvent<T> args)
    where T : Verb
  {
    string inHand;
    if (!args.CanAccess || this._whitelistSystem.IsWhitelistFail(component.RequiredItems, args.Using.GetValueOrDefault()) || component.RequiresComplex && (args.Hands == null || component.InHandsOnly && (!this._hands.IsHolding((Entity<HandsComponent>) (args.User, args.Hands), new EntityUid?(uid), out inHand) || component.RequireActiveHand && args.Hands.ActiveHandId != inHand)))
      return false;
    if (args.CanInteract)
      return true;
    return this.HasComp<GhostComponent>(args.User) && !component.BlockSpectators;
  }

  private void OnUseInHand(EntityUid uid, ActivatableUIComponent component, UseInHandEvent args)
  {
    if (args.Handled || component.VerbOnly || component.RequiredItems != null)
      return;
    args.Handled = this.InteractUI(args.User, uid, component);
  }

  private void OnActivate(
    EntityUid uid,
    ActivatableUIComponent component,
    ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex || component.VerbOnly || component.RequiredItems != null)
      return;
    args.Handled = this.InteractUI(args.User, uid, component);
  }

  private void OnInteractUsing(
    EntityUid uid,
    ActivatableUIComponent component,
    InteractUsingEvent args)
  {
    if (args.Handled || component.VerbOnly || component.RequiredItems == null || this._whitelistSystem.IsWhitelistFail(component.RequiredItems, args.Used))
      return;
    args.Handled = this.InteractUI(args.User, uid, component);
  }

  private void OnUIClose(EntityUid uid, ActivatableUIComponent component, BoundUIClosedEvent args)
  {
    EntityUid actor = args.Actor;
    EntityUid? currentSingleUser = component.CurrentSingleUser;
    if ((currentSingleUser.HasValue ? (actor != currentSingleUser.GetValueOrDefault() ? 1 : 0) : 1) != 0 || !object.Equals((object) args.UiKey, (object) component.Key))
      return;
    this.SetCurrentSingleUser(uid, new EntityUid?(), component);
  }

  private bool InteractUI(EntityUid user, EntityUid uiEntity, ActivatableUIComponent aui)
  {
    if (aui.Key == null || !this._uiSystem.HasUi(uiEntity, aui.Key))
      return false;
    if (this._uiSystem.IsUiOpen((Entity<UserInterfaceComponent>) uiEntity, aui.Key, user))
    {
      this._uiSystem.CloseUi((Entity<UserInterfaceComponent>) uiEntity, aui.Key, new EntityUid?(user));
      return true;
    }
    HandsComponent comp;
    string inHand;
    if (!this._blockerSystem.CanInteract(user, new EntityUid?(uiEntity)) && (!this.HasComp<GhostComponent>(user) || aui.BlockSpectators) || aui.RequiresComplex && !this._blockerSystem.CanComplexInteract(user) || aui.InHandsOnly && (!this.TryComp<HandsComponent>(user, out comp) || !this._hands.IsHolding((Entity<HandsComponent>) (user, comp), new EntityUid?(uiEntity), out inHand) || aui.RequireActiveHand && comp.ActiveHandId != inHand) || aui.AdminOnly && !this._adminManager.IsAdmin(user))
      return false;
    if (aui.SingleUser && aui.CurrentSingleUser.HasValue)
    {
      EntityUid entityUid = user;
      EntityUid? currentSingleUser = aui.CurrentSingleUser;
      if ((currentSingleUser.HasValue ? (entityUid != currentSingleUser.GetValueOrDefault() ? 1 : 0) : 1) != 0)
      {
        this._popupSystem.PopupClient(this.Loc.GetString("machine-already-in-use", ("machine", (object) uiEntity)), uiEntity, new EntityUid?(user));
        if (this._uiSystem.IsUiOpen((Entity<UserInterfaceComponent>) uiEntity, aui.Key))
          return true;
        this.Log.Error($"Activatable UI has user without being opened? Entity: {this.ToPrettyString((Entity<MetaDataComponent>) uiEntity)}. User: {aui.CurrentSingleUser}, Key: {aui.Key}");
      }
    }
    ActivatableUIOpenAttemptEvent args1 = new ActivatableUIOpenAttemptEvent(user);
    UserOpenActivatableUIAttemptEvent args2 = new UserOpenActivatableUIAttemptEvent(user, uiEntity);
    this.RaiseLocalEvent<UserOpenActivatableUIAttemptEvent>(user, args2);
    this.RaiseLocalEvent<ActivatableUIOpenAttemptEvent>(uiEntity, args1);
    if (args1.Cancelled || args2.Cancelled)
      return false;
    BeforeActivatableUIOpenEvent args3 = new BeforeActivatableUIOpenEvent(user);
    this.RaiseLocalEvent<BeforeActivatableUIOpenEvent>(uiEntity, args3);
    this.SetCurrentSingleUser(uiEntity, new EntityUid?(user), aui);
    this._uiSystem.OpenUi((Entity<UserInterfaceComponent>) uiEntity, aui.Key, new EntityUid?(user));
    AfterActivatableUIOpenEvent args4 = new AfterActivatableUIOpenEvent(user, user);
    this.RaiseLocalEvent<AfterActivatableUIOpenEvent>(uiEntity, args4);
    return true;
  }

  public void SetCurrentSingleUser(EntityUid uid, EntityUid? user, ActivatableUIComponent? aui = null)
  {
    if (!this.Resolve<ActivatableUIComponent>(uid, ref aui) || !aui.SingleUser)
      return;
    aui.CurrentSingleUser = user;
    this.Dirty(uid, (IComponent) aui);
    this.RaiseLocalEvent<ActivatableUIPlayerChangedEvent>(uid, new ActivatableUIPlayerChangedEvent());
  }

  public void CloseAll(EntityUid uid, ActivatableUIComponent? aui = null)
  {
    if (!this.Resolve<ActivatableUIComponent>(uid, ref aui, false))
      return;
    if (aui.Key == null)
      this.Log.Error($"Encountered null key in activatable ui on entity {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
    else
      this._uiSystem.CloseUi((Entity<UserInterfaceComponent>) uid, aui.Key);
  }

  private void OnHandDeselected(Entity<ActivatableUIComponent> ent, ref HandDeselectedEvent args)
  {
    if (!ent.Comp.InHandsOnly || !ent.Comp.RequireActiveHand)
      return;
    this.CloseAll((EntityUid) ent, (ActivatableUIComponent) ent);
  }

  private void OnHandUnequipped(Entity<ActivatableUIComponent> ent, ref GotUnequippedHandEvent args)
  {
    if (!ent.Comp.InHandsOnly)
      return;
    this.CloseAll((EntityUid) ent, (ActivatableUIComponent) ent);
  }

  private void InitializePower()
  {
    this.SubscribeLocalEvent<ActivatableUIRequiresPowerCellComponent, ActivatableUIOpenAttemptEvent>(new ComponentEventHandler<ActivatableUIRequiresPowerCellComponent, ActivatableUIOpenAttemptEvent>(this.OnBatteryOpenAttempt));
    this.SubscribeLocalEvent<ActivatableUIRequiresPowerCellComponent, BoundUIOpenedEvent>(new ComponentEventHandler<ActivatableUIRequiresPowerCellComponent, BoundUIOpenedEvent>(this.OnBatteryOpened));
    this.SubscribeLocalEvent<ActivatableUIRequiresPowerCellComponent, BoundUIClosedEvent>(new ComponentEventHandler<ActivatableUIRequiresPowerCellComponent, BoundUIClosedEvent>(this.OnBatteryClosed));
    this.SubscribeLocalEvent<ActivatableUIRequiresPowerCellComponent, ItemToggledEvent>(new EntityEventRefHandler<ActivatableUIRequiresPowerCellComponent, ItemToggledEvent>(this.OnToggled));
  }

  private void OnToggled(
    Entity<ActivatableUIRequiresPowerCellComponent> ent,
    ref ItemToggledEvent args)
  {
    ActivatableUIComponent comp;
    if (!this.TryComp<ActivatableUIComponent>((EntityUid) ent, out comp) || args.Activated)
      return;
    if (comp.Key == null)
      this.Log.Error($"Encountered null key in activatable ui on entity {this.ToPrettyString(new EntityUid?((EntityUid) ent))}");
    else
      this._uiSystem.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, comp.Key);
  }

  private void OnBatteryOpened(
    EntityUid uid,
    ActivatableUIRequiresPowerCellComponent component,
    BoundUIOpenedEvent args)
  {
    ActivatableUIComponent activatableUiComponent = this.Comp<ActivatableUIComponent>(uid);
    if (!args.UiKey.Equals((object) activatableUiComponent.Key))
      return;
    this._toggle.TryActivate((Entity<ItemToggleComponent>) uid);
  }

  private void OnBatteryClosed(
    EntityUid uid,
    ActivatableUIRequiresPowerCellComponent component,
    BoundUIClosedEvent args)
  {
    ActivatableUIComponent activatableUiComponent = this.Comp<ActivatableUIComponent>(uid);
    if (!args.UiKey.Equals((object) activatableUiComponent.Key) || this._uiSystem.IsUiOpen((Entity<UserInterfaceComponent>) uid, activatableUiComponent.Key))
      return;
    this._toggle.TryDeactivate((Entity<ItemToggleComponent>) uid);
  }

  public void CheckUsage(
    EntityUid uid,
    ActivatableUIComponent? active = null,
    ActivatableUIRequiresPowerCellComponent? component = null,
    PowerCellDrawComponent? draw = null)
  {
    if (!this.Resolve<ActivatableUIRequiresPowerCellComponent, PowerCellDrawComponent, ActivatableUIComponent>(uid, ref component, ref draw, ref active, false))
      return;
    if (active.Key == null)
    {
      this.Log.Error($"Encountered null key in activatable ui on entity {this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
    }
    else
    {
      if (this._cell.HasActivatableCharge(uid))
        return;
      this._uiSystem.CloseUi((Entity<UserInterfaceComponent>) uid, active.Key);
    }
  }

  private void OnBatteryOpenAttempt(
    EntityUid uid,
    ActivatableUIRequiresPowerCellComponent component,
    ActivatableUIOpenAttemptEvent args)
  {
    PowerCellDrawComponent comp;
    if (!this.TryComp<PowerCellDrawComponent>(uid, out comp) || !args.Cancelled && this._cell.HasActivatableCharge(uid, comp, user: new EntityUid?(args.User)) && this._cell.HasDrawCharge(uid, comp, user: new EntityUid?(args.User)))
      return;
    args.Cancel();
  }
}
