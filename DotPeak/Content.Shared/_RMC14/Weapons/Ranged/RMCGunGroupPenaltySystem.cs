// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.RMCGunGroupPenaltySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class RMCGunGroupPenaltySystem : EntitySystem
{
  [Dependency]
  private SharedGunSystem _gun;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private CMGunSystem _rmcGun;
  private Robust.Shared.GameObjects.EntityQuery<GunGroupPenaltyComponent> _gunGroupPenalty;
  private Robust.Shared.GameObjects.EntityQuery<ProjectileComponent> _projectileQuery;

  public override void Initialize()
  {
    this._gunGroupPenalty = this.GetEntityQuery<GunGroupPenaltyComponent>();
    this._projectileQuery = this.GetEntityQuery<ProjectileComponent>();
    this.SubscribeLocalEvent<GunGroupPenaltyComponent, GotEquippedHandEvent>(new EntityEventRefHandler<GunGroupPenaltyComponent, GotEquippedHandEvent>(this.OnGroupSpreadPenaltyEquippedHand));
    this.SubscribeLocalEvent<GunGroupPenaltyComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<GunGroupPenaltyComponent, GotUnequippedHandEvent>(this.OnGroupSpreadPenaltyUnequippedHand));
    this.SubscribeLocalEvent<GunGroupPenaltyComponent, GunRefreshModifiersEvent>(new EntityEventRefHandler<GunGroupPenaltyComponent, GunRefreshModifiersEvent>(this.OnGroupSpreadPenaltyRefreshModifiers));
    this.SubscribeLocalEvent<GunGroupPenaltyComponent, AmmoShotEvent>(new EntityEventRefHandler<GunGroupPenaltyComponent, AmmoShotEvent>(this.OnGroupSpreadPenaltyAmmoShot), new Type[1]
    {
      typeof (CMGunSystem)
    });
  }

  private void OnGroupSpreadPenaltyEquippedHand(
    Entity<GunGroupPenaltyComponent> ent,
    ref GotEquippedHandEvent args)
  {
    this.RefreshGunHolderModifiers(ent);
  }

  private void OnGroupSpreadPenaltyUnequippedHand(
    Entity<GunGroupPenaltyComponent> ent,
    ref GotUnequippedHandEvent args)
  {
    this.RefreshGunHolderModifiers(ent);
  }

  private void OnGroupSpreadPenaltyRefreshModifiers(
    Entity<GunGroupPenaltyComponent> ent,
    ref GunRefreshModifiersEvent args)
  {
    Entity<HandsComponent> user;
    if (!this._rmcGun.TryGetGunUser((EntityUid) ent, out user))
      return;
    foreach (EntityUid uid in this._hands.EnumerateHeld((Entity<HandsComponent>) ((EntityUid) user, (HandsComponent) user)))
    {
      if (!(uid == ent.Owner) && this._gunGroupPenalty.HasComp(uid))
      {
        args.CameraRecoilScalar += ent.Comp.Recoil;
        ref GunRefreshModifiersEvent local1 = ref args;
        local1.AngleIncrease = Angle.op_Addition(local1.AngleIncrease, ent.Comp.AngleIncrease);
        ref GunRefreshModifiersEvent local2 = ref args;
        local2.MinAngle = Angle.op_Addition(local2.MinAngle, Angle.op_Implicit(Angle.op_Implicit(ent.Comp.AngleIncrease) / 2.0));
        ref GunRefreshModifiersEvent local3 = ref args;
        local3.MaxAngle = Angle.op_Addition(local3.MaxAngle, ent.Comp.AngleIncrease);
        break;
      }
    }
  }

  private void OnGroupSpreadPenaltyAmmoShot(
    Entity<GunGroupPenaltyComponent> ent,
    ref AmmoShotEvent args)
  {
    Entity<HandsComponent> user;
    if (!this._rmcGun.TryGetGunUser((EntityUid) ent, out user))
      return;
    bool flag = false;
    foreach (EntityUid uid in this._hands.EnumerateHeld((Entity<HandsComponent>) ((EntityUid) user, (HandsComponent) user)))
    {
      if (uid != ent.Owner && this._gunGroupPenalty.HasComp(uid))
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      return;
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
    {
      ProjectileComponent component;
      if (this._projectileQuery.TryComp(firedProjectile, out component))
        component.Damage *= ent.Comp.DamageMultiplier;
    }
  }

  private void RefreshGunHolderModifiers(Entity<GunGroupPenaltyComponent> gun)
  {
    this._gun.RefreshModifiers((Entity<GunComponent>) gun.Owner);
    Entity<HandsComponent> user;
    if (!this._rmcGun.TryGetGunUser((EntityUid) gun, out user))
      return;
    foreach (EntityUid gun1 in this._hands.EnumerateHeld((Entity<HandsComponent>) ((EntityUid) user, (HandsComponent) user)))
    {
      if (gun1 != gun.Owner)
        this._gun.RefreshModifiers((Entity<GunComponent>) gun1);
    }
  }
}
