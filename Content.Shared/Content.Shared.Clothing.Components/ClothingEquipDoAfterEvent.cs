using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Clothing.Components;

[Serializable]
[NetSerializable]
public sealed class ClothingEquipDoAfterEvent : DoAfterEvent, ISerializationGenerated<ClothingEquipDoAfterEvent>, ISerializationGenerated
{
	public string Slot;

	public ClothingEquipDoAfterEvent(string slot)
	{
		Slot = slot;
	}

	public override DoAfterEvent Clone()
	{
		return this;
	}

	public ClothingEquipDoAfterEvent()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ClothingEquipDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ClothingEquipDoAfterEvent)definitionCast;
		serialization.TryCustomCopy<ClothingEquipDoAfterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ClothingEquipDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref DoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClothingEquipDoAfterEvent cast = (ClothingEquipDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClothingEquipDoAfterEvent cast = (ClothingEquipDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ClothingEquipDoAfterEvent Instantiate()
	{
		return new ClothingEquipDoAfterEvent();
	}
}
