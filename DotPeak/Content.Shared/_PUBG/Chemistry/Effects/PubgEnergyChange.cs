// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Chemistry.Effects.PubgEnergyChange
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._PUBG.Medicine;
using Content.Shared.EntityEffects;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

#nullable enable
namespace Content.Shared._PUBG.Chemistry.Effects;

public sealed class PubgEnergyChange : 
  EntityEffect,
  ISerializationGenerated<PubgEnergyChange>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [JsonPropertyName("amount")]
  public float Amount;
  [DataField(null, false, 1, false, false, null)]
  [JsonPropertyName("scaleByQuantity")]
  public bool ScaleByQuantity = true;

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("pubg-reagent-effect-energy", ("amount", (object) this.Amount));
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    PubgEnergyComponent component;
    if ((object) effectReagentArgs == null || !effectReagentArgs.EntityManager.TryGetComponent<PubgEnergyComponent>(effectReagentArgs.TargetEntity, out component))
      return;
    float amount = this.Amount;
    if (this.ScaleByQuantity)
      amount *= effectReagentArgs.Quantity.Float();
    if ((double) amount <= 0.0)
      return;
    float num = MathF.Min(component.MaxEnergy, component.Energy + amount);
    if ((double) MathF.Abs(num - component.Energy) <= 1.0 / 1000.0)
      return;
    component.Energy = num;
    effectReagentArgs.EntityManager.Dirty(effectReagentArgs.TargetEntity, (IComponent) component);
    effectReagentArgs.EntityManager.System<MovementSpeedModifierSystem>().RefreshMovementSpeedModifiers(effectReagentArgs.TargetEntity);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PubgEnergyChange target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PubgEnergyChange) target1;
    if (serialization.TryCustomCopy<PubgEnergyChange>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Amount, ref target2, hookCtx, false, context))
      target2 = this.Amount;
    target.Amount = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.ScaleByQuantity, ref target3, hookCtx, false, context))
      target3 = this.ScaleByQuantity;
    target.ScaleByQuantity = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PubgEnergyChange target,
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
    PubgEnergyChange target1 = (PubgEnergyChange) target;
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
    PubgEnergyChange target1 = (PubgEnergyChange) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PubgEnergyChange EntityEffect.Instantiate() => new PubgEnergyChange();
}
