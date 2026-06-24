using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.EntityTable.EntitySelectors;

public sealed class AllSelector : EntityTableSelector, ISerializationGenerated<AllSelector>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public List<EntityTableSelector> Children;

	protected override IEnumerable<EntProtoId> GetSpawnsImplementation(System.Random rand, IEntityManager entMan, IPrototypeManager proto, EntityTableContext ctx)
	{
		foreach (EntityTableSelector child in Children)
		{
			foreach (EntProtoId spawn in child.GetSpawns(rand, entMan, proto, ctx))
			{
				yield return spawn;
			}
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AllSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityTableSelector definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AllSelector)definitionCast;
		if (!serialization.TryCustomCopy<AllSelector>(this, ref target, hookCtx, false, context))
		{
			List<EntityTableSelector> ChildrenTemp = null;
			if (Children == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntityTableSelector>>(Children, ref ChildrenTemp, hookCtx, true, context))
			{
				ChildrenTemp = serialization.CreateCopy<List<EntityTableSelector>>(Children, hookCtx, context, false);
			}
			target.Children = ChildrenTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AllSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityTableSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AllSelector cast = (AllSelector)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AllSelector cast = (AllSelector)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AllSelector Instantiate()
	{
		return new AllSelector();
	}
}
