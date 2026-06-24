using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Internals;

[Serializable]
[NetSerializable]
public sealed class InternalsDoAfterEvent : DoAfterEvent, ISerializationGenerated<InternalsDoAfterEvent>, ISerializationGenerated
{
	public ToggleMode ToggleMode;

	public InternalsDoAfterEvent(ToggleMode mode)
	{
		ToggleMode = mode;
	}

	public override DoAfterEvent Clone()
	{
		return this;
	}

	public InternalsDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref InternalsDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (InternalsDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<InternalsDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref InternalsDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalsDoAfterEvent cast = (InternalsDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalsDoAfterEvent cast = (InternalsDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override InternalsDoAfterEvent Instantiate()
	{
		return new InternalsDoAfterEvent();
	}
}
