using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Vehicle.Supply;

[RegisterComponent]
public sealed class VehicleHardpointVendorComponent : Component, ISerializationGenerated<VehicleHardpointVendorComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float ConsoleSearchRange = 20f;

	[NonSerialized]
	public readonly Dictionary<string, int> LastVehicleCounts = new Dictionary<string, int>();

	[NonSerialized]
	public readonly Dictionary<string, int> RemainingGroupAmounts = new Dictionary<string, int>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleHardpointVendorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleHardpointVendorComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VehicleHardpointVendorComponent>(this, ref target, hookCtx, false, context))
		{
			float ConsoleSearchRangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ConsoleSearchRange, ref ConsoleSearchRangeTemp, hookCtx, false, context))
			{
				ConsoleSearchRangeTemp = ConsoleSearchRange;
			}
			target.ConsoleSearchRange = ConsoleSearchRangeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleHardpointVendorComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleHardpointVendorComponent cast = (VehicleHardpointVendorComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleHardpointVendorComponent cast = (VehicleHardpointVendorComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleHardpointVendorComponent def = (VehicleHardpointVendorComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleHardpointVendorComponent Instantiate()
	{
		return new VehicleHardpointVendorComponent();
	}
}
