// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.WashCreamPieReaction
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
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects;

public sealed class WashCreamPieReaction : 
  EntityEffect,
  ISerializationGenerated<WashCreamPieReaction>,
  ISerializationGenerated
{
  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-wash-cream-pie-reaction", ("chance", (object) this.Probability));
  }

  public override void Effect(EntityEffectBaseArgs args)
  {
    CreamPiedComponent component;
    if (!args.EntityManager.TryGetComponent<CreamPiedComponent>(args.TargetEntity, out component))
      return;
    args.EntityManager.System<SharedCreamPieSystem>().SetCreamPied(args.TargetEntity, component, false);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WashCreamPieReaction target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WashCreamPieReaction) target1;
    serialization.TryCustomCopy<WashCreamPieReaction>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WashCreamPieReaction target,
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
    WashCreamPieReaction target1 = (WashCreamPieReaction) target;
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
    WashCreamPieReaction target1 = (WashCreamPieReaction) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual WashCreamPieReaction EntityEffect.Instantiate() => new WashCreamPieReaction();
}
