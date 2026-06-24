using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.BattlePass;

[Serializable]
[NetSerializable]
public sealed class BattlePassClaimResultMessage : EntityEventArgs
{
	public bool Success { get; }

	public string? Error { get; }

	public string? RewardType { get; }

	public string? RewardValue { get; }

	public BattlePassClaimResultMessage(bool success, string? error = null, string? rewardType = null, string? rewardValue = null)
	{
		Success = success;
		Error = error;
		RewardType = rewardType;
		RewardValue = rewardValue;
	}
}
