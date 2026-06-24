using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbyKickMessage : EntityEventArgs
{
	public int LobbyId { get; }

	public NetUserId TargetUserId { get; }

	public MiniGameLobbyKickMessage(int lobbyId, NetUserId targetUserId)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		LobbyId = lobbyId;
		TargetUserId = targetUserId;
	}
}
