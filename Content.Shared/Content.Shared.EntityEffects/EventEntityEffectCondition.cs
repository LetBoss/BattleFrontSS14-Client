using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.EntityEffects;

public abstract class EventEntityEffectCondition<T> : EntityEffectCondition, ISerializationGenerated<EventEntityEffectCondition<T>>, ISerializationGenerated where T : EventEntityEffectCondition<T>
{
	public override bool Condition(EntityEffectBaseArgs args)
	{
		if (!(this is T type))
		{
			return false;
		}
		CheckEntityEffectConditionEvent<T> evt = new CheckEntityEffectConditionEvent<T>
		{
			Condition = type,
			Args = args
		};
		((IBroadcastEventBus)args.EntityManager.EventBus).RaiseEvent<CheckEntityEffectConditionEvent<T>>((EventSource)1, ref evt);
		return evt.Result;
	}

	public EventEntityEffectCondition()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref EventEntityEffectCondition<T> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffectCondition definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EventEntityEffectCondition<T>)definitionCast;
		serialization.TryCustomCopy<EventEntityEffectCondition<T>>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref EventEntityEffectCondition<T> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffectCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffectCondition<T> cast = (EventEntityEffectCondition<T>)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffectCondition<T> cast = (EventEntityEffectCondition<T>)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EventEntityEffectCondition<T> Instantiate()
	{
		throw new NotImplementedException();
	}
}
