// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Stacks.GunStacksSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Stacks;

public sealed class GunStacksSystem : EntitySystem
{
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private CMArmorSystem _rmcArmor;
  [Dependency]
  private CMGunSystem _rmcGun;
  [Dependency]
  private RMCSelectiveFireSystem _rmcSelectiveFire;
  [Dependency]
  private IGameTiming _timing;
  private Robust.Shared.GameObjects.EntityQuery<GunStacksComponent> _gunStacksQuery;
  private Robust.Shared.GameObjects.EntityQuery<RMCSelectiveFireComponent> _selectiveFireQuery;
  private Robust.Shared.GameObjects.EntityQuery<XenoComponent> _xenoQuery;

  public override void Initialize()
  {
    this._gunStacksQuery = this.GetEntityQuery<GunStacksComponent>();
    this._selectiveFireQuery = this.GetEntityQuery<RMCSelectiveFireComponent>();
    this._xenoQuery = this.GetEntityQuery<XenoComponent>();
    this.SubscribeLocalEvent<GunStacksComponent, AmmoShotEvent>(new EntityEventRefHandler<GunStacksComponent, AmmoShotEvent>(this.OnStacksAmmoShot));
    this.SubscribeLocalEvent<GunStacksActiveComponent, GetGunDamageModifierEvent>(new EntityEventRefHandler<GunStacksActiveComponent, GetGunDamageModifierEvent>(this.OnStacksActiveGetGunDamageModifier));
    this.SubscribeLocalEvent<GunStacksActiveComponent, GunGetFireRateEvent>(new EntityEventRefHandler<GunStacksActiveComponent, GunGetFireRateEvent>(this.OnStacksActiveGetGunFireRate));
    this.SubscribeLocalEvent<GunStacksActiveComponent, DroppedEvent>(new EntityEventRefHandler<GunStacksActiveComponent, DroppedEvent>(this.OnStacksActiveDropped));
    this.SubscribeLocalEvent<GunStacksProjectileComponent, ProjectileHitEvent>(new EntityEventRefHandler<GunStacksProjectileComponent, ProjectileHitEvent>(this.OnStacksProjectileHit));
  }

  private void OnStacksAmmoShot(Entity<GunStacksComponent> ent, ref AmmoShotEvent args)
  {
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
    {
      GunStacksProjectileComponent projectileComponent = this.EnsureComp<GunStacksProjectileComponent>(firedProjectile);
      projectileComponent.Gun = new EntityUid?((EntityUid) ent);
      this.Dirty(firedProjectile, (IComponent) projectileComponent);
      CMArmorPiercingComponent piercingComponent = this.EnsureComp<CMArmorPiercingComponent>(firedProjectile);
      int num = 0;
      GunStacksActiveComponent comp;
      if (this.TryComp<GunStacksActiveComponent>((EntityUid) ent, out comp))
        num = comp.Hits;
      int amount = Math.Min(ent.Comp.MaxAP, ent.Comp.IncreaseAP * num);
      this._rmcArmor.SetArmorPiercing((Entity<CMArmorPiercingComponent>) (firedProjectile, piercingComponent), amount);
    }
  }

  private void OnStacksActiveGetGunDamageModifier(
    Entity<GunStacksActiveComponent> ent,
    ref GetGunDamageModifierEvent args)
  {
    GunStacksComponent comp;
    if (!this.TryComp<GunStacksComponent>((EntityUid) ent, out comp) || ent.Comp.Hits <= 0)
      return;
    args.Multiplier += comp.DamageIncrease;
  }

  private void OnStacksActiveGetGunFireRate(
    Entity<GunStacksActiveComponent> ent,
    ref GunGetFireRateEvent args)
  {
    GunStacksComponent comp;
    if (!this.TryComp<GunStacksComponent>((EntityUid) ent, out comp) || ent.Comp.Hits <= 0)
      return;
    args.FireRate = comp.SetFireRate;
  }

  private void OnStacksActiveDropped(Entity<GunStacksActiveComponent> ent, ref DroppedEvent args)
  {
    this.Reset(ent);
  }

  private void OnStacksProjectileHit(
    Entity<GunStacksProjectileComponent> ent,
    ref ProjectileHitEvent args)
  {
    ProjectileComponent comp1;
    if (!ent.Comp.Gun.HasValue || !this._gunStacksQuery.HasComp(ent.Comp.Gun) || this.TryComp<ProjectileComponent>((EntityUid) ent, out comp1) && comp1.ProjectileSpent)
      return;
    EntityUid target = args.Target;
    if (this._xenoQuery.HasComp(target) && !this._mobState.IsDead(target))
    {
      GunStacksActiveComponent comp2;
      if (!this.TryComp<GunStacksActiveComponent>(ent.Comp.Gun, out comp2))
        comp2 = this.EnsureComp<GunStacksActiveComponent>(ent.Comp.Gun.Value);
      ++comp2.Hits;
      comp2.ExpireAt = this._timing.CurTime + comp2.StacksExpire;
      EntityUid? shooter = args.Shooter;
      if (shooter.HasValue)
      {
        EntityUid valueOrDefault = shooter.GetValueOrDefault();
        if (this._net.IsServer)
          this._popup.PopupEntity(comp2.Hits == 1 ? this.Loc.GetString("rmc-gun-stacks-hit-single") : this.Loc.GetString("rmc-gun-stacks-hit-multiple", ("hits", (object) comp2.Hits)), valueOrDefault, valueOrDefault);
      }
    }
    this.RefreshGunStats(ent.Comp.Gun.Value);
    this.Dirty<GunStacksProjectileComponent>(ent);
  }

  private void Reset(Entity<GunStacksActiveComponent> gun)
  {
    this.RemComp<GunStacksActiveComponent>(gun.Owner);
    if (this._net.IsServer)
      this._popup.PopupEntity(this.Loc.GetString("rmc-gun-stacks-reset", ("weapon", (object) gun.Owner)), (EntityUid) gun, PopupType.SmallCaution);
    this.RefreshGunStats(gun.Owner);
  }

  private void RefreshGunStats(EntityUid gun)
  {
    this._rmcGun.RefreshGunDamageMultiplier((Entity<GunDamageModifierComponent>) gun);
    RMCSelectiveFireComponent component;
    if (!this._selectiveFireQuery.TryComp(gun, out component))
      return;
    this._rmcSelectiveFire.RefreshFireModeGunValues((Entity<RMCSelectiveFireComponent>) (gun, component));
    this.Dirty(gun, (IComponent) component);
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<GunStacksActiveComponent> entityQueryEnumerator = this.EntityQueryEnumerator<GunStacksActiveComponent>();
    EntityUid uid;
    GunStacksActiveComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(comp1.ExpireAt > curTime))
        this.Reset((Entity<GunStacksActiveComponent>) (uid, comp1));
    }
  }
}
