// Decompiled with JetBrains decompiler
// Type: Content.Shared.Body.Systems.StomachSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Components;
using Content.Shared.Body.Events;
using Content.Shared.Body.Organ;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Body.Systems;

public sealed class StomachSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _gameTiming;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainerSystem;
  public const string DefaultSolutionName = "stomach";

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StomachComponent, MapInitEvent>(new EntityEventRefHandler<StomachComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StomachComponent, EntityUnpausedEvent>(new EntityEventRefHandler<StomachComponent, EntityUnpausedEvent>((object) this, __methodptr(OnUnpaused)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StomachComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<StomachComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnEntRemoved)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StomachComponent, ApplyMetabolicMultiplierEvent>(new EntityEventRefHandler<StomachComponent, ApplyMetabolicMultiplierEvent>((object) this, __methodptr(OnApplyMetabolicMultiplier)), (Type[]) null, (Type[]) null);
  }

  private void OnMapInit(Entity<StomachComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.NextUpdate = this._gameTiming.CurTime + ent.Comp.AdjustedUpdateInterval;
  }

  private void OnUnpaused(Entity<StomachComponent> ent, ref EntityUnpausedEvent args)
  {
    ent.Comp.NextUpdate += args.PausedTime;
  }

  private void OnEntRemoved(Entity<StomachComponent> ent, ref EntRemovedFromContainerMessage args)
  {
    Entity<SolutionComponent>? solution = ent.Comp.Solution;
    if (!solution.HasValue)
      return;
    Entity<SolutionComponent> valueOrDefault = solution.GetValueOrDefault();
    if (EntityUid.op_Inequality(((ContainerModifiedMessage) args).Entity, valueOrDefault.Owner))
      return;
    ent.Comp.Solution = new Entity<SolutionComponent>?();
  }

  public virtual void Update(float frameTime)
  {
    EntityQueryEnumerator<StomachComponent, OrganComponent, SolutionContainerManagerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<StomachComponent, OrganComponent, SolutionContainerManagerComponent>();
    EntityUid entityUid;
    StomachComponent stomachComponent;
    OrganComponent organComponent;
    SolutionContainerManagerComponent managerComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref stomachComponent, ref organComponent, ref managerComponent))
    {
      if (!(this._gameTiming.CurTime < stomachComponent.NextUpdate))
      {
        stomachComponent.NextUpdate += stomachComponent.AdjustedUpdateInterval;
        Solution solution;
        if (this._solutionContainerSystem.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((entityUid, managerComponent)), "stomach", ref stomachComponent.Solution, out solution))
        {
          EntityUid? body = organComponent.Body;
          Entity<SolutionComponent>? entity;
          if (body.HasValue && this._solutionContainerSystem.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(body.GetValueOrDefault()), stomachComponent.BodySolutionName, out entity))
          {
            Solution toAdd = new Solution();
            RemQueue<StomachComponent.ReagentDelta> remQueue = new RemQueue<StomachComponent.ReagentDelta>();
            foreach (StomachComponent.ReagentDelta reagentDelta in stomachComponent.ReagentDeltas)
            {
              reagentDelta.Increment(stomachComponent.AdjustedUpdateInterval);
              if (reagentDelta.Lifetime > stomachComponent.DigestionDelay)
              {
                ReagentQuantity quantity;
                if (solution.TryGetReagent(reagentDelta.ReagentQuantity.Reagent, out quantity))
                {
                  if (quantity.Quantity > reagentDelta.ReagentQuantity.Quantity)
                    quantity = new ReagentQuantity(quantity.Reagent, reagentDelta.ReagentQuantity.Quantity);
                  solution.RemoveReagent(quantity);
                  toAdd.AddReagent(quantity);
                }
                remQueue.Add(reagentDelta);
              }
            }
            foreach (StomachComponent.ReagentDelta reagentDelta in remQueue)
              stomachComponent.ReagentDeltas.Remove(reagentDelta);
            this._solutionContainerSystem.UpdateChemicals(stomachComponent.Solution.Value);
            this._solutionContainerSystem.TryAddSolution(entity.Value, toAdd);
          }
        }
      }
    }
  }

  private void OnApplyMetabolicMultiplier(
    Entity<StomachComponent> ent,
    ref ApplyMetabolicMultiplierEvent args)
  {
    ent.Comp.UpdateIntervalMultiplier = args.Multiplier;
  }

  public bool CanTransferSolution(
    EntityUid uid,
    Solution solution,
    StomachComponent? stomach = null,
    SolutionContainerManagerComponent? solutions = null)
  {
    Solution solution1;
    return this.Resolve<StomachComponent, SolutionContainerManagerComponent>(uid, ref stomach, ref solutions, false) && this._solutionContainerSystem.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((uid, solutions)), nameof (stomach), ref stomach.Solution, out solution1) && solution1.CanAddSolution(solution);
  }

  public bool TryTransferSolution(
    EntityUid uid,
    Solution solution,
    StomachComponent? stomach = null,
    SolutionContainerManagerComponent? solutions = null)
  {
    if (!this.Resolve<StomachComponent, SolutionContainerManagerComponent>(uid, ref stomach, ref solutions, false) || !this._solutionContainerSystem.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((uid, solutions)), nameof (stomach), ref stomach.Solution) || !this.CanTransferSolution(uid, solution, stomach, solutions))
      return false;
    this._solutionContainerSystem.TryAddSolution(stomach.Solution.Value, solution);
    foreach (ReagentQuantity content in solution.Contents)
      stomach.ReagentDeltas.Add(new StomachComponent.ReagentDelta(content));
    return true;
  }
}
