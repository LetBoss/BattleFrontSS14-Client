using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.MagicMirror;

[Serializable]
[NetSerializable]
public sealed class MagicMirrorAddSlotDoAfterEvent : DoAfterEvent, ISerializationGenerated<MagicMirrorAddSlotDoAfterEvent>, ISerializationGenerated
{
	public MagicMirrorCategory Category;

	public override DoAfterEvent Clone()
	{
		return this;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MagicMirrorAddSlotDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MagicMirrorAddSlotDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<MagicMirrorAddSlotDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MagicMirrorAddSlotDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagicMirrorAddSlotDoAfterEvent cast = (MagicMirrorAddSlotDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagicMirrorAddSlotDoAfterEvent cast = (MagicMirrorAddSlotDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MagicMirrorAddSlotDoAfterEvent Instantiate()
	{
		return new MagicMirrorAddSlotDoAfterEvent();
	}
}
