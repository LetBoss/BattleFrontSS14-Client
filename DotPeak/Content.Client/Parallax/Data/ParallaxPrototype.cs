// Decompiled with JetBrains decompiler
// Type: Content.Client.Parallax.Data.ParallaxPrototype
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Parallax.Data;

[Prototype(null, 1)]
public sealed class ParallaxPrototype : IPrototype
{
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("layers", false, 1, false, false, null)]
  public List<ParallaxLayerConfig> Layers { get; private set; } = new List<ParallaxLayerConfig>();

  [DataField("layersLQ", false, 1, false, false, null)]
  public List<ParallaxLayerConfig> LayersLQ { get; private set; } = new List<ParallaxLayerConfig>();

  [DataField("layersLQUseHQ", false, 1, false, false, null)]
  public bool LayersLQUseHQ { get; private set; } = true;
}
