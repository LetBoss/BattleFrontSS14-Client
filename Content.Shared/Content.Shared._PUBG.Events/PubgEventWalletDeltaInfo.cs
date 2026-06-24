using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Events;

[Serializable]
[NetSerializable]
public sealed class PubgEventWalletDeltaInfo
{
	public string WalletKey { get; set; } = string.Empty;

	public int Delta { get; set; }

	public int Balance { get; set; }

	public int TotalEarned { get; set; }
}
