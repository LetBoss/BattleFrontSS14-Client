// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Commander.PurchaseRequestEntryState
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
public sealed class PurchaseRequestEntryState
{
  public string RequestId { get; }

  public string RequesterName { get; }

  public string Faction { get; }

  public List<PurchaseRequestItemState> Items { get; }

  public int TotalPrice { get; }

  public double RequestTime { get; }

  public PurchaseRequestEntryState(
    string requestId,
    string requesterName,
    string faction,
    IEnumerable<PurchaseRequestItemState> items,
    int totalPrice,
    double requestTime)
  {
    this.RequestId = requestId;
    this.RequesterName = requesterName;
    this.Faction = faction;
    this.Items = items.ToList<PurchaseRequestItemState>();
    this.TotalPrice = totalPrice;
    this.RequestTime = requestTime;
  }
}
