using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Stab;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Empower;

public sealed class XenoEmpowerSystem : EntitySystem
{
	[Dependency]
	private XenoPlasmaSystem _plasma;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private XenoShieldSystem _shield;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private SharedRMCEmoteSystem _emote;

	[Dependency]
	private SharedAuraSystem _aura;

	[Dependency]
	private TurfSystem _turf;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private DamageableSystem _damagable;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private RMCSizeStunSystem _sizeStun;

	private readonly HashSet<Entity<MobStateComponent>> _mobs = new HashSet<Entity<MobStateComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoEmpowerComponent, XenoEmpowerActionEvent>((EntityEventRefHandler<XenoEmpowerComponent, XenoEmpowerActionEvent>)OnXenoEmpowerAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEmpowerComponent, BeforeDamageChangedEvent>((EntityEventRefHandler<XenoEmpowerComponent, BeforeDamageChangedEvent>)OnXenoEmpowerBeforeDamageChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoEmpowerComponent, RemovedShieldEvent>((EntityEventRefHandler<XenoEmpowerComponent, RemovedShieldEvent>)OnXenoEmpowerShieldRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoSuperEmpoweredComponent, GetMeleeDamageEvent>((EntityEventRefHandler<XenoSuperEmpoweredComponent, GetMeleeDamageEvent>)OnXenoSuperEmpoweredGetMeleeDamage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoSuperEmpoweredComponent, RMCGetTailStabBonusDamageEvent>((EntityEventRefHandler<XenoSuperEmpoweredComponent, RMCGetTailStabBonusDamageEvent>)OnXenoSuperEmpoweredGetTailDamage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoSuperEmpoweredComponent, XenoLeapHitEvent>((EntityEventRefHandler<XenoSuperEmpoweredComponent, XenoLeapHitEvent>)OnXenoSuperEmpoweredLeapHit, (Type[])null, (Type[])null);
	}

	private void OnXenoEmpowerBeforeDamageChanged(Entity<XenoEmpowerComponent> xeno, ref BeforeDamageChangedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!xeno.Comp.ShieldDecayAt.HasValue || !(args.Damage.GetTotal() <= 0))
		{
			xeno.Comp.ShieldDecayAt = _timing.CurTime + xeno.Comp.ShieldDecayTime;
		}
	}

	private void OnXenoEmpowerShieldRemoved(Entity<XenoEmpowerComponent> xeno, ref RemovedShieldEvent args)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (args.Type == XenoShieldSystem.ShieldType.Ravager && _net.IsServer)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-ravager-shield-end"), Entity<XenoEmpowerComponent>.op_Implicit(xeno), Entity<XenoEmpowerComponent>.op_Implicit(xeno), PopupType.SmallCaution);
		}
	}

	private void OnXenoEmpowerAction(Entity<XenoEmpowerComponent> xeno, ref XenoEmpowerActionEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (!xeno.Comp.ActivatedOnce)
		{
			_actions.SetUseDelay(args.Action.AsNullable(), TimeSpan.Zero);
			if (!_plasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.Cost))
			{
				return;
			}
			xeno.Comp.ActivatedOnce = true;
			_shield.ApplyShield(Entity<XenoEmpowerComponent>.op_Implicit(xeno), XenoShieldSystem.ShieldType.Ravager, xeno.Comp.InitialShield);
			xeno.Comp.ShieldDecayAt = _timing.CurTime + xeno.Comp.ShieldDecayTime;
			xeno.Comp.TimeoutAt = _timing.CurTime + xeno.Comp.TimeoutDuration;
			xeno.Comp.FirstActivationAt = _timing.CurTime;
			foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoEmpowerActionEvent>(Entity<XenoEmpowerComponent>.op_Implicit(xeno)))
			{
				_actions.SetToggled(action.AsNullable(), toggled: true);
			}
			_popup.PopupPredicted(base.Loc.GetString("rmc-xeno-empower-start-self"), base.Loc.GetString("rmc-xeno-empower-start-others", (ValueTuple<string, object>)("user", xeno)), Entity<XenoEmpowerComponent>.op_Implicit(xeno), Entity<XenoEmpowerComponent>.op_Implicit(xeno), PopupType.MediumCaution);
		}
		else
		{
			FullEmpower(xeno);
		}
	}

	private void FullEmpower(Entity<XenoEmpowerComponent> xeno)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoEmpowerActionEvent>(Entity<XenoEmpowerComponent>.op_Implicit(xeno)))
		{
			_actions.SetToggled(action.AsNullable(), toggled: false);
		}
		((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.EmpowerEffect), xeno.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		xeno.Comp.ActivatedOnce = false;
		_mobs.Clear();
		_lookup.GetEntitiesInRange<MobStateComponent>(xeno.Owner.ToCoordinates(), xeno.Comp.Range, _mobs, (LookupFlags)110);
		int hits = 0;
		foreach (Entity<MobStateComponent> ent in _mobs)
		{
			if (_examine.InRangeUnOccluded(xeno.Owner, Entity<MobStateComponent>.op_Implicit(ent), xeno.Comp.Range) && _xeno.CanAbilityAttackTarget(Entity<XenoEmpowerComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(ent)) && !((EntitySystem)this).HasComp<XenoNestedComponent>(Entity<MobStateComponent>.op_Implicit(ent)))
			{
				hits++;
				TileRef? tileRef = _turf.GetTileRef(ent.Owner.ToCoordinates());
				if (tileRef.HasValue)
				{
					TileRef tile = tileRef.GetValueOrDefault();
					((EntitySystem)this).SpawnAtPosition(EntProtoId.op_Implicit(xeno.Comp.TargetEffect), _turf.GetTileCenter(tile), (ComponentRegistry)null);
				}
				if (hits >= xeno.Comp.MaxTargets)
				{
					break;
				}
			}
		}
		if (hits > 0)
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-ravager-empower"), Entity<XenoEmpowerComponent>.op_Implicit(xeno), Entity<XenoEmpowerComponent>.op_Implicit(xeno), PopupType.SmallCaution);
		}
		else
		{
			_popup.PopupEntity(base.Loc.GetString("rmc-xeno-ravager-empower-fizzle"), Entity<XenoEmpowerComponent>.op_Implicit(xeno), Entity<XenoEmpowerComponent>.op_Implicit(xeno), PopupType.SmallCaution);
		}
		_shield.ApplyShield(Entity<XenoEmpowerComponent>.op_Implicit(xeno), XenoShieldSystem.ShieldType.Ravager, hits * xeno.Comp.ShieldPerTarget);
		xeno.Comp.ShieldDecayAt = _timing.CurTime + xeno.Comp.ShieldDecayTime;
		if (hits >= xeno.Comp.SuperThreshold)
		{
			_emote.TryEmoteWithChat(Entity<XenoEmpowerComponent>.op_Implicit(xeno), xeno.Comp.RoarEmote);
			_aura.GiveAura(Entity<XenoEmpowerComponent>.op_Implicit(xeno), xeno.Comp.SuperEmpowerColor, xeno.Comp.SuperEmpowerPartialDuration, 4f);
			XenoSuperEmpoweredComponent xenoSuperEmpoweredComponent = ((EntitySystem)this).EnsureComp<XenoSuperEmpoweredComponent>(Entity<XenoEmpowerComponent>.op_Implicit(xeno));
			xenoSuperEmpoweredComponent.PartialExpireAt = _timing.CurTime + xeno.Comp.SuperEmpowerPartialDuration;
			xenoSuperEmpoweredComponent.EmpoweredTargets = hits;
			xenoSuperEmpoweredComponent.DamageIncreasePer = xeno.Comp.DamageIncreasePer;
			xenoSuperEmpoweredComponent.DamageTailIncreasePer = xeno.Comp.DamageTailIncreasePer;
			xenoSuperEmpoweredComponent.LeapDamage = xeno.Comp.LeapDamage;
		}
		else
		{
			_emote.TryEmoteWithChat(Entity<XenoEmpowerComponent>.op_Implicit(xeno), xeno.Comp.TailEmote);
		}
		((EntitySystem)this).Dirty<XenoEmpowerComponent>(xeno, (MetaDataComponent)null);
		xeno.Comp.TimeoutAt = null;
		DoCooldown(xeno);
	}

	private void OnXenoSuperEmpoweredGetMeleeDamage(Entity<XenoSuperEmpoweredComponent> xeno, ref GetMeleeDamageEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		args.Damage += xeno.Comp.DamageIncreasePer * xeno.Comp.EmpoweredTargets;
	}

	private void OnXenoSuperEmpoweredGetTailDamage(Entity<XenoSuperEmpoweredComponent> xeno, ref RMCGetTailStabBonusDamageEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		args.Damage += xeno.Comp.DamageTailIncreasePer * xeno.Comp.EmpoweredTargets;
	}

	private void OnXenoSuperEmpoweredLeapHit(Entity<XenoSuperEmpoweredComponent> xeno, ref XenoLeapHitEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		if (!_xeno.CanAbilityAttackTarget(Entity<XenoSuperEmpoweredComponent>.op_Implicit(xeno), args.Hit))
		{
			return;
		}
		_rmcPulling.TryStopAllPullsFromAndOn(args.Hit);
		if (_damagable.TryChangeDamage(args.Hit, xeno.Comp.LeapDamage, ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoSuperEmpoweredComponent>.op_Implicit(xeno), Entity<XenoSuperEmpoweredComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
		{
			Filter filter = Filter.Pvs(args.Hit, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
			_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { args.Hit }, filter);
		}
		if (!_net.IsClient)
		{
			_stun.TryParalyze(args.Hit, xeno.Comp.StunDuration, refresh: true);
			MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoSuperEmpoweredComponent>.op_Implicit(xeno), (TransformComponent)null);
			_sizeStun.KnockBack(args.Hit, origin, xeno.Comp.FlingDistance, xeno.Comp.FlingDistance, 10f, ignoreSize: true);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoEmpowerComponent> empowerQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoEmpowerComponent>();
		EntityUid uid = default(EntityUid);
		XenoEmpowerComponent empower = default(XenoEmpowerComponent);
		while (empowerQuery.MoveNext(ref uid, ref empower))
		{
			if (empower.ShieldDecayAt.HasValue)
			{
				TimeSpan value = time;
				TimeSpan? shieldDecayAt = empower.ShieldDecayAt;
				if (value >= shieldDecayAt)
				{
					_shield.RemoveShield(uid, XenoShieldSystem.ShieldType.Ravager);
					empower.ShieldDecayAt = null;
				}
			}
			if (empower.TimeoutAt.HasValue)
			{
				TimeSpan value = time;
				TimeSpan? shieldDecayAt = empower.TimeoutAt;
				if (!(value < shieldDecayAt))
				{
					FullEmpower(Entity<XenoEmpowerComponent>.op_Implicit((uid, empower)));
				}
			}
		}
		EntityQueryEnumerator<XenoSuperEmpoweredComponent> superQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoSuperEmpoweredComponent>();
		EntityUid uid2 = default(EntityUid);
		XenoSuperEmpoweredComponent super = default(XenoSuperEmpoweredComponent);
		while (superQuery.MoveNext(ref uid2, ref super))
		{
			bool hasValue = super.ExpiresAt.HasValue;
			TimeSpan value = time;
			TimeSpan? shieldDecayAt = super.ExpiresAt;
			if (hasValue & (value >= shieldDecayAt))
			{
				((EntitySystem)this).RemCompDeferred<XenoSuperEmpoweredComponent>(uid2);
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-ravager-super-empower-fade"), uid2, uid2, PopupType.SmallCaution);
			}
			else if (!super.ExpiresAt.HasValue && !(time < super.PartialExpireAt))
			{
				_aura.GiveAura(uid2, super.FadingEmpowerColor, super.ExpireTime, 3f);
				super.ExpiresAt = time + super.ExpireTime;
			}
		}
	}

	private void DoCooldown(Entity<XenoEmpowerComponent> xeno)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		foreach (Entity<ActionComponent> item in _rmcActions.GetActionsWithEvent<XenoEmpowerActionEvent>(Entity<XenoEmpowerComponent>.op_Implicit(xeno)))
		{
			Entity<ActionComponent> actionEnt = item.AsNullable();
			_actions.SetToggled(actionEnt, toggled: false);
			TimeSpan cooldownTime = xeno.Comp.CooldownDuration - (_timing.CurTime - xeno.Comp.FirstActivationAt);
			_actions.SetUseDelay(actionEnt, cooldownTime);
			_actions.SetCooldown(actionEnt, cooldownTime);
		}
	}
}
