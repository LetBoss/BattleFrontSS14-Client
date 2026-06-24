// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.EffectConditions.Temperature
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class Temperature : 
  EventEntityEffectCondition<Temperature>,
  ISerializationGenerated<Temperature>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Min;
  [DataField(null, false, 1, false, false, null)]
  public float Max = float.PositiveInfinity;

  public override string GuidebookExplanation(IPrototypeManager prototype)
  {
    return Loc.GetString("reagent-effect-condition-guidebook-body-temperature", ("max", (object) (float) (float.IsPositiveInfinity(this.Max) ? 2147483648.0 : (double) this.Max)), ("min", (object) this.Min));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Temperature target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffectCondition<Temperature> target1 = (EventEntityEffectCondition<Temperature>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Temperature) target1;
    if (serialization.TryCustomCopy<Temperature>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Min, ref target2, hookCtx, false, context))
      target2 = this.Min;
    target.Min = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Max, ref target3, hookCtx, false, context))
      target3 = this.Max;
    target.Max = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Temperature target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffectCondition<Temperature> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Temperature target1 = (Temperature) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffectCondition<Temperature>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Temperature target1 = (Temperature) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Temperature EventEntityEffectCondition<Temperature>.Instantiate() => new Temperature();
}
