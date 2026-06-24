using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Animation;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.HiveLeader;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.CCVar;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Charge;

public sealed class XenoChargeSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private SharedMoverController _moverController;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private SharedRMCDamageableSystem _rmcDamageable;

	[Dependency]
	private SharedRMCEmoteSystem _rmcEmote;

	[Dependency]
	private RMCObstacleSlammingSystem _rmcObstacleSlamming;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private ThrowingSystem _throwing;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private ThrownItemSystem _thrownItem;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private XenoAnimationsSystem _xenoAnimations;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private SharedXenoHiveSystem _xenoHive;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private SharedDestructibleSystem _destruct;

	[Dependency]
	private RMCSizeStunSystem _sizeStun;

	private readonly ProtoId<DamageTypePrototype> _blunt = ProtoId<DamageTypePrototype>.op_Implicit("Blunt");

	private EntityQuery<InputMoverComponent> _inputMoverQuery;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	private EntityQuery<ThrownItemComponent> _thrownItemQuery;

	private EntityQuery<XenoToggleChargingComponent> _xenoToggleChargingQuery;

	private EntityQuery<ActiveXenoToggleChargingComponent> _activeXenoToggleChargingQuery;

	private EntityQuery<XenoToggleChargingRecentlyHitComponent> _xenoToggleChargingRecentlyHitQuery;

	private bool _relativeMovement;

	private readonly HashSet<(Entity<ActiveXenoToggleChargingComponent> Crusher, EntityUid Target)> _hit = new HashSet<(Entity<ActiveXenoToggleChargingComponent>, EntityUid)>();

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		_inputMoverQuery = ((EntitySystem)this).GetEntityQuery<InputMoverComponent>();
		_physicsQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		_thrownItemQuery = ((EntitySystem)this).GetEntityQuery<ThrownItemComponent>();
		_xenoToggleChargingQuery = ((EntitySystem)this).GetEntityQuery<XenoToggleChargingComponent>();
		_activeXenoToggleChargingQuery = ((EntitySystem)this).GetEntityQuery<ActiveXenoToggleChargingComponent>();
		_xenoToggleChargingRecentlyHitQuery = ((EntitySystem)this).GetEntityQuery<XenoToggleChargingRecentlyHitComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoChargeComponent, XenoChargeActionEvent>((EntityEventRefHandler<XenoChargeComponent, XenoChargeActionEvent>)OnXenoChargeAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoChargeComponent, ThrowDoHitEvent>((EntityEventRefHandler<XenoChargeComponent, ThrowDoHitEvent>)OnXenoChargeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoChargeComponent, XenoChargeDoAfterEvent>((EntityEventRefHandler<XenoChargeComponent, XenoChargeDoAfterEvent>)OnXenoChargeDoAfterEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoChargeComponent, StopThrowEvent>((EntityEventRefHandler<XenoChargeComponent, StopThrowEvent>)OnXenoChargeStop, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoToggleChargingComponent, XenoToggleChargingActionEvent>((EntityEventRefHandler<XenoToggleChargingComponent, XenoToggleChargingActionEvent>)OnXenoToggleChargingAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveXenoToggleChargingComponent, MapInitEvent>((EntityEventRefHandler<ActiveXenoToggleChargingComponent, MapInitEvent>)OnActiveToggleChargingMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveXenoToggleChargingComponent, ComponentRemove>((EntityEventRefHandler<ActiveXenoToggleChargingComponent, ComponentRemove>)OnActiveToggleChargingRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveXenoToggleChargingComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<ActiveXenoToggleChargingComponent, RefreshMovementSpeedModifiersEvent>)OnActiveToggleChargingSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveXenoToggleChargingComponent, MoveInputEvent>((EntityEventRefHandler<ActiveXenoToggleChargingComponent, MoveInputEvent>)OnActiveToggleChargingMoveInput, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveXenoToggleChargingComponent, MoveEvent>((EntityEventRefHandler<ActiveXenoToggleChargingComponent, MoveEvent>)OnActiveToggleChargingMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveXenoToggleChargingComponent, StartCollideEvent>((EntityEventRefHandler<ActiveXenoToggleChargingComponent, StartCollideEvent>)OnActiveToggleChargingCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveXenoToggleChargingComponent, MobStateChangedEvent>((EntityEventRefHandler<ActiveXenoToggleChargingComponent, MobStateChangedEvent>)OnActiveToggleChargingMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoToggleChargingDamageComponent, XenoToggleChargingCollideEvent>((EntityEventRefHandler<XenoToggleChargingDamageComponent, XenoToggleChargingCollideEvent>)OnChargingDamageCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoToggleChargingKnockbackComponent, XenoToggleChargingCollideEvent>((EntityEventRefHandler<XenoToggleChargingKnockbackComponent, XenoToggleChargingCollideEvent>)OnChargingKnockbackCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoToggleChargingKnockbackComponent, AttemptMobTargetCollideEvent>((EntityEventRefHandler<XenoToggleChargingKnockbackComponent, AttemptMobTargetCollideEvent>)OnChargingKnockbackAttemptCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoToggleChargingParalyzeComponent, XenoToggleChargingCollideEvent>((EntityEventRefHandler<XenoToggleChargingParalyzeComponent, XenoToggleChargingCollideEvent>)OnChargingParalyzeCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoToggleChargingStopComponent, XenoToggleChargingCollideEvent>((EntityEventRefHandler<XenoToggleChargingStopComponent, XenoToggleChargingCollideEvent>)OnChargingStopCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HiveLeaderComponent, XenoToggleChargingCollideEvent>((EntityEventRefHandler<HiveLeaderComponent, XenoToggleChargingCollideEvent>)OnLeaderCollide, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<bool>(((EntitySystem)this).Subs, _config, CCVars.RelativeMovement, (Action<bool>)delegate(bool v)
		{
			_relativeMovement = v;
		}, true);
	}

	private void OnChargingDamageCollide(Entity<XenoToggleChargingDamageComponent> damage, ref XenoToggleChargingCollideEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		args.Handled = true;
		Entity<ActiveXenoToggleChargingComponent> ent = args.Charger;
		if (ent.Comp.Stage < damage.Comp.MinimumStage)
		{
			return;
		}
		if (_net.IsServer)
		{
			_audio.PlayPvs(damage.Comp.Sound, _transform.GetMoverCoordinates(Entity<XenoToggleChargingDamageComponent>.op_Implicit(damage)), (AudioParams?)null);
		}
		DamageableComponent damageable = ((EntitySystem)this).CompOrNull<DamageableComponent>(Entity<XenoToggleChargingDamageComponent>.op_Implicit(damage));
		if (damage.Comp.Destroy)
		{
			if (_net.IsServer)
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-charge-plow-through", (ValueTuple<string, object>)("xeno", ent), (ValueTuple<string, object>)("target", damage)), Entity<XenoToggleChargingDamageComponent>.op_Implicit(damage), PopupType.SmallCaution);
			}
			if (_net.IsClient)
			{
				_transform.DetachEntity(Entity<XenoToggleChargingDamageComponent>.op_Implicit(damage), ((EntitySystem)this).Transform(Entity<XenoToggleChargingDamageComponent>.op_Implicit(damage)));
			}
			else
			{
				((EntitySystem)this).QueueDel((EntityUid?)Entity<XenoToggleChargingDamageComponent>.op_Implicit(damage));
			}
		}
		else
		{
			if (_net.IsServer)
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-xeno-charge-smashes", (ValueTuple<string, object>)("xeno", ent), (ValueTuple<string, object>)("target", damage)), Entity<XenoToggleChargingDamageComponent>.op_Implicit(damage), PopupType.SmallCaution);
			}
			int stage = ent.Comp.Stage;
			if (damage.Comp.StageMultipliers != null && damage.Comp.StageMultipliers.TryGetValue(stage, out var stageMult))
			{
				stage = stageMult * stageMult;
			}
			else if (damage.Comp.DefaultMultiplier != 0)
			{
				stage = damage.Comp.DefaultMultiplier * damage.Comp.DefaultMultiplier;
			}
			else if (stage < damage.Comp.MinimumStage)
			{
				stage = damage.Comp.MinimumStage;
			}
			if (damage.Comp.Damage != null)
			{
				_damageable.TryChangeDamage(Entity<XenoToggleChargingDamageComponent>.op_Implicit(damage), damage.Comp.Damage * stage, ignoreResistances: false, interruptsDoAfters: true, damageable);
			}
			if (damage.Comp.ArmorPiercingDamage != null)
			{
				DamageableSystem damageable2 = _damageable;
				EntityUid? uid = Entity<XenoToggleChargingDamageComponent>.op_Implicit(damage);
				DamageSpecifier damage2 = damage.Comp.ArmorPiercingDamage * stage;
				int armorPiercing = damage.Comp.ArmorPiercing;
				damageable2.TryChangeDamage(uid, damage2, ignoreResistances: false, interruptsDoAfters: true, damageable, null, null, armorPiercing);
			}
			if (damage.Comp.PercentageDamage > FixedPoint2.Zero && _rmcDamageable.TryGetDestroyedAt(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent), out var destroyed))
			{
				DamageSpecifier bluntDamage = new DamageSpecifier();
				bluntDamage.DamageDict[ProtoId<DamageTypePrototype>.op_Implicit(_blunt)] = destroyed.Value * damage.Comp.PercentageDamage * stage;
				_damageable.TryChangeDamage(Entity<XenoToggleChargingDamageComponent>.op_Implicit(damage), bluntDamage, ignoreResistances: false, interruptsDoAfters: true, damageable);
			}
		}
		if (_net.IsClient && damageable != null && damage.Comp.DestroyDamage > FixedPoint2.Zero && damageable.TotalDamage >= damage.Comp.DestroyDamage)
		{
			_transform.DetachEntity(Entity<XenoToggleChargingDamageComponent>.op_Implicit(damage), ((EntitySystem)this).Transform(Entity<XenoToggleChargingDamageComponent>.op_Implicit(damage)));
		}
		if (damage.Comp.Unanchor && !((EntitySystem)this).TerminatingOrDeleted(Entity<XenoToggleChargingDamageComponent>.op_Implicit(damage), (MetaDataComponent)null) && !base.EntityManager.IsQueuedForDeletion(Entity<XenoToggleChargingDamageComponent>.op_Implicit(damage)))
		{
			_transform.Unanchor(Entity<XenoToggleChargingDamageComponent>.op_Implicit(damage));
		}
		if (damage.Comp.Stop)
		{
			ResetCharging(ent, resetInput: false);
		}
		else if (damage.Comp.StageLoss > 0)
		{
			IncrementStages(ent, -damage.Comp.StageLoss);
		}
	}

	private void OnChargingKnockbackCollide(Entity<XenoToggleChargingKnockbackComponent> ent, ref XenoToggleChargingCollideEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		args.Handled = true;
		TransformComponent xform = default(TransformComponent);
		if (((EntitySystem)this).TryComp(Entity<XenoToggleChargingKnockbackComponent>.op_Implicit(ent), ref xform) && xform.Anchored)
		{
			ResetStage(args.Charger);
			return;
		}
		if (!ent.Comp.Enabled || args.Charger.Comp.Stage == 0)
		{
			ResetStage(args.Charger);
			return;
		}
		_rmcPulling.TryStopAllPullsFromAndOn(Entity<XenoToggleChargingKnockbackComponent>.op_Implicit(ent));
		if (_xenoHive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(ent.Owner), Entity<HiveMemberComponent>.op_Implicit(args.Charger.Owner)))
		{
			_rmcObstacleSlamming.MakeImmune(Entity<XenoToggleChargingKnockbackComponent>.op_Implicit(ent));
		}
		DirectionFlag direction = args.Charger.Comp.Direction;
		if ((int)direction != 0)
		{
			(Direction, Direction) perpendiculars = DirectionExtensions.AsDir(direction).GetPerpendiculars();
			Vector2 diff = Vector2Helpers.Normalized(DirectionExtensions.ToVec(RandomExtensions.Prob(_random, 0.5f) ? perpendiculars.Item1 : perpendiculars.Item2));
			_throwing.TryThrow(Entity<XenoToggleChargingKnockbackComponent>.op_Implicit(ent), diff, 10f, null, 2f, null, compensateFriction: true);
			IncrementStages(args.Charger, -1);
			if (_net.IsServer)
			{
				_audio.PlayPvs(ent.Comp.Sound, Entity<XenoToggleChargingKnockbackComponent>.op_Implicit(ent), (AudioParams?)null);
				string othersMsg = base.Loc.GetString("rmc-xeno-charge-knockback-others", (ValueTuple<string, object>)("user", args.Charger), (ValueTuple<string, object>)("target", ent));
				_popup.PopupEntity(othersMsg, Entity<XenoToggleChargingKnockbackComponent>.op_Implicit(ent), PopupType.MediumCaution);
			}
		}
	}

	private void OnChargingKnockbackAttemptCollide(Entity<XenoToggleChargingKnockbackComponent> ent, ref AttemptMobTargetCollideEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		ActiveXenoToggleChargingComponent active = default(ActiveXenoToggleChargingComponent);
		if (_activeXenoToggleChargingQuery.TryComp(args.Entity, ref active) && active.Stage > 0)
		{
			args.Cancelled = true;
		}
	}

	private void OnChargingParalyzeCollide(Entity<XenoToggleChargingParalyzeComponent> ent, ref XenoToggleChargingCollideEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		args.Handled = true;
		int stage = args.Charger.Comp.Stage;
		XenoToggleChargingComponent charging = default(XenoToggleChargingComponent);
		if (stage > 0 && _xenoToggleChargingQuery.TryComp(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(args.Charger), ref charging))
		{
			TimeSpan duration = ((stage >= charging.MaxStage) ? ent.Comp.MaxStageDuration : ent.Comp.Duration);
			_stun.TryParalyze(Entity<XenoToggleChargingParalyzeComponent>.op_Implicit(ent), duration, refresh: false);
		}
	}

	private void OnChargingStopCollide(Entity<XenoToggleChargingStopComponent> ent, ref XenoToggleChargingCollideEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		args.Handled = true;
		ResetStage(args.Charger);
	}

	private void OnLeaderCollide(Entity<HiveLeaderComponent> ent, ref XenoToggleChargingCollideEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		args.Handled = true;
		ResetStage(args.Charger);
	}

	private void OnXenoChargeAction(Entity<XenoChargeComponent> xeno, ref XenoChargeActionEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			XenoChargeAttemptEvent attempt = default(XenoChargeAttemptEvent);
			((EntitySystem)this).RaiseLocalEvent<XenoChargeAttemptEvent>(Entity<XenoChargeComponent>.op_Implicit(xeno), ref attempt, false);
			if (!attempt.Cancelled && _xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost))
			{
				((HandledEntityEventArgs)args).Handled = true;
				XenoChargeDoAfterEvent ev = new XenoChargeDoAfterEvent(((EntitySystem)this).GetNetCoordinates(args.Target, (MetaDataComponent)null));
				DoAfterArgs doAfter = new DoAfterArgs((IEntityManager)(object)base.EntityManager, Entity<XenoChargeComponent>.op_Implicit(xeno), xeno.Comp.ChargeDelay, ev, Entity<XenoChargeComponent>.op_Implicit(xeno))
				{
					BreakOnMove = true,
					Hidden = true
				};
				_stun.TrySlowdown(Entity<XenoChargeComponent>.op_Implicit(xeno), TimeSpan.FromSeconds(1.75), refresh: false, 0f, 0f);
				_doAfter.TryStartDoAfter(doAfter);
			}
		}
	}

	private void StopCrusherCharge(Entity<XenoChargeComponent> xeno)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		ThrownItemComponent thrown = default(ThrownItemComponent);
		if (_physicsQuery.TryGetComponent(Entity<XenoChargeComponent>.op_Implicit(xeno), ref physics) && _thrownItemQuery.TryGetComponent(Entity<XenoChargeComponent>.op_Implicit(xeno), ref thrown))
		{
			_thrownItem.LandComponent(Entity<XenoChargeComponent>.op_Implicit(xeno), thrown, physics, playSound: true);
			_thrownItem.StopThrow(Entity<XenoChargeComponent>.op_Implicit(xeno), thrown);
		}
		if (_timing.IsFirstTimePredicted)
		{
			Vector2? charge = xeno.Comp.Charge;
			if (charge.HasValue)
			{
				Vector2 charge2 = charge.GetValueOrDefault();
				xeno.Comp.Charge = null;
				_xenoAnimations.PlayLungeAnimationEvent(Entity<XenoChargeComponent>.op_Implicit(xeno), charge2);
			}
		}
	}

	private void OnXenoChargeHit(Entity<XenoChargeComponent> xeno, ref ThrowDoHitEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		EntityUid targetId = args.Target;
		if (_mobState.IsDead(targetId))
		{
			return;
		}
		StopCrusherCharge(xeno);
		XenoCrusherChargableComponent crush = null;
		bool pass = false;
		if (!_xeno.CanAbilityAttackTarget(Entity<XenoChargeComponent>.op_Implicit(xeno), targetId) && !((EntitySystem)this).TryComp<XenoCrusherChargableComponent>(targetId, ref crush))
		{
			return;
		}
		if (_net.IsServer)
		{
			_audio.PlayPvs(xeno.Comp.Sound, Entity<XenoChargeComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
		DamageSpecifier structDamage = xeno.Comp.Damage;
		if (crush != null)
		{
			if (crush.SetDamage != null)
			{
				structDamage = crush.SetDamage;
			}
			if (crush.InstantDestroy)
			{
				if (_net.IsClient && pass)
				{
					_transform.DetachEntity(targetId, ((EntitySystem)this).Transform(targetId));
				}
				else if (_net.IsServer)
				{
					_destruct.DestroyEntity(targetId);
				}
				return;
			}
		}
		DamageSpecifier damage = _damageable.TryChangeDamage(targetId, _xeno.TryApplyXenoSlashDamageMultiplier(targetId, structDamage), ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoChargeComponent>.op_Implicit(xeno), Entity<XenoChargeComponent>.op_Implicit(xeno));
		if (damage?.GetTotal() > FixedPoint2.Zero)
		{
			Filter filter = Filter.Pvs(targetId, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
			_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { targetId }, filter);
		}
		DamageableComponent damageable = default(DamageableComponent);
		if (crush != null && crush.DestroyDamage.HasValue && ((EntitySystem)this).TryComp<DamageableComponent>(targetId, ref damageable) && damage != null && crush.PassOnDestroy && crush.DestroyDamage > FixedPoint2.Zero)
		{
			FixedPoint2 totalDamage = damageable.TotalDamage;
			FixedPoint2? destroyDamage = crush.DestroyDamage;
			if (totalDamage >= destroyDamage)
			{
				pass = true;
				if (_net.IsClient)
				{
					_transform.DetachEntity(targetId, ((EntitySystem)this).Transform(targetId));
				}
			}
		}
		_ = xeno.Comp.Range;
		if (crush != null && crush.ThrowRange.HasValue)
		{
			_ = crush.ThrowRange.Value;
		}
		_rmcPulling.TryStopAllPullsFromAndOn(targetId);
		MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoChargeComponent>.op_Implicit(xeno), (TransformComponent)null);
		_stun.TryParalyze(targetId, xeno.Comp.StunTime, refresh: true);
		_sizeStun.KnockBack(targetId, origin, 2f, 2f, 10f);
	}

	private void OnXenoChargeDoAfterEvent(Entity<XenoChargeComponent> xeno, ref XenoChargeDoAfterEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled)
		{
			_rmcPulling.TryStopAllPullsFromAndOn(Entity<XenoChargeComponent>.op_Implicit(xeno));
			EntityCoordinates coordinates = ((EntitySystem)this).GetCoordinates(args.Coordinates);
			MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoChargeComponent>.op_Implicit(xeno), (TransformComponent)null);
			Vector2 diff = _transform.ToMapCoordinates(coordinates, true).Position - origin.Position;
			diff = Vector2Helpers.Normalized(diff) * xeno.Comp.Range;
			xeno.Comp.Charge = diff;
			((EntitySystem)this).Dirty<XenoChargeComponent>(xeno, (MetaDataComponent)null);
			_rmcObstacleSlamming.MakeImmune(Entity<XenoChargeComponent>.op_Implicit(xeno));
			_throwing.TryThrow(Entity<XenoChargeComponent>.op_Implicit(xeno), diff, xeno.Comp.Strength, null, 2f, null, compensateFriction: false, recoil: true, animated: false);
		}
	}

	private void OnXenoChargeStop(Entity<XenoChargeComponent> xeno, ref StopThrowEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (!xeno.Comp.Charge.HasValue)
		{
			return;
		}
		foreach (Entity<MobStateComponent> slower in _lookup.GetEntitiesInRange<MobStateComponent>(_transform.GetMapCoordinates(Entity<XenoChargeComponent>.op_Implicit(xeno), (TransformComponent)null), xeno.Comp.SlowRange, (LookupFlags)110))
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoChargeComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(slower)))
			{
				_slow.TrySlowdown(Entity<MobStateComponent>.op_Implicit(slower), xeno.Comp.SlowTime, refresh: true, ignoreDurationModifier: true);
			}
		}
	}

	private void OnXenoToggleChargingAction(Entity<XenoToggleChargingComponent> ent, ref XenoToggleChargingActionEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		InputMoverComponent mover = default(InputMoverComponent);
		if (!_timing.ApplyingState && !((EntitySystem)this).RemComp<ActiveXenoToggleChargingComponent>(Entity<XenoToggleChargingComponent>.op_Implicit(ent)) && ((EntitySystem)this).TryComp<InputMoverComponent>(Entity<XenoToggleChargingComponent>.op_Implicit(ent), ref mover))
		{
			DirectionFlag direction = GetHeldButton(Entity<XenoToggleChargingComponent>.op_Implicit(ent), mover.HeldMoveButtons);
			ActiveXenoToggleChargingComponent active = new ActiveXenoToggleChargingComponent();
			((EntitySystem)this).AddComp<ActiveXenoToggleChargingComponent>(Entity<XenoToggleChargingComponent>.op_Implicit(ent), active, true);
			if ((direction & (sbyte)(direction - 1)) == 0)
			{
				active.Direction = direction;
				((EntitySystem)this).Dirty(Entity<XenoToggleChargingComponent>.op_Implicit(ent), (IComponent)(object)active, (MetaDataComponent)null);
			}
		}
	}

	private void OnActiveToggleChargingMapInit(Entity<ActiveXenoToggleChargingComponent> ent, ref MapInitEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent));
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoToggleChargingActionEvent>(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent)))
		{
			_actions.SetToggled(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action))), toggled: true);
		}
	}

	private void OnActiveToggleChargingRemove(Entity<ActiveXenoToggleChargingComponent> ent, ref ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent));
		foreach (Entity<ActionComponent> action in _rmcActions.GetActionsWithEvent<XenoToggleChargingActionEvent>(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent)))
		{
			_actions.SetToggled(Entity<ActionComponent>.op_Implicit((Entity<ActionComponent>.op_Implicit(action), Entity<ActionComponent>.op_Implicit(action))), toggled: false);
		}
	}

	private void OnActiveToggleChargingSpeed(Entity<ActiveXenoToggleChargingComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		XenoToggleChargingComponent charging = default(XenoToggleChargingComponent);
		if (ent.Comp.Stage != 0 && _xenoToggleChargingQuery.TryComp(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent), ref charging))
		{
			float speed = 1f + (float)ent.Comp.Stage * charging.SpeedPerStage;
			args.ModifySpeed(speed, speed);
		}
	}

	private void OnActiveToggleChargingMoveInput(Entity<ActiveXenoToggleChargingComponent> ent, ref MoveInputEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		DirectionFlag direction = GetHeldButton(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent), args.Entity.Comp.HeldMoveButtons & MoveButtons.AnyDirection);
		if ((int)direction != 0 && (DirectionFlag)(ent.Comp.Direction & direction) == direction && (direction & (sbyte)(direction - 1)) == 0)
		{
			return;
		}
		if ((int)ent.Comp.Direction != 0)
		{
			(Direction, Direction) perpendiculars = DirectionExtensions.AsDir(ent.Comp.Direction).GetPerpendiculars();
			if ((ent.Comp.Direction == DirectionExtensions.AsFlag(perpendiculars.Item1) || ent.Comp.Direction == DirectionExtensions.AsFlag(perpendiculars.Item2)) && ((int)ent.Comp.Deviated == 0 || ent.Comp.Deviated == direction))
			{
				ent.Comp.Deviated = direction;
				return;
			}
		}
		ResetCharging(ent);
		ent.Comp.Direction = direction;
	}

	private void OnActiveToggleChargingMove(Entity<ActiveXenoToggleChargingComponent> ent, ref MoveEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		XenoToggleChargingComponent charging = default(XenoToggleChargingComponent);
		float distance = default(float);
		if (!_xenoToggleChargingQuery.TryComp(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent), ref charging) || _rmcPulling.IsBeingPulled(Entity<PullableComponent>.op_Implicit(ent.Owner), out var _) || !((EntityCoordinates)(ref args.OldPosition)).TryDistance((IEntityManager)(object)base.EntityManager, _transform, args.NewPosition, ref distance))
		{
			return;
		}
		float absDistance = Math.Abs(distance);
		ent.Comp.Distance += absDistance;
		ent.Comp.LastMovedAt = _timing.CurTime;
		((EntitySystem)this).Dirty<ActiveXenoToggleChargingComponent>(ent, (MetaDataComponent)null);
		InputMoverComponent mover = default(InputMoverComponent);
		if (_inputMoverQuery.TryComp(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent), ref mover))
		{
			Angle? lastRelativeRotation = ent.Comp.LastRelativeRotation;
			ent.Comp.LastRelativeRotation = mover.RelativeRotation;
			Angle? lastRelativeRotation2 = ent.Comp.LastRelativeRotation;
			Angle? val = lastRelativeRotation;
			if (lastRelativeRotation2.HasValue != val.HasValue || (lastRelativeRotation2.HasValue && lastRelativeRotation2.GetValueOrDefault() != val.GetValueOrDefault()))
			{
				ResetStage(ent);
				return;
			}
		}
		if ((int)ent.Comp.Deviated != 0)
		{
			ent.Comp.DeviatedDistance += absDistance;
			if (ent.Comp.DeviatedDistance >= charging.MaxDeviation)
			{
				ResetCharging(ent);
				return;
			}
		}
		if (ent.Comp.Distance < charging.StepIncrement)
		{
			return;
		}
		ent.Comp.Steps += charging.StepIncrement;
		ent.Comp.Distance -= charging.StepIncrement;
		if (ent.Comp.Steps < charging.MinimumSteps)
		{
			return;
		}
		if (!_xenoPlasma.TryRemovePlasma(Entity<XenoPlasmaComponent>.op_Implicit(ent.Owner), charging.PlasmaPerStep))
		{
			ResetCharging(ent, resetInput: false);
			return;
		}
		_rmcPulling.TryStopAllPullsFromAndOn(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent));
		if (ent.Comp.Stage == charging.MaxStage - 1)
		{
			ProtoId<EmotePrototype>? emote = charging.Emote;
			if (emote.HasValue)
			{
				ProtoId<EmotePrototype> emote2 = emote.GetValueOrDefault();
				_rmcEmote.TryEmoteWithChat(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent), emote2, hideLog: false, null, ignoreActionBlocker: false, forceEmote: false, charging.EmoteCooldown);
			}
		}
		ent.Comp.Stage = Math.Min(charging.MaxStage, ent.Comp.Stage + 1);
		ent.Comp.SoundSteps += charging.StepIncrement;
		if (ent.Comp.Stage == 1 || ent.Comp.SoundSteps >= (float)charging.SoundEvery)
		{
			ent.Comp.SoundSteps = 0f;
			if (_timing.InSimulation)
			{
				_audio.PlayPredicted(charging.Sound, Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent), (EntityUid?)Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent), (AudioParams?)null);
			}
		}
		((EntitySystem)this).Dirty<ActiveXenoToggleChargingComponent>(ent, (MetaDataComponent)null);
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent));
	}

	private void OnActiveToggleChargingCollide(Entity<ActiveXenoToggleChargingComponent> ent, ref StartCollideEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!((double)Math.Abs(ent.Comp.Steps - 1f) < 0.001))
		{
			_hit.Add((ent, args.OtherEntity));
		}
	}

	private void OnActiveToggleChargingMobStateChanged(Entity<ActiveXenoToggleChargingComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState != MobState.Alive)
		{
			ResetCharging(ent);
			if (!_timing.ApplyingState)
			{
				((EntitySystem)this).RemComp<ActiveXenoToggleChargingComponent>(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent));
			}
		}
	}

	private void ResetCharging(Entity<ActiveXenoToggleChargingComponent> xeno, bool resetInput = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		ResetStage(xeno);
		xeno.Comp.DeviatedDistance = 0f;
		if (resetInput)
		{
			xeno.Comp.Direction = (DirectionFlag)0;
		}
		((EntitySystem)this).Dirty<ActiveXenoToggleChargingComponent>(xeno, (MetaDataComponent)null);
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(xeno));
	}

	private void ResetStage(Entity<ActiveXenoToggleChargingComponent> xeno)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		xeno.Comp.Steps = 0f;
		xeno.Comp.SoundSteps = 0f;
		xeno.Comp.Stage = 0;
		((EntitySystem)this).Dirty<ActiveXenoToggleChargingComponent>(xeno, (MetaDataComponent)null);
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(xeno));
	}

	private void IncrementStages(Entity<ActiveXenoToggleChargingComponent> ent, int increment)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Stage = Math.Max(0, ent.Comp.Stage + increment);
		XenoToggleChargingComponent charging = default(XenoToggleChargingComponent);
		if (_xenoToggleChargingQuery.TryComp(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent), ref charging))
		{
			ent.Comp.Stage = Math.Min(charging.MaxStage, ent.Comp.Stage);
		}
		((EntitySystem)this).Dirty<ActiveXenoToggleChargingComponent>(ent, (MetaDataComponent)null);
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(ent));
	}

	private DirectionFlag GetHeldButton(EntityUid mover, MoveButtons button)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		InputMoverComponent moverComp = default(InputMoverComponent);
		if (!((EntitySystem)this).TryComp<InputMoverComponent>(mover, ref moverComp))
		{
			return (DirectionFlag)0;
		}
		Angle parentRotation = _moverController.GetParentGridAngle(moverComp);
		Vector2 total = _moverController.DirVecForButtons(button);
		return DirectionExtensions.AsFlag(DirectionExtensions.GetDir(_relativeMovement ? ((Angle)(ref parentRotation)).RotateVec(ref total) : total));
	}

	public override void Update(float frameTime)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Invalid comparison between Unknown and I4
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		try
		{
			XenoToggleChargingRecentlyHitComponent recently = default(XenoToggleChargingRecentlyHitComponent);
			foreach (var hit in _hit)
			{
				if (((EntitySystem)this).TerminatingOrDeleted(Entity<ActiveXenoToggleChargingComponent>.op_Implicit(hit.Crusher), (MetaDataComponent)null) || ((EntitySystem)this).TerminatingOrDeleted(hit.Target, (MetaDataComponent)null))
				{
					continue;
				}
				if (_xenoToggleChargingRecentlyHitQuery.TryComp(hit.Target, ref recently) && time < recently.LastHitAt + recently.Cooldown)
				{
					return;
				}
				XenoToggleChargingCollideEvent ev = new XenoToggleChargingCollideEvent(hit.Crusher);
				((EntitySystem)this).RaiseLocalEvent<XenoToggleChargingCollideEvent>(hit.Target, ref ev, false);
				if (ev.Handled)
				{
					recently = ((EntitySystem)this).EnsureComp<XenoToggleChargingRecentlyHitComponent>(hit.Target);
					recently.LastHitAt = time;
					((EntitySystem)this).Dirty(hit.Target, (IComponent)(object)recently, (MetaDataComponent)null);
					if (hit.Crusher.Comp.Stage == 0)
					{
						hit.Crusher.Comp.Steps = 0f;
						((EntitySystem)this).Dirty<ActiveXenoToggleChargingComponent>(hit.Crusher, (MetaDataComponent)null);
					}
				}
			}
		}
		finally
		{
			_hit.Clear();
		}
		EntityQueryEnumerator<ActiveXenoToggleChargingComponent, XenoToggleChargingComponent, PhysicsComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ActiveXenoToggleChargingComponent, XenoToggleChargingComponent, PhysicsComponent>();
		EntityUid uid = default(EntityUid);
		ActiveXenoToggleChargingComponent active = default(ActiveXenoToggleChargingComponent);
		XenoToggleChargingComponent charging = default(XenoToggleChargingComponent);
		PhysicsComponent physics = default(PhysicsComponent);
		while (query.MoveNext(ref uid, ref active, ref charging, ref physics))
		{
			if ((int)physics.BodyStatus == 1)
			{
				ResetCharging(Entity<ActiveXenoToggleChargingComponent>.op_Implicit((uid, active)));
			}
			else if (time >= active.LastMovedAt + charging.LastMovedGrace)
			{
				ResetCharging(Entity<ActiveXenoToggleChargingComponent>.op_Implicit((uid, active)), resetInput: false);
			}
		}
	}
}
