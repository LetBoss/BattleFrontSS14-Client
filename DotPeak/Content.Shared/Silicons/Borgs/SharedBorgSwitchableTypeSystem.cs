// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Borgs.SharedBorgSwitchableTypeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Silicons.Borgs.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.Silicons.Borgs;

public abstract class SharedBorgSwitchableTypeSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actionsSystem;
  [Dependency]
  private SharedUserInterfaceSystem _userInterface;
  [Dependency]
  protected IPrototypeManager Prototypes;
  [Dependency]
  private InteractionPopupSystem _interactionPopup;
  public static readonly EntProtoId ActionId = (EntProtoId) "ActionSelectBorgType";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<BorgSwitchableTypeComponent, MapInitEvent>(new EntityEventRefHandler<BorgSwitchableTypeComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<BorgSwitchableTypeComponent, ComponentShutdown>(new EntityEventRefHandler<BorgSwitchableTypeComponent, ComponentShutdown>(this.OnShutdown));
    this.SubscribeLocalEvent<BorgSwitchableTypeComponent, BorgToggleSelectTypeEvent>(new EntityEventRefHandler<BorgSwitchableTypeComponent, BorgToggleSelectTypeEvent>(this.OnSelectBorgTypeAction));
    this.Subs.BuiEvents<BorgSwitchableTypeComponent>((object) BorgSwitchableTypeUiKey.SelectBorgType, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<BorgSwitchableTypeComponent>) (sub => sub.Event<BorgSelectTypeMessage>(new EntityEventRefHandler<BorgSwitchableTypeComponent, BorgSelectTypeMessage>(this.SelectTypeMessageHandler))));
  }

  private void OnMapInit(Entity<BorgSwitchableTypeComponent> ent, ref MapInitEvent args)
  {
    this._actionsSystem.AddAction((EntityUid) ent, ref ent.Comp.SelectTypeAction, (string) SharedBorgSwitchableTypeSystem.ActionId);
    this.Dirty<BorgSwitchableTypeComponent>(ent);
    if (!ent.Comp.SelectedBorgType.HasValue)
      return;
    this.SelectBorgModule(ent, ent.Comp.SelectedBorgType.Value);
  }

  private void OnShutdown(Entity<BorgSwitchableTypeComponent> ent, ref ComponentShutdown args)
  {
    SharedActionsSystem actionsSystem = this._actionsSystem;
    Entity<ActionsComponent> owner = (Entity<ActionsComponent>) ent.Owner;
    EntityUid? selectTypeAction = ent.Comp.SelectTypeAction;
    Entity<ActionComponent>? action = selectTypeAction.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) selectTypeAction.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actionsSystem.RemoveAction(owner, action);
  }

  private void OnSelectBorgTypeAction(
    Entity<BorgSwitchableTypeComponent> ent,
    ref BorgToggleSelectTypeEvent args)
  {
    ActorComponent comp;
    if (args.Handled || !this.TryComp<ActorComponent>((EntityUid) ent, out comp))
      return;
    args.Handled = true;
    this._userInterface.TryToggleUi((Entity<UserInterfaceComponent>) (ent.Owner, (UserInterfaceComponent) null), (Enum) BorgSwitchableTypeUiKey.SelectBorgType, comp.PlayerSession);
  }

  private void SelectTypeMessageHandler(
    Entity<BorgSwitchableTypeComponent> ent,
    ref BorgSelectTypeMessage args)
  {
    if (ent.Comp.SelectedBorgType.HasValue || !this.Prototypes.HasIndex<BorgTypePrototype>(args.Prototype))
      return;
    this.SelectBorgModule(ent, args.Prototype);
  }

  protected virtual void SelectBorgModule(
    Entity<BorgSwitchableTypeComponent> ent,
    ProtoId<BorgTypePrototype> borgType)
  {
    ent.Comp.SelectedBorgType = new ProtoId<BorgTypePrototype>?(borgType);
    SharedActionsSystem actionsSystem = this._actionsSystem;
    Entity<ActionsComponent> owner = (Entity<ActionsComponent>) ent.Owner;
    EntityUid? selectTypeAction = ent.Comp.SelectTypeAction;
    Entity<ActionComponent>? action = selectTypeAction.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) selectTypeAction.GetValueOrDefault()) : new Entity<ActionComponent>?();
    actionsSystem.RemoveAction(owner, action);
    ent.Comp.SelectTypeAction = new EntityUid?();
    this.Dirty<BorgSwitchableTypeComponent>(ent);
    this._userInterface.CloseUi((Entity<UserInterfaceComponent>) (ent.Owner, (UserInterfaceComponent) null), (Enum) BorgSwitchableTypeUiKey.SelectBorgType);
    this.UpdateEntityAppearance(ent);
  }

  protected void UpdateEntityAppearance(Entity<BorgSwitchableTypeComponent> entity)
  {
    BorgTypePrototype prototype;
    if (!this.Prototypes.TryIndex<BorgTypePrototype>(entity.Comp.SelectedBorgType, out prototype))
      return;
    this.UpdateEntityAppearance(entity, prototype);
  }

  protected virtual void UpdateEntityAppearance(
    Entity<BorgSwitchableTypeComponent> entity,
    BorgTypePrototype prototype)
  {
    InteractionPopupComponent comp1;
    if (this.TryComp<InteractionPopupComponent>((EntityUid) entity, out comp1))
    {
      this._interactionPopup.SetInteractSuccessString((Entity<InteractionPopupComponent>) (entity.Owner, comp1), prototype.PetSuccessString);
      this._interactionPopup.SetInteractFailureString((Entity<InteractionPopupComponent>) (entity.Owner, comp1), prototype.PetFailureString);
    }
    FootstepModifierComponent comp2;
    if (!this.TryComp<FootstepModifierComponent>((EntityUid) entity, out comp2))
      return;
    comp2.FootstepSoundCollection = prototype.FootstepCollection;
  }
}
