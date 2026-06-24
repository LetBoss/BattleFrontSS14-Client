// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Body.SharedRMCBloodstreamSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared._RMC14.Body;

public abstract class SharedRMCBloodstreamSystem : EntitySystem
{
  [Dependency]
  private RMCReagentSystem _rmcReagent;
  [Dependency]
  private SharedSolutionContainerSystem _solution;
  private readonly List<ReagentId> _reagentsToRemove = new List<ReagentId>();

  public virtual bool TryGetBloodSolution(EntityUid uid, [NotNullWhen(true)] out Solution? solution)
  {
    return this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) uid, "bloodstream", out Entity<SolutionComponent>? _, out solution);
  }

  public virtual bool TryGetChemicalSolution(
    EntityUid uid,
    out Entity<SolutionComponent> solutionEnt,
    [NotNullWhen(true)] out Solution? solution)
  {
    solutionEnt = new Entity<SolutionComponent>();
    Entity<SolutionComponent>? entity;
    if (!this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) uid, "chemicals", out entity, out solution))
      return false;
    solutionEnt = entity.Value;
    return true;
  }

  public virtual bool IsBleeding(EntityUid uid) => false;

  public void RemoveBloodstreamToxins(EntityUid body, FixedPoint2 amount)
  {
    Entity<SolutionComponent> solutionEnt;
    if (!this.TryGetChemicalSolution(body, out solutionEnt, out Solution _))
      return;
    this._reagentsToRemove.Clear();
    foreach (ReagentQuantity content in solutionEnt.Comp.Solution.Contents)
    {
      Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
      if (this._rmcReagent.TryIndex(content.Reagent, out reagent) && reagent.Toxin)
        this._reagentsToRemove.Add(content.Reagent);
    }
    foreach (ReagentId reagentId in this._reagentsToRemove)
      this._solution.RemoveReagent(solutionEnt, reagentId, amount);
  }

  public void RemoveBloodstreamChemical(
    EntityUid body,
    ProtoId<ReagentPrototype> reagentId,
    FixedPoint2 amount)
  {
    Entity<SolutionComponent> solutionEnt;
    if (!this.TryGetChemicalSolution(body, out solutionEnt, out Solution _))
      return;
    this._solution.RemoveReagent(solutionEnt, (string) reagentId, amount);
  }

  public bool RemoveBloodstreamAlcohols(EntityUid body, FixedPoint2 amount)
  {
    Entity<SolutionComponent> solutionEnt;
    if (!this.TryGetChemicalSolution(body, out solutionEnt, out Solution _))
      return false;
    this._reagentsToRemove.Clear();
    foreach (ReagentQuantity content in solutionEnt.Comp.Solution.Contents)
    {
      Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
      if (this._rmcReagent.TryIndex(content.Reagent, out reagent) && reagent.Alcohol)
        this._reagentsToRemove.Add(content.Reagent);
    }
    bool flag = this._reagentsToRemove.Count > 0;
    foreach (ReagentId reagentId in this._reagentsToRemove)
      this._solution.RemoveReagent(solutionEnt, reagentId, amount);
    return flag;
  }
}
