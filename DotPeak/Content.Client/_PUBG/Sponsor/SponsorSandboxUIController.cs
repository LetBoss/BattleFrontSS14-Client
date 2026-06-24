// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Sponsor.SponsorSandboxUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.Sponsor;

public sealed class SponsorSandboxUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
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

  private SponsorSandboxSystem SponsorSandboxSystem
  {
    get => this.EntityManager.System<SponsorSandboxSystem>();
  }

  private SponsorEntitySpawningUIController SponsorEntitySpawningController
  {
    get => this.UIManager.GetUIController<SponsorEntitySpawningUIController>();
  }

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

  public void OnStateEntered(GameplayState state)
  {
    this.EnsureWindow();
    this.EnsureMenuButton();
    this.EnsureSystemSubscribed();
    this.ApplyState(this.SponsorSandboxSystem.State);
    // ISSUE: method pointer
    this._input.SetInputCommand(ContentKeyFunctions.OpenEntitySpawnWindow, InputCmdHandler.FromDelegate(new StateInputCmdDelegate((object) this, __methodptr(\u003COnStateEntered\u003Eb__16_0)), (StateInputCmdDelegate) null, true, true));
  }

  public void OnStateExited(GameplayState state)
  {
    if (this._menuButton != null)
    {
      ((Control) this._menuButton).Parent?.RemoveChild((Control) this._menuButton);
      this._menuButton = (MenuButton) null;
    }
    if (this._window != null)
    {
      ((BaseWindow) this._window).Close();
      this._window = (SponsorMenuWindow) null;
    }
    if (!this._systemSubscribed)
      return;
    SponsorSandboxSystem sponsorSandboxSystem = this.EntityManager.SystemOrNull<SponsorSandboxSystem>();
    if (sponsorSandboxSystem != null)
      sponsorSandboxSystem.OnStateUpdated -= new Action<SponsorSandboxState>(this.OnSponsorStateUpdated);
    this._systemSubscribed = false;
  }

  private void EnsureWindow()
  {
    SponsorMenuWindow window = this._window;
    if (window != null && !((Control) window).Disposed)
      return;
    this._window = this.UIManager.CreateWindow<SponsorMenuWindow>();
    ((BaseWindow) this._window).OpenCentered();
    ((BaseWindow) this._window).Close();
    ((BaseWindow) this._window).OnOpen += (Action) (() =>
    {
      if (this._menuButton == null)
        return;
      ((BaseButton) this._menuButton).Pressed = true;
    });
    ((BaseWindow) this._window).OnClose += (Action) (() =>
    {
      if (this._menuButton == null)
        return;
      ((BaseButton) this._menuButton).Pressed = false;
    });
    ((BaseButton) this._window.SpawnTabControl.SpawnEntitiesButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SponsorEntitySpawningController.ToggleWindow());
    ((BaseButton) this._window.SpawnTabControl.SpawnTilesButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.TileSpawningController.ToggleWindow());
    ((BaseButton) this._window.SpawnTabControl.SpawnDecalsButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.DecalPlacerController.ToggleWindow());
    ((BaseButton) this._window.SpawnTabControl.SponsorArenaButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SponsorSandboxSystem.RequestSponsorArenaTeleport());
    ((BaseButton) this._window.SpawnTabControl.SponsorAghostButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SponsorSandboxSystem.RequestSponsorAghost());
  }

  private void EnsureMenuButton()
  {
    if (this._menuButton != null)
      return;
    GameTopMenuBar activeUiWidgetOrNull = this.UIManager.GetActiveUIWidgetOrNull<GameTopMenuBar>();
    if (activeUiWidgetOrNull == null)
      return;
    TextureResource resource = this._resources.GetResource<TextureResource>("/Textures/Interface/sandbox.svg.192dpi.png", true);
    MenuButton menuButton = new MenuButton();
    menuButton.Icon = TextureResource.op_Implicit(resource);
    ((Control) menuButton).ToolTip = Loc.GetString("pubg-sponsor-sandbox-button-tooltip");
    ((Control) menuButton).MinSize = new Vector2(42f, 64f);
    ((Control) menuButton).HorizontalExpand = true;
    this._menuButton = menuButton;
    ((Control) this._menuButton).AddStyleClass("ButtonSquare");
    ((BaseButton) this._menuButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.SponsorSandboxButtonPressed);
    ((Control) activeUiWidgetOrNull).AddChild((Control) this._menuButton);
  }

  private void SponsorSandboxButtonPressed(BaseButton.ButtonEventArgs args) => this.ToggleWindow();

  private void ToggleWindow()
  {
    if (this._window == null || !this.IsSponsorMenuVisible(this.SponsorSandboxSystem.State))
      return;
    if (((BaseWindow) this._window).IsOpen)
      ((BaseWindow) this._window).Close();
    else
      ((BaseWindow) this._window).Open();
  }

  private void EnsureSystemSubscribed()
  {
    if (this._systemSubscribed)
      return;
    this.SponsorSandboxSystem.OnStateUpdated += new Action<SponsorSandboxState>(this.OnSponsorStateUpdated);
    this._systemSubscribed = true;
  }

  private void OnSponsorStateUpdated(SponsorSandboxState state) => this.ApplyState(state);

  private void ApplyState(SponsorSandboxState state)
  {
    bool flag = this.IsSponsorMenuVisible(state);
    if (this._menuButton != null)
      ((Control) this._menuButton).Visible = flag;
    this.SponsorEntitySpawningController.UpdatePermissions(state);
    if (!state.AllowSpawnEntities)
      this.SponsorEntitySpawningController.CloseWindow();
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    if (!flag && ((BaseWindow) this._window).IsOpen)
      ((BaseWindow) this._window).Close();
    ((BaseButton) this._window.SpawnTabControl.SpawnEntitiesButton).Disabled = !state.AllowSpawnEntities;
    ((BaseButton) this._window.SpawnTabControl.SpawnTilesButton).Disabled = !state.AllowSpawnTiles;
    ((BaseButton) this._window.SpawnTabControl.SpawnDecalsButton).Disabled = !state.AllowSpawnDecals;
    ((BaseButton) this._window.SpawnTabControl.SponsorArenaButton).Disabled = !state.AllowSponsorArena;
    ((BaseButton) this._window.SpawnTabControl.SponsorAghostButton).Disabled = !state.AllowSponsorAghost;
  }

  private bool IsSponsorMenuVisible(SponsorSandboxState state)
  {
    return state.Enabled && !state.IsMiniGameSandbox;
  }
}
