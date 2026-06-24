using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Strip.Components;

[Serializable]
[NetSerializable]
public sealed class StrippableDoAfterEvent : DoAfterEvent, ISerializationGenerated<StrippableDoAfterEvent>, ISerializationGenerated
{
	public readonly bool InsertOrRemove;

	public readonly bool InventoryOrHand;

	public readonly string SlotOrHandName;

	public StrippableDoAfterEvent(bool insertOrRemove, bool inventoryOrHand, string slotOrHandName)
	{
		InsertOrRemove = insertOrRemove;
		InventoryOrHand = inventoryOrHand;
		SlotOrHandName = slotOrHandName;
	}

	public override DoAfterEvent Clone()
	{
		return this;
	}

	public StrippableDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StrippableDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StrippableDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<StrippableDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StrippableDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StrippableDoAfterEvent cast = (StrippableDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StrippableDoAfterEvent cast = (StrippableDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StrippableDoAfterEvent Instantiate()
	{
		return new StrippableDoAfterEvent();
	}
}
