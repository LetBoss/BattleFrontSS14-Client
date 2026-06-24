// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Loadout.PubgLoadoutItemState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Loadout;

[NetSerializable]
[Serializable]
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

  public PubgLoadoutItemState(
    NetEntity entity,
    string name,
    string prototypeId,
    PubgLoadoutItemSourceType sourceType,
    string sourcePrimary,
    string sourceSecondary,
    bool isWeapon,
    bool isModule,
    PubgModuleSlotType moduleSlotType,
    float moduleSpreadMultiplier,
    float moduleFocusBonusTiles,
    float moduleRecoilMultiplier,
    float moduleHipfireSpreadMultiplier,
    float moduleReloadTimeMultiplier,
    int moduleMagazineCapacityBonus,
    bool moduleHasSoundOverride,
    bool moduleDisableSpatialFarSound,
    float moduleSpatialAudioRangeMultiplier,
    float moduleSpatialNearRangeMultiplier,
    float moduleSpatialNearVolumeMultiplier,
    bool moduleSuppressesMuzzleFlash,
    string moduleCategoryLocKey,
    bool hasAmmoInfo,
    int ammoCount,
    int ammoCapacity,
    int stackCount,
    PubgLoadoutSection section)
  {
    this.Entity = entity;
    this.Name = name;
    this.PrototypeId = prototypeId;
    this.SourceType = sourceType;
    this.SourcePrimary = sourcePrimary;
    this.SourceSecondary = sourceSecondary;
    this.IsWeapon = isWeapon;
    this.IsModule = isModule;
    this.ModuleSlotType = moduleSlotType;
    this.ModuleSpreadMultiplier = moduleSpreadMultiplier;
    this.ModuleFocusBonusTiles = moduleFocusBonusTiles;
    this.ModuleRecoilMultiplier = moduleRecoilMultiplier;
    this.ModuleHipfireSpreadMultiplier = moduleHipfireSpreadMultiplier;
    this.ModuleReloadTimeMultiplier = moduleReloadTimeMultiplier;
    this.ModuleMagazineCapacityBonus = moduleMagazineCapacityBonus;
    this.ModuleHasSoundOverride = moduleHasSoundOverride;
    this.ModuleDisableSpatialFarSound = moduleDisableSpatialFarSound;
    this.ModuleSpatialAudioRangeMultiplier = moduleSpatialAudioRangeMultiplier;
    this.ModuleSpatialNearRangeMultiplier = moduleSpatialNearRangeMultiplier;
    this.ModuleSpatialNearVolumeMultiplier = moduleSpatialNearVolumeMultiplier;
    this.ModuleSuppressesMuzzleFlash = moduleSuppressesMuzzleFlash;
    this.ModuleCategoryLocKey = moduleCategoryLocKey;
    this.HasAmmoInfo = hasAmmoInfo;
    this.AmmoCount = ammoCount;
    this.AmmoCapacity = ammoCapacity;
    this.StackCount = stackCount;
    this.Section = section;
  }
}
