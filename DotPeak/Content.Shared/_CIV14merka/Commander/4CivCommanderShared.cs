// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Commander.CivCommanderSquadState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._CIV14merka.Commander;

[NetSerializable]
[Serializable]
public sealed class CivCommanderSquadState
{
  public int TeamId { get; }

  public int SquadId { get; }

  public bool IsArtillery { get; }

  public CivCommanderOrderType Order { get; }

  public string LeaderName { get; }

  public List<CivCommanderPlayerState> Members { get; }

  public CivCommanderSquadState(
    int teamId,
    int squadId,
    bool isArtillery,
    CivCommanderOrderType order,
    string leaderName,
    IEnumerable<CivCommanderPlayerState> members)
  {
    this.TeamId = teamId;
    this.SquadId = squadId;
    this.IsArtillery = isArtillery;
    this.Order = order;
    this.LeaderName = leaderName;
    this.Members = members.ToList<CivCommanderPlayerState>();
  }
}
