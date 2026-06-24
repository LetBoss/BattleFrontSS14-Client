using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.MagicMirror;

[Serializable]
[NetSerializable]
public sealed class MagicMirrorRemoveSlotDoAfterEvent : DoAfterEvent, ISerializationGenerated<MagicMirrorRemoveSlotDoAfterEvent>, ISerializationGenerated
{
	public MagicMirrorCategory Category;

	public int Slot;

	public override DoAfterEvent Clone()
	{
		return this;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref MagicMirrorRemoveSlotDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (MagicMirrorRemoveSlotDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<MagicMirrorRemoveSlotDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref MagicMirrorRemoveSlotDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagicMirrorRemoveSlotDoAfterEvent cast = (MagicMirrorRemoveSlotDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		MagicMirrorRemoveSlotDoAfterEvent cast = (MagicMirrorRemoveSlotDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override MagicMirrorRemoveSlotDoAfterEvent Instantiate()
	{
		return new MagicMirrorRemoveSlotDoAfterEvent();
	}
}
