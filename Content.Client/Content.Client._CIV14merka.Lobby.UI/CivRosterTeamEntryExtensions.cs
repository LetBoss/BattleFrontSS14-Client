using System.Linq;
using Content.Shared._CIV14merka;

namespace Content.Client._CIV14merka.Lobby.UI;

internal static class CivRosterTeamEntryExtensions
{
	public static CivRosterSquadEntry? SelectedSquadIfPresent(this CivRosterTeamEntry team, int? selectedSquadId)
	{
		if (selectedSquadId.HasValue)
		{
			int id = selectedSquadId.GetValueOrDefault();
			return team.Squads.FirstOrDefault((CivRosterSquadEntry s) => s.SquadId == id);
		}
		return null;
	}
}
