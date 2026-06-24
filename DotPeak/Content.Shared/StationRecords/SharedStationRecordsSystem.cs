// Decompiled with JetBrains decompiler
// Type: Content.Shared.StationRecords.SharedStationRecordsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.StationRecords;

public abstract class SharedStationRecordsSystem : EntitySystem
{
  public StationRecordKey? Convert((NetEntity, uint)? input)
  {
    return input.HasValue ? new StationRecordKey?(this.Convert(input.Value)) : new StationRecordKey?();
  }

  public (NetEntity, uint)? Convert(StationRecordKey? input)
  {
    return input.HasValue ? new (NetEntity, uint)?(this.Convert(input.Value)) : new (NetEntity, uint)?();
  }

  public StationRecordKey Convert((NetEntity, uint) input)
  {
    return new StationRecordKey(input.Item2, this.GetEntity(input.Item1));
  }

  public (NetEntity, uint) Convert(StationRecordKey input)
  {
    return (this.GetNetEntity(input.OriginStation), input.Id);
  }

  public List<(NetEntity, uint)> Convert(ICollection<StationRecordKey> input)
  {
    List<(NetEntity, uint)> valueTupleList = new List<(NetEntity, uint)>(input.Count);
    foreach (StationRecordKey input1 in (IEnumerable<StationRecordKey>) input)
      valueTupleList.Add(this.Convert(input1));
    return valueTupleList;
  }

  public List<StationRecordKey> Convert(ICollection<(NetEntity, uint)> input)
  {
    List<StationRecordKey> stationRecordKeyList = new List<StationRecordKey>(input.Count);
    foreach ((NetEntity, uint) input1 in (IEnumerable<(NetEntity, uint)>) input)
      stationRecordKeyList.Add(this.Convert(input1));
    return stationRecordKeyList;
  }
}
