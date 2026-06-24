using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.MiniGames;

[Serializable]
[NetSerializable]
public sealed class MiniGameLobbySetLockMessage : EntityEventArgs
{
	public int LobbyId { get; }

	public bool IsLocked { get; }

	public string Password { get; }

	public bool UpdatePassword { get; }

	public MiniGameLobbySetLockMessage(int lobbyId, bool isLocked, string password, bool updatePassword)
	{
		LobbyId = lobbyId;
		IsLocked = isLocked;
		Password = password;
		UpdatePassword = updatePassword;
	}
}
