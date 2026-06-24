using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.EntityEffects;

public abstract class EventEntityEffect<T> : EntityEffect, ISerializationGenerated<EventEntityEffect<T>>, ISerializationGenerated where T : EntityEffect
{
	public override void Effect(EntityEffectBaseArgs args)
	{
		if (this is T type)
		{
			ExecuteEntityEffectEvent<T> ev = new ExecuteEntityEffectEvent<T>(type, args);
			((IBroadcastEventBus)args.EntityManager.EventBus).RaiseEvent<ExecuteEntityEffectEvent<T>>((EventSource)1, ref ev);
		}
	}

	public EventEntityEffect()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref EventEntityEffect<T> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EventEntityEffect<T>)definitionCast;
		serialization.TryCustomCopy<EventEntityEffect<T>>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref EventEntityEffect<T> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<T> cast = (EventEntityEffect<T>)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<T> cast = (EventEntityEffect<T>)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EventEntityEffect<T> Instantiate()
	{
		throw new NotImplementedException();
	}
}
