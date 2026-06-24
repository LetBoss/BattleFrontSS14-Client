using System.Numerics;
using Content.Client.Administration.Managers;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.DecalPlacer;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared.Input;
using Robust.Client.Input;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controllers.Implementations;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client._PUBG.Sponsor;

public sealed class SponsorSandboxUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	[Dependency]
	private IClientAdminManager _admin;

	[Dependency]
	private IInputManager _input;

	[Dependency]
	private IResourceCache _resources;

	private SponsorMenuWindow? _window;

	private MenuButton? _menuButton;

	private bool _systemSubscribed;

	private SponsorSandboxSystem SponsorSandboxSystem => base.EntityManager.System<SponsorSandboxSystem>();

	private SponsorEntitySpawningUIController SponsorEntitySpawningController => base.UIManager.GetUIController<SponsorEntitySpawningUIController>();

	private EntitySpawningUIController EntitySpawningController => base.UIManager.GetUIController<EntitySpawningUIController>();

	private TileSpawningUIController TileSpawningController => base.UIManager.GetUIController<TileSpawningUIController>();

	private DecalPlacerUIController DecalPlacerController => base.UIManager.GetUIController<DecalPlacerUIController>();

	public void OnStateEntered(GameplayState state)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		EnsureWindow();
		EnsureMenuButton();
		EnsureSystemSubscribed();
		ApplyState(SponsorSandboxSystem.State);
		_input.SetInputCommand(ContentKeyFunctions.OpenEntitySpawnWindow, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			if (_admin.CanAdminPlace())
			{
				EntitySpawningController.ToggleWindow();
			}
			else if (SponsorSandboxSystem.State.AllowSpawnEntities)
			{
				SponsorEntitySpawningController.ToggleWindow();
			}
		}, (StateInputCmdDelegate)null, true, true));
	}

	public void OnStateExited(GameplayState state)
	{
		if (_menuButton != null)
		{
			Control parent = ((Control)_menuButton).Parent;
			if (parent != null)
			{
				parent.RemoveChild((Control)(object)_menuButton);
			}
			_menuButton = null;
		}
		if (_window != null)
		{
			((BaseWindow)_window).Close();
			_window = null;
		}
		if (_systemSubscribed)
		{
			SponsorSandboxSystem sponsorSandboxSystem = base.EntityManager.SystemOrNull<SponsorSandboxSystem>();
			if (sponsorSandboxSystem != null)
			{
				sponsorSandboxSystem.OnStateUpdated -= OnSponsorStateUpdated;
			}
			_systemSubscribed = false;
		}
	}

	private void EnsureWindow()
	{
		SponsorMenuWindow window = _window;
		if (window != null && !((Control)window).Disposed)
		{
			return;
		}
		_window = base.UIManager.CreateWindow<SponsorMenuWindow>();
		((BaseWindow)_window).OpenCentered();
		((BaseWindow)_window).Close();
		((BaseWindow)_window).OnOpen += delegate
		{
			if (_menuButton != null)
			{
				((BaseButton)_menuButton).Pressed = true;
			}
		};
		((BaseWindow)_window).OnClose += delegate
		{
			if (_menuButton != null)
			{
				((BaseButton)_menuButton).Pressed = false;
			}
		};
		((BaseButton)_window.SpawnTabControl.SpawnEntitiesButton).OnPressed += delegate
		{
			SponsorEntitySpawningController.ToggleWindow();
		};
		((BaseButton)_window.SpawnTabControl.SpawnTilesButton).OnPressed += delegate
		{
			TileSpawningController.ToggleWindow();
		};
		((BaseButton)_window.SpawnTabControl.SpawnDecalsButton).OnPressed += delegate
		{
			DecalPlacerController.ToggleWindow();
		};
		((BaseButton)_window.SpawnTabControl.SponsorArenaButton).OnPressed += delegate
		{
			SponsorSandboxSystem.RequestSponsorArenaTeleport();
		};
		((BaseButton)_window.SpawnTabControl.SponsorAghostButton).OnPressed += delegate
		{
			SponsorSandboxSystem.RequestSponsorAghost();
		};
	}

	private void EnsureMenuButton()
	{
		if (_menuButton == null)
		{
			GameTopMenuBar activeUIWidgetOrNull = base.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>();
			if (activeUIWidgetOrNull != null)
			{
				TextureResource resource = _resources.GetResource<TextureResource>("/Textures/Interface/sandbox.svg.192dpi.png", true);
				MenuButton obj = new MenuButton
				{
					Icon = TextureResource.op_Implicit(resource)
				};
				((Control)obj).ToolTip = Loc.GetString("pubg-sponsor-sandbox-button-tooltip");
				((Control)obj).MinSize = new Vector2(42f, 64f);
				((Control)obj).HorizontalExpand = true;
				_menuButton = obj;
				((Control)_menuButton).AddStyleClass("ButtonSquare");
				((BaseButton)_menuButton).OnPressed += SponsorSandboxButtonPressed;
				((Control)activeUIWidgetOrNull).AddChild((Control)(object)_menuButton);
			}
		}
	}

	private void SponsorSandboxButtonPressed(ButtonEventArgs args)
	{
		ToggleWindow();
	}

	private void ToggleWindow()
	{
		if (_window != null && IsSponsorMenuVisible(SponsorSandboxSystem.State))
		{
			if (((BaseWindow)_window).IsOpen)
			{
				((BaseWindow)_window).Close();
			}
			else
			{
				((BaseWindow)_window).Open();
			}
		}
	}

	private void EnsureSystemSubscribed()
	{
		if (!_systemSubscribed)
		{
			SponsorSandboxSystem.OnStateUpdated += OnSponsorStateUpdated;
			_systemSubscribed = true;
		}
	}

	private void OnSponsorStateUpdated(SponsorSandboxState state)
	{
		ApplyState(state);
	}

	private void ApplyState(SponsorSandboxState state)
	{
		bool flag = IsSponsorMenuVisible(state);
		if (_menuButton != null)
		{
			((Control)_menuButton).Visible = flag;
		}
		SponsorEntitySpawningController.UpdatePermissions(state);
		if (!state.AllowSpawnEntities)
		{
			SponsorEntitySpawningController.CloseWindow();
		}
		if (_window != null && !((Control)_window).Disposed)
		{
			if (!flag && ((BaseWindow)_window).IsOpen)
			{
				((BaseWindow)_window).Close();
			}
			((BaseButton)_window.SpawnTabControl.SpawnEntitiesButton).Disabled = !state.AllowSpawnEntities;
			((BaseButton)_window.SpawnTabControl.SpawnTilesButton).Disabled = !state.AllowSpawnTiles;
			((BaseButton)_window.SpawnTabControl.SpawnDecalsButton).Disabled = !state.AllowSpawnDecals;
			((BaseButton)_window.SpawnTabControl.SponsorArenaButton).Disabled = !state.AllowSponsorArena;
			((BaseButton)_window.SpawnTabControl.SponsorAghostButton).Disabled = !state.AllowSponsorAghost;
		}
	}

	private bool IsSponsorMenuVisible(SponsorSandboxState state)
	{
		if (state.Enabled)
		{
			return !state.IsMiniGameSandbox;
		}
		return false;
	}
}
