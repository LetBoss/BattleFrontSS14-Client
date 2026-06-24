// Decompiled with JetBrains decompiler
// Type: Content.Shared.CriminalRecords.CriminalRecordsConsoleState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Security;
using Content.Shared.StationRecords;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.CriminalRecords;

[NetSerializable]
[Serializable]
public sealed class CriminalRecordsConsoleState : BoundUserInterfaceState
{
  public uint? SelectedKey;
  public CriminalRecord? CriminalRecord;
  public GeneralStationRecord? StationRecord;
  public SecurityStatus FilterStatus;
  public readonly Dictionary<uint, string>? RecordListing;
  public readonly StationRecordsFilter? Filter;

  public CriminalRecordsConsoleState(
    Dictionary<uint, string>? recordListing,
    StationRecordsFilter? newFilter)
  {
    this.RecordListing = recordListing;
    this.Filter = newFilter;
  }

  public CriminalRecordsConsoleState()
    : this((Dictionary<uint, string>) null, (StationRecordsFilter) null)
  {
  }

  public bool IsEmpty()
  {
    return !this.SelectedKey.HasValue && this.StationRecord == (GeneralStationRecord) null && this.CriminalRecord == (CriminalRecord) null && this.RecordListing == null;
  }
}
