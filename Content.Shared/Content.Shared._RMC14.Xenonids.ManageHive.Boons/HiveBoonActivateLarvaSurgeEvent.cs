using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Xenonids.ManageHive.Boons;

[Serializable]
[NetSerializable]
public sealed class HiveBoonActivateLarvaSurgeEvent : HiveBoonEvent, ISerializationGenerated<HiveBoonActivateLarvaSurgeEvent>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HiveBoonActivateLarvaSurgeEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HiveBoonEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HiveBoonActivateLarvaSurgeEvent)definitionCast;
		serialization.TryCustomCopy<HiveBoonActivateLarvaSurgeEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HiveBoonActivateLarvaSurgeEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref HiveBoonEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HiveBoonActivateLarvaSurgeEvent cast = (HiveBoonActivateLarvaSurgeEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HiveBoonActivateLarvaSurgeEvent cast = (HiveBoonActivateLarvaSurgeEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HiveBoonActivateLarvaSurgeEvent Instantiate()
	{
		return new HiveBoonActivateLarvaSurgeEvent();
	}
}
