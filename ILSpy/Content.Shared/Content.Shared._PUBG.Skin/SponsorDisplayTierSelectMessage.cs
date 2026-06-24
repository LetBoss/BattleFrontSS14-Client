using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public sealed class SponsorDisplayTierSelectMessage : EntityEventArgs
{
	public SponsorDisplayMode Mode { get; }

	public string? TierKey { get; }

	public SponsorDisplayTierSelectMessage(SponsorDisplayMode mode, string? tierKey = null)
	{
		Mode = mode;
		TierKey = tierKey;
	}
}
