// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.AdjustTemperature
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

public sealed class AdjustTemperature : 
  EventEntityEffect<AdjustTemperature>,
  ISerializationGenerated<AdjustTemperature>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Amount;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-adjust-temperature", ("chance", (object) this.Probability), ("deltasign", (object) MathF.Sign(this.Amount)), ("amount", (object) MathF.Abs(this.Amount)));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AdjustTemperature target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<AdjustTemperature> target1 = (EventEntityEffect<AdjustTemperature>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AdjustTemperature) target1;
    if (serialization.TryCustomCopy<AdjustTemperature>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Amount, ref target2, hookCtx, false, context))
      target2 = this.Amount;
    target.Amount = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AdjustTemperature target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<AdjustTemperature> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AdjustTemperature target1 = (AdjustTemperature) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<AdjustTemperature>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    AdjustTemperature target1 = (AdjustTemperature) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual AdjustTemperature EventEntityEffect<AdjustTemperature>.Instantiate()
  {
    return new AdjustTemperature();
  }
}
