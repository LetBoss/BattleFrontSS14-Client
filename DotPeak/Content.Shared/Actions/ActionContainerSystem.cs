// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.ActionContainerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions.Components;
using Content.Shared.Ghost;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared.Actions;

public sealed class ActionContainerSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private INetManager _netMan;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedMindSystem _mind;
  private EntityQuery<ActionComponent> _query;

  public virtual void Initialize()
  {
    base.Initialize();
    this._query = this.GetEntityQuery<ActionComponent>();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsContainerComponent, ComponentInit>(new ComponentEventHandler<ActionsContainerComponent, ComponentInit>((object) this, __methodptr(OnInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsContainerComponent, ComponentShutdown>(new ComponentEventHandler<ActionsContainerComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsContainerComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ActionsContainerComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnEntityRemoved)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsContainerComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ActionsContainerComponent, EntInsertedIntoContainerMessage>((object) this, __methodptr(OnEntityInserted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsContainerComponent, ActionAddedEvent>(new ComponentEventHandler<ActionsContainerComponent, ActionAddedEvent>((object) this, __methodptr(OnActionAdded)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsContainerComponent, MindAddedMessage>(new ComponentEventHandler<ActionsContainerComponent, MindAddedMessage>((object) this, __methodptr(OnMindAdded)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionsContainerComponent, MindRemovedMessage>(new ComponentEventHandler<ActionsContainerComponent, MindRemovedMessage>((object) this, __methodptr(OnMindRemoved)), (Type[]) null, (Type[]) null);
  }

  private void OnMindAdded(
    EntityUid uid,
    ActionsContainerComponent component,
    MindAddedMessage args)
  {
    EntityUid mindId;
    ActionsContainerComponent containerComponent;
    if (!this._mind.TryGetMind(uid, out mindId, out MindComponent _) || !this.TryComp<ActionsContainerComponent>(mindId, ref containerComponent) || this.HasComp<GhostComponent>(uid) || ((BaseContainer) containerComponent.Container).ContainedEntities.Count <= 0)
      return;
    this._actions.GrantContainedActions(Entity<ActionsComponent>.op_Implicit(uid), Entity<ActionsContainerComponent>.op_Implicit(mindId));
  }

  private void OnMindRemoved(
    EntityUid uid,
    ActionsContainerComponent component,
    MindRemovedMessage args)
  {
    this._actions.RemoveProvidedActions(uid, Entity<MindComponent>.op_Implicit(args.Mind));
  }

  public EntityUid? AddAction(
    EntityUid uid,
    string actionPrototypeId,
    ActionsContainerComponent? comp = null)
  {
    EntityUid? actionId = new EntityUid?();
    this.EnsureAction(uid, ref actionId, actionPrototypeId, comp);
    return actionId;
  }

  public bool EnsureAction(
    EntityUid uid,
    [NotNullWhen(true)] ref EntityUid? actionId,
    string actionPrototypeId,
    ActionsContainerComponent? comp = null)
  {
    return this.EnsureAction(uid, ref actionId, out ActionComponent _, actionPrototypeId, comp);
  }

  public bool EnsureAction(
    EntityUid uid,
    [NotNullWhen(true)] ref EntityUid? actionId,
    [NotNullWhen(true)] out ActionComponent? action,
    string? actionPrototypeId,
    ActionsContainerComponent? comp = null)
  {
    action = (ActionComponent) null;
    if (comp == null)
      comp = this.EnsureComp<ActionsContainerComponent>(uid);
    if (this.Exists(actionId))
    {
      if (!((BaseContainer) comp.Container).Contains(actionId.Value))
      {
        this.Log.Error($"Action {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(actionId.Value))} is not contained in the expected container {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
        return false;
      }
      SharedActionsSystem actions = this._actions;
      EntityUid? nullable = actionId;
      Entity<ActionComponent>? action1 = nullable.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable.GetValueOrDefault())) : new Entity<ActionComponent>?();
      Entity<ActionComponent>? action2 = actions.GetAction(action1);
      if (!action2.HasValue)
        return false;
      Entity<ActionComponent> valueOrDefault = action2.GetValueOrDefault();
      actionId = new EntityUid?(Entity<ActionComponent>.op_Implicit(valueOrDefault));
      action = valueOrDefault.Comp;
      return true;
    }
    if (actionPrototypeId == null || this._netMan.IsClient && !this.IsClientSide(uid, (MetaDataComponent) null))
      return false;
    actionId = new EntityUid?(this.Spawn(actionPrototypeId, (ComponentRegistry) null, true));
    if (!this._query.TryComp(actionId, ref action))
    {
      this.Log.Error($"Tried to add invalid action {this.ToPrettyString(actionId, (MetaDataComponent) null)} to {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}!");
      this.Del(actionId);
      return false;
    }
    if (this.AddAction(uid, actionId.Value, action, comp))
      return true;
    this.Del(new EntityUid?(actionId.Value));
    actionId = new EntityUid?();
    return false;
  }

  public void TransferAction(
    EntityUid actionId,
    EntityUid newContainer,
    ActionComponent? action = null,
    ActionsContainerComponent? container = null)
  {
    Entity<ActionComponent>? action1 = this._actions.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((actionId, action))));
    if (!action1.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action1.GetValueOrDefault();
    EntityUid? container1 = valueOrDefault.Comp.Container;
    EntityUid entityUid = newContainer;
    if ((container1.HasValue ? (EntityUid.op_Equality(container1.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) != 0)
      return;
    EntityUid? attachedEntity = valueOrDefault.Comp.AttachedEntity;
    this.AddAction(newContainer, Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp, container);
  }

  public void TransferAllActions(
    EntityUid from,
    EntityUid to,
    ActionsContainerComponent? oldContainer = null,
    ActionsContainerComponent? newContainer = null)
  {
    if (!this.Resolve<ActionsContainerComponent>(from, ref oldContainer, true) || !this.Resolve<ActionsContainerComponent>(to, ref newContainer, true))
      return;
    foreach (EntityUid actionId in ((BaseContainer) oldContainer.Container).ContainedEntities.ToArray<EntityUid>())
      this.TransferAction(actionId, to, container: newContainer);
  }

  public void TransferActionWithNewAttached(
    EntityUid actionId,
    EntityUid newContainer,
    EntityUid newAttached,
    ActionComponent? action = null,
    ActionsContainerComponent? container = null)
  {
    Entity<ActionComponent>? action1 = this._actions.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((actionId, action))));
    if (!action1.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action1.GetValueOrDefault();
    EntityUid? container1 = valueOrDefault.Comp.Container;
    EntityUid entityUid = newContainer;
    if ((container1.HasValue ? (EntityUid.op_Equality(container1.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) != 0 || !this.AddAction(newContainer, Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp, container))
      return;
    this._actions.AddActionDirect(Entity<ActionsComponent>.op_Implicit(newAttached), new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp))));
  }

  public void TransferAllActionsWithNewAttached(
    EntityUid from,
    EntityUid to,
    EntityUid newAttached,
    ActionsContainerComponent? oldContainer = null,
    ActionsContainerComponent? newContainer = null)
  {
    if (!this.Resolve<ActionsContainerComponent>(from, ref oldContainer, true) || !this.Resolve<ActionsContainerComponent>(to, ref newContainer, true))
      return;
    foreach (EntityUid actionId in ((BaseContainer) oldContainer.Container).ContainedEntities.ToArray<EntityUid>())
      this.TransferActionWithNewAttached(actionId, to, newAttached, container: newContainer);
  }

  public bool AddAction(
    EntityUid uid,
    EntityUid actionId,
    ActionComponent? action = null,
    ActionsContainerComponent? comp = null)
  {
    Entity<ActionComponent>? action1 = this._actions.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((actionId, action))));
    if (!action1.HasValue)
      return false;
    Entity<ActionComponent> valueOrDefault = action1.GetValueOrDefault();
    if (valueOrDefault.Comp.Container.HasValue)
      this.RemoveAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), Entity<ActionComponent>.op_Implicit(valueOrDefault)))));
    if (comp == null)
      comp = this.EnsureComp<ActionsContainerComponent>(uid);
    if (this._container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(valueOrDefault.Owner), (BaseContainer) comp.Container, (TransformComponent) null, false))
      return true;
    this.Log.Error($"Failed to insert action {this.ToPrettyString(new EntityUid?(Entity<ActionComponent>.op_Implicit(valueOrDefault)), (MetaDataComponent) null)} into {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
    return false;
  }

  public void RemoveAction(Entity<ActionComponent?>? action, bool logMissing = true)
  {
    Entity<ActionComponent>? action1 = this._actions.GetAction(action, logMissing);
    if (!action1.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault1 = action1.GetValueOrDefault();
    if (!valueOrDefault1.Comp.Container.HasValue)
      return;
    this._transform.DetachEntity(Entity<ActionComponent>.op_Implicit(valueOrDefault1), this.Transform(Entity<ActionComponent>.op_Implicit(valueOrDefault1)));
    EntityUid? nullable = valueOrDefault1.Comp.Container;
    if (nullable.HasValue)
    {
      EntityUid valueOrDefault2 = nullable.GetValueOrDefault();
      if (this.Exists(valueOrDefault2))
        this.Log.Error($"Failed to remove action {this.ToPrettyString(new EntityUid?(Entity<ActionComponent>.op_Implicit(valueOrDefault1)), (MetaDataComponent) null)} from its container {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(valueOrDefault2))}?");
      valueOrDefault1.Comp.Container = new EntityUid?();
      this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault1), valueOrDefault1.Comp, "Container", (MetaDataComponent) null);
    }
    nullable = valueOrDefault1.Comp.AttachedEntity;
    if (!nullable.HasValue)
      return;
    this._actions.RemoveAction(Entity<ActionsComponent>.op_Implicit(nullable.GetValueOrDefault()), new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault1), Entity<ActionComponent>.op_Implicit(valueOrDefault1)))));
  }

  private void OnInit(EntityUid uid, ActionsContainerComponent component, ComponentInit args)
  {
    component.Container = this._container.EnsureContainer<Container>(uid, "actions", (ContainerManagerComponent) null);
  }

  private void OnShutdown(
    EntityUid uid,
    ActionsContainerComponent component,
    ComponentShutdown args)
  {
    if (this._timing.ApplyingState && component.NetSyncEnabled)
      return;
    this._container.ShutdownContainer((BaseContainer) component.Container);
  }

  private void OnEntityInserted(
    EntityUid uid,
    ActionsContainerComponent component,
    EntInsertedIntoContainerMessage args)
  {
    if (((ContainerModifiedMessage) args).Container.ID != "actions")
      return;
    Entity<ActionComponent>? action = this._actions.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(((ContainerModifiedMessage) args).Entity)));
    if (!action.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
    EntityUid? container = valueOrDefault.Comp.Container;
    EntityUid entityUid = uid;
    if ((container.HasValue ? (EntityUid.op_Inequality(container.GetValueOrDefault(), entityUid) ? 1 : 0) : 1) != 0)
    {
      valueOrDefault.Comp.Container = new EntityUid?(uid);
      this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp, "Container", (MetaDataComponent) null);
    }
    ActionAddedEvent actionAddedEvent = new ActionAddedEvent(((ContainerModifiedMessage) args).Entity, Entity<ActionComponent>.op_Implicit(valueOrDefault));
    this.RaiseLocalEvent<ActionAddedEvent>(uid, ref actionAddedEvent, false);
  }

  private void OnEntityRemoved(
    EntityUid uid,
    ActionsContainerComponent component,
    EntRemovedFromContainerMessage args)
  {
    if (((ContainerModifiedMessage) args).Container.ID != "actions")
      return;
    Entity<ActionComponent>? action = this._actions.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(((ContainerModifiedMessage) args).Entity)), false);
    if (!action.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
    ActionRemovedEvent actionRemovedEvent = new ActionRemovedEvent(((ContainerModifiedMessage) args).Entity, Entity<ActionComponent>.op_Implicit(valueOrDefault));
    this.RaiseLocalEvent<ActionRemovedEvent>(uid, ref actionRemovedEvent, false);
    if (!valueOrDefault.Comp.Container.HasValue)
      return;
    valueOrDefault.Comp.Container = new EntityUid?();
    this.DirtyField<ActionComponent>(Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp, "Container", (MetaDataComponent) null);
  }

  private void OnActionAdded(
    EntityUid uid,
    ActionsContainerComponent component,
    ActionAddedEvent args)
  {
    MindComponent mindComponent;
    if (!this.TryComp<MindComponent>(uid, ref mindComponent) || !mindComponent.OwnedEntity.HasValue)
      return;
    EntityUid? ownedEntity = mindComponent.OwnedEntity;
    if (!this.HasComp<ActionsContainerComponent>(ownedEntity.Value))
      return;
    SharedActionsSystem actions = this._actions;
    ownedEntity = mindComponent.OwnedEntity;
    Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(ownedEntity.Value);
    Entity<ActionsContainerComponent> container = Entity<ActionsContainerComponent>.op_Implicit(uid);
    EntityUid action = args.Action;
    actions.GrantContainedAction(performer, container, action);
  }
}
