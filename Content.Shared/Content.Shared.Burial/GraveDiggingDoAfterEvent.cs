using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Burial;

[Serializable]
[NetSerializable]
public sealed class GraveDiggingDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<GraveDiggingDoAfterEvent>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GraveDiggingDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GraveDiggingDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<GraveDiggingDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref GraveDiggingDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GraveDiggingDoAfterEvent cast = (GraveDiggingDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GraveDiggingDoAfterEvent cast = (GraveDiggingDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GraveDiggingDoAfterEvent Instantiate()
	{
		return new GraveDiggingDoAfterEvent();
	}
}
