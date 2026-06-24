// Decompiled with JetBrains decompiler
// Type: Content.Shared.ItemRecall.SharedItemRecallSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Player;

#nullable enable
namespace Content.Shared.ItemRecall;

public abstract class SharedItemRecallSystem : EntitySystem
{
  [Dependency]
  private ISharedPlayerManager _player;
  [Dependency]
  private SharedPvsOverrideSystem _pvs;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private MetaDataSystem _metaData;
  [Dependency]
  private SharedPopupSystem _popups;
  [Dependency]
  private SharedProjectileSystem _proj;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ItemRecallComponent, MapInitEvent>(new EntityEventRefHandler<ItemRecallComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<ItemRecallComponent, OnItemRecallActionEvent>(new EntityEventRefHandler<ItemRecallComponent, OnItemRecallActionEvent>(this.OnItemRecallActionUse));
    this.SubscribeLocalEvent<RecallMarkerComponent, ComponentShutdown>(new EntityEventRefHandler<RecallMarkerComponent, ComponentShutdown>(this.OnRecallMarkerShutdown));
  }

  private void OnMapInit(Entity<ItemRecallComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.InitialName = this.Name((EntityUid) ent);
    ent.Comp.InitialDescription = this.Description((EntityUid) ent);
  }

  private void OnItemRecallActionUse(
    Entity<ItemRecallComponent> ent,
    ref OnItemRecallActionEvent args)
  {
    if (!ent.Comp.MarkedEntity.HasValue)
    {
      HandsComponent comp;
      if (!this.TryComp<HandsComponent>(args.Performer, out comp))
        return;
      EntityUid? activeItem = this._hands.GetActiveItem((Entity<HandsComponent>) (args.Performer, comp));
      if (!activeItem.HasValue)
        this._popups.PopupClient(this.Loc.GetString("item-recall-item-mark-empty"), args.Performer, new EntityUid?(args.Performer));
      else if (this.HasComp<RecallMarkerComponent>(activeItem))
      {
        this._popups.PopupClient(this.Loc.GetString("item-recall-item-already-marked", ("item", (object) activeItem)), args.Performer, new EntityUid?(args.Performer));
      }
      else
      {
        this._popups.PopupClient(this.Loc.GetString("item-recall-item-marked", ("item", (object) activeItem.Value)), args.Performer, new EntityUid?(args.Performer));
        this.TryMarkItem(ent, activeItem.Value);
      }
    }
    else
    {
      this.RecallItem((Entity<RecallMarkerComponent>) ent.Comp.MarkedEntity.Value);
      args.Handled = true;
    }
  }

  private void RecallItem(Entity<RecallMarkerComponent?> ent)
  {
    if (!this.Resolve<RecallMarkerComponent>(ent.Owner, ref ent.Comp, false))
      return;
    SharedActionsSystem actions = this._actions;
    EntityUid? nullable = ent.Comp.MarkedByAction;
    Entity<ActionComponent>? action1 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    Entity<ActionComponent>? action2 = actions.GetAction(action1);
    if (!action2.HasValue)
      return;
    nullable = action2.GetValueOrDefault().Comp.AttachedEntity;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    EmbeddableProjectileComponent comp;
    if (this.TryComp<EmbeddableProjectileComponent>((EntityUid) ent, out comp))
      this._proj.EmbedDetach((EntityUid) ent, comp, new EntityUid?(valueOrDefault));
    SharedPopupSystem popups = this._popups;
    string recipientMessage = this.Loc.GetString("item-recall-item-summon-self", ("item", (object) ent));
    ILocalizationManager loc = this.Loc;
    (string, object) valueTuple1 = ("item", (object) ent);
    EntityUid uid1 = valueOrDefault;
    EntityManager entityManager = this.EntityManager;
    nullable = new EntityUid?();
    EntityUid? viewer = nullable;
    (string, object) valueTuple2 = ("name", (object) Identity.Entity(uid1, (IEntityManager) entityManager, viewer));
    string othersMessage = loc.GetString("item-recall-item-summon-others", valueTuple1, valueTuple2);
    EntityUid uid2 = valueOrDefault;
    EntityUid? recipient = new EntityUid?(valueOrDefault);
    popups.PopupPredicted(recipientMessage, othersMessage, uid2, recipient);
    this._popups.PopupPredictedCoordinates(this.Loc.GetString("item-recall-item-disappear", ("item", (object) ent)), this.Transform((EntityUid) ent).Coordinates, new EntityUid?(valueOrDefault));
    this._hands.TryForcePickupAnyHand(valueOrDefault, (EntityUid) ent);
  }

  private void OnRecallMarkerShutdown(Entity<RecallMarkerComponent> ent, ref ComponentShutdown args)
  {
    this.TryUnmarkItem((EntityUid) ent);
  }

