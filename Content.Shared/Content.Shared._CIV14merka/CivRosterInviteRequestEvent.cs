using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRosterInviteRequestEvent : EntityEventArgs
{
	public int TeamId { get; }

	public int SquadId { get; }

	public NetUserId TargetUserId { get; }

	public CivRosterInviteRequestEvent(int teamId, int squadId, NetUserId targetUserId)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		TeamId = teamId;
		SquadId = squadId;
		TargetUserId = targetUserId;
	}
}
