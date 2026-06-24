using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.MotionDetector;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(MotionDetectorSystem) })]
public sealed class MotionDetectorTrackedComponent : Component, ISerializationGenerated<MotionDetectorTrackedComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public TimeSpan LastMove;

	[DataField(null, false, 1, false, false, null)]
	public bool IsQueenEye;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MotionDetectorTrackedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MotionDetectorTrackedComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<MotionDetectorTrackedComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan LastMoveTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(LastMove, ref LastMoveTemp, hookCtx, false, context))
			{
				LastMoveTemp = serialization.CreateCopy<TimeSpan>(LastMove, hookCtx, context, false);
			}
			target.LastMove = LastMoveTemp;
			bool IsQueenEyeTemp = false;
			if (!serialization.TryCustomCopy<bool>(IsQueenEye, ref IsQueenEyeTemp, hookCtx, false, context))
			{
				IsQueenEyeTemp = IsQueenEye;
			}
			target.IsQueenEye = IsQueenEyeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MotionDetectorTrackedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MotionDetectorTrackedComponent cast = (MotionDetectorTrackedComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MotionDetectorTrackedComponent cast = (MotionDetectorTrackedComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MotionDetectorTrackedComponent def = (MotionDetectorTrackedComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MotionDetectorTrackedComponent Instantiate()
	{
		return new MotionDetectorTrackedComponent();
	}
}
