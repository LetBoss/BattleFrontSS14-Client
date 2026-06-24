using System;
using System.Collections.Generic;
using Content.Shared.Examine;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Construction;

[ImplicitDataDefinitionForInheritors]
public interface IGraphCondition : ISerializationGenerated<IGraphCondition>, ISerializationGenerated
{
	bool Condition(EntityUid uid, IEntityManager entityManager);

	bool DoExamine(ExaminedEvent args);

	IEnumerable<ConstructionGuideEntry> GenerateGuideEntry();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void InternalCopy(ref IGraphCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		serialization.TryCustomCopy<IGraphCondition>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref IGraphCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IGraphCondition cast = (IGraphCondition)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	IGraphCondition Instantiate()
	{
		throw new NotImplementedException();
	}
}
