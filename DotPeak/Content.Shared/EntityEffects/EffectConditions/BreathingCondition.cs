// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.EffectConditions.Breathing
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

public sealed class Breathing : 
  EventEntityEffectCondition<Breathing>,
  ISerializationGenerated<Breathing>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool IsBreathing = true;

  public override string GuidebookExplanation(IPrototypeManager prototype)
  {
    return Loc.GetString("reagent-effect-condition-guidebook-breathing", ("isBreathing", (object) this.IsBreathing));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Breathing target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffectCondition<Breathing> target1 = (EventEntityEffectCondition<Breathing>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Breathing) target1;
    if (serialization.TryCustomCopy<Breathing>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsBreathing, ref target2, hookCtx, false, context))
      target2 = this.IsBreathing;
    target.IsBreathing = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Breathing target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffectCondition<Breathing> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Breathing target1 = (Breathing) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffectCondition<Breathing>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Breathing target1 = (Breathing) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Breathing EventEntityEffectCondition<Breathing>.Instantiate() => new Breathing();
}
