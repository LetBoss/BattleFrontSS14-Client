using System;
using Content.Shared._RMC14.Xenonids.Hive;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.ManageHive.Boons;

[Serializable]
[DataDefinition]
[ImplicitDataDefinitionForInheritors]
[NetSerializable]
[Virtual]
public class HiveBoonEvent : ISerializationGenerated<HiveBoonEvent>, ISerializationGenerated
{
	[NonSerialized]
	public EntityUid Boon;

	[NonSerialized]
	public Entity<HiveComponent> Hive;

	[NonSerialized]
	public EntityUid? Core;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref HiveBoonEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<HiveBoonEvent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref HiveBoonEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		HiveBoonEvent cast = (HiveBoonEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual HiveBoonEvent Instantiate()
	{
		return new HiveBoonEvent();
	}
}
