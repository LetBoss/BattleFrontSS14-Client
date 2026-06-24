// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.FoodMetamorphRules.FoodHasReagent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Destructible.Thresholds;
using Content.Shared.FixedPoint;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nutrition.FoodMetamorphRules;

[NetSerializable]
[Serializable]
public sealed class FoodHasReagent : 
  FoodMetamorphRule,
  ISerializationGenerated<FoodHasReagent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<ReagentPrototype> Reagent;
  [DataField(null, false, 1, true, false, null)]
  public MinMax Count;
  [DataField(null, false, 1, false, false, null)]
  public string Solution = "food";

  public override bool Check(
    IPrototypeManager protoMan,
    EntityManager entMan,
    EntityUid food,
    List<FoodSequenceVisualLayer> ingredients)
  {
    Entity<SolutionComponent>? entity;
    if (!entMan.TryGetComponent<SolutionContainerManagerComponent>(food, out SolutionContainerManagerComponent _) || !entMan.System<SharedSolutionContainerSystem>().TryGetSolution((Entity<SolutionContainerManagerComponent>) food, this.Solution, out entity, out Content.Shared.Chemistry.Components.Solution _))
      return false;
    foreach ((ReagentId id, FixedPoint2 quantity) in entity.Value.Comp.Solution.Contents)
    {
      if (!(id.Prototype != this.Reagent.Id))
      {
        if (!(quantity < this.Count.Min))
        {
          if (!(quantity > this.Count.Max))
            return true;
          break;
        }
        break;
      }
    }
    return false;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FoodHasReagent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FoodMetamorphRule target1 = (FoodMetamorphRule) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FoodHasReagent) target1;
    if (serialization.TryCustomCopy<FoodHasReagent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<ReagentPrototype> target2 = new ProtoId<ReagentPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ReagentPrototype>>(this.Reagent, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ProtoId<ReagentPrototype>>(this.Reagent, hookCtx, context);
    target.Reagent = target2;
    MinMax target3 = new MinMax();
    if (!serialization.TryCustomCopy<MinMax>(this.Count, ref target3, hookCtx, false, context))
      serialization.CopyTo<MinMax>(this.Count, ref target3, hookCtx, context);
    target.Count = target3;
    string target4 = (string) null;
    if (this.Solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Solution, ref target4, hookCtx, false, context))
      target4 = this.Solution;
    target.Solution = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FoodHasReagent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref FoodMetamorphRule target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FoodHasReagent target1 = (FoodHasReagent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (FoodMetamorphRule) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    FoodHasReagent target1 = (FoodHasReagent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual FoodHasReagent FoodMetamorphRule.Instantiate() => new FoodHasReagent();
}
