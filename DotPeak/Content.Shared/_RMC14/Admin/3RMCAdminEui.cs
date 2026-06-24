// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Admin.RMCAdminEuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.TacticalMap;
using Content.Shared.Eui;
using Content.Shared.NPC.Prototypes;
using Robust.Shared.Analyzers;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Admin;

[NetSerializable]
[Virtual]
[Serializable]
public class RMCAdminEuiState(
  List<Hive> hives,
  List<Squad> squads,
  List<Xeno> xenos,
  int marines,
  List<(Guid Id, string Actor, int Round)> tacticalMapHistory,
  (Guid Id, List<TacticalMapLine> Lines, string Actor, int RoundId) tacticalMapLines,
  Dictionary<string, FactionData> factions) : EuiStateBase
{
  public readonly List<Hive> Hives = hives;
  public readonly List<Squad> Squads = squads;
  public readonly List<Xeno> Xenos = xenos;
  public readonly int Marines = marines;
  public readonly List<(Guid Id, string Actor, int Round)> TacticalMapHistory = tacticalMapHistory;
  public readonly (Guid Id, List<TacticalMapLine> Lines, string Actor, int RoundId) TacticalMapLines = tacticalMapLines;
  public readonly Dictionary<string, FactionData> Factions = factions;
}
