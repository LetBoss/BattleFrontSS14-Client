using System;
using Content.Shared._CIV14merka.Stats;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRoundEndPlayerEntry
{
	public string PlayerName { get; }

	public string RoleText { get; }

	public string SquadText { get; }

	public string StatusText { get; }

	public bool IsCommander { get; }

	public CivPlayerRoundStats? Stats { get; }

	public CivRoundEndPlayerEntry(string playerName, string roleText, string squadText, string statusText, bool isCommander, CivPlayerRoundStats? stats = null)
	{
		PlayerName = playerName;
		RoleText = roleText;
		SquadText = squadText;
		StatusText = statusText;
		IsCommander = isCommander;
		Stats = stats;
	}
}
