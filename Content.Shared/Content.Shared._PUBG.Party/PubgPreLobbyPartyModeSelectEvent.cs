using System;
using Content.Shared._PUBG.Match;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public sealed class PubgPreLobbyPartyModeSelectEvent : EntityEventArgs
{
	public PubgMatchMode? SelectedMode { get; }

	public bool PreferFullSquad { get; }

	public PubgPreLobbyPartyModeSelectEvent(PubgMatchMode? selectedMode, bool preferFullSquad)
	{
		SelectedMode = selectedMode;
		PreferFullSquad = preferFullSquad;
	}
}
