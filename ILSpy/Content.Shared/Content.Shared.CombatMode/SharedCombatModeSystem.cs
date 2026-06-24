using System;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Mind;
using Content.Shared.MouseRotator;
using Content.Shared.Movement.Components;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.CombatMode;

public abstract class SharedCombatModeSystem : EntitySystem
{
	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	private SharedActionsSystem _actionsSystem;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedMindSystem _mind;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<CombatModeComponent, MapInitEvent>((ComponentEventHandler<CombatModeComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CombatModeComponent, ComponentShutdown>((ComponentEventHandler<CombatModeComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CombatModeComponent, ToggleCombatActionEvent>((ComponentEventHandler<CombatModeComponent, ToggleCombatActionEvent>)OnActionPerform, (Type[])null, (Type[])null);
	}

	private void OnMapInit(EntityUid uid, CombatModeComponent component, MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		_actionsSystem.AddAction(uid, ref component.CombatToggleActionEntity, component.CombatToggleAction);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnShutdown(EntityUid uid, CombatModeComponent component, ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		SharedActionsSystem actionsSystem = _actionsSystem;
		Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(uid);
		EntityUid? combatToggleActionEntity = component.CombatToggleActionEntity;
		actionsSystem.RemoveAction(performer, combatToggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(combatToggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null));
		SetMouseRotatorComponents(uid, value: false);
	}

	private void OnActionPerform(EntityUid uid, CombatModeComponent component, ToggleCombatActionEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			SetInCombatMode(uid, !component.IsInCombatMode, component);
			string msg = (component.IsInCombatMode ? "action-popup-combat-enabled" : "action-popup-combat-disabled");
			_popup.PopupClient(base.Loc.GetString(msg), args.Performer, args.Performer);
		}
	}

	public void SetCanDisarm(EntityUid entity, bool canDisarm, CombatModeComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<CombatModeComponent>(entity, ref component, true))
		{
			component.CanDisarm = canDisarm;
		}
	}

	public bool IsInCombatMode(EntityUid? entity, CombatModeComponent? component = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (entity.HasValue && ((EntitySystem)this).Resolve<CombatModeComponent>(entity.Value, ref component, false))
		{
			return component.IsInCombatMode;
		}
		return false;
	}

	public virtual void SetInCombatMode(EntityUid entity, bool value, CombatModeComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<CombatModeComponent>(entity, ref component, true) && component.IsInCombatMode != value)
		{
			component.IsInCombatMode = value;
			((EntitySystem)this).Dirty(entity, (IComponent)(object)component, (MetaDataComponent)null);
			if (component.CombatToggleActionEntity.HasValue)
			{
				SharedActionsSystem actionsSystem = _actionsSystem;
				EntityUid? combatToggleActionEntity = component.CombatToggleActionEntity;
				actionsSystem.SetToggled(combatToggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(combatToggleActionEntity.GetValueOrDefault())) : ((Entity<ActionComponent>?)null), component.IsInCombatMode);
			}
			if (component.ToggleMouseRotator && (!IsNpc(entity) || _mind.TryGetMind(entity, out EntityUid _, out MindComponent _)))
			{
				SetMouseRotatorComponents(entity, value);
			}
		}
	}

	private void SetMouseRotatorComponents(EntityUid uid, bool value)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (value)
		{
			((EntitySystem)this).EnsureComp<MouseRotatorComponent>(uid);
			((EntitySystem)this).EnsureComp<NoRotateOnMoveComponent>(uid);
		}
		else
		{
			((EntitySystem)this).RemComp<MouseRotatorComponent>(uid);
			((EntitySystem)this).RemComp<NoRotateOnMoveComponent>(uid);
		}
	}

	protected abstract bool IsNpc(EntityUid uid);
}
