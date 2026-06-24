using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[Access(new Type[] { typeof(VehiclePortGunSystem) })]
public sealed class VehiclePortGunControllerComponent : Component, ISerializationGenerated<VehiclePortGunControllerComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string GunSlotId = "port-gun";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehiclePortGunControllerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehiclePortGunControllerComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VehiclePortGunControllerComponent>(this, ref target, hookCtx, false, context))
		{
			string GunSlotIdTemp = null;
			if (GunSlotId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(GunSlotId, ref GunSlotIdTemp, hookCtx, false, context))
			{
				GunSlotIdTemp = GunSlotId;
			}
			target.GunSlotId = GunSlotIdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehiclePortGunControllerComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehiclePortGunControllerComponent cast = (VehiclePortGunControllerComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehiclePortGunControllerComponent cast = (VehiclePortGunControllerComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehiclePortGunControllerComponent def = (VehiclePortGunControllerComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehiclePortGunControllerComponent Instantiate()
	{
		return new VehiclePortGunControllerComponent();
	}
}
