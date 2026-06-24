// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Gulag.GulagUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Gameplay;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared._PUBG.Gulag;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Gulag;

public sealed class GulagUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
{
  private GulagQueueHud? _queueHud;
  private GulagFightHud? _fightHud;
  private GulagSpectatorHud? _spectatorHud;
  private GulagAdminOfferWindow? _adminOfferWindow;
  private bool _inGulag;
  private bool _isFighting;
  private bool _isSpectating;
  private bool _systemSubscribed;
  private bool _queueHiddenByUser;

  public virtual void Initialize()
  {
    base.Initialize();
    this.UIManager.GetUIController<GameplayStateLoadController>().OnScreenLoad += (Action) (() =>
    {
      if (this._inGulag && !this._isFighting && !this._queueHiddenByUser)
      {
        this.EnsureQueueHud();
      }
      else
      {
        if (!this._isFighting)
          return;
        this.EnsureFightHud();
      }
    });
  }

  public void OnStateEntered(GameplayState state)
  {
    this.EnsureSystemSubscribed();
    if (this._inGulag && !this._isFighting)
    {
      this.EnsureQueueHud();
    }
    else
    {
      if (!this._isFighting)
        return;
      this.EnsureFightHud();
    }
  }

  public void OnStateExited(GameplayState state)
  {
    this.UnsubscribeFromSystem();
    this.HideQueueHud();
    this.HideFightHud();
    this.HideSpectatorHud();
    this.HideAdminOfferWindow();
  }

  private void EnsureSystemSubscribed()
  {
    if (this._systemSubscribed)
      return;
    GulagSystem gulagSystem = this.EntityManager.System<GulagSystem>();
    gulagSystem.OnGulagStatusReceived += new Action<GulagStatusEvent, EntitySessionEventArgs>(this.OnGulagStatus);
    gulagSystem.OnQueueUpdateReceived += new Action<GulagQueueUpdateEvent, EntitySessionEventArgs>(this.OnQueueUpdate);
    gulagSystem.OnFightStartReceived += new Action<GulagFightStartEvent, EntitySessionEventArgs>(this.OnFightStart);
    gulagSystem.OnFightUpdateReceived += new Action<GulagFightUpdateEvent, EntitySessionEventArgs>(this.OnFightUpdate);
    gulagSystem.OnSpectatorUpdateReceived += new Action<GulagSpectatorUpdateEvent, EntitySessionEventArgs>(this.OnSpectatorUpdate);
    gulagSystem.OnAdminOfferReceived += new Action<GulagAdminOfferEvent, EntitySessionEventArgs>(this.OnAdminOffer);
    gulagSystem.OnMapInfoReceived += new Action<GulagMapInfoEvent, EntitySessionEventArgs>(this.OnMapInfo);
    gulagSystem.OnLocalGulagMapChanged += new Action<bool>(this.OnLocalGulagMapChanged);
    this._systemSubscribed = true;
  }

  private void UnsubscribeFromSystem()
  {
    if (!this._systemSubscribed)
      return;
    GulagSystem gulagSystem = this.EntityManager.SystemOrNull<GulagSystem>();
    if (gulagSystem != null)
    {
      gulagSystem.OnGulagStatusReceived -= new Action<GulagStatusEvent, EntitySessionEventArgs>(this.OnGulagStatus);
      gulagSystem.OnQueueUpdateReceived -= new Action<GulagQueueUpdateEvent, EntitySessionEventArgs>(this.OnQueueUpdate);
      gulagSystem.OnFightStartReceived -= new Action<GulagFightStartEvent, EntitySessionEventArgs>(this.OnFightStart);
      gulagSystem.OnFightUpdateReceived -= new Action<GulagFightUpdateEvent, EntitySessionEventArgs>(this.OnFightUpdate);
      gulagSystem.OnSpectatorUpdateReceived -= new Action<GulagSpectatorUpdateEvent, EntitySessionEventArgs>(this.OnSpectatorUpdate);
      gulagSystem.OnAdminOfferReceived -= new Action<GulagAdminOfferEvent, EntitySessionEventArgs>(this.OnAdminOffer);
      gulagSystem.OnMapInfoReceived -= new Action<GulagMapInfoEvent, EntitySessionEventArgs>(this.OnMapInfo);
      gulagSystem.OnLocalGulagMapChanged -= new Action<bool>(this.OnLocalGulagMapChanged);
    }
    this._systemSubscribed = false;
  }

  private void OnGulagStatus(GulagStatusEvent msg, EntitySessionEventArgs args)
  {
    this._inGulag = msg.InGulag;
    if (this._inGulag)
    {
      if (this._isFighting || this._queueHiddenByUser)
        return;
      this.EnsureQueueHud();
    }
    else
    {
      this.HideQueueHud();
      this.HideFightHud();
      this.HideSpectatorHud();
      this._queueHiddenByUser = false;
    }
  }

  private void OnQueueUpdate(GulagQueueUpdateEvent msg, EntitySessionEventArgs args)
  {
    if (this._queueHud == null)
      return;
    this._queueHud.UpdatePosition(msg.QueuePosition, msg.TotalInQueue);
  }

  private void OnFightStart(GulagFightStartEvent msg, EntitySessionEventArgs args)
  {
    this._isFighting = true;
    this.HideQueueHud();
    this.EnsureFightHud();
  }

  private void OnFightUpdate(GulagFightUpdateEvent msg, EntitySessionEventArgs args)
  {
    if (this._fightHud == null)
      return;
    this._fightHud.UpdateFight(msg.OpponentUsername, msg.OpponentRank, msg.TimeRemaining);
  }

