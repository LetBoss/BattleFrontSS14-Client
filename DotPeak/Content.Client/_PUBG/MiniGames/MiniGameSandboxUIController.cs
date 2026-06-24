// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.MiniGames.MiniGameSandboxUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.MiniGames;

public sealed class MiniGameSandboxUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
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

  private SponsorSandboxSystem SponsorSandboxSystem
  {
    get => this.EntityManager.System<SponsorSandboxSystem>();
  }

  private MiniGameCustomArenaClientSystem ArenaSystem
  {
    get => this.EntityManager.System<MiniGameCustomArenaClientSystem>();
  }

  private SponsorEntitySpawningUIController SponsorEntitySpawningController
  {
    get => this.UIManager.GetUIController<SponsorEntitySpawningUIController>();
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
    this._customizationActive = false;
    this.EnsureWindow();
    this.EnsureSandboxStateSubscribed();
    this.EnsureArenaSystemSubscribed();
    if (this._menuButton == null)
      return;
    ((Control) this._menuButton).Visible = false;
  }

  public void OnStateExited(GameplayState state)
  {
    this._customizationActive = false;
    if (this._menuButton != null)
    {
      ((Control) this._menuButton).Parent?.RemoveChild((Control) this._menuButton);
      this._menuButton = (MenuButton) null;
    }
    if (this._window != null)
    {
      ((BaseWindow) this._window).Close();
      this._window = (MiniGameSandboxMenuWindow) null;
    }
    if (this._sandboxStateSubscribed)
    {
      SponsorSandboxSystem sponsorSandboxSystem = this.EntityManager.SystemOrNull<SponsorSandboxSystem>();
      if (sponsorSandboxSystem != null)
        sponsorSandboxSystem.OnStateUpdated -= new Action<SponsorSandboxState>(this.OnSandboxStateUpdated);
      this._sandboxStateSubscribed = false;
    }
    if (!this._arenaSystemSubscribed)
      return;
    MiniGameCustomArenaClientSystem arenaClientSystem = this.EntityManager.SystemOrNull<MiniGameCustomArenaClientSystem>();
    if (arenaClientSystem != null)
    {
      arenaClientSystem.OnArenaListUpdated -= new Action<List<MiniGameArenaInfo>, int>(this.OnArenaListUpdated);
      arenaClientSystem.OnError -= new Action<string>(this.OnArenaError);
      arenaClientSystem.OnUIOpen -= new Action(this.OnArenaUIOpen);
      arenaClientSystem.OnUIClose -= new Action(this.OnArenaUIClose);
    }
    this._arenaSystemSubscribed = false;
  }

  private void EnsureWindow()
  {
    MiniGameSandboxMenuWindow window = this._window;
    if (window != null && !((Control) window).Disposed)
      return;
    this._window = this.UIManager.CreateWindow<MiniGameSandboxMenuWindow>();
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
    ((BaseButton) this._window.SandboxTabControl.SpawnEntitiesButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      if (!this.SponsorSandboxSystem.State.AllowSpawnEntities)
        return;
      this.SponsorEntitySpawningController.ToggleWindow();
    });
    ((BaseButton) this._window.SandboxTabControl.SpawnTilesButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.TileSpawningController.ToggleWindow());
    ((BaseButton) this._window.SandboxTabControl.SpawnDecalsButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.DecalPlacerController.ToggleWindow());
    ((BaseButton) this._window.SandboxTabControl.ShowSpawnsButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ToggleShowSpawns());
    this._window.ArenaTabControl.OnShowSpawnsToggle += new Action(this.ToggleShowSpawns);
    this._window.ArenaTabControl.OnExitCustomization += (Action) (() => this.ArenaSystem.RequestExitCustomization());
    this._window.ArenaTabControl.OnSaveArena += (Action<string, bool, string>) ((displayName, overwrite, existingName) => this.ArenaSystem.RequestSaveArena(displayName, overwrite, existingName));
    this._window.ArenaTabControl.OnDeleteArena += (Action<string>) (arenaName => this.ArenaSystem.RequestDeleteArena(arenaName));
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
    ((Control) menuButton).ToolTip = Loc.GetString("pubg-mini-sandbox-button-tooltip");
    ((Control) menuButton).MinSize = new Vector2(42f, 64f);
    ((Control) menuButton).HorizontalExpand = true;
    ((Control) menuButton).Visible = this._customizationActive;
    this._menuButton = menuButton;
    ((Control) this._menuButton).AddStyleClass("ButtonSquare");
    ((BaseButton) this._menuButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.OnSandboxButtonPressed);
    ((Control) activeUiWidgetOrNull).AddChild((Control) this._menuButton);
  }

  private void OnSandboxButtonPressed(BaseButton.ButtonEventArgs args) => this.ToggleWindow();

  private void ToggleWindow()
  {
    if (this._window == null || !this._customizationActive)
      return;
    if (((BaseWindow) this._window).IsOpen)
      ((BaseWindow) this._window).Close();
    else
      ((BaseWindow) this._window).Open();
  }

  private void EnsureSandboxStateSubscribed()
  {
    if (this._sandboxStateSubscribed)
      return;
    this.SponsorSandboxSystem.OnStateUpdated += new Action<SponsorSandboxState>(this.OnSandboxStateUpdated);
    this._sandboxStateSubscribed = true;
  }

  private void EnsureArenaSystemSubscribed()
  {
    if (this._arenaSystemSubscribed)
      return;
    this.ArenaSystem.OnArenaListUpdated += new Action<List<MiniGameArenaInfo>, int>(this.OnArenaListUpdated);
    this.ArenaSystem.OnError += new Action<string>(this.OnArenaError);
    this.ArenaSystem.OnUIOpen += new Action(this.OnArenaUIOpen);
    this.ArenaSystem.OnUIClose += new Action(this.OnArenaUIClose);
    this._arenaSystemSubscribed = true;
  }

  private void OnSandboxStateUpdated(SponsorSandboxState state) => this.ApplySandboxState(state);

  private void ApplySandboxState(SponsorSandboxState state)
  {
    this.SponsorEntitySpawningController.UpdatePermissions(state);
    if (!state.AllowSpawnEntities)
      this.SponsorEntitySpawningController.CloseWindow();
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    ((BaseButton) this._window.SandboxTabControl.SpawnEntitiesButton).Disabled = !state.AllowSpawnEntities;
    ((BaseButton) this._window.SandboxTabControl.SpawnTilesButton).Disabled = !state.AllowSpawnTiles;
    ((BaseButton) this._window.SandboxTabControl.SpawnDecalsButton).Disabled = !state.AllowSpawnDecals;
  }

  private void OnArenaListUpdated(List<MiniGameArenaInfo> arenas, int maxArenas)
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    this._window.ArenaTabControl.UpdateArenaList(arenas, maxArenas);
  }

  private void OnArenaError(string errorLocKey)
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    this._window.ArenaTabControl.ShowError(errorLocKey);
  }

  private void OnArenaUIOpen()
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    this._customizationActive = true;
    this.EnsureMenuButton();
    if (this._menuButton != null)
      ((Control) this._menuButton).Visible = true;
    ((BaseWindow) this._window).Open();
    this.ApplySandboxState(this.SponsorSandboxSystem.State);
    this.ArenaSystem.RequestArenaList();
    this.SyncSpawnButton();
  }

  private void OnArenaUIClose()
  {
    this._customizationActive = false;
    if (this._menuButton != null)
    {
      ((Control) this._menuButton).Visible = false;
      ((BaseButton) this._menuButton).Pressed = false;
    }
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    ((BaseWindow) this._window).Close();
    this._marker.MarkersVisible = false;
    this._showingSpawns = false;
    this.SyncSpawnButton();
  }

  private void ToggleShowSpawns()
  {
    this._showingSpawns = !this._showingSpawns;
    this._marker.MarkersVisible = this._showingSpawns;
    this.SyncSpawnButton();
  }

  private void SyncSpawnButton()
  {
    if (this._window == null || ((Control) this._window).Disposed)
      return;
    this._window.SandboxTabControl.ShowSpawnsButton.Text = this._showingSpawns ? Loc.GetString("pubg-arena-hide-spawns") : Loc.GetString("pubg-arena-show-spawns");
    this._window.ArenaTabControl.SyncShowSpawns(this._showingSpawns);
  }
}
