using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction;

[ImplicitDataDefinitionForInheritors]
public interface IGraphAction : ISerializationGenerated<IGraphAction>, ISerializationGenerated
{
	void PerformAction(EntityUid uid, EntityUid? userUid, IEntityManager entityManager);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void InternalCopy(ref IGraphAction target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<IGraphAction>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref IGraphAction target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IGraphAction cast = (IGraphAction)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	IGraphAction Instantiate()
	{
		throw new NotImplementedException();
	}
}
