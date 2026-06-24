// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.CMGunSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
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
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;
  private Robust.Shared.GameObjects.EntityQuery<ProjectileComponent> _projectileQuery;
  private HashSet<Entity<FixturesComponent>> _intersectedEntities = new HashSet<Entity<FixturesComponent>>();
  private HashSet<Entity<FixturesComponent>> _impassableEntities = new HashSet<Entity<FixturesComponent>>();
  private readonly int _blockArcCollisionGroup = 10;
  private const string AccuracyExamineColour = "yellow";

  public override void Initialize()
  {
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this._projectileQuery = this.GetEntityQuery<ProjectileComponent>();
    this.SubscribeLocalEvent<ShootAtFixedPointComponent, AmmoShotEvent>(new EntityEventRefHandler<ShootAtFixedPointComponent, AmmoShotEvent>(this.OnShootAtFixedPointShot));
    this.SubscribeLocalEvent<IgnoreArcComponent, BeforeArcEvent>(new EntityEventRefHandler<IgnoreArcComponent, BeforeArcEvent>(this.OnBeforeArc));
    this.SubscribeLocalEvent<RMCWeaponDamageFalloffComponent, AmmoShotEvent>(new EntityEventRefHandler<RMCWeaponDamageFalloffComponent, AmmoShotEvent>(this.OnWeaponDamageFalloffShot));
    this.SubscribeLocalEvent<RMCWeaponDamageFalloffComponent, GunRefreshModifiersEvent>(new EntityEventRefHandler<RMCWeaponDamageFalloffComponent, GunRefreshModifiersEvent>(this.OnWeaponDamageFalloffRefreshModifiers));
    this.SubscribeLocalEvent<RMCExtraProjectilesDamageModsComponent, AmmoShotEvent>(new EntityEventRefHandler<RMCExtraProjectilesDamageModsComponent, AmmoShotEvent>(this.OnExtraProjectilesShot));
    this.SubscribeLocalEvent<RMCWeaponAccuracyComponent, ExaminedEvent>(new EntityEventRefHandler<RMCWeaponAccuracyComponent, ExaminedEvent>(this.OnWeaponAccuracyExamined));
    this.SubscribeLocalEvent<RMCWeaponAccuracyComponent, GunRefreshModifiersEvent>(new EntityEventRefHandler<RMCWeaponAccuracyComponent, GunRefreshModifiersEvent>(this.OnWeaponAccuracyRefreshModifiers));
    this.SubscribeLocalEvent<RMCWeaponAccuracyComponent, AmmoShotEvent>(new EntityEventRefHandler<RMCWeaponAccuracyComponent, AmmoShotEvent>(this.OnWeaponAccuracyShot));
    this.SubscribeLocalEvent<ProjectileFixedDistanceComponent, PreventCollideEvent>(new EntityEventRefHandler<ProjectileFixedDistanceComponent, PreventCollideEvent>(this.OnCollisionCheckArc));
    this.SubscribeLocalEvent<ProjectileFixedDistanceComponent, PhysicsSleepEvent>(new EntityEventRefHandler<ProjectileFixedDistanceComponent, PhysicsSleepEvent>(this.OnEventToStopProjectile<PhysicsSleepEvent>));
    this.SubscribeLocalEvent<GunShowUseDelayComponent, GunShotEvent>(new EntityEventRefHandler<GunShowUseDelayComponent, GunShotEvent>(this.OnShowUseDelayShot));
    this.SubscribeLocalEvent<GunShowUseDelayComponent, ItemWieldedEvent>(new EntityEventRefHandler<GunShowUseDelayComponent, ItemWieldedEvent>(this.OnShowUseDelayWielded));
    this.SubscribeLocalEvent<GunUserWhitelistComponent, AttemptShootEvent>(new EntityEventRefHandler<GunUserWhitelistComponent, AttemptShootEvent>(this.OnGunUserWhitelistAttemptShoot));
    this.SubscribeLocalEvent<GunUnskilledPenaltyComponent, GotEquippedHandEvent>(new EntityEventRefHandler<GunUnskilledPenaltyComponent, GotEquippedHandEvent>(this.TryRefreshGunModifiers<GunUnskilledPenaltyComponent, GotEquippedHandEvent>));
    this.SubscribeLocalEvent<GunUnskilledPenaltyComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<GunUnskilledPenaltyComponent, GotUnequippedHandEvent>(this.TryRefreshGunModifiers<GunUnskilledPenaltyComponent, GotUnequippedHandEvent>));
    this.SubscribeLocalEvent<GunUnskilledPenaltyComponent, GunRefreshModifiersEvent>(new EntityEventRefHandler<GunUnskilledPenaltyComponent, GunRefreshModifiersEvent>(this.OnGunUnskilledPenaltyRefresh));
    this.SubscribeLocalEvent<GunUnskilledPenaltyComponent, GetWeaponAccuracyEvent>(new EntityEventRefHandler<GunUnskilledPenaltyComponent, GetWeaponAccuracyEvent>(this.OnGunUnskilledPenaltyGetWeaponAccuracy));
    this.SubscribeLocalEvent<GunDamageModifierComponent, AmmoShotEvent>(new EntityEventRefHandler<GunDamageModifierComponent, AmmoShotEvent>(this.OnGunDamageModifierAmmoShot));
    this.SubscribeLocalEvent<GunDamageModifierComponent, MapInitEvent>(new EntityEventRefHandler<GunDamageModifierComponent, MapInitEvent>(this.OnGunDamageModifierMapInit));
    this.SubscribeLocalEvent<GunPointBlankComponent, AmmoShotEvent>(new EntityEventRefHandler<GunPointBlankComponent, AmmoShotEvent>(this.OnGunPointBlankAmmoShot));
    this.SubscribeLocalEvent<GunSkilledRecoilComponent, GotEquippedHandEvent>(new EntityEventRefHandler<GunSkilledRecoilComponent, GotEquippedHandEvent>(this.TryRefreshGunModifiers<GunSkilledRecoilComponent, GotEquippedHandEvent>));
    this.SubscribeLocalEvent<GunSkilledRecoilComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<GunSkilledRecoilComponent, GotUnequippedHandEvent>(this.TryRefreshGunModifiers<GunSkilledRecoilComponent, GotUnequippedHandEvent>));
    this.SubscribeLocalEvent<GunSkilledRecoilComponent, ItemWieldedEvent>(new EntityEventRefHandler<GunSkilledRecoilComponent, ItemWieldedEvent>(this.TryRefreshGunModifiers<GunSkilledRecoilComponent, ItemWieldedEvent>));
    this.SubscribeLocalEvent<GunSkilledRecoilComponent, ItemUnwieldedEvent>(new EntityEventRefHandler<GunSkilledRecoilComponent, ItemUnwieldedEvent>(this.TryRefreshGunModifiers<GunSkilledRecoilComponent, ItemUnwieldedEvent>));
    this.SubscribeLocalEvent<GunSkilledRecoilComponent, GunRefreshModifiersEvent>(new EntityEventRefHandler<GunSkilledRecoilComponent, GunRefreshModifiersEvent>(this.OnRecoilSkilledRefreshModifiers));
    this.SubscribeLocalEvent<GunSkilledAccuracyComponent, GotEquippedHandEvent>(new EntityEventRefHandler<GunSkilledAccuracyComponent, GotEquippedHandEvent>(this.TryRefreshGunModifiers<GunSkilledAccuracyComponent, GotEquippedHandEvent>));
    this.SubscribeLocalEvent<GunSkilledAccuracyComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<GunSkilledAccuracyComponent, GotUnequippedHandEvent>(this.TryRefreshGunModifiers<GunSkilledAccuracyComponent, GotUnequippedHandEvent>));
    this.SubscribeLocalEvent<GunSkilledAccuracyComponent, ItemWieldedEvent>(new EntityEventRefHandler<GunSkilledAccuracyComponent, ItemWieldedEvent>(this.TryRefreshGunModifiers<GunSkilledAccuracyComponent, ItemWieldedEvent>));
    this.SubscribeLocalEvent<GunSkilledAccuracyComponent, ItemUnwieldedEvent>(new EntityEventRefHandler<GunSkilledAccuracyComponent, ItemUnwieldedEvent>(this.TryRefreshGunModifiers<GunSkilledAccuracyComponent, ItemUnwieldedEvent>));
    this.SubscribeLocalEvent<GunSkilledAccuracyComponent, GetWeaponAccuracyEvent>(new EntityEventRefHandler<GunSkilledAccuracyComponent, GetWeaponAccuracyEvent>(this.OnAccuracySkilledGetWeaponAccuracy));
    this.SubscribeLocalEvent<GunRequiresSkillsComponent, AttemptShootEvent>(new EntityEventRefHandler<GunRequiresSkillsComponent, AttemptShootEvent>(this.OnRequiresSkillsAttemptShoot));
    this.SubscribeLocalEvent<GunRequireEquippedComponent, AttemptShootEvent>(new EntityEventRefHandler<GunRequireEquippedComponent, AttemptShootEvent>(this.OnRequireEquippedAttemptShoot));
    this.SubscribeLocalEvent<RevolverAmmoProviderComponent, UniqueActionEvent>(new EntityEventRefHandler<RevolverAmmoProviderComponent, UniqueActionEvent>(this.OnRevolverUniqueAction));
    this.SubscribeLocalEvent<UserBlockShootingInsideContainersComponent, ShotAttemptedEvent>(new EntityEventRefHandler<UserBlockShootingInsideContainersComponent, ShotAttemptedEvent>(this.OnUserBlockShootingInsideContainersAttemptShoot));
    this.SubscribeLocalEvent<RMCAmmoEjectComponent, ActivateInWorldEvent>(new EntityEventRefHandler<RMCAmmoEjectComponent, ActivateInWorldEvent>(this.OnAmmoEjectActivateInWorld));
    this.SubscribeLocalEvent<AssistedReloadAmmoComponent, AfterInteractEvent>(new EntityEventRefHandler<AssistedReloadAmmoComponent, AfterInteractEvent>(this.OnAssistedReloadAmmoAfterInteract));
    this.SubscribeLocalEvent<AssistedReloadWeaponComponent, ItemWieldedEvent>(new EntityEventRefHandler<AssistedReloadWeaponComponent, ItemWieldedEvent>(this.OnAssistedReloadWeaponWielded));
    this.SubscribeLocalEvent<AssistedReloadWeaponComponent, ItemUnwieldedEvent>(new EntityEventRefHandler<AssistedReloadWeaponComponent, ItemUnwieldedEvent>(this.OnAssistedReloadWeaponUnwielded));
    this.SubscribeLocalEvent<GunDualWieldingComponent, GotEquippedHandEvent>(new EntityEventRefHandler<GunDualWieldingComponent, GotEquippedHandEvent>(this.OnDualWieldingEquippedHand));
    this.SubscribeLocalEvent<GunDualWieldingComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<GunDualWieldingComponent, GotUnequippedHandEvent>(this.OnDualWieldingUnequippedHand));
    this.SubscribeLocalEvent<GunDualWieldingComponent, GunRefreshModifiersEvent>(new EntityEventRefHandler<GunDualWieldingComponent, GunRefreshModifiersEvent>(this.OnDualWieldingRefreshModifiers));
    this.SubscribeLocalEvent<GunDualWieldingComponent, GetWeaponAccuracyEvent>(new EntityEventRefHandler<GunDualWieldingComponent, GetWeaponAccuracyEvent>(this.OnDualWieldingGetWeaponAccuracy));
    this.SubscribeAllEvent<RequestStopShootEvent>(new EntitySessionEventHandler<RequestStopShootEvent>(this.OnDualWieldingStopShoot));
    this.SubscribeLocalEvent<UnremoveableComponent, RMCItemDropAttemptEvent>(new EntityEventRefHandler<UnremoveableComponent, RMCItemDropAttemptEvent>(this.OnUnremoveableDropAttempt));
  }

  private void OnShootAtFixedPointShot(
    Entity<ShootAtFixedPointComponent> ent,
    ref AmmoShotEvent args)
  {
    GunComponent comp1;
    if (!this.TryComp<GunComponent>((EntityUid) ent, out comp1))
      return;
    EntityCoordinates? shootCoordinates = comp1.ShootCoordinates;
    if (!shootCoordinates.HasValue)
      return;
    EntityCoordinates valueOrDefault = shootCoordinates.GetValueOrDefault();
    MapCoordinates mapCoordinates1 = this._transform.GetMapCoordinates((EntityUid) ent);
    if (args.FiredProjectiles.Count > 0)
      mapCoordinates1 = this._transform.GetMapCoordinates(args.FiredProjectiles[0]);
    MapCoordinates mapCoordinates2 = this._transform.ToMapCoordinates(valueOrDefault);
    if (mapCoordinates1.MapId != mapCoordinates2.MapId)
      return;
    Vector2 vector2_1 = mapCoordinates2.Position - mapCoordinates1.Position;
    if (vector2_1 == Vector2.Zero)
      return;
    float maxLength = ent.Comp.MaxFixedRange.HasValue ? Math.Min(ent.Comp.MaxFixedRange.Value, vector2_1.Length()) : vector2_1.Length();
    if (ent.Comp.AutoAimClosestObstacle)
    {
      CollisionRay ray = new CollisionRay(mapCoordinates1.Position, Vector2Helpers.Normalized(vector2_1), 2);
      RayCastResults? element;
      if (this._physics.IntersectRay(mapCoordinates1.MapId, ray, maxLength).TryFirstOrNull<RayCastResults>(out element) && element.HasValue)
        maxLength = element.GetValueOrDefault().Distance;
    }
    TimeSpan curTime = this._timing.CurTime;
    Vector2 vector2_2 = Vector2Helpers.Normalized(vector2_1);
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
    {
      PhysicsComponent component;
      if (this._physicsQuery.TryComp(firedProjectile, out component))
      {
        Vector2 vector2_3 = vector2_2;
        if (args.FiredProjectiles.Count > 1 && component.LinearVelocity != Vector2.Zero)
          vector2_3 = Vector2Helpers.Normalized(component.LinearVelocity);
        Vector2 impulse = vector2_3 * comp1.ProjectileSpeedModified * component.Mass;
        this._physics.SetLinearVelocity(firedProjectile, Vector2.Zero, body: component);
        this._physics.ApplyLinearImpulse(firedProjectile, impulse, body: component);
        this._physics.SetBodyStatus(firedProjectile, component, BodyStatus.InAir);
        ProjectileFixedDistanceComponent distanceComponent = this.EnsureComp<ProjectileFixedDistanceComponent>(firedProjectile);
        BeforeArcEvent args1 = new BeforeArcEvent();
        this.RaiseLocalEvent<BeforeArcEvent>(firedProjectile, ref args1);
        if (this.Comp<ShootAtFixedPointComponent>((EntityUid) ent).ShootArcProj && !args1.Cancelled)
          distanceComponent.ArcProj = true;
        float val2 = maxLength;
        ProjectileComponent comp2;
        if (this.TryComp<ProjectileComponent>(firedProjectile, out comp2))
        {
          float? maxFixedRange = comp2.MaxFixedRange;
          float num = 0.0f;
          if ((double) maxFixedRange.GetValueOrDefault() > (double) num & maxFixedRange.HasValue)
            val2 = (double) val2 > 0.0 ? Math.Min(comp2.MaxFixedRange.Value, val2) : comp2.MaxFixedRange.Value;
        }
        distanceComponent.TargetCoordinates = new MapCoordinates?(new MapCoordinates(mapCoordinates1.Position + vector2_3 * val2, mapCoordinates1.MapId));
        distanceComponent.FlyEndTime = curTime + TimeSpan.FromSeconds((double) val2 / (double) comp1.ProjectileSpeedModified);
      }
    }
  }

  private void OnCollisionCheckArc(
    Entity<ProjectileFixedDistanceComponent> ent,
    ref PreventCollideEvent args)
  {
    if (!ent.Comp.ArcProj || (args.OtherFixture.CollisionLayer & this._blockArcCollisionGroup) != 0)
      return;
    args.Cancelled = true;
  }

  private void OnEventToStopProjectile<T>(Entity<ProjectileFixedDistanceComponent> ent, ref T args)
  {
    this.StopProjectile(ent);
  }

  private void OnWeaponDamageFalloffRefreshModifiers(
    Entity<RMCWeaponDamageFalloffComponent> weapon,
    ref GunRefreshModifiersEvent args)
  {
    GetDamageFalloffEvent args1 = new GetDamageFalloffEvent(weapon.Comp.FalloffMultiplier, weapon.Comp.RangeFlat);
    this.RaiseLocalEvent<GetDamageFalloffEvent>(weapon.Owner, ref args1);
    weapon.Comp.ModifiedFalloffMultiplier = FixedPoint2.Max(args1.FalloffMultiplier, (FixedPoint2) 0);
    weapon.Comp.RangeFlatModified = args1.Range;
    this.Dirty<RMCWeaponDamageFalloffComponent>(weapon);
  }

  private void OnWeaponDamageFalloffShot(
    Entity<RMCWeaponDamageFalloffComponent> weapon,
    ref AmmoShotEvent args)
  {
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
    {
      RMCProjectileDamageFalloffComponent comp;
      if (this.TryComp<RMCProjectileDamageFalloffComponent>(firedProjectile, out comp))
        this._rmcProjectileSystem.SetProjectileFalloffWeaponMult((Entity<RMCProjectileDamageFalloffComponent>) (firedProjectile, comp), weapon.Comp.ModifiedFalloffMultiplier, weapon.Comp.RangeFlatModified);
    }
  }

  private void OnExtraProjectilesShot(
    Entity<RMCExtraProjectilesDamageModsComponent> weapon,
    ref AmmoShotEvent args)
  {
    for (int index = 1; index < args.FiredProjectiles.Count; ++index)
    {
      ProjectileComponent comp;
      if (this.TryComp<ProjectileComponent>(args.FiredProjectiles[index], out comp))
        comp.Damage *= weapon.Comp.DamageMultiplier;
    }
  }

  private void OnWeaponAccuracyExamined(
    Entity<RMCWeaponAccuracyComponent> weapon,
    ref ExaminedEvent args)
  {
    if (!this.HasComp<GunComponent>(weapon.Owner))
      return;
    using (args.PushGroup("RMCWeaponAccuracyComponent"))
      args.PushMarkup(this.Loc.GetString("rmc-examine-text-weapon-accuracy", ("colour", (object) "yellow"), ("accuracy", (object) weapon.Comp.ModifiedAccuracyMultiplier)));
  }

  private void OnWeaponAccuracyRefreshModifiers(
    Entity<RMCWeaponAccuracyComponent> weapon,
    ref GunRefreshModifiersEvent args)
  {
    FixedPoint2 AccuracyMultiplier = weapon.Comp.AccuracyMultiplierUnwielded;
    WieldableComponent comp;
    if (this.TryComp<WieldableComponent>(weapon.Owner, out comp) && comp.Wielded)
      AccuracyMultiplier = weapon.Comp.AccuracyMultiplier;
    GetWeaponAccuracyEvent args1 = new GetWeaponAccuracyEvent(AccuracyMultiplier, weapon.Comp.RangeFlat);
    this.RaiseLocalEvent<GetWeaponAccuracyEvent>(weapon.Owner, ref args1);
    weapon.Comp.ModifiedAccuracyMultiplier = (FixedPoint2) Math.Max(0.1, (double) args1.AccuracyMultiplier);
    weapon.Comp.RangeFlatModified = args1.Range;
    this.Dirty<RMCWeaponAccuracyComponent>(weapon);
  }

  private void OnWeaponAccuracyShot(
    Entity<RMCWeaponAccuracyComponent> weapon,
    ref AmmoShotEvent args)
  {
    int id = this.GetNetEntity(weapon.Owner).Id;
    FixedPoint2 fixedPoint2_1 = (FixedPoint2) 0;
    FixedPoint2 fixedPoint2_2 = (FixedPoint2) 0;
    TransformComponent comp1;
    FocusOrderComponent comp2;
    if (this.TryComp(weapon.Owner, out comp1) && comp1.ParentUid.Valid && this.TryComp<FocusOrderComponent>(comp1.ParentUid, out comp2) && comp2.Received.Count != 0)
    {
      fixedPoint2_1 = comp2.Received[0].Multiplier * comp2.AccuracyModifier;
      fixedPoint2_2 = comp2.Received[0].Multiplier * comp2.AccuracyPerTileModifier;
    }
    for (int index1 = 0; index1 < args.FiredProjectiles.Count; ++index1)
    {
      RMCProjectileAccuracyComponent comp3;
      if (this.TryComp<RMCProjectileAccuracyComponent>(args.FiredProjectiles[index1], out comp3))
      {
        comp3.Accuracy *= weapon.Comp.ModifiedAccuracyMultiplier;
        comp3.Accuracy += fixedPoint2_1;
        for (int index2 = 0; comp3.Thresholds.Count > index2; ++index2)
        {
          AccuracyFalloffThreshold threshold = comp3.Thresholds[index2];
          comp3.Thresholds[index2] = threshold with
          {
            Range = threshold.Range + weapon.Comp.RangeFlatModified
          };
        }
        if (fixedPoint2_2 != 0)
          comp3.Thresholds.Add(new AccuracyFalloffThreshold(0.0f, -fixedPoint2_2, false));
        comp3.GunSeed = (long) index1 << 32 /*0x20*/ | (long) (uint) id;
        this.Dirty<RMCProjectileAccuracyComponent>((Entity<RMCProjectileAccuracyComponent>) (args.FiredProjectiles[index1], comp3));
      }
    }
  }

  private void OnShowUseDelayShot(Entity<GunShowUseDelayComponent> ent, ref GunShotEvent args)
  {
    this.UpdateDelay(ent);
  }

  private void OnShowUseDelayWielded(
    Entity<GunShowUseDelayComponent> ent,
    ref ItemWieldedEvent args)
  {
    this.UpdateDelay(ent);
  }

  private void OnGunUserWhitelistAttemptShoot(
    Entity<GunUserWhitelistComponent> ent,
    ref AttemptShootEvent args)
  {
    if (args.Cancelled || this.HasComp<BypassInteractionChecksComponent>(args.User) || this._whitelist.IsValid(ent.Comp.Whitelist, args.User))
      return;
    args.Cancelled = true;
    this._popup.PopupClient(this.Loc.GetString("cm-gun-unskilled", ("gun", (object) ent.Owner)), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
  }

  private void OnGunUnskilledPenaltyRefresh(
    Entity<GunUnskilledPenaltyComponent> ent,
    ref GunRefreshModifiersEvent args)
  {
    Entity<SkillsComponent> user;
    if (this.TryGetUserSkills((EntityUid) ent, out user) && this._skills.HasSkill((Entity<SkillsComponent>) ((EntityUid) user, (SkillsComponent) user), ent.Comp.Skill, ent.Comp.Firearms))
      return;
    ref GunRefreshModifiersEvent local1 = ref args;
    local1.MinAngle = Angle.op_Addition(local1.MinAngle, ent.Comp.AngleIncrease);
    ref GunRefreshModifiersEvent local2 = ref args;
    local2.MaxAngle = Angle.op_Addition(local2.MaxAngle, ent.Comp.AngleIncrease);
  }

  private void OnGunUnskilledPenaltyGetWeaponAccuracy(
    Entity<GunUnskilledPenaltyComponent> ent,
    ref GetWeaponAccuracyEvent args)
  {
    Entity<SkillsComponent> user;
    if (this.TryGetUserSkills((EntityUid) ent, out user) && this._skills.HasSkill((Entity<SkillsComponent>) ((EntityUid) user, (SkillsComponent) user), ent.Comp.Skill, ent.Comp.Firearms))
      return;
    args.AccuracyMultiplier += ent.Comp.AccuracyAddMult;
  }

  private void OnGunDamageModifierMapInit(
    Entity<GunDamageModifierComponent> ent,
    ref MapInitEvent args)
  {
    this.RefreshGunDamageMultiplier((Entity<GunDamageModifierComponent>) (ent.Owner, ent.Comp));
  }

  private void OnGunDamageModifierAmmoShot(
    Entity<GunDamageModifierComponent> ent,
    ref AmmoShotEvent args)
  {
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
    {
      ProjectileComponent component;
      if (this._projectileQuery.TryGetComponent(firedProjectile, out component))
        component.Damage *= ent.Comp.ModifiedMultiplier;
    }
  }

  private void OnGunPointBlankMeleeHit(Entity<GunPointBlankComponent> gun, ref MeleeHitEvent args)
  {
    Entity<HandsComponent> user;
    if (!this.TryComp<MeleeWeaponComponent>((EntityUid) gun, out MeleeWeaponComponent _) || !this.TryGetGunUser(gun.Owner, out user))
      return;
    this.EnsureComp<UserPointblankCooldownComponent>((EntityUid) user).LastPBAt = this._timing.CurTime;
  }

  private void OnGunPointBlankAmmoShot(Entity<GunPointBlankComponent> gun, ref AmmoShotEvent args)
  {
    GunComponent comp1;
    if (!this.TryComp<GunComponent>(gun.Owner, out comp1) || !comp1.Target.HasValue)
      return;
    EntityUid uid = comp1.Target.Value;
    Entity<HandsComponent> user;
    if (!this.HasComp<TransformComponent>(uid) || !this.HasComp<EvasionComponent>(uid) || !this.TryGetGunUser(gun.Owner, out user))
      return;
    GetIFFFactionEvent args1 = new GetIFFFactionEvent(SlotFlags.IDCARD, new HashSet<EntProtoId<IFFFactionComponent>>());
    this.RaiseLocalEvent<GetIFFFactionEvent>(user.Owner, ref args1);
    GetIFFFactionEvent args2 = new GetIFFFactionEvent(SlotFlags.IDCARD, new HashSet<EntProtoId<IFFFactionComponent>>());
    this.RaiseLocalEvent<GetIFFFactionEvent>(uid, ref args2);
    if (args1.Factions.Count != 0 && args2.Factions.Count != 0 && args1.Factions.Overlaps((IEnumerable<EntProtoId<IFFFactionComponent>>) args2.Factions) && this.HasComp<EntityActiveInvisibleComponent>(uid))
      return;
    UserPointblankCooldownComponent cooldownComponent = this.EnsureComp<UserPointblankCooldownComponent>((EntityUid) user);
    if (this._timing.CurTime < cooldownComponent.LastPBAt + cooldownComponent.TimeBetweenPBs)
      return;
    ICommonSession playerSession = this.CompOrNull<ActorComponent>((EntityUid) user)?.PlayerSession;
    if (comp1.Target.Value == user.Owner && (comp1.SelectedMode == SelectiveFire.FullAuto || playerSession != null && !this._netConfig.GetClientCVar<bool>(playerSession.Channel, RMCCVars.RMCDamageYourself)) || !this._interaction.InRangeUnobstructed((Entity<TransformComponent>) gun.Owner, (Entity<TransformComponent>) comp1.Target.Value, gun.Comp.Range, user: new EntityUid?((EntityUid) user)))
      return;
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
    {
      ProjectileComponent comp2;
      PhysicsComponent comp3;
      if (this.TryComp<ProjectileComponent>(firedProjectile, out comp2) && this.TryComp<PhysicsComponent>(firedProjectile, out comp3) && this._rmcLagCompensation.IsWithinMargin((Entity<TransformComponent>) firedProjectile, (Entity<TransformComponent>) comp1.Target.Value, playerSession, gun.Comp.Range))
      {
        if (this._standing.IsDown(comp1.Target.Value))
        {
          comp2.Damage *= gun.Comp.ProneDamageMult;
          this.Dirty(firedProjectile, (IComponent) comp2);
        }
        this._projectile.ProjectileCollide((Entity<ProjectileComponent, PhysicsComponent>) (firedProjectile, comp2, comp3), comp1.Target.Value);
      }
    }
    cooldownComponent.LastPBAt = this._timing.CurTime;
    this.Dirty((EntityUid) user, (IComponent) cooldownComponent);
    MeleeWeaponComponent comp4;
    if (!this.TryComp<MeleeWeaponComponent>((EntityUid) gun, out comp4))
      return;
    comp4.NextAttack = cooldownComponent.LastPBAt + cooldownComponent.TimeBetweenPBs;
    this.Dirty((EntityUid) gun, (IComponent) comp4);
  }

  private void OnRecoilSkilledRefreshModifiers(
    Entity<GunSkilledRecoilComponent> ent,
    ref GunRefreshModifiersEvent args)
  {
    Entity<SkillsComponent> user;
    if (!this.TryGetUserSkills((EntityUid) ent, out user) || !this._skills.HasAllSkills((Entity<SkillsComponent>) ((EntityUid) user, (SkillsComponent) user), ent.Comp.Skills))
      return;
    if (ent.Comp.MustBeWielded)
    {
      WieldableComponent wieldableComponent = this.CompOrNull<WieldableComponent>((EntityUid) ent);
      if ((wieldableComponent != null ? (!wieldableComponent.Wielded ? 1 : 0) : 1) != 0)
        return;
    }
    args.CameraRecoilScalar = 0.0f;
  }

  private void OnAccuracySkilledGetWeaponAccuracy(
    Entity<GunSkilledAccuracyComponent> gun,
    ref GetWeaponAccuracyEvent args)
  {
    Entity<SkillsComponent> user;
    if (!this.TryGetUserSkills((EntityUid) gun, out user))
      return;
    args.AccuracyMultiplier += gun.Comp.AccuracyAddMult * this._skills.GetSkill((Entity<SkillsComponent>) ((EntityUid) user, (SkillsComponent) user), gun.Comp.Skill);
  }

  private void OnRequiresSkillsAttemptShoot(
    Entity<GunRequiresSkillsComponent> ent,
    ref AttemptShootEvent args)
  {
    if (args.Cancelled || this._skills.HasAllSkills((Entity<SkillsComponent>) args.User, ent.Comp.Skills))
      return;
    args.Cancelled = true;
    this._popup.PopupClient(this.Loc.GetString("cm-gun-unskilled", ("gun", (object) ent.Owner)), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
  }

  private void OnRequireEquippedAttemptShoot(
    Entity<GunRequireEquippedComponent> ent,
    ref AttemptShootEvent args)
  {
    if (args.Cancelled || this.HasRequiredEquippedPopup((Entity<GunRequireEquippedComponent>) ((EntityUid) ent, (GunRequireEquippedComponent) ent), args.User))
      return;
    args.Cancelled = true;
  }

  private void StopProjectile(
    Entity<ProjectileFixedDistanceComponent> projectile)
  {
    PhysicsComponent component;
    if (!this._physicsQuery.TryGetComponent((EntityUid) projectile, out component))
      return;
    this._physics.SetLinearVelocity((EntityUid) projectile, Vector2.Zero, body: component);
    this._physics.SetBodyStatus((EntityUid) projectile, component, BodyStatus.OnGround);
    if (!component.Awake)
      return;
    this._broadphase.RegenerateContacts((EntityUid) projectile, component);
  }

  private void UpdateDelay(Entity<GunShowUseDelayComponent> ent)
  {
    GunComponent comp;
    if (!this.TryComp<GunComponent>((EntityUid) ent, out comp))
      return;
    TimeSpan length = comp.NextFire - this._timing.CurTime;
    if (length <= TimeSpan.Zero)
      return;
    UseDelayComponent useDelayComponent = this.EnsureComp<UseDelayComponent>((EntityUid) ent);
    this._useDelay.SetLength((Entity<UseDelayComponent>) ((EntityUid) ent, useDelayComponent), length, ent.Comp.DelayId);
    this._useDelay.TryResetDelay((Entity<UseDelayComponent>) ((EntityUid) ent, useDelayComponent), id: ent.Comp.DelayId);
  }

  private void TryRefreshGunModifiers<TComp, TEvent>(Entity<TComp> ent, ref TEvent args) where TComp : IComponent?
  {
    GunComponent comp;
    if (!this.TryComp<GunComponent>((EntityUid) ent, out comp))
      return;
    this._gun.RefreshModifiers((Entity<GunComponent>) ((EntityUid) ent, comp));
  }

  private bool TryGetUserSkills(EntityUid gun, out Entity<SkillsComponent> user)
  {
    user = new Entity<SkillsComponent>();
    Entity<HandsComponent> user1;
    SkillsComponent comp;
    if (!this.TryGetGunUser(gun, out user1) || !this.TryComp<SkillsComponent>((EntityUid) user1, out comp))
      return false;
    user = (Entity<SkillsComponent>) ((EntityUid) user1, comp);
    return true;
  }

  public void RefreshGunDamageMultiplier(Entity<GunDamageModifierComponent?> gun)
  {
    gun.Comp = this.EnsureComp<GunDamageModifierComponent>((EntityUid) gun);
    GetGunDamageModifierEvent args = new GetGunDamageModifierEvent(gun.Comp.Multiplier);
    this.RaiseLocalEvent<GetGunDamageModifierEvent>((EntityUid) gun, ref args);
    gun.Comp.ModifiedMultiplier = args.Multiplier;
    this.Dirty<GunDamageModifierComponent>(gun);
  }

  public bool HasRequiredEquippedPopup(Entity<GunRequireEquippedComponent?> gun, EntityUid user)
  {
    if (!this.Resolve<GunRequireEquippedComponent>((EntityUid) gun, ref gun.Comp, false))
      return true;
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) user, SlotFlags.OUTERCLOTHING);
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      if (this._whitelist.IsValid(gun.Comp.Whitelist, container.ContainedEntity))
        return true;
    }
    this._popup.PopupClient(this.Loc.GetString("rmc-shoot-harness-required"), user, new EntityUid?(user), PopupType.MediumCaution);
    return false;
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ProjectileFixedDistanceComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ProjectileFixedDistanceComponent>();
    EntityUid uid;
    ProjectileFixedDistanceComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(curTime < comp1.FlyEndTime))
      {
        MapCoordinates? targetCoordinates = comp1.TargetCoordinates;
        if (targetCoordinates.HasValue)
        {
          MapCoordinates valueOrDefault = targetCoordinates.GetValueOrDefault();
          this._transform.SetMapCoordinates(uid, valueOrDefault);
        }
        this.StopProjectile((Entity<ProjectileFixedDistanceComponent>) (uid, comp1));
        this.RemCompDeferred<ProjectileFixedDistanceComponent>(uid);
        ProjectileFixedDistanceStopEvent args = new ProjectileFixedDistanceStopEvent();
        this.RaiseLocalEvent<ProjectileFixedDistanceStopEvent>(uid, ref args);
        if (this._net.IsClient && this.IsClientSide(uid) && this.HasComp<DeleteOnFixedDistanceStopComponent>(uid))
          this.QueueDel(new EntityUid?(uid));
      }
    }
  }

  private void OnRevolverUniqueAction(
    Entity<RevolverAmmoProviderComponent> gun,
    ref UniqueActionEvent args)
  {
    if (args.Handled)
      return;
    int num = this._random.Next(1, gun.Comp.Capacity + 1);
    gun.Comp.CurrentIndex = (gun.Comp.CurrentIndex + num) % gun.Comp.Capacity;
    this._audio.PlayPredicted(gun.Comp.SoundSpin, gun.Owner, new EntityUid?(args.UserUid));
    this._popup.PopupClient(this.Loc.GetString("rmc-revolver-spin", (nameof (gun), (object) args.UserUid)), args.UserUid, new EntityUid?(args.UserUid), PopupType.SmallCaution);
    this.Dirty<RevolverAmmoProviderComponent>(gun);
  }

  private void OnUserBlockShootingInsideContainersAttemptShoot(
    Entity<UserBlockShootingInsideContainersComponent> ent,
    ref ShotAttemptedEvent args)
  {
    if (args.Cancelled || !this._container.IsEntityInContainer((EntityUid) ent))
      return;
    args.Cancel();
  }

  private void OnAmmoEjectActivateInWorld(
    Entity<RMCAmmoEjectComponent> gun,
    ref ActivateInWorldEvent args)
  {
    BaseContainer container;
    if (args.Handled || !this._container.TryGetContainer(gun.Owner, gun.Comp.ContainerID, out container) || container.ContainedEntities.Count <= 0)
      return;
    string activeHand = this._hands.GetActiveHand((Entity<HandsComponent>) args.User);
    if (activeHand == null || !this._hands.HandIsEmpty((Entity<HandsComponent>) args.User, activeHand) || !this._hands.CanPickupToHand(args.User, container.ContainedEntities[0], activeHand))
      return;
    RMCTryAmmoEjectEvent args1 = new RMCTryAmmoEjectEvent(args.User, false);
    this.RaiseLocalEvent<RMCTryAmmoEjectEvent>(gun.Owner, ref args1);
    if (args1.Cancelled)
      return;
    args.Handled = true;
    EntityUid containedEntity = container.ContainedEntities[0];
    if (this.TryComp<BallisticAmmoProviderComponent>(gun.Owner, out BallisticAmmoProviderComponent _))
    {
      TakeAmmoEvent args2 = new TakeAmmoEvent(1, new List<(EntityUid?, IShootable)>(), this.Transform(gun.Owner).Coordinates, new EntityUid?(args.User));
      this.RaiseLocalEvent<TakeAmmoEvent>(gun.Owner, args2);
      if (args2.Ammo.Count <= 0)
        return;
      EntityUid? entity = args2.Ammo[0].Entity;
      if (!entity.HasValue)
        return;
      containedEntity = entity.Value;
    }
    if (!this.HasComp<ItemSlotsComponent>(gun.Owner) || !this._slots.TryEject(gun.Owner, gun.Comp.ContainerID, new EntityUid?(args.User), out EntityUid? _, excludeUserAudio: true))
      this._audio.PlayPredicted(gun.Comp.EjectSound, gun.Owner, new EntityUid?(args.User));
    this._hands.TryPickup(args.User, containedEntity, activeHand);
  }

  private void OnDualWieldingEquippedHand(
    Entity<GunDualWieldingComponent> gun,
    ref GotEquippedHandEvent args)
  {
    this.RefreshGunHolderModifiers(gun, args.User);
  }

  private void OnDualWieldingUnequippedHand(
    Entity<GunDualWieldingComponent> gun,
    ref GotUnequippedHandEvent args)
  {
    this.RefreshGunHolderModifiers(gun, args.User);
  }

  private void OnDualWieldingRefreshModifiers(
    Entity<GunDualWieldingComponent> gun,
    ref GunRefreshModifiersEvent args)
  {
    Entity<HandsComponent> user;
    if (gun.Comp.WeaponGroup == GunDualWieldingGroup.None || !this.TryGetGunUser((EntityUid) gun, out user) || !this.TryGetOtherDualWieldedGun((EntityUid) user, gun, out Entity<GunDualWieldingComponent> _))
      return;
    args.CameraRecoilScalar += gun.Comp.RecoilModifier;
    ref GunRefreshModifiersEvent local1 = ref args;
    local1.MinAngle = Angle.op_Addition(local1.MinAngle, gun.Comp.ScatterModifier);
    ref GunRefreshModifiersEvent local2 = ref args;
    local2.MaxAngle = Angle.op_Addition(local2.MaxAngle, gun.Comp.ScatterModifier);
  }

  private void OnDualWieldingGetWeaponAccuracy(
    Entity<GunDualWieldingComponent> gun,
    ref GetWeaponAccuracyEvent args)
  {
    Entity<HandsComponent> user;
    if (gun.Comp.WeaponGroup == GunDualWieldingGroup.None || !this.TryGetGunUser((EntityUid) gun, out user) || !this.TryGetOtherDualWieldedGun((EntityUid) user, gun, out Entity<GunDualWieldingComponent> _))
      return;
    args.AccuracyMultiplier += gun.Comp.AccuracyAddMult;
  }

  private void OnDualWieldingStopShoot(RequestStopShootEvent ev, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    EntityUid entity = this.GetEntity(ev.Gun);
    GunComponent comp1;
    EntityUid offHand;
    GunComponent comp2;
    if (!this.TryComp<GunComponent>(entity, out comp1) || !this.TryGetAkimboOffHand(valueOrDefault, (Entity<GunComponent>) (entity, comp1), out offHand) || !this.TryComp<GunComponent>(offHand, out comp2))
      return;
    this._gun.StopShooting(offHand, comp2);
  }

  public bool TryGetAkimboOffHand(
    EntityUid user,
    Entity<GunComponent> activeGun,
    out EntityUid offHand)
  {
    offHand = new EntityUid();
    GunDualWieldingComponent comp;
    Entity<GunDualWieldingComponent> otherGun;
    if (!this.TryComp<GunDualWieldingComponent>((EntityUid) activeGun, out comp) || !comp.Akimbo || comp.WeaponGroup == GunDualWieldingGroup.None || !this.TryGetOtherDualWieldedGun(user, (Entity<GunDualWieldingComponent>) ((EntityUid) activeGun, comp), out otherGun) || !otherGun.Comp.Akimbo)
      return false;
    offHand = otherGun.Owner;
    return true;
  }

  private void OnUnremoveableDropAttempt(
    Entity<UnremoveableComponent> ent,
    ref RMCItemDropAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private bool TryGetOtherDualWieldedGun(
    EntityUid user,
    Entity<GunDualWieldingComponent> gun,
    out Entity<GunDualWieldingComponent> otherGun)
  {
    otherGun = new Entity<GunDualWieldingComponent>();
    HandsComponent comp1;
    if (!this.TryComp<HandsComponent>(user, out comp1))
      return false;
    foreach (string key in comp1.Hands.Keys)
    {
      EntityUid? heldItem = this._hands.GetHeldItem((Entity<HandsComponent>) user, key);
      if (heldItem.HasValue)
      {
        EntityUid valueOrDefault = heldItem.GetValueOrDefault();
        GunDualWieldingComponent comp2;
        if (valueOrDefault != gun.Owner && this.TryComp<GunDualWieldingComponent>(valueOrDefault, out comp2) && comp2.WeaponGroup == gun.Comp.WeaponGroup)
        {
          otherGun = (Entity<GunDualWieldingComponent>) (valueOrDefault, comp2);
          return true;
        }
      }
    }
    return false;
  }

  public bool TryGetGunUser(EntityUid gun, out Entity<HandsComponent> user)
  {
    BaseContainer container;
    EntityUid operatorUid;
    HandsComponent comp1;
    if (this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (gun, (TransformComponent) null), out container) && this._vehicleWeapons.TryGetOperatorForSelectedWeapon(container.Owner, gun, out operatorUid) && this.TryComp<HandsComponent>(operatorUid, out comp1))
    {
      user = (Entity<HandsComponent>) (operatorUid, comp1);
      return true;
    }
    HandsComponent comp2;
    if (container != null && this.TryComp<HandsComponent>(container.Owner, out comp2))
    {
      user = (Entity<HandsComponent>) (container.Owner, comp2);
      return true;
    }
    AttachableHolderComponent comp3;
    if (container != null && this.TryComp<AttachableHolderComponent>(container.Owner, out comp3))
    {
      EntityUid? supercedingAttachable = comp3.SupercedingAttachable;
      EntityUid entityUid = gun;
      if ((supercedingAttachable.HasValue ? (supercedingAttachable.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
        return this.TryGetGunUser(container.Owner, out user);
    }
    user = new Entity<HandsComponent>();
    return false;
  }

  private void RefreshGunHolderModifiers(Entity<GunDualWieldingComponent> gun, EntityUid user)
  {
    this._gun.RefreshModifiers((Entity<GunComponent>) gun.Owner);
    Entity<GunDualWieldingComponent> otherGun;
    if (!this.TryGetOtherDualWieldedGun(user, gun, out otherGun))
      return;
    this._gun.RefreshModifiers((Entity<GunComponent>) otherGun.Owner);
  }

  private void OnAssistedReloadAmmoAfterInteract(
    Entity<AssistedReloadAmmoComponent> ent,
    ref AfterInteractEvent args)
  {
    if (!args.CanReach)
      return;
    EntityUid? target1 = args.Target;
    if (!target1.HasValue)
      return;
    EntityUid user = args.User;
    target1 = args.Target;
    EntityUid target2 = target1.Value;
    Entity<AssistedReloadAmmoComponent> ammo = ent;
    this.TryAssistedReload(user, target2, ammo);
  }

  private bool IsBehindTarget(EntityUid user, EntityUid target)
  {
    Angle localRotation = this.Transform(target).LocalRotation;
    Angle angle = DirectionExtensions.ToAngle(DirectionExtensions.GetOpposite(((Angle) ref localRotation).GetCardinalDir()));
    Angle worldAngle = DirectionExtensions.ToWorldAngle(this._transform.GetMapCoordinates(user).Position - this._transform.GetMapCoordinates(target).Position);
    double num = (((Angle) ref angle).Degrees - ((Angle) ref worldAngle).Degrees + 180.0 + 360.0) % 360.0 - 180.0;
    return num > -45.0 && num < 45.0;
  }

  private void TryAssistedReload(
    EntityUid user,
    EntityUid target,
    Entity<AssistedReloadAmmoComponent> ammo)
  {
    AssistedReloadReceiverComponent comp1;
    BallisticAmmoProviderComponent comp2;
    if (!this.TryComp<AssistedReloadReceiverComponent>(target, out comp1) || !comp1.Weapon.HasValue || !this.TryComp<BallisticAmmoProviderComponent>(comp1.Weapon, out comp2))
      return;
    if (this._whitelist.IsWhitelistFailOrNull(comp2.Whitelist, ammo.Owner))
      this._popup.PopupClient(this.Loc.GetString("rmc-assisted-reload-fail-mismatch", (nameof (ammo), (object) ammo.Owner), ("weapon", (object) comp1.Weapon)), user, new EntityUid?(user), PopupType.SmallCaution);
    else if (!this.IsBehindTarget(user, target))
      this._popup.PopupClient(this.Loc.GetString("rmc-assisted-reload-fail-angle", (nameof (target), (object) target)), user, new EntityUid?(user), PopupType.SmallCaution);
    else if (!this._gun.TryAmmoInsert(comp1.Weapon.Value, comp2, ammo.Owner, user, comp1.Weapon.Value, ammo.Comp.InsertDelay))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-assisted-reload-fail-full", (nameof (target), (object) target), ("weapon", (object) comp1.Weapon)), user, new EntityUid?(user), PopupType.SmallCaution);
    }
    else
    {
      string message1 = this.Loc.GetString("rmc-assisted-reload-start-user", (nameof (target), (object) target), ("weapon", (object) comp1.Weapon));
      string message2 = this.Loc.GetString("rmc-assisted-reload-start-target", ("reloader", (object) user), ("weapon", (object) comp1.Weapon), (nameof (ammo), (object) ammo.Owner));
      this._popup.PopupClient(message1, user, new EntityUid?(user));
      this._popup.PopupEntity(message2, target, target);
    }
  }

  private void OnAssistedReloadWeaponWielded(
    Entity<AssistedReloadWeaponComponent> ent,
    ref ItemWieldedEvent args)
  {
    Entity<HandsComponent> user;
    if (!this.TryGetGunUser(ent.Owner, out user))
      return;
    this.EnsureComp<AssistedReloadReceiverComponent>((EntityUid) user).Weapon = new EntityUid?(ent.Owner);
  }

  private void OnAssistedReloadWeaponUnwielded(
    Entity<AssistedReloadWeaponComponent> ent,
    ref ItemUnwieldedEvent args)
  {
    Entity<HandsComponent> user;
    if (!this.TryGetGunUser(ent.Owner, out user))
      return;
    this.RemCompDeferred<AssistedReloadReceiverComponent>((EntityUid) user);
  }

  private void OnBeforeArc(Entity<IgnoreArcComponent> ent, ref BeforeArcEvent args)
  {
    args.Cancelled = true;
  }
}
