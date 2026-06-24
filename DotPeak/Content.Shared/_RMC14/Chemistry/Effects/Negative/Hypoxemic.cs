// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Effects.Negative.Hypoxemic
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Emote;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.Effects.Negative;

public sealed class Hypoxemic : 
  RMCChemicalEffect,
  ISerializationGenerated<Hypoxemic>,
  ISerializationGenerated
{
  private static readonly ProtoId<DamageTypePrototype> BluntType = (ProtoId<DamageTypePrototype>) "Blunt";
  private static readonly ProtoId<DamageTypePrototype> PoisonType = (ProtoId<DamageTypePrototype>) "Poison";
  private static readonly ProtoId<DamageTypePrototype> AsphyxiationType = (ProtoId<DamageTypePrototype>) "Asphyxiation";
  private static readonly ProtoId<EmotePrototype> GaspEmote = (ProtoId<EmotePrototype>) "Gasp";

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return $"Deals [color=red]{this.PotencyPerSecond * 2f}[/color] airloss damage and causes the victim to gasp for air.\nOverdoses cause [color=red]{this.PotencyPerSecond}[/color] brute, [color=red]{this.PotencyPerSecond}[/color] toxin, and [color=red]{this.PotencyPerSecond * 5f}[/color] airloss damage.\nCritical overdoses cause [color=red]{this.PotencyPerSecond * 5f}[/color] brute and [color=red]{this.PotencyPerSecond * 2f}[/color] toxin damage";
  }

  protected override void Tick(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), new DamageSpecifier()
    {
      DamageDict = {
        [(string) Hypoxemic.AsphyxiationType] = potency * 2f
      }
    }, true, false);
    if (!IoCManager.Resolve<IRobustRandom>().Prob(0.1f))
      return;
    args.EntityManager.System<SharedRMCEmoteSystem>().TryEmoteWithChat(args.TargetEntity, Hypoxemic.GaspEmote, true, ignoreActionBlocker: true, forceEmote: true);
  }

  protected override void TickOverdose(
    DamageableSystem damageable,
    FixedPoint2 potency,
    EntityEffectReagentArgs args)
  {
    damageable.TryChangeDamage(new EntityUid?(args.TargetEntity), new DamageSpecifier()
    {
      DamageDict = {
        [(string) Hypoxemic.BluntType] = potency,
        [(string) Hypoxemic.PoisonType] = potency,
        [(string) Hypoxemic.AsphyxiationType] = potency * 5f
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
        [(string) Hypoxemic.BluntType] = potency * 5f,
        [(string) Hypoxemic.PoisonType] = potency * 2f
      }
    }, true, false);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Hypoxemic target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCChemicalEffect target1 = (RMCChemicalEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Hypoxemic) target1;
    serialization.TryCustomCopy<Hypoxemic>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Hypoxemic target,
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
    Hypoxemic target1 = (Hypoxemic) target;
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
    Hypoxemic target1 = (Hypoxemic) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Hypoxemic RMCChemicalEffect.Instantiate() => new Hypoxemic();
}
