using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.DoAfter;

[Serializable]
[NetSerializable]
[ImplicitDataDefinitionForInheritors]
public abstract class DoAfterEvent : HandledEntityEventArgs, ISerializationGenerated<DoAfterEvent>, ISerializationGenerated
{
	[NonSerialized]
	public DoAfter DoAfter;

	public bool Repeat;

	public bool Cancelled => DoAfter.Cancelled;

	public EntityUid User => DoAfter.Args.User;

	public EntityUid? Target => DoAfter.Args.Target;

	public EntityUid? Used => DoAfter.Args.Used;

	public DoAfterArgs Args => DoAfter.Args;

	public abstract DoAfterEvent Clone();

	public virtual bool IsDuplicate(DoAfterEvent other)
	{
		return ((object)this).GetType() == ((object)other).GetType();
	}

	public DoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<DoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterEvent cast = (DoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual DoAfterEvent Instantiate()
	{
		throw new NotImplementedException();
	}
}
