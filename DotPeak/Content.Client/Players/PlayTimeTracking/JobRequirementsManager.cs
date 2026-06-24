// Decompiled with JetBrains decompiler
// Type: Content.Client.Players.PlayTimeTracking.JobRequirementsManager
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.PlayTimeTracking;
using Content.Shared.CCVar;
using Content.Shared.Localizations;
using Content.Shared.Players;
using Content.Shared.Players.JobWhitelist;
using Content.Shared.Players.PlayTimeTracking;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Robust.Client;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Client.Players.PlayTimeTracking;

public sealed class JobRequirementsManager : ISharedPlaytimeManager
{
  [Dependency]
  private IBaseClient _client;
  [Dependency]
  private IClientNetManager _net;
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IEntityManager _entManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private RMCPlayTimeManager _rmcPlayTime;
  private readonly Dictionary<string, TimeSpan> _roles = new Dictionary<string, TimeSpan>();
  private readonly List<string> _roleBans = new List<string>();
  private readonly List<string> _jobWhitelists = new List<string>();
  private ISawmill _sawmill;

  public event Action? Updated;

  public void Initialize()
  {
    this._sawmill = Logger.GetSawmill("job_requirements");
    // ISSUE: method pointer
    ((INetManager) this._net).RegisterNetMessage<MsgRoleBans>(new ProcessMessage<MsgRoleBans>((object) this, __methodptr(RxRoleBans)), (NetMessageAccept) 3);
    // ISSUE: method pointer
    ((INetManager) this._net).RegisterNetMessage<MsgPlayTime>(new ProcessMessage<MsgPlayTime>((object) this, __methodptr(RxPlayTime)), (NetMessageAccept) 3);
    // ISSUE: method pointer
    ((INetManager) this._net).RegisterNetMessage<MsgJobWhitelist>(new ProcessMessage<MsgJobWhitelist>((object) this, __methodptr(RxJobWhitelist)), (NetMessageAccept) 3);
    this._client.RunLevelChanged += new EventHandler<RunLevelChangedEventArgs>(this.ClientOnRunLevelChanged);
    this._rmcPlayTime.Updated += (Action) (() =>
    {
      Action updated = this.Updated;
      if (updated == null)
        return;
      updated();
    });
  }

  private void ClientOnRunLevelChanged(object? sender, RunLevelChangedEventArgs e)
  {
    if (e.NewLevel != 1)
      return;
    this._roles.Clear();
    this._jobWhitelists.Clear();
    this._roleBans.Clear();
  }

  private void RxRoleBans(MsgRoleBans message)
  {
    this._sawmill.Debug($"Received roleban info containing {message.Bans.Count} entries.");
    this._roleBans.Clear();
    this._roleBans.AddRange((IEnumerable<string>) message.Bans);
    Action updated = this.Updated;
    if (updated == null)
      return;
    updated();
  }

  private void RxPlayTime(MsgPlayTime message)
  {
    this._roles.Clear();
    foreach (KeyValuePair<string, TimeSpan> tracker in message.Trackers)
    {
      string key;
      (key, this._roles[key]) = tracker;
    }
    Action updated = this.Updated;
    if (updated == null)
      return;
    updated();
  }

  private void RxJobWhitelist(MsgJobWhitelist message)
  {
    this._jobWhitelists.Clear();
    this._jobWhitelists.AddRange((IEnumerable<string>) message.Whitelist);
    Action updated = this.Updated;
    if (updated == null)
      return;
    updated();
  }

  private bool IsWhitelistedInternal(string jobId)
  {
    if (this._jobWhitelists.Contains(jobId))
      return true;
    JobPrototype jobPrototype;
    if (!this._prototypes.TryIndex<JobPrototype>(jobId, ref jobPrototype))
    {
      this._sawmill.Error($"Failed to index job prototype {jobId} during whitelist check. Assuming not whitelisted");
      return false;
    }
    return jobPrototype.WhitelistParent.HasValue && this.IsWhitelistedInternal(jobPrototype.WhitelistParent.Value.Id);
  }

  public bool IsAllowed(
    JobPrototype job,
    HumanoidCharacterProfile? profile,
    [NotNullWhen(false)] out FormattedMessage? reason)
  {
    reason = (FormattedMessage) null;
    if (this._roleBans.Contains("Job:" + job.ID))
    {
      reason = FormattedMessage.FromUnformatted(Loc.GetString("role-ban"));
      return false;
    }
    if (!this.CheckWhitelist(job, out reason))
      return false;
    return ((ISharedPlayerManager) this._playerManager).LocalSession == null || this.CheckRoleRequirements(job, profile, out reason);
  }

  public bool CheckRoleRequirements(
    JobPrototype job,
    HumanoidCharacterProfile? profile,
    [NotNullWhen(false)] out FormattedMessage? reason)
  {
    reason = (FormattedMessage) null;
    return this._rmcPlayTime.IsExcluded(job.ID) || this.CheckRoleRequirements(this._entManager.System<SharedRoleSystem>().GetJobRequirement(job), profile, out reason);
  }

