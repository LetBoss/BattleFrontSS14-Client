// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Effects.Neutral.Focusing
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Body;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Drunk;
using Content.Shared.EntityEffects;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.Speech.Muting;
using Content.Shared.StatusEffect;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.Effects.Neutral;

public sealed class Focusing : 
  RMCChemicalEffect,
  ISerializationGenerated<Focusing>,
  ISerializationGenerated
{
  private static readonly ProtoId<DamageTypePrototype> PoisonType = (ProtoId<DamageTypePrototype>) "Poison";

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return $"Removes [color=green]{this.PotencyPerSecond}[/color] units of alcoholic substances and [color=green]{this.PotencyPerSecond * 2f}[/color] seconds of drunkenness{((double) this.ActualPotency >= 3.0 ? ". Also powerful enough to instantly cure mute and blindness." : ".")}\nOverdoses cause [color=red]{this.PotencyPerSecond}[/color] toxin damage.\nCritical overdoses cause [color=red]{this.PotencyPerSecond * 3f}[/color] toxin damage";
  }

  protected override void Tick(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    SharedRMCBloodstreamSystem bloodstreamSystem = args.EntityManager.System<SharedRMCBloodstreamSystem>();
    SharedDrunkSystem sharedDrunkSystem = args.EntityManager.System<SharedDrunkSystem>();
    SharedStutteringSystem stutteringSystem = args.EntityManager.System<SharedStutteringSystem>();
    StatusEffectsSystem statusEffectsSystem = args.EntityManager.System<StatusEffectsSystem>();
    EntityUid targetEntity = args.TargetEntity;
    FixedPoint2 amount = potency;
    bloodstreamSystem.RemoveBloodstreamAlcohols(targetEntity, amount);
    sharedDrunkSystem.TryRemoveDrunkenessTime(args.TargetEntity, (double) this.PotencyPerSecond * 2.0);
    stutteringSystem.DoRemoveStutterTime(args.TargetEntity, (double) this.PotencyPerSecond * 2.0);
    statusEffectsSystem.TryRemoveTime(args.TargetEntity, "Jitter", TimeSpan.FromSeconds((double) this.PotencyPerSecond * 2.0));
    if ((double) this.ActualPotency < 3.0)
      return;
    args.EntityManager.EntitySysManager.GetEntitySystem<BlindableSystem>().AdjustEyeDamage((Entity<BlindableComponent>) args.TargetEntity, -9);
    args.EntityManager.RemoveComponent<MutedComponent>(args.TargetEntity);
  }

  protected override void TickOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), new DamageSpecifier()
    {
      DamageDict = {
        [(string) Focusing.PoisonType] = potency
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
        [(string) Focusing.PoisonType] = potency * 3
      }
    }, true, false);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Focusing target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCChemicalEffect target1 = (RMCChemicalEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Focusing) target1;
    serialization.TryCustomCopy<Focusing>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Focusing target,
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
    Focusing target1 = (Focusing) target;
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
    Focusing target1 = (Focusing) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Focusing RMCChemicalEffect.Instantiate() => new Focusing();
}
