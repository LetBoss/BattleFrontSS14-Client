using System;
using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Revenant;

public sealed class RevenantOverloadLightsActionEvent : InstantActionEvent, ISerializationGenerated<RevenantOverloadLightsActionEvent>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RevenantOverloadLightsActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstantActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RevenantOverloadLightsActionEvent)definitionCast;
		serialization.TryCustomCopy<RevenantOverloadLightsActionEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RevenantOverloadLightsActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RevenantOverloadLightsActionEvent cast = (RevenantOverloadLightsActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RevenantOverloadLightsActionEvent cast = (RevenantOverloadLightsActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RevenantOverloadLightsActionEvent Instantiate()
	{
		return new RevenantOverloadLightsActionEvent();
	}
}
