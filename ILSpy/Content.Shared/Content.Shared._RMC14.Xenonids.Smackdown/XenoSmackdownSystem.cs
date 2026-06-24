using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Slow;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Xenonids.Smackdown;

public sealed class XenoSmackdownSystem : EntitySystem
{
	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoSmackdownComponent, MeleeHitEvent>((EntityEventRefHandler<XenoSmackdownComponent, MeleeHitEvent>)OnSmackdownMelee, (Type[])null, (Type[])null);
	}

	private void OnSmackdownMelee(Entity<XenoSmackdownComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid ent in args.HitEntities)
		{
			if (!_xeno.CanAbilityAttackTarget(Entity<XenoSmackdownComponent>.op_Implicit(xeno), ent) || (!((EntitySystem)this).HasComp<RMCSlowdownComponent>(ent) && !((EntitySystem)this).HasComp<RMCSuperSlowdownComponent>(ent) && !((EntitySystem)this).HasComp<RMCRootedComponent>(ent) && !((EntitySystem)this).HasComp<StunnedComponent>(ent) && !_standing.IsDown(ent)))
			{
				continue;
			}
			if (_damageable.TryChangeDamage(ent, _xeno.TryApplyXenoSlashDamageMultiplier(ent, xeno.Comp.Damage), ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoSmackdownComponent>.op_Implicit(xeno), Entity<XenoSmackdownComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
			{
				Filter filter = Filter.Pvs(ent, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
				_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { ent }, filter);
			}
			_audio.PlayPredicted(xeno.Comp.Sound, Entity<XenoSmackdownComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoSmackdownComponent>.op_Implicit(xeno), (AudioParams?)null);
			break;
		}
	}
}
