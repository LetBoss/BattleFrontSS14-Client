using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Anomaly.Effects.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedGravityAnomalySystem) })]
public sealed class GravityAnomalyComponent : Component, ISerializationGenerated<GravityAnomalyComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MaxGravityWellRange = 10f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MaxThrowRange = 5f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MaxThrowStrength = 10f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MaxRadiationIntensity = 3f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MinAccel;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MaxAccel = 5f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MinRadialAccel;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MaxRadialAccel = 5f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MinSpeed = 0.1f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float MaxSpeed = 1f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float SpeedVariation = 0.1f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float SpaceRange = 3f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GravityAnomalyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GravityAnomalyComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<GravityAnomalyComponent>(this, ref target, hookCtx, false, context))
		{
			float MaxGravityWellRangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxGravityWellRange, ref MaxGravityWellRangeTemp, hookCtx, false, context))
			{
				MaxGravityWellRangeTemp = MaxGravityWellRange;
			}
			target.MaxGravityWellRange = MaxGravityWellRangeTemp;
			float MaxThrowRangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxThrowRange, ref MaxThrowRangeTemp, hookCtx, false, context))
			{
				MaxThrowRangeTemp = MaxThrowRange;
			}
			target.MaxThrowRange = MaxThrowRangeTemp;
			float MaxThrowStrengthTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxThrowStrength, ref MaxThrowStrengthTemp, hookCtx, false, context))
			{
				MaxThrowStrengthTemp = MaxThrowStrength;
			}
			target.MaxThrowStrength = MaxThrowStrengthTemp;
			float MaxRadiationIntensityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxRadiationIntensity, ref MaxRadiationIntensityTemp, hookCtx, false, context))
			{
				MaxRadiationIntensityTemp = MaxRadiationIntensity;
			}
			target.MaxRadiationIntensity = MaxRadiationIntensityTemp;
			float MinAccelTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MinAccel, ref MinAccelTemp, hookCtx, false, context))
			{
				MinAccelTemp = MinAccel;
			}
			target.MinAccel = MinAccelTemp;
			float MaxAccelTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxAccel, ref MaxAccelTemp, hookCtx, false, context))
			{
				MaxAccelTemp = MaxAccel;
			}
			target.MaxAccel = MaxAccelTemp;
			float MinRadialAccelTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MinRadialAccel, ref MinRadialAccelTemp, hookCtx, false, context))
			{
				MinRadialAccelTemp = MinRadialAccel;
			}
			target.MinRadialAccel = MinRadialAccelTemp;
			float MaxRadialAccelTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxRadialAccel, ref MaxRadialAccelTemp, hookCtx, false, context))
			{
				MaxRadialAccelTemp = MaxRadialAccel;
			}
			target.MaxRadialAccel = MaxRadialAccelTemp;
			float MinSpeedTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MinSpeed, ref MinSpeedTemp, hookCtx, false, context))
			{
				MinSpeedTemp = MinSpeed;
			}
			target.MinSpeed = MinSpeedTemp;
			float MaxSpeedTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxSpeed, ref MaxSpeedTemp, hookCtx, false, context))
			{
				MaxSpeedTemp = MaxSpeed;
			}
			target.MaxSpeed = MaxSpeedTemp;
			float SpeedVariationTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpeedVariation, ref SpeedVariationTemp, hookCtx, false, context))
			{
				SpeedVariationTemp = SpeedVariation;
			}
			target.SpeedVariation = SpeedVariationTemp;
			float SpaceRangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpaceRange, ref SpaceRangeTemp, hookCtx, false, context))
			{
				SpaceRangeTemp = SpaceRange;
			}
			target.SpaceRange = SpaceRangeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GravityAnomalyComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GravityAnomalyComponent cast = (GravityAnomalyComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GravityAnomalyComponent cast = (GravityAnomalyComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GravityAnomalyComponent def = (GravityAnomalyComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GravityAnomalyComponent Instantiate()
	{
		return new GravityAnomalyComponent();
	}
}
