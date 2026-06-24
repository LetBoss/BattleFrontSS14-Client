// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceNetwork.DeviceLinkUserInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceLinking;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.DeviceNetwork;

[NetSerializable]
[Serializable]
public sealed class DeviceLinkUserInterfaceState : BoundUserInterfaceState
{
  public readonly ProtoId<SourcePortPrototype>[] Sources;
  public readonly ProtoId<SinkPortPrototype>[] Sinks;
  public readonly HashSet<(ProtoId<SourcePortPrototype> source, ProtoId<SinkPortPrototype> sink)> Links;
  public readonly List<(string source, string sink)>? Defaults;
  public readonly string SourceAddress;
  public readonly string SinkAddress;

  public DeviceLinkUserInterfaceState(
    ProtoId<SourcePortPrototype>[] sources,
    ProtoId<SinkPortPrototype>[] sinks,
    HashSet<(ProtoId<SourcePortPrototype> source, ProtoId<SinkPortPrototype> sink)> links,
    string sourceAddress,
    string sinkAddress,
    List<(string source, string sink)>? defaults = null)
  {
    this.Links = links;
    this.SourceAddress = sourceAddress;
    this.SinkAddress = sinkAddress;
    this.Defaults = defaults;
    this.Sources = sources;
    this.Sinks = sinks;
  }
}
