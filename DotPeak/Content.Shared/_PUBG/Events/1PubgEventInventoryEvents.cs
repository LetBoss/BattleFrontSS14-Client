// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Events.PubgEventInventoryStateMessage
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
public sealed class PubgEventInventoryStateMessage : EntityEventArgs
{
  public DateTime ServerNowUtc { get; }

  public List<PubgEventWalletInfo> Wallets { get; }

  public List<PubgEventInventoryAssetInfo> Assets { get; }

  public PubgEventInventoryStateMessage(
    DateTime serverNowUtc,
    List<PubgEventWalletInfo> wallets,
    List<PubgEventInventoryAssetInfo> assets)
  {
    this.ServerNowUtc = serverNowUtc;
    this.Wallets = wallets;
    this.Assets = assets;
  }
}
