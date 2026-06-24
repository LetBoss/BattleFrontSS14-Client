using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
public sealed class VehicleWeaponSupportAttachmentComponent : Component, ISerializationGenerated<VehicleWeaponSupportAttachmentComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 AccuracyMultiplier = 1;

	[DataField(null, false, 1, false, false, null)]
	public float FireRateMultiplier = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleWeaponSupportAttachmentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleWeaponSupportAttachmentComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VehicleWeaponSupportAttachmentComponent>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 AccuracyMultiplierTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(AccuracyMultiplier, ref AccuracyMultiplierTemp, hookCtx, false, context))
			{
				AccuracyMultiplierTemp = serialization.CreateCopy<FixedPoint2>(AccuracyMultiplier, hookCtx, context, false);
			}
			target.AccuracyMultiplier = AccuracyMultiplierTemp;
			float FireRateMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(FireRateMultiplier, ref FireRateMultiplierTemp, hookCtx, false, context))
			{
				FireRateMultiplierTemp = FireRateMultiplier;
			}
			target.FireRateMultiplier = FireRateMultiplierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleWeaponSupportAttachmentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleWeaponSupportAttachmentComponent cast = (VehicleWeaponSupportAttachmentComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleWeaponSupportAttachmentComponent cast = (VehicleWeaponSupportAttachmentComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleWeaponSupportAttachmentComponent def = (VehicleWeaponSupportAttachmentComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleWeaponSupportAttachmentComponent Instantiate()
	{
		return new VehicleWeaponSupportAttachmentComponent();
	}
}
