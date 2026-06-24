using System;
using Content.Shared._PUBG.Match;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Lobby;

[Serializable]
[NetSerializable]
public sealed class LobbyJoinModeMessage : EntityEventArgs
{
	public PubgMatchMode MatchMode { get; }

	public bool PreferFullSquad { get; }

	public LobbyJoinModeMessage(PubgMatchMode matchMode, bool preferFullSquad)
	{
		MatchMode = matchMode;
		PreferFullSquad = preferFullSquad;
	}
}
