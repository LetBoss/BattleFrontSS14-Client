// Decompiled with JetBrains decompiler
// Type: Content.Client.Launcher.LauncherConnecting
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System;

#nullable enable
namespace Content.Client.Launcher;

public sealed class LauncherConnecting : Robust.Client.State.State
{
  [Dependency]
  private IUserInterfaceManager _userInterfaceManager;
  [Dependency]
  private IClientNetManager _clientNetManager;
  [Dependency]
  private IGameController _gameController;
  [Dependency]
  private IBaseClient _baseClient;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IConfigurationManager _cfg;
  [Dependency]
  private IClipboardManager _clipboard;
  private LauncherConnectingGui? _control;
  private readonly ISawmill _sawmill = Logger.GetSawmill("launcher-ui");
  private LauncherConnecting.Page _currentPage;
  private string? _connectFailReason;

  public string? Address
  {
    get
    {
      return this._gameController.LaunchState.Ss14Address ?? this._gameController.LaunchState.ConnectAddress;
    }
  }

  public string? ConnectFailReason
  {
    get => this._connectFailReason;
    private set
    {
      this._connectFailReason = value;
      Action<string> failReasonChanged = this.ConnectFailReasonChanged;
      if (failReasonChanged == null)
        return;
      failReasonChanged(value);
    }
  }

  public string? LastDisconnectReason => this._baseClient.LastDisconnectReason;

  public LauncherConnecting.Page CurrentPage
  {
    get => this._currentPage;
    private set
    {
      this._currentPage = value;
      Action<LauncherConnecting.Page> pageChanged = this.PageChanged;
      if (pageChanged == null)
        return;
      pageChanged(value);
    }
  }

  public ClientConnectionState ConnectionState => this._clientNetManager.ClientConnectState;

  public event Action<LauncherConnecting.Page>? PageChanged;

  public event Action<string?>? ConnectFailReasonChanged;

  public event Action<ClientConnectionState>? ConnectionStateChanged;

  public event Action<NetConnectFailArgs>? ConnectFailed;

  protected virtual void Startup()
  {
    this._control = new LauncherConnectingGui(this, this._random, this._prototypeManager, this._cfg, this._clipboard);
    ((Control) this._userInterfaceManager.StateRoot).AddChild((Control) this._control);
    this._clientNetManager.ConnectFailed += new EventHandler<NetConnectFailArgs>(this.OnConnectFailed);
    this._clientNetManager.ClientConnectStateChanged += new Action<ClientConnectionState>(this.OnConnectStateChanged);
    this.CurrentPage = LauncherConnecting.Page.Connecting;
  }

  protected virtual void Shutdown()
  {
    this._control?.Orphan();
    this._clientNetManager.ConnectFailed -= new EventHandler<NetConnectFailArgs>(this.OnConnectFailed);
    this._clientNetManager.ClientConnectStateChanged -= new Action<ClientConnectionState>(this.OnConnectStateChanged);
  }

  private void OnConnectFailed(object? _, NetConnectFailArgs args)
  {
    if (args.RedialFlag)
      this.Redial();
    this.ConnectFailReason = args.Reason;
    this.CurrentPage = LauncherConnecting.Page.ConnectFailed;
    Action<NetConnectFailArgs> connectFailed = this.ConnectFailed;
    if (connectFailed == null)
      return;
    connectFailed(args);
  }

  private void OnConnectStateChanged(ClientConnectionState state)
  {
    Action<ClientConnectionState> connectionStateChanged = this.ConnectionStateChanged;
    if (connectionStateChanged == null)
      return;
    connectionStateChanged(state);
  }

  public void RetryConnect()
  {
    if (this._gameController.LaunchState.ConnectEndpoint == null)
      return;
    this._baseClient.ConnectToServer(this._gameController.LaunchState.ConnectEndpoint);
    this.CurrentPage = LauncherConnecting.Page.Connecting;
  }

  public bool Redial()
  {
    try
    {
      if (this._gameController.LaunchState.Ss14Address != null)
      {
        this._gameController.Redial(this._gameController.LaunchState.Ss14Address, (string) null);
        return true;
      }
      this._sawmill.Info("Redial not possible, no Ss14Address");
    }
    catch (Exception ex)
    {
      this._sawmill.Error($"Redial exception: {ex}");
    }
    return false;
  }

  public void Exit() => this._gameController.Shutdown("Exit button pressed");

  public void SetDisconnected() => this.CurrentPage = LauncherConnecting.Page.Disconnected;

  public enum Page : byte
  {
    Connecting,
    ConnectFailed,
    Disconnected,
  }
}
