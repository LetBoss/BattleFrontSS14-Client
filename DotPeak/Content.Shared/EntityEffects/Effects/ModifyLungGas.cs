// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.ModifyLungGas
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class ModifyLungGas : 
  EventEntityEffect<ModifyLungGas>,
  ISerializationGenerated<ModifyLungGas>,
  ISerializationGenerated
{
  [DataField("ratios", false, 1, true, false, null)]
  public Dictionary<Gas, float> Ratios;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return (string) null;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ModifyLungGas target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<ModifyLungGas> target1 = (EventEntityEffect<ModifyLungGas>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ModifyLungGas) target1;
    if (serialization.TryCustomCopy<ModifyLungGas>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<Gas, float> target2 = (Dictionary<Gas, float>) null;
    if (this.Ratios == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<Gas, float>>(this.Ratios, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<Gas, float>>(this.Ratios, hookCtx, context);
    target.Ratios = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ModifyLungGas target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<ModifyLungGas> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ModifyLungGas target1 = (ModifyLungGas) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<ModifyLungGas>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ModifyLungGas target1 = (ModifyLungGas) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ModifyLungGas EventEntityEffect<ModifyLungGas>.Instantiate() => new ModifyLungGas();
}
