// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.Drunk
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Drunk;
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

public sealed class Drunk : EntityEffect, ISerializationGenerated<Content.Shared.EntityEffects.Effects.Drunk>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float BoozePower = 3f;
  [DataField(null, false, 1, false, false, null)]
  public bool SlurSpeech = true;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-drunk", ("chance", (object) this.Probability));
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    float boozePower = this.BoozePower;
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs != null)
      boozePower *= effectReagentArgs.Scale.Float();
    args.EntityManager.EntitySysManager.GetEntitySystem<SharedDrunkSystem>().TryApplyDrunkenness(args.TargetEntity, boozePower, this.SlurSpeech);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Content.Shared.EntityEffects.Effects.Drunk target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Content.Shared.EntityEffects.Effects.Drunk) target1;
    if (serialization.TryCustomCopy<Content.Shared.EntityEffects.Effects.Drunk>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BoozePower, ref target2, hookCtx, false, context))
      target2 = this.BoozePower;
    target.BoozePower = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.SlurSpeech, ref target3, hookCtx, false, context))
      target3 = this.SlurSpeech;
    target.SlurSpeech = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Content.Shared.EntityEffects.Effects.Drunk target,
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
    Content.Shared.EntityEffects.Effects.Drunk target1 = (Content.Shared.EntityEffects.Effects.Drunk) target;
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
    Content.Shared.EntityEffects.Effects.Drunk target1 = (Content.Shared.EntityEffects.Effects.Drunk) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Content.Shared.EntityEffects.Effects.Drunk EntityEffect.Instantiate() => new Content.Shared.EntityEffects.Effects.Drunk();
}
