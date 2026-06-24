using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Rotate;
using Content.Shared._RMC14.Xenonids.ScissorCut;
using Content.Shared._RMC14.Xenonids.Stab;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.TailJab;

public sealed class XenoTailJabSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private DamageableSystem _damage;

	[Dependency]
	private SharedRMCEmoteSystem _emote;

	[Dependency]
	private SharedColorFlashEffectSystem _flash;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private SharedRMCMeleeWeaponSystem _rmcMelee;

	[Dependency]
	private RMCObstacleSlammingSystem _rmcObstacleSlamming;

	[Dependency]
	private RMCSlowSystem _rmcSlow;

	[Dependency]
	private XenoRotateSystem _rotate;

	[Dependency]
	private RMCSizeStunSystem _size;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private XenoSystem _xeno;

	private const string WindowBonusDamageType = "Structural";

	private const int WindowDamageBonus = 100;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoTailJabComponent, XenoTailJabActionEvent>((EntityEventRefHandler<XenoTailJabComponent, XenoTailJabActionEvent>)OnXenoImpaleAction, (Type[])null, (Type[])null);
	}

	private void OnXenoImpaleAction(Entity<XenoTailJabComponent> xeno, ref XenoTailJabActionEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_rmcActions.TryUseAction(args))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid target = args.Target;
		DamageSpecifier damage = new DamageSpecifier(xeno.Comp.Damage);
		RMCGetTailStabBonusDamageEvent ev = new RMCGetTailStabBonusDamageEvent(new DamageSpecifier());
		((EntitySystem)this).RaiseLocalEvent<RMCGetTailStabBonusDamageEvent>(Entity<XenoTailJabComponent>.op_Implicit(xeno), ref ev, false);
		damage += ev.Damage;
		if (((EntitySystem)this).HasComp<DestroyOnXenoPierceScissorComponent>(target))
		{
			damage.DamageDict.TryAdd("Structural", 100);
		}
		if (_damage.TryChangeDamage(target, _xeno.TryApplyXenoSlashDamageMultiplier(target, damage), ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoTailJabComponent>.op_Implicit(xeno), Entity<XenoTailJabComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
		{
			Filter filter = Filter.Pvs(target, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
			_flash.RaiseEffect(Color.Red, new List<EntityUid> { target }, filter);
		}
		_rmcMelee.DoLunge(Entity<XenoTailJabComponent>.op_Implicit(xeno), target);
		_rmcSlow.TrySlowdown(target, xeno.Comp.SlowdownTime);
		_rmcObstacleSlamming.ApplyBonuses(target, xeno.Comp.WallSlamStunTime, xeno.Comp.WallSlamSlowdownTime);
		MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoTailJabComponent>.op_Implicit(xeno), (TransformComponent)null);
		_size.KnockBack(target, origin, xeno.Comp.ThrowRange, xeno.Comp.ThrowRange);
		Angle val = _transform.GetWorldRotation(Entity<XenoTailJabComponent>.op_Implicit(xeno));
		Angle angle = DirectionExtensions.ToAngle(((Angle)(ref val)).GetDir()) - Angle.FromDegrees(180.0);
		_rotate.RotateXeno(Entity<XenoTailJabComponent>.op_Implicit(xeno), ((Angle)(ref angle)).GetDir());
		if (!_net.IsClient)
		{
			_audio.PlayPvs(xeno.Comp.Sound, Entity<XenoTailJabComponent>.op_Implicit(xeno), (AudioParams?)null);
			_emote.TryEmoteWithChat(Entity<XenoTailJabComponent>.op_Implicit(xeno), xeno.Comp.Emote, hideLog: false, null, ignoreActionBlocker: false, forceEmote: false, xeno.Comp.EmoteCooldown);
			string text = EntProtoId.op_Implicit(xeno.Comp.AttackEffect);
			EntityCoordinates val2 = target.ToCoordinates();
			val = default(Angle);
			((EntitySystem)this).SpawnAttachedTo(text, val2, (ComponentRegistry)null, val);
		}
	}
}
