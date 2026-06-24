using System;
using System.Collections.Generic;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Actions.Events;
using Content.Shared.Interaction;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Actions;

public abstract class SharedRMCActionsSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedInteractionSystem _interaction;

	private EntityQuery<ActionSharedCooldownComponent> _actionSharedCooldownQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_actionSharedCooldownQuery = ((EntitySystem)this).GetEntityQuery<ActionSharedCooldownComponent>();
		((EntitySystem)this).SubscribeAllEvent<RMCMissedTargetActionEvent>((EntityEventHandler<RMCMissedTargetActionEvent>)OnMissedTargetAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionSharedCooldownComponent, ActionPerformedEvent>((EntityEventRefHandler<ActionSharedCooldownComponent, ActionPerformedEvent>)OnSharedCooldownPerformed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionCooldownComponent, RMCActionUseEvent>((EntityEventRefHandler<ActionCooldownComponent, RMCActionUseEvent>)OnCooldownUse, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionInRangeUnobstructedComponent, RMCActionUseAttemptEvent>((EntityEventRefHandler<ActionInRangeUnobstructedComponent, RMCActionUseAttemptEvent>)OnInRangeUnobstructedUseAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActionComponent, ActionReducedUseDelayEvent>((ComponentEventHandler<ActionComponent, ActionReducedUseDelayEvent>)OnReducedUseDelayEvent, (Type[])null, (Type[])null);
	}

	private void OnMissedTargetAction(RMCMissedTargetActionEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EntityUid action = ((EntitySystem)this).GetEntity(args.Action);
		RMCCooldownOnMissComponent cooldown = default(RMCCooldownOnMissComponent);
		if (((EntitySystem)this).TryComp<RMCCooldownOnMissComponent>(action, ref cooldown))
		{
			_actions.SetIfBiggerCooldown(Entity<ActionComponent>.op_Implicit(action), cooldown.MissCooldown);
		}
	}

	private void OnSharedCooldownPerformed(Entity<ActionSharedCooldownComponent> ent, ref ActionPerformedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.OnPerform)
		{
			ActivateSharedCooldown(Entity<ActionSharedCooldownComponent>.op_Implicit((Entity<ActionSharedCooldownComponent>.op_Implicit(ent), Entity<ActionSharedCooldownComponent>.op_Implicit(ent))), args.Performer);
		}
	}

	public void ActivateSharedCooldown(Entity<ActionSharedCooldownComponent?> action, EntityUid performer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ActionSharedCooldownComponent>(Entity<ActionSharedCooldownComponent>.op_Implicit(action), ref action.Comp, false) || action.Comp.Cooldown == TimeSpan.Zero)
		{
			return;
		}
		EntityUid val = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		ActionSharedCooldownComponent shared = default(ActionSharedCooldownComponent);
		foreach (Entity<ActionComponent> action2 in _actions.GetActions(performer))
		{
			action2.Deconstruct(ref val, ref actionComponent);
			EntityUid actionId = val;
			if (!_actionSharedCooldownQuery.TryComp(actionId, ref shared))
			{
				continue;
			}
			if (shared.Id.HasValue)
			{
				EntProtoId? id = shared.Id;
				EntProtoId? id2 = action.Comp.Id;
				if (id.HasValue == id2.HasValue && (!id.HasValue || id.GetValueOrDefault() == id2.GetValueOrDefault()))
				{
					goto IL_00f6;
				}
			}
			if (!action.Comp.Id.HasValue || !shared.Ids.Contains(action.Comp.Id.Value))
			{
				continue;
			}
			goto IL_00f6;
			IL_00f6:
			_actions.SetIfBiggerCooldown(Entity<ActionComponent>.op_Implicit(actionId), action.Comp.Cooldown);
		}
	}

	public void EnableSharedCooldownEvents(Entity<ActionSharedCooldownComponent?> action, EntityUid performer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		SetStatusOfSharedCooldownEvents(action, performer, newStatus: true);
	}

	public void DisableSharedCooldownEvents(Entity<ActionSharedCooldownComponent?> action, EntityUid performer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		SetStatusOfSharedCooldownEvents(action, performer, newStatus: false);
	}

	private void SetStatusOfSharedCooldownEvents(Entity<ActionSharedCooldownComponent?> action, EntityUid performer, bool newStatus)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ActionSharedCooldownComponent>(Entity<ActionSharedCooldownComponent>.op_Implicit(action), ref action.Comp, false) || action.Comp.Cooldown == TimeSpan.Zero)
		{
			return;
		}
		EntityUid val = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		ActionSharedCooldownComponent shared = default(ActionSharedCooldownComponent);
		foreach (Entity<ActionComponent> action2 in _actions.GetActions(performer))
		{
			action2.Deconstruct(ref val, ref actionComponent);
			EntityUid actionId = val;
			ActionComponent comp = actionComponent;
			if (!_actionSharedCooldownQuery.TryComp(actionId, ref shared))
			{
				continue;
			}
			if (shared.Id.HasValue)
			{
				EntProtoId? id = shared.Id;
				EntProtoId? id2 = action.Comp.Id;
				if (id.HasValue == id2.HasValue && (!id.HasValue || id.GetValueOrDefault() == id2.GetValueOrDefault()))
				{
					goto IL_0118;
				}
			}
			if (!action.Comp.Id.HasValue || (!shared.Ids.Contains(action.Comp.Id.Value) && !shared.ActiveIds.Contains(action.Comp.Id.Value)))
			{
				continue;
			}
			goto IL_0118;
			IL_0118:
			_actions.SetEnabled(Entity<ActionComponent>.op_Implicit((actionId, comp)), newStatus);
		}
	}

	private void OnReducedUseDelayEvent(EntityUid uid, ActionComponent component, ActionReducedUseDelayEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		ActionReducedUseDelayComponent comp = default(ActionReducedUseDelayComponent);
		if (!((EntitySystem)this).TryComp<ActionReducedUseDelayComponent>(uid, ref comp) || args.Amount < 0 || args.Amount > 1)
		{
			return;
		}
		comp.UseDelayReduction = args.Amount;
		ActionSharedCooldownComponent shared = default(ActionSharedCooldownComponent);
		if (((EntitySystem)this).TryComp<ActionSharedCooldownComponent>(uid, ref shared))
		{
			ActionReducedUseDelayComponent actionReducedUseDelayComponent = comp;
			TimeSpan valueOrDefault = actionReducedUseDelayComponent.UseDelayBase.GetValueOrDefault();
			if (!actionReducedUseDelayComponent.UseDelayBase.HasValue)
			{
				valueOrDefault = shared.Cooldown;
				actionReducedUseDelayComponent.UseDelayBase = valueOrDefault;
			}
			RefreshSharedUseDelay(Entity<ActionReducedUseDelayComponent>.op_Implicit((uid, comp)), shared);
		}
		else
		{
			ActionReducedUseDelayComponent actionReducedUseDelayComponent = comp;
			TimeSpan? useDelayBase = actionReducedUseDelayComponent.UseDelayBase;
			if (!useDelayBase.HasValue)
			{
				actionReducedUseDelayComponent.UseDelayBase = component.UseDelay;
			}
			RefreshUseDelay(Entity<ActionReducedUseDelayComponent>.op_Implicit((uid, comp)));
		}
	}

	private void RefreshUseDelay(Entity<ActionReducedUseDelayComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan? useDelayBase = ent.Comp.UseDelayBase;
		if (useDelayBase.HasValue)
		{
			TimeSpan delayBase = useDelayBase.GetValueOrDefault();
			double reduction = ent.Comp.UseDelayReduction.Double();
			TimeSpan delayNew = delayBase.Multiply(1.0 - reduction);
			_actions.SetUseDelay(Entity<ActionComponent>.op_Implicit(ent.Owner), delayNew);
		}
	}

	private void RefreshSharedUseDelay(Entity<ActionReducedUseDelayComponent> ent, ActionSharedCooldownComponent shared)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan? useDelayBase = ent.Comp.UseDelayBase;
		if (useDelayBase.HasValue)
		{
			TimeSpan delayBase = useDelayBase.GetValueOrDefault();
			double reduction = ent.Comp.UseDelayReduction.Double();
			TimeSpan delayNew = delayBase.Multiply(1.0 - reduction);
			shared.Cooldown = delayNew;
		}
	}

	private void OnCooldownUse(Entity<ActionCooldownComponent> ent, ref RMCActionUseEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		_actions.SetIfBiggerCooldown(Entity<ActionComponent>.op_Implicit(ent.Owner), ent.Comp.Cooldown);
	}

	private void OnInRangeUnobstructedUseAttempt(Entity<ActionInRangeUnobstructedComponent> ent, ref RMCActionUseAttemptEvent args)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			if (!_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(args.User), Entity<TransformComponent>.op_Implicit(target2), ent.Comp.Range))
			{
				args.Cancelled = true;
			}
		}
	}

	public bool CanUseActionPopup(EntityUid user, EntityUid action, EntityUid? target = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		RMCActionUseAttemptEvent ev = new RMCActionUseAttemptEvent(user, target);
		((EntitySystem)this).RaiseLocalEvent<RMCActionUseAttemptEvent>(action, ref ev, false);
		return !ev.Cancelled;
	}

	private void ActionUsed(EntityUid user, EntityUid action)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		RMCActionUseEvent ev = new RMCActionUseEvent(user);
		((EntitySystem)this).RaiseLocalEvent<RMCActionUseEvent>(action, ref ev, false);
	}

	public bool TryUseAction(EntityUid user, EntityUid action, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!CanUseActionPopup(user, action, target))
		{
			return false;
		}
		ActionUsed(user, action);
		return true;
	}

	public bool TryUseAction(InstantActionEvent action)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!CanUseActionPopup(action.Performer, Entity<ActionComponent>.op_Implicit(action.Action)))
		{
			return false;
		}
		ActionUsed(action.Performer, Entity<ActionComponent>.op_Implicit(action.Action));
		return true;
	}

	public bool TryUseAction(EntityTargetActionEvent action)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!CanUseActionPopup(action.Performer, Entity<ActionComponent>.op_Implicit(action.Action), action.Target))
		{
			return false;
		}
		ActionUsed(action.Performer, Entity<ActionComponent>.op_Implicit(action.Action));
		return true;
	}

	public bool TryUseAction(WorldTargetActionEvent action)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!CanUseActionPopup(action.Performer, Entity<ActionComponent>.op_Implicit(action.Action)))
		{
			return false;
		}
		ActionUsed(action.Performer, Entity<ActionComponent>.op_Implicit(action.Action));
		return true;
	}

	public IEnumerable<Entity<ActionComponent>> GetActionsWithEvent<T>(EntityUid user) where T : BaseActionEvent
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<ActionComponent> action in _actions.GetActions(user))
		{
			if (_actions.GetEvent(Entity<ActionComponent>.op_Implicit(action)) is T)
			{
				yield return action;
			}
		}
	}

	public IEnumerable<Entity<ActionComponent, T>> GetActionsWithComp<T>(EntityUid user) where T : IComponent
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		T comp = default(T);
		foreach (Entity<ActionComponent> action in _actions.GetActions(user))
		{
			if (((EntitySystem)this).TryComp<T>(Entity<ActionComponent>.op_Implicit(action), ref comp))
			{
				yield return Entity<ActionComponent, T>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action), comp));
			}
		}
	}
}
