using System;
using Content.Shared.CCVar;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Client.Playtime;

public sealed class ClientsidePlaytimeTrackingManager
{
	[Dependency]
	private IClientNetManager _clientNetManager;

	[Dependency]
	private IConfigurationManager _configurationManager;

	[Dependency]
	private ILogManager _logManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IGameTiming _gameTiming;

	private ISawmill _sawmill;

	private const string InternalDateFormat = "yyyy-MM-dd";

	[ViewVariables]
	private TimeSpan? _mobAttachmentTime;

	[ViewVariables]
	public float PlaytimeMinutesToday
	{
		get
		{
			float cVar = _configurationManager.GetCVar<float>(CCVars.PlaytimeMinutesToday);
			if (!_mobAttachmentTime.HasValue)
			{
				return cVar;
			}
			return cVar + (float)(_gameTiming.RealTime - _mobAttachmentTime.Value).TotalMinutes;
		}
	}

	public void Initialize()
	{
		_sawmill = _logManager.GetSawmill("clientplaytime");
		((INetManager)_clientNetManager).Connected += OnConnected;
		_playerManager.LocalPlayerAttached += OnPlayerAttached;
		_playerManager.LocalPlayerDetached += OnPlayerDetached;
	}

	private void OnConnected(object? sender, NetChannelArgs args)
	{
		DateTime now = DateTime.Now;
		_sawmill.Info($"Current day: {now.Day} Current Date: {now.Date.ToString("yyyy-MM-dd")}");
		string cVar = _configurationManager.GetCVar<string>(CCVars.PlaytimeLastConnectDate);
		string text = now.Date.ToString("yyyy-MM-dd");
		if (!(text == cVar))
		{
			_configurationManager.SetCVar<float>(CCVars.PlaytimeMinutesToday, 0f, false);
			_configurationManager.SetCVar<string>(CCVars.PlaytimeLastConnectDate, text, false);
		}
	}

	private void OnPlayerAttached(EntityUid entity)
	{
		_mobAttachmentTime = _gameTiming.RealTime;
	}

	private void OnPlayerDetached(EntityUid entity)
	{
		if (!_mobAttachmentTime.HasValue)
		{
			return;
		}
		float playtimeMinutesToday = PlaytimeMinutesToday;
		_mobAttachmentTime = null;
		float num = playtimeMinutesToday - _configurationManager.GetCVar<float>(CCVars.PlaytimeMinutesToday);
		if (num < 0f)
		{
			_sawmill.Error("Time differential on player detachment somehow less than zero!");
		}
		else if (!(num < 1f))
		{
			_configurationManager.SetCVar<float>(CCVars.PlaytimeMinutesToday, playtimeMinutesToday, false);
			_sawmill.Info($"Recorded {num} minutes of living playtime!");
			try
			{
				_configurationManager.SaveToFile();
			}
			catch
			{
			}
		}
	}
}
