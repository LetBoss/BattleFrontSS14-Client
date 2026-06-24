using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Clothing.Components;

[Serializable]
[NetSerializable]
public sealed class ClothingUnequipDoAfterEvent : DoAfterEvent, ISerializationGenerated<ClothingUnequipDoAfterEvent>, ISerializationGenerated
{
	public string Slot;

	public ClothingUnequipDoAfterEvent(string slot)
	{
		Slot = slot;
	}

	public override DoAfterEvent Clone()
	{
		return this;
	}

	public ClothingUnequipDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ClothingUnequipDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ClothingUnequipDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<ClothingUnequipDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ClothingUnequipDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClothingUnequipDoAfterEvent cast = (ClothingUnequipDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClothingUnequipDoAfterEvent cast = (ClothingUnequipDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ClothingUnequipDoAfterEvent Instantiate()
	{
		return new ClothingUnequipDoAfterEvent();
	}
}
