using System;
using System.Linq;
using Content.Client.Administration.Managers;
using Content.Client.Administration.Systems;
using Content.Client.Gameplay;
using Content.Client.Lobby;
using Content.Client.Lobby.UI;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Content.Shared.Input;
using Robust.Client.Audio;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Client.UserInterface.Systems.Bwoink;

public sealed class AHelpUIController : UIController, IOnSystemChanged<BwoinkSystem>, IOnSystemLoaded<BwoinkSystem>, IOnSystemUnloaded<BwoinkSystem>, IOnStateChanged<GameplayState>, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>, IOnStateChanged<LobbyState>, IOnStateEntered<LobbyState>, IOnStateExited<LobbyState>
{
	[Dependency]
	private IClientAdminManager _adminManager;

	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IClyde _clyde;

	[Dependency]
	private IUserInterfaceManager _uiManager;

	[UISystemDependency]
	private readonly AudioSystem _audio;

	private BwoinkSystem? _bwoinkSystem;

	public IAHelpUIHandler? UIHelper;

	private bool _discordRelayActive;

	private bool _hasUnreadAHelp;

	private bool _bwoinkSoundEnabled;

	private string? _aHelpSound;

	public MenuButton? GameAHelpButton => base.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.AHelpButton;

	public Button? LobbyAHelpButton => (base.UIManager.ActiveScreen as LobbyGui)?.AHelpButton;

	protected override string SawmillName => "c.s.go.es.bwoink";

	public override void Initialize()
	{
		((UIController)this).Initialize();
		((UIController)this).SubscribeNetworkEvent<BwoinkDiscordRelayUpdated>((EntitySessionEventHandler<BwoinkDiscordRelayUpdated>)DiscordRelayUpdated, (Type[])null, (Type[])null);
		((UIController)this).SubscribeNetworkEvent<BwoinkPlayerTypingUpdated>((EntitySessionEventHandler<BwoinkPlayerTypingUpdated>)PeopleTypingUpdated, (Type[])null, (Type[])null);
		_adminManager.AdminStatusUpdated += OnAdminStatusUpdated;
		_config.OnValueChanged<string>(CCVars.AHelpSound, (Action<string>)delegate(string v)
		{
			_aHelpSound = v;
		}, true);
		_config.OnValueChanged<bool>(CCVars.BwoinkSoundEnabled, (Action<bool>)delegate(bool v)
		{
			_bwoinkSoundEnabled = v;
		}, true);
	}

	public void UnloadButton()
	{
		if (GameAHelpButton != null)
		{
			((BaseButton)GameAHelpButton).OnPressed -= AHelpButtonPressed;
		}
		if (LobbyAHelpButton != null)
		{
			((BaseButton)LobbyAHelpButton).OnPressed -= AHelpButtonPressed;
		}
	}

	public void LoadButton()
	{
		if (GameAHelpButton != null)
		{
			((BaseButton)GameAHelpButton).OnPressed += AHelpButtonPressed;
		}
		if (LobbyAHelpButton != null)
		{
			((BaseButton)LobbyAHelpButton).OnPressed += AHelpButtonPressed;
		}
	}

	private void OnAdminStatusUpdated()
	{
		IAHelpUIHandler uIHelper = UIHelper;
		if (uIHelper != null && uIHelper.IsOpen)
		{
			EnsureUIHelper();
		}
	}

	private void AHelpButtonPressed(ButtonEventArgs obj)
	{
		EnsureUIHelper();
		UIHelper.ToggleWindow();
	}

