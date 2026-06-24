using System;
using System.Text.RegularExpressions;
using Content.Client.MainMenu.UI;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Robust.Client;
using Robust.Client.ResourceManagement;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared;
using Robust.Shared.AuthLib;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Client.MainMenu;

public sealed class MainScreen : State
{
	[Dependency]
	private IBaseClient _client;

	[Dependency]
	private IClientNetManager _netManager;

	[Dependency]
	private IConfigurationManager _configurationManager;

	[Dependency]
	private IGameController _controllerProxy;

	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private IUserInterfaceManager _userInterfaceManager;

	[Dependency]
	private ILogManager _logManager;

	private ISawmill _sawmill;

	private MainMenuControl _mainMenuControl;

	private bool _isConnecting;

	private static readonly Regex IPv6Regex = new Regex("\\[(.*:.*:.*)](?::(\\d+))?");

	protected override void Startup()
	{
		_sawmill = _logManager.GetSawmill("mainmenu");
		_mainMenuControl = new MainMenuControl(_resourceCache, _configurationManager);
		((Control)_userInterfaceManager.StateRoot).AddChild((Control)(object)_mainMenuControl);
		((BaseButton)_mainMenuControl.QuitButton).OnPressed += QuitButtonPressed;
		((BaseButton)_mainMenuControl.OptionsButton).OnPressed += OptionsButtonPressed;
		((BaseButton)_mainMenuControl.DirectConnectButton).OnPressed += DirectConnectButtonPressed;
		_mainMenuControl.AddressBox.OnTextEntered += AddressBoxEntered;
		((BaseButton)_mainMenuControl.ChangelogButton).OnPressed += ChangelogButtonPressed;
		_client.RunLevelChanged += RunLevelChanged;
	}

	protected override void Shutdown()
	{
		_client.RunLevelChanged -= RunLevelChanged;
		_netManager.ConnectFailed -= _onConnectFailed;
		((Control)_mainMenuControl).Orphan();
	}

	private void ChangelogButtonPressed(ButtonEventArgs args)
	{
		_userInterfaceManager.GetUIController<ChangelogUIController>().ToggleWindow();
	}

	private void OptionsButtonPressed(ButtonEventArgs args)
	{
		_userInterfaceManager.GetUIController<OptionsUIController>().ToggleWindow();
	}

	private void QuitButtonPressed(ButtonEventArgs args)
	{
		_controllerProxy.Shutdown((string)null);
	}

	private void DirectConnectButtonPressed(ButtonEventArgs args)
	{
		LineEdit addressBox = _mainMenuControl.AddressBox;
		TryConnect(addressBox.Text);
	}

	private void AddressBoxEntered(LineEditEventArgs args)
	{
		if (!_isConnecting)
		{
			TryConnect(args.Text);
		}
	}

	private void TryConnect(string address)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		string text = _mainMenuControl.UsernameBox.Text.Trim();
		UsernameInvalidReason val = default(UsernameInvalidReason);
		if (!UsernameHelpers.IsNameValid(text, ref val))
		{
			string item = Loc.GetString(UsernameHelpersExt.ToText(val));
			_userInterfaceManager.Popup(Loc.GetString("main-menu-invalid-username-with-reason", new(string, object)[1] { ("invalidReason", item) }), Loc.GetString("main-menu-invalid-username"), true);
			return;
		}
		string cVar = _configurationManager.GetCVar<string>(CVars.PlayerName);
		if (_mainMenuControl.UsernameBox.Text != cVar)
		{
			_configurationManager.SetCVar<string>(CVars.PlayerName, text, false);
			_configurationManager.SaveToFile();
		}
		_setConnectingState(state: true);
		_netManager.ConnectFailed += _onConnectFailed;
		try
		{
			ParseAddress(address, out string ip, out ushort port);
			_client.ConnectToServer(ip, port);
		}
		catch (ArgumentException ex)
		{
			_userInterfaceManager.Popup("Unable to connect: " + ex.Message, "Connection error.", true);
			_sawmill.Warning(ex.ToString());
			_netManager.ConnectFailed -= _onConnectFailed;
			_setConnectingState(state: false);
		}
	}

	private void RunLevelChanged(object? obj, RunLevelChangedEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		ClientRunLevel newLevel = args.NewLevel;
		if ((int)newLevel != 1)
		{
			if ((int)newLevel == 2)
			{
				_setConnectingState(state: true);
			}
		}
		else
		{
			_setConnectingState(state: false);
			_netManager.ConnectFailed -= _onConnectFailed;
		}
	}

	private void ParseAddress(string address, out string ip, out ushort port)
	{
		Match match = IPv6Regex.Match(address);
		if (match != Match.Empty)
		{
			ip = match.Groups[1].Value;
			if (!match.Groups[2].Success)
			{
				port = _client.DefaultPort;
			}
			else if (!ushort.TryParse(match.Groups[2].Value, out port))
			{
				throw new ArgumentException("Not a valid port.");
			}
			return;
		}
		string[] array = address.Split(':');
		ip = address;
		port = _client.DefaultPort;
		if (array.Length > 2)
		{
			throw new ArgumentException("Not a valid Address.");
		}
		if (array.Length == 2)
		{
			ip = array[0];
			if (!ushort.TryParse(array[1], out port))
			{
				throw new ArgumentException("Not a valid port.");
			}
		}
	}

	private void _onConnectFailed(object? _, NetConnectFailArgs args)
	{
		_userInterfaceManager.Popup(Loc.GetString("main-menu-failed-to-connect", new(string, object)[1] { ("reason", args.Reason) }), (string)null, true);
		_netManager.ConnectFailed -= _onConnectFailed;
		_setConnectingState(state: false);
	}

	private void _setConnectingState(bool state)
	{
		_isConnecting = state;
		((BaseButton)_mainMenuControl.DirectConnectButton).Disabled = state;
	}
}
