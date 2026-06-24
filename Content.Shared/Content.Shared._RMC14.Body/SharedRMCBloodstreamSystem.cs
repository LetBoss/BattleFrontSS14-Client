using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent>? entity;
		return _solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(uid), "bloodstream", out entity, out solution);
	}

	public virtual bool TryGetChemicalSolution(EntityUid uid, out Entity<SolutionComponent> solutionEnt, [NotNullWhen(true)] out Solution? solution)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		solutionEnt = default(Entity<SolutionComponent>);
		if (!_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(uid), "chemicals", out Entity<SolutionComponent>? nullableSolutionEnt, out solution))
		{
			return false;
		}
		solutionEnt = nullableSolutionEnt.Value;
		return true;
	}

	public virtual bool IsBleeding(EntityUid uid)
	{
		return false;
	}

	public void RemoveBloodstreamToxins(EntityUid body, FixedPoint2 amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetChemicalSolution(body, out Entity<SolutionComponent> solutionEnt, out Solution _))
		{
			return;
		}
		_reagentsToRemove.Clear();
		foreach (ReagentQuantity content in solutionEnt.Comp.Solution.Contents)
		{
			if (_rmcReagent.TryIndex(content.Reagent, out Reagent reagent) && reagent.Toxin)
			{
				_reagentsToRemove.Add(content.Reagent);
			}
		}
		foreach (ReagentId remove in _reagentsToRemove)
		{
			_solution.RemoveReagent(solutionEnt, remove, amount);
		}
	}

	public void RemoveBloodstreamChemical(EntityUid body, ProtoId<ReagentPrototype> reagentId, FixedPoint2 amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetChemicalSolution(body, out Entity<SolutionComponent> solutionEnt, out Solution _))
		{
			_solution.RemoveReagent(solutionEnt, ProtoId<ReagentPrototype>.op_Implicit(reagentId), amount);
		}
	}

	public bool RemoveBloodstreamAlcohols(EntityUid body, FixedPoint2 amount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetChemicalSolution(body, out Entity<SolutionComponent> solutionEnt, out Solution _))
		{
			return false;
		}
		_reagentsToRemove.Clear();
		foreach (ReagentQuantity content in solutionEnt.Comp.Solution.Contents)
		{
			if (_rmcReagent.TryIndex(content.Reagent, out Reagent reagent) && reagent.Alcohol)
			{
				_reagentsToRemove.Add(content.Reagent);
			}
		}
		bool alcoholRemoved = _reagentsToRemove.Count > 0;
		foreach (ReagentId remove in _reagentsToRemove)
		{
			_solution.RemoveReagent(solutionEnt, remove, amount);
		}
		return alcoholRemoved;
	}
}
