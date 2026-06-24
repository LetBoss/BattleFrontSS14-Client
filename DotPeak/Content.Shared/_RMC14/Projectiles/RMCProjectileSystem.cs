// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Projectiles.RMCProjectileSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Evasion;
using Content.Shared._RMC14.Random;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Mobs.Systems;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Systems;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Projectiles;

public sealed class RMCProjectileSystem : EntitySystem
{
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private NpcFactionSystem _npcFaction;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private SharedXenoHiveSystem _hive;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<DeleteOnCollideComponent, StartCollideEvent>(new EntityEventRefHandler<DeleteOnCollideComponent, StartCollideEvent>(this.OnDeleteOnCollideStartCollide));
    this.SubscribeLocalEvent<ModifyTargetOnHitComponent, ProjectileHitEvent>(new EntityEventRefHandler<ModifyTargetOnHitComponent, ProjectileHitEvent>(this.OnModifyTargetOnHit));
    this.SubscribeLocalEvent<ProjectileMaxRangeComponent, MapInitEvent>(new EntityEventRefHandler<ProjectileMaxRangeComponent, MapInitEvent>(this.OnProjectileMaxRangeMapInit));
    this.SubscribeLocalEvent<RMCProjectileDamageFalloffComponent, MapInitEvent>(new EntityEventRefHandler<RMCProjectileDamageFalloffComponent, MapInitEvent>(this.OnFalloffProjectileMapInit));
    this.SubscribeLocalEvent<RMCProjectileDamageFalloffComponent, ProjectileHitEvent>(new EntityEventRefHandler<RMCProjectileDamageFalloffComponent, ProjectileHitEvent>(this.OnFalloffProjectileHit));
    this.SubscribeLocalEvent<RMCProjectileAccuracyComponent, MapInitEvent>(new EntityEventRefHandler<RMCProjectileAccuracyComponent, MapInitEvent>(this.OnProjectileAccuracyMapInit));
    this.SubscribeLocalEvent<RMCProjectileAccuracyComponent, PreventCollideEvent>(new EntityEventRefHandler<RMCProjectileAccuracyComponent, PreventCollideEvent>(this.OnProjectileAccuracyPreventCollide));
    this.SubscribeLocalEvent<SpawnOnTerminateComponent, MapInitEvent>(new EntityEventRefHandler<SpawnOnTerminateComponent, MapInitEvent>(this.OnSpawnOnTerminatingMapInit));
    this.SubscribeLocalEvent<SpawnOnTerminateComponent, EntityTerminatingEvent>(new EntityEventRefHandler<SpawnOnTerminateComponent, EntityTerminatingEvent>(this.OnSpawnOnTerminatingTerminate));
    this.SubscribeLocalEvent<PreventCollideWithDeadComponent, PreventCollideEvent>(new EntityEventRefHandler<PreventCollideWithDeadComponent, PreventCollideEvent>(this.OnPreventCollideWithDead));
  }

  private void OnDeleteOnCollideStartCollide(
    Entity<DeleteOnCollideComponent> ent,
    ref StartCollideEvent args)
  {
    if (!this._net.IsServer)
      return;
    this.QueueDel(new EntityUid?((EntityUid) ent));
  }

  private void OnModifyTargetOnHit(
    Entity<ModifyTargetOnHitComponent> ent,
    ref ProjectileHitEvent args)
  {
    if (!this._whitelist.IsWhitelistPassOrNull(ent.Comp.Whitelist, args.Target))
      return;
    ComponentRegistry add = ent.Comp.Add;
    if (add == null)
      return;
    this.EntityManager.AddComponents(args.Target, add, true);
  }

  private void OnProjectileMaxRangeMapInit(
    Entity<ProjectileMaxRangeComponent> ent,
    ref MapInitEvent args)
  {
    ent.Comp.Origin = new EntityCoordinates?(this._transform.GetMoverCoordinates((EntityUid) ent));
    this.Dirty<ProjectileMaxRangeComponent>(ent);
  }

  private void OnFalloffProjectileMapInit(
    Entity<RMCProjectileDamageFalloffComponent> projectile,
    ref MapInitEvent args)
  {
    projectile.Comp.ShotFrom = new EntityCoordinates?(this._transform.GetMoverCoordinates(projectile.Owner));
    this.Dirty<RMCProjectileDamageFalloffComponent>(projectile);
  }

  private void OnFalloffProjectileHit(
    Entity<RMCProjectileDamageFalloffComponent> projectile,
    ref ProjectileHitEvent args)
  {
    if (!projectile.Comp.ShotFrom.HasValue || projectile.Comp.MinRemainingDamageMult < 0)
      return;
    float num1 = (this._transform.GetMoverCoordinates(args.Target).Position - projectile.Comp.ShotFrom.Value.Position).Length();
    FixedPoint2 fixedPoint2_1 = args.Damage.GetTotal() * projectile.Comp.MinRemainingDamageMult;
    foreach (DamageFalloffThreshold threshold in projectile.Comp.Thresholds)
    {
      float num2 = num1 - threshold.Range;
      if ((double) num2 > 0.0)
      {
        FixedPoint2 total = args.Damage.GetTotal();
        if (total <= fixedPoint2_1)
          break;
        FixedPoint2 fixedPoint2_2 = threshold.IgnoreModifiers ? (FixedPoint2) 1 : projectile.Comp.WeaponMult;
        FixedPoint2 min = FixedPoint2.Min(fixedPoint2_1 / total, (FixedPoint2) 1);
        args.Damage *= FixedPoint2.Clamp((total - (FixedPoint2) num2 * threshold.Falloff * fixedPoint2_2) / total, min, (FixedPoint2) 1);
      }
    }
  }

  public void SetProjectileFalloffWeaponMult(
    Entity<RMCProjectileDamageFalloffComponent> projectile,
    FixedPoint2 mult,
    float range)
  {
    for (int index = 0; projectile.Comp.Thresholds.Count > index; ++index)
    {
      DamageFalloffThreshold threshold = projectile.Comp.Thresholds[index];
      projectile.Comp.Thresholds[index] = threshold with
      {
        Range = threshold.Range + range
      };
    }
    projectile.Comp.WeaponMult = mult;
    this.Dirty<RMCProjectileDamageFalloffComponent>(projectile);
  }

  private void OnProjectileAccuracyMapInit(
    Entity<RMCProjectileAccuracyComponent> projectile,
    ref MapInitEvent args)
  {
    projectile.Comp.ShotFrom = new EntityCoordinates?(this._transform.GetMoverCoordinates(projectile.Owner));
    projectile.Comp.Tick = this._timing.CurTick.Value;
    this.Dirty<RMCProjectileAccuracyComponent>(projectile);
  }

  private void OnProjectileAccuracyPreventCollide(
    Entity<RMCProjectileAccuracyComponent> projectile,
    ref PreventCollideEvent args)
  {
    EvasionComponent comp;
    if (args.Cancelled || projectile.Comp.ForceHit || !projectile.Comp.ShotFrom.HasValue || !this.TryComp<ProjectileComponent>(projectile.Owner, out ProjectileComponent _) || !this.TryComp<EvasionComponent>(args.OtherEntity, out comp))
      return;
    FixedPoint2 accuracy = projectile.Comp.Accuracy;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(args.OtherEntity);
    float range = (moverCoordinates.Position - projectile.Comp.ShotFrom.Value.Position).Length();
    foreach (AccuracyFalloffThreshold threshold in projectile.Comp.Thresholds)
    {
      float num = range - threshold.Range;
      if (threshold.Buildup)
      {
        if ((double) num < 0.0)
          accuracy += threshold.Falloff * num;
      }
      else if ((double) num > 0.0)
        accuracy -= threshold.Falloff * num;
    }
    if (!this._examine.InRangeUnOccluded(this._transform.ToMapCoordinates(projectile.Comp.ShotFrom.Value), this._transform.ToMapCoordinates(moverCoordinates), range, (SharedInteractionSystem.Ignored) null))
      accuracy += (FixedPoint2) -15;
    if (!projectile.Comp.IgnoreFriendlyEvasion && this.IsProjectileTargetFriendly(projectile.Owner, args.OtherEntity))
      accuracy -= comp.ModifiedEvasionFriendly;
    FixedPoint2 fixedPoint2 = accuracy - comp.ModifiedEvasion;
    if ((fixedPoint2 > projectile.Comp.MinAccuracy ? fixedPoint2 : projectile.Comp.MinAccuracy) >= (FixedPoint2) new Xoshiro128P(projectile.Comp.GunSeed, (long) projectile.Comp.Tick << 32 /*0x20*/ | (long) (uint) this.GetNetEntity(args.OtherEntity).Id).NextFloat(0.0f, 100f))
      return;
    args.Cancelled = true;
  }

  private bool IsProjectileTargetFriendly(EntityUid projectile, EntityUid target)
  {
    ProjectileComponent comp;
    return this.TryComp<ProjectileComponent>(projectile, out comp) && comp.Shooter.HasValue && this._npcFaction.IsEntityFriendly((Entity<NpcFactionMemberComponent>) comp.Shooter.Value, (Entity<NpcFactionMemberComponent>) target);
  }

  private void OnSpawnOnTerminatingMapInit(
    Entity<SpawnOnTerminateComponent> ent,
    ref MapInitEvent args)
  {
    ent.Comp.Origin = new EntityCoordinates?(this._transform.GetMoverCoordinates((EntityUid) ent));
    this.Dirty<SpawnOnTerminateComponent>(ent);
  }

  private void OnSpawnOnTerminatingTerminate(
    Entity<SpawnOnTerminateComponent> ent,
    ref EntityTerminatingEvent args)
  {
    TransformComponent comp;
    if (this._net.IsClient || !this.TryComp((EntityUid) ent, out comp) || this.TerminatingOrDeleted(comp.ParentUid))
      return;
    EntityCoordinates coordinates = comp.Coordinates;
    if (ent.Comp.ProjectileAdjust)
    {
      EntityCoordinates? origin = ent.Comp.Origin;
      if (origin.HasValue)
      {
        EntityCoordinates valueOrDefault = origin.GetValueOrDefault();
        Vector2 delta;
        if (coordinates.TryDelta((IEntityManager) this.EntityManager, this._transform, valueOrDefault, out delta) && (double) delta.Length() > 0.0)
        {
          coordinates = coordinates.Offset(Vector2Helpers.Normalized(delta) / -2f);
          if (this.HasComp<RMCFireProjectileComponent>((EntityUid) ent))
            coordinates = coordinates.Offset(Vector2Helpers.Normalized(delta));
        }
      }
    }
    EntityUid dest = this.SpawnAtPosition((string) ent.Comp.Spawn, coordinates);
    this._hive.SetSameHive((Entity<HiveMemberComponent>) ent.Owner, (Entity<HiveMemberComponent>) dest);
    LocId? popup = ent.Comp.Popup;
    if (!popup.HasValue)
      return;
    this._popup.PopupCoordinates(this.Loc.GetString((string) popup.GetValueOrDefault()), coordinates, ent.Comp.PopupType.GetValueOrDefault());
  }

  private void OnPreventCollideWithDead(
    Entity<PreventCollideWithDeadComponent> ent,
    ref PreventCollideEvent args)
  {
    if (args.Cancelled || !this._mobState.IsDead(args.OtherEntity))
      return;
    args.Cancelled = true;
  }

  public void SetMaxRange(Entity<ProjectileMaxRangeComponent> ent, float max)
  {
    ent.Comp.Max = max;
    this.Dirty<ProjectileMaxRangeComponent>(ent);
  }

  private void StopProjectile(Entity<ProjectileMaxRangeComponent> ent)
  {
    if (ent.Comp.Delete)
    {
      if (!this._net.IsServer && !this.IsClientSide((EntityUid) ent))
        return;
      this.QueueDel(new EntityUid?((EntityUid) ent));
    }
    else
    {
      this._physics.SetLinearVelocity((EntityUid) ent, Vector2.Zero);
      this.RemCompDeferred<ProjectileMaxRangeComponent>((EntityUid) ent);
    }
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<ProjectileMaxRangeComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ProjectileMaxRangeComponent>();
    EntityUid uid;
    ProjectileMaxRangeComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(uid);
      EntityCoordinates? origin = comp1.Origin;
      if (origin.HasValue)
      {
        EntityCoordinates valueOrDefault = origin.GetValueOrDefault();
        float distance;
        if (moverCoordinates.TryDistance((IEntityManager) this.EntityManager, this._transform, valueOrDefault, out distance))
        {
          if ((double) distance >= (double) comp1.Max || (double) Math.Abs(distance - comp1.Max) <= 0.10000000149011612)
          {
            this.StopProjectile((Entity<ProjectileMaxRangeComponent>) (uid, comp1));
            continue;
          }
          continue;
        }
      }
      this.StopProjectile((Entity<ProjectileMaxRangeComponent>) (uid, comp1));
    }
  }
}
