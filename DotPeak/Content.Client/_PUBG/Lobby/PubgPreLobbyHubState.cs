// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Lobby.PubgPreLobbyHubState
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.Lobby.UI;
using Robust.Client.UserInterface;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._PUBG.Lobby;

public sealed class PubgPreLobbyHubState : Robust.Client.State.State
{
  [Dependency]
  private IUserInterfaceManager _userInterfaceManager;

  public PubgPreLobbyHubGui? Hub { get; private set; }

  protected virtual Type? LinkedScreenType => typeof (PubgPreLobbyHubGui);

  protected virtual void Startup()
  {
    this.Hub = (PubgPreLobbyHubGui) this._userInterfaceManager.ActiveScreen;
  }

  protected virtual void Shutdown() => this.Hub = (PubgPreLobbyHubGui) null;
}
