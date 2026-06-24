// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Skin.BalanceUpdateMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared._PUBG.Skin;

[NetSerializable]
[Serializable]
public sealed class BalanceUpdateMessage : EntityEventArgs
{
  public int Coins { get; }

  public int Scrap { get; }

  public int PremiumCoins { get; }

  public BalanceUpdateMessage(int coins, int scrap, int premiumCoins)
  {
    this.Coins = coins;
    this.Scrap = scrap;
    this.PremiumCoins = premiumCoins;
  }
}
