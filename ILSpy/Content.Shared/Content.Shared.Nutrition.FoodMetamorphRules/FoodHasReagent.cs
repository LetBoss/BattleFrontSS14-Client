using System;
using System.Collections.Generic;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Destructible.Thresholds;
using Content.Shared.Nutrition.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Nutrition.FoodMetamorphRules;

[Serializable]
[NetSerializable]
public sealed class FoodHasReagent : FoodMetamorphRule, ISerializationGenerated<FoodHasReagent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<ReagentPrototype> Reagent;

	[DataField(null, false, 1, true, false, null)]
	public MinMax Count;

	[DataField(null, false, 1, false, false, null)]
	public string Solution = "food";

	public override bool Check(IPrototypeManager protoMan, EntityManager entMan, EntityUid food, List<FoodSequenceVisualLayer> ingredients)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		SolutionContainerManagerComponent solMan = default(SolutionContainerManagerComponent);
		if (!entMan.TryGetComponent<SolutionContainerManagerComponent>(food, ref solMan))
		{
			return false;
		}
		if (!entMan.System<SharedSolutionContainerSystem>().TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(food), Solution, out Entity<SolutionComponent>? foodSoln, out Solution _))
		{
			return false;
		}
		foreach (var (id, quantity) in foodSoln.Value.Comp.Solution.Contents)
		{
			if (!(id.Prototype != Reagent.Id))
			{
				if (quantity < Count.Min || quantity > Count.Max)
				{
					break;
				}
				return true;
			}
		}
		return false;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FoodHasReagent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		FoodMetamorphRule definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FoodHasReagent)definitionCast;
		if (!serialization.TryCustomCopy<FoodHasReagent>(this, ref target, hookCtx, false, context))
		{
			ProtoId<ReagentPrototype> ReagentTemp = default(ProtoId<ReagentPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<ReagentPrototype>>(Reagent, ref ReagentTemp, hookCtx, false, context))
			{
				ReagentTemp = serialization.CreateCopy<ProtoId<ReagentPrototype>>(Reagent, hookCtx, context, false);
			}
			target.Reagent = ReagentTemp;
			MinMax CountTemp = default(MinMax);
			if (!serialization.TryCustomCopy<MinMax>(Count, ref CountTemp, hookCtx, false, context))
			{
				serialization.CopyTo<MinMax>(Count, ref CountTemp, hookCtx, context, false);
			}
			target.Count = CountTemp;
			string SolutionTemp = null;
			if (Solution == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Solution, ref SolutionTemp, hookCtx, false, context))
			{
				SolutionTemp = Solution;
			}
			target.Solution = SolutionTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FoodHasReagent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref FoodMetamorphRule target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodHasReagent cast = (FoodHasReagent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FoodHasReagent cast = (FoodHasReagent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FoodHasReagent Instantiate()
	{
		return new FoodHasReagent();
	}
}
