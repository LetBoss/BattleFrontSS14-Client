// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.ResetNarcolepsy
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

public sealed class ResetNarcolepsy : 
  EventEntityEffect<ResetNarcolepsy>,
  ISerializationGenerated<ResetNarcolepsy>,
  ISerializationGenerated
{
  [DataField("TimerReset", false, 1, false, false, null)]
  public int TimerReset = 600;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-reset-narcolepsy", ("chance", (object) this.Probability));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ResetNarcolepsy target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<ResetNarcolepsy> target1 = (EventEntityEffect<ResetNarcolepsy>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ResetNarcolepsy) target1;
    if (serialization.TryCustomCopy<ResetNarcolepsy>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.TimerReset, ref target2, hookCtx, false, context))
      target2 = this.TimerReset;
    target.TimerReset = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ResetNarcolepsy target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<ResetNarcolepsy> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ResetNarcolepsy target1 = (ResetNarcolepsy) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<ResetNarcolepsy>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    ResetNarcolepsy target1 = (ResetNarcolepsy) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ResetNarcolepsy EventEntityEffect<ResetNarcolepsy>.Instantiate() => new ResetNarcolepsy();
}
