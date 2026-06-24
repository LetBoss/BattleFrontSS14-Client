using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRosterInvitePromptEvent : EntityEventArgs
{
	public int InviteId { get; }

	public string InviterName { get; }

	public string TeamName { get; }

	public int TeamId { get; }

	public int SquadId { get; }

	public CivRosterInvitePromptEvent(int inviteId, string inviterName, string teamName, int teamId, int squadId)
	{
		InviteId = inviteId;
		InviterName = inviterName;
		TeamName = teamName;
		TeamId = teamId;
		SquadId = squadId;
	}
}
