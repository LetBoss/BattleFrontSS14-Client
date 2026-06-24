using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
public sealed class VehicleGunnerBinocularsComponent : Component, ISerializationGenerated<VehicleGunnerBinocularsComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? Gunner;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleGunnerBinocularsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleGunnerBinocularsComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VehicleGunnerBinocularsComponent>(this, ref target, hookCtx, false, context))
		{
			EntityUid? GunnerTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(Gunner, ref GunnerTemp, hookCtx, false, context))
			{
				GunnerTemp = serialization.CreateCopy<EntityUid?>(Gunner, hookCtx, context, false);
			}
			target.Gunner = GunnerTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleGunnerBinocularsComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleGunnerBinocularsComponent cast = (VehicleGunnerBinocularsComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleGunnerBinocularsComponent cast = (VehicleGunnerBinocularsComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleGunnerBinocularsComponent def = (VehicleGunnerBinocularsComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleGunnerBinocularsComponent Instantiate()
	{
		return new VehicleGunnerBinocularsComponent();
	}
}
