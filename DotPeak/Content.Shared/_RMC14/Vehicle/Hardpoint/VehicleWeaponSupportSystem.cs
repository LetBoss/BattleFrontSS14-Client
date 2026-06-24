// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleWeaponSupportSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleWeaponSupportSystem : EntitySystem
{
  [Dependency]
  private readonly VehicleTopologySystem _topology;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<GunComponent, GunRefreshModifiersEvent>(new EntityEventRefHandler<GunComponent, GunRefreshModifiersEvent>(this.OnGunRefresh));
    this.SubscribeLocalEvent<GunComponent, GetWeaponAccuracyEvent>(new EntityEventRefHandler<GunComponent, GetWeaponAccuracyEvent>(this.OnGetAccuracy));
  }

  private void OnGunRefresh(Entity<GunComponent> ent, ref GunRefreshModifiersEvent args)
  {
    EntityUid vehicle;
    VehicleWeaponSupportModifierComponent comp;
    if (!this._topology.TryGetVehicle(ent.Owner, out vehicle) || !this.TryComp<VehicleWeaponSupportModifierComponent>(vehicle, out comp))
      return;
    args.FireRate *= comp.FireRateMultiplier;
  }

  private void OnGetAccuracy(Entity<GunComponent> ent, ref GetWeaponAccuracyEvent args)
  {
    EntityUid vehicle;
    VehicleWeaponSupportModifierComponent comp;
    if (!this._topology.TryGetVehicle(ent.Owner, out vehicle) || !this.TryComp<VehicleWeaponSupportModifierComponent>(vehicle, out comp))
      return;
    args.AccuracyMultiplier *= comp.AccuracyMultiplier;
  }
}
