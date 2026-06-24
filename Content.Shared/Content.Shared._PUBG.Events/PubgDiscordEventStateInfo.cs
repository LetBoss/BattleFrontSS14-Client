using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Events;

[Serializable]
[NetSerializable]
public sealed class PubgDiscordEventStateInfo
{
	public string ClaimKey { get; set; } = string.Empty;

	public string RewardType { get; set; } = string.Empty;

	public string RewardValue { get; set; } = string.Empty;

	public bool Linked { get; set; }

	public bool AlreadyClaimed { get; set; }

	public DateTime? ClaimedAt { get; set; }

	public bool CanClaim { get; set; }
}
