// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Sandbox.SandboxUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;

#nullable enable
namespace Content.Client.UserInterface.Systems.Sandbox;

public sealed class SandboxUIController : 
  UIController,
  IOnStateChanged<GameplayState>,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>,
  IOnSystemChanged<SandboxSystem>,
  IOnSystemLoaded<SandboxSystem>,
  IOnSystemUnloaded<SandboxSystem>
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

  private EntitySpawningUIController EntitySpawningController
  {
    get => this.UIManager.GetUIController<EntitySpawningUIController>();
  }

  private TileSpawningUIController TileSpawningController
  {
    get => this.UIManager.GetUIController<TileSpawningUIController>();
  }

  private DecalPlacerUIController DecalPlacerController
  {
    get => this.UIManager.GetUIController<DecalPlacerUIController>();
  }

  private MenuButton? SandboxButton
  {
    get => this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>()?.SandboxButton;
  }

  public void OnStateEntered(GameplayState state)
  {
    this.EnsureWindow();
    this.CheckSandboxVisibility();
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.OpenEntitySpawnWindow, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003COnStateEntered\u003Eb__18_0)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.OpenSandboxWindow, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003COnStateEntered\u003Eb__18_1)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.OpenTileSpawnWindow, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003COnStateEntered\u003Eb__18_2)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.OpenDecalSpawnWindow, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003COnStateEntered\u003Eb__18_3)), (StateInputCmdDelegate) null, true, true));
    // ISSUE: method pointer
    CommandBinds.Builder.Bind(ContentKeyFunctions.EditorCopyObject, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate((object) this, __methodptr(Copy)), true, false)).Register<SandboxSystem>();
  }

  public void UnloadButton()
  {
    if (this.SandboxButton == null)
      return;
    ((BaseButton) this.SandboxButton).OnPressed -= new Action<BaseButton.ButtonEventArgs>(this.SandboxButtonPressed);
  }

  public void LoadButton()
  {
    if (this.SandboxButton == null)
      return;
    ((BaseButton) this.SandboxButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.SandboxButtonPressed);
  }

  private void EnsureWindow()
  {
    SandboxWindow window = this._window;
    if (window != null && !((Control) window).Disposed)
      return;
    this._window = this.UIManager.CreateWindow<SandboxWindow>();
    ((BaseWindow) this._window).OpenCentered();
    ((BaseWindow) this._window).Close();
    ((BaseWindow) this._window).OnOpen += (Action) (() => ((BaseButton) this.SandboxButton).Pressed = true);
    ((BaseWindow) this._window).OnClose += (Action) (() => ((BaseButton) this.SandboxButton).Pressed = false);
    ((BaseButton) this._window.ToggleLightButton).Pressed = !this._light.Enabled;
    ((BaseButton) this._window.ToggleFovButton).Pressed = !this._eye.CurrentEye.DrawFov;
    ((BaseButton) this._window.ToggleShadowsButton).Pressed = !this._light.DrawShadows;
    ((BaseButton) this._window.ShowMarkersButton).Pressed = this._marker.MarkersVisible;
    ((BaseButton) this._window.ShowBbButton).Pressed = (this._debugPhysics.Flags & 4) > 0;
    ((BaseButton) this._window.AiOverlayButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (args =>
    {
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      if (!localEntity.HasValue)
        return;
      NetEntity netEntity = this.EntityManager.GetNetEntity(localEntity.Value, (MetaDataComponent) null);
      if (args.Button.Pressed)
        this._console.ExecuteCommand($"addcomp {netEntity.Id} StationAiOverlay");
      else
        this._console.ExecuteCommand($"rmcomp {netEntity.Id} StationAiOverlay");
    });
    ((BaseButton) this._window.RespawnButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._sandbox.Respawn());
    ((BaseButton) this._window.SpawnTilesButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.TileSpawningController.ToggleWindow());
    ((BaseButton) this._window.SpawnEntitiesButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.EntitySpawningController.ToggleWindow());
    ((BaseButton) this._window.SpawnDecalsButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.DecalPlacerController.ToggleWindow());
    ((BaseButton) this._window.GiveFullAccessButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._sandbox.GiveAdminAccess());
    ((BaseButton) this._window.GiveAghostButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._sandbox.GiveAGhost());
    ((BaseButton) this._window.ToggleLightButton).OnToggled += (Action<BaseButton.ButtonToggledEventArgs>) (_ => this._sandbox.ToggleLight());
    ((BaseButton) this._window.ToggleFovButton).OnToggled += (Action<BaseButton.ButtonToggledEventArgs>) (_ => this._sandbox.ToggleFov());
    ((BaseButton) this._window.ToggleShadowsButton).OnToggled += (Action<BaseButton.ButtonToggledEventArgs>) (_ => this._sandbox.ToggleShadows());
    ((BaseButton) this._window.SuicideButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._sandbox.Suicide());
    ((BaseButton) this._window.ToggleSubfloorButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._sandbox.ToggleSubFloor());
    ((BaseButton) this._window.ShowMarkersButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._sandbox.ShowMarkers());
    ((BaseButton) this._window.ShowBbButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this._sandbox.ShowBb());
  }

  private void CheckSandboxVisibility()
  {
    if (this.SandboxButton == null)
      return;
    ((Control) this.SandboxButton).Visible = this._sandbox.SandboxAllowed;
  }

  public void OnStateExited(GameplayState state)
  {
    if (this._window != null)
    {
      ((BaseWindow) this._window).Close();
      this._window = (SandboxWindow) null;
    }
    CommandBinds.Unregister<SandboxSystem>();
  }

  public void OnSystemLoaded(SandboxSystem system)
  {
    system.SandboxDisabled += new Action(this.CloseAll);
    system.SandboxEnabled += new Action(this.CheckSandboxVisibility);
    system.SandboxDisabled += new Action(this.CheckSandboxVisibility);
  }

  public void OnSystemUnloaded(SandboxSystem system)
  {
    system.SandboxDisabled -= new Action(this.CloseAll);
    system.SandboxEnabled -= new Action(this.CheckSandboxVisibility);
    system.SandboxDisabled -= new Action(this.CheckSandboxVisibility);
  }

  private void SandboxButtonPressed(BaseButton.ButtonEventArgs args) => this.ToggleWindow();

  private void CloseAll()
  {
    ((BaseWindow) this._window)?.Close();
    this.EntitySpawningController.CloseWindow();
    this.TileSpawningController.CloseWindow();
  }

  private bool Copy(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
  {
    return this._sandbox.Copy(session, coords, uid);
  }

  private void ToggleWindow()
  {
    if (this._window == null)
      return;
    if (this._sandbox.SandboxAllowed && !((BaseWindow) this._window).IsOpen)
    {
      this.UIManager.ClickSound();
      ((BaseWindow) this._window).Open();
    }
    else
    {
      this.UIManager.ClickSound();
      ((BaseWindow) this._window).Close();
    }
  }

  public void SetToggleSubfloors(bool value)
  {
    if (this._window == null)
      return;
    ((BaseButton) this._window.ToggleSubfloorButton).Pressed = value;
  }
}
