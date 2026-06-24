using System;
using System.Collections.Generic;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Shared.ReagentSpeed;

public sealed class ReagentSpeedSystem : EntitySystem
{
	[Dependency]
	private SharedSolutionContainerSystem _solution;

	public TimeSpan ApplySpeed(Entity<ReagentSpeedComponent?> ent, TimeSpan time)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<ReagentSpeedComponent>(Entity<ReagentSpeedComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			return time;
		}
		if (!_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(ent.Owner), ent.Comp.Solution, out Entity<SolutionComponent>? _, out Solution solution))
		{
			return time;
		}
		foreach (KeyValuePair<ProtoId<ReagentPrototype>, float> modifier2 in ent.Comp.Modifiers)
		{
			modifier2.Deconstruct(out var key, out var value);
			ProtoId<ReagentPrototype> reagent = key;
			float fullModifier = value;
			float efficiency = (solution.RemoveReagent(ProtoId<ReagentPrototype>.op_Implicit(reagent), ent.Comp.Cost) / ent.Comp.Cost).Float();
			float reduction = (1f - fullModifier) * efficiency;
			float modifier = 1f - reduction;
			time *= (double)modifier;
		}
		return time;
	}
}
