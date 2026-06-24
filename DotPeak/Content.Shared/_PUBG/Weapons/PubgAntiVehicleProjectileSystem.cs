// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Weapons.PubgAntiVehicleProjectileSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Projectiles;
using Content.Shared._RMC14.Vehicle;
using Content.Shared.Projectiles;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;

#nullable enable
namespace Content.Shared._PUBG.Weapons;

public sealed class PubgAntiVehicleProjectileSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private HardpointSystem _hardpoints;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PubgAntiVehicleProjectileComponent, ProjectileHitEvent>(new EntityEventRefHandler<PubgAntiVehicleProjectileComponent, ProjectileHitEvent>(this.OnProjectileHit), after: new Type[1]
    {
      typeof (RMCProjectileSystem)
    });
  }

  private void OnProjectileHit(
    Entity<PubgAntiVehicleProjectileComponent> ent,
    ref ProjectileHitEvent args)
  {
    EntityUid vehicle;
    HardpointIntegrityComponent frame;
    if (!this.TryGetVehicle(args.Target, out vehicle, out frame))
      return;
    if ((double) ent.Comp.VehicleDamageMultiplier > 0.0 && (double) MathF.Abs(ent.Comp.VehicleDamageMultiplier - 1f) > 9.9999997473787516E-05)
      args.Damage *= ent.Comp.VehicleDamageMultiplier;
    if (this._net.IsClient || (double) ent.Comp.BonusFrameDamageFraction <= 0.0)
      return;
    float amount = args.Damage.GetTotal().Float() * ent.Comp.BonusFrameDamageFraction;
    if ((double) amount <= 0.0)
      return;
    this._hardpoints.DamageHardpoint(vehicle, vehicle, amount, frame);
  }

  private bool TryGetVehicle(
    EntityUid uid,
    out EntityUid vehicle,
    out HardpointIntegrityComponent frame)
  {
    vehicle = new EntityUid();
    frame = (HardpointIntegrityComponent) null;
    EntityUid uid1;
    HardpointIntegrityComponent comp;
    BaseContainer container;
    for (uid1 = uid; !this.HasComp<HardpointSlotsComponent>(uid1) || !this.TryComp<HardpointIntegrityComponent>(uid1, out comp); uid1 = container.Owner)
    {
      if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (uid1, (TransformComponent) null), out container))
        return false;
    }
    vehicle = uid1;
    frame = comp;
    return true;
  }
}
