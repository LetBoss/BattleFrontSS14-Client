using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Movement.Components;

[ComponentProtoName("EyeCursorOffset")]
[NetworkedComponent]
public abstract class SharedEyeCursorOffsetComponent : Component, ISerializationGenerated<SharedEyeCursorOffsetComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float MaxOffset = 3f;

	[DataField(null, false, 1, false, false, null)]
	public float OffsetSpeed = 0.5f;

	[DataField(null, false, 1, false, false, null)]
	public float PvsIncrease = 0.3f;

	public SharedEyeCursorOffsetComponent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref SharedEyeCursorOffsetComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SharedEyeCursorOffsetComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SharedEyeCursorOffsetComponent>(this, ref target, hookCtx, false, context))
		{
			float MaxOffsetTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxOffset, ref MaxOffsetTemp, hookCtx, false, context))
			{
				MaxOffsetTemp = MaxOffset;
			}
			target.MaxOffset = MaxOffsetTemp;
			float OffsetSpeedTemp = 0f;
			if (!serialization.TryCustomCopy<float>(OffsetSpeed, ref OffsetSpeedTemp, hookCtx, false, context))
			{
				OffsetSpeedTemp = OffsetSpeed;
			}
			target.OffsetSpeed = OffsetSpeedTemp;
			float PvsIncreaseTemp = 0f;
			if (!serialization.TryCustomCopy<float>(PvsIncrease, ref PvsIncreaseTemp, hookCtx, false, context))
			{
				PvsIncreaseTemp = PvsIncrease;
			}
			target.PvsIncrease = PvsIncreaseTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref SharedEyeCursorOffsetComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedEyeCursorOffsetComponent cast = (SharedEyeCursorOffsetComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedEyeCursorOffsetComponent cast = (SharedEyeCursorOffsetComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedEyeCursorOffsetComponent def = (SharedEyeCursorOffsetComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SharedEyeCursorOffsetComponent Instantiate()
	{
		throw new NotImplementedException();
	}
}
