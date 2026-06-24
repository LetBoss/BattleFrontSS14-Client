// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tracker.TrackerModePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

#nullable enable
namespace Content.Shared._RMC14.Tracker;

[Prototype(null, 1)]
public sealed class TrackerModePrototype : IPrototype
{
  [DataField(null, false, 1, false, false, typeof (ComponentNameSerializer))]
  public string? Component;

  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<JobPrototype>? Job { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<AlertPrototype> Alert { get; private set; } = (ProtoId<AlertPrototype>) "SquadTracker";
}
