using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Store;

[Serializable]
[NetSerializable]
public sealed class StoreBuyListingMessage(ProtoId<ListingPrototype> listing) : BoundUserInterfaceMessage
{
	public ProtoId<ListingPrototype> Listing = listing;
}
