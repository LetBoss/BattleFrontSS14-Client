using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Events;

[Serializable]
[NetSerializable]
public sealed class PubgEventInventoryStateMessage : EntityEventArgs
{
	public DateTime ServerNowUtc { get; }

	public List<PubgEventWalletInfo> Wallets { get; }

	public List<PubgEventInventoryAssetInfo> Assets { get; }

	public PubgEventInventoryStateMessage(DateTime serverNowUtc, List<PubgEventWalletInfo> wallets, List<PubgEventInventoryAssetInfo> assets)
	{
		ServerNowUtc = serverNowUtc;
		Wallets = wallets;
		Assets = assets;
	}
}
