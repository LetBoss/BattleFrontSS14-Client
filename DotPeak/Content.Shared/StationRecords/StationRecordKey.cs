// Decompiled with JetBrains decompiler
// Type: Content.Shared.StationRecords.StationRecordKey
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.StationRecords;

public readonly struct StationRecordKey(uint id, EntityUid originStation) : 
  IEquatable<StationRecordKey>
{
  [DataField(null, false, 1, false, false, null)]
  public readonly uint Id = id;
  [DataField("station", false, 1, false, false, null)]
  public readonly EntityUid OriginStation = originStation;
  public static StationRecordKey Invalid;

  public bool Equals(StationRecordKey other)
  {
    return (int) this.Id == (int) other.Id && this.OriginStation.Id == other.OriginStation.Id;
  }

  public override bool Equals(object? obj) => obj is StationRecordKey other && this.Equals(other);

  public override int GetHashCode()
  {
    return HashCode.Combine<uint, EntityUid>(this.Id, this.OriginStation);
  }

  public bool IsValid() => this.OriginStation.IsValid();
}
