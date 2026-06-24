using System;
using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._CIV14merka.Commander;

[Prototype(null, 1)]
public sealed class CivCommanderShopEntryPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public string Name = string.Empty;

	[DataField(null, false, 1, true, false, null)]
	public string Description = string.Empty;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? IconEntity;

	[DataField(null, false, 1, false, false, null)]
	public CivCommanderShopEntryKind Kind;

	[DataField(null, false, 1, false, false, null)]
	public CivCommanderShopPurchaseType ServiceType;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId? EntityPrototype;

	[DataField(null, false, 1, false, false, null)]
	public List<EntProtoId>? SquadMembers;

	[DataField(null, false, 1, false, false, null)]
	public int Price;

	[DataField(null, false, 1, false, false, null)]
	public string[] SideIds = Array.Empty<string>();

	[DataField(null, false, 1, false, false, null)]
	public float PriceAfterPurchaseMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public int PriceAfterPurchaseCooldownSeconds;

	[DataField(null, false, 1, false, false, null)]
	public int PurchaseLimitPerTeam;

	[DataField(null, false, 1, false, false, null)]
	public int Order;

	[DataField(null, false, 1, false, false, null)]
	public bool Enabled = true;

	[DataField(null, false, 1, false, false, null)]
	public bool RequiresPurchaseAnchor = true;

	[DataField(null, false, 1, false, false, null)]
	public bool KeepPlacing = true;

	[IdDataField(1, null)]
	public string ID { get; private set; }

	public bool IsAvailableForSide(string? sideId)
	{
		if (SideIds.Length == 0)
		{
			return true;
		}
		if (string.IsNullOrWhiteSpace(sideId))
		{
			return false;
		}
		string[] sideIds = SideIds;
		foreach (string allowedSideId in sideIds)
		{
			if (!string.IsNullOrWhiteSpace(allowedSideId) && string.Equals(allowedSideId, sideId, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}
}
