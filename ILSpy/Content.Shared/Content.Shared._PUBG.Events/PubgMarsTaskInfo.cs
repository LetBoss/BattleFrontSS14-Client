using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Events;

[Serializable]
[NetSerializable]
public sealed class PubgMarsTaskInfo
{
	public string TemplateId { get; set; } = string.Empty;

	public string TaskKey { get; set; } = string.Empty;

	public string Category { get; set; } = string.Empty;

	public string PeriodType { get; set; } = string.Empty;

	public string ObjectiveType { get; set; } = string.Empty;

	public int TargetValue { get; set; }

	public int TokenReward { get; set; }

	public int CoinsReward { get; set; }

	public int? MinSurvivalSeconds { get; set; }

	public int Progress { get; set; }

	public bool IsCompleted { get; set; }

	public bool IsClaimed { get; set; }

	public string PeriodKey { get; set; } = string.Empty;

	public bool IsClaimable { get; set; }
}
