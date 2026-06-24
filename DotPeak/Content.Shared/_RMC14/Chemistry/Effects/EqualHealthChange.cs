// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Effects.EqualHealthChange
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Content.Shared.Localizations;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.Effects;

public sealed class EqualHealthChange : 
  EntityEffect,
  ISerializationGenerated<EqualHealthChange>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [JsonPropertyName("damage")]
  public List<(ProtoId<DamageGroupPrototype> Group, FixedPoint2 Amount)> Damage = new List<(ProtoId<DamageGroupPrototype>, FixedPoint2)>();
  [DataField(null, false, 1, false, false, null)]
  [JsonPropertyName("ignoreResistances")]
  public bool IgnoreResistances = true;

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    List<string> list = new List<string>();
    bool flag1 = false;
    bool flag2 = false;
    foreach ((ProtoId<DamageGroupPrototype> protoId, FixedPoint2 Amount) in this.Damage)
    {
      DamageGroupPrototype prototype1;
      if (prototype.TryIndex<DamageGroupPrototype>(protoId, out prototype1))
      {
        int num = FixedPoint2.Sign(Amount);
        if (num < 0)
          flag1 = true;
        if (num > 0)
          flag2 = true;
        list.Add(Loc.GetString("health-change-display", ("kind", (object) prototype1.LocalizedName), ("amount", (object) MathF.Abs(Amount.Float())), ("deltasign", (object) num)));
      }
    }
    string str = flag1 ? (flag2 ? "both" : "heals") : (flag2 ? "deals" : "none");
    return Loc.GetString("reagent-effect-guidebook-health-change", ("chance", (object) this.Probability), ("changes", (object) ContentLocalizationManager.FormatList(list)), ("healsordeals", (object) str));
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    DamageSpecifier damageSpecifier = new DamageSpecifier();
    SharedRMCDamageableSystem damageableSystem = args.EntityManager.System<SharedRMCDamageableSystem>();
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    FixedPoint2 fixedPoint2 = (object) effectReagentArgs != null ? effectReagentArgs.Scale : (FixedPoint2) 1;
    foreach ((ProtoId<DamageGroupPrototype> protoId, FixedPoint2 Amount) in this.Damage)
      damageSpecifier = damageableSystem.DistributeDamageCached((Entity<DamageableComponent>) args.TargetEntity, protoId, Amount * fixedPoint2, damageSpecifier);
    args.EntityManager.System<DamageableSystem>().TryChangeDamage(new EntityUid?(args.TargetEntity), damageSpecifier, this.IgnoreResistances, false);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EqualHealthChange target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EqualHealthChange) target1;
    if (serialization.TryCustomCopy<EqualHealthChange>(this, ref target, hookCtx, false, context))
      return;
    List<(ProtoId<DamageGroupPrototype>, FixedPoint2)> target2 = (List<(ProtoId<DamageGroupPrototype>, FixedPoint2)>) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<(ProtoId<DamageGroupPrototype>, FixedPoint2)>>(this.Damage, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<(ProtoId<DamageGroupPrototype>, FixedPoint2)>>(this.Damage, hookCtx, context);
    target.Damage = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreResistances, ref target3, hookCtx, false, context))
      target3 = this.IgnoreResistances;
    target.IgnoreResistances = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EqualHealthChange target,
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
    EqualHealthChange target1 = (EqualHealthChange) target;
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
    EqualHealthChange target1 = (EqualHealthChange) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual EqualHealthChange EntityEffect.Instantiate() => new EqualHealthChange();
}
