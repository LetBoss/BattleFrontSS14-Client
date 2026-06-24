// Decompiled with JetBrains decompiler
// Type: Content.Shared.StationRecords.GeneralStationRecordConsoleState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.StationRecords;

[NetSerializable]
[Serializable]
public sealed class GeneralStationRecordConsoleState : BoundUserInterfaceState
{
  public readonly uint? SelectedKey;
  public readonly GeneralStationRecord? Record;
  public readonly Dictionary<uint, string>? RecordListing;
  public readonly StationRecordsFilter? Filter;
  public readonly bool CanDeleteEntries;

  public GeneralStationRecordConsoleState(
    uint? key,
    GeneralStationRecord? record,
    Dictionary<uint, string>? recordListing,
    StationRecordsFilter? newFilter,
    bool canDeleteEntries)
  {
    this.SelectedKey = key;
    this.Record = record;
    this.RecordListing = recordListing;
    this.Filter = newFilter;
    this.CanDeleteEntries = canDeleteEntries;
  }

  public GeneralStationRecordConsoleState()
    : this(new uint?(), (GeneralStationRecord) null, (Dictionary<uint, string>) null, (StationRecordsFilter) null, false)
  {
  }

  public bool IsEmpty()
  {
    return !this.SelectedKey.HasValue && this.Record == (GeneralStationRecord) null && this.RecordListing == null;
  }
}
