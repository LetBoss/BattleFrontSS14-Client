// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Commander.CivCommanderState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._CIV14merka.Commander;

[NetSerializable]
[Serializable]
public sealed class CivCommanderState
{
  public bool IsCommander { get; }

  public int TeamId { get; }

  public int Currency { get; }

  public List<CivCommanderShopEntryState> ShopEntries { get; }

  public List<CivCommanderSquadState> Squads { get; }

  public List<CivCommanderPlayerState> ReservePlayers { get; }

  public List<PurchaseRequestEntryState> PurchaseRequests { get; }

  public CivCommanderState(
    bool isCommander,
    int teamId,
    int currency,
    IEnumerable<CivCommanderShopEntryState> shopEntries,
    IEnumerable<CivCommanderSquadState> squads,
    IEnumerable<CivCommanderPlayerState> reservePlayers,
    IEnumerable<PurchaseRequestEntryState>? purchaseRequests = null)
  {
    this.IsCommander = isCommander;
    this.TeamId = teamId;
    this.Currency = currency;
    this.ShopEntries = shopEntries.ToList<CivCommanderShopEntryState>();
    this.Squads = squads.ToList<CivCommanderSquadState>();
    this.ReservePlayers = reservePlayers.ToList<CivCommanderPlayerState>();
    this.PurchaseRequests = (purchaseRequests != null ? purchaseRequests.ToList<PurchaseRequestEntryState>() : (List<PurchaseRequestEntryState>) null) ?? new List<PurchaseRequestEntryState>();
  }
}
