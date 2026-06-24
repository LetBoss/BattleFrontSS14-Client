using System;
using System.Collections.Generic;
using Content.Shared.EntityTable.ValueSelector;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.EntityTable.EntitySelectors;

public sealed class EntSelector : EntityTableSelector, ISerializationGenerated<EntSelector>, ISerializationGenerated
{
	public const string IdDataFieldTag = "id";

	[DataField("id", false, 1, true, false, null)]
	public EntProtoId Id;

	[DataField(null, false, 1, false, false, null)]
	public NumberSelector Amount = new ConstantNumberSelector(1);

	protected override IEnumerable<EntProtoId> GetSpawnsImplementation(System.Random rand, IEntityManager entMan, IPrototypeManager proto, EntityTableContext ctx)
	{
		int num = Amount.Get(rand);
		for (int i = 0; i < num; i++)
		{
			yield return Id;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref EntSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		EntityTableSelector definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (EntSelector)definitionCast;
		if (!serialization.TryCustomCopy<EntSelector>(this, ref target, hookCtx, false, context))
		{
			EntProtoId IdTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Id, ref IdTemp, hookCtx, false, context))
			{
				IdTemp = serialization.CreateCopy<EntProtoId>(Id, hookCtx, context, false);
			}
			target.Id = IdTemp;
			NumberSelector AmountTemp = null;
			if (Amount == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<NumberSelector>(Amount, ref AmountTemp, hookCtx, true, context))
			{
				AmountTemp = serialization.CreateCopy<NumberSelector>(Amount, hookCtx, context, false);
			}
			target.Amount = AmountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref EntSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityTableSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntSelector cast = (EntSelector)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntSelector cast = (EntSelector)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override EntSelector Instantiate()
	{
		return new EntSelector();
	}
}
