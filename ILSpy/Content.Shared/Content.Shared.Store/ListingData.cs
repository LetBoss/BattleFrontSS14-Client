using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Store;

[Serializable]
[NetSerializable]
[Virtual]
[DataDefinition]
public class ListingData : IEquatable<ListingData>, ISerializationGenerated<ListingData>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string? Name;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<DiscountCategoryPrototype>? DiscountCategory;

	[DataField(null, false, 1, false, false, null)]
	public string? Description;

	[DataField(null, false, 1, false, false, null)]
	public HashSet<ProtoId<StoreCategoryPrototype>> Categories = new HashSet<ProtoId<StoreCategoryPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2> OriginalCost = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>();

	[NonSerialized]
	[DataField(null, false, 1, false, true, null)]
	public List<ListingCondition>? Conditions;

	[DataField(null, false, 1, false, false, null)]
	public SpriteSpecifier? Icon;

	[DataField(null, false, 1, false, false, null)]
	public int Priority;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? ProductEntity;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? ProductAction;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<ListingPrototype>? ProductUpgradeId;

	[NonSerialized]
	[DataField(null, false, 1, false, false, null)]
	public EntityUid? ProductActionEntity;

	[DataField(null, false, 1, false, false, null)]
	public object? ProductEvent;

	[DataField(null, false, 1, false, false, null)]
	public bool RaiseProductEventOnUser;

	[DataField(null, false, 1, false, false, null)]
	public int PurchaseAmount;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan RestockTime = TimeSpan.Zero;

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> DiscountDownTo = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>();

	[DataField(null, false, 1, false, false, null)]
	public bool DisableRefund;

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	public ListingData()
	{
	}

	public ListingData(ListingData other)
		: this(other.Name, other.DiscountCategory, other.Description, other.Conditions, other.Icon, other.Priority, other.ProductEntity, other.ProductAction, other.ProductUpgradeId, other.ProductActionEntity, other.ProductEvent, other.RaiseProductEventOnUser, other.PurchaseAmount, other.ID, other.Categories, other.OriginalCost, other.RestockTime, other.DiscountDownTo, other.DisableRefund)
	{
	}

	public ListingData(string? name, ProtoId<DiscountCategoryPrototype>? discountCategory, string? description, List<ListingCondition>? conditions, SpriteSpecifier? icon, int priority, EntProtoId? productEntity, EntProtoId? productAction, ProtoId<ListingPrototype>? productUpgradeId, EntityUid? productActionEntity, object? productEvent, bool raiseProductEventOnUser, int purchaseAmount, string id, HashSet<ProtoId<StoreCategoryPrototype>> categories, IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2> originalCost, TimeSpan restockTime, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> dataDiscountDownTo, bool disableRefund)
	{
		Name = name;
		DiscountCategory = discountCategory;
		Description = description;
		Conditions = conditions?.ToList();
		Icon = icon;
		Priority = priority;
		ProductEntity = productEntity;
		ProductAction = productAction;
		ProductUpgradeId = productUpgradeId;
		ProductActionEntity = productActionEntity;
		ProductEvent = productEvent;
		RaiseProductEventOnUser = raiseProductEventOnUser;
		PurchaseAmount = purchaseAmount;
		ID = id;
		Categories = categories.ToHashSet();
		OriginalCost = originalCost;
		RestockTime = restockTime;
		DiscountDownTo = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>(dataDiscountDownTo);
		DisableRefund = disableRefund;
	}

	public bool Equals(ListingData? listing)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (listing == null)
		{
			return false;
		}
		if (Priority == listing.Priority && !(Name != listing.Name) && !(Description != listing.Description))
		{
			EntProtoId? productEntity = ProductEntity;
			EntProtoId? productEntity2 = listing.ProductEntity;
			if (productEntity.HasValue == productEntity2.HasValue && (!productEntity.HasValue || !(productEntity.GetValueOrDefault() != productEntity2.GetValueOrDefault())))
			{
				productEntity2 = ProductAction;
				productEntity = listing.ProductAction;
				if (productEntity2.HasValue == productEntity.HasValue && (!productEntity2.HasValue || !(productEntity2.GetValueOrDefault() != productEntity.GetValueOrDefault())) && !(ProductEvent?.GetType() != listing.ProductEvent?.GetType()) && !(RestockTime != listing.RestockTime))
				{
					if (Icon != null && !((object)Icon).Equals((object?)listing.Icon))
					{
						return false;
					}
					if (!Categories.OrderBy<ProtoId<StoreCategoryPrototype>, ProtoId<StoreCategoryPrototype>>((ProtoId<StoreCategoryPrototype> x) => x).SequenceEqual(listing.Categories.OrderBy<ProtoId<StoreCategoryPrototype>, ProtoId<StoreCategoryPrototype>>((ProtoId<StoreCategoryPrototype> x) => x)))
					{
						return false;
					}
					if (!OriginalCost.OrderBy<KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>, KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>>((KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2> x) => x).SequenceEqual(listing.OriginalCost.OrderBy<KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>, KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2>>((KeyValuePair<ProtoId<CurrencyPrototype>, FixedPoint2> x) => x)))
					{
						return false;
					}
					if (Conditions != null && listing.Conditions != null && !Conditions.OrderBy((ListingCondition x) => x).SequenceEqual(listing.Conditions.OrderBy((ListingCondition x) => x)))
					{
						return false;
					}
					return true;
				}
			}
		}
		return false;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref ListingData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<ListingData>(this, ref target, hookCtx, false, context))
		{
			string IDTemp = null;
			if (ID == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(ID, ref IDTemp, hookCtx, false, context))
			{
				IDTemp = ID;
			}
			target.ID = IDTemp;
			string NameTemp = null;
			if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = Name;
			}
			target.Name = NameTemp;
			ProtoId<DiscountCategoryPrototype>? DiscountCategoryTemp = null;
			if (!serialization.TryCustomCopy<ProtoId<DiscountCategoryPrototype>?>(DiscountCategory, ref DiscountCategoryTemp, hookCtx, false, context))
			{
				DiscountCategoryTemp = serialization.CreateCopy<ProtoId<DiscountCategoryPrototype>?>(DiscountCategory, hookCtx, context, false);
			}
			target.DiscountCategory = DiscountCategoryTemp;
			string DescriptionTemp = null;
			if (!serialization.TryCustomCopy<string>(Description, ref DescriptionTemp, hookCtx, false, context))
			{
				DescriptionTemp = Description;
			}
			target.Description = DescriptionTemp;
			HashSet<ProtoId<StoreCategoryPrototype>> CategoriesTemp = null;
			if (Categories == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ProtoId<StoreCategoryPrototype>>>(Categories, ref CategoriesTemp, hookCtx, true, context))
			{
				CategoriesTemp = serialization.CreateCopy<HashSet<ProtoId<StoreCategoryPrototype>>>(Categories, hookCtx, context, false);
			}
			target.Categories = CategoriesTemp;
			IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2> OriginalCostTemp = null;
			if (OriginalCost == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(OriginalCost, ref OriginalCostTemp, hookCtx, true, context))
			{
				OriginalCostTemp = serialization.CreateCopy<IReadOnlyDictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(OriginalCost, hookCtx, context, false);
			}
			target.OriginalCost = OriginalCostTemp;
			List<ListingCondition> ConditionsTemp = null;
			if (!serialization.TryCustomCopy<List<ListingCondition>>(Conditions, ref ConditionsTemp, hookCtx, true, context))
			{
				ConditionsTemp = serialization.CreateCopy<List<ListingCondition>>(Conditions, hookCtx, context, false);
			}
			target.Conditions = ConditionsTemp;
			SpriteSpecifier IconTemp = null;
			if (!serialization.TryCustomCopy<SpriteSpecifier>(Icon, ref IconTemp, hookCtx, true, context))
			{
				IconTemp = serialization.CreateCopy<SpriteSpecifier>(Icon, hookCtx, context, false);
			}
			target.Icon = IconTemp;
			int PriorityTemp = 0;
			if (!serialization.TryCustomCopy<int>(Priority, ref PriorityTemp, hookCtx, false, context))
			{
				PriorityTemp = Priority;
			}
			target.Priority = PriorityTemp;
			EntProtoId? ProductEntityTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(ProductEntity, ref ProductEntityTemp, hookCtx, false, context))
			{
				ProductEntityTemp = serialization.CreateCopy<EntProtoId?>(ProductEntity, hookCtx, context, false);
			}
			target.ProductEntity = ProductEntityTemp;
			EntProtoId? ProductActionTemp = null;
			if (!serialization.TryCustomCopy<EntProtoId?>(ProductAction, ref ProductActionTemp, hookCtx, false, context))
			{
				ProductActionTemp = serialization.CreateCopy<EntProtoId?>(ProductAction, hookCtx, context, false);
			}
			target.ProductAction = ProductActionTemp;
			ProtoId<ListingPrototype>? ProductUpgradeIdTemp = null;
			if (!serialization.TryCustomCopy<ProtoId<ListingPrototype>?>(ProductUpgradeId, ref ProductUpgradeIdTemp, hookCtx, false, context))
			{
				ProductUpgradeIdTemp = serialization.CreateCopy<ProtoId<ListingPrototype>?>(ProductUpgradeId, hookCtx, context, false);
			}
			target.ProductUpgradeId = ProductUpgradeIdTemp;
			EntityUid? ProductActionEntityTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(ProductActionEntity, ref ProductActionEntityTemp, hookCtx, false, context))
			{
				ProductActionEntityTemp = serialization.CreateCopy<EntityUid?>(ProductActionEntity, hookCtx, context, false);
			}
			target.ProductActionEntity = ProductActionEntityTemp;
			object ProductEventTemp = null;
			if (!serialization.TryCustomCopy<object>(ProductEvent, ref ProductEventTemp, hookCtx, true, context))
			{
				ProductEventTemp = serialization.CreateCopy(ProductEvent, hookCtx, context, false);
			}
			target.ProductEvent = ProductEventTemp;
			bool RaiseProductEventOnUserTemp = false;
			if (!serialization.TryCustomCopy<bool>(RaiseProductEventOnUser, ref RaiseProductEventOnUserTemp, hookCtx, false, context))
			{
				RaiseProductEventOnUserTemp = RaiseProductEventOnUser;
			}
			target.RaiseProductEventOnUser = RaiseProductEventOnUserTemp;
			int PurchaseAmountTemp = 0;
			if (!serialization.TryCustomCopy<int>(PurchaseAmount, ref PurchaseAmountTemp, hookCtx, false, context))
			{
				PurchaseAmountTemp = PurchaseAmount;
			}
			target.PurchaseAmount = PurchaseAmountTemp;
			TimeSpan RestockTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(RestockTime, ref RestockTimeTemp, hookCtx, false, context))
			{
				RestockTimeTemp = serialization.CreateCopy<TimeSpan>(RestockTime, hookCtx, context, false);
			}
			target.RestockTime = RestockTimeTemp;
			Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> DiscountDownToTemp = null;
			if (DiscountDownTo == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(DiscountDownTo, ref DiscountDownToTemp, hookCtx, true, context))
			{
				DiscountDownToTemp = serialization.CreateCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(DiscountDownTo, hookCtx, context, false);
			}
			target.DiscountDownTo = DiscountDownToTemp;
			bool DisableRefundTemp = false;
			if (!serialization.TryCustomCopy<bool>(DisableRefund, ref DisableRefundTemp, hookCtx, false, context))
			{
				DisableRefundTemp = DisableRefund;
			}
			target.DisableRefund = DisableRefundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref ListingData target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ListingData cast = (ListingData)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual ListingData Instantiate()
	{
		return new ListingData();
	}
}
