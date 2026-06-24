using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Events;

[Serializable]
[NetSerializable]
public sealed class PubgEventDetailInfo
{
	public string EventKey { get; set; } = string.Empty;

	public string Kind { get; set; } = string.Empty;

	public string TitleKey { get; set; } = string.Empty;

	public string DescriptionKey { get; set; } = string.Empty;

	public string? SelectorIconPath { get; set; }

	public string? SelectorBannerPath { get; set; }

	public string? SelectorAccentHex { get; set; }

	public DateTime StartAt { get; set; }

	public DateTime? EndAt { get; set; }

	public int SortOrder { get; set; }

	public bool IsActive { get; set; }

	public bool HasClaimable { get; set; }

	public bool RedDotOneTime { get; set; }

	public bool RedDotTasks { get; set; }

	public bool RedDotMilestones { get; set; }

	public PubgDiscordEventStateInfo? DiscordState { get; set; }

	public PubgMarsEventStateInfo? MarsState { get; set; }
}
