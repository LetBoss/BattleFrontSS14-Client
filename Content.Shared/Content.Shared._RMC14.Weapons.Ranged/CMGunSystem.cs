using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Evasion;
using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Marines.Orders;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Projectiles;
using Content.Shared._RMC14.Stealth;
using Content.Shared._RMC14.Vehicle;
using Content.Shared._RMC14.Weapons.Common;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared._RMC14.Weapons.Ranged.Whitelist;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Inventory;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Standing;
using Content.Shared.Timing;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Content.Shared.Wieldable;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class CMGunSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedBroadphaseSystem _broadphase;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedGunSystem _gun;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private INetConfigurationManager _netConfig;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	private EntityLookupSystem _lookup;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedProjectileSystem _projectile;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private SharedRMCLagCompensationSystem _rmcLagCompensation;

	[Dependency]
	private RMCProjectileSystem _rmcProjectileSystem;

	[Dependency]
	private VehicleWeaponsSystem _vehicleWeapons;

	[Dependency]
	private SkillsSystem _skills;

	[Dependency]
	private ItemSlotsSystem _slots;

	[Dependency]
	private StandingStateSystem _standing;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private UseDelaySystem _useDelay;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	private EntityQuery<PhysicsComponent> _physicsQuery;

	private EntityQuery<ProjectileComponent> _projectileQuery;

	private HashSet<Entity<FixturesComponent>> _intersectedEntities = new HashSet<Entity<FixturesComponent>>();

	private HashSet<Entity<FixturesComponent>> _impassableEntities = new HashSet<Entity<FixturesComponent>>();

	private readonly int _blockArcCollisionGroup = 10;

	private const string AccuracyExamineColour = "yellow";

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_physicsQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		_projectileQuery = ((EntitySystem)this).GetEntityQuery<ProjectileComponent>();
		((EntitySystem)this).SubscribeLocalEvent<ShootAtFixedPointComponent, AmmoShotEvent>((EntityEventRefHandler<ShootAtFixedPointComponent, AmmoShotEvent>)OnShootAtFixedPointShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<IgnoreArcComponent, BeforeArcEvent>((EntityEventRefHandler<IgnoreArcComponent, BeforeArcEvent>)OnBeforeArc, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCWeaponDamageFalloffComponent, AmmoShotEvent>((EntityEventRefHandler<RMCWeaponDamageFalloffComponent, AmmoShotEvent>)OnWeaponDamageFalloffShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCWeaponDamageFalloffComponent, GunRefreshModifiersEvent>((EntityEventRefHandler<RMCWeaponDamageFalloffComponent, GunRefreshModifiersEvent>)OnWeaponDamageFalloffRefreshModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCExtraProjectilesDamageModsComponent, AmmoShotEvent>((EntityEventRefHandler<RMCExtraProjectilesDamageModsComponent, AmmoShotEvent>)OnExtraProjectilesShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCWeaponAccuracyComponent, ExaminedEvent>((EntityEventRefHandler<RMCWeaponAccuracyComponent, ExaminedEvent>)OnWeaponAccuracyExamined, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCWeaponAccuracyComponent, GunRefreshModifiersEvent>((EntityEventRefHandler<RMCWeaponAccuracyComponent, GunRefreshModifiersEvent>)OnWeaponAccuracyRefreshModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCWeaponAccuracyComponent, AmmoShotEvent>((EntityEventRefHandler<RMCWeaponAccuracyComponent, AmmoShotEvent>)OnWeaponAccuracyShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProjectileFixedDistanceComponent, PreventCollideEvent>((EntityEventRefHandler<ProjectileFixedDistanceComponent, PreventCollideEvent>)OnCollisionCheckArc, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ProjectileFixedDistanceComponent, PhysicsSleepEvent>((EntityEventRefHandler<ProjectileFixedDistanceComponent, PhysicsSleepEvent>)OnEventToStopProjectile, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunShowUseDelayComponent, GunShotEvent>((EntityEventRefHandler<GunShowUseDelayComponent, GunShotEvent>)OnShowUseDelayShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunShowUseDelayComponent, ItemWieldedEvent>((EntityEventRefHandler<GunShowUseDelayComponent, ItemWieldedEvent>)OnShowUseDelayWielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunUserWhitelistComponent, AttemptShootEvent>((EntityEventRefHandler<GunUserWhitelistComponent, AttemptShootEvent>)OnGunUserWhitelistAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunUnskilledPenaltyComponent, GotEquippedHandEvent>((EntityEventRefHandler<GunUnskilledPenaltyComponent, GotEquippedHandEvent>)TryRefreshGunModifiers<GunUnskilledPenaltyComponent, GotEquippedHandEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunUnskilledPenaltyComponent, GotUnequippedHandEvent>((EntityEventRefHandler<GunUnskilledPenaltyComponent, GotUnequippedHandEvent>)TryRefreshGunModifiers<GunUnskilledPenaltyComponent, GotUnequippedHandEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunUnskilledPenaltyComponent, GunRefreshModifiersEvent>((EntityEventRefHandler<GunUnskilledPenaltyComponent, GunRefreshModifiersEvent>)OnGunUnskilledPenaltyRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunUnskilledPenaltyComponent, GetWeaponAccuracyEvent>((EntityEventRefHandler<GunUnskilledPenaltyComponent, GetWeaponAccuracyEvent>)OnGunUnskilledPenaltyGetWeaponAccuracy, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunDamageModifierComponent, AmmoShotEvent>((EntityEventRefHandler<GunDamageModifierComponent, AmmoShotEvent>)OnGunDamageModifierAmmoShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunDamageModifierComponent, MapInitEvent>((EntityEventRefHandler<GunDamageModifierComponent, MapInitEvent>)OnGunDamageModifierMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunPointBlankComponent, AmmoShotEvent>((EntityEventRefHandler<GunPointBlankComponent, AmmoShotEvent>)OnGunPointBlankAmmoShot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunSkilledRecoilComponent, GotEquippedHandEvent>((EntityEventRefHandler<GunSkilledRecoilComponent, GotEquippedHandEvent>)TryRefreshGunModifiers<GunSkilledRecoilComponent, GotEquippedHandEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunSkilledRecoilComponent, GotUnequippedHandEvent>((EntityEventRefHandler<GunSkilledRecoilComponent, GotUnequippedHandEvent>)TryRefreshGunModifiers<GunSkilledRecoilComponent, GotUnequippedHandEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunSkilledRecoilComponent, ItemWieldedEvent>((EntityEventRefHandler<GunSkilledRecoilComponent, ItemWieldedEvent>)TryRefreshGunModifiers<GunSkilledRecoilComponent, ItemWieldedEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunSkilledRecoilComponent, ItemUnwieldedEvent>((EntityEventRefHandler<GunSkilledRecoilComponent, ItemUnwieldedEvent>)TryRefreshGunModifiers<GunSkilledRecoilComponent, ItemUnwieldedEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunSkilledRecoilComponent, GunRefreshModifiersEvent>((EntityEventRefHandler<GunSkilledRecoilComponent, GunRefreshModifiersEvent>)OnRecoilSkilledRefreshModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunSkilledAccuracyComponent, GotEquippedHandEvent>((EntityEventRefHandler<GunSkilledAccuracyComponent, GotEquippedHandEvent>)TryRefreshGunModifiers<GunSkilledAccuracyComponent, GotEquippedHandEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunSkilledAccuracyComponent, GotUnequippedHandEvent>((EntityEventRefHandler<GunSkilledAccuracyComponent, GotUnequippedHandEvent>)TryRefreshGunModifiers<GunSkilledAccuracyComponent, GotUnequippedHandEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunSkilledAccuracyComponent, ItemWieldedEvent>((EntityEventRefHandler<GunSkilledAccuracyComponent, ItemWieldedEvent>)TryRefreshGunModifiers<GunSkilledAccuracyComponent, ItemWieldedEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunSkilledAccuracyComponent, ItemUnwieldedEvent>((EntityEventRefHandler<GunSkilledAccuracyComponent, ItemUnwieldedEvent>)TryRefreshGunModifiers<GunSkilledAccuracyComponent, ItemUnwieldedEvent>, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunSkilledAccuracyComponent, GetWeaponAccuracyEvent>((EntityEventRefHandler<GunSkilledAccuracyComponent, GetWeaponAccuracyEvent>)OnAccuracySkilledGetWeaponAccuracy, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunRequiresSkillsComponent, AttemptShootEvent>((EntityEventRefHandler<GunRequiresSkillsComponent, AttemptShootEvent>)OnRequiresSkillsAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunRequireEquippedComponent, AttemptShootEvent>((EntityEventRefHandler<GunRequireEquippedComponent, AttemptShootEvent>)OnRequireEquippedAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RevolverAmmoProviderComponent, UniqueActionEvent>((EntityEventRefHandler<RevolverAmmoProviderComponent, UniqueActionEvent>)OnRevolverUniqueAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UserBlockShootingInsideContainersComponent, ShotAttemptedEvent>((EntityEventRefHandler<UserBlockShootingInsideContainersComponent, ShotAttemptedEvent>)OnUserBlockShootingInsideContainersAttemptShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCAmmoEjectComponent, ActivateInWorldEvent>((EntityEventRefHandler<RMCAmmoEjectComponent, ActivateInWorldEvent>)OnAmmoEjectActivateInWorld, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AssistedReloadAmmoComponent, AfterInteractEvent>((EntityEventRefHandler<AssistedReloadAmmoComponent, AfterInteractEvent>)OnAssistedReloadAmmoAfterInteract, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AssistedReloadWeaponComponent, ItemWieldedEvent>((EntityEventRefHandler<AssistedReloadWeaponComponent, ItemWieldedEvent>)OnAssistedReloadWeaponWielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AssistedReloadWeaponComponent, ItemUnwieldedEvent>((EntityEventRefHandler<AssistedReloadWeaponComponent, ItemUnwieldedEvent>)OnAssistedReloadWeaponUnwielded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunDualWieldingComponent, GotEquippedHandEvent>((EntityEventRefHandler<GunDualWieldingComponent, GotEquippedHandEvent>)OnDualWieldingEquippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunDualWieldingComponent, GotUnequippedHandEvent>((EntityEventRefHandler<GunDualWieldingComponent, GotUnequippedHandEvent>)OnDualWieldingUnequippedHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunDualWieldingComponent, GunRefreshModifiersEvent>((EntityEventRefHandler<GunDualWieldingComponent, GunRefreshModifiersEvent>)OnDualWieldingRefreshModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunDualWieldingComponent, GetWeaponAccuracyEvent>((EntityEventRefHandler<GunDualWieldingComponent, GetWeaponAccuracyEvent>)OnDualWieldingGetWeaponAccuracy, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<RequestStopShootEvent>((EntitySessionEventHandler<RequestStopShootEvent>)OnDualWieldingStopShoot, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<UnremoveableComponent, RMCItemDropAttemptEvent>((EntityEventRefHandler<UnremoveableComponent, RMCItemDropAttemptEvent>)OnUnremoveableDropAttempt, (Type[])null, (Type[])null);
	}

	private void OnShootAtFixedPointShot(Entity<ShootAtFixedPointComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gun = default(GunComponent);
		if (!((EntitySystem)this).TryComp<GunComponent>(Entity<ShootAtFixedPointComponent>.op_Implicit(ent), ref gun))
		{
			return;
		}
		EntityCoordinates? shootCoordinates = gun.ShootCoordinates;
		if (!shootCoordinates.HasValue)
		{
			return;
		}
		EntityCoordinates target = shootCoordinates.GetValueOrDefault();
		MapCoordinates from = _transform.GetMapCoordinates(Entity<ShootAtFixedPointComponent>.op_Implicit(ent), (TransformComponent)null);
		if (args.FiredProjectiles.Count > 0)
		{
			from = _transform.GetMapCoordinates(args.FiredProjectiles[0], (TransformComponent)null);
		}
		MapCoordinates to = _transform.ToMapCoordinates(target, true);
		if (from.MapId != to.MapId)
		{
			return;
		}
		Vector2 direction = to.Position - from.Position;
		if (direction == Vector2.Zero)
		{
			return;
		}
		float baseDistance = (ent.Comp.MaxFixedRange.HasValue ? Math.Min(ent.Comp.MaxFixedRange.Value, direction.Length()) : direction.Length());
		if (ent.Comp.AutoAimClosestObstacle)
		{
			CollisionRay ray = default(CollisionRay);
			((CollisionRay)(ref ray))._002Ector(from.Position, Vector2Helpers.Normalized(direction), 2);
			RayCastResults? hitResult = default(RayCastResults?);
			if (Extensions.TryFirstOrNull<RayCastResults>(_physics.IntersectRay(from.MapId, ray, baseDistance, (EntityUid?)null, true), ref hitResult) && hitResult.HasValue)
			{
				RayCastResults trueHit = hitResult.GetValueOrDefault();
				baseDistance = ((RayCastResults)(ref trueHit)).Distance;
			}
		}
		TimeSpan time = _timing.CurTime;
		Vector2 normalized = Vector2Helpers.Normalized(direction);
		PhysicsComponent physics = default(PhysicsComponent);
		ProjectileComponent normalProjectile = default(ProjectileComponent);
		foreach (EntityUid projectile in args.FiredProjectiles)
		{
			if (_physicsQuery.TryComp(projectile, ref physics))
			{
				Vector2 projectileDirection = normalized;
				if (args.FiredProjectiles.Count > 1 && physics.LinearVelocity != Vector2.Zero)
				{
					projectileDirection = Vector2Helpers.Normalized(physics.LinearVelocity);
				}
				Vector2 impulse = projectileDirection * gun.ProjectileSpeedModified * physics.Mass;
				_physics.SetLinearVelocity(projectile, Vector2.Zero, true, true, (FixturesComponent)null, physics);
				_physics.ApplyLinearImpulse(projectile, impulse, (FixturesComponent)null, physics);
				_physics.SetBodyStatus(projectile, physics, (BodyStatus)1, true);
				ProjectileFixedDistanceComponent comp = ((EntitySystem)this).EnsureComp<ProjectileFixedDistanceComponent>(projectile);
				BeforeArcEvent ev = default(BeforeArcEvent);
				((EntitySystem)this).RaiseLocalEvent<BeforeArcEvent>(projectile, ref ev, false);
				if (((EntitySystem)this).Comp<ShootAtFixedPointComponent>(Entity<ShootAtFixedPointComponent>.op_Implicit(ent)).ShootArcProj && !ev.Cancelled)
				{
					comp.ArcProj = true;
				}
				float distance = baseDistance;
				if (((EntitySystem)this).TryComp<ProjectileComponent>(projectile, ref normalProjectile) && normalProjectile.MaxFixedRange > 0f)
				{
					distance = ((distance > 0f) ? Math.Min(normalProjectile.MaxFixedRange.Value, distance) : normalProjectile.MaxFixedRange.Value);
				}
				comp.TargetCoordinates = new MapCoordinates(from.Position + projectileDirection * distance, from.MapId);
				comp.FlyEndTime = time + TimeSpan.FromSeconds(distance / gun.ProjectileSpeedModified);
			}
		}
	}

	private void OnCollisionCheckArc(Entity<ProjectileFixedDistanceComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.ArcProj && (args.OtherFixture.CollisionLayer & _blockArcCollisionGroup) == 0)
		{
			args.Cancelled = true;
		}
	}

	private void OnEventToStopProjectile<T>(Entity<ProjectileFixedDistanceComponent> ent, ref T args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		StopProjectile(ent);
	}

	private void OnWeaponDamageFalloffRefreshModifiers(Entity<RMCWeaponDamageFalloffComponent> weapon, ref GunRefreshModifiersEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		GetDamageFalloffEvent ev = new GetDamageFalloffEvent(weapon.Comp.FalloffMultiplier, weapon.Comp.RangeFlat);
		((EntitySystem)this).RaiseLocalEvent<GetDamageFalloffEvent>(weapon.Owner, ref ev, false);
		weapon.Comp.ModifiedFalloffMultiplier = FixedPoint2.Max(ev.FalloffMultiplier, 0);
		weapon.Comp.RangeFlatModified = ev.Range;
		((EntitySystem)this).Dirty<RMCWeaponDamageFalloffComponent>(weapon, (MetaDataComponent)null);
	}

	private void OnWeaponDamageFalloffShot(Entity<RMCWeaponDamageFalloffComponent> weapon, ref AmmoShotEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		RMCProjectileDamageFalloffComponent falloffComponent = default(RMCProjectileDamageFalloffComponent);
		foreach (EntityUid projectile in args.FiredProjectiles)
		{
			if (((EntitySystem)this).TryComp<RMCProjectileDamageFalloffComponent>(projectile, ref falloffComponent))
			{
				_rmcProjectileSystem.SetProjectileFalloffWeaponMult(Entity<RMCProjectileDamageFalloffComponent>.op_Implicit((projectile, falloffComponent)), weapon.Comp.ModifiedFalloffMultiplier, weapon.Comp.RangeFlatModified);
			}
		}
	}

	private void OnExtraProjectilesShot(Entity<RMCExtraProjectilesDamageModsComponent> weapon, ref AmmoShotEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		ProjectileComponent projectileComponent = default(ProjectileComponent);
		for (int t = 1; t < args.FiredProjectiles.Count; t++)
		{
			if (((EntitySystem)this).TryComp<ProjectileComponent>(args.FiredProjectiles[t], ref projectileComponent))
			{
				projectileComponent.Damage *= weapon.Comp.DamageMultiplier;
			}
		}
	}

	private void OnWeaponAccuracyExamined(Entity<RMCWeaponAccuracyComponent> weapon, ref ExaminedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<GunComponent>(weapon.Owner))
		{
			return;
		}
		using (args.PushGroup("RMCWeaponAccuracyComponent"))
		{
			args.PushMarkup(base.Loc.GetString("rmc-examine-text-weapon-accuracy", (ValueTuple<string, object>)("colour", "yellow"), (ValueTuple<string, object>)("accuracy", weapon.Comp.ModifiedAccuracyMultiplier)));
		}
	}

	private void OnWeaponAccuracyRefreshModifiers(Entity<RMCWeaponAccuracyComponent> weapon, ref GunRefreshModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		FixedPoint2 baseMult = weapon.Comp.AccuracyMultiplierUnwielded;
		WieldableComponent wieldableComponent = default(WieldableComponent);
		if (((EntitySystem)this).TryComp<WieldableComponent>(weapon.Owner, ref wieldableComponent) && wieldableComponent.Wielded)
		{
			baseMult = weapon.Comp.AccuracyMultiplier;
		}
		GetWeaponAccuracyEvent ev = new GetWeaponAccuracyEvent(baseMult, weapon.Comp.RangeFlat);
		((EntitySystem)this).RaiseLocalEvent<GetWeaponAccuracyEvent>(weapon.Owner, ref ev, false);
		weapon.Comp.ModifiedAccuracyMultiplier = Math.Max(0.1, (double)ev.AccuracyMultiplier);
		weapon.Comp.RangeFlatModified = ev.Range;
		((EntitySystem)this).Dirty<RMCWeaponAccuracyComponent>(weapon, (MetaDataComponent)null);
	}

	private void OnWeaponAccuracyShot(Entity<RMCWeaponAccuracyComponent> weapon, ref AmmoShotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		int netId = ((EntitySystem)this).GetNetEntity(weapon.Owner, (MetaDataComponent)null).Id;
		FixedPoint2 orderAccuracy = 0;
		FixedPoint2 orderAccuracyPerTile = 0;
		TransformComponent transformComponent = default(TransformComponent);
		if (((EntitySystem)this).TryComp(weapon.Owner, ref transformComponent))
		{
			EntityUid parentUid = transformComponent.ParentUid;
			FocusOrderComponent orderComponent = default(FocusOrderComponent);
			if (((EntityUid)(ref parentUid)).Valid && ((EntitySystem)this).TryComp<FocusOrderComponent>(transformComponent.ParentUid, ref orderComponent) && orderComponent.Received.Count != 0)
			{
				orderAccuracy = orderComponent.Received[0].Multiplier * orderComponent.AccuracyModifier;
				orderAccuracyPerTile = orderComponent.Received[0].Multiplier * orderComponent.AccuracyPerTileModifier;
			}
		}
		RMCProjectileAccuracyComponent accuracyComponent = default(RMCProjectileAccuracyComponent);
		for (int t = 0; t < args.FiredProjectiles.Count; t++)
		{
			if (((EntitySystem)this).TryComp<RMCProjectileAccuracyComponent>(args.FiredProjectiles[t], ref accuracyComponent))
			{
				accuracyComponent.Accuracy *= weapon.Comp.ModifiedAccuracyMultiplier;
				accuracyComponent.Accuracy += orderAccuracy;
				for (int count = 0; accuracyComponent.Thresholds.Count > count; count++)
				{
					AccuracyFalloffThreshold threshold = accuracyComponent.Thresholds[count];
					accuracyComponent.Thresholds[count] = threshold with
					{
						Range = threshold.Range + weapon.Comp.RangeFlatModified
					};
				}
				if (orderAccuracyPerTile != 0)
				{
					accuracyComponent.Thresholds.Add(new AccuracyFalloffThreshold(0f, -orderAccuracyPerTile, Buildup: false));
				}
				accuracyComponent.GunSeed = ((long)t << 32) | (uint)netId;
				((EntitySystem)this).Dirty<RMCProjectileAccuracyComponent>(Entity<RMCProjectileAccuracyComponent>.op_Implicit((args.FiredProjectiles[t], accuracyComponent)), (MetaDataComponent)null);
			}
		}
	}

	private void OnShowUseDelayShot(Entity<GunShowUseDelayComponent> ent, ref GunShotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateDelay(ent);
	}

	private void OnShowUseDelayWielded(Entity<GunShowUseDelayComponent> ent, ref ItemWieldedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateDelay(ent);
	}

	private void OnGunUserWhitelistAttemptShoot(Entity<GunUserWhitelistComponent> ent, ref AttemptShootEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !((EntitySystem)this).HasComp<BypassInteractionChecksComponent>(args.User) && !_whitelist.IsValid(ent.Comp.Whitelist, args.User))
		{
			args.Cancelled = true;
			string popup = base.Loc.GetString("cm-gun-unskilled", (ValueTuple<string, object>)("gun", ent.Owner));
			_popup.PopupClient(popup, args.User, args.User, PopupType.SmallCaution);
		}
	}

	private void OnGunUnskilledPenaltyRefresh(Entity<GunUnskilledPenaltyComponent> ent, ref GunRefreshModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetUserSkills(Entity<GunUnskilledPenaltyComponent>.op_Implicit(ent), out Entity<SkillsComponent> user) || !_skills.HasSkill(Entity<SkillsComponent>.op_Implicit((Entity<SkillsComponent>.op_Implicit(user), Entity<SkillsComponent>.op_Implicit(user))), ent.Comp.Skill, ent.Comp.Firearms))
		{
			args.MinAngle += ent.Comp.AngleIncrease;
			args.MaxAngle += ent.Comp.AngleIncrease;
		}
	}

	private void OnGunUnskilledPenaltyGetWeaponAccuracy(Entity<GunUnskilledPenaltyComponent> ent, ref GetWeaponAccuracyEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetUserSkills(Entity<GunUnskilledPenaltyComponent>.op_Implicit(ent), out Entity<SkillsComponent> user) || !_skills.HasSkill(Entity<SkillsComponent>.op_Implicit((Entity<SkillsComponent>.op_Implicit(user), Entity<SkillsComponent>.op_Implicit(user))), ent.Comp.Skill, ent.Comp.Firearms))
		{
			args.AccuracyMultiplier += ent.Comp.AccuracyAddMult;
		}
	}

	private void OnGunDamageModifierMapInit(Entity<GunDamageModifierComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		RefreshGunDamageMultiplier(Entity<GunDamageModifierComponent>.op_Implicit((ent.Owner, ent.Comp)));
	}

	private void OnGunDamageModifierAmmoShot(Entity<GunDamageModifierComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		ProjectileComponent comp = default(ProjectileComponent);
		foreach (EntityUid projectile in args.FiredProjectiles)
		{
			if (_projectileQuery.TryGetComponent(projectile, ref comp))
			{
				comp.Damage *= ent.Comp.ModifiedMultiplier;
			}
		}
	}

	private void OnGunPointBlankMeleeHit(Entity<GunPointBlankComponent> gun, ref MeleeHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		MeleeWeaponComponent melee = default(MeleeWeaponComponent);
		if (((EntitySystem)this).TryComp<MeleeWeaponComponent>(Entity<GunPointBlankComponent>.op_Implicit(gun), ref melee) && TryGetGunUser(gun.Owner, out Entity<HandsComponent> user))
		{
			((EntitySystem)this).EnsureComp<UserPointblankCooldownComponent>(Entity<HandsComponent>.op_Implicit(user)).LastPBAt = _timing.CurTime;
		}
	}

	private void OnGunPointBlankAmmoShot(Entity<GunPointBlankComponent> gun, ref AmmoShotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gunComp = default(GunComponent);
		if (!((EntitySystem)this).TryComp<GunComponent>(gun.Owner, ref gunComp) || !gunComp.Target.HasValue)
		{
			return;
		}
		EntityUid target = gunComp.Target.Value;
		if (!((EntitySystem)this).HasComp<TransformComponent>(target) || !((EntitySystem)this).HasComp<EvasionComponent>(target) || !TryGetGunUser(gun.Owner, out Entity<HandsComponent> user))
		{
			return;
		}
		GetIFFFactionEvent shooterFactionEvent = new GetIFFFactionEvent(SlotFlags.IDCARD, new HashSet<EntProtoId<IFFFactionComponent>>());
		((EntitySystem)this).RaiseLocalEvent<GetIFFFactionEvent>(user.Owner, ref shooterFactionEvent, false);
		GetIFFFactionEvent targetFactionEvent = new GetIFFFactionEvent(SlotFlags.IDCARD, new HashSet<EntProtoId<IFFFactionComponent>>());
		((EntitySystem)this).RaiseLocalEvent<GetIFFFactionEvent>(target, ref targetFactionEvent, false);
		if (shooterFactionEvent.Factions.Count != 0 && targetFactionEvent.Factions.Count != 0 && shooterFactionEvent.Factions.Overlaps(targetFactionEvent.Factions) && ((EntitySystem)this).HasComp<EntityActiveInvisibleComponent>(target))
		{
			return;
		}
		UserPointblankCooldownComponent userDelay = ((EntitySystem)this).EnsureComp<UserPointblankCooldownComponent>(Entity<HandsComponent>.op_Implicit(user));
		if (_timing.CurTime < userDelay.LastPBAt + userDelay.TimeBetweenPBs)
		{
			return;
		}
		ActorComponent obj = ((EntitySystem)this).CompOrNull<ActorComponent>(Entity<HandsComponent>.op_Implicit(user));
		ICommonSession session = ((obj != null) ? obj.PlayerSession : null);
		if ((gunComp.Target.Value == user.Owner && (gunComp.SelectedMode == SelectiveFire.FullAuto || (session != null && !_netConfig.GetClientCVar<bool>(session.Channel, RMCCVars.RMCDamageYourself)))) || !_interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(gun.Owner), Entity<TransformComponent>.op_Implicit(gunComp.Target.Value), gun.Comp.Range, CollisionGroup.Impassable | CollisionGroup.InteractImpassable, null, popup: false, overlapCheck: true, Entity<HandsComponent>.op_Implicit(user)))
		{
			return;
		}
		ProjectileComponent projectileComp = default(ProjectileComponent);
		PhysicsComponent physicsComp = default(PhysicsComponent);
		foreach (EntityUid projectile in args.FiredProjectiles)
		{
			if (((EntitySystem)this).TryComp<ProjectileComponent>(projectile, ref projectileComp) && ((EntitySystem)this).TryComp<PhysicsComponent>(projectile, ref physicsComp) && _rmcLagCompensation.IsWithinMargin(Entity<TransformComponent>.op_Implicit(projectile), Entity<TransformComponent>.op_Implicit(gunComp.Target.Value), session, gun.Comp.Range))
			{
				if (_standing.IsDown(gunComp.Target.Value))
				{
					projectileComp.Damage *= gun.Comp.ProneDamageMult;
					((EntitySystem)this).Dirty(projectile, (IComponent)(object)projectileComp, (MetaDataComponent)null);
				}
				_projectile.ProjectileCollide(Entity<ProjectileComponent, PhysicsComponent>.op_Implicit((projectile, projectileComp, physicsComp)), gunComp.Target.Value);
			}
		}
		userDelay.LastPBAt = _timing.CurTime;
		((EntitySystem)this).Dirty(Entity<HandsComponent>.op_Implicit(user), (IComponent)(object)userDelay, (MetaDataComponent)null);
		MeleeWeaponComponent melee = default(MeleeWeaponComponent);
		if (((EntitySystem)this).TryComp<MeleeWeaponComponent>(Entity<GunPointBlankComponent>.op_Implicit(gun), ref melee))
		{
			melee.NextAttack = userDelay.LastPBAt + userDelay.TimeBetweenPBs;
			((EntitySystem)this).Dirty(Entity<GunPointBlankComponent>.op_Implicit(gun), (IComponent)(object)melee, (MetaDataComponent)null);
		}
	}

	private void OnRecoilSkilledRefreshModifiers(Entity<GunSkilledRecoilComponent> ent, ref GunRefreshModifiersEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetUserSkills(Entity<GunSkilledRecoilComponent>.op_Implicit(ent), out Entity<SkillsComponent> user) || !_skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit((Entity<SkillsComponent>.op_Implicit(user), Entity<SkillsComponent>.op_Implicit(user))), ent.Comp.Skills))
		{
			return;
		}
		if (ent.Comp.MustBeWielded)
		{
			WieldableComponent wieldableComponent = ((EntitySystem)this).CompOrNull<WieldableComponent>(Entity<GunSkilledRecoilComponent>.op_Implicit(ent));
			if (wieldableComponent == null || !wieldableComponent.Wielded)
			{
				return;
			}
		}
		args.CameraRecoilScalar = 0f;
	}

	private void OnAccuracySkilledGetWeaponAccuracy(Entity<GunSkilledAccuracyComponent> gun, ref GetWeaponAccuracyEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetUserSkills(Entity<GunSkilledAccuracyComponent>.op_Implicit(gun), out Entity<SkillsComponent> user))
		{
			args.AccuracyMultiplier += gun.Comp.AccuracyAddMult * _skills.GetSkill(Entity<SkillsComponent>.op_Implicit((Entity<SkillsComponent>.op_Implicit(user), Entity<SkillsComponent>.op_Implicit(user))), gun.Comp.Skill);
		}
	}

	private void OnRequiresSkillsAttemptShoot(Entity<GunRequiresSkillsComponent> ent, ref AttemptShootEvent args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !_skills.HasAllSkills(Entity<SkillsComponent>.op_Implicit(args.User), ent.Comp.Skills))
		{
			args.Cancelled = true;
			string popup = base.Loc.GetString("cm-gun-unskilled", (ValueTuple<string, object>)("gun", ent.Owner));
			_popup.PopupClient(popup, args.User, args.User, PopupType.SmallCaution);
		}
	}

	private void OnRequireEquippedAttemptShoot(Entity<GunRequireEquippedComponent> ent, ref AttemptShootEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && !HasRequiredEquippedPopup(Entity<GunRequireEquippedComponent>.op_Implicit((Entity<GunRequireEquippedComponent>.op_Implicit(ent), Entity<GunRequireEquippedComponent>.op_Implicit(ent))), args.User))
		{
			args.Cancelled = true;
		}
	}

	private void StopProjectile(Entity<ProjectileFixedDistanceComponent> projectile)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent physics = default(PhysicsComponent);
		if (_physicsQuery.TryGetComponent(Entity<ProjectileFixedDistanceComponent>.op_Implicit(projectile), ref physics))
		{
			_physics.SetLinearVelocity(Entity<ProjectileFixedDistanceComponent>.op_Implicit(projectile), Vector2.Zero, true, true, (FixturesComponent)null, physics);
			_physics.SetBodyStatus(Entity<ProjectileFixedDistanceComponent>.op_Implicit(projectile), physics, (BodyStatus)0, true);
			if (physics.Awake)
			{
				_broadphase.RegenerateContacts(Entity<ProjectileFixedDistanceComponent>.op_Implicit(projectile), physics, (FixturesComponent)null, (TransformComponent)null);
			}
		}
	}

	private void UpdateDelay(Entity<GunShowUseDelayComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gun = default(GunComponent);
		if (((EntitySystem)this).TryComp<GunComponent>(Entity<GunShowUseDelayComponent>.op_Implicit(ent), ref gun))
		{
			TimeSpan remaining = gun.NextFire - _timing.CurTime;
			if (!(remaining <= TimeSpan.Zero))
			{
				UseDelayComponent useDelay = ((EntitySystem)this).EnsureComp<UseDelayComponent>(Entity<GunShowUseDelayComponent>.op_Implicit(ent));
				_useDelay.SetLength(Entity<UseDelayComponent>.op_Implicit((Entity<GunShowUseDelayComponent>.op_Implicit(ent), useDelay)), remaining, ent.Comp.DelayId);
				_useDelay.TryResetDelay(Entity<UseDelayComponent>.op_Implicit((Entity<GunShowUseDelayComponent>.op_Implicit(ent), useDelay)), checkDelayed: false, ent.Comp.DelayId);
			}
		}
	}

	private void TryRefreshGunModifiers<TComp, TEvent>(Entity<TComp> ent, ref TEvent args) where TComp : IComponent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		GunComponent gun = default(GunComponent);
		if (((EntitySystem)this).TryComp<GunComponent>(Entity<TComp>.op_Implicit(ent), ref gun))
		{
			_gun.RefreshModifiers(Entity<GunComponent>.op_Implicit((Entity<TComp>.op_Implicit(ent), gun)));
		}
	}

	private bool TryGetUserSkills(EntityUid gun, out Entity<SkillsComponent> user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		user = default(Entity<SkillsComponent>);
		SkillsComponent skills = default(SkillsComponent);
		if (!TryGetGunUser(gun, out Entity<HandsComponent> gunUser) || !((EntitySystem)this).TryComp<SkillsComponent>(Entity<HandsComponent>.op_Implicit(gunUser), ref skills))
		{
			return false;
		}
		user = Entity<SkillsComponent>.op_Implicit((Entity<HandsComponent>.op_Implicit(gunUser), skills));
		return true;
	}

	public void RefreshGunDamageMultiplier(Entity<GunDamageModifierComponent?> gun)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		gun.Comp = ((EntitySystem)this).EnsureComp<GunDamageModifierComponent>(Entity<GunDamageModifierComponent>.op_Implicit(gun));
		GetGunDamageModifierEvent ev = new GetGunDamageModifierEvent(gun.Comp.Multiplier);
		((EntitySystem)this).RaiseLocalEvent<GetGunDamageModifierEvent>(Entity<GunDamageModifierComponent>.op_Implicit(gun), ref ev, false);
		gun.Comp.ModifiedMultiplier = ev.Multiplier;
		((EntitySystem)this).Dirty<GunDamageModifierComponent>(gun, (MetaDataComponent)null);
	}

	public bool HasRequiredEquippedPopup(Entity<GunRequireEquippedComponent?> gun, EntityUid user)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<GunRequireEquippedComponent>(Entity<GunRequireEquippedComponent>.op_Implicit(gun), ref gun.Comp, false))
		{
			return true;
		}
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(user), SlotFlags.OUTERCLOTHING);
		ContainerSlot slot;
		while (slots.MoveNext(out slot))
		{
			if (_whitelist.IsValid(gun.Comp.Whitelist, slot.ContainedEntity))
			{
				return true;
			}
		}
		_popup.PopupClient(base.Loc.GetString("rmc-shoot-harness-required"), user, user, PopupType.MediumCaution);
		return false;
	}

	public override void Update(float frameTime)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<ProjectileFixedDistanceComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ProjectileFixedDistanceComponent>();
		EntityUid uid = default(EntityUid);
		ProjectileFixedDistanceComponent comp = default(ProjectileFixedDistanceComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (!(time < comp.FlyEndTime))
			{
				MapCoordinates? targetCoordinates = comp.TargetCoordinates;
				if (targetCoordinates.HasValue)
				{
					MapCoordinates targetCoords = targetCoordinates.GetValueOrDefault();
					_transform.SetMapCoordinates(uid, targetCoords);
				}
				StopProjectile(Entity<ProjectileFixedDistanceComponent>.op_Implicit((uid, comp)));
				((EntitySystem)this).RemCompDeferred<ProjectileFixedDistanceComponent>(uid);
				ProjectileFixedDistanceStopEvent ev = default(ProjectileFixedDistanceStopEvent);
				((EntitySystem)this).RaiseLocalEvent<ProjectileFixedDistanceStopEvent>(uid, ref ev, false);
				if (_net.IsClient && ((EntitySystem)this).IsClientSide(uid, (MetaDataComponent)null) && ((EntitySystem)this).HasComp<DeleteOnFixedDistanceStopComponent>(uid))
				{
					((EntitySystem)this).QueueDel((EntityUid?)uid);
				}
			}
		}
	}

	private void OnRevolverUniqueAction(Entity<RevolverAmmoProviderComponent> gun, ref UniqueActionEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			int randomCount = _random.Next(1, gun.Comp.Capacity + 1);
			gun.Comp.CurrentIndex = (gun.Comp.CurrentIndex + randomCount) % gun.Comp.Capacity;
			_audio.PlayPredicted(gun.Comp.SoundSpin, gun.Owner, (EntityUid?)args.UserUid, (AudioParams?)null);
			string popup = base.Loc.GetString("rmc-revolver-spin", (ValueTuple<string, object>)("gun", args.UserUid));
			_popup.PopupClient(popup, args.UserUid, args.UserUid, PopupType.SmallCaution);
			((EntitySystem)this).Dirty<RevolverAmmoProviderComponent>(gun, (MetaDataComponent)null);
		}
	}

	private void OnUserBlockShootingInsideContainersAttemptShoot(Entity<UserBlockShootingInsideContainersComponent> ent, ref ShotAttemptedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Cancelled && _container.IsEntityInContainer(Entity<UserBlockShootingInsideContainersComponent>.op_Implicit(ent), (MetaDataComponent)null))
		{
			args.Cancel();
		}
	}

	private void OnAmmoEjectActivateInWorld(Entity<RMCAmmoEjectComponent> gun, ref ActivateInWorldEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (((HandledEntityEventArgs)args).Handled || !_container.TryGetContainer(gun.Owner, gun.Comp.ContainerID, ref container, (ContainerManagerComponent)null) || container.ContainedEntities.Count <= 0)
		{
			return;
		}
		string hand = _hands.GetActiveHand(Entity<HandsComponent>.op_Implicit(args.User));
		if (hand == null || !_hands.HandIsEmpty(Entity<HandsComponent>.op_Implicit(args.User), hand) || !_hands.CanPickupToHand(args.User, container.ContainedEntities[0], hand))
		{
			return;
		}
		RMCTryAmmoEjectEvent cancelEvent = new RMCTryAmmoEjectEvent(args.User, Cancelled: false);
		((EntitySystem)this).RaiseLocalEvent<RMCTryAmmoEjectEvent>(gun.Owner, ref cancelEvent, false);
		if (cancelEvent.Cancelled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid ejectedAmmo = container.ContainedEntities[0];
		BallisticAmmoProviderComponent ammoProviderComponent = default(BallisticAmmoProviderComponent);
		if (((EntitySystem)this).TryComp<BallisticAmmoProviderComponent>(gun.Owner, ref ammoProviderComponent))
		{
			TakeAmmoEvent takeAmmoEvent = new TakeAmmoEvent(1, new List<(EntityUid?, IShootable)>(), ((EntitySystem)this).Transform(gun.Owner).Coordinates, args.User);
			((EntitySystem)this).RaiseLocalEvent<TakeAmmoEvent>(gun.Owner, takeAmmoEvent, false);
			if (takeAmmoEvent.Ammo.Count <= 0)
			{
				return;
			}
			EntityUid? ammo = takeAmmoEvent.Ammo[0].Entity;
			if (!ammo.HasValue)
			{
				return;
			}
			ejectedAmmo = ammo.Value;
		}
		if (!((EntitySystem)this).HasComp<ItemSlotsComponent>(gun.Owner) || !_slots.TryEject(gun.Owner, gun.Comp.ContainerID, args.User, out var _, null, excludeUserAudio: true))
		{
			_audio.PlayPredicted(gun.Comp.EjectSound, gun.Owner, (EntityUid?)args.User, (AudioParams?)null);
		}
		_hands.TryPickup(args.User, ejectedAmmo, hand);
	}

	private void OnDualWieldingEquippedHand(Entity<GunDualWieldingComponent> gun, ref GotEquippedHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		RefreshGunHolderModifiers(gun, args.User);
	}

	private void OnDualWieldingUnequippedHand(Entity<GunDualWieldingComponent> gun, ref GotUnequippedHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		RefreshGunHolderModifiers(gun, args.User);
	}

	private void OnDualWieldingRefreshModifiers(Entity<GunDualWieldingComponent> gun, ref GunRefreshModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (gun.Comp.WeaponGroup != GunDualWieldingGroup.None && TryGetGunUser(Entity<GunDualWieldingComponent>.op_Implicit(gun), out Entity<HandsComponent> user) && TryGetOtherDualWieldedGun(Entity<HandsComponent>.op_Implicit(user), gun, out Entity<GunDualWieldingComponent> _))
		{
			args.CameraRecoilScalar += gun.Comp.RecoilModifier;
			args.MinAngle += gun.Comp.ScatterModifier;
			args.MaxAngle += gun.Comp.ScatterModifier;
		}
	}

	private void OnDualWieldingGetWeaponAccuracy(Entity<GunDualWieldingComponent> gun, ref GetWeaponAccuracyEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (gun.Comp.WeaponGroup != GunDualWieldingGroup.None && TryGetGunUser(Entity<GunDualWieldingComponent>.op_Implicit(gun), out Entity<HandsComponent> user) && TryGetOtherDualWieldedGun(Entity<HandsComponent>.op_Implicit(user), gun, out Entity<GunDualWieldingComponent> _))
		{
			args.AccuracyMultiplier += gun.Comp.AccuracyAddMult;
		}
	}

	private void OnDualWieldingStopShoot(RequestStopShootEvent ev, EntitySessionEventArgs args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid user = attachedEntity.GetValueOrDefault();
			EntityUid gunUid = ((EntitySystem)this).GetEntity(ev.Gun);
			GunComponent gun = default(GunComponent);
			GunComponent offGun = default(GunComponent);
			if (((EntitySystem)this).TryComp<GunComponent>(gunUid, ref gun) && TryGetAkimboOffHand(user, Entity<GunComponent>.op_Implicit((gunUid, gun)), out var offHand) && ((EntitySystem)this).TryComp<GunComponent>(offHand, ref offGun))
			{
				_gun.StopShooting(offHand, offGun);
			}
		}
	}

	public bool TryGetAkimboOffHand(EntityUid user, Entity<GunComponent> activeGun, out EntityUid offHand)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		offHand = default(EntityUid);
		GunDualWieldingComponent dual = default(GunDualWieldingComponent);
		if (!((EntitySystem)this).TryComp<GunDualWieldingComponent>(Entity<GunComponent>.op_Implicit(activeGun), ref dual) || !dual.Akimbo || dual.WeaponGroup == GunDualWieldingGroup.None)
		{
			return false;
		}
		if (!TryGetOtherDualWieldedGun(user, Entity<GunDualWieldingComponent>.op_Implicit((Entity<GunComponent>.op_Implicit(activeGun), dual)), out Entity<GunDualWieldingComponent> other) || !other.Comp.Akimbo)
		{
			return false;
		}
		offHand = other.Owner;
		return true;
	}

	private void OnUnremoveableDropAttempt(Entity<UnremoveableComponent> ent, ref RMCItemDropAttemptEvent args)
	{
		args.Cancelled = true;
	}

	private bool TryGetOtherDualWieldedGun(EntityUid user, Entity<GunDualWieldingComponent> gun, out Entity<GunDualWieldingComponent> otherGun)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		otherGun = default(Entity<GunDualWieldingComponent>);
		HandsComponent handsComp = default(HandsComponent);
		if (!((EntitySystem)this).TryComp<HandsComponent>(user, ref handsComp))
		{
			return false;
		}
		GunDualWieldingComponent dualWieldingComp = default(GunDualWieldingComponent);
		foreach (string hand in handsComp.Hands.Keys)
		{
			EntityUid? heldItem = _hands.GetHeldItem(Entity<HandsComponent>.op_Implicit(user), hand);
			if (heldItem.HasValue)
			{
				EntityUid held = heldItem.GetValueOrDefault();
				if (held != gun.Owner && ((EntitySystem)this).TryComp<GunDualWieldingComponent>(held, ref dualWieldingComp) && dualWieldingComp.WeaponGroup == gun.Comp.WeaponGroup)
				{
					otherGun = Entity<GunDualWieldingComponent>.op_Implicit((held, dualWieldingComp));
					return true;
				}
			}
		}
		return false;
	}

	public bool TryGetGunUser(EntityUid gun, out Entity<HandsComponent> user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		HandsComponent operatorHands = default(HandsComponent);
		if (_container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((ValueTuple<EntityUid, TransformComponent>)(gun, null)), ref container) && _vehicleWeapons.TryGetOperatorForSelectedWeapon(container.Owner, gun, out var operatorUid) && ((EntitySystem)this).TryComp<HandsComponent>(operatorUid, ref operatorHands))
		{
			user = Entity<HandsComponent>.op_Implicit((operatorUid, operatorHands));
			return true;
		}
		HandsComponent hands = default(HandsComponent);
		if (container != null && ((EntitySystem)this).TryComp<HandsComponent>(container.Owner, ref hands))
		{
			user = Entity<HandsComponent>.op_Implicit((container.Owner, hands));
			return true;
		}
		AttachableHolderComponent holder = default(AttachableHolderComponent);
		if (container != null && ((EntitySystem)this).TryComp<AttachableHolderComponent>(container.Owner, ref holder))
		{
			EntityUid? supercedingAttachable = holder.SupercedingAttachable;
			if (supercedingAttachable.HasValue && supercedingAttachable.GetValueOrDefault() == gun)
			{
				return TryGetGunUser(container.Owner, out user);
			}
		}
		user = default(Entity<HandsComponent>);
		return false;
	}

	private void RefreshGunHolderModifiers(Entity<GunDualWieldingComponent> gun, EntityUid user)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		_gun.RefreshModifiers(Entity<GunComponent>.op_Implicit(gun.Owner));
		if (TryGetOtherDualWieldedGun(user, gun, out Entity<GunDualWieldingComponent> otherGun))
		{
			_gun.RefreshModifiers(Entity<GunComponent>.op_Implicit(otherGun.Owner));
		}
	}

	private void OnAssistedReloadAmmoAfterInteract(Entity<AssistedReloadAmmoComponent> ent, ref AfterInteractEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanReach && args.Target.HasValue)
		{
			TryAssistedReload(args.User, args.Target.Value, ent);
		}
	}

	private bool IsBehindTarget(EntityUid user, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Angle localRotation = ((EntitySystem)this).Transform(target).LocalRotation;
		Angle behindAngle = DirectionExtensions.ToAngle(DirectionExtensions.GetOpposite(((Angle)(ref localRotation)).GetCardinalDir()));
		MapCoordinates mapCoordinates = _transform.GetMapCoordinates(user, (TransformComponent)null);
		MapCoordinates targetMapPos = _transform.GetMapCoordinates(target, (TransformComponent)null);
		Angle currentAngle = DirectionExtensions.ToWorldAngle(mapCoordinates.Position - targetMapPos.Position);
		double differenceFromBehindAngle = (((Angle)(ref behindAngle)).Degrees - ((Angle)(ref currentAngle)).Degrees + 180.0 + 360.0) % 360.0 - 180.0;
		if (differenceFromBehindAngle > -45.0 && differenceFromBehindAngle < 45.0)
		{
			return true;
		}
		return false;
	}

	private void TryAssistedReload(EntityUid user, EntityUid target, Entity<AssistedReloadAmmoComponent> ammo)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		AssistedReloadReceiverComponent reloadReceiver = default(AssistedReloadReceiverComponent);
		BallisticAmmoProviderComponent ballisticAmmoProvider = default(BallisticAmmoProviderComponent);
		if (((EntitySystem)this).TryComp<AssistedReloadReceiverComponent>(target, ref reloadReceiver) && reloadReceiver.Weapon.HasValue && ((EntitySystem)this).TryComp<BallisticAmmoProviderComponent>(reloadReceiver.Weapon, ref ballisticAmmoProvider))
		{
			if (_whitelist.IsWhitelistFailOrNull(ballisticAmmoProvider.Whitelist, ammo.Owner))
			{
				string failMismatchPopup = base.Loc.GetString("rmc-assisted-reload-fail-mismatch", (ValueTuple<string, object>)("ammo", ammo.Owner), (ValueTuple<string, object>)("weapon", reloadReceiver.Weapon));
				_popup.PopupClient(failMismatchPopup, user, user, PopupType.SmallCaution);
				return;
			}
			if (!IsBehindTarget(user, target))
			{
				string failAnglePopup = base.Loc.GetString("rmc-assisted-reload-fail-angle", (ValueTuple<string, object>)("target", target));
				_popup.PopupClient(failAnglePopup, user, user, PopupType.SmallCaution);
				return;
			}
			if (!_gun.TryAmmoInsert(reloadReceiver.Weapon.Value, ballisticAmmoProvider, ammo.Owner, user, reloadReceiver.Weapon.Value, ammo.Comp.InsertDelay))
			{
				string failFullPopup = base.Loc.GetString("rmc-assisted-reload-fail-full", (ValueTuple<string, object>)("target", target), (ValueTuple<string, object>)("weapon", reloadReceiver.Weapon));
				_popup.PopupClient(failFullPopup, user, user, PopupType.SmallCaution);
				return;
			}
			string userPopup = base.Loc.GetString("rmc-assisted-reload-start-user", (ValueTuple<string, object>)("target", target), (ValueTuple<string, object>)("weapon", reloadReceiver.Weapon));
			string targetPopup = base.Loc.GetString("rmc-assisted-reload-start-target", new(string, object)[3]
			{
				("reloader", user),
				("weapon", reloadReceiver.Weapon),
				("ammo", ammo.Owner)
			});
			_popup.PopupClient(userPopup, user, user);
			_popup.PopupEntity(targetPopup, target, target);
		}
	}

	private void OnAssistedReloadWeaponWielded(Entity<AssistedReloadWeaponComponent> ent, ref ItemWieldedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetGunUser(ent.Owner, out Entity<HandsComponent> wielder))
		{
			((EntitySystem)this).EnsureComp<AssistedReloadReceiverComponent>(Entity<HandsComponent>.op_Implicit(wielder)).Weapon = ent.Owner;
		}
	}

	private void OnAssistedReloadWeaponUnwielded(Entity<AssistedReloadWeaponComponent> ent, ref ItemUnwieldedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetGunUser(ent.Owner, out Entity<HandsComponent> wielder))
		{
			((EntitySystem)this).RemCompDeferred<AssistedReloadReceiverComponent>(Entity<HandsComponent>.op_Implicit(wielder));
		}
	}

	private void OnBeforeArc(Entity<IgnoreArcComponent> ent, ref BeforeArcEvent args)
	{
		args.Cancelled = true;
	}
}