  private void OnSpectatorUpdate(GulagSpectatorUpdateEvent msg, EntitySessionEventArgs args)
  {
    this._isSpectating = true;
    this.EnsureSpectatorHud();
    if (this._spectatorHud == null)
      return;
    this._spectatorHud.UpdateFight(msg.Fighter1Username, msg.Fighter1Rank, msg.Fighter2Username, msg.Fighter2Rank, msg.TimeRemaining, msg.QueueSize);
  }

  private void OnAdminOffer(GulagAdminOfferEvent msg, EntitySessionEventArgs args)
  {
    this.EnsureAdminOfferWindow();
  }

  private void OnMapInfo(GulagMapInfoEvent msg, EntitySessionEventArgs args)
  {
  }

  private void OnLocalGulagMapChanged(bool onGulagMap)
  {
    if (onGulagMap)
    {
      if (this._inGulag && !this._isFighting && !this._queueHiddenByUser)
        this.EnsureQueueHud();
      else if (this._isFighting)
        this.EnsureFightHud();
      if (!this._isSpectating)
        return;
      this.EnsureSpectatorHud();
    }
    else
    {
      this._isFighting = false;
      this._isSpectating = false;
      this.HideQueueHud();
      this.HideFightHud();
      this.HideSpectatorHud();
      this.HideAdminOfferWindow();
    }
  }

  private void EnsureQueueHud()
  {
    if (this._queueHud != null)
      return;
    UIScreen activeScreen = this.UIManager.ActiveScreen;
    if (activeScreen == null)
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
    this._queueHud = new GulagQueueHud();
    this._queueHud.HideRequested += new Action(this.OnQueueHideRequested);
    ((Control) control).AddChild((Control) this._queueHud);
    LayoutContainer.SetAnchorAndMarginPreset((Control) this._queueHud, (LayoutContainer.LayoutPreset) 8, (LayoutContainer.LayoutPresetMode) 0, 0);
  }

  private void HideQueueHud()
  {
    if (this._queueHud == null)
      return;
    this._queueHud.HideRequested -= new Action(this.OnQueueHideRequested);
    if (!((Control) this._queueHud).Disposed)
      ((Control) this._queueHud).Orphan();
    this._queueHud = (GulagQueueHud) null;
  }

  private void OnQueueHideRequested()
  {
    this._queueHiddenByUser = true;
    this.HideQueueHud();
  }

  private void EnsureFightHud()
  {
    if (this._fightHud != null)
      return;
    UIScreen activeScreen = this.UIManager.ActiveScreen;
    if (activeScreen == null)
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
    this._fightHud = new GulagFightHud();
    ((Control) control).AddChild((Control) this._fightHud);
    LayoutContainer.SetAnchorAndMarginPreset((Control) this._fightHud, (LayoutContainer.LayoutPreset) 10, (LayoutContainer.LayoutPresetMode) 0, 0);
    LayoutContainer.SetMarginTop((Control) this._fightHud, 20f);
    LayoutContainer.SetGrowHorizontal((Control) this._fightHud, (LayoutContainer.GrowDirection) 2);
  }

  private void HideFightHud()
  {
    if (this._fightHud == null)
      return;
    if (!((Control) this._fightHud).Disposed)
      ((Control) this._fightHud).Orphan();
    this._fightHud = (GulagFightHud) null;
    this._isFighting = false;
  }

  private void EnsureSpectatorHud()
  {
    if (this._spectatorHud != null)
      return;
    UIScreen activeScreen = this.UIManager.ActiveScreen;
    if (activeScreen == null)
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
    this._spectatorHud = new GulagSpectatorHud();
    ((Control) control).AddChild((Control) this._spectatorHud);
    LayoutContainer.SetAnchorAndMarginPreset((Control) this._spectatorHud, (LayoutContainer.LayoutPreset) 10, (LayoutContainer.LayoutPresetMode) 0, 0);
    LayoutContainer.SetMarginTop((Control) this._spectatorHud, 20f);
    LayoutContainer.SetGrowHorizontal((Control) this._spectatorHud, (LayoutContainer.GrowDirection) 2);
  }

  private void HideSpectatorHud()
  {
    if (this._spectatorHud == null)
      return;
    if (!((Control) this._spectatorHud).Disposed)
      ((Control) this._spectatorHud).Orphan();
    this._spectatorHud = (GulagSpectatorHud) null;
    this._isSpectating = false;
  }

  private void EnsureAdminOfferWindow()
  {
    if (this._adminOfferWindow != null)
      return;
    UIScreen activeScreen = this.UIManager.ActiveScreen;
    if (activeScreen == null)
      return;
    this._adminOfferWindow = new GulagAdminOfferWindow();
    this._adminOfferWindow.OnResponse += (Action<bool>) (accepted =>
    {
      this.EntityManager.RaisePredictiveEvent<GulagAdminResponseEvent>(new GulagAdminResponseEvent(accepted));
      this.HideAdminOfferWindow();
    });
    ((Control) activeScreen).AddChild((Control) this._adminOfferWindow);
    LayoutContainer.SetAnchorAndMarginPreset((Control) this._adminOfferWindow, (LayoutContainer.LayoutPreset) 8, (LayoutContainer.LayoutPresetMode) 0, 0);
  }

  private void HideAdminOfferWindow()
  {
    if (this._adminOfferWindow == null)
      return;
    if (!((Control) this._adminOfferWindow).Disposed)
      ((Control) this._adminOfferWindow).Orphan();
    this._adminOfferWindow = (GulagAdminOfferWindow) null;
  }
}
