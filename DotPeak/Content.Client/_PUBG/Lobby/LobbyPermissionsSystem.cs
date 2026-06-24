// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Lobby.LobbyPermissionsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Lobby;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.Lobby;

public sealed class LobbyPermissionsSystem : EntitySystem
{
  public Dictionary<string, int> Permissions { get; private set; } = new Dictionary<string, int>();

  public event Action<Dictionary<string, int>>? OnPermissionsReceived;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<LobbyPermissionsMessage>(new EntityEventHandler<LobbyPermissionsMessage>(this.OnLobbyPermissions), (Type[]) null, (Type[]) null);
  }

  private void OnLobbyPermissions(LobbyPermissionsMessage msg)
  {
    this.Permissions = new Dictionary<string, int>((IDictionary<string, int>) msg.Permissions);
    Action<Dictionary<string, int>> permissionsReceived = this.OnPermissionsReceived;
    if (permissionsReceived == null)
      return;
    permissionsReceived(this.Permissions);
  }

  public void RequestPermissions()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new LobbyPermissionsRequestMessage());
  }

  public void ClearPermissions() => this.Permissions.Clear();
}
