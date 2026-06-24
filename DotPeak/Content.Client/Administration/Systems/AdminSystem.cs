// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.Systems.AdminSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.Managers;
using Content.Shared.Administration;
using Content.Shared.Administration.Events;
using Content.Shared.Roles;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client.Administration.Systems;

public sealed class AdminSystem : EntitySystem
{
  private Dictionary<NetUserId, PlayerInfo>? _playerList;
  [Dependency]
  private IOverlayManager _overlayManager;
  [Dependency]
  private IResourceCache _resourceCache;
  [Dependency]
  private IClientAdminManager _adminManager;
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private IUserInterfaceManager _userInterfaceManager;
  [Dependency]
  private IConfigurationManager _configurationManager;
  [Dependency]
  private SharedRoleSystem _roles;
  [Dependency]
  private IPrototypeManager _proto;
  private AdminNameOverlay _adminNameOverlay;

  public event Action<List<PlayerInfo>>? PlayerListChanged;

  public IReadOnlyList<PlayerInfo> PlayerList
  {
    get
    {
      return this._playerList != null ? (IReadOnlyList<PlayerInfo>) this._playerList.Values.ToList<PlayerInfo>() : (IReadOnlyList<PlayerInfo>) new List<PlayerInfo>();
    }
  }

  public virtual void Initialize()
  {
    base.Initialize();
    this.InitializeOverlay();
    this.SubscribeNetworkEvent<FullPlayerListEvent>(new EntityEventHandler<FullPlayerListEvent>(this.OnPlayerListChanged), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<PlayerInfoChangedEvent>(new EntityEventHandler<PlayerInfoChangedEvent>(this.OnPlayerInfoChanged), (Type[]) null, (Type[]) null);
  }

  public virtual void Shutdown()
  {
    base.Shutdown();
    this.ShutdownOverlay();
  }

  private void OnPlayerInfoChanged(PlayerInfoChangedEvent ev)
  {
    if (ev.PlayerInfo == (PlayerInfo) null)
      return;
    if (this._playerList == null)
      this._playerList = new Dictionary<NetUserId, PlayerInfo>();
    this._playerList[ev.PlayerInfo.SessionId] = ev.PlayerInfo;
    Action<List<PlayerInfo>> playerListChanged = this.PlayerListChanged;
    if (playerListChanged == null)
      return;
    playerListChanged(this._playerList.Values.ToList<PlayerInfo>());
  }

  private void OnPlayerListChanged(FullPlayerListEvent msg)
  {
    this._playerList = msg.PlayersInfo.ToDictionary<PlayerInfo, NetUserId, PlayerInfo>((Func<PlayerInfo, NetUserId>) (x => x.SessionId), (Func<PlayerInfo, PlayerInfo>) (x => x));
    Action<List<PlayerInfo>> playerListChanged = this.PlayerListChanged;
    if (playerListChanged == null)
      return;
    playerListChanged(msg.PlayersInfo);
  }

  public event Action? OverlayEnabled;

  public event Action? OverlayDisabled;

  private void InitializeOverlay()
  {
    this._adminNameOverlay = new AdminNameOverlay(this, (IEntityManager) this.EntityManager, this._eyeManager, this._resourceCache, this._entityLookup, this._userInterfaceManager, this._configurationManager, this._roles, this._proto);
    this._adminManager.AdminStatusUpdated += new Action(this.OnAdminStatusUpdated);
  }

  private void ShutdownOverlay()
  {
    this._adminManager.AdminStatusUpdated -= new Action(this.OnAdminStatusUpdated);
  }

  private void OnAdminStatusUpdated() => this.AdminOverlayOff();

  public void AdminOverlayOn()
  {
    if (this._overlayManager.HasOverlay<AdminNameOverlay>())
      return;
    this._overlayManager.AddOverlay((Overlay) this._adminNameOverlay);
    Action overlayEnabled = this.OverlayEnabled;
    if (overlayEnabled == null)
      return;
    overlayEnabled();
  }

  public void AdminOverlayOff()
  {
    this._overlayManager.RemoveOverlay<AdminNameOverlay>();
    Action overlayDisabled = this.OverlayDisabled;
    if (overlayDisabled == null)
      return;
    overlayDisabled();
  }
}
