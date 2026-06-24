using System;
using Content.Shared.Atmos;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Power.Generator;

[RegisterComponent]
public sealed class GeneratorExhaustGasComponent : Component, ISerializationGenerated<GeneratorExhaustGasComponent>, ISerializationGenerated
{
	[DataField("gasType", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Gas GasType = Gas.CarbonDioxide;

	[DataField("moleRatio", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MoleRatio = 1f;

	[DataField("temperature", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float Temperature = 373.15f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GeneratorExhaustGasComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GeneratorExhaustGasComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GeneratorExhaustGasComponent>(this, ref target, hookCtx, false, context))
		{
			Gas GasTypeTemp = Gas.Oxygen;
			if (!serialization.TryCustomCopy<Gas>(GasType, ref GasTypeTemp, hookCtx, false, context))
			{
				GasTypeTemp = GasType;
			}
			target.GasType = GasTypeTemp;
			float MoleRatioTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MoleRatio, ref MoleRatioTemp, hookCtx, false, context))
			{
				MoleRatioTemp = MoleRatio;
			}
			target.MoleRatio = MoleRatioTemp;
			float TemperatureTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Temperature, ref TemperatureTemp, hookCtx, false, context))
			{
				TemperatureTemp = Temperature;
			}
			target.Temperature = TemperatureTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GeneratorExhaustGasComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GeneratorExhaustGasComponent cast = (GeneratorExhaustGasComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GeneratorExhaustGasComponent cast = (GeneratorExhaustGasComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GeneratorExhaustGasComponent def = (GeneratorExhaustGasComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GeneratorExhaustGasComponent Instantiate()
	{
		return new GeneratorExhaustGasComponent();
	}
}
