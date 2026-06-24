using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderShopPurchaseRequestEvent : EntityEventArgs
{
	public string EntryId { get; }

	public CivCommanderShopPurchaseRequestEvent(string entryId)
	{
		EntryId = entryId;
	}
}