  public bool CheckRoleRequirements(
    HashSet<JobRequirement>? requirements,
    HumanoidCharacterProfile? profile,
    [NotNullWhen(false)] out FormattedMessage? reason)
  {
    reason = (FormattedMessage) null;
    if (requirements == null || !this._cfg.GetCVar<bool>(CCVars.GameRoleTimers))
      return true;
    List<string> values = new List<string>();
    foreach (JobRequirement requirement in requirements)
    {
      FormattedMessage reason1;
      if (!requirement.Check(this._entManager, this._prototypes, profile, (IReadOnlyDictionary<string, TimeSpan>) this._roles, out reason1))
        values.Add(reason1.ToMarkup());
    }
    reason = values.Count == 0 ? (FormattedMessage) null : FormattedMessage.FromMarkupOrThrow(string.Join<string>('\n', (IEnumerable<string>) values));
    return reason == null;
  }

  public bool CheckWhitelist(JobPrototype job, [NotNullWhen(false)] out FormattedMessage? reason)
  {
    reason = (FormattedMessage) null;
    if (!this._cfg.GetCVar<bool>(CCVars.GameRoleWhitelist) || !job.Whitelisted || this.IsWhitelistedInternal(job.ID))
      return true;
    reason = FormattedMessage.FromUnformatted(Loc.GetString("role-not-whitelisted"));
    return false;
  }

  public TimeSpan FetchOverallPlaytime()
  {
    TimeSpan timeSpan;
    return !this._roles.TryGetValue("Overall", out timeSpan) ? TimeSpan.Zero : timeSpan;
  }

  public IEnumerable<KeyValuePair<string, TimeSpan>> FetchPlaytimeByRoles()
  {
    SharedJobSystem jobSystem = this._entManager.System<SharedJobSystem>();
    HashSet<ProtoId<PlayTimeTrackerPrototype>> protoIdSet = new HashSet<ProtoId<PlayTimeTrackerPrototype>>();
    foreach (JobPrototype enumeratePrototype in this._prototypes.EnumeratePrototypes<JobPrototype>())
    {
      string playTimeTracker = enumeratePrototype.PlayTimeTracker;
      if (playTimeTracker != null)
        protoIdSet.Add(ProtoId<PlayTimeTrackerPrototype>.op_Implicit(playTimeTracker));
    }
    foreach (ProtoId<PlayTimeTrackerPrototype> trackerProto in protoIdSet)
    {
      List<string> list = new List<string>();
      PlayTimeTrackerPrototype trackerPrototype = this._prototypes.Index<PlayTimeTrackerPrototype>(trackerProto);
      if (trackerPrototype.ShowInStatsMenu)
      {
        List<ProtoId<JobPrototype>> jobPrototypes = jobSystem.GetJobPrototypes(trackerProto);
        LocId? name = trackerPrototype.Name;
        if (name.HasValue)
        {
          LocId valueOrDefault = name.GetValueOrDefault();
          list.Add(Loc.GetString(LocId.op_Implicit(valueOrDefault)));
        }
        else
        {
          foreach (ProtoId<JobPrototype> protoId in jobPrototypes)
          {
            JobPrototype jobPrototype = this._prototypes.Index<JobPrototype>(protoId);
            list.Add(jobPrototype.LocalizedName);
          }
        }
        TimeSpan timeSpan;
        if (this._roles.TryGetValue(ProtoId<PlayTimeTrackerPrototype>.op_Implicit(trackerProto), out timeSpan))
          yield return new KeyValuePair<string, TimeSpan>(ContentLocalizationManager.FormatList(list), timeSpan);
      }
    }
  }

  public IEnumerable<KeyValuePair<string, TimeSpan>> FetchPlaytimeJobIdByRoles()
  {
    JobPrototype[] array = this._prototypes.EnumeratePrototypes<JobPrototype>().ToArray<JobPrototype>();
    HashSet<ProtoId<PlayTimeTrackerPrototype>> protoIdSet = new HashSet<ProtoId<PlayTimeTrackerPrototype>>();
    HashSet<ProtoId<PlayTimeTrackerPrototype>> duplicateTrackers = new HashSet<ProtoId<PlayTimeTrackerPrototype>>();
    foreach (JobPrototype jobPrototype in array)
    {
      if (!protoIdSet.Add(ProtoId<PlayTimeTrackerPrototype>.op_Implicit(jobPrototype.PlayTimeTracker)))
        duplicateTrackers.Add(ProtoId<PlayTimeTrackerPrototype>.op_Implicit(jobPrototype.PlayTimeTracker));
    }
    JobPrototype[] jobPrototypeArray = array;
    for (int index = 0; index < jobPrototypeArray.Length; ++index)
    {
      JobPrototype jobPrototype = jobPrototypeArray[index];
      TimeSpan timeSpan;
      if ((!duplicateTrackers.Contains(ProtoId<PlayTimeTrackerPrototype>.op_Implicit(jobPrototype.PlayTimeTracker)) || jobPrototype.BasePlaytimeTracker) && this._roles.TryGetValue(jobPrototype.PlayTimeTracker, out timeSpan))
        yield return new KeyValuePair<string, TimeSpan>(jobPrototype.ID, timeSpan);
    }
    jobPrototypeArray = (JobPrototype[]) null;
  }

  public IReadOnlyDictionary<string, TimeSpan> GetPlayTimes(ICommonSession session)
  {
    return session != ((ISharedPlayerManager) this._playerManager).LocalSession ? (IReadOnlyDictionary<string, TimeSpan>) new Dictionary<string, TimeSpan>() : (IReadOnlyDictionary<string, TimeSpan>) this._roles;
  }
}
