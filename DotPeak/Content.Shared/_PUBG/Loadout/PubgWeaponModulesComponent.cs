// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Loadout.PubgWeaponModulesComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._PUBG.Loadout;

[RegisterComponent]
public sealed class PubgWeaponModulesComponent : 
  Component,
  ISerializationGenerated<PubgWeaponModulesComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<PubgWeaponModuleSlotDefinition> Slots = new List<PubgWeaponModuleSlotDefinition>();
  [DataField(null, false, 1, false, false, null)]
  public string FlashlightToggleAction = "ActionTogglePubgWeaponLight";
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? FlashlightToggleActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public string BipodToggleAction = "ActionTogglePubgBipod";
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? BipodToggleActionEntity;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgWeaponModulesComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgWeaponModulesComponent) target1;
    if (serialization.TryCustomCopy<PubgWeaponModulesComponent>(this, ref target, hookCtx, false, context))
      return;
    List<PubgWeaponModuleSlotDefinition> target2 = (List<PubgWeaponModuleSlotDefinition>) null;
    if (this.Slots == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<PubgWeaponModuleSlotDefinition>>(this.Slots, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<PubgWeaponModuleSlotDefinition>>(this.Slots, hookCtx, context);
    target.Slots = target2;
    string target3 = (string) null;
    if (this.FlashlightToggleAction == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.FlashlightToggleAction, ref target3, hookCtx, false, context))
      target3 = this.FlashlightToggleAction;
    target.FlashlightToggleAction = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.FlashlightToggleActionEntity, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.FlashlightToggleActionEntity, hookCtx, context);
    target.FlashlightToggleActionEntity = target4;
    string target5 = (string) null;
    if (this.BipodToggleAction == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BipodToggleAction, ref target5, hookCtx, false, context))
      target5 = this.BipodToggleAction;
    target.BipodToggleAction = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.BipodToggleActionEntity, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.BipodToggleActionEntity, hookCtx, context);
    target.BipodToggleActionEntity = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgWeaponModulesComponent target,
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
    PubgWeaponModulesComponent target1 = (PubgWeaponModulesComponent) target;
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
    PubgWeaponModulesComponent target1 = (PubgWeaponModulesComponent) target;
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
    PubgWeaponModulesComponent target1 = (PubgWeaponModulesComponent) target;
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
  virtual PubgWeaponModulesComponent Component.Instantiate() => new PubgWeaponModulesComponent();
}
