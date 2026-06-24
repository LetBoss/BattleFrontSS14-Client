using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Xenonids.ManageHive.Boons;

[Serializable]
[NetSerializable]
public sealed class HiveBoonActivateFireResistanceEvent : HiveBoonEvent, ISerializationGenerated<HiveBoonActivateFireResistanceEvent>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref HiveBoonActivateFireResistanceEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HiveBoonEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (HiveBoonActivateFireResistanceEvent)definitionCast;
		serialization.TryCustomCopy<HiveBoonActivateFireResistanceEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref HiveBoonActivateFireResistanceEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref HiveBoonEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HiveBoonActivateFireResistanceEvent cast = (HiveBoonActivateFireResistanceEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HiveBoonActivateFireResistanceEvent cast = (HiveBoonActivateFireResistanceEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override HiveBoonActivateFireResistanceEvent Instantiate()
	{
		return new HiveBoonActivateFireResistanceEvent();
	}
}
