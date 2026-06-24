using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Slow;
using Content.Shared.Standing;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Xenonids.MeleeSlow;

public sealed class XenoMeleeSlowSystem : EntitySystem
{
	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private RMCSlowSystem _slow;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoMeleeSlowComponent, MeleeHitEvent>((EntityEventRefHandler<XenoMeleeSlowComponent, MeleeHitEvent>)OnHit, (Type[])null, (Type[])null);
	}

	private void OnHit(Entity<XenoMeleeSlowComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsHit || args.HitEntities.Count == 0)
		{
			return;
		}
		using IEnumerator<EntityUid> enumerator = args.HitEntities.GetEnumerator();
		if (enumerator.MoveNext())
		{
			EntityUid entity = enumerator.Current;
			if (_xeno.CanAbilityAttackTarget(Entity<XenoMeleeSlowComponent>.op_Implicit(xeno), entity) && (!xeno.Comp.RequiresKnockDown || _standing.IsDown(entity)))
			{
				TimeSpan slow = (xeno.Comp.HigherOnXenos ? _xeno.TryApplyXenoDebuffMultiplier(entity, xeno.Comp.SlowTime) : xeno.Comp.SlowTime);
				_slow.TrySlowdown(entity, slow, refresh: true, ignoreDurationModifier: true);
			}
		}
	}
}
