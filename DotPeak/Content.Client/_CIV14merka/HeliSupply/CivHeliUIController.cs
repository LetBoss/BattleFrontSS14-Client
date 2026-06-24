// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.HeliSupply.CivHeliUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Shared._CIV14merka.HeliSupply;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client._CIV14merka.HeliSupply;

public sealed class CivHeliUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
{
  [UISystemDependency]
  private readonly CivHeliSupplySystem? _heli;
  private CivHeliWindow? _window;
  private CivHeliRoutePanel? _routePanel;
  private bool _suppressCancelOnClose;
  private int _lastEtaPointCount = -1;

  public void OnStateEntered(GameplayState state)
  {
    if (this._heli == null)
      return;
    this._heli.OnOpenReceived += new Action(this.OnOpen);
    this._heli.OnStateReceived += new Action<CivHeliStateMessage>(this.OnState);
    this._heli.OnRouteModeEnded += new Action<bool>(this.OnRouteModeEnded);
  }

  public void OnStateExited(GameplayState state)
  {
    if (this._heli != null)
    {
      this._heli.OnOpenReceived -= new Action(this.OnOpen);
      this._heli.OnStateReceived -= new Action<CivHeliStateMessage>(this.OnState);
      this._heli.OnRouteModeEnded -= new Action<bool>(this.OnRouteModeEnded);
    }
    this.CloseWindow(true);
    this.DisposeRoutePanel();
  }

  public virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    CivHeliSupplySystem heli = this._heli;
    bool flag = heli != null && heli.IsRouteMode;
    if (flag)
    {
      this.EnsureRoutePanel();
      int count = this._heli.RoutePoints.Count;
      this._routePanel?.SetPointCount(count);
      if (count != this._lastEtaPointCount)
      {
        this._lastEtaPointCount = count;
        this._routePanel?.SetEta(this._heli.EstimateEta());
      }
    }
    else
      this._lastEtaPointCount = -1;
    if (this._routePanel == null)
      return;
    ((Control) this._routePanel).Visible = flag;
  }

  private void OnOpen()
  {
    if (this._heli == null)
      return;
    if (this._window == null)
    {
      this._window = new CivHeliWindow();
      this._window.OnAddItem += (Action<string, int>) ((protoId, amount) => this._heli.RequestAdd(protoId, amount));
      this._window.OnRemoveItem += (Action<string>) (protoId => this._heli.RequestRemove(protoId));
      this._window.OnBuildRoute += new Action(this.OnBuildRoute);
      this._window.OnCancelPressed += new Action(this.OnCancelPressed);
      ((BaseWindow) this._window).OnClose += new Action(this.OnWindowClosed);
      ((BaseWindow) this._window).OpenCentered();
    }
    else if (!((BaseWindow) this._window).IsOpen)
      ((BaseWindow) this._window).OpenCentered();
    if (this._heli.LastState == null)
      return;
    this._window.UpdateState(this._heli.LastState);
  }

  private void OnState(CivHeliStateMessage state) => this._window?.UpdateState(state);

  private void OnBuildRoute()
  {
    if (this._heli == null)
      return;
    this._suppressCancelOnClose = true;
    ((BaseWindow) this._window)?.Close();
    this._heli.StartRouteMode();
  }

  private void OnCancelPressed()
  {
    this._heli?.RequestCancel();
    this._suppressCancelOnClose = true;
    ((BaseWindow) this._window)?.Close();
  }

  private void OnWindowClosed()
  {
    if (!this._suppressCancelOnClose)
      this._heli?.RequestCancel();
    this._suppressCancelOnClose = false;
    this._window = (CivHeliWindow) null;
  }

  private void OnRouteModeEnded(bool launched)
  {
    if (launched)
      return;
    this.OnOpen();
  }

  private void CloseWindow(bool silent)
  {
    if (this._window == null)
      return;
    if (silent)
      this._suppressCancelOnClose = true;
    ((BaseWindow) this._window).Close();
  }

  private void EnsureRoutePanel()
  {
    UIScreen activeScreen = this.UIManager.ActiveScreen;
    if (activeScreen == null)
      return;
    if (activeScreen.GetWidget<MainViewport>() == null)
      return;
    LayoutContainer control;
    try
    {
      control = ((Control) activeScreen).FindControl<LayoutContainer>("ViewportContainer");
    }
    catch (ArgumentException ex)
    {
      return;
    }
    if (this._routePanel == null)
    {
      this._routePanel = new CivHeliRoutePanel();
      this._routePanel.LaunchPressed += new Action(this.OnRouteLaunchPressed);
      this._routePanel.BackPressed += new Action(this.OnRouteBackPressed);
      CivHeliRoutePanel routePanel = this._routePanel;
      CivHeliSupplySystem heli = this._heli;
      int launchCost = heli != null ? heli.GetConfig().LaunchCost : 0;
      routePanel.SetCost(launchCost);
    }
    if (((Control) this._routePanel).Parent != control)
    {
      ((Control) this._routePanel).Orphan();
      ((Control) control).AddChild((Control) this._routePanel);
      LayoutContainer.SetAnchorAndMarginPreset((Control) this._routePanel, (LayoutContainer.LayoutPreset) 5, (LayoutContainer.LayoutPresetMode) 0, 8);
      LayoutContainer.SetGrowHorizontal((Control) this._routePanel, (LayoutContainer.GrowDirection) 2);
      LayoutContainer.SetGrowVertical((Control) this._routePanel, (LayoutContainer.GrowDirection) 0);
    }
    ((Control) this._routePanel).SetPositionLast();
  }

  private void OnRouteLaunchPressed() => this._heli?.FinishRoute();

  private void OnRouteBackPressed() => this._heli?.CancelRouteMode();

  private void DisposeRoutePanel()
  {
    if (this._routePanel == null)
      return;
    this._routePanel.LaunchPressed -= new Action(this.OnRouteLaunchPressed);
    this._routePanel.BackPressed -= new Action(this.OnRouteBackPressed);
    ((Control) this._routePanel).Orphan();
    this._routePanel = (CivHeliRoutePanel) null;
  }
}
