using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Standing;
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

namespace Content.Shared._RMC14.Xenonids.Stomp;

public sealed class XenoStompSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private RMCSizeStunSystem _size;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	private readonly HashSet<Entity<MobStateComponent>> _receivers = new HashSet<Entity<MobStateComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoStompComponent, XenoStompActionEvent>((EntityEventRefHandler<XenoStompComponent, XenoStompActionEvent>)OnXenoStompAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoStompComponent, XenoStompDoAfterEvent>((EntityEventRefHandler<XenoStompComponent, XenoStompDoAfterEvent>)OnXenoStompDoAfter, (Type[])null, (Type[])null);
	}

	private void OnXenoStompAction(Entity<XenoStompComponent> xeno, ref XenoStompActionEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		XenoStompAttemptEvent attemptEv = default(XenoStompAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoStompAttemptEvent>(Entity<XenoStompComponent>.op_Implicit(xeno), ref attemptEv, false);
		if (!attemptEv.Cancelled && !_mobState.IsDead(Entity<XenoStompComponent>.op_Implicit(xeno)) && _xenoPlasma.HasPlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost))
		{
			((HandledEntityEventArgs)args).Handled = true;
			XenoStompDoAfterEvent ev = new XenoStompDoAfterEvent();
			DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoStompComponent>.op_Implicit(xeno), xeno.Comp.Delay, ev, Entity<XenoStompComponent>.op_Implicit(xeno))
			{
				BreakOnMove = true
			};
			_doAfter.TryStartDoAfter(doAfter);
		}
	}

	private void OnXenoStompDoAfter(Entity<XenoStompComponent> xeno, ref XenoStompDoAfterEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (args.Cancelled)
		{
			foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoStompActionEvent>(Entity<XenoStompComponent>.op_Implicit(xeno)))
			{
				_actions.ClearCooldown(action.AsNullable());
			}
			return;
		}
		TransformComponent xform = default(TransformComponent);
		if (!_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost) || !((EntitySystem)this).TryComp(Entity<XenoStompComponent>.op_Implicit(xeno), ref xform))
		{
			return;
		}
		_receivers.Clear();
		_entityLookup.GetEntitiesInRange<MobStateComponent>(xform.Coordinates, xeno.Comp.Range, _receivers, (LookupFlags)110);
		if (_net.IsServer)
		{
			_audio.PlayPvs(xeno.Comp.Sound, Entity<XenoStompComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
		float distance = default(float);
		foreach (Entity<MobStateComponent> receiver in _receivers)
		{
			if (!_xeno.CanAbilityAttackTarget(Entity<XenoStompComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(receiver)))
			{
				continue;
			}
			if (xeno.Comp.SlowBigInsteadOfStun && _size.TryGetSize(Entity<MobStateComponent>.op_Implicit(receiver), out var size) && (int)size >= 5)
			{
				_slow.TrySlowdown(Entity<MobStateComponent>.op_Implicit(receiver), xeno.Comp.DebuffsHurtXenosMore ? _xeno.TryApplyXenoDebuffMultiplier(Entity<MobStateComponent>.op_Implicit(receiver), xeno.Comp.ParalyzeTime) : xeno.Comp.ParalyzeTime);
			}
			else if (!xeno.Comp.ParalyzeUnderOnly)
			{
				_stun.TryParalyze(Entity<MobStateComponent>.op_Implicit(receiver), xeno.Comp.DebuffsHurtXenosMore ? _xeno.TryApplyXenoDebuffMultiplier(Entity<MobStateComponent>.op_Implicit(receiver), xeno.Comp.ParalyzeTime) : xeno.Comp.ParalyzeTime, refresh: true);
			}
			if (xeno.Comp.Slows)
			{
				_slow.TrySuperSlowdown(Entity<MobStateComponent>.op_Implicit(receiver), xeno.Comp.SlowTime);
			}
			EntityCoordinates coordinates = xform.Coordinates;
			if (!((EntityCoordinates)(ref coordinates)).TryDistance((IEntityManager)(object)base.EntityManager, receiver.Owner.ToCoordinates(), ref distance) || !(distance <= xeno.Comp.ShortRange) || !_standing.IsDown(Entity<MobStateComponent>.op_Implicit(receiver)))
			{
				continue;
			}
			if (_damageable.TryChangeDamage(Entity<MobStateComponent>.op_Implicit(receiver), _xeno.TryApplyXenoSlashDamageMultiplier(Entity<MobStateComponent>.op_Implicit(receiver), xeno.Comp.Damage), ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoStompComponent>.op_Implicit(xeno), Entity<XenoStompComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
			{
				Filter filter = Filter.Pvs(Entity<MobStateComponent>.op_Implicit(receiver), 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
				_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { Entity<MobStateComponent>.op_Implicit(receiver) }, filter);
			}
			if (xeno.Comp.ParalyzeUnderOnly && _size.TryGetSize(Entity<MobStateComponent>.op_Implicit(receiver), out size) && (int)size < 5)
			{
				_stun.TryParalyze(Entity<MobStateComponent>.op_Implicit(receiver), xeno.Comp.DebuffsHurtXenosMore ? _xeno.TryApplyXenoDebuffMultiplier(Entity<MobStateComponent>.op_Implicit(receiver), xeno.Comp.ParalyzeTime) : xeno.Comp.ParalyzeTime, refresh: true);
			}
		}
		if (_net.IsServer)
		{
			EntProtoId? selfEffect = xeno.Comp.SelfEffect;
			if (selfEffect.HasValue)
			{
				selfEffect = xeno.Comp.SelfEffect;
				((EntitySystem)this).SpawnAttachedTo(selfEffect.HasValue ? EntProtoId.op_Implicit(selfEffect.GetValueOrDefault()) : null, xeno.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			}
		}
	}
}
