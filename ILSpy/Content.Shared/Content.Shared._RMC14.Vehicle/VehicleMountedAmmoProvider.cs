using Content.Shared._RMC14.Weapons.Ranged.Ammo.BulletBox;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Vehicle;

public readonly record struct VehicleMountedAmmoProvider(VehicleMountedSlot Slot, EntityUid AmmoUid, BallisticAmmoProviderComponent Ammo, VehicleHardpointAmmoComponent HardpointAmmo, RefillableByBulletBoxComponent Refill);
