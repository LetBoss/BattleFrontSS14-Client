// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.PlantMetabolism.PlantAdjustAttribute`1
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects.PlantMetabolism;

[ImplicitDataDefinitionForInheritors]
public abstract class PlantAdjustAttribute<T> : 
  EventEntityEffect<T>,
  ISerializationGenerated<PlantAdjustAttribute<T>>,
  ISerializationGenerated
  where T : PlantAdjustAttribute<T>
{
  [DataField(null, false, 1, false, false, null)]
  public float Amount { get; protected set; } = 1f;

  [DataField(null, false, 1, false, false, null)]
  public abstract string GuidebookAttributeName { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public virtual bool GuidebookIsAttributePositive { get; protected set; } = true;

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    string str = !(this.GuidebookIsAttributePositive ^ (double) this.Amount < 0.0) ? "red" : "green";
    return Loc.GetString("reagent-effect-guidebook-plant-attribute", ("attribute", (object) Loc.GetString(this.GuidebookAttributeName)), ("amount", (object) this.Amount.ToString("0.00")), ("colorName", (object) str), ("chance", (object) this.Probability));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref PlantAdjustAttribute<T> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EventEntityEffect<T> target1 = (EventEntityEffect<T>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PlantAdjustAttribute<T>) target1;
    if (serialization.TryCustomCopy<PlantAdjustAttribute<T>>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Amount, ref target2, hookCtx, false, context))
      target2 = this.Amount;
    target.Amount = target2;
    string target3 = (string) null;
    if (this.GuidebookAttributeName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.GuidebookAttributeName, ref target3, hookCtx, false, context))
      target3 = this.GuidebookAttributeName;
    target.GuidebookAttributeName = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.GuidebookIsAttributePositive, ref target4, hookCtx, false, context))
      target4 = this.GuidebookIsAttributePositive;
    target.GuidebookIsAttributePositive = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref PlantAdjustAttribute<T> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EventEntityEffect<T> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PlantAdjustAttribute<T> target1 = (PlantAdjustAttribute<T>) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EventEntityEffect<T>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PlantAdjustAttribute<T> target1 = (PlantAdjustAttribute<T>) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PlantAdjustAttribute<T> EventEntityEffect<T>.Instantiate()
  {
    throw new NotImplementedException();
  }
}
