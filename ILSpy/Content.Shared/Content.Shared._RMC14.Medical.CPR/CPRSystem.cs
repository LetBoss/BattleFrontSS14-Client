using System;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Medical.Unrevivable;
using Content.Shared._RMC14.ShakeStun;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Medical.CPR;

public sealed class CPRSystem : EntitySystem
{
	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popups;

	[Dependency]
	private RMCUnrevivableSystem _unrevivable;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SkillsSystem _skills;

	public static readonly EntProtoId<SkillDefinitionComponent> SkillType = EntProtoId<SkillDefinitionComponent>.op_Implicit("RMCSkillMedical");

	private static readonly ProtoId<DamageTypePrototype> HealType = ProtoId<DamageTypePrototype>.op_Implicit("Asphyxiation");

	private static readonly FixedPoint2 HealAmount = FixedPoint2.New(10);

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<MarineComponent, InteractHandEvent>((EntityEventRefHandler<MarineComponent, InteractHandEvent>)OnMarineInteractHand, new Type[2]
		{
			typeof(InteractionPopupSystem),
			typeof(StunShakeableSystem)
		}, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MarineComponent, CPRDoAfterEvent>((EntityEventRefHandler<MarineComponent, CPRDoAfterEvent>)OnMarineDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ReceivingCPRComponent, ReceiveCPRAttemptEvent>((EntityEventRefHandler<ReceivingCPRComponent, ReceiveCPRAttemptEvent>)OnReceivingCPRAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<CPRReceivedComponent, ReceiveCPRAttemptEvent>((EntityEventRefHandler<CPRReceivedComponent, ReceiveCPRAttemptEvent>)OnReceivedCPRAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobStateComponent, ReceiveCPRAttemptEvent>((EntityEventRefHandler<MobStateComponent, ReceiveCPRAttemptEvent>)OnMobStateCPRAttempt, (Type[])null, (Type[])null);
	}

	private void OnMarineInteractHand(Entity<MarineComponent> ent, ref InteractHandEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = StartCPR(args.User, args.Target);
		}
	}

	private void OnMarineDoAfter(Entity<MarineComponent> ent, ref CPRDoAfterEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		EntityUid performer = args.User;
		if (args.Target.HasValue)
		{
			((EntitySystem)this).RemComp<ReceivingCPRComponent>(args.Target.Value);
		}
		if (args.Cancelled || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		if (!CanCPRPopup(performer, target2, start: false, out var damage))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		_unrevivable.AddRevivableTime(target2, TimeSpan.FromSeconds(7L));
		DamageableComponent damageable = default(DamageableComponent);
		if (!((EntitySystem)this).TryComp<DamageableComponent>(target2, ref damageable) || !damageable.Damage.DamageDict.TryGetValue(ProtoId<DamageTypePrototype>.op_Implicit(HealType), out damage))
		{
			return;
		}
		FixedPoint2 heal = -FixedPoint2.Min(damage, HealAmount);
		DamageSpecifier healSpecifier = new DamageSpecifier();
		healSpecifier.DamageDict.Add(ProtoId<DamageTypePrototype>.op_Implicit(HealType), heal);
		_damageable.TryChangeDamage(target2, healSpecifier, ignoreResistances: true);
		((EntitySystem)this).EnsureComp<CPRReceivedComponent>(target2).Last = _timing.CurTime;
		if (!_net.IsClient)
		{
			string selfPopup = base.Loc.GetString("cm-cpr-self-perform", (ValueTuple<string, object>)("target", target2), (ValueTuple<string, object>)("seconds", 7));
			_popups.PopupEntity(selfPopup, target2, performer, PopupType.Medium);
			string othersPopup = base.Loc.GetString("cm-cpr-other-perform", (ValueTuple<string, object>)("performer", performer), (ValueTuple<string, object>)("target", target2));
			Filter othersFilter = Filter.Pvs(performer, 2f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid e) => e == performer));
			_popups.PopupEntity(othersPopup, performer, othersFilter, recordReplay: true, PopupType.Medium);
		}
	}

	private void OnReceivingCPRAttempt(Entity<ReceivingCPRComponent> ent, ref ReceiveCPRAttemptEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (_timing.CurTime - ent.Comp.StartTime > TimeSpan.FromSeconds(7L))
		{
			((EntitySystem)this).RemCompDeferred<ReceivingCPRComponent>(Entity<ReceivingCPRComponent>.op_Implicit(ent));
			return;
		}
		args.Cancelled = true;
		if (!_net.IsClient)
		{
			string popup = base.Loc.GetString("cm-cpr-already-being-performed", (ValueTuple<string, object>)("target", ent.Owner));
			_popups.PopupEntity(popup, Entity<ReceivingCPRComponent>.op_Implicit(ent), args.Performer, PopupType.Medium);
		}
	}

	private void OnReceivedCPRAttempt(Entity<CPRReceivedComponent> ent, ref ReceiveCPRAttemptEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		if (args.Start)
		{
			return;
		}
		EntityUid target = ent.Owner;
		EntityUid performer = args.Performer;
		if (!_mobState.IsDead(Entity<CPRReceivedComponent>.op_Implicit(ent)) || ent.Comp.Last <= _timing.CurTime - TimeSpan.FromSeconds(7L))
		{
			return;
		}
		args.Cancelled = true;
		if (!_net.IsClient)
		{
			string selfPopup = base.Loc.GetString("cm-cpr-self-perform-fail-received-too-recently", (ValueTuple<string, object>)("target", target));
			_popups.PopupEntity(selfPopup, target, performer, PopupType.MediumCaution);
			string othersPopup = base.Loc.GetString("cm-cpr-other-perform-fail", (ValueTuple<string, object>)("performer", performer), (ValueTuple<string, object>)("target", target));
			Filter othersFilter = Filter.Pvs(performer, 2f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid e) => e == performer));
			_popups.PopupEntity(othersPopup, performer, othersFilter, recordReplay: true, PopupType.MediumCaution);
		}
	}

	private void OnMobStateCPRAttempt(Entity<MobStateComponent> ent, ref ReceiveCPRAttemptEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && (_mobState.IsAlive(Entity<MobStateComponent>.op_Implicit(ent)) || (_mobState.IsDead(Entity<MobStateComponent>.op_Implicit(ent)) && _unrevivable.IsUnrevivable(Entity<MobStateComponent>.op_Implicit(ent)))))
		{
			args.Cancelled = true;
		}
	}

	private bool CanCPRPopup(EntityUid performer, EntityUid target, bool start, out FixedPoint2 damage)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		damage = default(FixedPoint2);
		if (!((EntitySystem)this).HasComp<MarineComponent>(target) || !((EntitySystem)this).HasComp<MarineComponent>(performer))
		{
			return false;
		}
		PerformCPRAttemptEvent performAttempt = new PerformCPRAttemptEvent(target);
		((EntitySystem)this).RaiseLocalEvent<PerformCPRAttemptEvent>(performer, ref performAttempt, false);
		if (performAttempt.Cancelled)
		{
			return false;
		}
		ReceiveCPRAttemptEvent receiveAttempt = new ReceiveCPRAttemptEvent(performer, target, start);
		((EntitySystem)this).RaiseLocalEvent<ReceiveCPRAttemptEvent>(target, ref receiveAttempt, false);
		if (receiveAttempt.Cancelled)
		{
			return false;
		}
		if (!_hands.TryGetEmptyHand(Entity<HandsComponent>.op_Implicit(performer), out string _))
		{
			return false;
		}
		return true;
	}

	private bool StartCPR(EntityUid performer, EntityUid target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		if (!CanCPRPopup(performer, target, start: true, out var _))
		{
			return false;
		}
		ReceivingCPRComponent cprComp = ((EntitySystem)this).EnsureComp<ReceivingCPRComponent>(target);
		cprComp.StartTime = _timing.CurTime;
		((EntitySystem)this).Dirty(target, (IComponent)(object)cprComp, (MetaDataComponent)null);
		TimeSpan delay = TimeSpan.FromSeconds((float)cprComp.CPRPerformingTime * _skills.GetSkillDelayMultiplier(Entity<SkillsComponent>.op_Implicit(performer), SkillType));
		DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, performer, delay, new CPRDoAfterEvent(), performer, target)
		{
			BreakOnMove = true,
			NeedHand = true,
			BlockDuplicate = true,
			DuplicateCondition = DuplicateConditions.SameEvent,
			TargetEffect = EntProtoId.op_Implicit("RMCEffectHealBusy")
		};
		_doAfter.TryStartDoAfter(doAfter);
		if (_net.IsClient)
		{
			return true;
		}
		string selfPopup = base.Loc.GetString("cm-cpr-self-start-perform", (ValueTuple<string, object>)("target", target));
		_popups.PopupEntity(selfPopup, target, performer, PopupType.Medium);
		string othersPopup = base.Loc.GetString("cm-cpr-other-start-perform", (ValueTuple<string, object>)("performer", performer), (ValueTuple<string, object>)("target", target));
		Filter othersFilter = Filter.Pvs(performer, 2f, (IEntityManager)null, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid e) => e == performer));
		_popups.PopupEntity(othersPopup, performer, othersFilter, recordReplay: true, PopupType.Medium);
		return true;
	}
}
