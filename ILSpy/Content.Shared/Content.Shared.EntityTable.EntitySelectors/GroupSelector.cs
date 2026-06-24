using System;
using System.Collections.Generic;
using Content.Shared.Random.Helpers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.EntityTable.EntitySelectors;

public sealed class GroupSelector : EntityTableSelector, ISerializationGenerated<GroupSelector>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public List<EntityTableSelector> Children = new List<EntityTableSelector>();

	protected override IEnumerable<EntProtoId> GetSpawnsImplementation(System.Random rand, IEntityManager entMan, IPrototypeManager proto, EntityTableContext ctx)
	{
		Dictionary<EntityTableSelector, float> children = new Dictionary<EntityTableSelector, float>(Children.Count);
		foreach (EntityTableSelector child in Children)
		{
			if (child.CheckConditions(entMan, proto, ctx))
			{
				children.Add(child, child.Weight);
			}
		}
		return SharedRandomExtensions.Pick(children, rand).GetSpawns(rand, entMan, proto, ctx);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref GroupSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		EntityTableSelector definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (GroupSelector)definitionCast;
		if (!serialization.TryCustomCopy<GroupSelector>(this, ref target, hookCtx, false, context))
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
	public void Copy(ref GroupSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityTableSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GroupSelector cast = (GroupSelector)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		GroupSelector cast = (GroupSelector)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override GroupSelector Instantiate()
	{
		return new GroupSelector();
	}
}
