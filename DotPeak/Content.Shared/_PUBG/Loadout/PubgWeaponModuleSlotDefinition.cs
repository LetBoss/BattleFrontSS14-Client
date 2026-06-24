// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Loadout.PubgWeaponModuleSlotDefinition
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Loadout;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class PubgWeaponModuleSlotDefinition : 
  ISerializationGenerated<PubgWeaponModuleSlotDefinition>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public PubgModuleSlotType Slot;
  [DataField(null, false, 1, false, false, null)]
  public PubgModuleUiAnchor UiAnchor = PubgModuleUiAnchor.Top;
  [DataField(null, false, 1, true, false, null)]
  public string DisplayNameLocKey = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public List<EntProtoId> AllowedModules = new List<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  public string? ContainerId;
  [DataField(null, false, 1, false, false, null)]
  public bool RequiredForReloading;
  [DataField(null, false, 1, false, false, null)]
  public bool StoresAmmo;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? DefaultModule;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgWeaponModuleSlotDefinition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<PubgWeaponModuleSlotDefinition>(this, ref target, hookCtx, false, context))
      return;
    PubgModuleSlotType target1 = PubgModuleSlotType.Optic;
    if (!serialization.TryCustomCopy<PubgModuleSlotType>(this.Slot, ref target1, hookCtx, false, context))
      target1 = this.Slot;
    target.Slot = target1;
    PubgModuleUiAnchor target2 = PubgModuleUiAnchor.TopLeft;
    if (!serialization.TryCustomCopy<PubgModuleUiAnchor>(this.UiAnchor, ref target2, hookCtx, false, context))
      target2 = this.UiAnchor;
    target.UiAnchor = target2;
    string target3 = (string) null;
    if (this.DisplayNameLocKey == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DisplayNameLocKey, ref target3, hookCtx, false, context))
      target3 = this.DisplayNameLocKey;
    target.DisplayNameLocKey = target3;
    List<EntProtoId> target4 = (List<EntProtoId>) null;
    if (this.AllowedModules == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.AllowedModules, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<EntProtoId>>(this.AllowedModules, hookCtx, context);
    target.AllowedModules = target4;
    string target5 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref target5, hookCtx, false, context))
      target5 = this.ContainerId;
    target.ContainerId = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.RequiredForReloading, ref target6, hookCtx, false, context))
      target6 = this.RequiredForReloading;
    target.RequiredForReloading = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.StoresAmmo, ref target7, hookCtx, false, context))
      target7 = this.StoresAmmo;
    target.StoresAmmo = target7;
    EntProtoId? target8 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.DefaultModule, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<EntProtoId?>(this.DefaultModule, hookCtx, context);
    target.DefaultModule = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgWeaponModuleSlotDefinition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PubgWeaponModuleSlotDefinition target1 = (PubgWeaponModuleSlotDefinition) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public PubgWeaponModuleSlotDefinition Instantiate() => new PubgWeaponModuleSlotDefinition();
}
