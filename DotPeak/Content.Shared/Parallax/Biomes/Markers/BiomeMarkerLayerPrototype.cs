// Decompiled with JetBrains decompiler
// Type: Content.Shared.Parallax.Biomes.Markers.BiomeMarkerLayerPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Parallax.Biomes.Markers;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class BiomeMarkerLayerPrototype : IBiomeMarkerLayer, IPrototype
{
  [DataField("radius", false, 1, false, false, null)]
  public float Radius = 32f;
  [DataField("maxCount", false, 1, false, false, null)]
  public int MaxCount = int.MaxValue;
  [DataField(null, false, 1, false, false, null)]
  public int MinGroupSize = 1;
  [DataField(null, false, 1, false, false, null)]
  public int MaxGroupSize = 1;

  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public Dictionary<EntProtoId, EntProtoId> EntityMask { get; private set; } = new Dictionary<EntProtoId, EntProtoId>();

  [DataField(null, false, 1, false, false, null)]
  public string? Prototype { get; private set; }

  [DataField("size", false, 1, false, false, null)]
  public int Size { get; private set; } = 128 /*0x80*/;
}
