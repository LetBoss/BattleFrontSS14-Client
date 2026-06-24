// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.EffectConditions.Internals
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Components;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class Internals : 
  EntityEffectCondition,
  ISerializationGenerated<Internals>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool UsingInternals = true;

  public override bool Condition(EntityEffectBaseArgs args)
  {
    InternalsComponent component;
    return !args.EntityManager.TryGetComponent<InternalsComponent>(args.TargetEntity, out component) ? !this.UsingInternals : this.UsingInternals == component.GasTankEntity.HasValue;
  }

  public override string GuidebookExplanation(IPrototypeManager prototype)
  {
    return Loc.GetString("reagent-effect-condition-guidebook-internals", ("usingInternals", (object) this.UsingInternals));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref Internals target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffectCondition target1 = (EntityEffectCondition) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (Internals) target1;
    if (serialization.TryCustomCopy<Internals>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.UsingInternals, ref target2, hookCtx, false, context))
      target2 = this.UsingInternals;
    target.UsingInternals = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref Internals target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref EntityEffectCondition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Internals target1 = (Internals) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (EntityEffectCondition) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Internals target1 = (Internals) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual Internals EntityEffectCondition.Instantiate() => new Internals();
}
