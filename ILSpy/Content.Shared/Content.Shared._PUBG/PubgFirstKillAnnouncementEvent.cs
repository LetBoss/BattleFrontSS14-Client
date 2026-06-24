using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG;

[Serializable]
[NetSerializable]
public sealed class PubgFirstKillAnnouncementEvent : EntityEventArgs
{
	public string BannerPrototypeId { get; }

	public PubgFirstKillAnnouncementEvent(string bannerPrototypeId)
	{
		BannerPrototypeId = bannerPrototypeId;
	}
}
