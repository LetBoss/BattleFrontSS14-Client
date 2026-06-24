// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.BattlePass.BattlePassSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.BattlePass;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._PUBG.BattlePass;

public sealed class BattlePassSystem : EntitySystem
{
  private static readonly TimeSpan RequestDebounce = TimeSpan.FromMilliseconds(500L);
  private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(8L);
  private DateTime _lastRequestAt = DateTime.MinValue;
  private bool _requestInFlight;

  public event Action<BattlePassStateMessage>? OnStateReceived;

  public event Action<BattlePassClaimResultMessage>? OnClaimResultReceived;

  public event Action<BattlePassSkipResultMessage>? OnSkipResultReceived;

  public event Action<BattlePassClaimTaskResultMessage>? OnClaimTaskResultReceived;

  public event Action<BattlePassBuyPremiumResultMessage>? OnBuyPremiumResultReceived;

  public BattlePassStateMessage? LastState { get; private set; }

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<BattlePassStateMessage>(new EntityEventHandler<BattlePassStateMessage>(this.OnState), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<BattlePassClaimResultMessage>(new EntityEventHandler<BattlePassClaimResultMessage>(this.OnClaimResult), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<BattlePassSkipResultMessage>(new EntityEventHandler<BattlePassSkipResultMessage>(this.OnSkipResult), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<BattlePassClaimTaskResultMessage>(new EntityEventHandler<BattlePassClaimTaskResultMessage>(this.OnClaimTaskResult), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<BattlePassBuyPremiumResultMessage>(new EntityEventHandler<BattlePassBuyPremiumResultMessage>(this.OnBuyPremiumResult), (Type[]) null, (Type[]) null);
  }

  private void OnState(BattlePassStateMessage msg)
  {
    this._requestInFlight = false;
    this.LastState = msg;
    Action<BattlePassStateMessage> onStateReceived = this.OnStateReceived;
    if (onStateReceived == null)
      return;
    onStateReceived(msg);
  }

  private void OnClaimResult(BattlePassClaimResultMessage msg)
  {
    Action<BattlePassClaimResultMessage> claimResultReceived = this.OnClaimResultReceived;
    if (claimResultReceived == null)
      return;
    claimResultReceived(msg);
  }

  private void OnSkipResult(BattlePassSkipResultMessage msg)
  {
    Action<BattlePassSkipResultMessage> skipResultReceived = this.OnSkipResultReceived;
    if (skipResultReceived == null)
      return;
    skipResultReceived(msg);
  }

  private void OnClaimTaskResult(BattlePassClaimTaskResultMessage msg)
  {
    Action<BattlePassClaimTaskResultMessage> taskResultReceived = this.OnClaimTaskResultReceived;
    if (taskResultReceived == null)
      return;
    taskResultReceived(msg);
  }

  public void RequestBattlePass(bool force = false)
  {
    DateTime utcNow = DateTime.UtcNow;
    if (this._requestInFlight && utcNow - this._lastRequestAt < BattlePassSystem.RequestTimeout || !force && utcNow - this._lastRequestAt < BattlePassSystem.RequestDebounce)
      return;
    this._requestInFlight = true;
    this._lastRequestAt = utcNow;
    this.RaisePredictiveEvent<BattlePassRequestMessage>(new BattlePassRequestMessage());
  }

  public void ClaimReward(string rewardId)
  {
    this.RaisePredictiveEvent<BattlePassClaimMessage>(new BattlePassClaimMessage(rewardId));
  }

  public void SkipTask(string taskId)
  {
    this.RaisePredictiveEvent<BattlePassSkipTaskMessage>(new BattlePassSkipTaskMessage(taskId));
  }

  public void ClaimTaskXp(string taskId)
  {
    this.RaisePredictiveEvent<BattlePassClaimTaskMessage>(new BattlePassClaimTaskMessage(taskId));
  }

  private void OnBuyPremiumResult(BattlePassBuyPremiumResultMessage msg)
  {
    Action<BattlePassBuyPremiumResultMessage> premiumResultReceived = this.OnBuyPremiumResultReceived;
    if (premiumResultReceived == null)
      return;
    premiumResultReceived(msg);
  }

  public void BuyPremium()
  {
    this.RaisePredictiveEvent<BattlePassBuyPremiumMessage>(new BattlePassBuyPremiumMessage());
  }
}
