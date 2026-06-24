using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost;

[Serializable]
[NetSerializable]
public sealed class GhostUpdateGhostRoleCountEvent : EntityEventArgs
{
	public int AvailableGhostRoles { get; }

	public GhostUpdateGhostRoleCountEvent(int availableGhostRoleCount)
	{
		AvailableGhostRoles = availableGhostRoleCount;
	}
}
