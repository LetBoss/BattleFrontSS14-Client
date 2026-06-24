using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
public sealed class VehicleHardpointVisualsComponent : Component, ISerializationGenerated<VehicleHardpointVisualsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public List<VehicleHardpointLayerState> Layers = new List<VehicleHardpointLayerState>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleHardpointVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleHardpointVisualsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VehicleHardpointVisualsComponent>(this, ref target, hookCtx, false, context))
		{
			List<VehicleHardpointLayerState> LayersTemp = null;
			if (Layers == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<VehicleHardpointLayerState>>(Layers, ref LayersTemp, hookCtx, true, context))
			{
				LayersTemp = serialization.CreateCopy<List<VehicleHardpointLayerState>>(Layers, hookCtx, context, false);
			}
			target.Layers = LayersTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleHardpointVisualsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleHardpointVisualsComponent cast = (VehicleHardpointVisualsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleHardpointVisualsComponent cast = (VehicleHardpointVisualsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleHardpointVisualsComponent def = (VehicleHardpointVisualsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleHardpointVisualsComponent Instantiate()
	{
		return new VehicleHardpointVisualsComponent();
	}
}
