// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Lobby.LobbyTraitItemsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Lobby;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.Lobby;

public sealed class LobbyTraitItemsSystem : EntitySystem
{
  public HashSet<string> UnlockedTraitItems { get; private set; } = new HashSet<string>();

  public event Action<HashSet<string>>? OnTraitItemsReceived;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<LobbyTraitItemsMessage>(new EntityEventHandler<LobbyTraitItemsMessage>(this.OnTraitItems), (Type[]) null, (Type[]) null);
  }

  private void OnTraitItems(LobbyTraitItemsMessage msg)
  {
    this.UnlockedTraitItems = new HashSet<string>((IEnumerable<string>) msg.UnlockedTraitItems);
    Action<HashSet<string>> traitItemsReceived = this.OnTraitItemsReceived;
    if (traitItemsReceived == null)
      return;
    traitItemsReceived(new HashSet<string>((IEnumerable<string>) this.UnlockedTraitItems));
  }

  public void RequestTraitItems()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new LobbyTraitItemsRequestMessage());
  }

  public void ClearTraitItems() => this.UnlockedTraitItems.Clear();
}
