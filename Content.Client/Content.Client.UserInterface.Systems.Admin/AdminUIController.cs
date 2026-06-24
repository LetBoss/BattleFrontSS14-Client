using System;
using Content.Client.Administration.Managers;
using Content.Client.Administration.Systems;
using Content.Client.Administration.UI;
using Content.Client.Administration.UI.Tabs.ObjectsTab;
using Content.Client.Administration.UI.Tabs.PanicBunkerTab;
using Content.Client.Administration.UI.Tabs.PlayerTab;
using Content.Client.Gameplay;
using Content.Client.Lobby;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Client.Verbs.UI;
using Content.Shared.Administration;
using Content.Shared.Administration.Events;
using Content.Shared.Input;
using Robust.Client.Console;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;

namespace Content.Client.UserInterface.Systems.Admin;

public sealed class AdminUIController : UIController, IOnStateEntered<GameplayState>, IOnStateEntered<LobbyState>, IOnSystemChanged<AdminSystem>, IOnSystemLoaded<AdminSystem>, IOnSystemUnloaded<AdminSystem>
{
	[Dependency]
	private IClientAdminManager _admin;

	[Dependency]
	private IClientConGroupController _conGroups;

	[Dependency]
	private IClientConsoleHost _conHost;

	[Dependency]
	private IInputManager _input;

	[Dependency]
	private VerbMenuUIController _verb;

	private AdminMenuWindow? _window;

	private PanicBunkerStatus? _panicBunker;

	private MenuButton? AdminButton => base.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.AdminButton;

	public override void Initialize()
	{
		((UIController)this).Initialize();
		((UIController)this).SubscribeNetworkEvent<PanicBunkerChangedEvent>((EntitySessionEventHandler<PanicBunkerChangedEvent>)OnPanicBunkerUpdated, (Type[])null, (Type[])null);
	}

	private void OnPanicBunkerUpdated(PanicBunkerChangedEvent msg, EntitySessionEventArgs args)
	{
		bool num = _panicBunker == null && msg.Status.Enabled;
		_panicBunker = msg.Status;
		_window?.PanicBunkerControl.UpdateStatus(msg.Status);
		if (num)
		{
			((BaseWindow)base.UIManager.CreateWindow<PanicBunkerStatusWindow>()).OpenCentered();
		}
	}

	public void OnStateEntered(GameplayState state)
	{
		EnsureWindow();
		AdminStatusUpdated();
	}

	public void OnStateEntered(LobbyState state)
	{
		EnsureWindow();
		AdminStatusUpdated();
	}

