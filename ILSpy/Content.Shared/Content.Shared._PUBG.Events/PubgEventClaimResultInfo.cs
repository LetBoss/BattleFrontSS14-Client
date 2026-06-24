using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Events;

[Serializable]
[NetSerializable]
public sealed class PubgEventClaimResultInfo
{
	public string ClaimType { get; set; } = string.Empty;

	public string ClaimId { get; set; } = string.Empty;

	public string? RewardType { get; set; }

	public string? RewardValue { get; set; }

	public int? DurationDays { get; set; }

	public int TokenReward { get; set; }
}
