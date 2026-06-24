using System;
using System.Collections.Generic;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Logs;

public static class AdminLogsEuiMsg
{
	[Serializable]
	[NetSerializable]
	public sealed class SetLogFilter : EuiMessageBase
	{
		public string? Search { get; set; }

		public bool InvertTypes { get; set; }

		public HashSet<LogType>? Types { get; set; }

		public SetLogFilter(string? search = null, bool invertTypes = false, HashSet<LogType>? types = null)
		{
			Search = search;
			InvertTypes = invertTypes;
			Types = types;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class NewLogs : EuiMessageBase
	{
		public List<SharedAdminLog> Logs { get; set; }

		public bool Replace { get; set; }

		public bool HasNext { get; set; }

		public NewLogs(List<SharedAdminLog> logs, bool replace, bool hasNext)
		{
			Logs = logs;
			Replace = replace;
			HasNext = hasNext;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class LogsRequest : EuiMessageBase
	{
		public int? RoundId { get; set; }

		public string? Search { get; set; }

		public HashSet<LogType>? Types { get; set; }

		public HashSet<LogImpact>? Impacts { get; set; }

		public DateTime? Before { get; set; }

		public DateTime? After { get; set; }

		public bool IncludePlayers { get; set; }

		public Guid[]? AnyPlayers { get; set; }

		public Guid[]? AllPlayers { get; set; }

		public bool IncludeNonPlayers { get; set; }

		public DateOrder DateOrder { get; set; }

		public LogsRequest(int? roundId, string? search, HashSet<LogType>? types, HashSet<LogImpact>? impacts, DateTime? before, DateTime? after, bool includePlayers, Guid[]? anyPlayers, Guid[]? allPlayers, bool includeNonPlayers, DateOrder dateOrder)
		{
			RoundId = roundId;
			Search = search;
			Types = types;
			Impacts = impacts;
			Before = before;
			After = after;
			IncludePlayers = includePlayers;
			AnyPlayers = ((anyPlayers != null && anyPlayers.Length > 0) ? anyPlayers : null);
			AllPlayers = ((allPlayers != null && allPlayers.Length > 0) ? allPlayers : null);
			IncludeNonPlayers = includeNonPlayers;
			DateOrder = dateOrder;
		}
	}

	[Serializable]
	[NetSerializable]
	public sealed class NextLogsRequest : EuiMessageBase
	{
	}
}
