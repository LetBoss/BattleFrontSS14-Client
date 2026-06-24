using Content.Client._PUBG.Sponsor;
using Content.Client.Administration.Managers;
using Content.Client.Gameplay;
using Content.Client.Markers;
using Content.Client.Sandbox;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.DecalPlacer;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Client.UserInterface.Systems.Sandbox.Windows;
using Content.Shared.Input;
using Robust.Client.Debugging;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controllers.Implementations;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Client.UserInterface.Systems.Sandbox;

public sealed class SandboxUIController : UIController, IOnStateChanged<GameplayState>, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>, IOnSystemChanged<SandboxSystem>, IOnSystemLoaded<SandboxSystem>, IOnSystemUnloaded<SandboxSystem>
{
	[Dependency]
	private IConsoleHost _console;

	[Dependency]
	private IEyeManager _eye;

	[Dependency]
	private IInputManager _input;

	[Dependency]
	private ILightManager _light;

	[Dependency]
	private IClientAdminManager _admin;

	[Dependency]
	private IPlayerManager _player;

	[UISystemDependency]
	private readonly DebugPhysicsSystem _debugPhysics;

	[UISystemDependency]
	private readonly MarkerSystem _marker;

	[UISystemDependency]
	private readonly SandboxSystem _sandbox;

	private SandboxWindow? _window;

	private EntitySpawningUIController EntitySpawningController => base.UIManager.GetUIController<EntitySpawningUIController>();

	private TileSpawningUIController TileSpawningController => base.UIManager.GetUIController<TileSpawningUIController>();

	private DecalPlacerUIController DecalPlacerController => base.UIManager.GetUIController<DecalPlacerUIController>();

	private MenuButton? SandboxButton => base.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.SandboxButton;

