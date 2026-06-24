using System;
using System.Collections.Generic;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Logs;

[Serializable]
[NetSerializable]
public sealed class AdminLogsEuiState : EuiStateBase
{
	public bool IsLoading { get; set; }

	public int RoundId { get; }

	public Dictionary<Guid, string> Players { get; }

	public int RoundLogs { get; }

	public AdminLogsEuiState(int roundId, Dictionary<Guid, string> players, int roundLogs)
	{
		RoundId = roundId;
		Players = players;
		RoundLogs = roundLogs;
	}
}
