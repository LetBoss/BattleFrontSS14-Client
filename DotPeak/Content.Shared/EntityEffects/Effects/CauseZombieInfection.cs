// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.CauseZombieInfection
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class CauseZombieInfection : 
  EventEntityEffect<CauseZombieInfection>,
  ISerializationGenerated<CauseZombieInfection>,
  ISerializationGenerated
{
  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-cause-zombie-infection", ("chance", (object) this.Probability));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CauseZombieInfection target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<CauseZombieInfection> target1 = (EventEntityEffect<CauseZombieInfection>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CauseZombieInfection) target1;
    serialization.TryCustomCopy<CauseZombieInfection>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CauseZombieInfection target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<CauseZombieInfection> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CauseZombieInfection target1 = (CauseZombieInfection) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<CauseZombieInfection>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CauseZombieInfection target1 = (CauseZombieInfection) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual CauseZombieInfection EventEntityEffect<CauseZombieInfection>.Instantiate()
  {
    return new CauseZombieInfection();
  }
}
