using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Entrenching;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines.Orders;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Content.Shared.Armor;
using Content.Shared.Blocking;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Silicons.Borgs;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Damage;

public abstract class SharedRMCDamageableSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private MobThresholdSystem _mobThresholds;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private IPrototypeManager _prototypes;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private RMCMapSystem _rmcMap;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private SharedXenoHiveSystem _hive;

	private static readonly ProtoId<DamageGroupPrototype> BruteGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Brute");

	private static readonly ProtoId<DamageGroupPrototype> BurnGroup = ProtoId<DamageGroupPrototype>.op_Implicit("Burn");

	private static readonly ProtoId<DamageTypePrototype> LethalDamageType = ProtoId<DamageTypePrototype>.op_Implicit("Asphyxiation");

	private readonly HashSet<ProtoId<DamageTypePrototype>> _bruteTypes = new HashSet<ProtoId<DamageTypePrototype>>();

	private readonly HashSet<ProtoId<DamageTypePrototype>> _burnTypes = new HashSet<ProtoId<DamageTypePrototype>>();

	private readonly List<string> _types = new List<string>();

	private EntityQuery<BarricadeComponent> _barricadeQuery;

	private EntityQuery<DamageableComponent> _damageableQuery;

	private EntityQuery<DamageOverTimeComponent> _damageOverTimeQuery;

	private EntityQuery<MobStateComponent> _mobStateQuery;

	private EntityQuery<VictimInfectedComponent> _victimInfectedQuery;

	private EntityQuery<XenoNestedComponent> _xenoNestedQuery;

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
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		_barricadeQuery = ((EntitySystem)this).GetEntityQuery<BarricadeComponent>();
		_damageableQuery = ((EntitySystem)this).GetEntityQuery<DamageableComponent>();
		_damageOverTimeQuery = ((EntitySystem)this).GetEntityQuery<DamageOverTimeComponent>();
		_mobStateQuery = ((EntitySystem)this).GetEntityQuery<MobStateComponent>();
		_victimInfectedQuery = ((EntitySystem)this).GetEntityQuery<VictimInfectedComponent>();
		_xenoNestedQuery = ((EntitySystem)this).GetEntityQuery<XenoNestedComponent>();
		((EntitySystem)this).SubscribeLocalEvent<DamageMobStateComponent, MapInitEvent>((EntityEventRefHandler<DamageMobStateComponent, MapInitEvent>)OnDamageMobStateMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DamageOverTimeComponent, StartCollideEvent>((EntityEventRefHandler<DamageOverTimeComponent, StartCollideEvent>)OnDamageOverTimeStartCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UserDamageOverTimeComponent, EndCollideEvent>((EntityEventRefHandler<UserDamageOverTimeComponent, EndCollideEvent>)OnDamageOverTimeEndCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DamageMultiplierFlagsComponent, DamageModifyEvent>((EntityEventRefHandler<DamageMultiplierFlagsComponent, DamageModifyEvent>)OnMultiplierFlagsDamageModify, (Type[])null, new Type[7]
		{
			typeof(SharedArmorSystem),
			typeof(BlockingSystem),
			typeof(InventorySystem),
			typeof(SharedBorgSystem),
			typeof(SharedMarineOrdersSystem),
			typeof(CMArmorSystem),
			typeof(SharedXenoPheromonesSystem)
		});
		((EntitySystem)this).SubscribeLocalEvent<GunDamageMultipliersComponent, AmmoShotEvent>((EntityEventRefHandler<GunDamageMultipliersComponent, AmmoShotEvent>)OnGunDamageMultipliersAmmoShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MaxDamageComponent, BeforeDamageChangedEvent>((EntityEventRefHandler<MaxDamageComponent, BeforeDamageChangedEvent>)OnMaxBeforeDamageChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MaxDamageComponent, DamageModifyEvent>((EntityEventRefHandler<MaxDamageComponent, DamageModifyEvent>)OnMaxDamageModify, (Type[])null, new Type[7]
		{
			typeof(SharedArmorSystem),
			typeof(BlockingSystem),
			typeof(InventorySystem),
			typeof(SharedBorgSystem),
			typeof(SharedMarineOrdersSystem),
			typeof(CMArmorSystem),
			typeof(SharedXenoPheromonesSystem)
		});
		((EntitySystem)this).SubscribeLocalEvent<DamageDealtModifierComponent, GetMeleeDamageEvent>((EntityEventRefHandler<DamageDealtModifierComponent, GetMeleeDamageEvent>)OnDamageModifierGetMeleeDamage, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DamageReceivedModifierComponent, DamageModifyEvent>((EntityEventRefHandler<DamageReceivedModifierComponent, DamageModifyEvent>)OnDamageReceivedDamageModify, (Type[])null, new Type[7]
		{
			typeof(SharedArmorSystem),
			typeof(BlockingSystem),
			typeof(InventorySystem),
			typeof(SharedBorgSystem),
			typeof(SharedMarineOrdersSystem),
			typeof(CMArmorSystem),
			typeof(SharedXenoPheromonesSystem)
		});
		((EntitySystem)this).SubscribeLocalEvent<ProjectileDamageReceivedComponent, DamageModifyEvent>((EntityEventRefHandler<ProjectileDamageReceivedComponent, DamageModifyEvent>)OnProjectileDamageReceivedModify, (Type[])null, new Type[7]
		{
			typeof(SharedArmorSystem),
			typeof(BlockingSystem),
			typeof(InventorySystem),
			typeof(SharedBorgSystem),
			typeof(SharedMarineOrdersSystem),
			typeof(CMArmorSystem),
			typeof(SharedXenoPheromonesSystem)
		});
		((EntitySystem)this).SubscribeLocalEvent<DamageOnPulledWhileCritComponent, MobStateChangedEvent>((EntityEventRefHandler<DamageOnPulledWhileCritComponent, MobStateChangedEvent>)OnDamageOnPulledMobState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveDamageOnPulledWhileCritComponent, MoveEvent>((EntityEventRefHandler<ActiveDamageOnPulledWhileCritComponent, MoveEvent>)OnActiveDamageOnPulledMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveDamageOnPulledWhileCritComponent, MobStateChangedEvent>((EntityEventRefHandler<ActiveDamageOnPulledWhileCritComponent, MobStateChangedEvent>)OnActiveDamageOnPulledMobState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ActiveDamageOnPulledWhileCritComponent, XenoTargetDevouredAttemptEvent>((EntityEventRefHandler<ActiveDamageOnPulledWhileCritComponent, XenoTargetDevouredAttemptEvent>)OnActiveDamageOnPulledDevoured, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<DamageReceivedModifierWhenUnanchoredComponent, DamageModifyEvent>((EntityEventRefHandler<DamageReceivedModifierWhenUnanchoredComponent, DamageModifyEvent>)OnDamageReceivedModifierWhenUnanchoredModify, (Type[])null, new Type[7]
		{
			typeof(SharedArmorSystem),
			typeof(BlockingSystem),
			typeof(InventorySystem),
			typeof(SharedBorgSystem),
			typeof(SharedMarineOrdersSystem),
			typeof(CMArmorSystem),
			typeof(SharedXenoPheromonesSystem)
		});
		_bruteTypes.Clear();
		_burnTypes.Clear();
		DamageGroupPrototype bruteProto = default(DamageGroupPrototype);
		if (_prototypes.TryIndex<DamageGroupPrototype>(BruteGroup, ref bruteProto))
		{
			foreach (string type in bruteProto.DamageTypes)
			{
				_bruteTypes.Add(ProtoId<DamageTypePrototype>.op_Implicit(type));
			}
		}
		DamageGroupPrototype burnProto = default(DamageGroupPrototype);
		if (!_prototypes.TryIndex<DamageGroupPrototype>(BurnGroup, ref burnProto))
		{
			return;
		}
		foreach (string type2 in burnProto.DamageTypes)
		{
			_burnTypes.Add(ProtoId<DamageTypePrototype>.op_Implicit(type2));
		}
	}

	private void OnProjectileDamageReceivedModify(Entity<ProjectileDamageReceivedComponent> ent, ref DamageModifyEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<ProjectileComponent>(args.Tool))
		{
			args.Damage *= ent.Comp.Multiplier;
		}
	}

	private void OnDamageMobStateMapInit(Entity<DamageMobStateComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.DamageAt = _timing.CurTime + ent.Comp.Cooldown;
		((EntitySystem)this).DirtyField<DamageMobStateComponent>(Entity<DamageMobStateComponent>.op_Implicit(ent), ent.Comp, "DamageAt", (MetaDataComponent)null);
	}

	private void OnDamageOverTimeStartCollide(Entity<DamageOverTimeComponent> ent, ref StartCollideEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			EntityUid other = args.OtherEntity;
			if (CanDamage(ent, Entity<MobStateComponent>.op_Implicit(other)))
			{
				((EntitySystem)this).EnsureComp<UserDamageOverTimeComponent>(other);
			}
		}
	}

	private void OnDamageOverTimeEndCollide(Entity<UserDamageOverTimeComponent> ent, ref EndCollideEvent args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		foreach (EntityUid contact in _physics.GetContactingEntities(Entity<UserDamageOverTimeComponent>.op_Implicit(ent), (PhysicsComponent)null, true))
		{
			if (_damageOverTimeQuery.HasComp(contact))
			{
				return;
			}
		}
		((EntitySystem)this).RemCompDeferred<UserDamageOverTimeComponent>(Entity<UserDamageOverTimeComponent>.op_Implicit(ent));
	}

	private void OnMultiplierFlagsDamageModify(Entity<DamageMultiplierFlagsComponent> ent, ref DamageModifyEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		DamageMultipliersComponent multComponent = default(DamageMultipliersComponent);
		if (!_damageableQuery.HasComp(Entity<DamageMultiplierFlagsComponent>.op_Implicit(ent)) || !((EntitySystem)this).TryComp<DamageMultipliersComponent>(args.Tool, ref multComponent))
		{
			return;
		}
		foreach (DamageMultiplierFlag flag in multComponent.Multipliers.Keys)
		{
			if ((ent.Comp.Flags & flag) != DamageMultiplierFlag.None)
			{
				args.Damage *= multComponent.Multipliers[flag];
			}
		}
	}

	private void OnGunDamageMultipliersAmmoShot(Entity<GunDamageMultipliersComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid projectile in args.FiredProjectiles)
		{
			((EntitySystem)this).EnsureComp<DamageMultipliersComponent>(projectile).Multipliers = ent.Comp.Multipliers;
		}
	}

	private void OnMaxBeforeDamageChanged(Entity<MaxDamageComponent> ent, ref BeforeDamageChangedEvent args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent damageable = default(DamageableComponent);
		if (!args.Cancelled && _damageableQuery.TryComp(Entity<MaxDamageComponent>.op_Implicit(ent), ref damageable) && damageable.TotalDamage >= ent.Comp.Max && args.Damage.GetTotal() > FixedPoint2.Zero)
		{
			args.Cancelled = true;
		}
	}

	private void OnMaxDamageModify(Entity<MaxDamageComponent> ent, ref DamageModifyEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent damageable = default(DamageableComponent);
		if (!_damageableQuery.TryComp(Entity<MaxDamageComponent>.op_Implicit(ent), ref damageable))
		{
			return;
		}
		FixedPoint2 modifyTotal = args.Damage.GetTotal();
		if (!(modifyTotal <= FixedPoint2.Zero) && !(damageable.TotalDamage + modifyTotal <= ent.Comp.Max))
		{
			FixedPoint2 remaining = ent.Comp.Max - damageable.TotalDamage;
			if (ent.Comp.Max <= FixedPoint2.Zero)
			{
				args.Damage *= 0f;
			}
			else if (!(modifyTotal <= remaining))
			{
				args.Damage *= remaining.Float() / modifyTotal.Float();
			}
		}
	}

	private void OnDamageModifierGetMeleeDamage(Entity<DamageDealtModifierComponent> ent, ref GetMeleeDamageEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.Damage *= ent.Comp.MeleeMultiplier;
	}

	private void OnDamageReceivedDamageModify(Entity<DamageReceivedModifierComponent> ent, ref DamageModifyEvent args)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<string, FixedPoint2> item in args.Damage.DamageDict)
		{
			item.Deconstruct(out var key, out var value);
			string type = key;
			if (!(value <= FixedPoint2.Zero))
			{
				if (ent.Comp.Multiplier != 1)
				{
					Dictionary<string, FixedPoint2> damageDict = args.Damage.DamageDict;
					key = type;
					damageDict[key] *= ent.Comp.Multiplier;
				}
				if (ent.Comp.BruteMultiplier != 1 && _bruteTypes.Contains(ProtoId<DamageTypePrototype>.op_Implicit(type)))
				{
					Dictionary<string, FixedPoint2> damageDict = args.Damage.DamageDict;
					key = type;
					damageDict[key] *= ent.Comp.BruteMultiplier;
				}
				else if (ent.Comp.BurnMultiplier != 1 && _burnTypes.Contains(ProtoId<DamageTypePrototype>.op_Implicit(type)))
				{
					Dictionary<string, FixedPoint2> damageDict = args.Damage.DamageDict;
					key = type;
					damageDict[key] *= ent.Comp.BurnMultiplier;
				}
			}
		}
	}

	private void OnDamageOnPulledMobState(Entity<DamageOnPulledWhileCritComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		ActiveDamageOnPulledWhileCritComponent active = default(ActiveDamageOnPulledWhileCritComponent);
		if (args.NewMobState == MobState.Critical && !((EntitySystem)this).EnsureComp<ActiveDamageOnPulledWhileCritComponent>(Entity<DamageOnPulledWhileCritComponent>.op_Implicit(ent), ref active))
		{
			active.Damage = ent.Comp.Damage;
			active.PullerWhitelist = ent.Comp.PullerWhitelist;
			active.Every = ent.Comp.Every;
			((EntitySystem)this).Dirty(Entity<DamageOnPulledWhileCritComponent>.op_Implicit(ent), (IComponent)(object)active, (MetaDataComponent)null);
		}
	}

	private void OnActiveDamageOnPulledMove(Entity<ActiveDamageOnPulledWhileCritComponent> ent, ref MoveEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Damage == null)
		{
			return;
		}
		float distance = default(float);
		if (!_rmcPulling.IsBeingPulled(Entity<PullableComponent>.op_Implicit(ent.Owner), out var user) || !_entityWhitelist.IsWhitelistPassOrNull(ent.Comp.PullerWhitelist, user))
		{
			if (ent.Comp.Pulled)
			{
				ent.Comp.Pulled = false;
				((EntitySystem)this).Dirty<ActiveDamageOnPulledWhileCritComponent>(ent, (MetaDataComponent)null);
			}
		}
		else if (((EntityCoordinates)(ref args.NewPosition)).TryDistance((IEntityManager)(object)base.EntityManager, _transform, args.OldPosition, ref distance))
		{
			if (!ent.Comp.Pulled)
			{
				ent.Comp.Pulled = true;
			}
			ent.Comp.Accumulator += distance;
			((EntitySystem)this).Dirty<ActiveDamageOnPulledWhileCritComponent>(ent, (MetaDataComponent)null);
		}
	}

	private void OnActiveDamageOnPulledMobState(Entity<ActiveDamageOnPulledWhileCritComponent> ent, ref MobStateChangedEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (args.NewMobState != MobState.Critical)
		{
			((EntitySystem)this).RemCompDeferred<ActiveDamageOnPulledWhileCritComponent>(Entity<ActiveDamageOnPulledWhileCritComponent>.op_Implicit(ent));
		}
	}

	private void OnActiveDamageOnPulledDevoured(Entity<ActiveDamageOnPulledWhileCritComponent> ent, ref XenoTargetDevouredAttemptEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		DamageableComponent damageable = default(DamageableComponent);
		if (_mobThresholds.TryGetDeadThreshold(ent.Owner, out var mobThreshold) && ((EntitySystem)this).TryComp<DamageableComponent>(Entity<ActiveDamageOnPulledWhileCritComponent>.op_Implicit(ent), ref damageable))
		{
			FixedPoint2 lethalAmountOfDamage = mobThreshold.Value - damageable.TotalDamage;
			DamageSpecifier damage = new DamageSpecifier(_prototypes.Index<DamageTypePrototype>(LethalDamageType), lethalAmountOfDamage);
			_damageable.TryChangeDamage(ent.Owner, damage, ignoreResistances: true);
		}
		args.Cancelled = true;
	}

	private void OnDamageReceivedModifierWhenUnanchoredModify(Entity<DamageReceivedModifierWhenUnanchoredComponent> ent, ref DamageModifyEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = default(TransformComponent);
		if (((EntitySystem)this).TryComp(Entity<DamageReceivedModifierWhenUnanchoredComponent>.op_Implicit(ent), ref xform) && !xform.Anchored && !(args.Damage.GetTotal() <= FixedPoint2.Zero))
		{
			args.Damage *= ent.Comp.Multiplier;
		}
	}

	public DamageSpecifier DistributeDamageCached(Entity<DamageableComponent?> damageable, ProtoId<DamageGroupPrototype> groupId, FixedPoint2 amount, DamageSpecifier? equal = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		if (equal == null)
		{
			equal = new DamageSpecifier();
		}
		if (!_damageableQuery.Resolve(Entity<DamageableComponent>.op_Implicit(damageable), ref damageable.Comp, false))
		{
			return equal;
		}
		DamageGroupPrototype group = default(DamageGroupPrototype);
		if (!_prototypes.TryIndex<DamageGroupPrototype>(groupId, ref group))
		{
			return equal;
		}
		_types.Clear();
		foreach (string type in group.DamageTypes)
		{
			if (damageable.Comp.Damage.DamageDict.TryGetValue(type, out var current) && current > FixedPoint2.Zero)
			{
				_types.Add(type);
			}
		}
		Dictionary<string, FixedPoint2> damage = equal.DamageDict;
		bool add = amount > FixedPoint2.Zero;
		FixedPoint2 left = amount;
		while (add ? (left > 0) : (left < 0))
		{
			FixedPoint2 lastLeft = left;
			for (int i = _types.Count - 1; i >= 0; i--)
			{
				string type2 = _types[i];
				FixedPoint2 current2 = damageable.Comp.Damage.DamageDict[type2];
				FixedPoint2 existingHeal = (add ? (-damage.GetValueOrDefault(type2)) : damage.GetValueOrDefault(type2));
				left += existingHeal;
				FixedPoint2 toDamage = (add ? FixedPoint2.Min(existingHeal + left / (i + 1), current2) : (-FixedPoint2.Min(-(existingHeal + left / (i + 1)), current2)));
				if (current2 <= FixedPoint2.Abs(toDamage))
				{
					_types.RemoveAt(i);
				}
				damage[type2] = toDamage;
				left -= toDamage;
			}
			if (lastLeft == left)
			{
				break;
			}
		}
		return equal;
	}

	public DamageSpecifier DistributeHealing(Entity<DamageableComponent?> damageable, ProtoId<DamageGroupPrototype> groupId, FixedPoint2 amount)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (amount > FixedPoint2.Zero)
		{
			amount = -amount;
		}
		return DistributeDamageCached(damageable, groupId, amount);
	}

	public DamageSpecifier DistributeHealingCached(Entity<DamageableComponent?> damageable, ProtoId<DamageGroupPrototype> groupId, FixedPoint2 amount, DamageSpecifier? equal = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (amount > FixedPoint2.Zero)
		{
			amount = -amount;
		}
		return DistributeDamageCached(damageable, groupId, amount, equal);
	}

	public DamageSpecifier DistributeTypes(Entity<DamageableComponent?> damageable, FixedPoint2 amount)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier equal = new DamageSpecifier();
		foreach (DamageGroupPrototype group in _prototypes.EnumeratePrototypes<DamageGroupPrototype>())
		{
			equal = DistributeDamageCached(damageable, ProtoId<DamageGroupPrototype>.op_Implicit(group.ID), amount, equal);
		}
		return equal;
	}

	public DamageSpecifier DistributeTypesTotal(Entity<DamageableComponent?> damageable, FixedPoint2 amount)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier equal = new DamageSpecifier();
		foreach (DamageGroupPrototype group in _prototypes.EnumeratePrototypes<DamageGroupPrototype>())
		{
			FixedPoint2 total = equal?.GetTotal() ?? FixedPoint2.Zero;
			FixedPoint2 left = amount - total;
			if (left <= FixedPoint2.Zero)
			{
				break;
			}
			equal = DistributeDamageCached(damageable, ProtoId<DamageGroupPrototype>.op_Implicit(group.ID), left, equal);
		}
		return equal ?? new DamageSpecifier();
	}

	protected virtual void DoEmote(EntityUid ent, ProtoId<EmotePrototype> emote)
	{
	}

	private bool CanDamage(Entity<DamageOverTimeComponent> damage, Entity<MobStateComponent?> target)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (damage.Comp.BarricadeDamage != null && _barricadeQuery.HasComp(Entity<MobStateComponent>.op_Implicit(target)))
		{
			return true;
		}
		if (!((EntitySystem)this).Resolve<MobStateComponent>(Entity<MobStateComponent>.op_Implicit(target), ref target.Comp, false))
		{
			return false;
		}
		if (!_entityWhitelist.IsWhitelistPassOrNull(damage.Comp.Whitelist, Entity<MobStateComponent>.op_Implicit(target)))
		{
			return false;
		}
		if (!damage.Comp.AffectsDead && _mobState.IsDead(Entity<MobStateComponent>.op_Implicit(target)))
		{
			return false;
		}
		if (!damage.Comp.AffectsCrit && _mobState.IsCritical(Entity<MobStateComponent>.op_Implicit(target)))
		{
			return false;
		}
		if (!damage.Comp.AffectsInfectedNested && _xenoNestedQuery.HasComp(Entity<MobStateComponent>.op_Implicit(target)) && _victimInfectedQuery.HasComp(Entity<MobStateComponent>.op_Implicit(target)))
		{
			return false;
		}
		return true;
	}

	private void DoDamage(Entity<DamageOverTimeComponent> damageEnt, EntityUid target, DamageSpecifier damage, bool ignoreResistances = false, bool acidic = false)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		DamageSpecifier damageBase = damage;
		if (acidic)
		{
			damageBase = _xeno.TryApplyXenoAcidDamageMultiplier(target, damageBase);
		}
		List<DamageOverTimeComponent.DamageMultiplier> multipliers = damageEnt.Comp.Multipliers;
		if (multipliers != null)
		{
			foreach (DamageOverTimeComponent.DamageMultiplier multiplier in multipliers)
			{
				if (_entityWhitelist.IsWhitelistPass(multiplier.Whitelist, target))
				{
					_damageable.TryChangeDamage(target, damageBase * multiplier.Multiplier, ignoreResistances);
					return;
				}
			}
		}
		_damageable.TryChangeDamage(target, damageBase, ignoreResistances);
	}

	public virtual bool TryGetDestroyedAt(EntityUid destructible, [NotNullWhen(true)] out FixedPoint2? destroyed)
	{
		destroyed = null;
		return false;
	}

	public bool HasAnyDamage(Entity<DamageableComponent?> damageable, DamageSpecifier damage)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!_damageableQuery.Resolve(Entity<DamageableComponent>.op_Implicit(damageable), ref damageable.Comp, false))
		{
			return false;
		}
		foreach (var (type, _) in damage.DamageDict)
		{
			if (damageable.Comp.Damage.DamageDict.TryGetValue(type, out var healValue) && healValue > FixedPoint2.Zero)
			{
				return true;
			}
		}
		return false;
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<DamageMobStateComponent> damageMobStateQuery = ((EntitySystem)this).EntityQueryEnumerator<DamageMobStateComponent>();
		EntityUid uid = default(EntityUid);
		DamageMobStateComponent comp = default(DamageMobStateComponent);
		MobStateComponent state = default(MobStateComponent);
		DamageableComponent damageable = default(DamageableComponent);
		while (damageMobStateQuery.MoveNext(ref uid, ref comp))
		{
			if (time < comp.DamageAt)
			{
				continue;
			}
			comp.DamageAt = time + comp.Cooldown;
			((EntitySystem)this).DirtyField<DamageMobStateComponent>(uid, comp, "DamageAt", (MetaDataComponent)null);
			if (!_mobStateQuery.TryComp(uid, ref state) || !_damageableQuery.TryComp(uid, ref damageable))
			{
				continue;
			}
			switch (state.CurrentState)
			{
			case MobState.Alive:
				if (HasAnyDamage(Entity<DamageableComponent>.op_Implicit((uid, damageable)), comp.NonDeadDamage))
				{
					_damageable.TryChangeDamage(uid, comp.NonDeadDamage, ignoreResistances: true, interruptsDoAfters: true, damageable);
				}
				break;
			case MobState.Critical:
			{
				_damageable.TryChangeDamage(uid, comp.NonDeadDamage, ignoreResistances: true, interruptsDoAfters: true, damageable);
				DamageStateCritBeforeDamageEvent ev = new DamageStateCritBeforeDamageEvent(comp.CritDamage);
				((EntitySystem)this).RaiseLocalEvent<DamageStateCritBeforeDamageEvent>(uid, ref ev, false);
				_damageable.TryChangeDamage(uid, ev.Damage, ignoreResistances: true, interruptsDoAfters: true, damageable);
				break;
			}
			}
		}
		EntityQueryEnumerator<ActiveDamageOnPulledWhileCritComponent> pulledQuery = ((EntitySystem)this).EntityQueryEnumerator<ActiveDamageOnPulledWhileCritComponent>();
		EntityUid uid2 = default(EntityUid);
		ActiveDamageOnPulledWhileCritComponent comp2 = default(ActiveDamageOnPulledWhileCritComponent);
		while (pulledQuery.MoveNext(ref uid2, ref comp2))
		{
			if (comp2.Accumulator >= comp2.Every)
			{
				double num = comp2.Accumulator / comp2.Every;
				double rem = comp2.Accumulator % comp2.Every;
				double div = num;
				if (comp2.Damage != null)
				{
					_damageable.TryChangeDamage(uid2, comp2.Damage * div);
				}
				comp2.Accumulator = rem;
			}
		}
		if (_net.IsClient)
		{
			return;
		}
		EntityQueryEnumerator<DamageOverTimeComponent> damageOverTimeQuery = ((EntitySystem)this).EntityQueryEnumerator<DamageOverTimeComponent>();
		EntityUid uid3 = default(EntityUid);
		DamageOverTimeComponent damage = default(DamageOverTimeComponent);
		while (damageOverTimeQuery.MoveNext(ref uid3, ref damage))
		{
			if (time >= damage.NextDamageAt)
			{
				damage.NextDamageAt = time + damage.DamageEvery;
				RMCAnchoredEntitiesEnumerator anchoredEnumerator = _rmcMap.GetAnchoredEntitiesEnumerator(uid3, null, (DirectionFlag)0);
				EntityUid anchored;
				while (anchoredEnumerator.MoveNext(out anchored))
				{
					if (!_barricadeQuery.HasComp(anchored) || damage.BarricadeDamage == null)
					{
						continue;
					}
					UserDamageOverTimeComponent userDamage = ((EntitySystem)this).EnsureComp<UserDamageOverTimeComponent>(anchored);
					if (!(time < userDamage.NextDamageAt))
					{
						userDamage.NextDamageAt = time;
						DoDamage(Entity<DamageOverTimeComponent>.op_Implicit((uid3, damage)), anchored, damage.BarricadeDamage);
						if (RandomExtensions.Prob(_random, 0.75f))
						{
							_audio.PlayPvs(damage.BarricadeSound, anchored, (AudioParams?)null);
						}
					}
				}
			}
			if (damage.InitDamaged)
			{
				continue;
			}
			damage.InitDamaged = true;
			foreach (EntityUid contact in _physics.GetEntitiesIntersectingBody(uid3, (int)damage.Collision, true, (PhysicsComponent)null, (FixturesComponent)null, (TransformComponent)null))
			{
				if (CanDamage(Entity<DamageOverTimeComponent>.op_Implicit((uid3, damage)), Entity<MobStateComponent>.op_Implicit(contact)))
				{
					((EntitySystem)this).EnsureComp<UserDamageOverTimeComponent>(contact);
				}
			}
		}
		EntityQueryEnumerator<UserDamageOverTimeComponent> userDamageOverTimeQuery = ((EntitySystem)this).EntityQueryEnumerator<UserDamageOverTimeComponent>();
		EntityUid user = default(EntityUid);
		UserDamageOverTimeComponent userDamage2 = default(UserDamageOverTimeComponent);
		DamageOverTimeComponent damage2 = default(DamageOverTimeComponent);
		while (userDamageOverTimeQuery.MoveNext(ref user, ref userDamage2))
		{
			if (time < userDamage2.NextDamageAt)
			{
				continue;
			}
			HashSet<EntityUid> contacts = _physics.GetEntitiesIntersectingBody(user, (int)userDamage2.Collision, true, (PhysicsComponent)null, (FixturesComponent)null, (TransformComponent)null);
			if (contacts.Count == 0)
			{
				((EntitySystem)this).RemCompDeferred<UserDamageOverTimeComponent>(user);
				continue;
			}
			foreach (EntityUid contact2 in contacts)
			{
				if (_damageOverTimeQuery.TryComp(contact2, ref damage2) && (damage2.AffectsDead || !_mobState.IsDead(user)) && (damage2.AffectsCrit || !_mobState.IsCritical(user)) && (damage2.AffectsInfectedNested || !_xenoNestedQuery.HasComp(user) || !_victimInfectedQuery.HasComp(user)) && _entityWhitelist.IsWhitelistPassOrNull(damage2.Whitelist, user) && !_hive.FromSameHive(Entity<HiveMemberComponent>.op_Implicit(contact2), Entity<HiveMemberComponent>.op_Implicit(user)))
				{
					userDamage2.NextDamageAt = time + userDamage2.DamageEvery;
					if (damage2.Damage != null)
					{
						DoDamage(Entity<DamageOverTimeComponent>.op_Implicit((contact2, damage2)), user, damage2.Damage);
					}
					if (damage2.ArmorPiercingDamage != null)
					{
						DoDamage(Entity<DamageOverTimeComponent>.op_Implicit((contact2, damage2)), user, damage2.ArmorPiercingDamage, ignoreResistances: true, damage2.Acidic);
					}
					List<ProtoId<EmotePrototype>> emotes = damage2.Emotes;
					if (emotes != null && emotes.Count > 0)
					{
						ProtoId<EmotePrototype> emote = RandomExtensions.Pick<ProtoId<EmotePrototype>>(_random, (IReadOnlyList<ProtoId<EmotePrototype>>)emotes);
						DoEmote(user, emote);
					}
					string popup = damage2.Popup;
					if (popup != null && RandomExtensions.Prob(_random, 0.5f))
					{
						_popup.PopupEntity(popup, user, user, PopupType.SmallCaution);
					}
					_audio.PlayPvs(damage2.Sound, user, (AudioParams?)null);
					break;
				}
			}
		}
	}
}
