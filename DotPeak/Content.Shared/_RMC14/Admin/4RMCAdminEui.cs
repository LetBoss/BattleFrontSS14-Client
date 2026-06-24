// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Admin.RMCAdminEuiTargetState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.TacticalMap;
using Content.Shared.NPC.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Admin;

[NetSerializable]
[Serializable]
public sealed class RMCAdminEuiTargetState(
  List<Hive> hives,
  List<Squad> squads,
  List<Xeno> xenos,
  int marines,
  List<(Guid Id, string Actor, int Round)> tacticalMapHistory,
  (Guid Id, List<TacticalMapLine> Lines, string Actor, int RoundId) tacticalMapLines,
  Dictionary<string, FactionData> factions,
  List<(string Name, bool Present)> specialistSkills,
  int points,
  Dictionary<string, int> extraPoints) : RMCAdminEuiState(hives, squads, xenos, marines, tacticalMapHistory, tacticalMapLines, factions)
{
  public readonly List<(string Name, bool Present)> SpecialistSkills = specialistSkills;
  public readonly int Points = points;
  public readonly Dictionary<string, int> ExtraPoints = extraPoints;
}
