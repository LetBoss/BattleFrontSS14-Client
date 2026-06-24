using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public sealed class PubgPreLobbyModeOverviewEvent : EntityEventArgs
{
	public List<PubgPreLobbyModeOverviewEntry> Entries { get; }

	public PubgPreLobbyModeOverviewEvent(List<PubgPreLobbyModeOverviewEntry> entries)
	{
		Entries = entries;
	}
}
