using System;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class SolutionTemperature : EntityEffectCondition, ISerializationGenerated<SolutionTemperature>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Min;

	[DataField(null, false, 1, false, false, null)]
	public float Max = float.PositiveInfinity;

	public override bool Condition(EntityEffectBaseArgs args)
	{
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			if (reagentArgs.Source == null)
			{
				return false;
			}
			if (reagentArgs.Source.Temperature < Min)
			{
				return false;
			}
			if (reagentArgs.Source.Temperature > Max)
			{
				return false;
			}
			return true;
		}
		throw new NotImplementedException();
	}

	public override string GuidebookExplanation(IPrototypeManager prototype)
	{
		return Loc.GetString("reagent-effect-condition-guidebook-solution-temperature", new(string, object)[2]
		{
			("max", float.IsPositiveInfinity(Max) ? 2.1474836E+09f : Max),
			("min", Min)
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SolutionTemperature target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffectCondition definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SolutionTemperature)definitionCast;
		if (!serialization.TryCustomCopy<SolutionTemperature>(this, ref target, hookCtx, false, context))
		{
			float MinTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Min, ref MinTemp, hookCtx, false, context))
			{
				MinTemp = Min;
			}
			target.Min = MinTemp;
			float MaxTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Max, ref MaxTemp, hookCtx, false, context))
			{
				MaxTemp = Max;
			}
			target.Max = MaxTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SolutionTemperature target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffectCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SolutionTemperature cast = (SolutionTemperature)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SolutionTemperature cast = (SolutionTemperature)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SolutionTemperature Instantiate()
	{
		return new SolutionTemperature();
	}
}
