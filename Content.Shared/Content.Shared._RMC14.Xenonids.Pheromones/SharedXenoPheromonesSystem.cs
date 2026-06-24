using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Xenonids.CriticalGrace;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Stab;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Atmos.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Threading;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Pheromones;

public abstract class SharedXenoPheromonesSystem : EntitySystem
{
	private record struct PheromonesJob(EntityLookupSystem Lookup) : IParallelRobustJob, IParallelRangeRobustJob
	{
		public int BatchSize => 8;

		public ValueList<(EntityUid Uid, XenoActivePheromonesComponent Active, XenoPheromonesComponent Pheromones, TransformComponent Xform)> Pheromones = default(ValueList<(EntityUid, XenoActivePheromonesComponent, XenoPheromonesComponent, TransformComponent)>);

		public ValueList<(bool Update, HashSet<Entity<XenoComponent>> Receivers)> Receivers = default(ValueList<(bool, HashSet<Entity<XenoComponent>>)>);

		public void Execute(int index)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			ref(bool, HashSet<Entity<XenoComponent>>) receivers = ref Receivers[index];
			if (receivers.Item1)
			{
				(EntityUid, XenoActivePheromonesComponent, XenoPheromonesComponent, TransformComponent) tuple = Pheromones[index];
				XenoPheromonesComponent pheromones = tuple.Item3;
				TransformComponent xform = tuple.Item4;
				receivers.Item2.Clear();
				Lookup.GetEntitiesInRange<XenoComponent>(xform.Coordinates, (float)pheromones.PheromonesRange, receivers.Item2, (LookupFlags)110);
			}
		}
	}

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private MobThresholdSystem _mobThreshold;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IParallelManager _parallel;

	[Dependency]
	private SharedUserInterfaceSystem _ui;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private SharedRMCDamageableSystem _rmcDamageable;

	[Dependency]
	private SharedRMCFlammableSystem _rmcFlammable;

	[Dependency]
	private SharedXenoWeedsSystem _weeds;

	[Dependency]
	private IPrototypeManager _protoManager;

	private readonly TimeSpan _pheromonePlasmaUseDelay = TimeSpan.FromSeconds(1L);

	private readonly HashSet<EntityUid>[] _oldReceivers = (from _ in Enum.GetValues<XenoPheromones>()
		select new HashSet<EntityUid>()).ToArray();

	private readonly HashSet<EntityUid> _refreshSpeeds = new HashSet<EntityUid>();

	private EntityQuery<DamageableComponent> _damageableQuery;

	private PheromonesJob _pheromonesJob;

	public override void Initialize()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_pheromonesJob = new PheromonesJob(_entityLookup);
		_damageableQuery = ((EntitySystem)this).GetEntityQuery<DamageableComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoPheromonesComponent, XenoPheromonesActionEvent>((EntityEventRefHandler<XenoPheromonesComponent, XenoPheromonesActionEvent>)OnXenoPheromonesAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoPheromonesComponent, PlayerDetachedEvent>((EntityEventRefHandler<XenoPheromonesComponent, PlayerDetachedEvent>)OnXenoPheromonesDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWardingPheromonesComponent, UpdateMobStateEvent>((EntityEventRefHandler<XenoWardingPheromonesComponent, UpdateMobStateEvent>)OnWardingUpdateMobState, (Type[])null, new Type[1] { typeof(MobThresholdSystem) });
		((EntitySystem)this).SubscribeLocalEvent<XenoWardingPheromonesComponent, ComponentRemove>((EntityEventRefHandler<XenoWardingPheromonesComponent, ComponentRemove>)OnWardingRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWardingPheromonesComponent, DamageStateCritBeforeDamageEvent>((EntityEventRefHandler<XenoWardingPheromonesComponent, DamageStateCritBeforeDamageEvent>)OnWardingDamageCritModify, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoWardingPheromonesComponent, GetCriticalGraceTimeEvent>((EntityEventRefHandler<XenoWardingPheromonesComponent, GetCriticalGraceTimeEvent>)OnWardingGetGraceTime, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFrenzyPheromonesComponent, ComponentRemove>((EntityEventRefHandler<XenoFrenzyPheromonesComponent, ComponentRemove>)OnFrenzyRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFrenzyPheromonesComponent, GetMeleeDamageEvent>((EntityEventRefHandler<XenoFrenzyPheromonesComponent, GetMeleeDamageEvent>)OnFrenzyGetMeleeDamage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFrenzyPheromonesComponent, RMCGetTailStabBonusDamageEvent>((EntityEventRefHandler<XenoFrenzyPheromonesComponent, RMCGetTailStabBonusDamageEvent>)OnFrenzyGetTailStabDamage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFrenzyPheromonesComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<XenoFrenzyPheromonesComponent, RefreshMovementSpeedModifiersEvent>)OnFrenzyMovementSpeedModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoFrenzyPheromonesComponent, PullStartedMessage>((EntityEventRefHandler<XenoFrenzyPheromonesComponent, PullStartedMessage>)OnFrenzyPullStarted, (Type[])null, new Type[1] { typeof(RMCPullingSystem) });
		((EntitySystem)this).SubscribeLocalEvent<XenoFrenzyPheromonesComponent, PullStoppedMessage>((EntityEventRefHandler<XenoFrenzyPheromonesComponent, PullStoppedMessage>)OnFrenzyPullStopped, (Type[])null, new Type[1] { typeof(RMCPullingSystem) });
		((EntitySystem)this).SubscribeLocalEvent<XenoActivePheromonesComponent, MobStateChangedEvent>((EntityEventRefHandler<XenoActivePheromonesComponent, MobStateChangedEvent>)OnActiveMobStateChanged, (Type[])null, (Type[])null);
		BoundUserInterfaceRegisterExt.BuiEvents<XenoPheromonesComponent>(((EntitySystem)this).Subs, (object)XenoPheromonesUI.Key, (BuiEventSubscriber<XenoPheromonesComponent>)delegate(Subscriber<XenoPheromonesComponent> subs)
		{
			subs.Event<XenoPheromonesChosenBuiMsg>((EntityEventRefHandler<XenoPheromonesComponent, XenoPheromonesChosenBuiMsg>)OnXenoPheromonesChosenBui);
		});
	}

	private void OnActiveMobStateChanged(Entity<XenoActivePheromonesComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		MobState newMobState = args.NewMobState;
		if (newMobState - 2 <= MobState.Alive)
		{
			DeactivatePheromones(Entity<XenoPheromonesComponent>.op_Implicit(ent.Owner));
		}
	}

	private void OnXenoPheromonesAction(Entity<XenoPheromonesComponent> xeno, ref XenoPheromonesActionEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		((HandledEntityEventArgs)args).Handled = true;
		DeactivatePheromones(xeno.AsNullable());
		_ui.TryOpenUi(Entity<UserInterfaceComponent>.op_Implicit(xeno.Owner), (Enum)XenoPheromonesUI.Key, Entity<XenoPheromonesComponent>.op_Implicit(xeno), false);
	}

	private void OnXenoPheromonesDetached(Entity<XenoPheromonesComponent> xeno, ref PlayerDetachedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		DeactivatePheromones(xeno.AsNullable());
	}

	private void OnXenoPheromonesChosenBui(Entity<XenoPheromonesComponent> xeno, ref XenoPheromonesChosenBuiMsg args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		if (!Enum.IsDefined(args.Pheromones) || !_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PheromonesPlasmaCost))
		{
			return;
		}
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoPheromonesActionEvent>(Entity<XenoPheromonesComponent>.op_Implicit(xeno)))
		{
			_actions.SetToggled(action.AsNullable(), toggled: true);
		}
		string popup = base.Loc.GetString("cm-xeno-pheromones-start", (ValueTuple<string, object>)("pheromones", args.Pheromones.ToString()));
		_popup.PopupClient(popup, Entity<XenoPheromonesComponent>.op_Implicit(xeno), Entity<XenoPheromonesComponent>.op_Implicit(xeno));
		_ui.CloseUi(Entity<UserInterfaceComponent>.op_Implicit(xeno.Owner), (Enum)XenoPheromonesUI.Key, (EntityUid?)Entity<XenoPheromonesComponent>.op_Implicit(xeno), false);
		if (!_net.IsClient)
		{
			xeno.Comp.NextPheromonesPlasmaUse = _timing.CurTime + _pheromonePlasmaUseDelay;
			((EntitySystem)this).Dirty<XenoPheromonesComponent>(xeno, (MetaDataComponent)null);
			XenoActivePheromonesComponent active = ((EntitySystem)this).EnsureComp<XenoActivePheromonesComponent>(Entity<XenoPheromonesComponent>.op_Implicit(xeno));
			active.Pheromones = args.Pheromones;
			((EntitySystem)this).Dirty(Entity<XenoPheromonesComponent>.op_Implicit(xeno), (IComponent)(object)active, (MetaDataComponent)null);
			XenoPheromonesActivatedEvent ev = default(XenoPheromonesActivatedEvent);
			((EntitySystem)this).RaiseLocalEvent<XenoPheromonesActivatedEvent>(Entity<XenoPheromonesComponent>.op_Implicit(xeno), ref ev, false);
			_entityLookup.GetEntitiesInRange<XenoComponent>(xeno.Owner.ToCoordinates(), (float)xeno.Comp.PheromonesRange, active.Receivers, (LookupFlags)110);
		}
	}

	private void OnWardingUpdateMobState(Entity<XenoWardingPheromonesComponent> warding, ref UpdateMobStateEvent args)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent damageable = default(DamageableComponent);
		if (args.Component.CurrentState != MobState.Dead && args.State == MobState.Dead && _damageableQuery.TryGetComponent(Entity<XenoWardingPheromonesComponent>.op_Implicit(warding), ref damageable) && _mobThreshold.TryGetDeadThreshold(Entity<XenoWardingPheromonesComponent>.op_Implicit(warding), out var threshold) && _mobState.HasState(Entity<XenoWardingPheromonesComponent>.op_Implicit(warding), MobState.Critical))
		{
			FixedPoint2 wardingThreshold = threshold.Value + (1 + 20 * warding.Comp.Multiplier);
			if (!(damageable.TotalDamage >= wardingThreshold))
			{
				args.State = MobState.Critical;
			}
		}
	}

	private void OnWardingGetGraceTime(Entity<XenoWardingPheromonesComponent> warding, ref GetCriticalGraceTimeEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		args.Time += TimeSpan.FromSeconds(1L) * Math.Max(warding.Comp.Multiplier.Int() - 1, 0);
	}

	private void OnWardingRemove(Entity<XenoWardingPheromonesComponent> ent, ref ComponentRemove args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		MobThresholdsComponent thresholds = default(MobThresholdsComponent);
		if (((EntitySystem)this).TryComp<MobThresholdsComponent>(Entity<XenoWardingPheromonesComponent>.op_Implicit(ent), ref thresholds))
		{
			_mobThreshold.VerifyThresholds(Entity<XenoWardingPheromonesComponent>.op_Implicit(ent), thresholds);
		}
	}

	private void OnWardingDamageCritModify(Entity<XenoWardingPheromonesComponent> warding, ref DamageStateCritBeforeDamageEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!_rmcFlammable.IsOnFire(Entity<FlammableComponent>.op_Implicit(warding.Owner)))
		{
			XenoRegenComponent xeno = default(XenoRegenComponent);
			if (!((EntitySystem)this).TryComp<XenoRegenComponent>(Entity<XenoWardingPheromonesComponent>.op_Implicit(warding), ref xeno) || (!xeno.HealOffWeeds && !_weeds.IsOnFriendlyWeeds(Entity<TransformComponent>.op_Implicit(warding.Owner))))
			{
				DamageSpecifier damageReduct = _rmcDamageable.DistributeDamageCached(Entity<DamageableComponent>.op_Implicit(warding.Owner), warding.Comp.CritDamageGroup, warding.Comp.Multiplier * 0.25);
				args.Damage -= damageReduct;
			}
			else
			{
				args.Damage = -_rmcDamageable.DistributeDamageCached(Entity<DamageableComponent>.op_Implicit(warding.Owner), warding.Comp.CritDamageGroup, warding.Comp.Multiplier * 0.5f);
			}
		}
	}

	private void OnFrenzyRemove(Entity<XenoFrenzyPheromonesComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoFrenzyPheromonesComponent>.op_Implicit(ent));
	}

	private void OnFrenzyGetMeleeDamage(Entity<XenoFrenzyPheromonesComponent> frenzy, ref GetMeleeDamageEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		args.Damage += new DamageSpecifier(_protoManager.Index<DamageGroupPrototype>(frenzy.Comp.DamageGroup), frenzy.Comp.AttackDamageAddPerMult * frenzy.Comp.Multiplier);
	}

	private void OnFrenzyGetTailStabDamage(Entity<XenoFrenzyPheromonesComponent> frenzy, ref RMCGetTailStabBonusDamageEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		args.Damage += new DamageSpecifier(_protoManager.Index<DamageGroupPrototype>(frenzy.Comp.DamageGroup), frenzy.Comp.AttackDamageAddPerMult * frenzy.Comp.Multiplier * 1.2);
	}

	private void OnFrenzyMovementSpeedModifiers(Entity<XenoFrenzyPheromonesComponent> frenzy, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		float speed = 1f + (frenzy.Comp.MovementSpeedModifier * frenzy.Comp.Multiplier).Float();
		if (((EntitySystem)this).HasComp<PullingSlowedComponent>(frenzy.Owner))
		{
			speed = 1f + (frenzy.Comp.PullMovementSpeedModifier * frenzy.Comp.Multiplier).Float();
		}
		args.ModifySpeed(speed, speed);
	}

	private void OnFrenzyPullStarted(Entity<XenoFrenzyPheromonesComponent> frenzy, ref PullStartedMessage args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeed.RefreshMovementSpeedModifiers(args.PullerUid);
	}

	private void OnFrenzyPullStopped(Entity<XenoFrenzyPheromonesComponent> frenzy, ref PullStoppedMessage args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeed.RefreshMovementSpeedModifiers(args.PullerUid);
	}

	private void AssignMaxMultiplier(ref FixedPoint2 a, FixedPoint2 b)
	{
		a = FixedPoint2.Max(a, b);
	}

	public void DeactivatePheromones(Entity<XenoPheromonesComponent?> xeno)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoPheromonesComponent>(Entity<XenoPheromonesComponent>.op_Implicit(xeno), ref xeno.Comp, false))
		{
			return;
		}
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoPheromonesActionEvent>(Entity<XenoPheromonesComponent>.op_Implicit(xeno)))
		{
			_actions.SetToggled(action.AsNullable(), toggled: false);
		}
		if (((EntitySystem)this).HasComp<XenoActivePheromonesComponent>(Entity<XenoPheromonesComponent>.op_Implicit(xeno)))
		{
			if (_net.IsServer)
			{
				((EntitySystem)this).RemComp<XenoActivePheromonesComponent>(Entity<XenoPheromonesComponent>.op_Implicit(xeno));
			}
			_popup.PopupClient(base.Loc.GetString("cm-xeno-pheromones-stop"), Entity<XenoPheromonesComponent>.op_Implicit(xeno), Entity<XenoPheromonesComponent>.op_Implicit(xeno));
			XenoPheromonesDeactivatedEvent pheroEv = default(XenoPheromonesDeactivatedEvent);
			((EntitySystem)this).RaiseLocalEvent<XenoPheromonesDeactivatedEvent>(Entity<XenoPheromonesComponent>.op_Implicit(xeno), ref pheroEv, false);
		}
	}

	public void TryActivatePheromonesObject(Entity<XenoPheromonesObjectComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		XenoPheromonesComponent comp = default(XenoPheromonesComponent);
		if (((EntitySystem)this).Resolve<XenoPheromonesObjectComponent>(Entity<XenoPheromonesObjectComponent>.op_Implicit(ent), ref ent.Comp, false) && !_net.IsClient && ((EntitySystem)this).TryComp<XenoPheromonesComponent>(Entity<XenoPheromonesObjectComponent>.op_Implicit(ent), ref comp))
		{
			XenoActivePheromonesComponent active = ((EntitySystem)this).EnsureComp<XenoActivePheromonesComponent>(Entity<XenoPheromonesObjectComponent>.op_Implicit(ent));
			active.Pheromones = ent.Comp.Pheromones;
			((EntitySystem)this).Dirty(Entity<XenoPheromonesObjectComponent>.op_Implicit(ent), (IComponent)(object)active, (MetaDataComponent)null);
			_entityLookup.GetEntitiesInRange<XenoComponent>(ent.Owner.ToCoordinates(), (float)comp.PheromonesRange, active.Receivers, (LookupFlags)110);
			XenoPheromonesActivatedEvent ev = default(XenoPheromonesActivatedEvent);
			((EntitySystem)this).RaiseLocalEvent<XenoPheromonesActivatedEvent>(Entity<XenoPheromonesObjectComponent>.op_Implicit(ent), ref ev, false);
		}
	}

	private bool KeepWarding(EntityUid ent, XenoWardingPheromonesComponent warding, FixedPoint2 newWardMult)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent damageable = default(DamageableComponent);
		if (!_mobThreshold.TryGetIncapThreshold(ent, out var critThres) || !_damageableQuery.TryGetComponent(ent, ref damageable))
		{
			return false;
		}
		FixedPoint2 totalDamage = damageable.TotalDamage;
		FixedPoint2? fixedPoint = critThres;
		if (totalDamage < fixedPoint)
		{
			return false;
		}
		if (newWardMult > warding.Multiplier)
		{
			return false;
		}
		XenoRegenComponent xeno = default(XenoRegenComponent);
		if ((((EntitySystem)this).TryComp<XenoRegenComponent>(ent, ref xeno) && xeno.HealOffWeeds) || !_weeds.IsOnFriendlyWeeds(Entity<TransformComponent>.op_Implicit(ent)))
		{
			return false;
		}
		return true;
	}

	public string? GetPheroSuffix(Entity<XenoPheromonesComponent?> xeno)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<XenoPheromonesComponent>(Entity<XenoPheromonesComponent>.op_Implicit(xeno), ref xeno.Comp, false))
		{
			return null;
		}
		return xeno.Comp.PheroSuffix;
	}

	public override void Update(float frameTime)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (_net.IsClient)
		{
			return;
		}
		HashSet<EntityUid> oldRecovery = _oldReceivers[0];
		oldRecovery.Clear();
		EntityQueryEnumerator<XenoRecoveryPheromonesComponent> recoveryQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoRecoveryPheromonesComponent>();
		EntityUid uid = default(EntityUid);
		XenoRecoveryPheromonesComponent recovery = default(XenoRecoveryPheromonesComponent);
		while (recoveryQuery.MoveNext(ref uid, ref recovery))
		{
			oldRecovery.Add(uid);
			recovery.Multiplier = 0;
		}
		HashSet<EntityUid> oldWarding = _oldReceivers[1];
		oldWarding.Clear();
		EntityQueryEnumerator<XenoWardingPheromonesComponent> wardingQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoWardingPheromonesComponent>();
		EntityUid uid2 = default(EntityUid);
		XenoWardingPheromonesComponent warding = default(XenoWardingPheromonesComponent);
		while (wardingQuery.MoveNext(ref uid2, ref warding))
		{
			if (_mobState.IsDead(uid2) || !KeepWarding(uid2, warding, 0))
			{
				oldWarding.Add(uid2);
				warding.Multiplier = 0;
			}
		}
		HashSet<EntityUid> oldFrenzy = _oldReceivers[2];
		oldFrenzy.Clear();
		EntityQueryEnumerator<XenoFrenzyPheromonesComponent> frenzyQuery = ((EntitySystem)this).EntityQueryEnumerator<XenoFrenzyPheromonesComponent>();
		EntityUid uid3 = default(EntityUid);
		XenoFrenzyPheromonesComponent frenzy = default(XenoFrenzyPheromonesComponent);
		while (frenzyQuery.MoveNext(ref uid3, ref frenzy))
		{
			oldFrenzy.Add(uid3);
			frenzy.Multiplier = 0;
		}
		EntityQueryEnumerator<XenoActivePheromonesComponent, XenoPheromonesComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoActivePheromonesComponent, XenoPheromonesComponent, TransformComponent>();
		_pheromonesJob.Receivers.Clear();
		_pheromonesJob.Pheromones.Clear();
		_refreshSpeeds.Clear();
		EntityUid uid4 = default(EntityUid);
		XenoActivePheromonesComponent active = default(XenoActivePheromonesComponent);
		XenoPheromonesComponent pheromones = default(XenoPheromonesComponent);
		TransformComponent xform = default(TransformComponent);
		while (query.MoveNext(ref uid4, ref active, ref pheromones, ref xform))
		{
			_pheromonesJob.Pheromones.Add((uid4, active, pheromones, xform));
			if (_timing.CurTime < pheromones.NextPheromonesPlasmaUse)
			{
				_pheromonesJob.Receivers.Add((false, active.Receivers));
				continue;
			}
			pheromones.NextPheromonesPlasmaUse = _timing.CurTime + _pheromonePlasmaUseDelay;
			((EntitySystem)this).Dirty(uid4, (IComponent)(object)pheromones, (MetaDataComponent)null);
			if (!((EntitySystem)this).HasComp<XenoPheromonesObjectComponent>(uid4) && pheromones.PheromonesPlasmaUpkeep > 0 && !_xenoPlasma.TryRemovePlasma(Entity<XenoPlasmaComponent>.op_Implicit(uid4), pheromones.PheromonesPlasmaUpkeep))
			{
				_pheromonesJob.Pheromones.RemoveAt(_pheromonesJob.Pheromones.Count - 1);
				((EntitySystem)this).RemCompDeferred<XenoActivePheromonesComponent>(uid4);
			}
			else
			{
				_pheromonesJob.Receivers.Add((true, active.Receivers));
				((EntitySystem)this).Dirty(uid4, (IComponent)(object)pheromones, (MetaDataComponent)null);
			}
		}
		_parallel.ProcessNow((IParallelRobustJob)(object)_pheromonesJob, _pheromonesJob.Pheromones.Count);
		for (int i = 0; i < _pheromonesJob.Pheromones.Count; i++)
		{
			(EntityUid, XenoActivePheromonesComponent, XenoPheromonesComponent, TransformComponent) tuple = _pheromonesJob.Pheromones[i];
			XenoActivePheromonesComponent active2 = tuple.Item2;
			XenoPheromonesComponent pheromones2 = tuple.Item3;
			HashSet<Entity<XenoComponent>> receivers = _pheromonesJob.Receivers[i].Item2;
			switch (active2.Pheromones)
			{
			case XenoPheromones.Recovery:
				foreach (Entity<XenoComponent> receiver3 in receivers)
				{
					if (!((EntitySystem)this).Deleted(Entity<XenoComponent>.op_Implicit(receiver3), (MetaDataComponent)null) && !_mobState.IsDead(Entity<XenoComponent>.op_Implicit(receiver3)) && receiver3.Comp.IgnorePheromones != XenoPheromones.Recovery)
					{
						oldRecovery.Remove(Entity<XenoComponent>.op_Implicit(receiver3));
						XenoRecoveryPheromonesComponent recovery2 = ((EntitySystem)this).EnsureComp<XenoRecoveryPheromonesComponent>(Entity<XenoComponent>.op_Implicit(receiver3));
						AssignMaxMultiplier(ref recovery2.Multiplier, pheromones2.PheromonesMultiplier);
					}
				}
				break;
			case XenoPheromones.Warding:
				foreach (Entity<XenoComponent> receiver2 in active2.Receivers)
				{
					if (!((EntitySystem)this).Deleted(Entity<XenoComponent>.op_Implicit(receiver2), (MetaDataComponent)null) && !_mobState.IsDead(Entity<XenoComponent>.op_Implicit(receiver2)) && receiver2.Comp.IgnorePheromones != XenoPheromones.Warding)
					{
						oldWarding.Remove(Entity<XenoComponent>.op_Implicit(receiver2));
						XenoWardingPheromonesComponent warding2 = ((EntitySystem)this).EnsureComp<XenoWardingPheromonesComponent>(Entity<XenoComponent>.op_Implicit(receiver2));
						AssignMaxMultiplier(ref warding2.Multiplier, pheromones2.PheromonesMultiplier);
					}
				}
				break;
			case XenoPheromones.Frenzy:
				foreach (Entity<XenoComponent> receiver in active2.Receivers)
				{
					if (!((EntitySystem)this).Deleted(Entity<XenoComponent>.op_Implicit(receiver), (MetaDataComponent)null) && !_mobState.IsDead(Entity<XenoComponent>.op_Implicit(receiver)) && receiver.Comp.IgnorePheromones != XenoPheromones.Frenzy)
					{
						oldFrenzy.Remove(Entity<XenoComponent>.op_Implicit(receiver));
						XenoFrenzyPheromonesComponent frenzy2 = ((EntitySystem)this).EnsureComp<XenoFrenzyPheromonesComponent>(Entity<XenoComponent>.op_Implicit(receiver));
						FixedPoint2 old = frenzy2.Multiplier;
						AssignMaxMultiplier(ref frenzy2.Multiplier, pheromones2.PheromonesMultiplier);
						if (frenzy2.Multiplier != old)
						{
							_refreshSpeeds.Add(Entity<XenoComponent>.op_Implicit(receiver));
						}
					}
				}
				break;
			}
		}
		foreach (EntityUid uid5 in _refreshSpeeds)
		{
			_movementSpeed.RefreshMovementSpeedModifiers(uid5);
		}
		foreach (EntityUid uid6 in oldRecovery)
		{
			((EntitySystem)this).RemComp<XenoRecoveryPheromonesComponent>(uid6);
		}
		foreach (EntityUid uid7 in oldWarding)
		{
			((EntitySystem)this).RemComp<XenoWardingPheromonesComponent>(uid7);
		}
		foreach (EntityUid uid8 in oldFrenzy)
		{
			((EntitySystem)this).RemComp<XenoFrenzyPheromonesComponent>(uid8);
		}
	}
}
