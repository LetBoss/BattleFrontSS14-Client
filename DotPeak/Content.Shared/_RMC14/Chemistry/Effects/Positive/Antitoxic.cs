// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Effects.Positive.Antitoxic
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Body;
using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Stun;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.Effects.Positive;

public sealed class Antitoxic : 
  RMCChemicalEffect,
  ISerializationGenerated<Antitoxic>,
  ISerializationGenerated
{
  private static readonly ProtoId<DamageGroupPrototype> ToxinGroup = (ProtoId<DamageGroupPrototype>) "Toxin";
  private static readonly ProtoId<DamageGroupPrototype> GeneticGroup = (ProtoId<DamageGroupPrototype>) "Genetic";
  private static readonly ProtoId<StatusEffectPrototype> Unconscious = (ProtoId<StatusEffectPrototype>) nameof (Unconscious);

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return $"Heals [color=green]{this.PotencyPerSecond * 2f}[/color] toxin damage and removes [color=green]0.125[/color] units of toxic chemicals from the bloodstream.\nCritical overdoses cause [color=red]5[/color] seconds of unconsciousness with a [color=red]5%[/color] chance";
  }

  protected override void Tick(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    SharedRMCDamageableSystem damageableSystem = args.EntityManager.System<SharedRMCDamageableSystem>();
    DamageSpecifier equal = damageableSystem.DistributeHealingCached((Entity<DamageableComponent>) args.TargetEntity, Antitoxic.ToxinGroup, potency * 2f);
    DamageSpecifier damage = damageableSystem.DistributeHealingCached((Entity<DamageableComponent>) args.TargetEntity, Antitoxic.GeneticGroup, potency * 2f, equal);
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), damage, true, false);
    args.EntityManager.System<SharedRMCBloodstreamSystem>().RemoveBloodstreamToxins(args.TargetEntity, (FixedPoint2) 0.125f);
  }

  protected override void TickOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
  }

  protected override void TickCriticalOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    if (!IoCManager.Resolve<IRobustRandom>().Prob(0.05f))
      return;
    args.EntityManager.System<StatusEffectsSystem>().TryAddStatusEffect<RMCUnconsciousComponent>(args.TargetEntity, (string) Antitoxic.Unconscious, TimeSpan.FromSeconds(5L), false);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Antitoxic target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCChemicalEffect target1 = (RMCChemicalEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Antitoxic) target1;
    serialization.TryCustomCopy<Antitoxic>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Antitoxic target,
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
    Antitoxic target1 = (Antitoxic) target;
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
    Antitoxic target1 = (Antitoxic) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Antitoxic RMCChemicalEffect.Instantiate() => new Antitoxic();
}
