using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Tracker.SquadLeader;

[Serializable]
[NetSerializable]
public sealed class SquadLeaderTrackerDemoteFireteamLeaderMsg(int fireteam) : BoundUserInterfaceMessage
{
	public readonly int Fireteam = fireteam;
}
