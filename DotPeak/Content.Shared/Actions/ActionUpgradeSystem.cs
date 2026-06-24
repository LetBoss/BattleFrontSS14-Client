// Decompiled with JetBrains decompiler
// Type: Content.Shared.Actions.ActionUpgradeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions.Components;
using Content.Shared.Actions.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared.Actions;

public sealed class ActionUpgradeSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private ActionContainerSystem _actionContainer;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ActionUpgradeComponent, ActionUpgradeEvent>(new ComponentEventHandler<ActionUpgradeComponent, ActionUpgradeEvent>((object) this, __methodptr(OnActionUpgradeEvent)), (Type[]) null, (Type[]) null);
  }

  private void OnActionUpgradeEvent(
    EntityUid uid,
    ActionUpgradeComponent component,
    ActionUpgradeEvent args)
  {
    EntProtoId? newLevelProto;
    if (!this.CanUpgrade(args.NewLevel, component.EffectedLevels, out newLevelProto))
      return;
    Entity<ActionComponent>? action = this._actions.GetAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(uid)));
    if (!action.HasValue)
      return;
    Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
    EntityUid? container = valueOrDefault.Comp.Container;
    EntityUid? attachedEntity = valueOrDefault.Comp.AttachedEntity;
    this._actionContainer.RemoveAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), Entity<ActionComponent>.op_Implicit(valueOrDefault)))));
    EntityUid? nullable1 = new EntityUid?();
    ActionsContainerComponent containerComponent;
    if (container.HasValue && this.TryComp<ActionsContainerComponent>(container.Value, ref containerComponent))
    {
      ActionContainerSystem actionContainer = this._actionContainer;
      EntityUid uid1 = container.Value;
      EntProtoId? nullable2 = newLevelProto;
      string actionPrototypeId = nullable2.HasValue ? EntProtoId.op_Implicit(nullable2.GetValueOrDefault()) : (string) null;
      ActionsContainerComponent comp = containerComponent;
      nullable1 = actionContainer.AddAction(uid1, actionPrototypeId, comp);
      if (attachedEntity.HasValue)
        this._actions.GrantContainedActions(Entity<ActionsComponent>.op_Implicit(attachedEntity.Value), Entity<ActionsContainerComponent>.op_Implicit(container.Value));
      else
        this._actions.GrantContainedActions(Entity<ActionsComponent>.op_Implicit(container.Value), Entity<ActionsContainerComponent>.op_Implicit(container.Value));
    }
    else if (attachedEntity.HasValue)
    {
      ActionContainerSystem actionContainer = this._actionContainer;
      EntityUid uid2 = attachedEntity.Value;
      EntProtoId? nullable3 = newLevelProto;
      string actionPrototypeId = nullable3.HasValue ? EntProtoId.op_Implicit(nullable3.GetValueOrDefault()) : (string) null;
      nullable1 = actionContainer.AddAction(uid2, actionPrototypeId);
    }
    ActionUpgradeComponent upgradeComponent;
    if (!this.TryComp<ActionUpgradeComponent>(nullable1, ref upgradeComponent))
      return;
    upgradeComponent.Level = args.NewLevel;
    this.Del(new EntityUid?(uid));
  }

  public bool TryUpgradeAction(
    EntityUid? actionId,
    out EntityUid? upgradeActionId,
    ActionUpgradeComponent? actionUpgradeComponent = null,
    int newLevel = 0)
  {
    upgradeActionId = new EntityUid?();
    ActionUpgradeComponent result;
    if (!this.TryGetActionUpgrade(actionId, out result))
      return false;
    if (actionUpgradeComponent == null)
      actionUpgradeComponent = result;
    if (newLevel < 1)
      newLevel = actionUpgradeComponent.Level + 1;
    if (!this.CanLevelUp(newLevel, actionUpgradeComponent.EffectedLevels))
      return false;
    actionUpgradeComponent.Level = newLevel;
    EntProtoId? newLevelProto;
    if (!this.CanUpgrade(newLevel, actionUpgradeComponent.EffectedLevels, out newLevelProto))
    {
      upgradeActionId = actionId;
      return true;
    }
    upgradeActionId = this.UpgradeAction(actionId, result, newLevelProto, newLevel);
    return true;
  }

  private bool CanLevelUp(int newLevel, Dictionary<int, EntProtoId> levelDict)
  {
    if (levelDict.Count < 1)
      return false;
    bool flag = false;
    int num = levelDict.Keys.ToList<int>()[levelDict.Keys.Count - 1];
    foreach ((int key, EntProtoId _) in levelDict)
    {
      if (newLevel <= num && (newLevel <= num && newLevel != key || newLevel == key))
      {
        flag = true;
        break;
      }
    }
    return flag;
  }

  private bool CanUpgrade(
    int newLevel,
    Dictionary<int, EntProtoId> levelDict,
    [NotNullWhen(true)] out EntProtoId? newLevelProto)
  {
    bool flag = false;
    newLevelProto = new EntProtoId?();
    int num = levelDict.Keys.ToList<int>()[levelDict.Keys.Count - 1];
    foreach ((int key, EntProtoId entProtoId) in levelDict)
    {
      if (newLevel == key && newLevel <= num)
      {
        flag = true;
        newLevelProto = new EntProtoId?(entProtoId);
        break;
      }
    }
    return flag;
  }

  public EntityUid? UpgradeAction(
    EntityUid? actionId,
    ActionUpgradeComponent? actionUpgradeComponent = null,
    EntProtoId? newActionProto = null,
    int newLevel = 0)
  {
    ActionUpgradeComponent result;
    if (!this.TryGetActionUpgrade(actionId, out result))
      return new EntityUid?();
    if (actionUpgradeComponent == null)
      actionUpgradeComponent = result;
    if (newLevel < 1)
      newLevel = actionUpgradeComponent.Level + 1;
    actionUpgradeComponent.Level = newLevel;
    EntProtoId? newLevelProto;
    EntityUid? nullable1;
    if (this.CanUpgrade(newLevel, actionUpgradeComponent.EffectedLevels, out newLevelProto))
    {
      SharedActionsSystem actions = this._actions;
      nullable1 = actionId;
      Entity<ActionComponent>? action1 = nullable1.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable1.GetValueOrDefault())) : new Entity<ActionComponent>?();
      Entity<ActionComponent>? action2 = actions.GetAction(action1);
      if (action2.HasValue)
      {
        Entity<ActionComponent> valueOrDefault = action2.GetValueOrDefault();
        if (!newActionProto.HasValue)
          newActionProto = newLevelProto;
        EntityUid? container = valueOrDefault.Comp.Container;
        EntityUid? attachedEntity = valueOrDefault.Comp.AttachedEntity;
        this._actionContainer.RemoveAction(new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(valueOrDefault), valueOrDefault.Comp))));
        EntityUid? nullable2 = new EntityUid?();
        ActionsContainerComponent containerComponent;
        if (container.HasValue && this.TryComp<ActionsContainerComponent>(container.Value, ref containerComponent))
        {
          ActionContainerSystem actionContainer = this._actionContainer;
          EntityUid uid = container.Value;
          EntProtoId? nullable3 = newActionProto;
          string actionPrototypeId = nullable3.HasValue ? EntProtoId.op_Implicit(nullable3.GetValueOrDefault()) : (string) null;
          ActionsContainerComponent comp = containerComponent;
          nullable2 = actionContainer.AddAction(uid, actionPrototypeId, comp);
          if (attachedEntity.HasValue)
            this._actions.GrantContainedActions(Entity<ActionsComponent>.op_Implicit(attachedEntity.Value), Entity<ActionsContainerComponent>.op_Implicit(container.Value));
          else
            this._actions.GrantContainedActions(Entity<ActionsComponent>.op_Implicit(container.Value), Entity<ActionsContainerComponent>.op_Implicit(container.Value));
        }
        else if (attachedEntity.HasValue)
        {
          ActionContainerSystem actionContainer = this._actionContainer;
          EntityUid uid = attachedEntity.Value;
          EntProtoId? nullable4 = newActionProto;
          string actionPrototypeId = nullable4.HasValue ? EntProtoId.op_Implicit(nullable4.GetValueOrDefault()) : (string) null;
          nullable2 = actionContainer.AddAction(uid, actionPrototypeId);
        }
        ActionUpgradeComponent upgradeComponent;
        if (!this.TryComp<ActionUpgradeComponent>(nullable2, ref upgradeComponent))
        {
          nullable1 = new EntityUid?();
          return nullable1;
        }
        upgradeComponent.Level = newLevel;
        this.Del(actionId);
        return new EntityUid?(nullable2.Value);
      }
    }
    nullable1 = new EntityUid?();
    return nullable1;
  }

  private void RaiseActionUpgradeEvent(int level, EntityUid actionId)
  {
    ActionUpgradeEvent actionUpgradeEvent = new ActionUpgradeEvent(level, new EntityUid?(actionId));
    this.RaiseLocalEvent<ActionUpgradeEvent>(actionId, actionUpgradeEvent, false);
  }

  public bool TryGetActionUpgrade([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out ActionUpgradeComponent? result, bool logError = true)
  {
    result = (ActionUpgradeComponent) null;
    if (!this.Exists(uid))
      return false;
    ActionUpgradeComponent upgradeComponent;
    if (!this.TryComp<ActionUpgradeComponent>(uid, ref upgradeComponent))
    {
      this.Log.Error($"Failed to get action upgrade from action entity: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid.Value))}");
      return false;
    }
    result = upgradeComponent;
    return true;
  }
}
