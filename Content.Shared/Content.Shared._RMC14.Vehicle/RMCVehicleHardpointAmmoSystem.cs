using System;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Vehicle;

public sealed class RMCVehicleHardpointAmmoSystem : EntitySystem
{
	[Dependency]
	private SharedGunSystem _gun;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCVehicleHardpointAmmoComponent, AmmoShotEvent>((EntityEventRefHandler<RMCVehicleHardpointAmmoComponent, AmmoShotEvent>)OnAmmoShot, (Type[])null, (Type[])null);
	}

	private void OnAmmoShot(Entity<RMCVehicleHardpointAmmoComponent> ent, ref AmmoShotEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		BallisticAmmoProviderComponent ammo = default(BallisticAmmoProviderComponent);
		if (((EntitySystem)this).TryComp<BallisticAmmoProviderComponent>(ent.Owner, ref ammo) && ammo.Count <= 0)
		{
			TryChamberNextMagazine(ent, ammo);
		}
	}

	public bool TryChamberNextMagazine(Entity<RMCVehicleHardpointAmmoComponent> ent, BallisticAmmoProviderComponent ammo)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.StoredMagazines <= 0)
		{
			return false;
		}
		int chamberSize = Math.Min(Math.Max(1, ent.Comp.MagazineSize), ammo.Capacity);
		ent.Comp.StoredMagazines--;
		if (ent.Comp.MagazineProjectileQueue.Count > 0)
		{
			EntProtoId projectileProto = ent.Comp.MagazineProjectileQueue.Dequeue();
			ammo.Proto = projectileProto;
			((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)ammo, (MetaDataComponent)null);
		}
		((EntitySystem)this).Dirty<RMCVehicleHardpointAmmoComponent>(ent, (MetaDataComponent)null);
		_gun.SetBallisticUnspawned(Entity<BallisticAmmoProviderComponent>.op_Implicit((ent.Owner, ammo)), chamberSize);
		return true;
	}
}
