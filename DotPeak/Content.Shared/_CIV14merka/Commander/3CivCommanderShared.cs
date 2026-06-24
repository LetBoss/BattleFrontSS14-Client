// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Commander.CivCommanderPlayerState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._CIV14merka.Commander;

[NetSerializable]
[Serializable]
public sealed class CivCommanderPlayerState
{
  public NetUserId UserId { get; }

  public string Name { get; }

  public int TeamId { get; }

  public int SquadId { get; }

  public bool IsSquadLeader { get; }

  public bool IsCommander { get; }

  public bool IsSelf { get; }

  public CivCommanderPlayerState(
    NetUserId userId,
    string name,
    int teamId,
    int squadId,
    bool isSquadLeader,
    bool isCommander,
    bool isSelf)
  {
    this.UserId = userId;
    this.Name = name;
    this.TeamId = teamId;
    this.SquadId = squadId;
    this.IsSquadLeader = isSquadLeader;
    this.IsCommander = isCommander;
    this.IsSelf = isSelf;
  }
}