	public void OnSystemLoaded(AdminSystem system)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		EnsureWindow();
		_admin.AdminStatusUpdated += AdminStatusUpdated;
		_input.SetInputCommand(ContentKeyFunctions.OpenAdminMenu, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			Toggle();
		}, (StateInputCmdDelegate)null, true, true));
	}

	public void OnSystemUnloaded(AdminSystem system)
	{
		if (_window != null)
		{
			((Control)_window).Orphan();
		}
		_admin.AdminStatusUpdated -= AdminStatusUpdated;
		CommandBinds.Unregister<AdminUIController>();
	}

	private void EnsureWindow()
	{
		AdminMenuWindow window = _window;
		if (window == null || ((Control)window).Disposed)
		{
			AdminMenuWindow? window2 = _window;
			if (window2 != null && ((Control)window2).Disposed)
			{
				OnWindowDisposed();
			}
			_window = base.UIManager.CreateWindow<AdminMenuWindow>();
			LayoutContainer.SetAnchorPreset((Control)(object)_window, (LayoutPreset)8, false);
			if (_panicBunker != null)
			{
				_window.PanicBunkerControl.UpdateStatus(_panicBunker);
			}
			_window.PlayerTabControl.OnEntryKeyBindDown += PlayerTabEntryKeyBindDown;
			_window.ObjectsTabControl.OnEntryKeyBindDown += ObjectsTabEntryKeyBindDown;
			((BaseWindow)_window).OnOpen += OnWindowOpen;
			((BaseWindow)_window).OnClose += OnWindowClosed;
			_window.OnDisposed += OnWindowDisposed;
		}
	}

	public void UnloadButton()
	{
		if (AdminButton != null)
		{
			((BaseButton)AdminButton).OnPressed -= AdminButtonPressed;
		}
	}

	public void LoadButton()
	{
		if (AdminButton != null)
		{
			((BaseButton)AdminButton).OnPressed += AdminButtonPressed;
		}
	}

	private void OnWindowOpen()
	{
		MenuButton? adminButton = AdminButton;
		if (adminButton != null)
		{
			((BaseButton)adminButton).SetClickPressed(true);
		}
	}

	private void OnWindowClosed()
	{
		MenuButton? adminButton = AdminButton;
		if (adminButton != null)
		{
			((BaseButton)adminButton).SetClickPressed(false);
		}
	}

	private void OnWindowDisposed()
	{
		if (AdminButton != null)
		{
			((BaseButton)AdminButton).Pressed = false;
		}
		if (_window != null)
		{
			_window.PlayerTabControl.OnEntryKeyBindDown -= PlayerTabEntryKeyBindDown;
			_window.ObjectsTabControl.OnEntryKeyBindDown -= ObjectsTabEntryKeyBindDown;
			((BaseWindow)_window).OnOpen -= OnWindowOpen;
			((BaseWindow)_window).OnClose -= OnWindowClosed;
			_window.OnDisposed -= OnWindowDisposed;
			_window = null;
		}
	}

	private void AdminStatusUpdated()
	{
		if (AdminButton != null)
		{
			((Control)AdminButton).Visible = ((IClientConGroupImplementation)_conGroups).CanAdminMenu();
		}
	}

	private void AdminButtonPressed(ButtonEventArgs args)
	{
		Toggle();
	}

	private void Toggle()
	{
		AdminMenuWindow window = _window;
		if (window != null && ((BaseWindow)window).IsOpen)
		{
			((BaseWindow)_window).Close();
		}
		else if (((IClientConGroupImplementation)_conGroups).CanAdminMenu())
		{
			AdminMenuWindow? window2 = _window;
			if (window2 != null)
			{
				((BaseWindow)window2).Open();
			}
		}
	}

	private void PlayerTabEntryKeyBindDown(GUIBoundKeyEventArgs args, ListData? data)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (!(data is PlayerListData playerListData))
		{
			return;
		}
		PlayerInfo info = playerListData.Info;
		if (!info.NetEntity.HasValue)
		{
			return;
		}
		NetEntity value = info.NetEntity.Value;
		BoundKeyFunction function = ((BoundKeyEventArgs)args).Function;
		if (function == EngineKeyFunctions.UIClick)
		{
			((IConsoleHost)_conHost).ExecuteCommand($"vv {value}");
		}
		else
		{
			if (!(function == EngineKeyFunctions.UIRightClick))
			{
				return;
			}
			_verb.OpenVerbMenu(value, force: true);
		}
		((BoundKeyEventArgs)args).Handle();
	}

	private void ObjectsTabEntryKeyBindDown(GUIBoundKeyEventArgs args, ListData? data)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (!(data is ObjectsListData objectsListData))
		{
			return;
		}
		NetEntity item = objectsListData.Info.Entity;
		BoundKeyFunction function = ((BoundKeyEventArgs)args).Function;
		if (function == EngineKeyFunctions.UIClick)
		{
			((IConsoleHost)_conHost).ExecuteCommand($"vv {item}");
		}
		else
		{
			if (!(function == EngineKeyFunctions.UIRightClick))
			{
				return;
			}
			_verb.OpenVerbMenu(item, force: true);
		}
		((BoundKeyEventArgs)args).Handle();
	}
}
