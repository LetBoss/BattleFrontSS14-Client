// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Loadout.PubgWeaponModuleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Loadout;

[RegisterComponent]
public sealed class PubgWeaponModuleComponent : 
  Component,
  ISerializationGenerated<PubgWeaponModuleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public PubgModuleSlotType Slot;
  [DataField(null, false, 1, false, false, null)]
  public float SpreadMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float FocusBonusTiles;
  [DataField(null, false, 1, false, false, null)]
  public float RecoilMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float HipfireSpreadMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float ReloadTimeMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public int MagazineCapacityBonus;
  [DataField(null, false, 1, false, false, null)]
  public float RangeMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SoundGunshotOverride;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? SpatialFarSoundOverride;
  [DataField(null, false, 1, false, false, null)]
  public bool DisableSpatialFarSound;
  [DataField(null, false, 1, false, false, null)]
  public float SpatialAudioRangeMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float SpatialNearRangeMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float SpatialConeAngleMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float SpatialNearVolumeMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public bool SuppressMuzzleFlash;
  [DataField(null, false, 1, false, false, null)]
  public string UiCategoryLocKey = "pubg-loadout-module-category-generic";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? AttachSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Attachable/attachment_add.ogg", new AudioParams?(AudioParams.Default.WithVolume(-6.5f)));
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? DetachSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Attachable/attachment_remove.ogg", new AudioParams?(AudioParams.Default.WithVolume(-5.5f)));

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgWeaponModuleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgWeaponModuleComponent) target1;
    if (serialization.TryCustomCopy<PubgWeaponModuleComponent>(this, ref target, hookCtx, false, context))
      return;
    PubgModuleSlotType target2 = PubgModuleSlotType.Optic;
    if (!serialization.TryCustomCopy<PubgModuleSlotType>(this.Slot, ref target2, hookCtx, false, context))
      target2 = this.Slot;
    target.Slot = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpreadMultiplier, ref target3, hookCtx, false, context))
      target3 = this.SpreadMultiplier;
    target.SpreadMultiplier = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FocusBonusTiles, ref target4, hookCtx, false, context))
      target4 = this.FocusBonusTiles;
    target.FocusBonusTiles = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RecoilMultiplier, ref target5, hookCtx, false, context))
      target5 = this.RecoilMultiplier;
    target.RecoilMultiplier = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HipfireSpreadMultiplier, ref target6, hookCtx, false, context))
      target6 = this.HipfireSpreadMultiplier;
    target.HipfireSpreadMultiplier = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReloadTimeMultiplier, ref target7, hookCtx, false, context))
      target7 = this.ReloadTimeMultiplier;
    target.ReloadTimeMultiplier = target7;
    int target8 = 0;
    if (!serialization.TryCustomCopy<int>(this.MagazineCapacityBonus, ref target8, hookCtx, false, context))
      target8 = this.MagazineCapacityBonus;
    target.MagazineCapacityBonus = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RangeMultiplier, ref target9, hookCtx, false, context))
      target9 = this.RangeMultiplier;
    target.RangeMultiplier = target9;
    SoundSpecifier target10 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundGunshotOverride, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<SoundSpecifier>(this.SoundGunshotOverride, hookCtx, context);
    target.SoundGunshotOverride = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SpatialFarSoundOverride, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.SpatialFarSoundOverride, hookCtx, context);
    target.SpatialFarSoundOverride = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.DisableSpatialFarSound, ref target12, hookCtx, false, context))
      target12 = this.DisableSpatialFarSound;
    target.DisableSpatialFarSound = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpatialAudioRangeMultiplier, ref target13, hookCtx, false, context))
      target13 = this.SpatialAudioRangeMultiplier;
    target.SpatialAudioRangeMultiplier = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpatialNearRangeMultiplier, ref target14, hookCtx, false, context))
      target14 = this.SpatialNearRangeMultiplier;
    target.SpatialNearRangeMultiplier = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpatialConeAngleMultiplier, ref target15, hookCtx, false, context))
      target15 = this.SpatialConeAngleMultiplier;
    target.SpatialConeAngleMultiplier = target15;
    float target16 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpatialNearVolumeMultiplier, ref target16, hookCtx, false, context))
      target16 = this.SpatialNearVolumeMultiplier;
    target.SpatialNearVolumeMultiplier = target16;
    bool target17 = false;
    if (!serialization.TryCustomCopy<bool>(this.SuppressMuzzleFlash, ref target17, hookCtx, false, context))
      target17 = this.SuppressMuzzleFlash;
    target.SuppressMuzzleFlash = target17;
    string target18 = (string) null;
    if (this.UiCategoryLocKey == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.UiCategoryLocKey, ref target18, hookCtx, false, context))
      target18 = this.UiCategoryLocKey;
    target.UiCategoryLocKey = target18;
    SoundSpecifier target19 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.AttachSound, ref target19, hookCtx, true, context))
      target19 = serialization.CreateCopy<SoundSpecifier>(this.AttachSound, hookCtx, context);
    target.AttachSound = target19;
    SoundSpecifier target20 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.DetachSound, ref target20, hookCtx, true, context))
      target20 = serialization.CreateCopy<SoundSpecifier>(this.DetachSound, hookCtx, context);
    target.DetachSound = target20;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgWeaponModuleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PubgWeaponModuleComponent target1 = (PubgWeaponModuleComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PubgWeaponModuleComponent target1 = (PubgWeaponModuleComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PubgWeaponModuleComponent target1 = (PubgWeaponModuleComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PubgWeaponModuleComponent Component.Instantiate() => new PubgWeaponModuleComponent();
}
