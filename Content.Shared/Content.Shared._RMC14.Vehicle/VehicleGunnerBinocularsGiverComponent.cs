using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
public sealed class VehicleGunnerBinocularsGiverComponent : Component, ISerializationGenerated<VehicleGunnerBinocularsGiverComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId BinocularsPrototype = EntProtoId.op_Implicit("RMCBinocularsVehicle");

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleGunnerBinocularsGiverComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleGunnerBinocularsGiverComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VehicleGunnerBinocularsGiverComponent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId BinocularsPrototypeTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(BinocularsPrototype, ref BinocularsPrototypeTemp, hookCtx, false, context))
			{
				BinocularsPrototypeTemp = serialization.CreateCopy<EntProtoId>(BinocularsPrototype, hookCtx, context, false);
			}
			target.BinocularsPrototype = BinocularsPrototypeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleGunnerBinocularsGiverComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleGunnerBinocularsGiverComponent cast = (VehicleGunnerBinocularsGiverComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleGunnerBinocularsGiverComponent cast = (VehicleGunnerBinocularsGiverComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleGunnerBinocularsGiverComponent def = (VehicleGunnerBinocularsGiverComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleGunnerBinocularsGiverComponent Instantiate()
	{
		return new VehicleGunnerBinocularsGiverComponent();
	}
}
