// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Skin.SkinShopOfferInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Skin;

[NetSerializable]
[Serializable]
public sealed class SkinShopOfferInfo
{
  public string OfferId { get; }

  public string Currency { get; }

  public int Price { get; }

  public int? DurationDays { get; }

  public SkinShopOfferInfo(string offerId, string currency, int price, int? durationDays)
  {
    this.OfferId = offerId;
    this.Currency = currency;
    this.Price = price;
    this.DurationDays = durationDays;
  }
}
