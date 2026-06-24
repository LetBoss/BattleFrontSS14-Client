// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.AddToSolutionReaction
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
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
namespace Content.Shared.EntityEffects.Effects;

public sealed class AddToSolutionReaction : 
  EntityEffect,
  ISerializationGenerated<AddToSolutionReaction>,
  ISerializationGenerated
{
  [DataField("solution", false, 1, false, false, null)]
  private string _solution = "reagents";

  public override void Effect(EntityEffectBaseArgs args)
  {
    EntityEffectReagentArgs effectReagentArgs = args as EntityEffectReagentArgs;
    if ((object) effectReagentArgs == null)
      throw new NotImplementedException();
    if (effectReagentArgs.Reagent == null)
      return;
    SharedSolutionContainerSystem solutionContainerSystem = effectReagentArgs.EntityManager.System<SharedSolutionContainerSystem>();
    Entity<SolutionComponent>? entity;
    FixedPoint2 acceptedQuantity;
    if (!solutionContainerSystem.TryGetSolution((Entity<SolutionContainerManagerComponent>) effectReagentArgs.TargetEntity, this._solution, out entity) || !solutionContainerSystem.TryAddReagent(entity.Value, effectReagentArgs.Reagent.ID, effectReagentArgs.Quantity, out acceptedQuantity))
      return;
    effectReagentArgs.Source?.RemoveReagent(effectReagentArgs.Reagent.ID, acceptedQuantity);
  }

  protected override string? ReagentEffectGuidebookText(
    IPrototypeManager prototype,
    IEntitySystemManager entSys)
  {
    return Loc.GetString("reagent-effect-guidebook-add-to-solution-reaction", ("chance", (object) this.Probability));
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AddToSolutionReaction target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EntityEffect target1 = (EntityEffect) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (AddToSolutionReaction) target1;
    if (serialization.TryCustomCopy<AddToSolutionReaction>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this._solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this._solution, ref target2, hookCtx, false, context))
      target2 = this._solution;
    target._solution = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AddToSolutionReaction target,
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
    AddToSolutionReaction target1 = (AddToSolutionReaction) target;
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
    AddToSolutionReaction target1 = (AddToSolutionReaction) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual AddToSolutionReaction EntityEffect.Instantiate() => new AddToSolutionReaction();
}
