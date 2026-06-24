using System;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Xenonids.Cruelty;

public sealed class XenoCrueltySystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private XenoSystem _xeno;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoCrueltyComponent, MeleeHitEvent>((EntityEventRefHandler<XenoCrueltyComponent, MeleeHitEvent>)OnCrueltyHit, (Type[])null, (Type[])null);
	}

	private void OnCrueltyHit(Entity<XenoCrueltyComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		bool hit = false;
		foreach (EntityUid ent in args.HitEntities)
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoCrueltyComponent>.op_Implicit(xeno), ent))
			{
				hit = true;
				break;
			}
		}
		if (!hit)
		{
			return;
		}
		EntityUid val = default(EntityUid);
		ActionComponent actionComponent = default(ActionComponent);
		foreach (Entity<ActionComponent> item in _rmcActions.GetActionsWithEvent<XenoLeapActionEvent>(Entity<XenoCrueltyComponent>.op_Implicit(xeno)))
		{
			item.Deconstruct(ref val, ref actionComponent);
			EntityUid actionId = val;
			ActionComponent action = actionComponent;
			if (action.Cooldown.HasValue)
			{
				TimeSpan cooldownEnd = action.Cooldown.Value.End - xeno.Comp.CooldownReduction;
				if (cooldownEnd < action.Cooldown.Value.Start)
				{
					_actions.ClearCooldown(Entity<ActionComponent>.op_Implicit(actionId));
				}
				else
				{
					_actions.SetCooldown(Entity<ActionComponent>.op_Implicit(actionId), action.Cooldown.Value.Start, cooldownEnd);
				}
			}
		}
	}
}
