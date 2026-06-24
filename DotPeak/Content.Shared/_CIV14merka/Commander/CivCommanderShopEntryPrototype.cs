// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Commander.CivCommanderShopEntryPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._CIV14merka.Commander;

[Prototype(null, 1)]
public sealed class CivCommanderShopEntryPrototype : IPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public string Name = string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public string Description = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? IconEntity;
  [DataField(null, false, 1, false, false, null)]
  public CivCommanderShopEntryKind Kind;
  [DataField(null, false, 1, false, false, null)]
  public CivCommanderShopPurchaseType ServiceType;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? EntityPrototype;
  [DataField(null, false, 1, false, false, null)]
  public List<EntProtoId>? SquadMembers;
  [DataField(null, false, 1, false, false, null)]
  public int Price;
  [DataField(null, false, 1, false, false, null)]
  public string[] SideIds = Array.Empty<string>();
  [DataField(null, false, 1, false, false, null)]
  public float PriceAfterPurchaseMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public int PriceAfterPurchaseCooldownSeconds;
  [DataField(null, false, 1, false, false, null)]
  public int PurchaseLimitPerTeam;
  [DataField(null, false, 1, false, false, null)]
  public int Order;
  [DataField(null, false, 1, false, false, null)]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  public bool RequiresPurchaseAnchor = true;
  [DataField(null, false, 1, false, false, null)]
  public bool KeepPlacing = true;

  [IdDataField(1, null)]
  public string ID { get; private set; }

  public bool IsAvailableForSide(string? sideId)
  {
    if (this.SideIds.Length == 0)
      return true;
    if (string.IsNullOrWhiteSpace(sideId))
      return false;
    foreach (string sideId1 in this.SideIds)
    {
      if (!string.IsNullOrWhiteSpace(sideId1) && string.Equals(sideId1, sideId, StringComparison.OrdinalIgnoreCase))
        return true;
    }
    return false;
  }
}
