using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Attachable.Events;

[Serializable]
[NetSerializable]
public sealed class AttachableToggleDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<AttachableToggleDoAfterEvent>, ISerializationGenerated
{
	public readonly string SlotId;

	public readonly string PopupText;

	public AttachableToggleDoAfterEvent(string slotId, string popupText)
	{
		SlotId = slotId;
		PopupText = popupText;
	}

	public AttachableToggleDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AttachableToggleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AttachableToggleDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<AttachableToggleDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AttachableToggleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AttachableToggleDoAfterEvent cast = (AttachableToggleDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AttachableToggleDoAfterEvent cast = (AttachableToggleDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AttachableToggleDoAfterEvent Instantiate()
	{
		return new AttachableToggleDoAfterEvent();
	}
}
