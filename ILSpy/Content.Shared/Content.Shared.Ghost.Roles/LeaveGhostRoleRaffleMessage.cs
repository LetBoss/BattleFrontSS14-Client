using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles;

[Serializable]
[NetSerializable]
public sealed class LeaveGhostRoleRaffleMessage : EuiMessageBase
{
	public uint Identifier { get; }

	public LeaveGhostRoleRaffleMessage(uint identifier)
	{
		Identifier = identifier;
	}
}
