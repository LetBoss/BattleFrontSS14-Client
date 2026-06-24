using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public sealed class SponsorTierInfo
{
	public string TierKey { get; set; } = string.Empty;

	public string TierName { get; set; } = string.Empty;

	public string? Badge { get; set; }

	public string? Color { get; set; }

	public int Priority { get; set; }
}
