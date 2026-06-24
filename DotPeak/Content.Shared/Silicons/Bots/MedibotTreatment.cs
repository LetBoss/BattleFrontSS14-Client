// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Bots.MedibotTreatment
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Silicons.Bots;

[DataDefinition]
public sealed class MedibotTreatment : 
  ISerializationGenerated<MedibotTreatment>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<ReagentPrototype> Reagent = (ProtoId<ReagentPrototype>) string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public FixedPoint2 Quantity;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2? MinDamage;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2? MaxDamage;

  public bool IsValid(FixedPoint2 damage)
  {
    FixedPoint2? nullable;
    if (this.MaxDamage.HasValue)
    {
      FixedPoint2 fixedPoint2 = damage;
      nullable = this.MaxDamage;
      if ((nullable.HasValue ? (fixedPoint2 < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
        return false;
    }
    if (!this.MinDamage.HasValue)
      return true;
    FixedPoint2 fixedPoint2_1 = damage;
    nullable = this.MinDamage;
    return nullable.HasValue && fixedPoint2_1 > nullable.GetValueOrDefault();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MedibotTreatment target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<MedibotTreatment>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ReagentPrototype> target1 = new ProtoId<ReagentPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ReagentPrototype>>(this.Reagent, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<ProtoId<ReagentPrototype>>(this.Reagent, hookCtx, context);
    target.Reagent = target1;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Quantity, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.Quantity, hookCtx, context);
    target.Quantity = target2;
    FixedPoint2? target3 = new FixedPoint2?();
    if (!serialization.TryCustomCopy<FixedPoint2?>(this.MinDamage, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2?>(this.MinDamage, hookCtx, context);
    target.MinDamage = target3;
    FixedPoint2? target4 = new FixedPoint2?();
    if (!serialization.TryCustomCopy<FixedPoint2?>(this.MaxDamage, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2?>(this.MaxDamage, hookCtx, context);
    target.MaxDamage = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MedibotTreatment target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MedibotTreatment target1 = (MedibotTreatment) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public MedibotTreatment Instantiate() => new MedibotTreatment();
}
