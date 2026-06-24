// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Events.PubgEventDetailInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Events;

[NetSerializable]
[Serializable]
public sealed class PubgEventDetailInfo
{
  public string EventKey { get; set; } = string.Empty;

  public string Kind { get; set; } = string.Empty;

  public string TitleKey { get; set; } = string.Empty;

  public string DescriptionKey { get; set; } = string.Empty;

  public string? SelectorIconPath { get; set; }

  public string? SelectorBannerPath { get; set; }

  public string? SelectorAccentHex { get; set; }

  public DateTime StartAt { get; set; }

  public DateTime? EndAt { get; set; }

  public int SortOrder { get; set; }

  public bool IsActive { get; set; }

  public bool HasClaimable { get; set; }

  public bool RedDotOneTime { get; set; }

  public bool RedDotTasks { get; set; }

  public bool RedDotMilestones { get; set; }

  public PubgDiscordEventStateInfo? DiscordState { get; set; }

  public PubgMarsEventStateInfo? MarsState { get; set; }
}
