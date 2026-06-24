// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.ExplosionReactionEffect
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Database;
using Content.Shared.Explosion;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

[DataDefinition]
public sealed class ExplosionReactionEffect : 
  EventEntityEffect<ExplosionReactionEffect>,
  ISerializationGenerated<ExplosionReactionEffect>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, typeof (PrototypeIdSerializer<ExplosionPrototype>))]
  public string ExplosionType;
  [DataField(null, false, 1, false, false, null)]
  public float MaxIntensity = 5f;
  [DataField(null, false, 1, false, false, null)]
  public float IntensitySlope = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float MaxTotalIntensity = 100f;
  [DataField(null, false, 1, false, false, null)]
  public float IntensityPerUnit = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float TileBreakScale = 1f;

  public override bool ShouldLog => true;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-explosion-reaction-effect", ("chance", (object) this.Probability));
  }

  public override LogImpact LogImpact => LogImpact.High;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ExplosionReactionEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<ExplosionReactionEffect> target1 = (EventEntityEffect<ExplosionReactionEffect>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ExplosionReactionEffect) target1;
    if (serialization.TryCustomCopy<ExplosionReactionEffect>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.ExplosionType == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ExplosionType, ref target2, hookCtx, false, context))
      target2 = this.ExplosionType;
    target.ExplosionType = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxIntensity, ref target3, hookCtx, false, context))
      target3 = this.MaxIntensity;
    target.MaxIntensity = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.IntensitySlope, ref target4, hookCtx, false, context))
      target4 = this.IntensitySlope;
    target.IntensitySlope = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxTotalIntensity, ref target5, hookCtx, false, context))
      target5 = this.MaxTotalIntensity;
    target.MaxTotalIntensity = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.IntensityPerUnit, ref target6, hookCtx, false, context))
      target6 = this.IntensityPerUnit;
    target.IntensityPerUnit = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TileBreakScale, ref target7, hookCtx, false, context))
      target7 = this.TileBreakScale;
    target.TileBreakScale = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ExplosionReactionEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<ExplosionReactionEffect> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ExplosionReactionEffect target1 = (ExplosionReactionEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<ExplosionReactionEffect>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ExplosionReactionEffect target1 = (ExplosionReactionEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ExplosionReactionEffect EventEntityEffect<ExplosionReactionEffect>.Instantiate()
  {
    return new ExplosionReactionEffect();
  }
}
