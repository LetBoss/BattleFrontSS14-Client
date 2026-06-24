using System;
using System.Collections.Generic;
using Content.Shared.FixedPoint;
using Content.Shared.Store;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.StoreDiscount.Components;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class StoreDiscountData : ISerializationGenerated<StoreDiscountData>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public ProtoId<ListingPrototype> ListingId;

	[DataField(null, false, 1, false, false, null)]
	public int Count;

	[DataField(null, false, 1, true, false, null)]
	public ProtoId<DiscountCategoryPrototype> DiscountCategory;

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> DiscountAmountByCurrency = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StoreDiscountData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<StoreDiscountData>(this, ref target, hookCtx, false, context))
		{
			ProtoId<ListingPrototype> ListingIdTemp = default(ProtoId<ListingPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<ListingPrototype>>(ListingId, ref ListingIdTemp, hookCtx, false, context))
			{
				ListingIdTemp = serialization.CreateCopy<ProtoId<ListingPrototype>>(ListingId, hookCtx, context, false);
			}
			target.ListingId = ListingIdTemp;
			int CountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Count, ref CountTemp, hookCtx, false, context))
			{
				CountTemp = Count;
			}
			target.Count = CountTemp;
			ProtoId<DiscountCategoryPrototype> DiscountCategoryTemp = default(ProtoId<DiscountCategoryPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<DiscountCategoryPrototype>>(DiscountCategory, ref DiscountCategoryTemp, hookCtx, false, context))
			{
				DiscountCategoryTemp = serialization.CreateCopy<ProtoId<DiscountCategoryPrototype>>(DiscountCategory, hookCtx, context, false);
			}
			target.DiscountCategory = DiscountCategoryTemp;
			Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> DiscountAmountByCurrencyTemp = null;
			if (DiscountAmountByCurrency == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(DiscountAmountByCurrency, ref DiscountAmountByCurrencyTemp, hookCtx, true, context))
			{
				DiscountAmountByCurrencyTemp = serialization.CreateCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(DiscountAmountByCurrency, hookCtx, context, false);
			}
			target.DiscountAmountByCurrency = DiscountAmountByCurrencyTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StoreDiscountData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StoreDiscountData cast = (StoreDiscountData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public StoreDiscountData Instantiate()
	{
		return new StoreDiscountData();
	}
}
