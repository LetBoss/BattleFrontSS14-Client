using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Store;

[Serializable]
[NetSerializable]
[DataDefinition]
public sealed class ListingDataWithCostModifiers : ListingData, ISerializationGenerated<ListingDataWithCostModifiers>, ISerializationGenerated
{
	private IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>? _costModified;

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<string, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>> CostModifiersBySourceId = new Dictionary<string, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>();

	public bool IsCostModified => CostModifiersBySourceId.Count > 0;

	public IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2> Cost => _costModified ?? (_costModified = ((CostModifiersBySourceId.Count == 0) ? ((IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>?)new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>(OriginalCost)) : ((IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>?)ApplyAllModifiers())));

	public ListingDataWithCostModifiers(ListingData listingData)
		: base(listingData.Name, listingData.DiscountCategory, listingData.Description, listingData.Conditions, listingData.Icon, listingData.Priority, listingData.ProductEntity, listingData.ProductAction, listingData.ProductUpgradeId, listingData.ProductActionEntity, listingData.ProductEvent, listingData.RaiseProductEventOnUser, listingData.PurchaseAmount, listingData.ID, listingData.Categories, listingData.OriginalCost, listingData.RestockTime, listingData.DiscountDownTo, listingData.DisableRefund)
	{
	}

	public void AddCostModifier(string modifierSourceId, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> modifiers)
	{
		CostModifiersBySourceId.Add(modifierSourceId, modifiers);
		if (_costModified != null)
		{
			_costModified = ApplyAllModifiers();
		}
	}

	public void RemoveCostModifier(string modifierSourceId)
	{
		CostModifiersBySourceId.Remove(modifierSourceId);
		if (_costModified != null)
		{
			_costModified = ApplyAllModifiers();
		}
	}

	public bool CanBuyWith(Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> balance)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		foreach (var (currency, amount) in Cost)
		{
			if (!balance.ContainsKey(currency))
			{
				return false;
			}
			if (balance[currency] < amount)
			{
				return false;
			}
		}
		return true;
	}

	public IReadOnlyDictionary<ProtoId<CurrencyPrototype>, float> GetModifiersSummaryRelative()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> dictionary = CostModifiersBySourceId.Aggregate<KeyValuePair<string, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>(), delegate(Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> accumulator, KeyValuePair<string, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>> x)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			foreach (var (key, fixedPoint3) in x.Value)
			{
				accumulator.TryGetValue(key, out var value);
				accumulator[key] = value + fixedPoint3;
			}
			return accumulator;
		});
		Dictionary<ProtoId<CurrencyPrototype>, float> relativeModifiedPercent = new Dictionary<ProtoId<CurrencyPrototype>, float>();
		foreach (var (currency, discountAmount) in dictionary)
		{
			if (OriginalCost.TryGetValue(currency, out var originalAmount))
			{
				float discountPercent = (float)discountAmount.Value / (float)originalAmount.Value;
				relativeModifiedPercent.Add(currency, discountPercent);
			}
		}
		return relativeModifiedPercent;
	}

	private Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> ApplyAllModifiers()
	{
		Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> dictionary = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>(OriginalCost);
		foreach (var (_, modifier) in CostModifiersBySourceId)
		{
			ApplyModifier(dictionary, modifier);
		}
		return dictionary;
	}

	private void ApplyModifier(Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> applyTo, IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2> modifier)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		foreach (var (currency, modifyBy) in modifier)
		{
			if (applyTo.TryGetValue(currency, out var currentAmount))
			{
				FixedPoint2 modifiedAmount = currentAmount + modifyBy;
				if (modifiedAmount < 0)
				{
					modifiedAmount = 0;
				}
				applyTo[currency] = modifiedAmount;
			}
		}
	}

	public ListingDataWithCostModifiers()
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ListingDataWithCostModifiers target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		ListingData definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ListingDataWithCostModifiers)definitionCast;
		if (!serialization.TryCustomCopy<ListingDataWithCostModifiers>(this, ref target, hookCtx, false, context))
		{
			Dictionary<string, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>> CostModifiersBySourceIdTemp = null;
			if (CostModifiersBySourceId == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<string, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>>(CostModifiersBySourceId, ref CostModifiersBySourceIdTemp, hookCtx, true, context))
			{
				CostModifiersBySourceIdTemp = serialization.CreateCopy<Dictionary<string, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>>(CostModifiersBySourceId, hookCtx, context, false);
			}
			target.CostModifiersBySourceId = CostModifiersBySourceIdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ListingDataWithCostModifiers target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref ListingData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ListingDataWithCostModifiers cast = (ListingDataWithCostModifiers)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ListingDataWithCostModifiers cast = (ListingDataWithCostModifiers)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ListingDataWithCostModifiers Instantiate()
	{
		return new ListingDataWithCostModifiers();
	}
}
