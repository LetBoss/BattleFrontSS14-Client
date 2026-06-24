// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Events.PubgMarsTaskInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.Events;

[NetSerializable]
[Serializable]
public sealed class PubgMarsTaskInfo
{
  public string TemplateId { get; set; } = string.Empty;

  public string TaskKey { get; set; } = string.Empty;

  public string Category { get; set; } = string.Empty;

  public string PeriodType { get; set; } = string.Empty;

  public string ObjectiveType { get; set; } = string.Empty;

  public int TargetValue { get; set; }

  public int TokenReward { get; set; }

  public int CoinsReward { get; set; }

  public int? MinSurvivalSeconds { get; set; }

  public int Progress { get; set; }

  public bool IsCompleted { get; set; }

  public bool IsClaimed { get; set; }

  public string PeriodKey { get; set; } = string.Empty;

  public bool IsClaimable { get; set; }
}
