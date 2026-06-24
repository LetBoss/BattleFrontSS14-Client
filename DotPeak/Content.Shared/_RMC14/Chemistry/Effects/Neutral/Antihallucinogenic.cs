// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Effects.neutral.Antihallucinogenic
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Body;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.StatusEffect;
using Content.Shared.StatusEffectNew.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.Effects.neutral;

public sealed class Antihallucinogenic : 
  RMCChemicalEffect,
  ISerializationGenerated<Antihallucinogenic>,
  ISerializationGenerated
{
  private static readonly ProtoId<DamageTypePrototype> BluntType = (ProtoId<DamageTypePrototype>) "Blunt";
  private static readonly ProtoId<DamageTypePrototype> HeatType = (ProtoId<DamageTypePrototype>) "Heat";
  private static readonly ProtoId<DamageTypePrototype> PoisonType = (ProtoId<DamageTypePrototype>) "Poison";
  private static readonly EntProtoId<StatusEffectComponent> SeeingRainbows = (EntProtoId<StatusEffectComponent>) "StatusEffectSeeingRainbow";
  private static readonly ProtoId<ReagentPrototype> MindbreakerToxin = (ProtoId<ReagentPrototype>) "RMCMindbreakerToxin";
  private static readonly ProtoId<ReagentPrototype> SpaceDrugs = (ProtoId<ReagentPrototype>) "RMCSpaceDrugs";

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return $"Removes [color=green]2.5[/color] units of Mindbreaker Toxin and Space Drugs from the bloodstream. It also stabilizes perceptive abnormalities such as hallucinations\nOverdoses cause [color=red]{this.PotencyPerSecond}[/color] toxin damage.\nCritical overdoses cause [color=red]{this.PotencyPerSecond}[/color] brute, [color=red]{this.PotencyPerSecond}[/color] burn, and [color=red]{this.PotencyPerSecond * 3f}[/color] toxin damage";
  }

  protected override void Tick(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    SharedRMCBloodstreamSystem bloodstreamSystem = args.EntityManager.System<SharedRMCBloodstreamSystem>();
    bloodstreamSystem.RemoveBloodstreamChemical(args.TargetEntity, Antihallucinogenic.MindbreakerToxin, (FixedPoint2) 2.5f);
    bloodstreamSystem.RemoveBloodstreamChemical(args.TargetEntity, Antihallucinogenic.SpaceDrugs, (FixedPoint2) 2.5f);
    args.EntityManager.System<StatusEffectsSystem>().TryRemoveTime(args.TargetEntity, (string) Antihallucinogenic.SeeingRainbows, TimeSpan.FromSeconds((double) this.PotencyPerSecond * 10.0));
  }

  protected override void TickOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), new DamageSpecifier()
    {
      DamageDict = {
        [(string) Antihallucinogenic.PoisonType] = potency
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
        [(string) Antihallucinogenic.BluntType] = potency,
        [(string) Antihallucinogenic.HeatType] = potency,
        [(string) Antihallucinogenic.PoisonType] = potency * 3
      }
    }, true, false);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Antihallucinogenic target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCChemicalEffect target1 = (RMCChemicalEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Antihallucinogenic) target1;
    serialization.TryCustomCopy<Antihallucinogenic>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Antihallucinogenic target,
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
    Antihallucinogenic target1 = (Antihallucinogenic) target;
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
    Antihallucinogenic target1 = (Antihallucinogenic) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Antihallucinogenic RMCChemicalEffect.Instantiate() => new Antihallucinogenic();
}
