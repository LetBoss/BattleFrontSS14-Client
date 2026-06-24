// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.StatusEffects.ModifyStatusEffect
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.StatusEffectNew;
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

public sealed class ModifyStatusEffect : 
  EntityEffect,
  ISerializationGenerated<ModifyStatusEffect>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId EffectProto;
  [DataField(null, false, 1, false, false, null)]
  public float Time = 2f;
  [DataField(null, false, 1, false, false, null)]
  public bool Refresh = true;
  [DataField(null, false, 1, false, false, null)]
  public StatusEffectMetabolismType Type;

  public override void Effect(EntityEffectBaseArgs args)
  {
    SharedStatusEffectsSystem entitySystem = args.EntityManager.EntitySysManager.GetEntitySystem<SharedStatusEffectsSystem>();
    float time = this.Time;
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs != null)
      time *= effectReagentArgs.Scale.Float();
    TimeSpan duration = TimeSpan.FromSeconds((double) time);
    switch (this.Type)
    {
      case StatusEffectMetabolismType.Add:
        if (this.Refresh)
        {
          entitySystem.TryUpdateStatusEffectDuration(args.TargetEntity, this.EffectProto, new TimeSpan?(duration));
          break;
        }
        entitySystem.TryAddStatusEffectDuration(args.TargetEntity, this.EffectProto, duration);
        break;
      case StatusEffectMetabolismType.Remove:
        entitySystem.TryAddTime(args.TargetEntity, this.EffectProto, -duration);
        break;
      case StatusEffectMetabolismType.Set:
        entitySystem.TrySetStatusEffectDuration(args.TargetEntity, this.EffectProto, new TimeSpan?(duration));
        break;
    }
  }

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-status-effect", ("chance", (object) this.Probability), ("type", (object) this.Type), ("time", (object) this.Time), ("key", (object) prototype.Index(this.EffectProto).Name));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ModifyStatusEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ModifyStatusEffect) target1;
    if (serialization.TryCustomCopy<ModifyStatusEffect>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.EffectProto, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.EffectProto, hookCtx, context);
    target.EffectProto = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Time, ref target3, hookCtx, false, context))
      target3 = this.Time;
    target.Time = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Refresh, ref target4, hookCtx, false, context))
      target4 = this.Refresh;
    target.Refresh = target4;
    StatusEffectMetabolismType target5 = StatusEffectMetabolismType.Add;
    if (!serialization.TryCustomCopy<StatusEffectMetabolismType>(this.Type, ref target5, hookCtx, false, context))
      target5 = this.Type;
    target.Type = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ModifyStatusEffect target,
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
    ModifyStatusEffect target1 = (ModifyStatusEffect) target;
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
    ModifyStatusEffect target1 = (ModifyStatusEffect) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual ModifyStatusEffect EntityEffect.Instantiate() => new ModifyStatusEffect();
}
