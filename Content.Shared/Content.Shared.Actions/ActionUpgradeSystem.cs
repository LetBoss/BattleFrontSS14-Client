using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared.Actions.Components;
using Content.Shared.Actions.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.Actions;

public sealed class ActionUpgradeSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private ActionContainerSystem _actionContainer;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ActionUpgradeComponent, ActionUpgradeEvent>((ComponentEventHandler<ActionUpgradeComponent, ActionUpgradeEvent>)OnActionUpgradeEvent, (Type[])null, (Type[])null);
	}

	private void OnActionUpgradeEvent(EntityUid uid, ActionUpgradeComponent component, ActionUpgradeEvent args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (!CanUpgrade(args.NewLevel, component.EffectedLevels, out var newActionProto))
		{
			return;
		}
		Entity<ActionComponent>? action = _actions.GetAction(Entity<ActionComponent>.op_Implicit(uid));
		if (!action.HasValue)
		{
			return;
		}
		Entity<ActionComponent> action2 = action.GetValueOrDefault();
		EntityUid? originalContainer = action2.Comp.Container;
		EntityUid? originalAttachedEntity = action2.Comp.AttachedEntity;
		_actionContainer.RemoveAction(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action2), Entity<ActionComponent>.op_Implicit(action2))));
		EntityUid? upgradedActionId = null;
		ActionsContainerComponent actionContainerComp = default(ActionsContainerComponent);
		if (originalContainer.HasValue && ((EntitySystem)this).TryComp<ActionsContainerComponent>(originalContainer.Value, ref actionContainerComp))
		{
			ActionContainerSystem actionContainer = _actionContainer;
			EntityUid value = originalContainer.Value;
			EntProtoId? val = newActionProto;
			upgradedActionId = actionContainer.AddAction(value, val.HasValue ? EntProtoId.op_Implicit(val.GetValueOrDefault()) : null, actionContainerComp);
			if (originalAttachedEntity.HasValue)
			{
				_actions.GrantContainedActions(Entity<ActionsComponent>.op_Implicit(originalAttachedEntity.Value), Entity<ActionsContainerComponent>.op_Implicit(originalContainer.Value));
			}
			else
			{
				_actions.GrantContainedActions(Entity<ActionsComponent>.op_Implicit(originalContainer.Value), Entity<ActionsContainerComponent>.op_Implicit(originalContainer.Value));
			}
		}
		else if (originalAttachedEntity.HasValue)
		{
			ActionContainerSystem actionContainer2 = _actionContainer;
			EntityUid value2 = originalAttachedEntity.Value;
			EntProtoId? val = newActionProto;
			upgradedActionId = actionContainer2.AddAction(value2, val.HasValue ? EntProtoId.op_Implicit(val.GetValueOrDefault()) : null);
		}
		ActionUpgradeComponent upgradeComp = default(ActionUpgradeComponent);
		if (((EntitySystem)this).TryComp<ActionUpgradeComponent>(upgradedActionId, ref upgradeComp))
		{
			upgradeComp.Level = args.NewLevel;
			((EntitySystem)this).Del((EntityUid?)uid);
		}
	}

	public bool TryUpgradeAction(EntityUid? actionId, out EntityUid? upgradeActionId, ActionUpgradeComponent? actionUpgradeComponent = null, int newLevel = 0)
	{
		upgradeActionId = null;
		if (!TryGetActionUpgrade(actionId, out ActionUpgradeComponent actionUpgradeComp))
		{
			return false;
		}
		if (actionUpgradeComponent == null)
		{
			actionUpgradeComponent = actionUpgradeComp;
		}
		if (newLevel < 1)
		{
			newLevel = actionUpgradeComponent.Level + 1;
		}
		if (!CanLevelUp(newLevel, actionUpgradeComponent.EffectedLevels))
		{
			return false;
		}
		actionUpgradeComponent.Level = newLevel;
		if (!CanUpgrade(newLevel, actionUpgradeComponent.EffectedLevels, out var newActionProto))
		{
			upgradeActionId = actionId;
			return true;
		}
		upgradeActionId = UpgradeAction(actionId, actionUpgradeComp, newActionProto, newLevel);
		return true;
	}

	private bool CanLevelUp(int newLevel, Dictionary<int, EntProtoId> levelDict)
	{
		if (levelDict.Count < 1)
		{
			return false;
		}
		bool canLevel = false;
		int finalLevel = levelDict.Keys.ToList()[levelDict.Keys.Count - 1];
		foreach (var (level, _) in levelDict)
		{
			if (newLevel <= finalLevel && ((newLevel <= finalLevel && newLevel != level) || newLevel == level))
			{
				canLevel = true;
				break;
			}
		}
		return canLevel;
	}

	private bool CanUpgrade(int newLevel, Dictionary<int, EntProtoId> levelDict, [NotNullWhen(true)] out EntProtoId? newLevelProto)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		bool canUpgrade = false;
		newLevelProto = null;
		int finalLevel = levelDict.Keys.ToList()[levelDict.Keys.Count - 1];
		foreach (var (level, proto) in levelDict)
		{
			if (newLevel == level && newLevel <= finalLevel)
			{
				canUpgrade = true;
				newLevelProto = proto;
				break;
			}
		}
		return canUpgrade;
	}

	public EntityUid? UpgradeAction(EntityUid? actionId, ActionUpgradeComponent? actionUpgradeComponent = null, EntProtoId? newActionProto = null, int newLevel = 0)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetActionUpgrade(actionId, out ActionUpgradeComponent actionUpgradeComp))
		{
			return null;
		}
		if (actionUpgradeComponent == null)
		{
			actionUpgradeComponent = actionUpgradeComp;
		}
		if (newLevel < 1)
		{
			newLevel = actionUpgradeComponent.Level + 1;
		}
		actionUpgradeComponent.Level = newLevel;
		if (CanUpgrade(newLevel, actionUpgradeComponent.EffectedLevels, out var newActionPrototype))
		{
			SharedActionsSystem actions = _actions;
			EntityUid? val = actionId;
			Entity<ActionComponent>? action = actions.GetAction(val.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(val.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
			if (action.HasValue)
			{
				Entity<ActionComponent> action2 = action.GetValueOrDefault();
				EntProtoId? val2 = newActionProto;
				if (!val2.HasValue)
				{
					newActionProto = newActionPrototype;
				}
				EntityUid? originalContainer = action2.Comp.Container;
				EntityUid? originalAttachedEntity = action2.Comp.AttachedEntity;
				_actionContainer.RemoveAction(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action2), action2.Comp)));
				EntityUid? upgradedActionId = null;
				ActionsContainerComponent actionContainerComp = default(ActionsContainerComponent);
				if (originalContainer.HasValue && ((EntitySystem)this).TryComp<ActionsContainerComponent>(originalContainer.Value, ref actionContainerComp))
				{
					ActionContainerSystem actionContainer = _actionContainer;
					EntityUid value = originalContainer.Value;
					val2 = newActionProto;
					upgradedActionId = actionContainer.AddAction(value, val2.HasValue ? EntProtoId.op_Implicit(val2.GetValueOrDefault()) : null, actionContainerComp);
					if (originalAttachedEntity.HasValue)
					{
						_actions.GrantContainedActions(Entity<ActionsComponent>.op_Implicit(originalAttachedEntity.Value), Entity<ActionsContainerComponent>.op_Implicit(originalContainer.Value));
					}
					else
					{
						_actions.GrantContainedActions(Entity<ActionsComponent>.op_Implicit(originalContainer.Value), Entity<ActionsContainerComponent>.op_Implicit(originalContainer.Value));
					}
				}
				else if (originalAttachedEntity.HasValue)
				{
					ActionContainerSystem actionContainer2 = _actionContainer;
					EntityUid value2 = originalAttachedEntity.Value;
					val2 = newActionProto;
					upgradedActionId = actionContainer2.AddAction(value2, val2.HasValue ? EntProtoId.op_Implicit(val2.GetValueOrDefault()) : null);
				}
				ActionUpgradeComponent upgradeComp = default(ActionUpgradeComponent);
				if (!((EntitySystem)this).TryComp<ActionUpgradeComponent>(upgradedActionId, ref upgradeComp))
				{
					return null;
				}
				upgradeComp.Level = newLevel;
				((EntitySystem)this).Del(actionId);
				return upgradedActionId.Value;
			}
		}
		return null;
	}

	private void RaiseActionUpgradeEvent(int level, EntityUid actionId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		ActionUpgradeEvent ev = new ActionUpgradeEvent(level, actionId);
		((EntitySystem)this).RaiseLocalEvent<ActionUpgradeEvent>(actionId, ev, false);
	}

	public bool TryGetActionUpgrade([NotNullWhen(true)] EntityUid? uid, [NotNullWhen(true)] out ActionUpgradeComponent? result, bool logError = true)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		result = null;
		if (!((EntitySystem)this).Exists(uid))
		{
			return false;
		}
		ActionUpgradeComponent actionUpgradeComponent = default(ActionUpgradeComponent);
		if (!((EntitySystem)this).TryComp<ActionUpgradeComponent>(uid, ref actionUpgradeComponent))
		{
			((EntitySystem)this).Log.Error($"Failed to get action upgrade from action entity: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid.Value))}");
			return false;
		}
		result = actionUpgradeComponent;
		return true;
	}
}
