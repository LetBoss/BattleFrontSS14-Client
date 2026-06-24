using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.BattlePass;

[Serializable]
[NetSerializable]
public sealed class BattlePassTaskInfo
{
	public string Id { get; set; } = string.Empty;

	public int Slot { get; set; }

	public string TaskType { get; set; } = string.Empty;

	public string NameKey { get; set; } = string.Empty;

	public int TargetValue { get; set; }

	public int XpReward { get; set; }

	public string? RequiredPerm { get; set; }

	public string? WeaponId { get; set; }

	public int Progress { get; set; }

	public bool IsCompleted { get; set; }

	public bool IsSkipped { get; set; }

	public bool XpClaimed { get; set; }
}
