using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.BattlePass;

[Serializable]
[NetSerializable]
public sealed class BattlePassRewardInfo
{
	public string Id { get; set; } = string.Empty;

	public bool IsPremium { get; set; }

	public string RewardType { get; set; } = string.Empty;

	public string RewardValue { get; set; } = string.Empty;

	public int? Duration { get; set; }
}