  private void TryMarkItem(Entity<ItemRecallComponent> ent, EntityUid item)
  {
    Entity<ActionComponent>? action = this._actions.GetAction(new Entity<ActionComponent>?((Entity<ActionComponent>) ent.Owner));
    if (!action.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault1 = action.GetValueOrDefault();
    EntityUid? attachedEntity = valueOrDefault1.Comp.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault2 = attachedEntity.GetValueOrDefault();
    this.AddToPvsOverride(item, valueOrDefault2);
    ent.Comp.MarkedEntity = new EntityUid?(item);
    this.Dirty<ItemRecallComponent>(ent);
    RecallMarkerComponent recallMarkerComponent = this.AddComp<RecallMarkerComponent>(item);
    recallMarkerComponent.MarkedByAction = new EntityUid?((EntityUid) ent);
    this.Dirty(item, (IComponent) recallMarkerComponent);
    this.UpdateActionAppearance((Entity<ActionComponent, ItemRecallComponent>) ((EntityUid) valueOrDefault1, (ActionComponent) valueOrDefault1, (ItemRecallComponent) ent));
  }

  private void TryUnmarkItem(EntityUid item)
  {
    RecallMarkerComponent comp1;
    if (!this.TryComp<RecallMarkerComponent>(item, out comp1))
      return;
    SharedActionsSystem actions = this._actions;
    EntityUid? nullable = comp1.MarkedByAction;
    Entity<ActionComponent>? action1 = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
    Entity<ActionComponent>? action2 = actions.GetAction(action1);
    if (!action2.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault1 = action2.GetValueOrDefault();
    ItemRecallComponent comp2;
    if (this.TryComp<ItemRecallComponent>((EntityUid) valueOrDefault1, out comp2))
    {
      nullable = valueOrDefault1.Comp.AttachedEntity;
      if (nullable.HasValue)
      {
        EntityUid valueOrDefault2 = nullable.GetValueOrDefault();
        this._popups.PopupClient(this.Loc.GetString("item-recall-item-unmark", (nameof (item), (object) item)), valueOrDefault2, new EntityUid?(valueOrDefault2), PopupType.MediumCaution);
        this.RemoveFromPvsOverride(item, valueOrDefault2);
      }
      comp2.MarkedEntity = new EntityUid?();
      this.UpdateActionAppearance((Entity<ActionComponent, ItemRecallComponent>) ((EntityUid) valueOrDefault1, (ActionComponent) valueOrDefault1, comp2));
      this.Dirty((EntityUid) valueOrDefault1, (IComponent) comp2);
    }
    this.RemCompDeferred<RecallMarkerComponent>(item);
  }

  private void UpdateActionAppearance(
    Entity<ActionComponent, ItemRecallComponent> action)
  {
    EntityUid? markedEntity = action.Comp2.MarkedEntity;
    if (markedEntity.HasValue)
    {
      EntityUid valueOrDefault1 = markedEntity.GetValueOrDefault();
      LocId? whileMarkedName = action.Comp2.WhileMarkedName;
      if (whileMarkedName.HasValue)
      {
        LocId valueOrDefault2 = whileMarkedName.GetValueOrDefault();
        this._metaData.SetEntityName((EntityUid) action, this.Loc.GetString((string) valueOrDefault2, ("item", (object) valueOrDefault1)));
      }
      LocId? markedDescription = action.Comp2.WhileMarkedDescription;
      if (markedDescription.HasValue)
      {
        LocId valueOrDefault3 = markedDescription.GetValueOrDefault();
        this._metaData.SetEntityDescription((EntityUid) action, this.Loc.GetString((string) valueOrDefault3, ("item", (object) valueOrDefault1)));
      }
      this._actions.SetEntityIcon((Entity<ActionComponent>) ((EntityUid) action, (ActionComponent) action), new EntityUid?(valueOrDefault1));
    }
    else
    {
      string initialName = action.Comp2.InitialName;
      if (initialName != null)
        this._metaData.SetEntityName((EntityUid) action, initialName);
      string initialDescription = action.Comp2.InitialDescription;
      if (initialDescription != null)
        this._metaData.SetEntityDescription((EntityUid) action, initialDescription);
      this._actions.SetEntityIcon((Entity<ActionComponent>) ((EntityUid) action, (ActionComponent) action), new EntityUid?());
    }
  }

  private void AddToPvsOverride(EntityUid uid, EntityUid user)
  {
    ICommonSession session;
    if (!this._player.TryGetSessionByEntity(user, out session))
      return;
    this._pvs.AddSessionOverride(uid, session);
  }

  private void RemoveFromPvsOverride(EntityUid uid, EntityUid user)
  {
    ICommonSession session;
    if (!this._player.TryGetSessionByEntity(user, out session))
      return;
    this._pvs.RemoveSessionOverride(uid, session);
  }
}
