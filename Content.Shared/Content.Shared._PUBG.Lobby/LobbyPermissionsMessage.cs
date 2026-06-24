using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Lobby;

[Serializable]
[NetSerializable]
public sealed class LobbyPermissionsMessage : EntityEventArgs
{
	public Dictionary<string, int> Permissions { get; }

	public LobbyPermissionsMessage(Dictionary<string, int> permissions)
	{
		Permissions = permissions;
	}
}
