using System.Collections.Generic;
using System.Numerics;
using Content.Client._PUBG.Sponsor;
using Content.Client.Gameplay;
using Content.Client.Markers;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.DecalPlacer;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared._PUBG.MiniGames;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controllers.Implementations;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client._PUBG.MiniGames;

public sealed class MiniGameSandboxUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	[Dependency]
	private IResourceCache _resources;

	private MiniGameSandboxMenuWindow? _window;

	private MenuButton? _menuButton;

	private bool _sandboxStateSubscribed;

	private bool _arenaSystemSubscribed;

	private bool _showingSpawns;

	private bool _customizationActive;

	[UISystemDependency]
	private readonly MarkerSystem _marker;

	private SponsorSandboxSystem SponsorSandboxSystem => base.EntityManager.System<SponsorSandboxSystem>();

	private MiniGameCustomArenaClientSystem ArenaSystem => base.EntityManager.System<MiniGameCustomArenaClientSystem>();

	private SponsorEntitySpawningUIController SponsorEntitySpawningController => base.UIManager.GetUIController<SponsorEntitySpawningUIController>();

	private TileSpawningUIController TileSpawningController => base.UIManager.GetUIController<TileSpawningUIController>();

	private DecalPlacerUIController DecalPlacerController => base.UIManager.GetUIController<DecalPlacerUIController>();

	public void OnStateEntered(GameplayState state)
	{
		_customizationActive = false;
		EnsureWindow();
		EnsureSandboxStateSubscribed();
		EnsureArenaSystemSubscribed();
		if (_menuButton != null)
		{
			((Control)_menuButton).Visible = false;
		}
	}

	public void OnStateExited(GameplayState state)
	{
		_customizationActive = false;
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
		if (_sandboxStateSubscribed)
		{
			SponsorSandboxSystem sponsorSandboxSystem = base.EntityManager.SystemOrNull<SponsorSandboxSystem>();
			if (sponsorSandboxSystem != null)
			{
				sponsorSandboxSystem.OnStateUpdated -= OnSandboxStateUpdated;
			}
			_sandboxStateSubscribed = false;
		}
		if (_arenaSystemSubscribed)
		{
			MiniGameCustomArenaClientSystem miniGameCustomArenaClientSystem = base.EntityManager.SystemOrNull<MiniGameCustomArenaClientSystem>();
			if (miniGameCustomArenaClientSystem != null)
			{
				miniGameCustomArenaClientSystem.OnArenaListUpdated -= OnArenaListUpdated;
				miniGameCustomArenaClientSystem.OnError -= OnArenaError;
				miniGameCustomArenaClientSystem.OnUIOpen -= OnArenaUIOpen;
				miniGameCustomArenaClientSystem.OnUIClose -= OnArenaUIClose;
			}
			_arenaSystemSubscribed = false;
		}
	}

	private void EnsureWindow()
	{
		MiniGameSandboxMenuWindow window = _window;
		if (window != null && !((Control)window).Disposed)
		{
			return;
		}
		_window = base.UIManager.CreateWindow<MiniGameSandboxMenuWindow>();
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
		((BaseButton)_window.SandboxTabControl.SpawnEntitiesButton).OnPressed += delegate
		{
			if (SponsorSandboxSystem.State.AllowSpawnEntities)
			{
				SponsorEntitySpawningController.ToggleWindow();
			}
		};
		((BaseButton)_window.SandboxTabControl.SpawnTilesButton).OnPressed += delegate
		{
			TileSpawningController.ToggleWindow();
		};
		((BaseButton)_window.SandboxTabControl.SpawnDecalsButton).OnPressed += delegate
		{
			DecalPlacerController.ToggleWindow();
		};
		((BaseButton)_window.SandboxTabControl.ShowSpawnsButton).OnPressed += delegate
		{
			ToggleShowSpawns();
		};
		_window.ArenaTabControl.OnShowSpawnsToggle += ToggleShowSpawns;
		_window.ArenaTabControl.OnExitCustomization += delegate
		{
			ArenaSystem.RequestExitCustomization();
		};
		_window.ArenaTabControl.OnSaveArena += delegate(string displayName, bool overwrite, string? existingName)
		{
			ArenaSystem.RequestSaveArena(displayName, overwrite, existingName);
		};
		_window.ArenaTabControl.OnDeleteArena += delegate(string arenaName)
		{
			ArenaSystem.RequestDeleteArena(arenaName);
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
				((Control)obj).ToolTip = Loc.GetString("pubg-mini-sandbox-button-tooltip");
				((Control)obj).MinSize = new Vector2(42f, 64f);
				((Control)obj).HorizontalExpand = true;
				((Control)obj).Visible = _customizationActive;
				_menuButton = obj;
				((Control)_menuButton).AddStyleClass("ButtonSquare");
				((BaseButton)_menuButton).OnPressed += OnSandboxButtonPressed;
				((Control)activeUIWidgetOrNull).AddChild((Control)(object)_menuButton);
			}
		}
	}

	private void OnSandboxButtonPressed(ButtonEventArgs args)
	{
		ToggleWindow();
	}

	private void ToggleWindow()
	{
		if (_window != null && _customizationActive)
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

	private void EnsureSandboxStateSubscribed()
	{
		if (!_sandboxStateSubscribed)
		{
			SponsorSandboxSystem.OnStateUpdated += OnSandboxStateUpdated;
			_sandboxStateSubscribed = true;
		}
	}

	private void EnsureArenaSystemSubscribed()
	{
		if (!_arenaSystemSubscribed)
		{
			ArenaSystem.OnArenaListUpdated += OnArenaListUpdated;
			ArenaSystem.OnError += OnArenaError;
			ArenaSystem.OnUIOpen += OnArenaUIOpen;
			ArenaSystem.OnUIClose += OnArenaUIClose;
			_arenaSystemSubscribed = true;
		}
	}

	private void OnSandboxStateUpdated(SponsorSandboxState state)
	{
		ApplySandboxState(state);
	}

	private void ApplySandboxState(SponsorSandboxState state)
	{
		SponsorEntitySpawningController.UpdatePermissions(state);
		if (!state.AllowSpawnEntities)
		{
			SponsorEntitySpawningController.CloseWindow();
		}
		if (_window != null && !((Control)_window).Disposed)
		{
			((BaseButton)_window.SandboxTabControl.SpawnEntitiesButton).Disabled = !state.AllowSpawnEntities;
			((BaseButton)_window.SandboxTabControl.SpawnTilesButton).Disabled = !state.AllowSpawnTiles;
			((BaseButton)_window.SandboxTabControl.SpawnDecalsButton).Disabled = !state.AllowSpawnDecals;
		}
	}

	private void OnArenaListUpdated(List<MiniGameArenaInfo> arenas, int maxArenas)
	{
		if (_window != null && !((Control)_window).Disposed)
		{
			_window.ArenaTabControl.UpdateArenaList(arenas, maxArenas);
		}
	}

	private void OnArenaError(string errorLocKey)
	{
		if (_window != null && !((Control)_window).Disposed)
		{
			_window.ArenaTabControl.ShowError(errorLocKey);
		}
	}

	private void OnArenaUIOpen()
	{
		if (_window != null && !((Control)_window).Disposed)
		{
			_customizationActive = true;
			EnsureMenuButton();
			if (_menuButton != null)
			{
				((Control)_menuButton).Visible = true;
			}
			((BaseWindow)_window).Open();
			ApplySandboxState(SponsorSandboxSystem.State);
			ArenaSystem.RequestArenaList();
			SyncSpawnButton();
		}
	}

	private void OnArenaUIClose()
	{
		_customizationActive = false;
		if (_menuButton != null)
		{
			((Control)_menuButton).Visible = false;
			((BaseButton)_menuButton).Pressed = false;
		}
		if (_window != null && !((Control)_window).Disposed)
		{
			((BaseWindow)_window).Close();
			_marker.MarkersVisible = false;
			_showingSpawns = false;
			SyncSpawnButton();
		}
	}

	private void ToggleShowSpawns()
	{
		_showingSpawns = !_showingSpawns;
		_marker.MarkersVisible = _showingSpawns;
		SyncSpawnButton();
	}

	private void SyncSpawnButton()
	{
		if (_window != null && !((Control)_window).Disposed)
		{
			_window.SandboxTabControl.ShowSpawnsButton.Text = (_showingSpawns ? Loc.GetString("pubg-arena-hide-spawns") : Loc.GetString("pubg-arena-show-spawns"));
			_window.ArenaTabControl.SyncShowSpawns(_showingSpawns);
		}
	}
}
