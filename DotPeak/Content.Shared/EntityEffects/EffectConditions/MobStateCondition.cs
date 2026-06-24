// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.EffectConditions.MobStateCondition
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class MobStateCondition : 
  EntityEffectCondition,
  ISerializationGenerated<MobStateCondition>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public MobState Mobstate = MobState.Alive;

  public override bool Condition(EntityEffectBaseArgs args)
  {
    MobStateComponent component;
    return args.EntityManager.TryGetComponent<MobStateComponent>(args.TargetEntity, out component) && component.CurrentState == this.Mobstate;
  }

  public override string GuidebookExplanation(IPrototypeManager prototype)
  {
    return Loc.GetString("reagent-effect-condition-guidebook-mob-state-condition", ("state", (object) this.Mobstate));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MobStateCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffectCondition target1 = (EntityEffectCondition) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MobStateCondition) target1;
    if (serialization.TryCustomCopy<MobStateCondition>(this, ref target, hookCtx, false, context))
      return;
    MobState target2 = MobState.Invalid;
    if (!serialization.TryCustomCopy<MobState>(this.Mobstate, ref target2, hookCtx, false, context))
      target2 = this.Mobstate;
    target.Mobstate = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MobStateCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityEffectCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MobStateCondition target1 = (MobStateCondition) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityEffectCondition) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MobStateCondition target1 = (MobStateCondition) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual MobStateCondition EntityEffectCondition.Instantiate() => new MobStateCondition();
}
