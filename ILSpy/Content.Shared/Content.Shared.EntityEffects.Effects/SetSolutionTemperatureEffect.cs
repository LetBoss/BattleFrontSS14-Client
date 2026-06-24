using System;
using Content.Shared.Chemistry.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

[DataDefinition]
public sealed class SetSolutionTemperatureEffect : EntityEffect, ISerializationGenerated<SetSolutionTemperatureEffect>, ISerializationGenerated
{
	[DataField("temperature", false, 1, true, false, null)]
	private float _temperature;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-set-solution-temperature-effect", new(string, object)[2]
		{
			("chance", Probability),
			("temperature", _temperature)
		});
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			Solution solution = reagentArgs.Source;
			if (solution != null)
			{
				solution.Temperature = _temperature;
			}
			return;
		}
		throw new NotImplementedException();
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SetSolutionTemperatureEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SetSolutionTemperatureEffect)definitionCast;
		if (!serialization.TryCustomCopy<SetSolutionTemperatureEffect>(this, ref target, hookCtx, false, context))
		{
			float _temperatureTemp = 0f;
			if (!serialization.TryCustomCopy<float>(_temperature, ref _temperatureTemp, hookCtx, false, context))
			{
				_temperatureTemp = _temperature;
			}
			target._temperature = _temperatureTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SetSolutionTemperatureEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SetSolutionTemperatureEffect cast = (SetSolutionTemperatureEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SetSolutionTemperatureEffect cast = (SetSolutionTemperatureEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SetSolutionTemperatureEffect Instantiate()
	{
		return new SetSolutionTemperatureEffect();
	}
}
