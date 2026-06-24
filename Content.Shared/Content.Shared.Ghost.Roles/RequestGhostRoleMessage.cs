using System;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles;

[Serializable]
[NetSerializable]
public sealed class RequestGhostRoleMessage : EuiMessageBase
{
	public uint Identifier { get; }

	public RequestGhostRoleMessage(uint identifier)
	{
		Identifier = identifier;
	}
}
