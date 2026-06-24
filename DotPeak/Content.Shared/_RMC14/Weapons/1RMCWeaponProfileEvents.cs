// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.RMCWeaponProfileHelloEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weapons;

[NetSerializable]
[Serializable]
public sealed class RMCWeaponProfileHelloEvent : EntityEventArgs
{
  public bool Enabled { get; }

  public int Nonce { get; }

  public float HeartbeatIntervalSeconds { get; }

  public int MaxModulesPerList { get; }

  public int MaxModuleNameLength { get; }

  public float FocusDistanceThreshold { get; }

  public int MaxProfileFrameBytes { get; }

  public string DecoyCommandName { get; }

  public string DecoyCVarName { get; }

  public int RuleSalt { get; }

  public List<int> StrictCommandRuleIds { get; }

  public List<int> SuspiciousCommandRuleIds { get; }

  public List<int> DiscoverableRootRuleIds { get; }

  public List<int> DiscoverableTypeRuleIds { get; }

  public List<string> DynamicDecoyCommands { get; }

  public RMCWeaponProfileHelloEvent(
    bool enabled,
    int nonce,
    float heartbeatIntervalSeconds,
    int maxModulesPerList,
    int maxModuleNameLength,
    float focusDistanceThreshold,
    int maxProfileFrameBytes,
    string decoyCommandName,
    string decoyCVarName,
    int ruleSalt,
    List<int>? strictCommandRuleIds,
    List<int>? suspiciousCommandRuleIds,
    List<int>? discoverableRootRuleIds,
    List<int>? discoverableTypeRuleIds,
    List<string>? dynamicDecoyCommands)
  {
    this.Enabled = enabled;
    this.Nonce = nonce;
    this.HeartbeatIntervalSeconds = heartbeatIntervalSeconds;
    this.MaxModulesPerList = maxModulesPerList;
    this.MaxModuleNameLength = maxModuleNameLength;
    this.FocusDistanceThreshold = focusDistanceThreshold;
    this.MaxProfileFrameBytes = maxProfileFrameBytes;
    this.DecoyCommandName = decoyCommandName;
    this.DecoyCVarName = decoyCVarName;
    this.RuleSalt = ruleSalt;
    this.StrictCommandRuleIds = strictCommandRuleIds ?? new List<int>();
    this.SuspiciousCommandRuleIds = suspiciousCommandRuleIds ?? new List<int>();
    this.DiscoverableRootRuleIds = discoverableRootRuleIds ?? new List<int>();
    this.DiscoverableTypeRuleIds = discoverableTypeRuleIds ?? new List<int>();
    this.DynamicDecoyCommands = dynamicDecoyCommands ?? new List<string>();
  }
}
