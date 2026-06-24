// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.StatusEffects.Jitter
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Jittering;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects.StatusEffects;

public sealed class Jitter : EntityEffect, ISerializationGenerated<Jitter>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Amplitude = 10f;
  [DataField(null, false, 1, false, false, null)]
  public float Frequency = 4f;
  [DataField(null, false, 1, false, false, null)]
  public float Time = 2f;
  [DataField(null, false, 1, false, false, null)]
  public bool Refresh = true;

  public override void Effect(EntityEffectBaseArgs args)
  {
    float time = this.Time;
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs != null)
      time *= effectReagentArgs.Scale.Float();
    args.EntityManager.EntitySysManager.GetEntitySystem<SharedJitteringSystem>().DoJitter(args.TargetEntity, TimeSpan.FromSeconds((double) time), this.Refresh, this.Amplitude, this.Frequency);
  }

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-jittering", ("chance", (object) this.Probability));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Jitter target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Jitter) target1;
    if (serialization.TryCustomCopy<Jitter>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Amplitude, ref target2, hookCtx, false, context))
      target2 = this.Amplitude;
    target.Amplitude = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Frequency, ref target3, hookCtx, false, context))
      target3 = this.Frequency;
    target.Frequency = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Time, ref target4, hookCtx, false, context))
      target4 = this.Time;
    target.Time = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Refresh, ref target5, hookCtx, false, context))
      target5 = this.Refresh;
    target.Refresh = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Jitter target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Jitter target1 = (Jitter) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityEffect) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Jitter target1 = (Jitter) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Jitter EntityEffect.Instantiate() => new Jitter();
}
