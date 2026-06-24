// Decompiled with JetBrains decompiler
// Type: Content.Shared.RetractableItemAction.RetractableItemActionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Cuffs;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Components;
using Content.Shared.Inventory;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;

#nullable enable
namespace Content.Shared.RetractableItemAction;

public sealed class RetractableItemActionSystem : EntitySystem
{
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedContainerSystem _containers;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedPopupSystem _popups;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RetractableItemActionComponent, MapInitEvent>(new EntityEventRefHandler<RetractableItemActionComponent, MapInitEvent>(this.OnActionInit));
    this.SubscribeLocalEvent<RetractableItemActionComponent, OnRetractableItemActionEvent>(new EntityEventRefHandler<RetractableItemActionComponent, OnRetractableItemActionEvent>(this.OnRetractableItemAction));
    this.SubscribeLocalEvent<ActionRetractableItemComponent, ComponentShutdown>(new EntityEventRefHandler<ActionRetractableItemComponent, ComponentShutdown>(this.OnActionSummonedShutdown));
    this.Subs.SubscribeWithRelay<ActionRetractableItemComponent, HeldRelayedEvent<TargetHandcuffedEvent>>(new EntityEventRefHandler<ActionRetractableItemComponent, HeldRelayedEvent<TargetHandcuffedEvent>>(this.OnItemHandcuffed), inventory: false);
  }

  private void OnActionInit(Entity<RetractableItemActionComponent> ent, ref MapInitEvent args)
  {
    this._containers.EnsureContainer<Container>((EntityUid) ent, "item-action-item-container");
    this.PopulateActionItem((Entity<RetractableItemActionComponent>) ent.Owner);
  }

  private void OnRetractableItemAction(
    Entity<RetractableItemActionComponent> ent,
    ref OnRetractableItemActionEvent args)
  {
    string activeHand = this._hands.GetActiveHand((Entity<HandsComponent>) args.Performer);
    if (activeHand == null)
      return;
    Entity<ActionComponent>? action = this._actions.GetAction(new Entity<ActionComponent>?((Entity<ActionComponent>) ent.Owner));
    if (!action.HasValue || !action.GetValueOrDefault().Comp.AttachedEntity.HasValue || !ent.Comp.ActionItemUid.HasValue)
      return;
    if (this._hands.GetActiveItem((Entity<HandsComponent>) ent.Owner).HasValue && !this._hands.IsHolding((Entity<HandsComponent>) args.Performer, ent.Comp.ActionItemUid) && !this._hands.CanDropHeld(args.Performer, activeHand, false))
    {
      this._popups.PopupClient(this.Loc.GetString("retractable-item-hand-cannot-drop"), args.Performer, new EntityUid?(args.Performer));
    }
    else
    {
      if (this._hands.IsHolding((Entity<HandsComponent>) args.Performer, ent.Comp.ActionItemUid))
        this.RetractRetractableItem(args.Performer, ent.Comp.ActionItemUid.Value, (Entity<RetractableItemActionComponent>) ent.Owner);
      else
        this.SummonRetractableItem(args.Performer, ent.Comp.ActionItemUid.Value, activeHand, (Entity<RetractableItemActionComponent>) ent.Owner);
      args.Handled = true;
    }
  }

  private void OnActionSummonedShutdown(
    Entity<ActionRetractableItemComponent> ent,
    ref ComponentShutdown args)
  {
    SharedActionsSystem actions = this._actions;
    EntityUid? nullable = ent.Comp.SummoningAction;
    Entity<ActionComponent>? action1 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    Entity<ActionComponent>? action2 = actions.GetAction(action1);
    if (!action2.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action2.GetValueOrDefault();
    RetractableItemActionComponent comp;
    if (!this.TryComp<RetractableItemActionComponent>((EntityUid) valueOrDefault, out comp))
      return;
    nullable = comp.ActionItemUid;
    EntityUid owner = ent.Owner;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() != owner ? 1 : 0) : 1) != 0)
      return;
    this.PopulateActionItem((Entity<RetractableItemActionComponent>) valueOrDefault.Owner);
  }

  private void OnItemHandcuffed(
    Entity<ActionRetractableItemComponent> ent,
    ref HeldRelayedEvent<TargetHandcuffedEvent> args)
  {
    SharedActionsSystem actions = this._actions;
    EntityUid? summoningAction = ent.Comp.SummoningAction;
    Entity<ActionComponent>? action1 = summoningAction.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) summoningAction.GetValueOrDefault()) : new Entity<ActionComponent>?();
    Entity<ActionComponent>? action2 = actions.GetAction(action1);
    if (!action2.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action2.GetValueOrDefault();
    if (!valueOrDefault.Comp.AttachedEntity.HasValue || this._hands.GetActiveHand((Entity<HandsComponent>) valueOrDefault.Comp.AttachedEntity.Value) == null)
      return;
    this.RetractRetractableItem(valueOrDefault.Comp.AttachedEntity.Value, (EntityUid) ent, (Entity<RetractableItemActionComponent>) valueOrDefault.Owner);
  }

  private void PopulateActionItem(Entity<RetractableItemActionComponent?> ent)
  {
    EntityUid? uid;
    if (!this.Resolve<RetractableItemActionComponent>(ent.Owner, ref ent.Comp, false) || this.TerminatingOrDeleted((EntityUid) ent) || !this.PredictedTrySpawnInContainer((string) ent.Comp.SpawnedPrototype, ent.Owner, "item-action-item-container", out uid))
      return;
    ent.Comp.ActionItemUid = new EntityUid?(uid.Value);
    ActionRetractableItemComponent retractableItemComponent = this.AddComp<ActionRetractableItemComponent>(uid.Value);
    retractableItemComponent.SummoningAction = new EntityUid?(ent.Owner);
    this.Dirty(uid.Value, (IComponent) retractableItemComponent);
    this.Dirty<RetractableItemActionComponent>(ent);
  }

  private void RetractRetractableItem(
    EntityUid holder,
    EntityUid item,
    Entity<RetractableItemActionComponent?> action)
  {
    if (!this.Resolve<RetractableItemActionComponent>((EntityUid) action, ref action.Comp, false))
      return;
    this.RemComp<UnremoveableComponent>(item);
    BaseContainer container = this._containers.GetContainer((EntityUid) action, "item-action-item-container");
    this._containers.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) item, container);
    this._audio.PlayPredicted((SoundSpecifier) action.Comp.RetractSounds, holder, new EntityUid?(holder));
  }

  private void SummonRetractableItem(
    EntityUid holder,
    EntityUid item,
    string hand,
    Entity<RetractableItemActionComponent?> action)
  {
    if (!this.Resolve<RetractableItemActionComponent>((EntityUid) action, ref action.Comp, false))
      return;
    this._hands.TryForcePickup((Entity<HandsComponent>) holder, item, hand, false);
    this._audio.PlayPredicted((SoundSpecifier) action.Comp.SummonSounds, holder, new EntityUid?(holder));
    this.EnsureComp<UnremoveableComponent>(item);
  }
}
