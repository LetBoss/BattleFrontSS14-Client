// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.Logs.AdminLogsEuiMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Administration.Logs;

public static class AdminLogsEuiMsg
{
  [NetSerializable]
  [Serializable]
  public sealed class SetLogFilter : EuiMessageBase
  {
    public SetLogFilter(string? search = null, bool invertTypes = false, HashSet<LogType>? types = null)
    {
      this.Search = search;
      this.InvertTypes = invertTypes;
      this.Types = types;
    }

    public string? Search { get; set; }

    public bool InvertTypes { get; set; }

    public HashSet<LogType>? Types { get; set; }
  }

  [NetSerializable]
  [Serializable]
  public sealed class NewLogs : EuiMessageBase
  {
    public NewLogs(List<SharedAdminLog> logs, bool replace, bool hasNext)
    {
      this.Logs = logs;
      this.Replace = replace;
      this.HasNext = hasNext;
    }

    public List<SharedAdminLog> Logs { get; set; }

    public bool Replace { get; set; }

    public bool HasNext { get; set; }
  }

  [NetSerializable]
  [Serializable]
  public sealed class LogsRequest : EuiMessageBase
  {
    public LogsRequest(
      int? roundId,
      string? search,
      HashSet<LogType>? types,
      HashSet<LogImpact>? impacts,
      DateTime? before,
      DateTime? after,
      bool includePlayers,
      Guid[]? anyPlayers,
      Guid[]? allPlayers,
      bool includeNonPlayers,
      DateOrder dateOrder)
    {
      this.RoundId = roundId;
      this.Search = search;
      this.Types = types;
      this.Impacts = impacts;
      this.Before = before;
      this.After = after;
      this.IncludePlayers = includePlayers;
      this.AnyPlayers = anyPlayers == null || anyPlayers.Length <= 0 ? (Guid[]) null : anyPlayers;
      this.AllPlayers = allPlayers == null || allPlayers.Length <= 0 ? (Guid[]) null : allPlayers;
      this.IncludeNonPlayers = includeNonPlayers;
      this.DateOrder = dateOrder;
    }

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
  }

  [NetSerializable]
  [Serializable]
  public sealed class NextLogsRequest : EuiMessageBase
  {
  }
}
