// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.ChemCleanBloodstream
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class ChemCleanBloodstream : 
  EventEntityEffect<ChemCleanBloodstream>,
  ISerializationGenerated<ChemCleanBloodstream>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float CleanseRate = 3f;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-chem-clean-bloodstream", ("chance", (object) this.Probability));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ChemCleanBloodstream target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<ChemCleanBloodstream> target1 = (EventEntityEffect<ChemCleanBloodstream>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ChemCleanBloodstream) target1;
    if (serialization.TryCustomCopy<ChemCleanBloodstream>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CleanseRate, ref target2, hookCtx, false, context))
      target2 = this.CleanseRate;
    target.CleanseRate = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ChemCleanBloodstream target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<ChemCleanBloodstream> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ChemCleanBloodstream target1 = (ChemCleanBloodstream) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<ChemCleanBloodstream>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ChemCleanBloodstream target1 = (ChemCleanBloodstream) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ChemCleanBloodstream EventEntityEffect<ChemCleanBloodstream>.Instantiate()
  {
    return new ChemCleanBloodstream();
  }
}
