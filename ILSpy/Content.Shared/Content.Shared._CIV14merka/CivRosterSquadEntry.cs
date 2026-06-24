using System;
using System.Collections.Generic;
using Content.Shared._CIV14merka.Teams;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
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

	public CivRosterSquadEntry(int teamId, int squadId, CivSquadType type, bool isOpen, int memberCount, int maxMembers, NetUserId? leaderId, bool isSelected, bool isMember, bool canJoin, string? joinUnavailableReason, bool canLeave, string? leaveUnavailableReason, bool canManage, string leaderName, List<CivRosterPlayerEntry> members, List<NetUserId> pendingInviteTargets, Dictionary<CivTdmClass, (int Available, int Total)> roleTickets, string? squadName = null, bool canRename = false)
	{
		TeamId = teamId;
		SquadId = squadId;
		Type = type;
		IsOpen = isOpen;
		MemberCount = memberCount;
		MaxMembers = maxMembers;
		LeaderId = leaderId;
		IsSelected = isSelected;
		IsMember = isMember;
		CanJoin = canJoin;
		JoinUnavailableReason = joinUnavailableReason;
		CanLeave = canLeave;
		LeaveUnavailableReason = leaveUnavailableReason;
		CanManage = canManage;
		LeaderName = leaderName;
		Members = members;
		PendingInviteTargets = pendingInviteTargets;
		RoleTickets = roleTickets;
		SquadName = squadName;
		CanRename = canRename;
	}
}
