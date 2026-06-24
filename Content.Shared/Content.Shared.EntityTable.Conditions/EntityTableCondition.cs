using System;
using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityTable.Conditions;

[ImplicitDataDefinitionForInheritors]
public abstract class EntityTableCondition : ISerializationGenerated<EntityTableCondition>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Invert;

	public bool Evaluate(EntityTableSelector root, IEntityManager entMan, IPrototypeManager proto, EntityTableContext ctx)
	{
		return EvaluateImplementation(root, entMan, proto, ctx) ^ Invert;
	}

	protected abstract bool EvaluateImplementation(EntityTableSelector root, IEntityManager entMan, IPrototypeManager proto, EntityTableContext ctx);

	public EntityTableCondition()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref EntityTableCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<EntityTableCondition>(this, ref target, hookCtx, false, context))
		{
			bool InvertTemp = false;
			if (!serialization.TryCustomCopy<bool>(Invert, ref InvertTemp, hookCtx, false, context))
			{
				InvertTemp = Invert;
			}
			target.Invert = InvertTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref EntityTableCondition target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityTableCondition cast = (EntityTableCondition)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual EntityTableCondition Instantiate()
	{
		throw new NotImplementedException();
	}
}
