// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.BattlePass.BattlePassClaimResultMessage
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._PUBG.BattlePass;

[NetSerializable]
[Serializable]
public sealed class BattlePassClaimResultMessage : EntityEventArgs
{
  public bool Success { get; }

  public string? Error { get; }

  public string? RewardType { get; }

  public string? RewardValue { get; }

  public BattlePassClaimResultMessage(
    bool success,
    string? error = null,
    string? rewardType = null,
    string? rewardValue = null)
  {
    this.Success = success;
    this.Error = error;
    this.RewardType = rewardType;
    this.RewardValue = rewardValue;
  }
}
