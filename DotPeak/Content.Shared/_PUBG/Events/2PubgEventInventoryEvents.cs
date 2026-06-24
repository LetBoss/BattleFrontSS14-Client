// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Events.PubgEventInventoryAssetInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Events;

[NetSerializable]
[Serializable]
public sealed class PubgEventInventoryAssetInfo
{
  public string AssetKey { get; set; } = string.Empty;

  public string AssetType { get; set; } = string.Empty;

  public string TitleKey { get; set; } = string.Empty;

  public string? DescKey { get; set; }

  public string? IconPath { get; set; }

  public int Quantity { get; set; }

  public DateTime? ExpiresAt { get; set; }

  public bool IsExpired { get; set; }

  public string MetadataJson { get; set; } = "{}";
}
