using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Events;

[Serializable]
[NetSerializable]
public sealed class PubgMarsMilestoneInfo
{
	public string MilestoneId { get; set; } = string.Empty;

	public int Threshold { get; set; }

	public string RewardType { get; set; } = string.Empty;

	public string RewardValue { get; set; } = string.Empty;

	public int? DurationDays { get; set; }

	public bool IsClaimed { get; set; }

	public DateTime? ClaimedAt { get; set; }

	public bool IsClaimable { get; set; }
}
