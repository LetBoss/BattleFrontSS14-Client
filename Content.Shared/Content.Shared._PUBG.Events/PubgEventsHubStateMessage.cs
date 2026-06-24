using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Events;

[Serializable]
[NetSerializable]
public sealed class PubgEventsHubStateMessage : EntityEventArgs
{
	public DateTime ServerNowUtc { get; }

	public bool HubHasClaimable { get; }

	public List<PubgEventCardInfo> Events { get; }

	public PubgEventsHubStateMessage(DateTime serverNowUtc, bool hubHasClaimable, List<PubgEventCardInfo> events)
	{
		ServerNowUtc = serverNowUtc;
		HubHasClaimable = hubHasClaimable;
		Events = events;
	}
}
