using System;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Stats;

[Serializable]
[NetSerializable]
public sealed class CivRoundTopAward
{
	public string AwardId { get; set; } = string.Empty;

	public string Title { get; set; } = string.Empty;

	public NetUserId UserId { get; set; }

	public string PlayerName { get; set; } = string.Empty;

	public string ValueText { get; set; } = string.Empty;
}
