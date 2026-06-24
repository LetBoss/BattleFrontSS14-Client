// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Effects.Negative.Carcinogenic
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.Effects.Negative;

public sealed class Carcinogenic : 
  RMCChemicalEffect,
  ISerializationGenerated<Carcinogenic>,
  ISerializationGenerated
{
  private static readonly ProtoId<DamageTypePrototype> GeneticType = (ProtoId<DamageTypePrototype>) "Cellular";
  private static readonly ProtoId<DamageTypePrototype> BluntType = (ProtoId<DamageTypePrototype>) "Blunt";

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return $"Deals [color=red]{(double) this.PotencyPerSecond * 0.5}[/color] genetic damage.\nOverdoses cause [color=red]{this.PotencyPerSecond * 2f}[/color] genetic damage.\nCritical overdoses cause [color=red]{this.PotencyPerSecond * 2f}[/color] brute damage";
  }

  protected override void Tick(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), new DamageSpecifier()
    {
      DamageDict = {
        [(string) Carcinogenic.GeneticType] = potency * 0.5f
      }
    }, true, false);
  }

  protected override void TickOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), new DamageSpecifier()
    {
      DamageDict = {
        [(string) Carcinogenic.GeneticType] = potency * 2f
      }
    }, true, false);
  }

  protected override void TickCriticalOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), new DamageSpecifier()
    {
      DamageDict = {
        [(string) Carcinogenic.BluntType] = potency * 2f
      }
    }, true, false);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Carcinogenic target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCChemicalEffect target1 = (RMCChemicalEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Carcinogenic) target1;
    serialization.TryCustomCopy<Carcinogenic>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Carcinogenic target,
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
    Carcinogenic target1 = (Carcinogenic) target;
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
    Carcinogenic target1 = (Carcinogenic) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Carcinogenic RMCChemicalEffect.Instantiate() => new Carcinogenic();
}
