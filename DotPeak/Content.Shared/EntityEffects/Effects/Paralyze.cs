// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.Paralyze
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Stunnable;
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

public sealed class Paralyze : 
  EntityEffect,
  ISerializationGenerated<Paralyze>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public double ParalyzeTime = 2.0;
  [DataField(null, false, 1, false, false, null)]
  public bool Refresh = true;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-paralyze", ("chance", (object) this.Probability), ("time", (object) this.ParalyzeTime));
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    double paralyzeTime = this.ParalyzeTime;
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs != null)
      paralyzeTime *= (double) effectReagentArgs.Scale;
    args.EntityManager.System<SharedStunSystem>().TryParalyze(args.TargetEntity, TimeSpan.FromSeconds(paralyzeTime), this.Refresh);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Paralyze target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Paralyze) target1;
    if (serialization.TryCustomCopy<Paralyze>(this, ref target, hookCtx, false, context))
      return;
    double target2 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.ParalyzeTime, ref target2, hookCtx, false, context))
      target2 = this.ParalyzeTime;
    target.ParalyzeTime = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Refresh, ref target3, hookCtx, false, context))
      target3 = this.Refresh;
    target.Refresh = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Paralyze target,
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
    Paralyze target1 = (Paralyze) target;
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
    Paralyze target1 = (Paralyze) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Paralyze EntityEffect.Instantiate() => new Paralyze();
}
