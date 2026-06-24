using System;
using System.Collections.Generic;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared.NPC.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Admin;

[Serializable]
[NetSerializable]
public sealed class RMCAdminEuiTargetState(List<Hive> hives, List<Squad> squads, List<Xeno> xenos, int marines, List<(Guid Id, string Actor, int Round)> tacticalMapHistory, (Guid Id, List<TacticalMapLine> Lines, string Actor, int RoundId) tacticalMapLines, Dictionary<string, FactionData> factions, List<(string Name, bool Present)> specialistSkills, int points, Dictionary<string, int> extraPoints) : RMCAdminEuiState(hives, squads, xenos, marines, tacticalMapHistory, tacticalMapLines, factions)
{
	public readonly List<(string Name, bool Present)> SpecialistSkills = specialistSkills;

	public readonly int Points = points;

	public readonly Dictionary<string, int> ExtraPoints = extraPoints;
}
