using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.BattlePass;

[Serializable]
[NetSerializable]
public sealed class BattlePassStateMessage : EntityEventArgs
{
	public string SeasonId { get; }

	public string SeasonName { get; }

	public int CurrentXp { get; }

	public int CurrentLevel { get; }

	public bool HasPremium { get; }

	public List<BattlePassLevelInfo> Levels { get; }

	public List<string> ClaimedRewardIds { get; }

	public List<BattlePassTaskInfo> Tasks { get; }

	public DateTime TasksEndAt { get; }

	public int SkipsRemaining { get; }

	public BattlePassStateMessage(string seasonId, string seasonName, int currentXp, int currentLevel, bool hasPremium, List<BattlePassLevelInfo> levels, List<string> claimedRewardIds, List<BattlePassTaskInfo> tasks, DateTime tasksEndAt, int skipsRemaining)
	{
		SeasonId = seasonId;
		SeasonName = seasonName;
		CurrentXp = currentXp;
		CurrentLevel = currentLevel;
		HasPremium = hasPremium;
		Levels = levels;
		ClaimedRewardIds = claimedRewardIds;
		Tasks = tasks;
		TasksEndAt = tasksEndAt;
		SkipsRemaining = skipsRemaining;
	}
}
