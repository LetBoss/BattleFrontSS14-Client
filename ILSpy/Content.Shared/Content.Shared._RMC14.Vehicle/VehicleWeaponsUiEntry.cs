using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public sealed class VehicleWeaponsUiEntry
{
	public readonly string SlotId;

	public readonly string HardpointType;

	public readonly NetEntity? MountedEntity;

	public readonly string? InstalledName;

	public readonly NetEntity? InstalledEntity;

	public readonly bool HasItem;

	public readonly bool Selectable;

	public readonly bool Selected;

	public readonly int AmmoCount;

	public readonly int AmmoCapacity;

	public readonly bool HasAmmo;

	public readonly int MagazineSize;

	public readonly int StoredMagazines;

	public readonly int MaxStoredMagazines;

	public readonly bool HasMagazineData;

	public readonly string? OperatorName;

	public readonly bool OperatorIsSelf;

	public readonly float Integrity;

	public readonly float MaxIntegrity;

	public readonly bool HasIntegrity;

	public readonly float CooldownRemaining;

	public readonly float CooldownTotal;

	public readonly bool IsOnCooldown;

	public VehicleWeaponsUiEntry(string slotId, string hardpointType, NetEntity? mountedEntity, string? installedName, NetEntity? installedEntity, bool hasItem, bool selectable, bool selected, int ammoCount, int ammoCapacity, bool hasAmmo, int magazineSize, int storedMagazines, int maxStoredMagazines, bool hasMagazineData, string? operatorName, bool operatorIsSelf, float integrity, float maxIntegrity, bool hasIntegrity, float cooldownRemaining, float cooldownTotal, bool isOnCooldown)
	{
		SlotId = slotId;
		HardpointType = hardpointType;
		MountedEntity = mountedEntity;
		InstalledName = installedName;
		InstalledEntity = installedEntity;
		HasItem = hasItem;
		Selectable = selectable;
		Selected = selected;
		AmmoCount = ammoCount;
		AmmoCapacity = ammoCapacity;
		HasAmmo = hasAmmo;
		MagazineSize = magazineSize;
		StoredMagazines = storedMagazines;
		MaxStoredMagazines = maxStoredMagazines;
		HasMagazineData = hasMagazineData;
		OperatorName = operatorName;
		OperatorIsSelf = operatorIsSelf;
		Integrity = integrity;
		MaxIntegrity = maxIntegrity;
		HasIntegrity = hasIntegrity;
		CooldownRemaining = cooldownRemaining;
		CooldownTotal = cooldownTotal;
		IsOnCooldown = isOnCooldown;
	}
}
