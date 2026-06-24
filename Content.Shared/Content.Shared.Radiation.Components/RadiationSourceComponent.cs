using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Radiation.Components;

[RegisterComponent]
public sealed class RadiationSourceComponent : Component, ISerializationGenerated<RadiationSourceComponent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("intensity", false, 1, false, false, null)]
	public float Intensity = 1f;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField("slope", false, 1, false, false, null)]
	public float Slope = 0.5f;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool Enabled = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RadiationSourceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RadiationSourceComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<RadiationSourceComponent>(this, ref target, hookCtx, false, context))
		{
			float IntensityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Intensity, ref IntensityTemp, hookCtx, false, context))
			{
				IntensityTemp = Intensity;
			}
			target.Intensity = IntensityTemp;
			float SlopeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Slope, ref SlopeTemp, hookCtx, false, context))
			{
				SlopeTemp = Slope;
			}
			target.Slope = SlopeTemp;
			bool EnabledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Enabled, ref EnabledTemp, hookCtx, false, context))
			{
				EnabledTemp = Enabled;
			}
			target.Enabled = EnabledTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RadiationSourceComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RadiationSourceComponent cast = (RadiationSourceComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RadiationSourceComponent cast = (RadiationSourceComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RadiationSourceComponent def = (RadiationSourceComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RadiationSourceComponent Instantiate()
	{
		return new RadiationSourceComponent();
	}
}
