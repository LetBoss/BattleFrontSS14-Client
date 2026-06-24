// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.CivRosterSquadEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Teams;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._CIV14merka;

[NetSerializable]
[Serializable]
public sealed class CivRosterSquadEntry
{
  public int TeamId { get; }

  public int SquadId { get; }

  public CivSquadType Type { get; }

  public bool IsOpen { get; }

  public int MemberCount { get; }

  public int MaxMembers { get; }

  public NetUserId? LeaderId { get; }

  public bool IsSelected { get; }

  public bool IsMember { get; }

  public bool CanJoin { get; }

  public string? JoinUnavailableReason { get; }

  public bool CanLeave { get; }

  public string? LeaveUnavailableReason { get; }

  public bool CanManage { get; }

  public string LeaderName { get; }

  public List<CivRosterPlayerEntry> Members { get; }

  public List<NetUserId> PendingInviteTargets { get; }

  public Dictionary<CivTdmClass, (int Available, int Total)> RoleTickets { get; }

  public string? SquadName { get; }

  public bool CanRename { get; }

  public CivRosterSquadEntry(
    int teamId,
    int squadId,
    CivSquadType type,
    bool isOpen,
    int memberCount,
    int maxMembers,
    NetUserId? leaderId,
    bool isSelected,
    bool isMember,
    bool canJoin,
    string? joinUnavailableReason,
    bool canLeave,
    string? leaveUnavailableReason,
    bool canManage,
    string leaderName,
    List<CivRosterPlayerEntry> members,
    List<NetUserId> pendingInviteTargets,
    Dictionary<CivTdmClass, (int Available, int Total)> roleTickets,
    string? squadName = null,
    bool canRename = false)
  {
    this.TeamId = teamId;
    this.SquadId = squadId;
    this.Type = type;
    this.IsOpen = isOpen;
    this.MemberCount = memberCount;
    this.MaxMembers = maxMembers;
    this.LeaderId = leaderId;
    this.IsSelected = isSelected;
    this.IsMember = isMember;
    this.CanJoin = canJoin;
    this.JoinUnavailableReason = joinUnavailableReason;
    this.CanLeave = canLeave;
    this.LeaveUnavailableReason = leaveUnavailableReason;
    this.CanManage = canManage;
    this.LeaderName = leaderName;
    this.Members = members;
    this.PendingInviteTargets = pendingInviteTargets;
    this.RoleTickets = roleTickets;
    this.SquadName = squadName;
    this.CanRename = canRename;
  }
}
