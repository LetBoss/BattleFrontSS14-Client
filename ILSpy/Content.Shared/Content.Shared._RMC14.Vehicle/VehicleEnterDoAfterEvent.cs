using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Vehicle;

[Serializable]
[NetSerializable]
public sealed class VehicleEnterDoAfterEvent : SimpleDoAfterEvent, ISerializationGenerated<VehicleEnterDoAfterEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public int EntryIndex;

	public override DoAfterEvent Clone()
	{
		return new VehicleEnterDoAfterEvent
		{
			EntryIndex = EntryIndex
		};
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref VehicleEnterDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (VehicleEnterDoAfterEvent)definitionCast;
		if (!serialization.TryCustomCopy<VehicleEnterDoAfterEvent>(this, ref target, hookCtx, false, context))
		{
			int EntryIndexTemp = 0;
			if (!serialization.TryCustomCopy<int>(EntryIndex, ref EntryIndexTemp, hookCtx, false, context))
			{
				EntryIndexTemp = EntryIndex;
			}
			target.EntryIndex = EntryIndexTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref VehicleEnterDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleEnterDoAfterEvent cast = (VehicleEnterDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		VehicleEnterDoAfterEvent cast = (VehicleEnterDoAfterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override VehicleEnterDoAfterEvent Instantiate()
	{
		return new VehicleEnterDoAfterEvent();
	}
}