	public void OnStateEntered(GameplayState state)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		EnsureWindow();
		CheckSandboxVisibility();
		_input.SetInputCommand(ContentKeyFunctions.OpenEntitySpawnWindow, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			if (_admin.CanAdminPlace())
			{
				EntitySpawningController.ToggleWindow();
			}
			else if (base.EntityManager.System<SponsorSandboxSystem>().State.AllowSpawnEntities)
			{
				base.UIManager.GetUIController<SponsorEntitySpawningUIController>().ToggleWindow();
			}
		}, (StateInputCmdDelegate)null, true, true));
		_input.SetInputCommand(ContentKeyFunctions.OpenSandboxWindow, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			ToggleWindow();
		}, (StateInputCmdDelegate)null, true, true));
		_input.SetInputCommand(ContentKeyFunctions.OpenTileSpawnWindow, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			if (_admin.CanAdminPlace())
			{
				TileSpawningController.ToggleWindow();
			}
		}, (StateInputCmdDelegate)null, true, true));
		_input.SetInputCommand(ContentKeyFunctions.OpenDecalSpawnWindow, InputCmdHandler.FromDelegate((StateInputCmdDelegate)delegate
		{
			if (_admin.CanAdminPlace())
			{
				DecalPlacerController.ToggleWindow();
			}
		}, (StateInputCmdDelegate)null, true, true));
		CommandBinds.Builder.Bind(ContentKeyFunctions.EditorCopyObject, (InputCmdHandler)new PointerInputCmdHandler(new PointerInputCmdDelegate(Copy), true, false)).Register<SandboxSystem>();
	}

	public void UnloadButton()
	{
		if (SandboxButton != null)
		{
			((BaseButton)SandboxButton).OnPressed -= SandboxButtonPressed;
		}
	}

	public void LoadButton()
	{
		if (SandboxButton != null)
		{
			((BaseButton)SandboxButton).OnPressed += SandboxButtonPressed;
		}
	}

	private void EnsureWindow()
	{
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Invalid comparison between Unknown and I4
		SandboxWindow window = _window;
		if (window != null && !((Control)window).Disposed)
		{
			return;
		}
		_window = base.UIManager.CreateWindow<SandboxWindow>();
		((BaseWindow)_window).OpenCentered();
		((BaseWindow)_window).Close();
		((BaseWindow)_window).OnOpen += delegate
		{
			((BaseButton)SandboxButton).Pressed = true;
		};
		((BaseWindow)_window).OnClose += delegate
		{
			((BaseButton)SandboxButton).Pressed = false;
		};
		((BaseButton)_window.ToggleLightButton).Pressed = !_light.Enabled;
		((BaseButton)_window.ToggleFovButton).Pressed = !_eye.CurrentEye.DrawFov;
		((BaseButton)_window.ToggleShadowsButton).Pressed = !_light.DrawShadows;
		((BaseButton)_window.ShowMarkersButton).Pressed = _marker.MarkersVisible;
		((BaseButton)_window.ShowBbButton).Pressed = (_debugPhysics.Flags & 4) > 0;
		((BaseButton)_window.AiOverlayButton).OnPressed += delegate(ButtonEventArgs args)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
			if (localEntity.HasValue)
			{
				NetEntity netEntity = base.EntityManager.GetNetEntity(localEntity.Value, (MetaDataComponent)null);
				if (args.Button.Pressed)
				{
					_console.ExecuteCommand($"addcomp {netEntity.Id} StationAiOverlay");
				}
				else
				{
					_console.ExecuteCommand($"rmcomp {netEntity.Id} StationAiOverlay");
				}
			}
		};
		((BaseButton)_window.RespawnButton).OnPressed += delegate
		{
			_sandbox.Respawn();
		};
		((BaseButton)_window.SpawnTilesButton).OnPressed += delegate
		{
			TileSpawningController.ToggleWindow();
		};
		((BaseButton)_window.SpawnEntitiesButton).OnPressed += delegate
		{
			EntitySpawningController.ToggleWindow();
		};
		((BaseButton)_window.SpawnDecalsButton).OnPressed += delegate
		{
			DecalPlacerController.ToggleWindow();
		};
		((BaseButton)_window.GiveFullAccessButton).OnPressed += delegate
		{
			_sandbox.GiveAdminAccess();
		};
		((BaseButton)_window.GiveAghostButton).OnPressed += delegate
		{
			_sandbox.GiveAGhost();
		};
		((BaseButton)_window.ToggleLightButton).OnToggled += delegate
		{
			_sandbox.ToggleLight();
		};
		((BaseButton)_window.ToggleFovButton).OnToggled += delegate
		{
			_sandbox.ToggleFov();
		};
		((BaseButton)_window.ToggleShadowsButton).OnToggled += delegate
		{
			_sandbox.ToggleShadows();
		};
		((BaseButton)_window.SuicideButton).OnPressed += delegate
		{
			_sandbox.Suicide();
		};
		((BaseButton)_window.ToggleSubfloorButton).OnPressed += delegate
		{
			_sandbox.ToggleSubFloor();
		};
		((BaseButton)_window.ShowMarkersButton).OnPressed += delegate
		{
			_sandbox.ShowMarkers();
		};
		((BaseButton)_window.ShowBbButton).OnPressed += delegate
		{
			_sandbox.ShowBb();
		};
	}

	private void CheckSandboxVisibility()
	{
		if (SandboxButton != null)
		{
			((Control)SandboxButton).Visible = _sandbox.SandboxAllowed;
		}
	}

	public void OnStateExited(GameplayState state)
	{
		if (_window != null)
		{
			((BaseWindow)_window).Close();
			_window = null;
		}
		CommandBinds.Unregister<SandboxSystem>();
	}

	public void OnSystemLoaded(SandboxSystem system)
	{
		system.SandboxDisabled += CloseAll;
		system.SandboxEnabled += CheckSandboxVisibility;
		system.SandboxDisabled += CheckSandboxVisibility;
	}

	public void OnSystemUnloaded(SandboxSystem system)
	{
		system.SandboxDisabled -= CloseAll;
		system.SandboxEnabled -= CheckSandboxVisibility;
		system.SandboxDisabled -= CheckSandboxVisibility;
	}

	private void SandboxButtonPressed(ButtonEventArgs args)
	{
		ToggleWindow();
	}

	private void CloseAll()
	{
		SandboxWindow? window = _window;
		if (window != null)
		{
			((BaseWindow)window).Close();
		}
		EntitySpawningController.CloseWindow();
		TileSpawningController.CloseWindow();
	}

	private bool Copy(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return _sandbox.Copy(session, coords, uid);
	}

	private void ToggleWindow()
	{
		if (_window != null)
		{
			if (_sandbox.SandboxAllowed && !((BaseWindow)_window).IsOpen)
			{
				base.UIManager.ClickSound();
				((BaseWindow)_window).Open();
			}
			else
			{
				base.UIManager.ClickSound();
				((BaseWindow)_window).Close();
			}
		}
	}

	public void SetToggleSubfloors(bool value)
	{
		if (_window != null)
		{
			((BaseButton)_window.ToggleSubfloorButton).Pressed = value;
		}
	}
}