	public void OnSystemLoaded(BwoinkSystem system)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		_bwoinkSystem = system;
		_bwoinkSystem.OnBwoinkTextMessageRecieved += ReceivedBwoink;
		CommandBinds.Builder.Bind(ContentKeyFunctions.OpenAHelp, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			ToggleWindow();
		}, (StateInputCmdDelegate)null, true, true)).Register<AHelpUIController>();
	}

	public void OnSystemUnloaded(BwoinkSystem system)
	{
		CommandBinds.Unregister<AHelpUIController>();
		_bwoinkSystem.OnBwoinkTextMessageRecieved -= ReceivedBwoink;
		_bwoinkSystem = null;
	}

	private void SetAHelpPressed(bool pressed)
	{
		if (GameAHelpButton != null)
		{
			((BaseButton)GameAHelpButton).Pressed = pressed;
		}
		if (LobbyAHelpButton != null)
		{
			((BaseButton)LobbyAHelpButton).Pressed = pressed;
		}
		base.UIManager.ClickSound();
		UnreadAHelpRead();
	}

	private void ReceivedBwoink(object? sender, SharedBwoinkSystem.BwoinkTextMessage message)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		((UIController)this).Log.Info($"@{message.UserId}: {message.Text}");
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		if (localSession == null)
		{
			return;
		}
		if (message.PlaySound && localSession.UserId != message.TrueSender)
		{
			if (_aHelpSound != null && (_bwoinkSoundEnabled || !_adminManager.IsActive()))
			{
				((SharedAudioSystem)_audio).PlayGlobal((ResolvedSoundSpecifier)new ResolvedPathSpecifier(_aHelpSound), Filter.Local(), false, (AudioParams?)null);
			}
			_clyde.RequestWindowAttention();
		}
		EnsureUIHelper();
		if (!UIHelper.IsOpen)
		{
			UnreadAHelpReceived();
		}
		UIHelper.Receive(message);
	}

	private void DiscordRelayUpdated(BwoinkDiscordRelayUpdated args, EntitySessionEventArgs session)
	{
		_discordRelayActive = args.DiscordRelayEnabled;
		UIHelper?.DiscordRelayChanged(_discordRelayActive);
	}

	private void PeopleTypingUpdated(BwoinkPlayerTypingUpdated args, EntitySessionEventArgs session)
	{
		UIHelper?.PeopleTypingUpdated(args);
	}

	public void EnsureUIHelper()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		bool flag = _adminManager.HasFlag(AdminFlags.Adminhelp);
		if (UIHelper == null || UIHelper.IsAdmin != flag)
		{
			UIHelper?.Dispose();
			NetUserId value = ((ISharedPlayerManager)_playerManager).LocalUser.Value;
			IAHelpUIHandler uIHelper;
			if (!flag)
			{
				IAHelpUIHandler iAHelpUIHandler = new UserAHelpUIHandler(value);
				uIHelper = iAHelpUIHandler;
			}
			else
			{
				IAHelpUIHandler iAHelpUIHandler = new AdminAHelpUIHandler(value);
				uIHelper = iAHelpUIHandler;
			}
			UIHelper = uIHelper;
			UIHelper.DiscordRelayChanged(_discordRelayActive);
			UIHelper.SendMessageAction = delegate(NetUserId userId, string textMessage, bool playSound, bool adminOnly)
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				_bwoinkSystem?.Send(userId, textMessage, playSound, adminOnly);
			};
			UIHelper.InputTextChanged += delegate(NetUserId channel, string text)
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				_bwoinkSystem?.SendInputTextUpdated(channel, text.Length > 0);
			};
			UIHelper.OnClose += delegate
			{
				SetAHelpPressed(pressed: false);
			};
			UIHelper.OnOpen += delegate
			{
				SetAHelpPressed(pressed: true);
			};
			SetAHelpPressed(UIHelper.IsOpen);
		}
	}

	public void Open()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		NetUserId? localUser = ((ISharedPlayerManager)_playerManager).LocalUser;
		if (localUser.HasValue)
		{
			EnsureUIHelper();
			if (!UIHelper.IsOpen)
			{
				UIHelper.Open(localUser.Value, _discordRelayActive);
			}
		}
	}

	public void Open(NetUserId userId)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		EnsureUIHelper();
		if (UIHelper.IsAdmin)
		{
			UIHelper?.Open(userId, _discordRelayActive);
		}
	}

	public void ToggleWindow()
	{
		EnsureUIHelper();
		UIHelper?.ToggleWindow();
	}

	public void PopOut()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		EnsureUIHelper();
		if (UIHelper is AdminAHelpUIHandler { Window: not null, Control: not null } adminAHelpUIHandler)
		{
			((Control)adminAHelpUIHandler.Control).Orphan();
			((Control)adminAHelpUIHandler.Window).Orphan();
			adminAHelpUIHandler.Window = null;
			adminAHelpUIHandler.EverOpened = false;
			IClydeMonitor monitor = _clyde.EnumerateMonitors().First();
			adminAHelpUIHandler.ClydeWindow = _clyde.CreateWindow(new WindowCreateParameters
			{
				Maximized = false,
				Title = "Admin Help",
				Monitor = monitor,
				Width = 1100,
				Height = 620
			});
			adminAHelpUIHandler.ClydeWindow.RequestClosed += adminAHelpUIHandler.OnRequestClosed;
			adminAHelpUIHandler.ClydeWindow.DisposeOnClose = true;
			adminAHelpUIHandler.WindowRoot = _uiManager.CreateWindowRoot(adminAHelpUIHandler.ClydeWindow);
			((Control)adminAHelpUIHandler.WindowRoot).AddChild((Control)(object)adminAHelpUIHandler.Control);
			((BaseButton)adminAHelpUIHandler.Control.PopOut).Disabled = true;
			((Control)adminAHelpUIHandler.Control.PopOut).Visible = false;
		}
	}

	public void UnreadAHelpReceived()
	{
		MenuButton? gameAHelpButton = GameAHelpButton;
		if (gameAHelpButton != null)
		{
			((Control)gameAHelpButton).StyleClasses.Add("topButtonLabel");
		}
		Button? lobbyAHelpButton = LobbyAHelpButton;
		if (lobbyAHelpButton != null)
		{
			((Control)lobbyAHelpButton).StyleClasses.Add("ButtonColorRed");
		}
		_hasUnreadAHelp = true;
	}

	private void UnreadAHelpRead()
	{
		MenuButton? gameAHelpButton = GameAHelpButton;
		if (gameAHelpButton != null)
		{
			((Control)gameAHelpButton).StyleClasses.Remove("topButtonLabel");
		}
		Button? lobbyAHelpButton = LobbyAHelpButton;
		if (lobbyAHelpButton != null)
		{
			((Control)lobbyAHelpButton).StyleClasses.Remove("ButtonColorRed");
		}
		_hasUnreadAHelp = false;
	}

	public void OnStateEntered(GameplayState state)
	{
		if (GameAHelpButton != null)
		{
			((BaseButton)GameAHelpButton).OnPressed -= AHelpButtonPressed;
			((BaseButton)GameAHelpButton).OnPressed += AHelpButtonPressed;
			((BaseButton)GameAHelpButton).Pressed = UIHelper?.IsOpen ?? false;
			if (_hasUnreadAHelp)
			{
				UnreadAHelpReceived();
			}
			else
			{
				UnreadAHelpRead();
			}
		}
	}

	public void OnStateExited(GameplayState state)
	{
		if (GameAHelpButton != null)
		{
			((BaseButton)GameAHelpButton).OnPressed -= AHelpButtonPressed;
		}
	}

	public void OnStateEntered(LobbyState state)
	{
		if (LobbyAHelpButton != null)
		{
			((BaseButton)LobbyAHelpButton).OnPressed -= AHelpButtonPressed;
			((BaseButton)LobbyAHelpButton).OnPressed += AHelpButtonPressed;
			((BaseButton)LobbyAHelpButton).Pressed = UIHelper?.IsOpen ?? false;
			if (_hasUnreadAHelp)
			{
				UnreadAHelpReceived();
			}
			else
			{
				UnreadAHelpRead();
			}
		}
	}

	public void OnStateExited(LobbyState state)
	{
		if (LobbyAHelpButton != null)
		{
			((BaseButton)LobbyAHelpButton).OnPressed -= AHelpButtonPressed;
		}
	}
}
