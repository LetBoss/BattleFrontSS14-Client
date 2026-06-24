// Decompiled with JetBrains decompiler
// Type: Content.Shared.Anomaly.AnomalySpawnSettings
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization.Manager.Attributes;

#nullable disable
namespace Content.Shared.Anomaly;

[DataRecord]
public record struct AnomalySpawnSettings
{
  public AnomalySpawnSettings()
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CCanSpawnOnEntities\u003Ek__BackingField = false;
    // ISSUE: reference to a compiler-generated field
    this.\u003CMinAmount\u003Ek__BackingField = 0;
    // ISSUE: reference to a compiler-generated field
    this.\u003CMaxAmount\u003Ek__BackingField = 1;
    // ISSUE: reference to a compiler-generated field
    this.\u003CMinRange\u003Ek__BackingField = 0.0f;
    // ISSUE: reference to a compiler-generated field
    this.\u003CMaxRange\u003Ek__BackingField = 1f;
    // ISSUE: reference to a compiler-generated field
    this.\u003CSpawnOnPulse\u003Ek__BackingField = false;
    // ISSUE: reference to a compiler-generated field
    this.\u003CSpawnOnSuperCritical\u003Ek__BackingField = false;
    // ISSUE: reference to a compiler-generated field
    this.\u003CSpawnOnShutdown\u003Ek__BackingField = false;
    // ISSUE: reference to a compiler-generated field
    this.\u003CSpawnOnStabilityChanged\u003Ek__BackingField = false;
    // ISSUE: reference to a compiler-generated field
    this.\u003CSpawnOnSeverityChanged\u003Ek__BackingField = false;
  }

  public bool CanSpawnOnEntities { get; set; }

  public int MinAmount { get; set; }

  public int MaxAmount { get; set; }

  public float MinRange { get; set; }

  public float MaxRange { get; set; }

  public bool SpawnOnPulse { get; set; }

  public bool SpawnOnSuperCritical { get; set; }

  public bool SpawnOnShutdown { get; set; }

  public bool SpawnOnStabilityChanged { get; set; }

  public bool SpawnOnSeverityChanged { get; set; }
}
