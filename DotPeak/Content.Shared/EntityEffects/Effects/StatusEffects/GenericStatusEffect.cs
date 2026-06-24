// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.StatusEffects.GenericStatusEffect
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects.StatusEffects;

[Obsolete("Use ModifyStatusEffect with StatusEffectNewSystem instead")]
public sealed class GenericStatusEffect : 
  EntityEffect,
  ISerializationGenerated<GenericStatusEffect>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string Key;
  [DataField(null, false, 1, false, false, null)]
  public string Component = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public float Time = 2f;
  [DataField(null, false, 1, false, false, null)]
  public bool Refresh = true;
  [DataField(null, false, 1, false, false, null)]
  public StatusEffectMetabolismType Type;

  public override void Effect(EntityEffectBaseArgs args)
  {
    StatusEffectsSystem entitySystem = args.EntityManager.EntitySysManager.GetEntitySystem<StatusEffectsSystem>();
    float time = this.Time;
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs != null)
      time *= effectReagentArgs.Scale.Float();
    if (this.Type == StatusEffectMetabolismType.Add && this.Component != string.Empty)
      entitySystem.TryAddStatusEffect(args.TargetEntity, this.Key, TimeSpan.FromSeconds((double) time), this.Refresh, this.Component);
    else if (this.Type == StatusEffectMetabolismType.Remove)
    {
      entitySystem.TryRemoveTime(args.TargetEntity, this.Key, TimeSpan.FromSeconds((double) time));
    }
    else
    {
      if (this.Type != StatusEffectMetabolismType.Set)
        return;
      entitySystem.TrySetTime(args.TargetEntity, this.Key, TimeSpan.FromSeconds((double) time));
    }
  }

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-status-effect", ("chance", (object) this.Probability), ("type", (object) this.Type), ("time", (object) this.Time), ("key", (object) ("reagent-effect-status-effect-" + this.Key)));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GenericStatusEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GenericStatusEffect) target1;
    if (serialization.TryCustomCopy<GenericStatusEffect>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.Key == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Key, ref target2, hookCtx, false, context))
      target2 = this.Key;
    target.Key = target2;
    string target3 = (string) null;
    if (this.Component == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Component, ref target3, hookCtx, false, context))
      target3 = this.Component;
    target.Component = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Time, ref target4, hookCtx, false, context))
      target4 = this.Time;
    target.Time = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Refresh, ref target5, hookCtx, false, context))
      target5 = this.Refresh;
    target.Refresh = target5;
    StatusEffectMetabolismType target6 = StatusEffectMetabolismType.Add;
    if (!serialization.TryCustomCopy<StatusEffectMetabolismType>(this.Type, ref target6, hookCtx, false, context))
      target6 = this.Type;
    target.Type = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GenericStatusEffect target,
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
    GenericStatusEffect target1 = (GenericStatusEffect) target;
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
    GenericStatusEffect target1 = (GenericStatusEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual GenericStatusEffect EntityEffect.Instantiate() => new GenericStatusEffect();
}
