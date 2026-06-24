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
public sealed class AdjustSolutionTemperatureEffect : EntityEffect, ISerializationGenerated<AdjustSolutionTemperatureEffect>, ISerializationGenerated
{
	[DataField("delta", false, 1, true, false, null)]
	private float _delta;

	[DataField("minTemp", false, 1, false, false, null)]
	private float _minTemp;

	[DataField("maxTemp", false, 1, false, false, null)]
	private float _maxTemp = float.PositiveInfinity;

	[DataField("scaled", false, 1, false, false, null)]
	private bool _scaled;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-adjust-solution-temperature-effect", new(string, object)[4]
		{
			("chance", Probability),
			("deltasign", MathF.Sign(_delta)),
			("mintemp", _minTemp),
			("maxtemp", _maxTemp)
		});
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			Solution solution = reagentArgs.Source;
			if (solution != null && !(solution.Volume == 0))
			{
				float deltaT = (_scaled ? (_delta * (float)reagentArgs.Quantity) : _delta);
				solution.Temperature = Math.Clamp(solution.Temperature + deltaT, _minTemp, _maxTemp);
			}
			return;
		}
		throw new NotImplementedException();
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AdjustSolutionTemperatureEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AdjustSolutionTemperatureEffect)definitionCast;
		if (!serialization.TryCustomCopy<AdjustSolutionTemperatureEffect>(this, ref target, hookCtx, false, context))
		{
			float _deltaTemp = 0f;
			if (!serialization.TryCustomCopy<float>(_delta, ref _deltaTemp, hookCtx, false, context))
			{
				_deltaTemp = _delta;
			}
			target._delta = _deltaTemp;
			float _minTempTemp = 0f;
			if (!serialization.TryCustomCopy<float>(_minTemp, ref _minTempTemp, hookCtx, false, context))
			{
				_minTempTemp = _minTemp;
			}
			target._minTemp = _minTempTemp;
			float _maxTempTemp = 0f;
			if (!serialization.TryCustomCopy<float>(_maxTemp, ref _maxTempTemp, hookCtx, false, context))
			{
				_maxTempTemp = _maxTemp;
			}
			target._maxTemp = _maxTempTemp;
			bool _scaledTemp = false;
			if (!serialization.TryCustomCopy<bool>(_scaled, ref _scaledTemp, hookCtx, false, context))
			{
				_scaledTemp = _scaled;
			}
			target._scaled = _scaledTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AdjustSolutionTemperatureEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AdjustSolutionTemperatureEffect cast = (AdjustSolutionTemperatureEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AdjustSolutionTemperatureEffect cast = (AdjustSolutionTemperatureEffect)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AdjustSolutionTemperatureEffect Instantiate()
	{
		return new AdjustSolutionTemperatureEffect();
	}
}
