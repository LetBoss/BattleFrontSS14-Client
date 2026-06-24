using System;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleWeaponSupportSystem : EntitySystem
{
	[Dependency]
	private readonly VehicleTopologySystem _topology;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<GunComponent, GunRefreshModifiersEvent>((EntityEventRefHandler<GunComponent, GunRefreshModifiersEvent>)OnGunRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GunComponent, GetWeaponAccuracyEvent>((EntityEventRefHandler<GunComponent, GetWeaponAccuracyEvent>)OnGetAccuracy, (Type[])null, (Type[])null);
	}

	private void OnGunRefresh(Entity<GunComponent> ent, ref GunRefreshModifiersEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		VehicleWeaponSupportModifierComponent mods = default(VehicleWeaponSupportModifierComponent);
		if (_topology.TryGetVehicle(ent.Owner, out var vehicle) && ((EntitySystem)this).TryComp<VehicleWeaponSupportModifierComponent>(vehicle, ref mods))
		{
			args.FireRate *= mods.FireRateMultiplier;
		}
	}

	private void OnGetAccuracy(Entity<GunComponent> ent, ref GetWeaponAccuracyEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		VehicleWeaponSupportModifierComponent mods = default(VehicleWeaponSupportModifierComponent);
		if (_topology.TryGetVehicle(ent.Owner, out var vehicle) && ((EntitySystem)this).TryComp<VehicleWeaponSupportModifierComponent>(vehicle, ref mods))
		{
			args.AccuracyMultiplier *= mods.AccuracyMultiplier;
		}
	}
}
