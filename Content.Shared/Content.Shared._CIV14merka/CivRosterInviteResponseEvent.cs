using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRosterInviteResponseEvent : EntityEventArgs
{
	public int InviteId { get; }

	public bool Accept { get; }

	public CivRosterInviteResponseEvent(int inviteId, bool accept)
	{
		InviteId = inviteId;
		Accept = accept;
	}
}
