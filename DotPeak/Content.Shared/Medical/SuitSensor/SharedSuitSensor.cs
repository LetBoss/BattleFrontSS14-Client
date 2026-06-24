// Decompiled with JetBrains decompiler
// Type: Content.Shared.Medical.SuitSensor.SuitSensorStatus
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Medical.SuitSensor;

[NetSerializable]
[Serializable]
public sealed class SuitSensorStatus
{
  public TimeSpan Timestamp;
  public NetEntity SuitSensorUid;
  public NetEntity OwnerUid;
  public string Name;
  public string Job;
  public string JobIcon;
  public List<string> JobDepartments;
  public bool IsAlive;
  public int? TotalDamage;
  public int? TotalDamageThreshold;
  public NetCoordinates? Coordinates;

  public SuitSensorStatus(
    NetEntity ownerUid,
    NetEntity suitSensorUid,
    string name,
    string job,
    string jobIcon,
    List<string> jobDepartments)
  {
    this.OwnerUid = ownerUid;
    this.SuitSensorUid = suitSensorUid;
    this.Name = name;
    this.Job = job;
    this.JobIcon = jobIcon;
    this.JobDepartments = jobDepartments;
  }

  public float? DamagePercentage
  {
    get
    {
      if (!this.TotalDamageThreshold.HasValue || !this.TotalDamage.HasValue)
        return new float?();
      int? totalDamage = this.TotalDamage;
      float? nullable = totalDamage.HasValue ? new float?((float) totalDamage.GetValueOrDefault()) : new float?();
      float num = (float) this.TotalDamageThreshold.Value;
      return !nullable.HasValue ? new float?() : new float?(nullable.GetValueOrDefault() / num);
    }
  }
}
