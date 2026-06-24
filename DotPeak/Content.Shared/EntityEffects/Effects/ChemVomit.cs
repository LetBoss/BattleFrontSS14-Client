// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.ChemVomit
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

public sealed class ChemVomit : 
  EventEntityEffect<ChemVomit>,
  ISerializationGenerated<ChemVomit>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float ThirstAmount = -8f;
  [DataField(null, false, 1, false, false, null)]
  public float HungerAmount = -8f;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-chem-vomit", ("chance", (object) this.Probability));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ChemVomit target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<ChemVomit> target1 = (EventEntityEffect<ChemVomit>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ChemVomit) target1;
    if (serialization.TryCustomCopy<ChemVomit>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ThirstAmount, ref target2, hookCtx, false, context))
      target2 = this.ThirstAmount;
    target.ThirstAmount = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HungerAmount, ref target3, hookCtx, false, context))
      target3 = this.HungerAmount;
    target.HungerAmount = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ChemVomit target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<ChemVomit> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ChemVomit target1 = (ChemVomit) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<ChemVomit>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ChemVomit target1 = (ChemVomit) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ChemVomit EventEntityEffect<ChemVomit>.Instantiate() => new ChemVomit();
}
