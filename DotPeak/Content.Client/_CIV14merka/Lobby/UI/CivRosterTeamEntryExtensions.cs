// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Lobby.UI.CivRosterTeamEntryExtensions
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka;
using System;
using System.Linq;

#nullable enable
namespace Content.Client._CIV14merka.Lobby.UI;

internal static class CivRosterTeamEntryExtensions
{
  public static CivRosterSquadEntry? SelectedSquadIfPresent(
    this CivRosterTeamEntry team,
    int? selectedSquadId)
  {
    if (!selectedSquadId.HasValue)
      return (CivRosterSquadEntry) null;
    int id = selectedSquadId.GetValueOrDefault();
    return team.Squads.FirstOrDefault<CivRosterSquadEntry>((Func<CivRosterSquadEntry, bool>) (s => s.SquadId == id));
  }
}
