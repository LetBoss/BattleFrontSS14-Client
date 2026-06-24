using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
public sealed class VehicleSpeedModifierAttachmentComponent : Component, ISerializationGenerated<VehicleSpeedModifierAttachmentComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float SpeedMultiplier = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleSpeedModifierAttachmentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleSpeedModifierAttachmentComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VehicleSpeedModifierAttachmentComponent>(this, ref target, hookCtx, false, context))
		{
			float SpeedMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpeedMultiplier, ref SpeedMultiplierTemp, hookCtx, false, context))
			{
				SpeedMultiplierTemp = SpeedMultiplier;
			}
			target.SpeedMultiplier = SpeedMultiplierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleSpeedModifierAttachmentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleSpeedModifierAttachmentComponent cast = (VehicleSpeedModifierAttachmentComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleSpeedModifierAttachmentComponent cast = (VehicleSpeedModifierAttachmentComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleSpeedModifierAttachmentComponent def = (VehicleSpeedModifierAttachmentComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleSpeedModifierAttachmentComponent Instantiate()
	{
		return new VehicleSpeedModifierAttachmentComponent();
	}
}
