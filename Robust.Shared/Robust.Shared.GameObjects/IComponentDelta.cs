using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;

namespace Robust.Shared.GameObjects;

public interface IComponentDelta : IComponent, ISerializationGenerated<IComponent>, ISerializationGenerated, ISerializationGenerated<IComponentDelta>
{
	GameTick LastFieldUpdate { get; set; }

	GameTick[] LastModifiedFields { get; set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void InternalCopy(ref IComponentDelta target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void Copy(ref IComponentDelta target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IComponentDelta target2 = (IComponentDelta)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IComponentDelta target2 = (IComponentDelta)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	new IComponentDelta Instantiate()
	{
		throw new NotImplementedException();
	}

	IComponent IComponent.Instantiate()
	{
		return Instantiate();
	}

	IComponent ISerializationGenerated<IComponent>.Instantiate()
	{
		return Instantiate();
	}
}
