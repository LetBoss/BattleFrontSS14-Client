using System;
using System.Collections.Generic;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._RMC14.Marines.Orders;

public interface IOrderComponent : IComponent, ISerializationGenerated<IComponent>, ISerializationGenerated, ISerializationGenerated<IOrderComponent>
{
	List<(FixedPoint2 Multiplier, TimeSpan ExpiresAt)> Received { get; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void InternalCopy(ref IOrderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<IOrderComponent>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref IOrderComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IOrderComponent cast = (IOrderComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IOrderComponent def = (IOrderComponent)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	IOrderComponent Instantiate()
	{
		throw new NotImplementedException();
	}

	IComponent IComponent.Instantiate()
	{
		return (IComponent)Instantiate();
	}

	IComponent ISerializationGenerated<IComponent>.Instantiate()
	{
		return (IComponent)Instantiate();
	}
}
