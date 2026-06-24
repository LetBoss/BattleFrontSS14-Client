using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Light.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ItemTogglePointLightComponent : Component, ISerializationGenerated<ItemTogglePointLightComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool ToggleableVisualsColorModulatesLights;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ItemTogglePointLightComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ItemTogglePointLightComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ItemTogglePointLightComponent>(this, ref target, hookCtx, false, context))
		{
			bool ToggleableVisualsColorModulatesLightsTemp = false;
			if (!serialization.TryCustomCopy<bool>(ToggleableVisualsColorModulatesLights, ref ToggleableVisualsColorModulatesLightsTemp, hookCtx, false, context))
			{
				ToggleableVisualsColorModulatesLightsTemp = ToggleableVisualsColorModulatesLights;
			}
			target.ToggleableVisualsColorModulatesLights = ToggleableVisualsColorModulatesLightsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ItemTogglePointLightComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemTogglePointLightComponent cast = (ItemTogglePointLightComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemTogglePointLightComponent cast = (ItemTogglePointLightComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ItemTogglePointLightComponent def = (ItemTogglePointLightComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ItemTogglePointLightComponent Instantiate()
	{
		return new ItemTogglePointLightComponent();
	}
}
