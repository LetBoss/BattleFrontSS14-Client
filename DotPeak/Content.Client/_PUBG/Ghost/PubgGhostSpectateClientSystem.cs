// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Ghost.PubgGhostSpectateClientSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.NightVision;
using Content.Client.Movement.Systems;
using Content.Client.UserInterface.Systems.Ghost.Widgets;
using Content.Shared._PUBG;
using Content.Shared._PUBG.Ghost;
using Content.Shared.Ghost;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.Ghost;

public sealed class PubgGhostSpectateClientSystem : EntitySystem
{
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IUserInterfaceManager _ui;
  [Dependency]
  private ContentEyeSystem _contentEye;
  [Dependency]
  private IOverlayManager _overlay;
  private bool _pubgActive;
  private int _teamSize;
  private PubgGhostFollowSelectWindow? _followWindow;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<PubgModeStatusEvent>(new EntitySessionEventHandler<PubgModeStatusEvent>(this.OnPubgModeStatus), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgTeamModeStatusEvent>(new EntitySessionEventHandler<PubgTeamModeStatusEvent>(this.OnPubgTeamStatus), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgGhostFollowTeammateOptionsEvent>(new EntitySessionEventHandler<PubgGhostFollowTeammateOptionsEvent>(this.OnFollowOptions), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PubgGhostFollowStateEvent>(new EntitySessionEventHandler<PubgGhostFollowStateEvent>(this.OnGhostFollowState), (Type[]) null, (Type[]) null);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this.CloseFollowWindow();
  }

  public void RequestFollowAllies()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgGhostFollowTeammateRequestEvent());
  }

  public void RefreshFollowButton()
  {
    this._ui.GetActiveUIWidgetOrNull<GhostGui>()?.SetFollowAlliesVisible(this.ShouldShowFollowButton());
  }

  private void OnPubgModeStatus(PubgModeStatusEvent ev, EntitySessionEventArgs args)
  {
    this._pubgActive = ev.Enabled;
    if (!ev.Enabled)
    {
      this._teamSize = 0;
      this.CloseFollowWindow();
    }
    this.RefreshFollowButton();
  }

  private void OnPubgTeamStatus(PubgTeamModeStatusEvent ev, EntitySessionEventArgs args)
  {
    this._pubgActive = ev.Enabled;
    this._teamSize = ev.TeamSize;
    if (!this.ShouldShowFollowButton())
      this.CloseFollowWindow();
    this.RefreshFollowButton();
  }

  private void OnFollowOptions(PubgGhostFollowTeammateOptionsEvent ev, EntitySessionEventArgs args)
  {
    if (!this.ShouldShowFollowButton())
    {
      this.CloseFollowWindow();
    }
    else
    {
      this.EnsureFollowWindow();
      this._followWindow.SetOptions((IReadOnlyList<PubgGhostFollowTeammateOptionState>) ev.Options);
      if (((BaseWindow) this._followWindow).IsOpen)
        return;
      ((BaseWindow) this._followWindow).OpenCentered();
    }
  }

  private void OnGhostFollowState(PubgGhostFollowStateEvent ev, EntitySessionEventArgs args)
  {
    if (!ev.Enabled)
      return;
    this.CloseFollowWindow();
    this._contentEye.RequestEye(true, true);
    this._overlay.RemoveOverlay<HalfNightVisionBrightnessOverlay>();
  }

  private bool ShouldShowFollowButton()
  {
    if (!this._pubgActive || this._teamSize <= 1)
      return false;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    return localEntity.HasValue && this.HasComp<GhostComponent>(localEntity.Value);
  }

  private void EnsureFollowWindow()
  {
    if (this._followWindow != null)
      return;
    this._followWindow = new PubgGhostFollowSelectWindow();
    this._followWindow.FollowRequested += new Action<NetEntity>(this.OnFollowRequested);
  }

  private void CloseFollowWindow()
  {
    if (this._followWindow == null)
      return;
    this._followWindow.FollowRequested -= new Action<NetEntity>(this.OnFollowRequested);
    ((BaseWindow) this._followWindow).Close();
    this._followWindow = (PubgGhostFollowSelectWindow) null;
  }

  private void OnFollowRequested(NetEntity teammate)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new PubgGhostFollowTeammateRequestEvent(new NetEntity?(teammate)));
    this.CloseFollowWindow();
  }
}
