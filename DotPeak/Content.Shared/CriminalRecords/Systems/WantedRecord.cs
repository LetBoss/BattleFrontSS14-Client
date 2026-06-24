// Decompiled with JetBrains decompiler
// Type: Content.Shared.CriminalRecords.Systems.WantedRecord
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Security;
using Content.Shared.StationRecords;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.CriminalRecords.Systems;

[NetSerializable]
[Serializable]
public struct WantedRecord(
  GeneralStationRecord targetInfo,
  SecurityStatus status,
  string? reason,
  string? initiator,
  List<CrimeHistory> history)
{
  public GeneralStationRecord TargetInfo = targetInfo;
  public SecurityStatus Status = status;
  public string? Reason = reason;
  public string? Initiator = initiator;
  public List<CrimeHistory> History = history;
}
