using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Attachable.Events;

[Serializable]
[NetSerializable]
public sealed class AttachableAttachDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<AttachableAttachDoAfterEvent>, ISerializationGenerated
{
	public readonly string SlotId;

	public AttachableAttachDoAfterEvent(string slotId)
	{
		SlotId = slotId;
	}

	public AttachableAttachDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AttachableAttachDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AttachableAttachDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<AttachableAttachDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AttachableAttachDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AttachableAttachDoAfterEvent cast = (AttachableAttachDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AttachableAttachDoAfterEvent cast = (AttachableAttachDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AttachableAttachDoAfterEvent Instantiate()
	{
		return new AttachableAttachDoAfterEvent();
	}
}
