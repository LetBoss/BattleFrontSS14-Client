using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Events;

[Serializable]
[NetSerializable]
public sealed class PubgEventCardHubSummaryInfo
{
	public int ProgressCurrent { get; set; }

	public int ProgressTarget { get; set; }

	public int? NextRewardThreshold { get; set; }

	public int NextRewardIn { get; set; }

	public int DailyCompleted { get; set; }

	public int DailyTotal { get; set; }

	public List<int> MilestoneThresholds { get; set; } = new List<int>();
}
