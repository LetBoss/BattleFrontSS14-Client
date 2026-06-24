// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.BattlePass.BattlePassTaskInfo
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.BattlePass;

[NetSerializable]
[Serializable]
public sealed class BattlePassTaskInfo
{
  public string Id { get; set; } = string.Empty;

  public int Slot { get; set; }

  public string TaskType { get; set; } = string.Empty;

  public string NameKey { get; set; } = string.Empty;

  public int TargetValue { get; set; }

  public int XpReward { get; set; }

  public string? RequiredPerm { get; set; }

  public string? WeaponId { get; set; }

  public int Progress { get; set; }

  public bool IsCompleted { get; set; }

  public bool IsSkipped { get; set; }

  public bool XpClaimed { get; set; }
}
