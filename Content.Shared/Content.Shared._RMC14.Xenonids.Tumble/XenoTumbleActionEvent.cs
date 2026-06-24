using System;
using Content.Shared.Actions;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Xenonids.Tumble;

public sealed class XenoTumbleActionEvent : WorldTargetActionEvent, ISerializationGenerated<XenoTumbleActionEvent>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoTumbleActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WorldTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoTumbleActionEvent)definitionCast;
		serialization.TryCustomCopy<XenoTumbleActionEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoTumbleActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref WorldTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoTumbleActionEvent cast = (XenoTumbleActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoTumbleActionEvent cast = (XenoTumbleActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoTumbleActionEvent Instantiate()
	{
		return new XenoTumbleActionEvent();
	}
}
