using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles;

[Serializable]
[NetSerializable]
public sealed class FollowGhostRoleMessage : EuiMessageBase
{
	public uint Identifier { get; }

	public FollowGhostRoleMessage(uint identifier)
	{
		Identifier = identifier;
	}
}
