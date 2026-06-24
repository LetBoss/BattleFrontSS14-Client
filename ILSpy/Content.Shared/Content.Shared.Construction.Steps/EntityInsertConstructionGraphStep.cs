using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Construction.Steps;

[ImplicitDataDefinitionForInheritors]
public abstract class EntityInsertConstructionGraphStep : ConstructionGraphStep, ISerializationGenerated<EntityInsertConstructionGraphStep>, ISerializationGenerated
{
	[DataField("store", false, 1, false, false, null)]
	public string Store { get; private set; } = string.Empty;

	public abstract bool EntityValid(EntityUid uid, IEntityManager entityManager, IComponentFactory compFactory);

	public EntityInsertConstructionGraphStep()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref EntityInsertConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		ConstructionGraphStep definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EntityInsertConstructionGraphStep)definitionCast;
		if (!serialization.TryCustomCopy<EntityInsertConstructionGraphStep>(this, ref target, hookCtx, false, context))
		{
			string StoreTemp = null;
			if (Store == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Store, ref StoreTemp, hookCtx, false, context))
			{
				StoreTemp = Store;
			}
			target.Store = StoreTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref EntityInsertConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref ConstructionGraphStep target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityInsertConstructionGraphStep cast = (EntityInsertConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityInsertConstructionGraphStep cast = (EntityInsertConstructionGraphStep)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EntityInsertConstructionGraphStep Instantiate()
	{
		throw new NotImplementedException();
	}
}
