using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Xenonids.Heal;
using Content.Shared._RMC14.Xenonids.Rage;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Xenonids.Eviscerate;

public sealed class XenoEviscerateSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private SharedXenoHealSystem _xenoHeal;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private SharedInteractionSystem _interact;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private XenoRageSystem _rage;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedRMCEmoteSystem _emote;

	private readonly HashSet<Entity<MobStateComponent>> _hit = new HashSet<Entity<MobStateComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoEviscerateComponent, XenoEviscerateActionEvent>((EntityEventRefHandler<XenoEviscerateComponent, XenoEviscerateActionEvent>)OnXenoEviscerateAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEviscerateComponent, XenoEviscerateDoAfterEvent>((EntityEventRefHandler<XenoEviscerateComponent, XenoEviscerateDoAfterEvent>)OnXenoEviscerateDoAfter, (Type[])null, (Type[])null);
	}

	private void OnXenoEviscerateAction(Entity<XenoEviscerateComponent> xeno, ref XenoEviscerateActionEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		int rage = _rage.GetRage(xeno.Owner);
		if (rage <= 0)
		{
			_popup.PopupClient(base.Loc.GetString("rmc-xeno-eviscerate-fail"), Entity<XenoEviscerateComponent>.op_Implicit(xeno), Entity<XenoEviscerateComponent>.op_Implicit(xeno), PopupType.SmallCaution);
			return;
		}
		int listRage = rage - 1;
		TimeSpan windupReduction = xeno.Comp.WindupReductionAtRageLevels[listRage];
		TimeSpan windupTime = xeno.Comp.WindupTime - windupReduction;
		XenoEviscerateDoAfterEvent ev = new XenoEviscerateDoAfterEvent(listRage);
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoEviscerateComponent>.op_Implicit(xeno), windupTime, ev, Entity<XenoEviscerateComponent>.op_Implicit(xeno))
		{
			BreakOnMove = true,
			Hidden = true,
			RootEntity = true,
			MovementThreshold = 0.5f
		};
		if (_doAfter.TryStartDoAfter(doAfter))
		{
			((HandledEntityEventArgs)args).Handled = true;
			_stun.TrySlowdown(Entity<XenoEviscerateComponent>.op_Implicit(xeno), windupTime, refresh: false, 0f, 0f);
			_rage.IncrementRage(Entity<XenoRageComponent>.op_Implicit(xeno.Owner), -1);
			if (rage > 1)
			{
				string selfMsg = base.Loc.GetString("rmc-xeno-eviscerate-windup-self");
				string msg = base.Loc.GetString("rmc-xeno-eviscerate-windup", (ValueTuple<string, object>)("xeno", xeno));
				_popup.PopupPredicted(selfMsg, msg, Entity<XenoEviscerateComponent>.op_Implicit(xeno), Entity<XenoEviscerateComponent>.op_Implicit(xeno), PopupType.MediumCaution);
			}
			else
			{
				string selfMsg2 = base.Loc.GetString("rmc-xeno-eviscerate-windup-small-self");
				string msg2 = base.Loc.GetString("rmc-xeno-eviscerate-windup-small", (ValueTuple<string, object>)("xeno", xeno));
				_popup.PopupPredicted(selfMsg2, msg2, Entity<XenoEviscerateComponent>.op_Implicit(xeno), Entity<XenoEviscerateComponent>.op_Implicit(xeno), PopupType.MediumCaution);
			}
		}
	}

	private void OnXenoEviscerateDoAfter(Entity<XenoEviscerateComponent> xeno, ref XenoEviscerateDoAfterEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled)
		{
			return;
		}
		((EntitySystem)this).EnsureComp<XenoSweepingComponent>(Entity<XenoEviscerateComponent>.op_Implicit(xeno));
		_audio.PlayPredicted(xeno.Comp.Sound, Entity<XenoEviscerateComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoEviscerateComponent>.op_Implicit(xeno), (AudioParams?)null);
		_rmcPulling.TryStopAllPullsFromAndOn(Entity<XenoEviscerateComponent>.op_Implicit(xeno));
		_emote.TryEmoteWithChat(Entity<XenoEviscerateComponent>.op_Implicit(xeno), xeno.Comp.RoarEmote);
		DamageSpecifier damage = xeno.Comp.DamageAtRageLevels[args.Rage];
		float range = xeno.Comp.RangeAtRageLevels[args.Rage];
		TransformComponent transform = ((EntitySystem)this).Transform(xeno.Owner);
		if (_net.IsClient)
		{
			return;
		}
		_hit.Clear();
		_entityLookup.GetEntitiesInRange<MobStateComponent>(transform.Coordinates, range, _hit, (LookupFlags)110);
		int validTargets = 0;
		_transform.GetMapCoordinates(Entity<XenoEviscerateComponent>.op_Implicit(xeno), (TransformComponent)null);
		foreach (Entity<MobStateComponent> mob in _hit)
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoEviscerateComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(mob)) && _interact.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(xeno.Owner), Entity<TransformComponent>.op_Implicit(mob.Owner), range))
			{
				_rmcPulling.TryStopAllPullsFromAndOn(Entity<MobStateComponent>.op_Implicit(mob));
				_damageable.TryChangeDamage(Entity<MobStateComponent>.op_Implicit(mob), _xeno.TryApplyXenoSlashDamageMultiplier(Entity<MobStateComponent>.op_Implicit(mob), damage), ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoEviscerateComponent>.op_Implicit(xeno), Entity<XenoEviscerateComponent>.op_Implicit(xeno));
				Filter filter = Filter.Pvs(Entity<MobStateComponent>.op_Implicit(mob), 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null);
				_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { Entity<MobStateComponent>.op_Implicit(mob) }, filter);
				if (range > 1.5f)
				{
					_audio.PlayPvs(xeno.Comp.RageHitSound, Entity<MobStateComponent>.op_Implicit(mob), (AudioParams?)null);
					_stun.TryParalyze(Entity<MobStateComponent>.op_Implicit(mob), xeno.Comp.StunTime, refresh: true);
				}
				else
				{
					_audio.PlayPvs(xeno.Comp.HitSound, Entity<MobStateComponent>.op_Implicit(mob), (AudioParams?)null);
				}
				validTargets++;
			}
		}
		int healAmount = Math.Clamp(validTargets * xeno.Comp.LifeStealPerMarine, 0, xeno.Comp.MaxLifeSteal);
		_xenoHeal.CreateHealStacks(Entity<XenoEviscerateComponent>.op_Implicit(xeno), healAmount, xeno.Comp.HealDelay, 1, xeno.Comp.HealDelay);
	}
}
