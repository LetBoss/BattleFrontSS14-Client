using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Events;

[Serializable]
[NetSerializable]
public sealed class PubgMarsEventStateInfo
{
	public int Points { get; set; }

	public string BalanceLabelKey { get; set; } = "pubg-events-mars-token-balance";

	public string TaskRewardLabelKey { get; set; } = "pubg-events-task-token-reward";

	public PubgEventWalletInfo Wallet { get; set; } = new PubgEventWalletInfo();

	public List<PubgMarsMilestoneInfo> Milestones { get; set; } = new List<PubgMarsMilestoneInfo>();

	public List<PubgMarsTaskInfo> LoginTasks { get; set; } = new List<PubgMarsTaskInfo>();

	public List<PubgMarsTaskInfo> ChallengeTasks { get; set; } = new List<PubgMarsTaskInfo>();

	public string WeekKey { get; set; } = string.Empty;

	public DateTime WeekStartsAt { get; set; }

	public DateTime WeekEndsAt { get; set; }
}
