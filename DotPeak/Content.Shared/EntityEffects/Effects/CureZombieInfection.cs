// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.CureZombieInfection
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

public sealed class CureZombieInfection : 
  EventEntityEffect<CureZombieInfection>,
  ISerializationGenerated<CureZombieInfection>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Innoculate;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return this.Innoculate ? Loc.GetString("reagent-effect-guidebook-innoculate-zombie-infection", ("chance", (object) this.Probability)) : Loc.GetString("reagent-effect-guidebook-cure-zombie-infection", ("chance", (object) this.Probability));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CureZombieInfection target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<CureZombieInfection> target1 = (EventEntityEffect<CureZombieInfection>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CureZombieInfection) target1;
    if (serialization.TryCustomCopy<CureZombieInfection>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Innoculate, ref target2, hookCtx, false, context))
      target2 = this.Innoculate;
    target.Innoculate = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CureZombieInfection target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<CureZombieInfection> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CureZombieInfection target1 = (CureZombieInfection) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<CureZombieInfection>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CureZombieInfection target1 = (CureZombieInfection) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual CureZombieInfection EventEntityEffect<CureZombieInfection>.Instantiate()
  {
    return new CureZombieInfection();
  }
}
