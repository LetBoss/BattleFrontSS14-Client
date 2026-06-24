// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.LinkAccount.LinkAccountSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.LinkAccount;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.LinkAccount;

public sealed class LinkAccountSystem : EntitySystem
{
  public event Action<SharedRMCDisplayLobbyMessageEvent>? LobbyMessageReceived;

  public virtual void Initialize()
  {
    this.SubscribeNetworkEvent<SharedRMCDisplayLobbyMessageEvent>(new EntityEventHandler<SharedRMCDisplayLobbyMessageEvent>(this.OnDisplayLobbyMessage), (Type[]) null, (Type[]) null);
  }

  private void OnDisplayLobbyMessage(SharedRMCDisplayLobbyMessageEvent ev)
  {
    Action<SharedRMCDisplayLobbyMessageEvent> lobbyMessageReceived = this.LobbyMessageReceived;
    if (lobbyMessageReceived == null)
      return;
    lobbyMessageReceived(ev);
  }
}
