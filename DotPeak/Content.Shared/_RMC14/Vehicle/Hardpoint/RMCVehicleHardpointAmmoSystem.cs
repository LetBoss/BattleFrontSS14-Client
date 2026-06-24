// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.RMCVehicleHardpointAmmoSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class RMCVehicleHardpointAmmoSystem : EntitySystem
{
  [Dependency]
  private SharedGunSystem _gun;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCVehicleHardpointAmmoComponent, AmmoShotEvent>(new EntityEventRefHandler<RMCVehicleHardpointAmmoComponent, AmmoShotEvent>(this.OnAmmoShot));
  }

  private void OnAmmoShot(Entity<RMCVehicleHardpointAmmoComponent> ent, ref AmmoShotEvent args)
  {
    BallisticAmmoProviderComponent comp;
    if (!this.TryComp<BallisticAmmoProviderComponent>(ent.Owner, out comp) || comp.Count > 0)
      return;
    this.TryChamberNextMagazine(ent, comp);
  }

  public bool TryChamberNextMagazine(
    Entity<RMCVehicleHardpointAmmoComponent> ent,
    BallisticAmmoProviderComponent ammo)
  {
    if (ent.Comp.StoredMagazines <= 0)
      return false;
    int count = Math.Min(Math.Max(1, ent.Comp.MagazineSize), ammo.Capacity);
    --ent.Comp.StoredMagazines;
    if (ent.Comp.MagazineProjectileQueue.Count > 0)
    {
      EntProtoId entProtoId = ent.Comp.MagazineProjectileQueue.Dequeue();
      ammo.Proto = new EntProtoId?(entProtoId);
      this.Dirty(ent.Owner, (IComponent) ammo);
    }
    this.Dirty<RMCVehicleHardpointAmmoComponent>(ent);
    this._gun.SetBallisticUnspawned((Entity<BallisticAmmoProviderComponent>) (ent.Owner, ammo), count);
    return true;
  }
}
