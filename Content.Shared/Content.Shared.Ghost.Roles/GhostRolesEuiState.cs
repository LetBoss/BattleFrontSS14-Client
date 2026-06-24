using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles;

[Serializable]
[NetSerializable]
public sealed class GhostRolesEuiState : EuiStateBase
{
	public GhostRoleInfo[] GhostRoles { get; }

	public GhostRolesEuiState(GhostRoleInfo[] ghostRoles)
	{
		GhostRoles = ghostRoles;
	}
}
