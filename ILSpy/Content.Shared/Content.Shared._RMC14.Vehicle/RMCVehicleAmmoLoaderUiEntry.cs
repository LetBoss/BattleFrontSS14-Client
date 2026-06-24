using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public sealed class RMCVehicleAmmoLoaderUiEntry
{
	public readonly string SlotId;

	public readonly string HardpointType;

	public readonly string? InstalledName;

	public readonly NetEntity? InstalledEntity;

	public readonly int ChamberedRounds;

	public readonly int MagazineSize;

	public readonly int StoredMagazines;

	public readonly int MaxStoredMagazines;

	public readonly bool CanLoad;

	public readonly string? LoadedAmmoType;

	public RMCVehicleAmmoLoaderUiEntry(string slotId, string hardpointType, string? installedName, NetEntity? installedEntity, int chamberedRounds, int magazineSize, int storedMagazines, int maxStoredMagazines, bool canLoad, string? loadedAmmoType = null)
	{
		SlotId = slotId;
		HardpointType = hardpointType;
		InstalledName = installedName;
		InstalledEntity = installedEntity;
		ChamberedRounds = chamberedRounds;
		MagazineSize = magazineSize;
		StoredMagazines = storedMagazines;
		MaxStoredMagazines = maxStoredMagazines;
		CanLoad = canLoad;
		LoadedAmmoType = loadedAmmoType;
	}
}
