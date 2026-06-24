using System;
using System.Collections.Generic;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Store.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class StoreComponent : Component, ISerializationGenerated<StoreComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public LocId Name = LocId.op_Implicit("store-ui-default-title");

	[DataField(null, false, 1, false, false, null)]
	public HashSet<ProtoId<StoreCategoryPrototype>> Categories = new HashSet<ProtoId<StoreCategoryPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> Balance = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>();

	[DataField(null, false, 1, false, false, null)]
	public HashSet<ProtoId<CurrencyPrototype>> CurrencyWhitelist = new HashSet<ProtoId<CurrencyPrototype>>();

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? AccountOwner;

	[DataField(null, false, 1, false, false, null)]
	public HashSet<ListingDataWithCostModifiers> FullListingsCatalog = new HashSet<ListingDataWithCostModifiers>();

	[ViewVariables]
	public HashSet<ListingDataWithCostModifiers> LastAvailableListings = new HashSet<ListingDataWithCostModifiers>();

	[ViewVariables]
	[DataField(null, false, 1, false, false, null)]
	public List<EntityUid> BoughtEntities = new List<EntityUid>();

	[ViewVariables]
	[DataField(null, false, 1, false, false, null)]
	public Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> BalanceSpent = new Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>();

	[ViewVariables]
	[DataField(null, false, 1, false, false, null)]
	public bool RefundAllowed;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	[DataField(null, false, 1, false, false, null)]
	public bool OwnerOnly;

	[DataField(null, false, 1, false, false, null)]
	public EntityUid? StartingMap;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier BuySuccessSound = (SoundSpecifier)new SoundPathSpecifier("/Audio/Effects/kaching.ogg", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StoreComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StoreComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StoreComponent>(this, ref target, hookCtx, false, context))
		{
			LocId NameTemp = default(LocId);
			if (!serialization.TryCustomCopy<LocId>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = serialization.CreateCopy<LocId>(Name, hookCtx, context, false);
			}
			target.Name = NameTemp;
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
			Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> BalanceTemp = null;
			if (Balance == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(Balance, ref BalanceTemp, hookCtx, true, context))
			{
				BalanceTemp = serialization.CreateCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(Balance, hookCtx, context, false);
			}
			target.Balance = BalanceTemp;
			HashSet<ProtoId<CurrencyPrototype>> CurrencyWhitelistTemp = null;
			if (CurrencyWhitelist == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ProtoId<CurrencyPrototype>>>(CurrencyWhitelist, ref CurrencyWhitelistTemp, hookCtx, true, context))
			{
				CurrencyWhitelistTemp = serialization.CreateCopy<HashSet<ProtoId<CurrencyPrototype>>>(CurrencyWhitelist, hookCtx, context, false);
			}
			target.CurrencyWhitelist = CurrencyWhitelistTemp;
			EntityUid? AccountOwnerTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(AccountOwner, ref AccountOwnerTemp, hookCtx, false, context))
			{
				AccountOwnerTemp = serialization.CreateCopy<EntityUid?>(AccountOwner, hookCtx, context, false);
			}
			target.AccountOwner = AccountOwnerTemp;
			HashSet<ListingDataWithCostModifiers> FullListingsCatalogTemp = null;
			if (FullListingsCatalog == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<ListingDataWithCostModifiers>>(FullListingsCatalog, ref FullListingsCatalogTemp, hookCtx, true, context))
			{
				FullListingsCatalogTemp = serialization.CreateCopy<HashSet<ListingDataWithCostModifiers>>(FullListingsCatalog, hookCtx, context, false);
			}
			target.FullListingsCatalog = FullListingsCatalogTemp;
			List<EntityUid> BoughtEntitiesTemp = null;
			if (BoughtEntities == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<EntityUid>>(BoughtEntities, ref BoughtEntitiesTemp, hookCtx, true, context))
			{
				BoughtEntitiesTemp = serialization.CreateCopy<List<EntityUid>>(BoughtEntities, hookCtx, context, false);
			}
			target.BoughtEntities = BoughtEntitiesTemp;
			Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> BalanceSpentTemp = null;
			if (BalanceSpent == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(BalanceSpent, ref BalanceSpentTemp, hookCtx, true, context))
			{
				BalanceSpentTemp = serialization.CreateCopy<Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2>>(BalanceSpent, hookCtx, context, false);
			}
			target.BalanceSpent = BalanceSpentTemp;
			bool RefundAllowedTemp = false;
			if (!serialization.TryCustomCopy<bool>(RefundAllowed, ref RefundAllowedTemp, hookCtx, false, context))
			{
				RefundAllowedTemp = RefundAllowed;
			}
			target.RefundAllowed = RefundAllowedTemp;
			bool OwnerOnlyTemp = false;
			if (!serialization.TryCustomCopy<bool>(OwnerOnly, ref OwnerOnlyTemp, hookCtx, false, context))
			{
				OwnerOnlyTemp = OwnerOnly;
			}
			target.OwnerOnly = OwnerOnlyTemp;
			EntityUid? StartingMapTemp = null;
			if (!serialization.TryCustomCopy<EntityUid?>(StartingMap, ref StartingMapTemp, hookCtx, false, context))
			{
				StartingMapTemp = serialization.CreateCopy<EntityUid?>(StartingMap, hookCtx, context, false);
			}
			target.StartingMap = StartingMapTemp;
			SoundSpecifier BuySuccessSoundTemp = null;
			if (BuySuccessSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(BuySuccessSound, ref BuySuccessSoundTemp, hookCtx, true, context))
			{
				BuySuccessSoundTemp = serialization.CreateCopy<SoundSpecifier>(BuySuccessSound, hookCtx, context, false);
			}
			target.BuySuccessSound = BuySuccessSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StoreComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StoreComponent cast = (StoreComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StoreComponent cast = (StoreComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StoreComponent def = (StoreComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StoreComponent Instantiate()
	{
		return new StoreComponent();
	}
}
