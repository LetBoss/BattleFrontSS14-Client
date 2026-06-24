// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Effects.Neutral.Thermostabilizing
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Temperature;
using Content.Shared.Damage;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.StatusEffect;
using Content.Shared.Temperature;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.Effects.Neutral;

public sealed class Thermostabilizing : 
  RMCChemicalEffect,
  ISerializationGenerated<Thermostabilizing>,
  ISerializationGenerated
{
  private static readonly ProtoId<StatusEffectPrototype> Unconscious = (ProtoId<StatusEffectPrototype>) nameof (Unconscious);

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return $"Stabilizes the temperature of the body to [color=green]{TemperatureHelpers.CelsiusToKelvin(37f)}[/color] kelvins, by [color=green]{(float) (40.0 * (double) this.PotencyPerSecond * 1.5)}[/color] K at a time.\nOverdoses cause [color=red]10[/color] seconds of unconsciousness.\nCritical overdoses cause [color=red]5[/color] seconds of unconsciousness with a [color=red]5%[/color] chance";
  }

  protected override void Tick(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    SharedRMCTemperatureSystem entitySystem = args.EntityManager.EntitySysManager.GetEntitySystem<SharedRMCTemperatureSystem>();
    float temperature1 = entitySystem.GetTemperature(args.TargetEntity);
    float kelvin = TemperatureHelpers.CelsiusToKelvin(37f);
    if ((double) Math.Abs(temperature1 - kelvin) < 0.01)
      return;
    float num = (float) (40.0 * (double) potency.Float() * 1.5);
    float temperature2 = (double) temperature1 > (double) kelvin ? Math.Max(kelvin, temperature1 - num) : Math.Min(kelvin, temperature1 + num);
    entitySystem.ForceChangeTemperature(args.TargetEntity, temperature2);
  }

  protected override void TickOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    args.EntityManager.System<StatusEffectsSystem>().TryAddStatusEffect<RMCUnconsciousComponent>(args.TargetEntity, (string) Thermostabilizing.Unconscious, TimeSpan.FromSeconds(10L), false);
  }

  protected override void TickCriticalOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    if (!IoCManager.Resolve<IRobustRandom>().Prob(0.05f))
      return;
    args.EntityManager.System<StatusEffectsSystem>().TryAddStatusEffect<RMCUnconsciousComponent>(args.TargetEntity, (string) Thermostabilizing.Unconscious, TimeSpan.FromSeconds(5L), false);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Thermostabilizing target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCChemicalEffect target1 = (RMCChemicalEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Thermostabilizing) target1;
    serialization.TryCustomCopy<Thermostabilizing>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Thermostabilizing target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref RMCChemicalEffect target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Thermostabilizing target1 = (Thermostabilizing) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (RMCChemicalEffect) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Thermostabilizing target1 = (Thermostabilizing) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Thermostabilizing RMCChemicalEffect.Instantiate() => new Thermostabilizing();
}
