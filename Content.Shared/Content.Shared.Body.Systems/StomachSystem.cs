using System;
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

namespace Content.Shared.Body.Systems;

public sealed class StomachSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private SharedSolutionContainerSystem _solutionContainerSystem;

	public const string DefaultSolutionName = "stomach";

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<StomachComponent, MapInitEvent>((EntityEventRefHandler<StomachComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StomachComponent, EntityUnpausedEvent>((EntityEventRefHandler<StomachComponent, EntityUnpausedEvent>)OnUnpaused, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StomachComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<StomachComponent, EntRemovedFromContainerMessage>)OnEntRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StomachComponent, ApplyMetabolicMultiplierEvent>((EntityEventRefHandler<StomachComponent, ApplyMetabolicMultiplierEvent>)OnApplyMetabolicMultiplier, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<StomachComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.NextUpdate = _gameTiming.CurTime + ent.Comp.AdjustedUpdateInterval;
	}

	private void OnUnpaused(Entity<StomachComponent> ent, ref EntityUnpausedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.NextUpdate += args.PausedTime;
	}

	private void OnEntRemoved(Entity<StomachComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Entity<SolutionComponent>? solution = ent.Comp.Solution;
		if (solution.HasValue)
		{
			Entity<SolutionComponent> solution2 = solution.GetValueOrDefault();
			if (!(((ContainerModifiedMessage)args).Entity != solution2.Owner))
			{
				ent.Comp.Solution = null;
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryEnumerator<StomachComponent, OrganComponent, SolutionContainerManagerComponent> query = ((EntitySystem)this).EntityQueryEnumerator<StomachComponent, OrganComponent, SolutionContainerManagerComponent>();
		EntityUid uid = default(EntityUid);
		StomachComponent stomach = default(StomachComponent);
		OrganComponent organ = default(OrganComponent);
		SolutionContainerManagerComponent sol = default(SolutionContainerManagerComponent);
		while (query.MoveNext(ref uid, ref stomach, ref organ, ref sol))
		{
			if (_gameTiming.CurTime < stomach.NextUpdate)
			{
				continue;
			}
			stomach.NextUpdate += stomach.AdjustedUpdateInterval;
			if (!_solutionContainerSystem.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((uid, sol)), "stomach", ref stomach.Solution, out Solution stomachSolution))
			{
				continue;
			}
			EntityUid? body = organ.Body;
			if (!body.HasValue)
			{
				continue;
			}
			EntityUid body2 = body.GetValueOrDefault();
			if (!_solutionContainerSystem.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(body2), stomach.BodySolutionName, out Entity<SolutionComponent>? bodySolution))
			{
				continue;
			}
			Solution transferSolution = new Solution();
			RemQueue<StomachComponent.ReagentDelta> queue = default(RemQueue<StomachComponent.ReagentDelta>);
			foreach (StomachComponent.ReagentDelta delta in stomach.ReagentDeltas)
			{
				delta.Increment(stomach.AdjustedUpdateInterval);
				if (!(delta.Lifetime > stomach.DigestionDelay))
				{
					continue;
				}
				if (stomachSolution.TryGetReagent(delta.ReagentQuantity.Reagent, out var reagent))
				{
					if (reagent.Quantity > delta.ReagentQuantity.Quantity)
					{
						reagent = new ReagentQuantity(reagent.Reagent, delta.ReagentQuantity.Quantity);
					}
					stomachSolution.RemoveReagent(reagent);
					transferSolution.AddReagent(reagent);
				}
				queue.Add(delta);
			}
			Enumerator<StomachComponent.ReagentDelta> enumerator2 = queue.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					StomachComponent.ReagentDelta item = enumerator2.Current;
					stomach.ReagentDeltas.Remove(item);
				}
			}
			finally
			{
				((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
			}
			_solutionContainerSystem.UpdateChemicals(stomach.Solution.Value);
			_solutionContainerSystem.TryAddSolution(bodySolution.Value, transferSolution);
		}
	}

	private void OnApplyMetabolicMultiplier(Entity<StomachComponent> ent, ref ApplyMetabolicMultiplierEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.UpdateIntervalMultiplier = args.Multiplier;
	}

	public bool CanTransferSolution(EntityUid uid, Solution solution, StomachComponent? stomach = null, SolutionContainerManagerComponent? solutions = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StomachComponent, SolutionContainerManagerComponent>(uid, ref stomach, ref solutions, false) && _solutionContainerSystem.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((uid, solutions)), "stomach", ref stomach.Solution, out Solution stomachSolution))
		{
			return stomachSolution.CanAddSolution(solution);
		}
		return false;
	}

	public bool TryTransferSolution(EntityUid uid, Solution solution, StomachComponent? stomach = null, SolutionContainerManagerComponent? solutions = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StomachComponent, SolutionContainerManagerComponent>(uid, ref stomach, ref solutions, false) || !_solutionContainerSystem.ResolveSolution(Entity<SolutionContainerManagerComponent>.op_Implicit((uid, solutions)), "stomach", ref stomach.Solution) || !CanTransferSolution(uid, solution, stomach, solutions))
		{
			return false;
		}
		_solutionContainerSystem.TryAddSolution(stomach.Solution.Value, solution);
		foreach (ReagentQuantity reagent in solution.Contents)
		{
			stomach.ReagentDeltas.Add(new StomachComponent.ReagentDelta(reagent));
		}
		return true;
	}
}
