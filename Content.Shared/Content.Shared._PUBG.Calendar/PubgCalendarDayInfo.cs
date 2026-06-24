using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Calendar;

[Serializable]
[NetSerializable]
public sealed class PubgCalendarDayInfo
{
	public int Day { get; set; }

	public string RewardType { get; set; } = string.Empty;

	public string RewardValue { get; set; } = string.Empty;

	public int? DurationDays { get; set; }
}
