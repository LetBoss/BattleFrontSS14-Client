// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.MainMenuSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Cases;
using Content.Shared._PUBG.Skin;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu;

public sealed class MainMenuSystem : EntitySystem
{
  private static readonly TimeSpan SkinStateRequestDebounce = TimeSpan.FromMilliseconds(500L);
  private static readonly TimeSpan SkinStateRequestTimeout = TimeSpan.FromSeconds(8L);
  private Dictionary<string, string> _cachedCurrentOutfit = new Dictionary<string, string>();
  private int _cachedCoins;
  private int _cachedScrap;
  private int _cachedPremiumCoins;
  private bool _hasCachedBalance;
  private int _cachedPlayerLevel = 1;
  private bool _hasCachedPlayerLevel;
  private DateTime _lastSkinStateRequestAt = DateTime.MinValue;
  private bool _skinStateRequestInFlight;

  public event Action<SkinOpenMessage>? OnSkinOpenReceived;

  public event Action<SkinStateMessage>? OnSkinStateReceived;

  public event Action<BalanceUpdateMessage>? OnBalanceUpdateReceived;

  public event Action<SkinProfileStateMessage>? OnSkinProfileStateReceived;

  public event Action<CaseOpenResultMessage>? OnCaseOpenResultReceived;

  public event Action<CaseOpenErrorMessage>? OnCaseOpenErrorReceived;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<SkinOpenMessage>(new EntityEventHandler<SkinOpenMessage>(this.OnSkinOpen), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<SkinStateMessage>(new EntityEventHandler<SkinStateMessage>(this.OnSkinState), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<BalanceUpdateMessage>(new EntityEventHandler<BalanceUpdateMessage>(this.OnBalanceUpdate), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<SkinProfileStateMessage>(new EntityEventHandler<SkinProfileStateMessage>(this.OnSkinProfileState), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CaseOpenResultMessage>(new EntityEventHandler<CaseOpenResultMessage>(this.OnCaseOpenResult), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<CaseOpenErrorMessage>(new EntityEventHandler<CaseOpenErrorMessage>(this.OnCaseOpenError), (Type[]) null, (Type[]) null);
  }

  private void OnSkinOpen(SkinOpenMessage msg)
  {
    Action<SkinOpenMessage> skinOpenReceived = this.OnSkinOpenReceived;
    if (skinOpenReceived == null)
      return;
    skinOpenReceived(msg);
  }

  private void OnSkinState(SkinStateMessage msg)
  {
    this._skinStateRequestInFlight = false;
    this._cachedCurrentOutfit = new Dictionary<string, string>((IDictionary<string, string>) msg.CurrentOutfit);
    this._cachedCoins = msg.PlayerCoins;
    this._cachedScrap = msg.PlayerScrap;
    this._cachedPremiumCoins = msg.PlayerPremiumCoins;
    this._hasCachedBalance = true;
    this._cachedPlayerLevel = msg.PlayerLevel;
    this._hasCachedPlayerLevel = true;
    Action<SkinStateMessage> skinStateReceived = this.OnSkinStateReceived;
    if (skinStateReceived == null)
      return;
    skinStateReceived(msg);
  }

  private void OnBalanceUpdate(BalanceUpdateMessage msg)
  {
    this._cachedCoins = msg.Coins;
    this._cachedScrap = msg.Scrap;
    this._cachedPremiumCoins = msg.PremiumCoins;
    this._hasCachedBalance = true;
    Action<BalanceUpdateMessage> balanceUpdateReceived = this.OnBalanceUpdateReceived;
    if (balanceUpdateReceived == null)
      return;
    balanceUpdateReceived(msg);
  }

  private void OnSkinProfileState(SkinProfileStateMessage msg)
  {
    Action<SkinProfileStateMessage> profileStateReceived = this.OnSkinProfileStateReceived;
    if (profileStateReceived == null)
      return;
    profileStateReceived(msg);
  }

  private void OnCaseOpenResult(CaseOpenResultMessage msg)
  {
    Action<CaseOpenResultMessage> openResultReceived = this.OnCaseOpenResultReceived;
    if (openResultReceived == null)
      return;
    openResultReceived(msg);
  }

  private void OnCaseOpenError(CaseOpenErrorMessage msg)
  {
    Action<CaseOpenErrorMessage> openErrorReceived = this.OnCaseOpenErrorReceived;
    if (openErrorReceived == null)
      return;
    openErrorReceived(msg);
  }

  public Dictionary<string, string> GetCachedCurrentOutfit()
  {
    return new Dictionary<string, string>((IDictionary<string, string>) this._cachedCurrentOutfit);
  }

  public bool TryGetCachedBalance(out int coins, out int scrap, out int premium)
  {
    coins = this._cachedCoins;
    scrap = this._cachedScrap;
    premium = this._cachedPremiumCoins;
    return this._hasCachedBalance;
  }

  public bool TryGetCachedPlayerLevel(out int level)
  {
    level = this._cachedPlayerLevel;
    return this._hasCachedPlayerLevel;
  }

  public void RequestSkinState(bool force = false)
  {
    DateTime utcNow = DateTime.UtcNow;
    if (this._skinStateRequestInFlight && utcNow - this._lastSkinStateRequestAt < MainMenuSystem.SkinStateRequestTimeout || !force && utcNow - this._lastSkinStateRequestAt < MainMenuSystem.SkinStateRequestDebounce)
      return;
    this._skinStateRequestInFlight = true;
    this._lastSkinStateRequestAt = utcNow;
    this.RaiseNetworkEvent((EntityEventArgs) new SkinOpenMessage());
  }
}
