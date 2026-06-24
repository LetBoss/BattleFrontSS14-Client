using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Robust.Shared.GameObjects;

public interface IComponentDebug : IComponent, ISerializationGenerated<IComponent>, ISerializationGenerated, ISerializationGenerated<IComponentDebug>
{
	string GetDebugString();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void InternalCopy(ref IComponentDebug target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void Copy(ref IComponentDebug target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IComponentDebug target2 = (IComponentDebug)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IComponentDebug target2 = (IComponentDebug)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	new void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	new IComponentDebug Instantiate()
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
