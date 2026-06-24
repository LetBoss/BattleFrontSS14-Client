// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Weapons.Ranged.PubgGunRangeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._PUBG.Loadout;
using Content.Shared._RMC14.Projectiles;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.FixedPoint;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._PUBG.Weapons.Ranged;

public sealed class PubgGunRangeSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private PubgWeaponModulesSystem _weaponModules;
  private Robust.Shared.GameObjects.EntityQuery<ProjectileComponent> _projectileQuery;
  private Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> _physicsQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._projectileQuery = this.GetEntityQuery<ProjectileComponent>();
    this._physicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this.SubscribeLocalEvent<GunRangeModifierComponent, MapInitEvent>(new EntityEventRefHandler<GunRangeModifierComponent, MapInitEvent>(this.OnGunRangeModifierMapInit));
    this.SubscribeLocalEvent<GunRangeModifierComponent, AmmoShotEvent>(new EntityEventRefHandler<GunRangeModifierComponent, AmmoShotEvent>(this.OnGunRangeModifierAmmoShot));
    this.SubscribeLocalEvent<PubgWeaponModulesComponent, GetGunRangeModifierEvent>(new EntityEventRefHandler<PubgWeaponModulesComponent, GetGunRangeModifierEvent>(this.OnWeaponModulesGetRangeModifier));
  }

  private void OnGunRangeModifierMapInit(
    Entity<GunRangeModifierComponent> ent,
    ref MapInitEvent args)
  {
    this.RefreshGunRangeMultiplier((Entity<GunRangeModifierComponent>) (ent.Owner, ent.Comp));
  }

  private void OnWeaponModulesGetRangeModifier(
    Entity<PubgWeaponModulesComponent> ent,
    ref GetGunRangeModifierEvent args)
  {
    float rangeMultiplier = this._weaponModules.GetRangeMultiplier((EntityUid) ent, ent.Comp);
    args.Multiplier *= rangeMultiplier;
  }

  private void OnGunRangeModifierAmmoShot(
    Entity<GunRangeModifierComponent> ent,
    ref AmmoShotEvent args)
  {
    if (ent.Comp.ModifiedMultiplier == (FixedPoint2) 1.0)
      return;
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
    {
      ProjectileMaxRangeComponent comp1;
      if (this.TryComp<ProjectileMaxRangeComponent>(firedProjectile, out comp1))
      {
        comp1.Max *= ent.Comp.ModifiedMultiplier.Float();
        this.Dirty(firedProjectile, (IComponent) comp1);
      }
      ProjectileFixedDistanceComponent comp2;
      if (this.TryComp<ProjectileFixedDistanceComponent>(firedProjectile, out comp2))
      {
        MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(firedProjectile);
        MapCoordinates? targetCoordinates = comp2.TargetCoordinates;
        if (targetCoordinates.HasValue)
        {
          MapCoordinates valueOrDefault = targetCoordinates.GetValueOrDefault();
          if (mapCoordinates.MapId == valueOrDefault.MapId)
          {
            Vector2 vector2 = valueOrDefault.Position - mapCoordinates.Position;
            float num1 = vector2.Length();
            float num2 = num1 * ent.Comp.ModifiedMultiplier.Float();
            comp2.TargetCoordinates = new MapCoordinates?(new MapCoordinates(mapCoordinates.Position + Vector2Helpers.Normalized(vector2) * num2, mapCoordinates.MapId));
            PhysicsComponent component;
            if (this._physicsQuery.TryComp(firedProjectile, out component) && (double) component.LinearVelocity.Length() > 0.0)
            {
              float num3 = component.LinearVelocity.Length();
              TimeSpan timeSpan = comp2.FlyEndTime - TimeSpan.FromSeconds((double) num1 / (double) num3);
              comp2.FlyEndTime = timeSpan + TimeSpan.FromSeconds((double) num2 / (double) num3);
            }
            this.Dirty(firedProjectile, (IComponent) comp2);
          }
        }
      }
      RMCProjectileDamageFalloffComponent comp3;
      if (this.TryComp<RMCProjectileDamageFalloffComponent>(firedProjectile, out comp3))
      {
        float num = ent.Comp.ModifiedMultiplier.Float();
        for (int index = 0; index < comp3.Thresholds.Count; ++index)
        {
          DamageFalloffThreshold threshold = comp3.Thresholds[index];
          comp3.Thresholds[index] = threshold with
          {
            Range = threshold.Range * num
          };
        }
        this.Dirty(firedProjectile, (IComponent) comp3);
      }
    }
  }

  public void RefreshGunRangeMultiplier(Entity<GunRangeModifierComponent?> gun)
  {
    gun.Comp = this.EnsureComp<GunRangeModifierComponent>((EntityUid) gun);
    GetGunRangeModifierEvent args = new GetGunRangeModifierEvent(gun.Comp.Multiplier);
    this.RaiseLocalEvent<GetGunRangeModifierEvent>((EntityUid) gun, ref args);
    gun.Comp.ModifiedMultiplier = args.Multiplier;
    this.Dirty<GunRangeModifierComponent>(gun);
  }
}
