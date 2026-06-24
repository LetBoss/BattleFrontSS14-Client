using System;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Dropship.Fabricator;

[Serializable]
[NetSerializable]
public sealed class DropshipFabricatoreRecycleDoafterEvent : SimpleDoAfterEvent, ISerializationGenerated<DropshipFabricatoreRecycleDoafterEvent>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref DropshipFabricatoreRecycleDoafterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SimpleDoAfterEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (DropshipFabricatoreRecycleDoafterEvent)definitionCast;
		serialization.TryCustomCopy<DropshipFabricatoreRecycleDoafterEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref DropshipFabricatoreRecycleDoafterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SimpleDoAfterEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DropshipFabricatoreRecycleDoafterEvent cast = (DropshipFabricatoreRecycleDoafterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DropshipFabricatoreRecycleDoafterEvent cast = (DropshipFabricatoreRecycleDoafterEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override DropshipFabricatoreRecycleDoafterEvent Instantiate()
	{
		return new DropshipFabricatoreRecycleDoafterEvent();
	}
}
