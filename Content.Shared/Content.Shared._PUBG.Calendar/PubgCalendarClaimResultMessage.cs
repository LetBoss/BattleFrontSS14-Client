using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Calendar;

[Serializable]
[NetSerializable]
public sealed class PubgCalendarClaimResultMessage : EntityEventArgs
{
	public bool Success { get; }

	public string? Error { get; }

	public int ClaimedDay { get; }

	public string? RewardType { get; }

	public string? RewardValue { get; }

	public int? DurationDays { get; }

	public int NextDayToClaim { get; }

	public bool CanClaimToday { get; }

	public PubgCalendarClaimResultMessage(bool success, string? error = null, int claimedDay = 0, string? rewardType = null, string? rewardValue = null, int? durationDays = null, int nextDayToClaim = 0, bool canClaimToday = false)
	{
		Success = success;
		Error = error;
		ClaimedDay = claimedDay;
		RewardType = rewardType;
		RewardValue = rewardValue;
		DurationDays = durationDays;
		NextDayToClaim = nextDayToClaim;
		CanClaimToday = canClaimToday;
	}
}
