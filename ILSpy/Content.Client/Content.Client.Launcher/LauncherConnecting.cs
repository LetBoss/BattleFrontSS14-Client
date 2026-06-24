using System;
using Robust.Client;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Client.Launcher;

public sealed class LauncherConnecting : State
{
	public enum Page : byte
	{
		Connecting,
		ConnectFailed,
		Disconnected
	}

	[Dependency]
	private IUserInterfaceManager _userInterfaceManager;

	[Dependency]
	private IClientNetManager _clientNetManager;

	[Dependency]
	private IGameController _gameController;

	[Dependency]
	private IBaseClient _baseClient;

	[Dependency]
	private IRobustRandom _random;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IClipboardManager _clipboard;

	private LauncherConnectingGui? _control;

	private readonly ISawmill _sawmill = Logger.GetSawmill("launcher-ui");

	private Page _currentPage;

	private string? _connectFailReason;

	public string? Address => _gameController.LaunchState.Ss14Address ?? _gameController.LaunchState.ConnectAddress;

	public string? ConnectFailReason
	{
		get
		{
			return _connectFailReason;
		}
		private set
		{
			_connectFailReason = value;
			this.ConnectFailReasonChanged?.Invoke(value);
		}
	}

	public string? LastDisconnectReason => _baseClient.LastDisconnectReason;

	public Page CurrentPage
	{
		get
		{
			return _currentPage;
		}
		private set
		{
			_currentPage = value;
			this.PageChanged?.Invoke(value);
		}
	}

	public ClientConnectionState ConnectionState => _clientNetManager.ClientConnectState;

	public event Action<Page>? PageChanged;

	public event Action<string?>? ConnectFailReasonChanged;

	public event Action<ClientConnectionState>? ConnectionStateChanged;

	public event Action<NetConnectFailArgs>? ConnectFailed;

	protected override void Startup()
	{
		_control = new LauncherConnectingGui(this, _random, _prototypeManager, _cfg, _clipboard);
		((Control)_userInterfaceManager.StateRoot).AddChild((Control)(object)_control);
		_clientNetManager.ConnectFailed += OnConnectFailed;
		_clientNetManager.ClientConnectStateChanged += OnConnectStateChanged;
		CurrentPage = Page.Connecting;
	}

	protected override void Shutdown()
	{
		LauncherConnectingGui? control = _control;
		if (control != null)
		{
			((Control)control).Orphan();
		}
		_clientNetManager.ConnectFailed -= OnConnectFailed;
		_clientNetManager.ClientConnectStateChanged -= OnConnectStateChanged;
	}

	private void OnConnectFailed(object? _, NetConnectFailArgs args)
	{
		if (args.RedialFlag)
		{
			Redial();
		}
		ConnectFailReason = args.Reason;
		CurrentPage = Page.ConnectFailed;
		this.ConnectFailed?.Invoke(args);
	}

	private void OnConnectStateChanged(ClientConnectionState state)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		this.ConnectionStateChanged?.Invoke(state);
	}

	public void RetryConnect()
	{
		if (_gameController.LaunchState.ConnectEndpoint != null)
		{
			_baseClient.ConnectToServer(_gameController.LaunchState.ConnectEndpoint);
			CurrentPage = Page.Connecting;
		}
	}

	public bool Redial()
	{
		try
		{
			if (_gameController.LaunchState.Ss14Address != null)
			{
				_gameController.Redial(_gameController.LaunchState.Ss14Address, (string)null);
				return true;
			}
			_sawmill.Info("Redial not possible, no Ss14Address");
		}
		catch (Exception value)
		{
			_sawmill.Error($"Redial exception: {value}");
		}
		return false;
	}

	public void Exit()
	{
		_gameController.Shutdown("Exit button pressed");
	}

	public void SetDisconnected()
	{
		CurrentPage = Page.Disconnected;
	}
}
