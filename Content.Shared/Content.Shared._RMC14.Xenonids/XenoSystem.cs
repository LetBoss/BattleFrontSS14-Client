using System;
using System.Linq;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Medical.Scanner;
using Content.Shared._RMC14.NightVision;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Tackle;
using Content.Shared._RMC14.Vendors;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Evolution;
using Content.Shared._RMC14.Xenonids.Fortify;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.HiveLeader;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared._RMC14.Xenonids.ScissorCut;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Access.Components;
using Content.Shared.Actions;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.Chat;
using Content.Shared.CombatMode;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Damage.Systems;
using Content.Shared.DragDrop;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Lathe;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Radio;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Storage.Components;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Stunnable;
using Content.Shared.Tools.Systems;
using Content.Shared.UserInterface;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids;

public sealed class XenoSystem : EntitySystem
{
	[Dependency]
	private SharedActionsSystem _action;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private SharedEntityStorageSystem _entityStorage;

	[Dependency]
	private SharedEyeSystem _eye;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	[Dependency]
	private HiveLeaderSystem _hiveLeader;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private MobThresholdSystem _mobThresholds;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private SharedNightVisionSystem _nightVision;

	[Dependency]
	private SharedRMCDamageableSystem _rmcDamageable;

	[Dependency]
	private SharedRMCFlammableSystem _rmcFlammable;

	[Dependency]
	private RMCPlanetSystem _rmcPlanet;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private StatusEffectsSystem _status;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private WeldableSystem _weldable;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	[Dependency]
	private SharedXenoWeedsSystem _weeds;

	private static readonly ProtoId<DamageTypePrototype> HeatDamage = ProtoId<DamageTypePrototype>.op_Implicit("Heat");

	private EntityQuery<AffectableByWeedsComponent> _affectableQuery;

	private EntityQuery<DamageableComponent> _damageableQuery;

	private EntityQuery<MobStateComponent> _mobStateQuery;

	private EntityQuery<MobThresholdsComponent> _mobThresholdsQuery;

	private EntityQuery<XenoFriendlyComponent> _xenoFriendlyQuery;

	private EntityQuery<XenoNestedComponent> _xenoNestedQuery;

	private EntityQuery<XenoPlasmaComponent> _xenoPlasmaQuery;

	private EntityQuery<XenoRecoveryPheromonesComponent> _xenoRecoveryQuery;

	private EntityQuery<VictimInfectedComponent> _victimInfectedQuery;

	private float _xenoDamageDealtMultiplier;

	private float _xenoDamageReceivedMultiplier;

	private float _xenoSpeedMultiplier;

	private TimeSpan _xenoSpawnMuteDuration;

	[Dependency]
	private RMCSizeStunSystem _size;

	public const float XENO_SLASH_DAMAGE_MULT = 1.5f;

	public const float XENO_DEBUFF_MULT = 1.25f;

	public const float XENO_ACID_DAMAGE_MULT = 2.625f;

