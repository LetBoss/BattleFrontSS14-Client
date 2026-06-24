// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.MovespeedModifier
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Timing;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class MovespeedModifier : 
  EntityEffect,
  ISerializationGenerated<MovespeedModifier>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float StatusLifetime = 2f;

  [DataField(null, false, 1, false, false, null)]
  public float WalkSpeedModifier { get; set; } = 1f;

  [DataField(null, false, 1, false, false, null)]
  public float SprintSpeedModifier { get; set; } = 1f;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-movespeed-modifier", ("chance", (object) this.Probability), ("walkspeed", (object) this.WalkSpeedModifier), ("time", (object) this.StatusLifetime));
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    MovespeedModifierMetabolismComponent status = args.EntityManager.EnsureComponent<MovespeedModifierMetabolismComponent>(args.TargetEntity);
    int num = !status.WalkSpeedModifier.Equals(this.WalkSpeedModifier) ? 1 : (!status.SprintSpeedModifier.Equals(this.SprintSpeedModifier) ? 1 : 0);
    status.WalkSpeedModifier = this.WalkSpeedModifier;
    status.SprintSpeedModifier = this.SprintSpeedModifier;
    float statusLifetime = this.StatusLifetime;
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs != null)
      statusLifetime *= effectReagentArgs.Scale.Float();
    this.IncreaseTimer(status, statusLifetime, args.EntityManager, args.TargetEntity);
    if (num == 0)
      return;
    args.EntityManager.System<MovementSpeedModifierSystem>().RefreshMovementSpeedModifiers(args.TargetEntity);
  }

  private void IncreaseTimer(
    MovespeedModifierMetabolismComponent status,
    float time,
    IEntityManager entityManager,
    EntityUid uid)
  {
    IGameTiming gameTiming = IoCManager.Resolve<IGameTiming>();
    TimeSpan timeSpan = status.ModifierTimer;
    double totalSeconds1 = timeSpan.TotalSeconds;
    timeSpan = gameTiming.CurTime;
    double totalSeconds2 = timeSpan.TotalSeconds;
    double num = Math.Max(totalSeconds1, totalSeconds2);
    status.ModifierTimer = TimeSpan.FromSeconds(num + (double) time);
    entityManager.Dirty(uid, (IComponent) status);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MovespeedModifier target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MovespeedModifier) target1;
    if (serialization.TryCustomCopy<MovespeedModifier>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WalkSpeedModifier, ref target2, hookCtx, false, context))
      target2 = this.WalkSpeedModifier;
    target.WalkSpeedModifier = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SprintSpeedModifier, ref target3, hookCtx, false, context))
      target3 = this.SprintSpeedModifier;
    target.SprintSpeedModifier = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StatusLifetime, ref target4, hookCtx, false, context))
      target4 = this.StatusLifetime;
    target.StatusLifetime = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MovespeedModifier target,
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
    MovespeedModifier target1 = (MovespeedModifier) target;
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
    MovespeedModifier target1 = (MovespeedModifier) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual MovespeedModifier EntityEffect.Instantiate() => new MovespeedModifier();
}
