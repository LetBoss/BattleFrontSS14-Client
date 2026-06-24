using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Events;

[Serializable]
[NetSerializable]
public sealed class PubgEventClaimResultMessage : EntityEventArgs
{
	public bool Success { get; }

	public string? Error { get; }

	public PubgEventClaimResultInfo? ClaimResult { get; }

	public int Coins { get; }

	public int Scrap { get; }

	public int PremiumCoins { get; }

	public bool HasBalances { get; }

	public List<PubgEventWalletDeltaInfo> WalletsDelta { get; }

	public bool HubHasClaimable { get; }

	public PubgEventClaimResultMessage(bool success, string? error, PubgEventClaimResultInfo? claimResult, int coins, int scrap, int premiumCoins, bool hasBalances, List<PubgEventWalletDeltaInfo> walletsDelta, bool hubHasClaimable)
	{
		Success = success;
		Error = error;
		ClaimResult = claimResult;
		Coins = coins;
		Scrap = scrap;
		PremiumCoins = premiumCoins;
		HasBalances = hasBalances;
		WalletsDelta = walletsDelta;
		HubHasClaimable = hubHasClaimable;
	}
}
