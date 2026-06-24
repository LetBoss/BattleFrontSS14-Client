using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

[DataDefinition]
public sealed class EmpReactionEffect : EventEntityEffect<EmpReactionEffect>, ISerializationGenerated<EmpReactionEffect>, ISerializationGenerated
{
	[DataField("rangePerUnit", false, 1, false, false, null)]
	public float EmpRangePerUnit = 0.5f;

	[DataField("maxRange", false, 1, false, false, null)]
	public float EmpMaxRange = 10f;

	[DataField(null, false, 1, false, false, null)]
	public float EnergyConsumption = 12500f;

	[DataField("duration", false, 1, false, false, null)]
	public float DisableDuration = 15f;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-emp-reaction-effect", new(string, object)[1] { ("chance", Probability) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EmpReactionEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<EmpReactionEffect> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EmpReactionEffect)definitionCast;
		if (!serialization.TryCustomCopy<EmpReactionEffect>(this, ref target, hookCtx, false, context))
		{
			float EmpRangePerUnitTemp = 0f;
			if (!serialization.TryCustomCopy<float>(EmpRangePerUnit, ref EmpRangePerUnitTemp, hookCtx, false, context))
			{
				EmpRangePerUnitTemp = EmpRangePerUnit;
			}
			target.EmpRangePerUnit = EmpRangePerUnitTemp;
			float EmpMaxRangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(EmpMaxRange, ref EmpMaxRangeTemp, hookCtx, false, context))
			{
				EmpMaxRangeTemp = EmpMaxRange;
			}
			target.EmpMaxRange = EmpMaxRangeTemp;
			float EnergyConsumptionTemp = 0f;
			if (!serialization.TryCustomCopy<float>(EnergyConsumption, ref EnergyConsumptionTemp, hookCtx, false, context))
			{
				EnergyConsumptionTemp = EnergyConsumption;
			}
			target.EnergyConsumption = EnergyConsumptionTemp;
			float DisableDurationTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DisableDuration, ref DisableDurationTemp, hookCtx, false, context))
			{
				DisableDurationTemp = DisableDuration;
			}
			target.DisableDuration = DisableDurationTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EmpReactionEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<EmpReactionEffect> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmpReactionEffect cast = (EmpReactionEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EmpReactionEffect cast = (EmpReactionEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EmpReactionEffect Instantiate()
	{
		return new EmpReactionEffect();
	}
}
