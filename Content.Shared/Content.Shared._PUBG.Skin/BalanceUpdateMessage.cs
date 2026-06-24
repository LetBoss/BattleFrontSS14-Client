using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public sealed class BalanceUpdateMessage : EntityEventArgs
{
	public int Coins { get; }

	public int Scrap { get; }

	public int PremiumCoins { get; }

	public BalanceUpdateMessage(int coins, int scrap, int premiumCoins)
	{
		Coins = coins;
		Scrap = scrap;
		PremiumCoins = premiumCoins;
	}
}
