// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.SatiateThirst
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class SatiateThirst : 
  EntityEffect,
  ISerializationGenerated<SatiateThirst>,
  ISerializationGenerated
{
  private const float DefaultHydrationFactor = 3f;

  [DataField("factor", false, 1, false, false, null)]
  public float HydrationFactor { get; set; } = 3f;

  public override void Effect(EntityEffectBaseArgs args)
  {
    EntityUid targetEntity = args.TargetEntity;
    ThirstComponent component;
    if (!args.EntityManager.TryGetComponent<ThirstComponent>(targetEntity, out component))
      return;
    args.EntityManager.System<ThirstSystem>().ModifyThirst(targetEntity, component, this.HydrationFactor);
  }

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-satiate-thirst", ("chance", (object) this.Probability), ("relative", (object) (float) ((double) this.HydrationFactor / 3.0)));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SatiateThirst target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SatiateThirst) target1;
    if (serialization.TryCustomCopy<SatiateThirst>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.HydrationFactor, ref target2, hookCtx, false, context))
      target2 = this.HydrationFactor;
    target.HydrationFactor = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SatiateThirst target,
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
    SatiateThirst target1 = (SatiateThirst) target;
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
    SatiateThirst target1 = (SatiateThirst) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SatiateThirst EntityEffect.Instantiate() => new SatiateThirst();
}
