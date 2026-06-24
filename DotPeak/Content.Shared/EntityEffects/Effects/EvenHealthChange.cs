// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.EvenHealthChange
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Localizations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class EvenHealthChange : 
  EntityEffect,
  ISerializationGenerated<EvenHealthChange>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2> Damage = new Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2>();
  [DataField(null, false, 1, false, false, null)]
  public bool ScaleByQuantity;
  [DataField(null, false, 1, false, false, null)]
  public bool IgnoreResistances = true;

  protected override string ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    List<string> list = new List<string>();
    bool flag1 = false;
    bool flag2 = false;
    DamageableSystem entitySystem = entSys.GetEntitySystem<DamageableSystem>();
    float reagentDamageModifier = entitySystem.UniversalReagentDamageModifier;
    float reagentHealModifier = entitySystem.UniversalReagentHealModifier;
    foreach ((ProtoId<DamageGroupPrototype> protoId, FixedPoint2 fixedPoint2) in this.Damage)
    {
      DamageGroupPrototype damageGroupPrototype = prototype.Index<DamageGroupPrototype>(protoId);
      int num1 = FixedPoint2.Sign(fixedPoint2);
      float num2 = 1f;
      if (num1 < 0)
      {
        flag1 = true;
        num2 = reagentHealModifier;
      }
      else if (num1 > 0)
      {
        flag2 = true;
        num2 = reagentDamageModifier;
      }
      list.Add(Loc.GetString("health-change-display", ("kind", (object) damageGroupPrototype.LocalizedName), ("amount", (object) MathF.Abs(fixedPoint2.Float() * num2)), ("deltasign", (object) num1)));
    }
    string str = flag1 ? (flag2 ? "both" : "heals") : (flag2 ? "deals" : "none");
    return Loc.GetString("reagent-effect-guidebook-even-health-change", ("chance", (object) this.Probability), ("changes", (object) ContentLocalizationManager.FormatList(list)), ("healsordeals", (object) str));
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    DamageableComponent component;
    if (!args.EntityManager.TryGetComponent<DamageableComponent>(args.TargetEntity, out component))
      return;
    IPrototypeManager prototypeManager = IoCManager.Resolve<IPrototypeManager>();
    FixedPoint2 fixedPoint2_1 = FixedPoint2.New(1);
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs != null)
      fixedPoint2_1 = this.ScaleByQuantity ? effectReagentArgs.Quantity * effectReagentArgs.Scale : effectReagentArgs.Scale;
    DamageableSystem damageableSystem = args.EntityManager.System<DamageableSystem>();
    float reagentDamageModifier = damageableSystem.UniversalReagentDamageModifier;
    float reagentHealModifier = damageableSystem.UniversalReagentHealModifier;
    DamageSpecifier damageSpecifier = new DamageSpecifier();
    string key4;
    foreach ((ProtoId<DamageGroupPrototype> protoId, FixedPoint2 fixedPoint2_7) in this.Damage)
    {
      FixedPoint2 fixedPoint2_3 = fixedPoint2_7;
      DamageGroupPrototype damageGroupPrototype = prototypeManager.Index<DamageGroupPrototype>(protoId);
      Dictionary<string, FixedPoint2> dictionary = new Dictionary<string, FixedPoint2>();
      foreach (string damageType in damageGroupPrototype.DamageTypes)
      {
        FixedPoint2 valueOrDefault = component.Damage.DamageDict.GetValueOrDefault<string, FixedPoint2>(damageType);
        if (valueOrDefault != FixedPoint2.Zero)
          dictionary.Add(damageType, valueOrDefault);
      }
      FixedPoint2 fixedPoint2_4 = dictionary.Values.Sum();
      foreach ((key4, fixedPoint2_7) in dictionary)
      {
        string key3 = key4;
        FixedPoint2 fixedPoint2_6 = fixedPoint2_7;
        FixedPoint2 orNew = damageSpecifier.DamageDict.GetOrNew<string, FixedPoint2>(key3);
        damageSpecifier.DamageDict[key3] = orNew + fixedPoint2_6 / fixedPoint2_4 * fixedPoint2_3;
      }
    }
    if ((double) reagentDamageModifier != 1.0 || (double) reagentHealModifier != 1.0)
    {
      foreach ((key4, fixedPoint2_7) in damageSpecifier.DamageDict)
      {
        string key5 = key4;
        FixedPoint2 fixedPoint2_8 = fixedPoint2_7;
        if (fixedPoint2_8 < (FixedPoint2) 0.0f)
          damageSpecifier.DamageDict[key5] = fixedPoint2_8 * reagentHealModifier;
        if (fixedPoint2_8 > (FixedPoint2) 0.0f)
          damageSpecifier.DamageDict[key5] = fixedPoint2_8 * reagentDamageModifier;
      }
    }
    damageableSystem.TryChangeDamage(new EntityUid?(args.TargetEntity), damageSpecifier * fixedPoint2_1, this.IgnoreResistances, false);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EvenHealthChange target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EvenHealthChange) target1;
    if (serialization.TryCustomCopy<EvenHealthChange>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2> target2 = (Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2>) null;
    if (this.Damage == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2>>(this.Damage, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<ProtoId<DamageGroupPrototype>, FixedPoint2>>(this.Damage, hookCtx, context);
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
    ref EvenHealthChange target,
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
    EvenHealthChange target1 = (EvenHealthChange) target;
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
    EvenHealthChange target1 = (EvenHealthChange) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual EvenHealthChange EntityEffect.Instantiate() => new EvenHealthChange();
}
