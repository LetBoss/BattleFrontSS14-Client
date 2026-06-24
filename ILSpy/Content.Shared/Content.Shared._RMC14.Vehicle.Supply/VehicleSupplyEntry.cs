using System;
using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle.Supply;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class VehicleSupplyEntry : ISerializationGenerated<VehicleSupplyEntry>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string? Name;

	[DataField(null, false, 1, true, false, null)]
	public EntProtoId Vehicle;

	[DataField(null, false, 1, false, false, null)]
	public string? Unlock;

	[DataField(null, false, 1, false, false, null)]
	public List<EntProtoId> Hardpoints = new List<EntProtoId>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleSupplyEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<VehicleSupplyEntry>(this, ref target, hookCtx, false, context))
		{
			string NameTemp = null;
			if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = Name;
			}
			target.Name = NameTemp;
			EntProtoId VehicleTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Vehicle, ref VehicleTemp, hookCtx, false, context))
			{
				VehicleTemp = serialization.CreateCopy<EntProtoId>(Vehicle, hookCtx, context, false);
			}
			target.Vehicle = VehicleTemp;
			string UnlockTemp = null;
			if (!serialization.TryCustomCopy<string>(Unlock, ref UnlockTemp, hookCtx, false, context))
			{
				UnlockTemp = Unlock;
			}
			target.Unlock = UnlockTemp;
			List<EntProtoId> HardpointsTemp = null;
			if (Hardpoints == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntProtoId>>(Hardpoints, ref HardpointsTemp, hookCtx, true, context))
			{
				HardpointsTemp = serialization.CreateCopy<List<EntProtoId>>(Hardpoints, hookCtx, context, false);
			}
			target.Hardpoints = HardpointsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleSupplyEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleSupplyEntry cast = (VehicleSupplyEntry)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public VehicleSupplyEntry Instantiate()
	{
		return new VehicleSupplyEntry();
	}
}
