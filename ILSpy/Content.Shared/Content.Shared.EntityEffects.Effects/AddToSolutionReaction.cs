using System;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.EntityEffects.Effects;

public sealed class AddToSolutionReaction : EntityEffect, ISerializationGenerated<AddToSolutionReaction>, ISerializationGenerated
{
	[DataField("solution", false, 1, false, false, null)]
	private string _solution = "reagents";

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			if (reagentArgs.Reagent != null)
			{
				SharedSolutionContainerSystem solutionContainerSystem = reagentArgs.EntityManager.System<SharedSolutionContainerSystem>();
				if (solutionContainerSystem.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(reagentArgs.TargetEntity), _solution, out Entity<SolutionComponent>? solutionContainer) && solutionContainerSystem.TryAddReagent(solutionContainer.Value, reagentArgs.Reagent.ID, reagentArgs.Quantity, out var accepted))
				{
					reagentArgs.Source?.RemoveReagent(reagentArgs.Reagent.ID, accepted);
				}
			}
			return;
		}
		throw new NotImplementedException();
	}

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-add-to-solution-reaction", new(string, object)[1] { ("chance", Probability) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AddToSolutionReaction target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AddToSolutionReaction)definitionCast;
		if (!serialization.TryCustomCopy<AddToSolutionReaction>(this, ref target, hookCtx, false, context))
		{
			string _solutionTemp = null;
			if (_solution == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(_solution, ref _solutionTemp, hookCtx, false, context))
			{
				_solutionTemp = _solution;
			}
			target._solution = _solutionTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AddToSolutionReaction target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AddToSolutionReaction cast = (AddToSolutionReaction)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AddToSolutionReaction cast = (AddToSolutionReaction)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AddToSolutionReaction Instantiate()
	{
		return new AddToSolutionReaction();
	}
}