	public const float XENO_PROJECTILE_DAMAGE_MULT = 2.625f;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Initialize();
		_affectableQuery = ((EntitySystem)this).GetEntityQuery<AffectableByWeedsComponent>();
		_damageableQuery = ((EntitySystem)this).GetEntityQuery<DamageableComponent>();
		_mobStateQuery = ((EntitySystem)this).GetEntityQuery<MobStateComponent>();
		_mobThresholdsQuery = ((EntitySystem)this).GetEntityQuery<MobThresholdsComponent>();
		_xenoFriendlyQuery = ((EntitySystem)this).GetEntityQuery<XenoFriendlyComponent>();
		_xenoNestedQuery = ((EntitySystem)this).GetEntityQuery<XenoNestedComponent>();
		_xenoPlasmaQuery = ((EntitySystem)this).GetEntityQuery<XenoPlasmaComponent>();
		_xenoRecoveryQuery = ((EntitySystem)this).GetEntityQuery<XenoRecoveryPheromonesComponent>();
		_victimInfectedQuery = ((EntitySystem)this).GetEntityQuery<VictimInfectedComponent>();
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, MapInitEvent>((EntityEventRefHandler<XenoComponent, MapInitEvent>)OnXenoMapInit, new Type[1] { typeof(SharedXenoPheromonesSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, GetAccessTagsEvent>((EntityEventRefHandler<XenoComponent, GetAccessTagsEvent>)OnXenoGetAdditionalAccess, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, NewXenoEvolvedEvent>((EntityEventRefHandler<XenoComponent, NewXenoEvolvedEvent>)OnNewXenoEvolved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, XenoDevolvedEvent>((EntityEventRefHandler<XenoComponent, XenoDevolvedEvent>)OnXenoDevolved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, HealthScannerAttemptTargetEvent>((EntityEventRefHandler<XenoComponent, HealthScannerAttemptTargetEvent>)OnXenoHealthScannerAttemptTarget, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, GetDefaultRadioChannelEvent>((EntityEventRefHandler<XenoComponent, GetDefaultRadioChannelEvent>)OnXenoGetDefaultRadioChannel, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, AttackAttemptEvent>((EntityEventRefHandler<XenoComponent, AttackAttemptEvent>)OnXenoAttackAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, MeleeAttackAttemptEvent>((EntityEventRefHandler<XenoComponent, MeleeAttackAttemptEvent>)OnXenoMeleeAttackAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, XenoHealAttemptEvent>((EntityEventRefHandler<XenoComponent, XenoHealAttemptEvent>)OnHealAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, UserOpenActivatableUIAttemptEvent>((EntityEventRefHandler<XenoComponent, UserOpenActivatableUIAttemptEvent>)OnXenoOpenActivatableUIAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, GetMeleeDamageEvent>((EntityEventRefHandler<XenoComponent, GetMeleeDamageEvent>)OnXenoGetMeleeDamage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, DamageModifyEvent>((EntityEventRefHandler<XenoComponent, DamageModifyEvent>)OnXenoDamageModify, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<XenoComponent, RefreshMovementSpeedModifiersEvent>)OnXenoRefreshSpeed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, MeleeHitEvent>((EntityEventRefHandler<XenoComponent, MeleeHitEvent>)OnXenoMeleeHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, HiveChangedEvent>((EntityEventRefHandler<XenoComponent, HiveChangedEvent>)OnHiveChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, IgnitedEvent>((EntityEventRefHandler<XenoComponent, IgnitedEvent>)OnXenoIgnite, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, CanDragEvent>((EntityEventRefHandler<XenoComponent, CanDragEvent>)OnXenoCanDrag, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, BuckleAttemptEvent>((EntityEventRefHandler<XenoComponent, BuckleAttemptEvent>)OnXenoBuckleAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, GetVisMaskEvent>((EntityEventRefHandler<XenoComponent, GetVisMaskEvent>)OnXenoGetVisMask, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, CMDisarmEvent>((EntityEventRefHandler<XenoComponent, CMDisarmEvent>)OnLeaderDisarmed, new Type[2]
		{
			typeof(SharedHandsSystem),
			typeof(SharedStaminaSystem)
		}, new Type[1] { typeof(TackleSystem) });
		((EntitySystem)this).SubscribeLocalEvent<XenoComponent, DisarmedEvent>((EntityEventRefHandler<XenoComponent, DisarmedEvent>)OnDisarmed, new Type[1] { typeof(SharedHandsSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRegenComponent, MapInitEvent>((EntityEventRefHandler<XenoRegenComponent, MapInitEvent>)OnXenoRegenMapInit, new Type[1] { typeof(SharedXenoPheromonesSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoRegenComponent, DamageStateCritBeforeDamageEvent>((EntityEventRefHandler<XenoRegenComponent, DamageStateCritBeforeDamageEvent>)OnXenoRegenBeforeCritDamage, new Type[1] { typeof(SharedXenoPheromonesSystem) }, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoStateVisualsComponent, MobStateChangedEvent>((EntityEventRefHandler<XenoStateVisualsComponent, MobStateChangedEvent>)OnVisualsMobStateChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoStateVisualsComponent, XenoFortifiedEvent>((EntityEventRefHandler<XenoStateVisualsComponent, XenoFortifiedEvent>)OnVisualsFortified, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoStateVisualsComponent, XenoRestEvent>((EntityEventRefHandler<XenoStateVisualsComponent, XenoRestEvent>)OnVisualsRest, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoStateVisualsComponent, DownedEvent>((EntityEventRefHandler<XenoStateVisualsComponent, DownedEvent>)OnVisualsProne, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoStateVisualsComponent, StoodEvent>((EntityEventRefHandler<XenoStateVisualsComponent, StoodEvent>)OnVisualsStand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<XenoStateVisualsComponent, XenoOvipositorChangedEvent>((EntityEventRefHandler<XenoStateVisualsComponent, XenoOvipositorChangedEvent>)OnVisualsOvipositor, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.CMXenoDamageDealtMultiplier, (Action<float>)delegate(float v)
		{
			_xenoDamageDealtMultiplier = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.CMXenoDamageReceivedMultiplier, (Action<float>)delegate(float v)
		{
			_xenoDamageReceivedMultiplier = v;
		}, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.CMXenoSpeedMultiplier, (Action<float>)UpdateXenoSpeedMultiplier, true);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, RMCCVars.RMCXenoSpawnInitialMuteDurationSeconds, (Action<float>)delegate(float v)
		{
			_xenoSpawnMuteDuration = TimeSpan.FromSeconds(v);
		}, true);
		((EntitySystem)this).UpdatesAfter.Add(typeof(SharedXenoPheromonesSystem));
	}

	private void OnXenoMapInit(Entity<XenoComponent> xeno, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntProtoId actionId in xeno.Comp.ActionIds)
		{
			if (!xeno.Comp.Actions.ContainsKey(actionId))
			{
				EntityUid? val = _action.AddAction(Entity<XenoComponent>.op_Implicit(xeno), EntProtoId.op_Implicit(actionId));
				if (val.HasValue)
				{
					EntityUid newAction = val.GetValueOrDefault();
					xeno.Comp.Actions[actionId] = newAction;
				}
			}
		}
		if (!MathHelper.CloseTo(_xenoSpeedMultiplier, 1f, 1E-07f))
		{
			_movementSpeed.RefreshMovementSpeedModifiers(Entity<XenoComponent>.op_Implicit(xeno));
		}
		if (xeno.Comp.MuteOnSpawn)
		{
			_status.TryAddStatusEffect(Entity<XenoComponent>.op_Implicit(xeno), "Muted", _xenoSpawnMuteDuration, refresh: true, "Muted");
		}
		_eye.RefreshVisibilityMask(Entity<EyeComponent>.op_Implicit(xeno.Owner));
		((EntitySystem)this).Dirty<XenoComponent>(xeno, (MetaDataComponent)null);
	}

	private void OnXenoGetAdditionalAccess(Entity<XenoComponent> xeno, ref GetAccessTagsEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		args.Tags.UnionWith(xeno.Comp.AccessLevels);
	}

	private void OnNewXenoEvolved(Entity<XenoComponent> newXeno, ref NewXenoEvolvedEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Angle oldRotation = _transform.GetWorldRotation(Entity<XenoEvolutionComponent>.op_Implicit(args.OldXeno));
		_transform.SetWorldRotation(Entity<XenoComponent>.op_Implicit(newXeno), oldRotation);
	}

	private void OnXenoDevolved(Entity<XenoComponent> newXeno, ref XenoDevolvedEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Angle oldRotation = _transform.GetWorldRotation(args.OldXeno);
		_transform.SetWorldRotation(Entity<XenoComponent>.op_Implicit(newXeno), oldRotation);
	}

	private void OnXenoHealthScannerAttemptTarget(Entity<XenoComponent> ent, ref HealthScannerAttemptTargetEvent args)
	{
		args.Popup = "The scanner can't make sense of this creature.";
		args.Cancelled = true;
	}

	private void OnXenoGetDefaultRadioChannel(Entity<XenoComponent> ent, ref GetDefaultRadioChannelEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		args.Channel = ProtoId<RadioChannelPrototype>.op_Implicit(SharedChatSystem.HivemindChannel);
	}

	private void OnXenoAttackAttempt(Entity<XenoComponent> xeno, ref AttackAttemptEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.Target;
		if (!target.HasValue)
		{
			return;
		}
		EntityUid target2 = target.GetValueOrDefault();
		if ((_xenoFriendlyQuery.HasComp(target2) && _hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(target2))) || _mobState.IsDead(target2))
		{
			if (!args.Disarm)
			{
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
		else if (_xenoNestedQuery.HasComp(target2) && _victimInfectedQuery.HasComp(target2) && !args.Disarm)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnXenoMeleeAttackAttempt(Entity<XenoComponent> xeno, ref MeleeAttackAttemptEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		XenoNestComponent nest = default(XenoNestComponent);
		if (!((EntitySystem)this).TryComp<XenoNestComponent>(((EntitySystem)this).GetEntity(args.Target), ref nest) || !nest.Nested.HasValue || !_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno.Owner), Entity<HiveMemberComponent>.op_Implicit(((EntitySystem)this).GetEntity(args.Target))))
		{
			return;
		}
		NetEntity attacker = ((EntitySystem)this).GetNetEntity(Entity<XenoComponent>.op_Implicit(xeno), (MetaDataComponent)null);
		args.Target = ((EntitySystem)this).GetNetEntity(nest.Nested.Value, (MetaDataComponent)null);
		AttackEvent attack = args.Attack;
		if (!(attack is LightAttackEvent attack2))
		{
			if (attack is DisarmAttackEvent disarm)
			{
				args.Attack = new DisarmAttackEvent(args.Target, disarm.Coordinates);
			}
		}
		else
		{
			args.Attack = new LightAttackEvent(args.Target, attacker, attack2.Coordinates);
		}
	}

	private void OnHealAttempt(Entity<XenoComponent> ent, ref XenoHealAttemptEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (_rmcFlammable.IsOnFire(Entity<FlammableComponent>.op_Implicit(ent.Owner)))
		{
			args.Cancelled = true;
		}
	}

	private void OnXenoOpenActivatableUIAttempt(Entity<XenoComponent> ent, ref UserOpenActivatableUIAttemptEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && (((EntitySystem)this).HasComp<LatheComponent>(args.Target) || ((EntitySystem)this).HasComp<CMAutomatedVendorComponent>(args.Target)))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnXenoGetMeleeDamage(Entity<XenoComponent> ent, ref GetMeleeDamageEvent args)
	{
		if (!MathHelper.CloseTo(_xenoDamageDealtMultiplier, 1f, 1E-07f))
		{
			args.Damage *= _xenoDamageDealtMultiplier;
		}
	}

	private void OnXenoDamageModify(Entity<XenoComponent> ent, ref DamageModifyEvent args)
	{
		if (!MathHelper.CloseTo(_xenoDamageReceivedMultiplier, 1f, 1E-07f))
		{
			args.Damage *= _xenoDamageReceivedMultiplier;
		}
	}

	private void OnXenoRefreshSpeed(Entity<XenoComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		if (!MathHelper.CloseTo(_xenoSpeedMultiplier, 1f, 1E-07f))
		{
			args.ModifySpeed(_xenoSpeedMultiplier, _xenoSpeedMultiplier);
		}
	}

	private void OnXenoMeleeHit(Entity<XenoComponent> xeno, ref MeleeHitEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid hit in args.HitEntities)
		{
			SharedEntityStorageComponent storage = null;
			if (_entityStorage.ResolveStorage(hit, ref storage))
			{
				if (_weldable.IsWelded(hit))
				{
					_weldable.SetWeldedState(hit, state: false);
				}
				_entityStorage.TryOpenStorage(Entity<XenoComponent>.op_Implicit(xeno), hit);
			}
		}
	}

	private void OnHiveChanged(Entity<XenoComponent> ent, ref HiveChangedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		_nightVision.SetSeeThroughContainers(Entity<NightVisionComponent>.op_Implicit(ent.Owner), args.Hive?.Comp.SeeThroughContainers ?? false);
	}

	private void OnXenoIgnite(Entity<XenoComponent> ent, ref IgnitedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		EntityUid[] array = _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(ent.Owner)).ToArray();
		foreach (EntityUid held in array)
		{
			if (((EntitySystem)this).HasComp<XenoParasiteComponent>(held))
			{
				DamageSpecifier damage = new DamageSpecifier
				{
					DamageDict = { [ProtoId<DamageTypePrototype>.op_Implicit(HeatDamage)] = 100 }
				};
				_damageable.TryChangeDamage(held, damage, ignoreResistances: true);
				_hands.TryDrop(Entity<HandsComponent>.op_Implicit(ent.Owner), held);
			}
		}
	}

	private void OnXenoCanDrag(Entity<XenoComponent> ent, ref CanDragEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (_mobState.IsDead(Entity<XenoComponent>.op_Implicit(ent)))
		{
			args.Handled = true;
		}
	}

	private void OnXenoBuckleAttempt(Entity<XenoComponent> ent, ref BuckleAttemptEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<XenoComponent>(args.User) || !_mobState.IsDead(Entity<XenoComponent>.op_Implicit(ent)))
		{
			args.Cancelled = true;
		}
	}

	private void OnXenoGetVisMask(Entity<XenoComponent> ent, ref GetVisMaskEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		args.VisibilityMask |= (int)ent.Comp.Visibility;
	}

	private void OnLeaderDisarmed(Entity<XenoComponent> ent, ref CMDisarmEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled && CanTackleOtherXeno(args.User, Entity<XenoComponent>.op_Implicit(ent), out var time))
		{
			_stun.TryParalyze(Entity<XenoComponent>.op_Implicit(ent), time, refresh: true);
		}
	}

	private void OnDisarmed(Entity<XenoComponent> ent, ref DisarmedEvent args)
	{
		args.PopupPrefix = "disarm-action-shove-";
		args.Handled = true;
	}

	private void OnXenoRegenMapInit(Entity<XenoRegenComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.NextRegenTime = _timing.CurTime + ent.Comp.RegenCooldown;
		((EntitySystem)this).Dirty<XenoRegenComponent>(ent, (MetaDataComponent)null);
	}

	private void OnXenoRegenBeforeCritDamage(Entity<XenoRegenComponent> ent, ref DamageStateCritBeforeDamageEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (_rmcFlammable.IsOnFire(Entity<FlammableComponent>.op_Implicit(ent.Owner)) || ent.Comp.HealOffWeeds || _weeds.IsOnFriendlyWeeds(Entity<TransformComponent>.op_Implicit(ent.Owner)))
		{
			args.Damage.ClampMax(0);
		}
	}

	private void UpdateXenoSpeedMultiplier(float speed)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		_xenoSpeedMultiplier = speed;
		EntityQueryEnumerator<XenoComponent, MovementSpeedModifierComponent> xenos = ((EntitySystem)this).EntityQueryEnumerator<XenoComponent, MovementSpeedModifierComponent>();
		EntityUid uid = default(EntityUid);
		XenoComponent xenoComponent = default(XenoComponent);
		MovementSpeedModifierComponent comp = default(MovementSpeedModifierComponent);
		while (xenos.MoveNext(ref uid, ref xenoComponent, ref comp))
		{
			_movementSpeed.RefreshMovementSpeedModifiers(uid, comp);
		}
	}

	public void MakeXeno(Entity<XenoComponent?> xeno)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).EnsureComp<XenoComponent>(Entity<XenoComponent>.op_Implicit(xeno));
	}

	private FixedPoint2 GetWeedsHealAmount(Entity<XenoRegenComponent> xeno)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		MobThresholdsComponent thresholds = default(MobThresholdsComponent);
		if (!_mobThresholdsQuery.TryComp(Entity<XenoRegenComponent>.op_Implicit(xeno), ref thresholds) || !_mobThresholds.TryGetIncapThreshold(Entity<XenoRegenComponent>.op_Implicit(xeno), out var threshold, thresholds))
		{
			return FixedPoint2.Zero;
		}
		FixedPoint2 multiplier = (_mobState.IsCritical(Entity<XenoRegenComponent>.op_Implicit(xeno)) ? xeno.Comp.CritHealMultiplier : ((!_standing.IsDown(Entity<XenoRegenComponent>.op_Implicit(xeno)) && !((EntitySystem)this).HasComp<XenoRestingComponent>(Entity<XenoRegenComponent>.op_Implicit(xeno))) ? xeno.Comp.StandHealingMultiplier : xeno.Comp.RestHealMultiplier));
		FixedPoint2 fixedPoint = threshold.Value / 65f + xeno.Comp.FlatHealing;
		FixedPoint2 recovery = ((EntitySystem)this).CompOrNull<XenoRecoveryPheromonesComponent>(Entity<XenoRegenComponent>.op_Implicit(xeno))?.Multiplier ?? ((FixedPoint2)0);
		if (!CanHeal(Entity<XenoRegenComponent>.op_Implicit(xeno)))
		{
			recovery = FixedPoint2.Zero;
		}
		FixedPoint2 recoveryHeal = threshold.Value / 65f * (recovery / 2f);
		return (fixedPoint + recoveryHeal) * multiplier / 2f;
	}

	public void HealDamage(Entity<DamageableComponent?> xeno, FixedPoint2 amount)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		MobStateComponent mobState = default(MobStateComponent);
		if (!_rmcFlammable.IsOnFire(Entity<FlammableComponent>.op_Implicit(xeno.Owner)) && _damageableQuery.Resolve(Entity<DamageableComponent>.op_Implicit(xeno), ref xeno.Comp, false) && !(xeno.Comp.Damage.GetTotal() <= FixedPoint2.Zero) && (!_mobStateQuery.TryGetComponent(Entity<DamageableComponent>.op_Implicit(xeno), ref mobState) || !_mobState.IsDead(Entity<DamageableComponent>.op_Implicit(xeno), mobState)))
		{
			DamageSpecifier heal = _rmcDamageable.DistributeTypes(Entity<DamageableComponent>.op_Implicit((Entity<DamageableComponent>.op_Implicit(xeno), xeno.Comp)), -amount);
			if (heal.GetTotal() > FixedPoint2.Zero)
			{
				((EntitySystem)this).Log.Error($"Tried to deal damage while healing xeno {((EntitySystem)this).ToPrettyString((EntityUid?)Entity<DamageableComponent>.op_Implicit(xeno), (MetaDataComponent)null)}");
			}
			else
			{
				_damageable.TryChangeDamage(Entity<DamageableComponent>.op_Implicit(xeno), heal, ignoreResistances: true, interruptsDoAfters: true, null, Entity<DamageableComponent>.op_Implicit(xeno));
			}
		}
	}

	public bool CanAbilityAttackTarget(EntityUid xeno, EntityUid target, bool canAttackBarricades = false, bool canAttackWindows = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (xeno == target)
		{
			return false;
		}
		if (_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(xeno), Entity<HiveMemberComponent>.op_Implicit(target)))
		{
			return false;
		}
		if (_mobState.IsDead(target))
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<DevouredComponent>(target))
		{
			return false;
		}
		if (_xenoNestedQuery.HasComp(target))
		{
			return false;
		}
		if (canAttackBarricades && ((EntitySystem)this).HasComp<BarricadeComponent>(target))
		{
			return true;
		}
		if (canAttackWindows && ((EntitySystem)this).HasComp<DestroyOnXenoPierceScissorComponent>(target))
		{
			return true;
		}
		if (!((EntitySystem)this).HasComp<MarineComponent>(target))
		{
			return ((EntitySystem)this).HasComp<XenoComponent>(target);
		}
		return true;
	}

	public bool CanHeal(EntityUid xeno)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		XenoHealAttemptEvent ev = default(XenoHealAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoHealAttemptEvent>(xeno, ref ev, false);
		return !ev.Cancelled;
	}

	public int GetGroundXenosAlive()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		int count = 0;
		EntityQueryEnumerator<ActorComponent, XenoComponent, MobStateComponent, TransformComponent> xenos = ((EntitySystem)this).EntityQueryEnumerator<ActorComponent, XenoComponent, MobStateComponent, TransformComponent>();
		ActorComponent val = default(ActorComponent);
		XenoComponent xenoComponent = default(XenoComponent);
		MobStateComponent mobState = default(MobStateComponent);
		TransformComponent xform = default(TransformComponent);
		while (xenos.MoveNext(ref val, ref xenoComponent, ref mobState, ref xform))
		{
			if (mobState.CurrentState != MobState.Dead && _rmcPlanet.IsOnPlanet(xform))
			{
				count++;
			}
		}
		return count;
	}

	public bool CanTackleOtherXeno(EntityUid sourceXeno, EntityUid targetXeno, out TimeSpan time)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		time = TimeSpan.Zero;
		if (!_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(targetXeno), Entity<HiveMemberComponent>.op_Implicit(sourceXeno)))
		{
			return false;
		}
		if (!_hiveLeader.IsLeader(sourceXeno, out HiveLeaderComponent leader))
		{
			return false;
		}
		if (_hiveLeader.IsLeader(targetXeno, out HiveLeaderComponent _))
		{
			return false;
		}
		if (((EntitySystem)this).HasComp<XenoEvolutionGranterComponent>(targetXeno))
		{
			return false;
		}
		time = leader.FriendlyStunTime;
		return true;
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<XenoRegenComponent> query = ((EntitySystem)this).EntityQueryEnumerator<XenoRegenComponent>();
		EntityUid uid = default(EntityUid);
		XenoRegenComponent xeno = default(XenoRegenComponent);
		XenoPlasmaComponent plasmaComp = default(XenoPlasmaComponent);
		XenoPlasmaComponent plasma = default(XenoPlasmaComponent);
		XenoRecoveryPheromonesComponent recovery = default(XenoRecoveryPheromonesComponent);
		while (query.MoveNext(ref uid, ref xeno))
		{
			if (time < xeno.NextRegenTime)
			{
				continue;
			}
			xeno.NextRegenTime = time + xeno.RegenCooldown;
			((EntitySystem)this).DirtyField<XenoRegenComponent>(uid, xeno, "NextRegenTime", (MetaDataComponent)null);
			if (!xeno.HealOffWeeds)
			{
				if (((EntitySystem)this).Transform(uid).Anchored)
				{
					_weeds.UpdateQueued(uid);
				}
				AffectableByWeedsComponent affectable = _affectableQuery.CompOrNull(uid);
				bool onWeeds = affectable != null && affectable.OnXenoWeeds && affectable.OnFriendlyWeeds;
				if (affectable == null || !onWeeds)
				{
					if (_xenoPlasmaQuery.TryComp(uid, ref plasmaComp))
					{
						FixedPoint2 amount = FixedPoint2.Max(plasmaComp.PlasmaRegenOffWeeds * plasmaComp.MaxPlasma / 100f / 2f, 0.01);
						_xenoPlasma.RegenPlasma(Entity<XenoPlasmaComponent>.op_Implicit((uid, plasmaComp)), amount);
					}
					continue;
				}
			}
			FixedPoint2 heal = GetWeedsHealAmount(Entity<XenoRegenComponent>.op_Implicit((uid, xeno)));
			if (!(heal > FixedPoint2.Zero))
			{
				continue;
			}
			HealDamage(Entity<DamageableComponent>.op_Implicit(uid), heal);
			if (_xenoPlasmaQuery.TryComp(uid, ref plasma))
			{
				FixedPoint2 plasmaRestored = plasma.PlasmaRegenOnWeeds * plasma.MaxPlasma / 100f / 2f;
				_xenoPlasma.RegenPlasma(Entity<XenoPlasmaComponent>.op_Implicit((uid, plasma)), plasmaRestored);
				if (_xenoRecoveryQuery.TryComp(uid, ref recovery))
				{
					FixedPoint2 amount2 = plasmaRestored * recovery.Multiplier / 4f;
					_xenoPlasma.RegenPlasma(Entity<XenoPlasmaComponent>.op_Implicit((uid, plasma)), amount2);
				}
			}
		}
	}

	public DamageSpecifier TryApplyXenoSlashDamageMultiplier(EntityUid target, DamageSpecifier baseDamage)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (!_size.TryGetSize(target, out var size) || !_size.IsXenoSized(size))
		{
			return baseDamage;
		}
		return baseDamage * 1.5f;
	}

	public TimeSpan TryApplyXenoDebuffMultiplier(EntityUid target, TimeSpan baseDuration)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (!_size.TryGetSize(target, out var size) || !_size.IsXenoSized(size))
		{
			return baseDuration;
		}
		return baseDuration * 1.25;
	}

	public DamageSpecifier TryApplyXenoAcidDamageMultiplier(EntityUid target, DamageSpecifier baseDamage)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(target))
		{
			return baseDamage;
		}
		return baseDamage * 2.625f;
	}

	public DamageSpecifier TryApplyXenoProjectileDamageMultiplier(EntityUid target, DamageSpecifier baseDamage)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<XenoComponent>(target))
		{
			return baseDamage;
		}
		return baseDamage * 2.625f;
	}

	private void OnVisualsMobStateChanged(Entity<XenoStateVisualsComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			_appearance.SetData(Entity<XenoStateVisualsComponent>.op_Implicit(ent), (Enum)RMCXenoStateVisuals.Downed, (object)(args.NewMobState != MobState.Alive), (AppearanceComponent)null);
			_appearance.SetData(Entity<XenoStateVisualsComponent>.op_Implicit(ent), (Enum)RMCXenoStateVisuals.Dead, (object)(args.NewMobState == MobState.Dead), (AppearanceComponent)null);
		}
	}

	private void OnVisualsFortified(Entity<XenoStateVisualsComponent> ent, ref XenoFortifiedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			_appearance.SetData(Entity<XenoStateVisualsComponent>.op_Implicit(ent), (Enum)RMCXenoStateVisuals.Fortified, (object)args.Fortified, (AppearanceComponent)null);
		}
	}

	private void OnVisualsRest(Entity<XenoStateVisualsComponent> ent, ref XenoRestEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			_appearance.SetData(Entity<XenoStateVisualsComponent>.op_Implicit(ent), (Enum)RMCXenoStateVisuals.Resting, (object)args.Resting, (AppearanceComponent)null);
		}
	}

	private void OnVisualsProne(Entity<XenoStateVisualsComponent> xeno, ref DownedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			_appearance.SetData(Entity<XenoStateVisualsComponent>.op_Implicit(xeno), (Enum)RMCXenoStateVisuals.Downed, (object)true, (AppearanceComponent)null);
		}
	}

	private void OnVisualsStand(Entity<XenoStateVisualsComponent> xeno, ref StoodEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			_appearance.SetData(Entity<XenoStateVisualsComponent>.op_Implicit(xeno), (Enum)RMCXenoStateVisuals.Downed, (object)false, (AppearanceComponent)null);
		}
	}

	private void OnVisualsOvipositor(Entity<XenoStateVisualsComponent> xeno, ref XenoOvipositorChangedEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState)
		{
			_appearance.SetData(Entity<XenoStateVisualsComponent>.op_Implicit(xeno), (Enum)RMCXenoStateVisuals.Ovipositor, (object)args.Attached, (AppearanceComponent)null);
		}
	}
}
