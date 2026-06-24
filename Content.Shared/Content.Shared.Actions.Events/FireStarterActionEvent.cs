using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Actions.Events;

public sealed class FireStarterActionEvent : InstantActionEvent, ISerializationGenerated<FireStarterActionEvent>, ISerializationGenerated
{
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float Severity = 0.3f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref FireStarterActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstantActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (FireStarterActionEvent)definitionCast;
		serialization.TryCustomCopy<FireStarterActionEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref FireStarterActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FireStarterActionEvent cast = (FireStarterActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		FireStarterActionEvent cast = (FireStarterActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override FireStarterActionEvent Instantiate()
	{
		return new FireStarterActionEvent();
	}
}
