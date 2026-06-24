// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.CivRosterPlayerEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Teams;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._CIV14merka;

[NetSerializable]
[Serializable]
public sealed class CivRosterPlayerEntry
{
  public NetUserId UserId { get; }

  public string Name { get; }

  public int? TeamId { get; }

  public int? SquadId { get; }

  public bool IsLeader { get; }

  public bool IsSelected { get; }

  public CivRosterPlayerState State { get; }

  public CivTdmClass Class { get; }

  public CivRosterPlayerEntry(
    NetUserId userId,
    string name,
    int? teamId,
    int? squadId,
    bool isLeader,
    bool isSelected,
    CivRosterPlayerState state,
    CivTdmClass selectedClass)
  {
    this.UserId = userId;
    this.Name = name;
    this.TeamId = teamId;
    this.SquadId = squadId;
    this.IsLeader = isLeader;
    this.IsSelected = isSelected;
    this.State = state;
    this.Class = selectedClass;
  }
}
