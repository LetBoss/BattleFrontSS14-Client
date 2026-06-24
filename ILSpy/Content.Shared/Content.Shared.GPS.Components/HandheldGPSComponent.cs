using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.GPS.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class HandheldGPSComponent : Component, ISerializationGenerated<HandheldGPSComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float UpdateRate = 1.5f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HandheldGPSComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HandheldGPSComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<HandheldGPSComponent>(this, ref target, hookCtx, false, context))
		{
			float UpdateRateTemp = 0f;
			if (!serialization.TryCustomCopy<float>(UpdateRate, ref UpdateRateTemp, hookCtx, false, context))
			{
				UpdateRateTemp = UpdateRate;
			}
			target.UpdateRate = UpdateRateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HandheldGPSComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HandheldGPSComponent cast = (HandheldGPSComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HandheldGPSComponent cast = (HandheldGPSComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HandheldGPSComponent def = (HandheldGPSComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HandheldGPSComponent Instantiate()
	{
		return new HandheldGPSComponent();
	}
}
