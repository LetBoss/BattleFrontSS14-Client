using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
public sealed class VehicleGunnerViewAttachmentComponent : Component, ISerializationGenerated<VehicleGunnerViewAttachmentComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float PvsScale = 0.35f;

	[DataField(null, false, 1, false, false, null)]
	public float CursorMaxOffset;

	[DataField(null, false, 1, false, false, null)]
	public float CursorOffsetSpeed = 0.5f;

	[DataField(null, false, 1, false, false, null)]
	public float CursorPvsIncrease;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleGunnerViewAttachmentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleGunnerViewAttachmentComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<VehicleGunnerViewAttachmentComponent>(this, ref target, hookCtx, false, context))
		{
			float PvsScaleTemp = 0f;
			if (!serialization.TryCustomCopy<float>(PvsScale, ref PvsScaleTemp, hookCtx, false, context))
			{
				PvsScaleTemp = PvsScale;
			}
			target.PvsScale = PvsScaleTemp;
			float CursorMaxOffsetTemp = 0f;
			if (!serialization.TryCustomCopy<float>(CursorMaxOffset, ref CursorMaxOffsetTemp, hookCtx, false, context))
			{
				CursorMaxOffsetTemp = CursorMaxOffset;
			}
			target.CursorMaxOffset = CursorMaxOffsetTemp;
			float CursorOffsetSpeedTemp = 0f;
			if (!serialization.TryCustomCopy<float>(CursorOffsetSpeed, ref CursorOffsetSpeedTemp, hookCtx, false, context))
			{
				CursorOffsetSpeedTemp = CursorOffsetSpeed;
			}
			target.CursorOffsetSpeed = CursorOffsetSpeedTemp;
			float CursorPvsIncreaseTemp = 0f;
			if (!serialization.TryCustomCopy<float>(CursorPvsIncrease, ref CursorPvsIncreaseTemp, hookCtx, false, context))
			{
				CursorPvsIncreaseTemp = CursorPvsIncrease;
			}
			target.CursorPvsIncrease = CursorPvsIncreaseTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleGunnerViewAttachmentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleGunnerViewAttachmentComponent cast = (VehicleGunnerViewAttachmentComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleGunnerViewAttachmentComponent cast = (VehicleGunnerViewAttachmentComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleGunnerViewAttachmentComponent def = (VehicleGunnerViewAttachmentComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleGunnerViewAttachmentComponent Instantiate()
	{
		return new VehicleGunnerViewAttachmentComponent();
	}
}
