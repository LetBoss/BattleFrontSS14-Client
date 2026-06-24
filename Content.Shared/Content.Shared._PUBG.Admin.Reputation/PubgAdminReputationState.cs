using System;
using System.Collections.Generic;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Admin.Reputation;

[Serializable]
[NetSerializable]
public sealed class PubgAdminReputationState(string playerName, int currentReputation, bool canEdit, List<PubgReputationHistoryEntry> history) : EuiStateBase
{
	public readonly string PlayerName = playerName;

	public readonly int CurrentReputation = currentReputation;

	public readonly bool CanEdit = canEdit;

	public readonly List<PubgReputationHistoryEntry> History = history;
}
