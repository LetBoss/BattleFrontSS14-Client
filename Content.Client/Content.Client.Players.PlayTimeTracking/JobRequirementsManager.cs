using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
		_sawmill = Logger.GetSawmill("job_requirements");
		((INetManager)_net).RegisterNetMessage<MsgRoleBans>((ProcessMessage<MsgRoleBans>)RxRoleBans, (NetMessageAccept)3);
		((INetManager)_net).RegisterNetMessage<MsgPlayTime>((ProcessMessage<MsgPlayTime>)RxPlayTime, (NetMessageAccept)3);
		((INetManager)_net).RegisterNetMessage<MsgJobWhitelist>((ProcessMessage<MsgJobWhitelist>)RxJobWhitelist, (NetMessageAccept)3);
		_client.RunLevelChanged += ClientOnRunLevelChanged;
		_rmcPlayTime.Updated += delegate
		{
			this.Updated?.Invoke();
		};
	}

	private void ClientOnRunLevelChanged(object? sender, RunLevelChangedEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		if ((int)e.NewLevel == 1)
		{
			_roles.Clear();
			_jobWhitelists.Clear();
			_roleBans.Clear();
		}
	}

	private void RxRoleBans(MsgRoleBans message)
	{
		_sawmill.Debug($"Received roleban info containing {message.Bans.Count} entries.");
		_roleBans.Clear();
		_roleBans.AddRange(message.Bans);
		this.Updated?.Invoke();
	}

	private void RxPlayTime(MsgPlayTime message)
	{
		_roles.Clear();
		foreach (var (key, value) in message.Trackers)
		{
			_roles[key] = value;
		}
		this.Updated?.Invoke();
	}

	private void RxJobWhitelist(MsgJobWhitelist message)
	{
		_jobWhitelists.Clear();
		_jobWhitelists.AddRange(message.Whitelist);
		this.Updated?.Invoke();
	}

	private bool IsWhitelistedInternal(string jobId)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (_jobWhitelists.Contains(jobId))
		{
			return true;
		}
		JobPrototype jobPrototype = default(JobPrototype);
		if (!_prototypes.TryIndex<JobPrototype>(jobId, ref jobPrototype))
		{
			_sawmill.Error("Failed to index job prototype " + jobId + " during whitelist check. Assuming not whitelisted");
			return false;
		}
		if (jobPrototype.WhitelistParent.HasValue)
		{
			return IsWhitelistedInternal(jobPrototype.WhitelistParent.Value.Id);
		}
		return false;
	}

	public bool IsAllowed(JobPrototype job, HumanoidCharacterProfile? profile, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		reason = null;
		if (_roleBans.Contains("Job:" + job.ID))
		{
			reason = FormattedMessage.FromUnformatted(Loc.GetString("role-ban"));
			return false;
		}
		if (!CheckWhitelist(job, out reason))
		{
			return false;
		}
		if (((ISharedPlayerManager)_playerManager).LocalSession == null)
		{
			return true;
		}
		return CheckRoleRequirements(job, profile, out reason);
	}

	public bool CheckRoleRequirements(JobPrototype job, HumanoidCharacterProfile? profile, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		reason = null;
		if (_rmcPlayTime.IsExcluded(job.ID))
		{
			return true;
		}
		HashSet<JobRequirement> jobRequirement = _entManager.System<SharedRoleSystem>().GetJobRequirement(job);
		return CheckRoleRequirements(jobRequirement, profile, out reason);
	}

	public bool CheckRoleRequirements(HashSet<JobRequirement>? requirements, HumanoidCharacterProfile? profile, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		reason = null;
		if (requirements == null || !_cfg.GetCVar<bool>(CCVars.GameRoleTimers))
		{
			return true;
		}
		List<string> list = new List<string>();
		foreach (JobRequirement requirement in requirements)
		{
			if (!requirement.Check(_entManager, _prototypes, profile, _roles, out FormattedMessage reason2))
			{
				list.Add(reason2.ToMarkup());
			}
		}
		reason = ((list.Count == 0) ? null : FormattedMessage.FromMarkupOrThrow(string.Join('\n', list)));
		return reason == null;
	}

	public bool CheckWhitelist(JobPrototype job, [NotNullWhen(false)] out FormattedMessage? reason)
	{
		reason = null;
		if (!_cfg.GetCVar<bool>(CCVars.GameRoleWhitelist))
		{
			return true;
		}
		if (job.Whitelisted)
		{
			if (IsWhitelistedInternal(job.ID))
			{
				return true;
			}
			reason = FormattedMessage.FromUnformatted(Loc.GetString("role-not-whitelisted"));
			return false;
		}
		return true;
	}

	public TimeSpan FetchOverallPlaytime()
	{
		if (!_roles.TryGetValue("Overall", out var value))
		{
			return TimeSpan.Zero;
		}
		return value;
	}

	public IEnumerable<KeyValuePair<string, TimeSpan>> FetchPlaytimeByRoles()
	{
		SharedJobSystem jobSystem = _entManager.System<SharedJobSystem>();
		HashSet<ProtoId<PlayTimeTrackerPrototype>> hashSet = new HashSet<ProtoId<PlayTimeTrackerPrototype>>();
		foreach (JobPrototype item in _prototypes.EnumeratePrototypes<JobPrototype>())
		{
			string playTimeTracker = item.PlayTimeTracker;
			if (playTimeTracker != null)
			{
				hashSet.Add(ProtoId<PlayTimeTrackerPrototype>.op_Implicit(playTimeTracker));
			}
		}
		foreach (ProtoId<PlayTimeTrackerPrototype> item2 in hashSet)
		{
			List<string> list = new List<string>();
			PlayTimeTrackerPrototype playTimeTrackerPrototype = _prototypes.Index<PlayTimeTrackerPrototype>(item2);
			if (!playTimeTrackerPrototype.ShowInStatsMenu)
			{
				continue;
			}
			List<ProtoId<JobPrototype>> jobPrototypes = jobSystem.GetJobPrototypes(item2);
			LocId? name = playTimeTrackerPrototype.Name;
			if (name.HasValue)
			{
				LocId valueOrDefault = name.GetValueOrDefault();
				list.Add(Loc.GetString(LocId.op_Implicit(valueOrDefault)));
			}
			else
			{
				foreach (ProtoId<JobPrototype> item3 in jobPrototypes)
				{
					JobPrototype jobPrototype = _prototypes.Index<JobPrototype>(item3);
					list.Add(jobPrototype.LocalizedName);
				}
			}
			if (_roles.TryGetValue(ProtoId<PlayTimeTrackerPrototype>.op_Implicit(item2), out var value))
			{
				string key = ContentLocalizationManager.FormatList(list);
				yield return new KeyValuePair<string, TimeSpan>(key, value);
			}
		}
	}

	public IEnumerable<KeyValuePair<string, TimeSpan>> FetchPlaytimeJobIdByRoles()
	{
		JobPrototype[] array = _prototypes.EnumeratePrototypes<JobPrototype>().ToArray();
		HashSet<ProtoId<PlayTimeTrackerPrototype>> hashSet = new HashSet<ProtoId<PlayTimeTrackerPrototype>>();
		HashSet<ProtoId<PlayTimeTrackerPrototype>> duplicateTrackers = new HashSet<ProtoId<PlayTimeTrackerPrototype>>();
		JobPrototype[] array2 = array;
		foreach (JobPrototype jobPrototype in array2)
		{
			if (!hashSet.Add(ProtoId<PlayTimeTrackerPrototype>.op_Implicit(jobPrototype.PlayTimeTracker)))
			{
				duplicateTrackers.Add(ProtoId<PlayTimeTrackerPrototype>.op_Implicit(jobPrototype.PlayTimeTracker));
			}
		}
		JobPrototype[] array3 = array;
		foreach (JobPrototype jobPrototype2 in array3)
		{
			if ((!duplicateTrackers.Contains(ProtoId<PlayTimeTrackerPrototype>.op_Implicit(jobPrototype2.PlayTimeTracker)) || jobPrototype2.BasePlaytimeTracker) && _roles.TryGetValue(jobPrototype2.PlayTimeTracker, out var value))
			{
				yield return new KeyValuePair<string, TimeSpan>(jobPrototype2.ID, value);
			}
		}
	}

	public IReadOnlyDictionary<string, TimeSpan> GetPlayTimes(ICommonSession session)
	{
		if (session != ((ISharedPlayerManager)_playerManager).LocalSession)
		{
			return new Dictionary<string, TimeSpan>();
		}
		return _roles;
	}
}
