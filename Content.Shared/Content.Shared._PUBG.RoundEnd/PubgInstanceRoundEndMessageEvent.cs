using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.RoundEnd;

[Serializable]
[NetSerializable]
public sealed class PubgInstanceRoundEndMessageEvent : EntityEventArgs
{
	public int GameInstanceId { get; }

	public string Title { get; }

	public string RoundEndText { get; }

	public List<PubgRoundEndPartyEntry> PartyEntries { get; }

	public PubgInstanceRoundEndMessageEvent(int gameInstanceId, string title, string roundEndText, List<PubgRoundEndPartyEntry>? partyEntries = null)
	{
		GameInstanceId = gameInstanceId;
		Title = title;
		RoundEndText = roundEndText;
		PartyEntries = partyEntries ?? new List<PubgRoundEndPartyEntry>();
	}
}
