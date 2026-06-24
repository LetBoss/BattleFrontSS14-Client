// Decompiled with JetBrains decompiler
// Type: Content.Shared.GameTicking.TickerJobsAvailableEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.GameTicking;

[NetSerializable]
[Serializable]
public sealed class TickerJobsAvailableEvent(
  Dictionary<NetEntity, string> stationNames,
  Dictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>> jobsAvailableByStation) : 
  EntityEventArgs
{
  public Dictionary<NetEntity, Dictionary<ProtoId<JobPrototype>, int?>> JobsAvailableByStation { get; } = jobsAvailableByStation;

  public Dictionary<NetEntity, string> StationNames { get; } = stationNames;
}
