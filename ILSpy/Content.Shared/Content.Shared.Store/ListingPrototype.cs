using System;
using System.Collections.Generic;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Store;

[Prototype(null, 1)]
[DataDefinition]
public sealed class ListingPrototype : ListingData, IPrototype, ISerializationGenerated<ListingPrototype>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2> Cost
	{
		get
		{
			return OriginalCost;
		}
		set
		{
			OriginalCost = value;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ListingPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		ListingData definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ListingPrototype)definitionCast;
		if (!serialization.TryCustomCopy<ListingPrototype>(this, ref target, hookCtx, false, context))
		{
			IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2> CostTemp = null;
			if (Cost == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(Cost, ref CostTemp, hookCtx, true, context))
			{
				CostTemp = serialization.CreateCopy<IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(Cost, hookCtx, context, false);
			}
			target.Cost = CostTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ListingPrototype target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref ListingData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ListingPrototype cast = (ListingPrototype)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ListingPrototype cast = (ListingPrototype)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ListingPrototype Instantiate()
	{
		return new ListingPrototype();
	}
}
