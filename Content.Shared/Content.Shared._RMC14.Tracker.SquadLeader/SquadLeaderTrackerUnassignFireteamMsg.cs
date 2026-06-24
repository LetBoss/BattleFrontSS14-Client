using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Tracker.SquadLeader;

[Serializable]
[NetSerializable]
public sealed class SquadLeaderTrackerUnassignFireteamMsg(NetEntity marine) : BoundUserInterfaceMessage
{
	public readonly NetEntity Marine = marine;
}
