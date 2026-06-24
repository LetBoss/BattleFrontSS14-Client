using System;
using System.Collections.Generic;
using Content.Shared._PUBG.Match;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Party;

[Serializable]
[NetSerializable]
public sealed class PubgPreLobbyPartyStateEvent : EntityEventArgs
{
	public List<PubgPreLobbyPartyMemberState> Members { get; }

	public NetUserId LeaderId { get; }

	public PubgMatchMode? SelectedMode { get; }

	public bool PreferFullSquad { get; }

	public PubgPreLobbyPartyStateEvent(List<PubgPreLobbyPartyMemberState> members, NetUserId leaderId, PubgMatchMode? selectedMode, bool preferFullSquad)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Members = members;
		LeaderId = leaderId;
		SelectedMode = selectedMode;
		PreferFullSquad = preferFullSquad;
	}
}
