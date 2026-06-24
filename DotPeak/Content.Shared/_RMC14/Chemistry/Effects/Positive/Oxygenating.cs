// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Effects.Positive.Oxygenating
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Body;
using Content.Shared._RMC14.Damage;
using Content.Shared.Chemistry.Reagent;
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
namespace Content.Shared._RMC14.Chemistry.Effects.Positive;

public sealed class Oxygenating : 
  RMCChemicalEffect,
  ISerializationGenerated<Oxygenating>,
  ISerializationGenerated
{
  private static readonly ProtoId<DamageGroupPrototype> AirlossGroup = (ProtoId<DamageGroupPrototype>) "Airloss";
  private static readonly ProtoId<DamageTypePrototype> BluntType = (ProtoId<DamageTypePrototype>) "Blunt";
  private static readonly ProtoId<DamageTypePrototype> PoisonType = (ProtoId<DamageTypePrototype>) "Poison";
  private static readonly ProtoId<ReagentPrototype> Lexorin = (ProtoId<ReagentPrototype>) "RMCLexorin";

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    string str;
    if ((double) this.ActualPotency < 3.0)
      str = $"Heals [color=green]{this.PotencyPerSecond}[/color] airloss damage and removes [color=green]{this.PotencyPerSecond}[/color] Lexorin from the bloodstream.";
    else
      str = $"Heals [color=green]all[/color] airloss damage and removes [color=green]{this.PotencyPerSecond}[/color] Lexorin from the bloodstream.";
    return $"{str}\nOverdoses cause [color=red]{(double) this.PotencyPerSecond * 0.5}[/color] toxin damage.\nCritical overdoses cause [color=red]{this.PotencyPerSecond}[/color] brute and [color=red]{this.PotencyPerSecond * 2f}[/color] toxin damage";
  }

  protected override void Tick(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    SharedRMCDamageableSystem damageableSystem = args.EntityManager.System<SharedRMCDamageableSystem>();
    FixedPoint2 fixedPoint2 = (double) this.ActualPotency >= 3.0 ? (FixedPoint2) 99999 : potency;
    Entity<DamageableComponent> targetEntity = (Entity<DamageableComponent>) args.TargetEntity;
    ProtoId<DamageGroupPrototype> airlossGroup = Oxygenating.AirlossGroup;
    FixedPoint2 amount = fixedPoint2;
    DamageSpecifier damage = damageableSystem.DistributeHealingCached(targetEntity, airlossGroup, amount);
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), damage, true, false);
    args.EntityManager.System<SharedRMCBloodstreamSystem>().RemoveBloodstreamChemical(args.TargetEntity, Oxygenating.Lexorin, potency);
  }

  protected override void TickOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), new DamageSpecifier()
    {
      DamageDict = {
        [(string) Oxygenating.PoisonType] = potency * 0.5f
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
        [(string) Oxygenating.BluntType] = potency,
        [(string) Oxygenating.PoisonType] = potency * 2f
      }
    }, true, false);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Oxygenating target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCChemicalEffect target1 = (RMCChemicalEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Oxygenating) target1;
    serialization.TryCustomCopy<Oxygenating>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Oxygenating target,
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
    Oxygenating target1 = (Oxygenating) target;
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
    Oxygenating target1 = (Oxygenating) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Oxygenating RMCChemicalEffect.Instantiate() => new Oxygenating();
}
