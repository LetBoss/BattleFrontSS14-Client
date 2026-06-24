using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.Loot;

[ImplicitDataDefinitionForInheritors]
public interface IDungeonLoot : ISerializationGenerated<IDungeonLoot>, ISerializationGenerated
{
	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void InternalCopy(ref IDungeonLoot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<IDungeonLoot>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref IDungeonLoot target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IDungeonLoot cast = (IDungeonLoot)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	IDungeonLoot Instantiate()
	{
		throw new NotImplementedException();
	}
}
