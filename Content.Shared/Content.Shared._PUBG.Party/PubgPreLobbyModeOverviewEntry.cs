using System;
using Content.Shared._PUBG.Match;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public sealed class PubgPreLobbyModeOverviewEntry
{
	public PubgMatchMode Mode { get; }

	public int InGamePlayers { get; }

	public int InLobbyPlayers { get; }

	public int? NextStartSeconds { get; }

	public PubgPreLobbyModeOverviewEntry(PubgMatchMode mode, int inGamePlayers, int inLobbyPlayers, int? nextStartSeconds)
	{
		Mode = mode;
		InGamePlayers = inGamePlayers;
		InLobbyPlayers = inLobbyPlayers;
		NextStartSeconds = nextStartSeconds;
	}
}
