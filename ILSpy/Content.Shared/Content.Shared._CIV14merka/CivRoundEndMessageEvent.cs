using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRoundEndMessageEvent : EntityEventArgs
{
	public string Title { get; }

	public CivRoundEndSummary Summary { get; }

	public List<CivRoundEndTeamEntry> TeamEntries { get; }

	public CivRoundEndMessageEvent(string title, CivRoundEndSummary summary, List<CivRoundEndTeamEntry>? teamEntries = null)
	{
		Title = title;
		Summary = summary;
		TeamEntries = teamEntries ?? new List<CivRoundEndTeamEntry>();
	}
}
