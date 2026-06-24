using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles;

[Serializable]
[NetSerializable]
public enum GhostRoleKind
{
	FirstComeFirstServe,
	RaffleReady,
	RaffleInProgress,
	RaffleJoined
}
