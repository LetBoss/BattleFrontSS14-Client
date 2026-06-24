using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityTable.EntitySelectors;

public sealed class NestedSelector : EntityTableSelector, ISerializationGenerated<NestedSelector>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<EntityTablePrototype> TableId;

	protected override IEnumerable<EntProtoId> GetSpawnsImplementation(System.Random rand, IEntityManager entMan, IPrototypeManager proto, EntityTableContext ctx)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return proto.Index<EntityTablePrototype>(TableId).Table.GetSpawns(rand, entMan, proto, ctx);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref NestedSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		EntityTableSelector definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (NestedSelector)definitionCast;
		if (!serialization.TryCustomCopy<NestedSelector>(this, ref target, hookCtx, false, context))
		{
			ProtoId<EntityTablePrototype> TableIdTemp = default(ProtoId<EntityTablePrototype>);
			if (!serialization.TryCustomCopy<ProtoId<EntityTablePrototype>>(TableId, ref TableIdTemp, hookCtx, false, context))
			{
				TableIdTemp = serialization.CreateCopy<ProtoId<EntityTablePrototype>>(TableId, hookCtx, context, false);
			}
			target.TableId = TableIdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref NestedSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityTableSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NestedSelector cast = (NestedSelector)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		NestedSelector cast = (NestedSelector)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override NestedSelector Instantiate()
	{
		return new NestedSelector();
	}
}
