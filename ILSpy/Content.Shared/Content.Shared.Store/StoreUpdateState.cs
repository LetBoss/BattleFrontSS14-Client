using System;
using System.Collections.Generic;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Store;

[Serializable]
[NetSerializable]
public sealed class StoreUpdateState : BoundUserInterfaceState
{
	public readonly HashSet<ListingDataWithCostModifiers> Listings;

	public readonly Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> Balance;

	public readonly bool ShowFooter;

	public readonly bool AllowRefund;

	public StoreUpdateState(HashSet<ListingDataWithCostModifiers> listings, Dictionary<ProtoId<CurrencyPrototype>, FixedPoint2> balance, bool showFooter, bool allowRefund)
	{
		Listings = listings;
		Balance = balance;
		ShowFooter = showFooter;
		AllowRefund = allowRefund;
	}
}
