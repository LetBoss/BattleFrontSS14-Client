using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Heal;
using Content.Shared._RMC14.Xenonids.Rage;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
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

namespace Content.Shared._RMC14.Xenonids.Fling;

public sealed class XenoFlingSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private RMCSlowSystem _rmcSlow;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private SharedRMCMeleeWeaponSystem _rmcMelee;

	[Dependency]
	private SharedXenoHealSystem _xenoHeal;

	[Dependency]
	private XenoRageSystem _rage;

	[Dependency]
	private RMCSizeStunSystem _size;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private RMCDazedSystem _daze;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoFlingComponent, XenoFlingActionEvent>((EntityEventRefHandler<XenoFlingComponent, XenoFlingActionEvent>)OnXenoFlingAction, (Type[])null, (Type[])null);
	}

	private void OnXenoFlingAction(Entity<XenoFlingComponent> xeno, ref XenoFlingActionEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		if (!_xeno.CanAbilityAttackTarget(Entity<XenoFlingComponent>.op_Implicit(xeno), args.Target) || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		XenoFlingAttemptEvent attempt = default(XenoFlingAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoFlingAttemptEvent>(Entity<XenoFlingComponent>.op_Implicit(xeno), ref attempt, false);
		if (attempt.Cancelled)
		{
			return;
		}
		if (_size.TryGetSize(args.Target, out var size) && (int)size >= 5)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-fling-too-big", (ValueTuple<string, object>)("target", args.Target)), Entity<XenoFlingComponent>.op_Implicit(xeno), Entity<XenoFlingComponent>.op_Implicit(xeno), PopupType.MediumCaution);
			return;
		}
		if (_net.IsServer)
		{
			((HandledEntityEventArgs)args).Handled = true;
			_audio.PlayPvs(xeno.Comp.Sound, Entity<XenoFlingComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
		int rage = _rage.GetRage(xeno.Owner);
		EntityUid targetId = args.Target;
		_rmcPulling.TryStopAllPullsFromAndOn(targetId);
		if (_damageable.TryChangeDamage(targetId, _xeno.TryApplyXenoSlashDamageMultiplier(targetId, xeno.Comp.Damage), ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoFlingComponent>.op_Implicit(xeno), Entity<XenoFlingComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
		{
			Filter filter = Filter.Pvs(targetId, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
			_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { targetId }, filter);
		}
		int healAmount = xeno.Comp.HealAmount;
		float throwRange = xeno.Comp.Range;
		bool daze = false;
		if (rage >= 2)
		{
			throwRange += xeno.Comp.EnragedRange;
			healAmount += xeno.Comp.EnragedHealAmount;
			daze = true;
		}
		MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoFlingComponent>.op_Implicit(xeno), (TransformComponent)null);
		_rmcMelee.DoLunge(Entity<XenoFlingComponent>.op_Implicit(xeno), targetId);
		_xenoHeal.CreateHealStacks(Entity<XenoFlingComponent>.op_Implicit(xeno), healAmount, xeno.Comp.HealDelay, 1, xeno.Comp.HealDelay);
		if (_net.IsServer)
		{
			_rmcSlow.TrySlowdown(targetId, xeno.Comp.SlowTime);
			_stun.TryParalyze(targetId, _xeno.TryApplyXenoDebuffMultiplier(targetId, xeno.Comp.ParalyzeTime), refresh: true);
			if (daze)
			{
				_daze.TryDaze(targetId, xeno.Comp.DazeTime);
			}
			_daze.TryDaze(targetId, xeno.Comp.DazeTime, refresh: true);
			_size.KnockBack(targetId, origin, throwRange, throwRange, xeno.Comp.ThrowSpeed);
			((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.Effect), targetId.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		}
	}
}
