// Decompiled with JetBrains decompiler
// Type: Content.Shared.Anomaly.Effects.Components.EntitySpawnSettingsEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Anomaly.Effects.Components;

[DataRecord]
public record struct EntitySpawnSettingsEntry
{
  public EntitySpawnSettingsEntry()
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003CSpawns\u003Ek__BackingField = new List<EntProtoId>();
    // ISSUE: reference to a compiler-generated field
    this.\u003CSettings\u003Ek__BackingField = new AnomalySpawnSettings();
  }

  public List<EntProtoId> Spawns { get; set; }

  public AnomalySpawnSettings Settings { get; set; }
}
