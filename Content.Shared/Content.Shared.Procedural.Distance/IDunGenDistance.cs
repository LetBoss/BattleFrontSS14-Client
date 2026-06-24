using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Procedural.Distance;

[ImplicitDataDefinitionForInheritors]
public interface IDunGenDistance : ISerializationGenerated<IDunGenDistance>, ISerializationGenerated
{
	float BlendWeight { get; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void InternalCopy(ref IDunGenDistance target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<IDunGenDistance>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref IDunGenDistance target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IDunGenDistance cast = (IDunGenDistance)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	IDunGenDistance Instantiate()
	{
		throw new NotImplementedException();
	}
}
