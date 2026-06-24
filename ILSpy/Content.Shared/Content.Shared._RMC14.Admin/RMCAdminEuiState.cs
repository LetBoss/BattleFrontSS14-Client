using System;
using System.Collections.Generic;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared.Eui;
using Content.Shared.NPC.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Admin;

[Serializable]
[NetSerializable]
[Virtual]
public class RMCAdminEuiState(List<Hive> hives, List<Squad> squads, List<Xeno> xenos, int marines, List<(Guid Id, string Actor, int Round)> tacticalMapHistory, (Guid Id, List<TacticalMapLine> Lines, string Actor, int RoundId) tacticalMapLines, Dictionary<string, FactionData> factions) : EuiStateBase
{
	public readonly List<Hive> Hives = hives;

	public readonly List<Squad> Squads = squads;

	public readonly List<Xeno> Xenos = xenos;

	public readonly int Marines = marines;

	public readonly List<(Guid Id, string Actor, int Round)> TacticalMapHistory = tacticalMapHistory;

	public readonly (Guid Id, List<TacticalMapLine> Lines, string Actor, int RoundId) TacticalMapLines = tacticalMapLines;

	public readonly Dictionary<string, FactionData> Factions = factions;
}
