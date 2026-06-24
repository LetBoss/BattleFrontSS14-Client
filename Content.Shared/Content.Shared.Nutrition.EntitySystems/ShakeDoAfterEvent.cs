using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Nutrition.EntitySystems;

[Serializable]
[NetSerializable]
public sealed class ShakeDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<ShakeDoAfterEvent>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ShakeDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ShakeDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<ShakeDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ShakeDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ShakeDoAfterEvent cast = (ShakeDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ShakeDoAfterEvent cast = (ShakeDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ShakeDoAfterEvent Instantiate()
	{
		return new ShakeDoAfterEvent();
	}
}
