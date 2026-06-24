using System;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.EffectConditions;

public sealed class Temperature : EventEntityEffectCondition<Temperature>, ISerializationGenerated<Temperature>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Min;

	[DataField(null, false, 1, false, false, null)]
	public float Max = float.PositiveInfinity;

	public override string GuidebookExplanation(IPrototypeManager prototype)
	{
		return Loc.GetString("reagent-effect-condition-guidebook-body-temperature", new(string, object)[2]
		{
			("max", float.IsPositiveInfinity(Max) ? 2.1474836E+09f : Max),
			("min", Min)
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Temperature target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffectCondition<Temperature> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Temperature)definitionCast;
		if (!serialization.TryCustomCopy<Temperature>(this, ref target, hookCtx, false, context))
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
	public void Copy(ref Temperature target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffectCondition<Temperature> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Temperature cast = (Temperature)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Temperature cast = (Temperature)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Temperature Instantiate()
	{
		return new Temperature();
	}
}
