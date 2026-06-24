using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Calendar;

[Serializable]
[NetSerializable]
public sealed class PubgCalendarStateMessage : EntityEventArgs
{
	public string MonthKey { get; }

	public int Year { get; }

	public int Month { get; }

	public DateTime StartsAt { get; }

	public DateTime EndsAt { get; }

	public DateTime ServerNowUtc { get; }

	public List<PubgCalendarDayInfo> Days { get; }

	public int NextDayToClaim { get; }

	public bool CanClaimToday { get; }

	public DateTime? LastClaimAt { get; }

	public List<int> ClaimedDays { get; }

	public int MaxUnlockDay { get; }

	public PubgCalendarStateMessage(string monthKey, int year, int month, DateTime startsAt, DateTime endsAt, DateTime serverNowUtc, List<PubgCalendarDayInfo> days, int nextDayToClaim, bool canClaimToday, DateTime? lastClaimAt, List<int> claimedDays, int maxUnlockDay)
	{
		MonthKey = monthKey;
		Year = year;
		Month = month;
		StartsAt = startsAt;
		EndsAt = endsAt;
		ServerNowUtc = serverNowUtc;
		Days = days;
		NextDayToClaim = nextDayToClaim;
		CanClaimToday = canClaimToday;
		LastClaimAt = lastClaimAt;
		ClaimedDays = claimedDays;
		MaxUnlockDay = maxUnlockDay;
	}
}
