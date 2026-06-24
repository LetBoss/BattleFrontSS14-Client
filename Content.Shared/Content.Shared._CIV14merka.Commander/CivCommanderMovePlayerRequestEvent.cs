using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderMovePlayerRequestEvent : EntityEventArgs
{
	public NetUserId TargetUserId { get; }

	public int DestinationSquadId { get; }

	public bool CreateNewSquad { get; }

	public CivCommanderMovePlayerRequestEvent(NetUserId targetUserId, int destinationSquadId, bool createNewSquad = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TargetUserId = targetUserId;
		DestinationSquadId = destinationSquadId;
		CreateNewSquad = createNewSquad;
	}
}
