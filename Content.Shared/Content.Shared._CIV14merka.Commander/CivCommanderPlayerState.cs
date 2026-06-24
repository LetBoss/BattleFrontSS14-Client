using System;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderPlayerState
{
	public NetUserId UserId { get; }

	public string Name { get; }

	public int TeamId { get; }

	public int SquadId { get; }

	public bool IsSquadLeader { get; }

	public bool IsCommander { get; }

	public bool IsSelf { get; }

	public CivCommanderPlayerState(NetUserId userId, string name, int teamId, int squadId, bool isSquadLeader, bool isCommander, bool isSelf)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UserId = userId;
		Name = name;
		TeamId = teamId;
		SquadId = squadId;
		IsSquadLeader = isSquadLeader;
		IsCommander = isCommander;
		IsSelf = isSelf;
	}
}
