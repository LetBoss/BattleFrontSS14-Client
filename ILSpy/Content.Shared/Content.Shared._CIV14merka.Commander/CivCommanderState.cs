using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderState
{
	public bool IsCommander { get; }

	public int TeamId { get; }

	public int Currency { get; }

	public List<CivCommanderShopEntryState> ShopEntries { get; }

	public List<CivCommanderSquadState> Squads { get; }

	public List<CivCommanderPlayerState> ReservePlayers { get; }

	public List<PurchaseRequestEntryState> PurchaseRequests { get; }

	public CivCommanderState(bool isCommander, int teamId, int currency, IEnumerable<CivCommanderShopEntryState> shopEntries, IEnumerable<CivCommanderSquadState> squads, IEnumerable<CivCommanderPlayerState> reservePlayers, IEnumerable<PurchaseRequestEntryState>? purchaseRequests = null)
	{
		IsCommander = isCommander;
		TeamId = teamId;
		Currency = currency;
		ShopEntries = shopEntries.ToList();
		Squads = squads.ToList();
		ReservePlayers = reservePlayers.ToList();
		PurchaseRequests = purchaseRequests?.ToList() ?? new List<PurchaseRequestEntryState>();
	}
}
