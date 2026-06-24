using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Events;

[Serializable]
[NetSerializable]
public sealed class PubgEventClaimMessage : EntityEventArgs
{
	public string EventKey { get; }

	public string ClaimType { get; }

	public string ClaimId { get; }

	public PubgEventClaimMessage(string eventKey, string claimType, string claimId)
	{
		EventKey = eventKey;
		ClaimType = claimType;
		ClaimId = claimId;
	}
}
