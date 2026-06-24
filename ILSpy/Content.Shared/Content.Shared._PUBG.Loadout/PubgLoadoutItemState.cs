using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Loadout;

[Serializable]
[NetSerializable]
public sealed class PubgLoadoutItemState
{
	public NetEntity Entity { get; }

	public string Name { get; }

	public string PrototypeId { get; }

	public PubgLoadoutItemSourceType SourceType { get; }

	public string SourcePrimary { get; }

	public string SourceSecondary { get; }

	public bool IsWeapon { get; }

	public bool IsModule { get; }

	public PubgModuleSlotType ModuleSlotType { get; }

	public float ModuleSpreadMultiplier { get; }

	public float ModuleFocusBonusTiles { get; }

	public float ModuleRecoilMultiplier { get; }

	public float ModuleHipfireSpreadMultiplier { get; }

	public float ModuleReloadTimeMultiplier { get; }

	public int ModuleMagazineCapacityBonus { get; }

	public bool ModuleHasSoundOverride { get; }

	public bool ModuleDisableSpatialFarSound { get; }

	public float ModuleSpatialAudioRangeMultiplier { get; }

	public float ModuleSpatialNearRangeMultiplier { get; }

	public float ModuleSpatialNearVolumeMultiplier { get; }

	public bool ModuleSuppressesMuzzleFlash { get; }

	public string ModuleCategoryLocKey { get; }

	public bool HasAmmoInfo { get; }

	public int AmmoCount { get; }

	public int AmmoCapacity { get; }

	public int StackCount { get; }

	public PubgLoadoutSection Section { get; }

	public PubgLoadoutItemState(NetEntity entity, string name, string prototypeId, PubgLoadoutItemSourceType sourceType, string sourcePrimary, string sourceSecondary, bool isWeapon, bool isModule, PubgModuleSlotType moduleSlotType, float moduleSpreadMultiplier, float moduleFocusBonusTiles, float moduleRecoilMultiplier, float moduleHipfireSpreadMultiplier, float moduleReloadTimeMultiplier, int moduleMagazineCapacityBonus, bool moduleHasSoundOverride, bool moduleDisableSpatialFarSound, float moduleSpatialAudioRangeMultiplier, float moduleSpatialNearRangeMultiplier, float moduleSpatialNearVolumeMultiplier, bool moduleSuppressesMuzzleFlash, string moduleCategoryLocKey, bool hasAmmoInfo, int ammoCount, int ammoCapacity, int stackCount, PubgLoadoutSection section)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Entity = entity;
		Name = name;
		PrototypeId = prototypeId;
		SourceType = sourceType;
		SourcePrimary = sourcePrimary;
		SourceSecondary = sourceSecondary;
		IsWeapon = isWeapon;
		IsModule = isModule;
		ModuleSlotType = moduleSlotType;
		ModuleSpreadMultiplier = moduleSpreadMultiplier;
		ModuleFocusBonusTiles = moduleFocusBonusTiles;
		ModuleRecoilMultiplier = moduleRecoilMultiplier;
		ModuleHipfireSpreadMultiplier = moduleHipfireSpreadMultiplier;
		ModuleReloadTimeMultiplier = moduleReloadTimeMultiplier;
		ModuleMagazineCapacityBonus = moduleMagazineCapacityBonus;
		ModuleHasSoundOverride = moduleHasSoundOverride;
		ModuleDisableSpatialFarSound = moduleDisableSpatialFarSound;
		ModuleSpatialAudioRangeMultiplier = moduleSpatialAudioRangeMultiplier;
		ModuleSpatialNearRangeMultiplier = moduleSpatialNearRangeMultiplier;
		ModuleSpatialNearVolumeMultiplier = moduleSpatialNearVolumeMultiplier;
		ModuleSuppressesMuzzleFlash = moduleSuppressesMuzzleFlash;
		ModuleCategoryLocKey = moduleCategoryLocKey;
		HasAmmoInfo = hasAmmoInfo;
		AmmoCount = ammoCount;
		AmmoCapacity = ammoCapacity;
		StackCount = stackCount;
		Section = section;
	}
}
