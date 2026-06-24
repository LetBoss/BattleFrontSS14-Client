// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Ghost.GhostUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Ghost;
using Content.Client._CIV14merka.Lobby;
using Content.Client._CIV14merka.Lobby.UI;
using Content.Client._PUBG.Ghost;
using Content.Client.Ghost;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Client.UserInterface.Systems.Ghost.Controls;
using Content.Client.UserInterface.Systems.Ghost.Widgets;
using Content.Shared._CIV14merka;
using Content.Shared._CIV14merka.Ghost;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Ghost;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.UserInterface.Systems.Ghost;

public sealed class GhostUIController : 
  UIController,
  IOnSystemChanged<GhostSystem>,
  IOnSystemLoaded<GhostSystem>,
  IOnSystemUnloaded<GhostSystem>,
  IOnSystemChanged<CivRosterSystem>,
  IOnSystemLoaded<CivRosterSystem>,
  IOnSystemUnloaded<CivRosterSystem>
{
  [Dependency]
  private IEntityNetworkManager _net;
  [Dependency]
  private IPlayerManager _player;
  [UISystemDependency]
  private readonly GhostSystem? _system;
  private CivLeaveRoundWarningWindow? _civLeaveRoundWarningWindow;
  private CivGhostChangeClassWindow? _civChangeClassWindow;

  private GhostGui? Gui => this.UIManager.GetActiveUIWidgetOrNull<GhostGui>();

  public virtual void Initialize()
  {
    base.Initialize();
    GameplayStateLoadController uiController = this.UIManager.GetUIController<GameplayStateLoadController>();
    uiController.OnScreenLoad += new Action(this.OnScreenLoad);
    uiController.OnScreenUnload += new Action(this.OnScreenUnload);
  }

  private void OnScreenLoad() => this.LoadGui();

  private void OnScreenUnload() => this.UnloadGui();

  public void OnSystemLoaded(GhostSystem system)
  {
    system.PlayerRemoved += new Action<GhostComponent>(this.OnPlayerRemoved);
    system.PlayerUpdated += new Action<GhostComponent>(this.OnPlayerUpdated);
    system.PlayerAttached += new Action<GhostComponent>(this.OnPlayerAttached);
    system.PlayerDetached += new Action(this.OnPlayerDetached);
    system.GhostWarpsResponse += new Action<GhostWarpsResponseEvent>(this.OnWarpsResponse);
    system.GhostRoleCountUpdated += new Action<GhostUpdateGhostRoleCountEvent>(this.OnRoleCountUpdated);
  }

  public void OnSystemUnloaded(GhostSystem system)
  {
    system.PlayerRemoved -= new Action<GhostComponent>(this.OnPlayerRemoved);
    system.PlayerUpdated -= new Action<GhostComponent>(this.OnPlayerUpdated);
    system.PlayerAttached -= new Action<GhostComponent>(this.OnPlayerAttached);
    system.PlayerDetached -= new Action(this.OnPlayerDetached);
    system.GhostWarpsResponse -= new Action<GhostWarpsResponseEvent>(this.OnWarpsResponse);
    system.GhostRoleCountUpdated -= new Action<GhostUpdateGhostRoleCountEvent>(this.OnRoleCountUpdated);
  }

  public void OnSystemLoaded(CivRosterSystem system)
  {
    system.StateUpdated += new Action<CivRosterStateEvent>(this.OnCivRosterStateUpdated);
  }

  public void OnSystemUnloaded(CivRosterSystem system)
  {
    system.StateUpdated -= new Action<CivRosterStateEvent>(this.OnCivRosterStateUpdated);
  }

  private void OnCivRosterStateUpdated(CivRosterStateEvent state) => this.UpdateGui();

  public void UpdateGui()
  {
    if (this.Gui == null)
      return;
    if (this.IsCommanderGhost())
    {
      this.Gui.Hide();
    }
    else
    {
      GhostGui gui = this.Gui;
      GhostSystem system = this._system;
      int num = system != null ? (system.IsGhost ? 1 : 0) : 0;
      ((Control) gui).Visible = num != 0;
      this.Gui.Update(this._system?.AvailableGhostRoleCount, this._system?.Player?.CanReturnToBody);
      CivRosterStateEvent state = this.EntityManager.System<CivRosterSystem>().GetState();
      this.Gui.SetTdmGhostButtonsVisible(state.RoundMode != Civ14RoundMode.PointCapture);
      this.Gui.SetCivChangeClassVisible(state.RoundInProgress || state.LateJoinActive);
    }
  }

  private void OnPlayerRemoved(GhostComponent component) => this.Gui?.Hide();

  private void OnPlayerUpdated(GhostComponent component) => this.UpdateGui();

  private void OnPlayerAttached(GhostComponent component)
  {
    if (this.Gui == null)
      return;
    if (this.IsCommanderGhost())
    {
      this.Gui.Hide();
    }
    else
    {
      ((Control) this.Gui).Visible = true;
      this.UpdateGui();
      this.EntityManager.System<PubgGhostSpectateClientSystem>().RefreshFollowButton();
    }
  }

  private void OnPlayerDetached()
  {
    this.Gui?.Hide();
    this.EntityManager.System<PubgGhostSpectateClientSystem>().RefreshFollowButton();
  }

  private void OnWarpsResponse(GhostWarpsResponseEvent msg)
  {
    GhostTargetWindow targetWindow = this.Gui?.TargetWindow;
    if (targetWindow == null)
      return;
    targetWindow.UpdateWarps((IEnumerable<GhostWarp>) msg.Warps);
    targetWindow.Populate();
  }

  private void OnRoleCountUpdated(GhostUpdateGhostRoleCountEvent msg) => this.UpdateGui();

  private void OnWarpClicked(NetEntity player)
  {
    this._net.SendSystemNetworkMessage((EntityEventArgs) new GhostWarpToTargetRequestEvent(player), true);
  }

  private void OnGhostnadoClicked()
  {
    this._net.SendSystemNetworkMessage((EntityEventArgs) new GhostnadoRequestEvent(), true);
  }

  public void LoadGui()
  {
    if (this.Gui == null)
      return;
    this.Gui.RequestWarpsPressed += new Action(this.RequestWarps);
    this.Gui.ReturnToBodyPressed += new Action(this.ReturnToBody);
    this.Gui.GhostRolesPressed += new Action(this.GhostRolesPressed);
    this.Gui.GhostBarPressed += new Action(this.GhostBarPressed);
    this.Gui.GhostLobbyPressed += new Action(this.GhostLobbyPressed);
    this.Gui.GhostFollowAlliesPressed += new Action(this.GhostFollowAlliesPressed);
    this.Gui.CivChangeClassPressed += new Action(this.OpenCivChangeClassWindow);
    this.Gui.TargetWindow.WarpClicked += new Action<NetEntity>(this.OnWarpClicked);
    this.Gui.TargetWindow.OnGhostnadoClicked += new Action(this.OnGhostnadoClicked);
    this.EntityManager.System<PubgGhostSpectateClientSystem>().RefreshFollowButton();
    this.UpdateGui();
  }

  public void UnloadGui()
  {
    if (this.Gui == null)
    {
      if (this._civLeaveRoundWarningWindow == null)
        return;
      this._civLeaveRoundWarningWindow.ConfirmPressed -= new Action(this.ConfirmCivLeaveRound);
      ((BaseWindow) this._civLeaveRoundWarningWindow).OnClose -= new Action(this.OnCivLeaveRoundWarningClosed);
      ((BaseWindow) this._civLeaveRoundWarningWindow).Close();
      this._civLeaveRoundWarningWindow = (CivLeaveRoundWarningWindow) null;
    }
    else
    {
      this.Gui.RequestWarpsPressed -= new Action(this.RequestWarps);
      this.Gui.ReturnToBodyPressed -= new Action(this.ReturnToBody);
      this.Gui.GhostRolesPressed -= new Action(this.GhostRolesPressed);
      this.Gui.GhostBarPressed -= new Action(this.GhostBarPressed);
      this.Gui.GhostLobbyPressed -= new Action(this.GhostLobbyPressed);
      this.Gui.GhostFollowAlliesPressed -= new Action(this.GhostFollowAlliesPressed);
      this.Gui.CivChangeClassPressed -= new Action(this.OpenCivChangeClassWindow);
      this.Gui.TargetWindow.WarpClicked -= new Action<NetEntity>(this.OnWarpClicked);
      this.Gui.Hide();
      if (this._civLeaveRoundWarningWindow == null)
        return;
      this._civLeaveRoundWarningWindow.ConfirmPressed -= new Action(this.ConfirmCivLeaveRound);
      ((BaseWindow) this._civLeaveRoundWarningWindow).OnClose -= new Action(this.OnCivLeaveRoundWarningClosed);
      ((BaseWindow) this._civLeaveRoundWarningWindow).Close();
      this._civLeaveRoundWarningWindow = (CivLeaveRoundWarningWindow) null;
    }
  }

  private void ReturnToBody() => this._system?.ReturnToBody();

  private void RequestWarps()
  {
    this._system?.RequestWarps();
    this.Gui?.TargetWindow.Populate();
    ((BaseWindow) this.Gui?.TargetWindow).OpenCentered();
  }

  private void GhostRolesPressed() => this._system?.OpenGhostRoles();

  private void GhostBarPressed() => this._system?.RequestGhostBar();

  private void GhostLobbyPressed()
  {
    if (this.ShouldWarnOnCivLobbyReturn())
      this.OpenCivLeaveRoundWarning();
    else
      this._system?.RequestLobbyRespawn();
  }

  private void GhostFollowAlliesPressed()
  {
    this.EntityManager.System<PubgGhostSpectateClientSystem>().RequestFollowAllies();
  }

  private bool IsCommanderGhost()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CivTeamMemberComponent teamMemberComponent;
    return localEntity.HasValue && this.EntityManager.TryGetComponent<CivTeamMemberComponent>(localEntity.GetValueOrDefault(), ref teamMemberComponent) && teamMemberComponent.IsCommander;
  }

  private bool ShouldWarnOnCivLobbyReturn()
  {
    CivRosterStateEvent state = this.EntityManager.System<CivRosterSystem>().GetState();
    return state.LateJoinActive && state.HasParticipatedInCurrentRound && !state.RejoinBlockedForCurrentRound;
  }

  private void OpenCivLeaveRoundWarning()
  {
    if (this._civLeaveRoundWarningWindow == null)
    {
      this._civLeaveRoundWarningWindow = new CivLeaveRoundWarningWindow();
      this._civLeaveRoundWarningWindow.ConfirmPressed += new Action(this.ConfirmCivLeaveRound);
      ((BaseWindow) this._civLeaveRoundWarningWindow).OnClose += new Action(this.OnCivLeaveRoundWarningClosed);
    }
    ((BaseWindow) this._civLeaveRoundWarningWindow).OpenCentered();
  }

  private void ConfirmCivLeaveRound() => this._system?.RequestLobbyRespawn();

  private void OnCivLeaveRoundWarningClosed()
  {
    if (this._civLeaveRoundWarningWindow == null)
      return;
    this._civLeaveRoundWarningWindow.ConfirmPressed -= new Action(this.ConfirmCivLeaveRound);
    ((BaseWindow) this._civLeaveRoundWarningWindow).OnClose -= new Action(this.OnCivLeaveRoundWarningClosed);
    this._civLeaveRoundWarningWindow = (CivLeaveRoundWarningWindow) null;
  }

  private void OpenCivChangeClassWindow()
  {
    CivRosterStateEvent state = this.EntityManager.System<CivRosterSystem>().GetState();
    if (this._civChangeClassWindow == null || ((Control) this._civChangeClassWindow).Disposed)
    {
      this._civChangeClassWindow = new CivGhostChangeClassWindow();
      this._civChangeClassWindow.ClassSelected -= new Action<CivTdmClass>(this.SendCivChangeClass);
      this._civChangeClassWindow.ClassSelected += new Action<CivTdmClass>(this.SendCivChangeClass);
      this._civChangeClassWindow.RefreshRequested += new Action(this.RefreshCivChangeClassWindow);
    }
    this._civChangeClassWindow.Populate(state);
    ((BaseWindow) this._civChangeClassWindow).OpenCentered();
  }

  private void RefreshCivChangeClassWindow()
  {
    if (this._civChangeClassWindow == null || ((Control) this._civChangeClassWindow).Disposed)
      return;
    this._civChangeClassWindow.Populate(this.EntityManager.System<CivRosterSystem>().GetState());
  }

  private void SendCivChangeClass(CivTdmClass cls)
  {
    this._net.SendSystemNetworkMessage((EntityEventArgs) new CivGhostChangeClassRequestEvent(cls), true);
  }
}
