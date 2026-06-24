using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[]
{
	typeof(HardpointSystem),
	typeof(HardpointSlotSystem)
})]
public sealed class HardpointItemComponent : Component, ISerializationGenerated<HardpointItemComponent>, ISerializationGenerated
{
	public const string ComponentId = "HardpointItem";

	[DataField(null, false, 1, true, false, null)]
	public string HardpointType = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<HardpointVehicleFamilyPrototype>? VehicleFamily;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<HardpointSlotTypePrototype>? SlotType;

	[DataField(null, false, 1, false, false, null)]
	public string? CompatibilityId;

	[DataField(null, false, 1, false, false, null)]
	public float DamageMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float RepairRate = 0.01f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HardpointItemComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HardpointItemComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<HardpointItemComponent>(this, ref target, hookCtx, false, context))
		{
			string HardpointTypeTemp = null;
			if (HardpointType == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(HardpointType, ref HardpointTypeTemp, hookCtx, false, context))
			{
				HardpointTypeTemp = HardpointType;
			}
			target.HardpointType = HardpointTypeTemp;
			ProtoId<HardpointVehicleFamilyPrototype>? VehicleFamilyTemp = null;
			if (!serialization.TryCustomCopy<ProtoId<HardpointVehicleFamilyPrototype>?>(VehicleFamily, ref VehicleFamilyTemp, hookCtx, false, context))
			{
				VehicleFamilyTemp = serialization.CreateCopy<ProtoId<HardpointVehicleFamilyPrototype>?>(VehicleFamily, hookCtx, context, false);
			}
			target.VehicleFamily = VehicleFamilyTemp;
			ProtoId<HardpointSlotTypePrototype>? SlotTypeTemp = null;
			if (!serialization.TryCustomCopy<ProtoId<HardpointSlotTypePrototype>?>(SlotType, ref SlotTypeTemp, hookCtx, false, context))
			{
				SlotTypeTemp = serialization.CreateCopy<ProtoId<HardpointSlotTypePrototype>?>(SlotType, hookCtx, context, false);
			}
			target.SlotType = SlotTypeTemp;
			string CompatibilityIdTemp = null;
			if (!serialization.TryCustomCopy<string>(CompatibilityId, ref CompatibilityIdTemp, hookCtx, false, context))
			{
				CompatibilityIdTemp = CompatibilityId;
			}
			target.CompatibilityId = CompatibilityIdTemp;
			float DamageMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DamageMultiplier, ref DamageMultiplierTemp, hookCtx, false, context))
			{
				DamageMultiplierTemp = DamageMultiplier;
			}
			target.DamageMultiplier = DamageMultiplierTemp;
			float RepairRateTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RepairRate, ref RepairRateTemp, hookCtx, false, context))
			{
				RepairRateTemp = RepairRate;
			}
			target.RepairRate = RepairRateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HardpointItemComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HardpointItemComponent cast = (HardpointItemComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HardpointItemComponent cast = (HardpointItemComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HardpointItemComponent def = (HardpointItemComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HardpointItemComponent Instantiate()
	{
		return new HardpointItemComponent();
	}
}
