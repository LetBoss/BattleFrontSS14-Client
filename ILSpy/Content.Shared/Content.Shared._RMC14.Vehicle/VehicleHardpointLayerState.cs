using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
[DataDefinition]
public record struct VehicleHardpointLayerState : ISerializationGenerated<VehicleHardpointLayerState>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public string Layer { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public string State { get; set; }

	public VehicleHardpointLayerState(string layer, string state)
	{
		Layer = string.Empty;
		State = string.Empty;
		Layer = layer;
		State = state;
	}

	public VehicleHardpointLayerState()
	{
		Layer = string.Empty;
		State = string.Empty;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleHardpointLayerState target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<VehicleHardpointLayerState>(this, ref target, hookCtx, false, context))
		{
			string LayerTemp = null;
			if (Layer == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Layer, ref LayerTemp, hookCtx, false, context))
			{
				LayerTemp = Layer;
			}
			string StateTemp = null;
			if (State == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(State, ref StateTemp, hookCtx, false, context))
			{
				StateTemp = State;
			}
			target = target with
			{
				Layer = LayerTemp,
				State = StateTemp
			};
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleHardpointLayerState target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleHardpointLayerState cast = (VehicleHardpointLayerState)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public VehicleHardpointLayerState Instantiate()
	{
		return new VehicleHardpointLayerState();
	}
}
