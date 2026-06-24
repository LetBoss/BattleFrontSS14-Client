using System;
using Content.Shared.CCVar;
using Robust.Client;
using Robust.Client.Graphics;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;

namespace Content.Client.GameTicking.Managers;

public sealed class TitleWindowManager
{
	[Dependency]
	private IBaseClient _client;

	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IGameController _gameController;

	public void Initialize()
	{
		_cfg.OnValueChanged<string>(CVars.GameHostName, (Action<string>)OnHostnameChange, true);
		_cfg.OnValueChanged<bool>(CCVars.GameHostnameInTitlebar, (Action<bool>)OnHostnameTitleChange, true);
		_client.RunLevelChanged += OnRunLevelChangedChange;
	}

	public void Shutdown()
	{
		_cfg.UnsubValueChanged<string>(CVars.GameHostName, (Action<string>)OnHostnameChange);
		_cfg.UnsubValueChanged<bool>(CCVars.GameHostnameInTitlebar, (Action<bool>)OnHostnameTitleChange);
	}

	private void OnHostnameChange(string hostname)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		string text = _gameController.GameTitle();
		if ((int)_client.RunLevel == 1)
		{
			_clyde.SetWindowTitle(text);
		}
		else if (_cfg.GetCVar<bool>(CCVars.GameHostnameInTitlebar))
		{
			_clyde.SetWindowTitle(hostname + " - " + text);
		}
		else
		{
			_clyde.SetWindowTitle(text);
		}
	}

	private void OnHostnameTitleChange(bool colonthree)
	{
		OnHostnameChange(_cfg.GetCVar<string>(CVars.GameHostName));
	}

	private void OnRunLevelChangedChange(object? sender, RunLevelChangedEventArgs runLevelChangedEventArgs)
	{
		OnHostnameChange(_cfg.GetCVar<string>(CVars.GameHostName));
	}
}
