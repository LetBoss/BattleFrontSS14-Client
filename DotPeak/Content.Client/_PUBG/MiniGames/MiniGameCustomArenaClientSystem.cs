// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.MiniGames.MiniGameCustomArenaClientSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.MiniGames;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.MiniGames;

public sealed class MiniGameCustomArenaClientSystem : EntitySystem
{
  private readonly List<MiniGameArenaInfo> _cachedArenas = new List<MiniGameArenaInfo>();
  private int _cachedMaxArenas;

  public event Action<List<MiniGameArenaInfo>, int>? OnArenaListUpdated;

  public event Action<string>? OnError;

  public event Action? OnUIOpen;

  public event Action? OnUIClose;

  public IReadOnlyList<MiniGameArenaInfo> CachedArenas
  {
    get => (IReadOnlyList<MiniGameArenaInfo>) this._cachedArenas;
  }

  public int CachedMaxArenas => this._cachedMaxArenas;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<MiniGameArenaListResponseMessage>(new EntityEventHandler<MiniGameArenaListResponseMessage>(this.OnArenaListResponse), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<MiniGameArenaErrorMessage>(new EntityEventHandler<MiniGameArenaErrorMessage>(this.OnErrorMessage), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<MiniGameArenaUIOpenMessage>(new EntityEventHandler<MiniGameArenaUIOpenMessage>(this.OnUIOpenMessage), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<MiniGameArenaUICloseMessage>(new EntityEventHandler<MiniGameArenaUICloseMessage>(this.OnUICloseMessage), (Type[]) null, (Type[]) null);
  }

  public void RequestEnterCustomization(string? arenaName = null)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameArenaEnterCustomizationMessage(arenaName));
  }

  public void RequestExitCustomization()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameArenaExitCustomizationMessage());
  }

  public void RequestSaveArena(string displayName, bool overwrite = false, string? existingArenaName = null)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameArenaSaveMessage(displayName, overwrite, existingArenaName));
  }

  public void RequestLoadArena(string arenaName)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameArenaLoadMessage(arenaName));
  }

  public void RequestDeleteArena(string arenaName)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameArenaDeleteMessage(arenaName));
  }

  public void RequestArenaList()
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameArenaListRequestMessage());
  }

  public void SetSpawnVisibility(bool showSpawns)
  {
    this.RaiseNetworkEvent((EntityEventArgs) new MiniGameArenaSpawnVisibilityMessage(showSpawns));
  }

  private void OnArenaListResponse(MiniGameArenaListResponseMessage msg)
  {
    this._cachedArenas.Clear();
    this._cachedArenas.AddRange((IEnumerable<MiniGameArenaInfo>) msg.Arenas);
    this._cachedMaxArenas = msg.MaxArenas;
    Action<List<MiniGameArenaInfo>, int> arenaListUpdated = this.OnArenaListUpdated;
    if (arenaListUpdated == null)
      return;
    arenaListUpdated(msg.Arenas, msg.MaxArenas);
  }

  private void OnErrorMessage(MiniGameArenaErrorMessage msg)
  {
    Action<string> onError = this.OnError;
    if (onError == null)
      return;
    onError(msg.ErrorLocKey);
  }

  private void OnUIOpenMessage(MiniGameArenaUIOpenMessage msg)
  {
    Action onUiOpen = this.OnUIOpen;
    if (onUiOpen == null)
      return;
    onUiOpen();
  }

  private void OnUICloseMessage(MiniGameArenaUICloseMessage msg)
  {
    Action onUiClose = this.OnUIClose;
    if (onUiClose == null)
      return;
    onUiClose();
  }
}
