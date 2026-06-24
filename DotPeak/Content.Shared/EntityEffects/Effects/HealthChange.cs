// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.HealthChange
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
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
namespace Content.Shared.EntityEffects.Effects;

public sealed class HealthChange : 
  EntityEffect,
  ISerializationGenerated<HealthChange>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [JsonPropertyName("damage")]
  public DamageSpecifier Damage;
  [DataField(null, false, 1, false, false, null)]
  [JsonPropertyName("scaleByQuantity")]
  public bool ScaleByQuantity;
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
    DamageSpecifier damage = new DamageSpecifier(this.Damage);
    float reagentDamageModifier = entSys.GetEntitySystem<DamageableSystem>().UniversalReagentDamageModifier;
    float reagentHealModifier = entSys.GetEntitySystem<DamageableSystem>().UniversalReagentHealModifier;
    string key4;
    FixedPoint2 fixedPoint2_4;
    if ((double) reagentDamageModifier != 1.0 || (double) reagentHealModifier != 1.0)
    {
      foreach ((key4, fixedPoint2_4) in damage.DamageDict)
      {
        string key3 = key4;
        FixedPoint2 fixedPoint2_3 = fixedPoint2_4;
        if (fixedPoint2_3 < (FixedPoint2) 0.0f)
          damage.DamageDict[key3] = fixedPoint2_3 * reagentHealModifier;
        if (fixedPoint2_3 > (FixedPoint2) 0.0f)
          damage.DamageDict[key3] = fixedPoint2_3 * reagentDamageModifier;
      }
    }
    foreach ((key4, fixedPoint2_4) in entSys.GetEntitySystem<DamageableSystem>().ApplyUniversalAllModifiers(damage).DamageDict)
    {
      string id = key4;
      FixedPoint2 fixedPoint2_5 = fixedPoint2_4;
      int num = FixedPoint2.Sign(fixedPoint2_5);
      if (num < 0)
        flag1 = true;
      if (num > 0)
        flag2 = true;
      list.Add(Loc.GetString("health-change-display", ("kind", (object) prototype.Index<DamageTypePrototype>(id).LocalizedName), ("amount", (object) MathF.Abs(fixedPoint2_5.Float())), ("deltasign", (object) num)));
    }
    string str = flag1 ? (flag2 ? "both" : "heals") : (flag2 ? "deals" : "none");
    return Loc.GetString("reagent-effect-guidebook-health-change", ("chance", (object) this.Probability), ("changes", (object) ContentLocalizationManager.FormatList(list)), ("healsordeals", (object) str));
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    FixedPoint2 fixedPoint2_1 = FixedPoint2.New(1);
    DamageSpecifier damageSpecifier = new DamageSpecifier(this.Damage);
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs != null)
      fixedPoint2_1 = this.ScaleByQuantity ? effectReagentArgs.Quantity * effectReagentArgs.Scale : effectReagentArgs.Scale;
    float reagentDamageModifier = args.EntityManager.System<DamageableSystem>().UniversalReagentDamageModifier;
    float reagentHealModifier = args.EntityManager.System<DamageableSystem>().UniversalReagentHealModifier;
    if ((double) reagentDamageModifier != 1.0 || (double) reagentHealModifier != 1.0)
    {
      foreach ((string key, FixedPoint2 fixedPoint2_2) in damageSpecifier.DamageDict)
      {
        if (fixedPoint2_2 < (FixedPoint2) 0.0f)
          damageSpecifier.DamageDict[key] = fixedPoint2_2 * reagentHealModifier;
        if (fixedPoint2_2 > (FixedPoint2) 0.0f)
          damageSpecifier.DamageDict[key] = fixedPoint2_2 * reagentDamageModifier;
      }
    }
    args.EntityManager.System<DamageableSystem>().TryChangeDamage(new EntityUid?(args.TargetEntity), damageSpecifier * fixedPoint2_1, this.IgnoreResistances, false);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HealthChange target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HealthChange) target1;
    if (serialization.TryCustomCopy<HealthChange>(this, ref target, hookCtx, false, context))
      return;
    DamageSpecifier target2 = (DamageSpecifier) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.Damage, ref target2, hookCtx, false, context))
    {
      if (this.Damage == null)
        target2 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.Damage, ref target2, hookCtx, context, true);
    }
    target.Damage = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.ScaleByQuantity, ref target3, hookCtx, false, context))
      target3 = this.ScaleByQuantity;
    target.ScaleByQuantity = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreResistances, ref target4, hookCtx, false, context))
      target4 = this.IgnoreResistances;
    target.IgnoreResistances = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HealthChange target,
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
    HealthChange target1 = (HealthChange) target;
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
    HealthChange target1 = (HealthChange) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual HealthChange EntityEffect.Instantiate() => new HealthChange();
}
