using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Xenonids.Charge;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Brutalize;

public sealed class XenoBrutalizeSystem : EntitySystem
{
	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedActionsSystem _actions;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoBrutalizeComponent, MeleeHitEvent>((EntityEventRefHandler<XenoBrutalizeComponent, MeleeHitEvent>)OnBrutalMeleeHit, (Type[])null, (Type[])null);
	}

	private void OnBrutalMeleeHit(Entity<XenoBrutalizeComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? mainTarget = null;
		foreach (EntityUid ent in args.HitEntities)
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoBrutalizeComponent>.op_Implicit(xeno), ent))
			{
				mainTarget = ent;
				break;
			}
		}
		if (!mainTarget.HasValue)
		{
			return;
		}
		int currHits = 0;
		DamageSpecifier damage = xeno.Comp.Damage;
		foreach (Entity<MobStateComponent> extra in _entityLookup.GetEntitiesInRange<MobStateComponent>(_transform.GetMapCoordinates(mainTarget.Value, (TransformComponent)null), xeno.Comp.Range, (LookupFlags)110))
		{
			if (!_xeno.CanAbilityAttackTarget(Entity<XenoBrutalizeComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(extra)) || _mobState.IsDead(Entity<MobStateComponent>.op_Implicit(extra)) || args.HitEntities.Contains(Entity<MobStateComponent>.op_Implicit(extra)))
			{
				continue;
			}
			currHits++;
			if (_damageable.TryChangeDamage(Entity<MobStateComponent>.op_Implicit(extra), _xeno.TryApplyXenoSlashDamageMultiplier(Entity<MobStateComponent>.op_Implicit(extra), damage), ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoBrutalizeComponent>.op_Implicit(xeno), Entity<XenoBrutalizeComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
			{
				Filter filter = Filter.Pvs(Entity<MobStateComponent>.op_Implicit(extra), 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
				_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { Entity<MobStateComponent>.op_Implicit(extra) }, filter);
			}
			if (xeno.Comp.MaxTargets.HasValue && currHits >= xeno.Comp.MaxTargets)
			{
				break;
			}
			if (_net.IsServer)
			{
				((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.Effect), extra.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			}
		}
		RefreshCooldowns(xeno, currHits);
	}

	private void RefreshCooldowns(Entity<XenoBrutalizeComponent> xeno, int hits)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<ActionComponent> action in _actions.GetActions(Entity<XenoBrutalizeComponent>.op_Implicit(xeno)))
		{
			BaseActionEvent actionEvent = _actions.GetEvent(Entity<ActionComponent>.op_Implicit(action));
			if ((actionEvent is XenoChargeActionEvent || actionEvent is XenoDefensiveShieldActionEvent) && action.Comp.Cooldown.HasValue)
			{
				TimeSpan cooldownEnd = action.Comp.Cooldown.Value.End - (xeno.Comp.BaseCooldownReduction + ((actionEvent is XenoChargeActionEvent) ? (hits * xeno.Comp.AddtionalCooldownReductions) : TimeSpan.Zero));
				if (cooldownEnd < action.Comp.Cooldown.Value.Start)
				{
					_actions.ClearCooldown(action.AsNullable());
				}
				else
				{
					_actions.SetCooldown(action.AsNullable(), action.Comp.Cooldown.Value.Start, cooldownEnd);
				}
			}
		}
	}
}
