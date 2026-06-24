// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.FlammableReaction
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Database;
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

public sealed class FlammableReaction : 
  EventEntityEffect<FlammableReaction>,
  ISerializationGenerated<FlammableReaction>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Multiplier = 0.05f;
  [DataField(null, false, 1, false, false, null)]
  public float MultiplierOnExisting = -1f;

  public override bool ShouldLog => true;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-flammable-reaction", ("chance", (object) this.Probability));
  }

  public override LogImpact LogImpact => LogImpact.Medium;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FlammableReaction target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<FlammableReaction> target1 = (EventEntityEffect<FlammableReaction>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FlammableReaction) target1;
    if (serialization.TryCustomCopy<FlammableReaction>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Multiplier, ref target2, hookCtx, false, context))
      target2 = this.Multiplier;
    target.Multiplier = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MultiplierOnExisting, ref target3, hookCtx, false, context))
      target3 = this.MultiplierOnExisting;
    target.MultiplierOnExisting = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FlammableReaction target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<FlammableReaction> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FlammableReaction target1 = (FlammableReaction) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<FlammableReaction>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FlammableReaction target1 = (FlammableReaction) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual FlammableReaction EventEntityEffect<FlammableReaction>.Instantiate()
  {
    return new FlammableReaction();
  }
}
