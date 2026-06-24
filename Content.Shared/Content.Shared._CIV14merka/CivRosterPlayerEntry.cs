using System;
using Content.Shared._CIV14merka.Teams;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRosterPlayerEntry
{
	public NetUserId UserId { get; }

	public string Name { get; }

	public int? TeamId { get; }

	public int? SquadId { get; }

	public bool IsLeader { get; }

	public bool IsSelected { get; }

	public CivRosterPlayerState State { get; }

	public CivTdmClass Class { get; }

	public CivRosterPlayerEntry(NetUserId userId, string name, int? teamId, int? squadId, bool isLeader, bool isSelected, CivRosterPlayerState state, CivTdmClass selectedClass)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UserId = userId;
		Name = name;
		TeamId = teamId;
		SquadId = squadId;
		IsLeader = isLeader;
		IsSelected = isSelected;
		State = state;
		Class = selectedClass;
	}
}
