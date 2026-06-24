using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Pointing.Components;

[NetworkedComponent]
public abstract class SharedPointingArrowComponent : Component, ISerializationGenerated<SharedPointingArrowComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public Vector2 StartPosition;

	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public TimeSpan EndTime;

	public SharedPointingArrowComponent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref SharedPointingArrowComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SharedPointingArrowComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SharedPointingArrowComponent>(this, ref target, hookCtx, false, context))
		{
			Vector2 StartPositionTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(StartPosition, ref StartPositionTemp, hookCtx, false, context))
			{
				StartPositionTemp = serialization.CreateCopy<Vector2>(StartPosition, hookCtx, context, false);
			}
			target.StartPosition = StartPositionTemp;
			TimeSpan EndTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(EndTime, ref EndTimeTemp, hookCtx, false, context))
			{
				EndTimeTemp = serialization.CreateCopy<TimeSpan>(EndTime, hookCtx, context, false);
			}
			target.EndTime = EndTimeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref SharedPointingArrowComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedPointingArrowComponent cast = (SharedPointingArrowComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedPointingArrowComponent cast = (SharedPointingArrowComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedPointingArrowComponent def = (SharedPointingArrowComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SharedPointingArrowComponent Instantiate()
	{
		throw new NotImplementedException();
	}
}
