using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle.Supply;

[RegisterComponent]
public sealed class VehicleSupplyConsoleComponent : Component, ISerializationGenerated<VehicleSupplyConsoleComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public List<VehicleSupplyEntry> Vehicles = new List<VehicleSupplyEntry>();

	[DataField(null, false, 1, false, false, null)]
	public float LiftSearchRange = 20f;

	[DataField(null, false, 1, false, false, null)]
	public string SelectedVehicle = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public int SelectedVehicleCopyIndex;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleSupplyConsoleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleSupplyConsoleComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VehicleSupplyConsoleComponent>(this, ref target, hookCtx, false, context))
		{
			List<VehicleSupplyEntry> VehiclesTemp = null;
			if (Vehicles == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<VehicleSupplyEntry>>(Vehicles, ref VehiclesTemp, hookCtx, true, context))
			{
				VehiclesTemp = serialization.CreateCopy<List<VehicleSupplyEntry>>(Vehicles, hookCtx, context, false);
			}
			target.Vehicles = VehiclesTemp;
			float LiftSearchRangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(LiftSearchRange, ref LiftSearchRangeTemp, hookCtx, false, context))
			{
				LiftSearchRangeTemp = LiftSearchRange;
			}
			target.LiftSearchRange = LiftSearchRangeTemp;
			string SelectedVehicleTemp = null;
			if (SelectedVehicle == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(SelectedVehicle, ref SelectedVehicleTemp, hookCtx, false, context))
			{
				SelectedVehicleTemp = SelectedVehicle;
			}
			target.SelectedVehicle = SelectedVehicleTemp;
			int SelectedVehicleCopyIndexTemp = 0;
			if (!serialization.TryCustomCopy<int>(SelectedVehicleCopyIndex, ref SelectedVehicleCopyIndexTemp, hookCtx, false, context))
			{
				SelectedVehicleCopyIndexTemp = SelectedVehicleCopyIndex;
			}
			target.SelectedVehicleCopyIndex = SelectedVehicleCopyIndexTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleSupplyConsoleComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleSupplyConsoleComponent cast = (VehicleSupplyConsoleComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleSupplyConsoleComponent cast = (VehicleSupplyConsoleComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleSupplyConsoleComponent def = (VehicleSupplyConsoleComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleSupplyConsoleComponent Instantiate()
	{
		return new VehicleSupplyConsoleComponent();
	}
}
