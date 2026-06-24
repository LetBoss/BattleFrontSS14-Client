// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Events.PubgEventClaimResultMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Events;

[NetSerializable]
[Serializable]
public sealed class PubgEventClaimResultMessage : EntityEventArgs
{
  public bool Success { get; }

  public string? Error { get; }

  public PubgEventClaimResultInfo? ClaimResult { get; }

  public int Coins { get; }

  public int Scrap { get; }

  public int PremiumCoins { get; }

  public bool HasBalances { get; }

  public List<PubgEventWalletDeltaInfo> WalletsDelta { get; }

  public bool HubHasClaimable { get; }

  public PubgEventClaimResultMessage(
    bool success,
    string? error,
    PubgEventClaimResultInfo? claimResult,
    int coins,
    int scrap,
    int premiumCoins,
    bool hasBalances,
    List<PubgEventWalletDeltaInfo> walletsDelta,
    bool hubHasClaimable)
  {
    this.Success = success;
    this.Error = error;
    this.ClaimResult = claimResult;
    this.Coins = coins;
    this.Scrap = scrap;
    this.PremiumCoins = premiumCoins;
    this.HasBalances = hasBalances;
    this.WalletsDelta = walletsDelta;
    this.HubHasClaimable = hubHasClaimable;
  }
}
