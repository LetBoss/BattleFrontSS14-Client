// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.Effects.RemoveDamage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.EntityEffects;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.Effects;

public sealed class RemoveDamage : 
  EntityEffect,
  ISerializationGenerated<RemoveDamage>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [JsonPropertyName("group")]
  public ProtoId<DamageGroupPrototype> Group;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    DamageGroupPrototype prototype1;
    return !prototype.TryIndex<DamageGroupPrototype>(this.Group, out prototype1) ? (string) null : $"Removes all {prototype1.LocalizedName} damage";
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    DamageableComponent component;
    DamageGroupPrototype prototype;
    if ((object) effectReagentArgs != null && effectReagentArgs.Scale < (FixedPoint2) 0.95f || !args.EntityManager.TryGetComponent<DamageableComponent>(args.TargetEntity, out component) || !IoCManager.Resolve<IPrototypeManager>().TryIndex<DamageGroupPrototype>(this.Group, out prototype))
      return;
    DamageSpecifier damage = new DamageSpecifier();
    foreach (string damageType in prototype.DamageTypes)
    {
      FixedPoint2 fixedPoint2;
      if (component.Damage.DamageDict.TryGetValue(damageType, out fixedPoint2))
        damage.DamageDict[damageType] = -fixedPoint2;
    }
    args.EntityManager.System<DamageableSystem>().TryChangeDamage(new EntityUid?(args.TargetEntity), damage, true, false);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RemoveDamage target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RemoveDamage) target1;
    if (serialization.TryCustomCopy<RemoveDamage>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<DamageGroupPrototype> target2 = new ProtoId<DamageGroupPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DamageGroupPrototype>>(this.Group, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<DamageGroupPrototype>>(this.Group, hookCtx, context);
    target.Group = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RemoveDamage target,
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
    RemoveDamage target1 = (RemoveDamage) target;
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
    RemoveDamage target1 = (RemoveDamage) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual RemoveDamage EntityEffect.Instantiate() => new RemoveDamage();
}
