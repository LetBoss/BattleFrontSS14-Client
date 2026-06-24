using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
public sealed class VehicleAccelerationModifierAttachmentComponent : Component, ISerializationGenerated<VehicleAccelerationModifierAttachmentComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float AccelerationMultiplier = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleAccelerationModifierAttachmentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleAccelerationModifierAttachmentComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VehicleAccelerationModifierAttachmentComponent>(this, ref target, hookCtx, false, context))
		{
			float AccelerationMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(AccelerationMultiplier, ref AccelerationMultiplierTemp, hookCtx, false, context))
			{
				AccelerationMultiplierTemp = AccelerationMultiplier;
			}
			target.AccelerationMultiplier = AccelerationMultiplierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleAccelerationModifierAttachmentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleAccelerationModifierAttachmentComponent cast = (VehicleAccelerationModifierAttachmentComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleAccelerationModifierAttachmentComponent cast = (VehicleAccelerationModifierAttachmentComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleAccelerationModifierAttachmentComponent def = (VehicleAccelerationModifierAttachmentComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleAccelerationModifierAttachmentComponent Instantiate()
	{
		return new VehicleAccelerationModifierAttachmentComponent();
	}
}
