using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.BattlePass;

[Serializable]
[NetSerializable]
public sealed class BattlePassBuyPremiumResultMessage : EntityEventArgs
{
	public bool Success { get; }

	public string? Error { get; }

	public BattlePassBuyPremiumResultMessage(bool success, string? error = null)
	{
		Success = success;
		Error = error;
	}
}
