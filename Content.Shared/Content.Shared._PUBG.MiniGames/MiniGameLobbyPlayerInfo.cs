using System;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbyPlayerInfo
{
	public NetUserId UserId { get; }

	public string Name { get; }

	public bool IsLeader { get; }

	public MiniGameLobbyPlayerInfo(NetUserId userId, string name, bool isLeader)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UserId = userId;
		Name = name;
		IsLeader = isLeader;
	}
}
