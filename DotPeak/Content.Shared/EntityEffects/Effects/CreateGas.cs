// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.CreateGas
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Atmos;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Database;
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

public sealed class CreateGas : 
  EventEntityEffect<CreateGas>,
  ISerializationGenerated<CreateGas>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public Gas Gas;
  [DataField(null, false, 1, false, false, null)]
  public float Multiplier = 3f;

  public override bool ShouldLog => true;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-create-gas", ("chance", (object) this.Probability), ("moles", (object) this.Multiplier), ("gas", (object) entSys.GetEntitySystem<SharedAtmosphereSystem>().GetGas(this.Gas).Name));
  }

  public override LogImpact LogImpact => LogImpact.High;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CreateGas target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<CreateGas> target1 = (EventEntityEffect<CreateGas>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CreateGas) target1;
    if (serialization.TryCustomCopy<CreateGas>(this, ref target, hookCtx, false, context))
      return;
    Gas target2 = Gas.Oxygen;
    if (!serialization.TryCustomCopy<Gas>(this.Gas, ref target2, hookCtx, false, context))
      target2 = this.Gas;
    target.Gas = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Multiplier, ref target3, hookCtx, false, context))
      target3 = this.Multiplier;
    target.Multiplier = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CreateGas target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<CreateGas> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CreateGas target1 = (CreateGas) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<CreateGas>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    CreateGas target1 = (CreateGas) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual CreateGas EventEntityEffect<CreateGas>.Instantiate() => new CreateGas();
}
