// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Prototypes.HumanoidSpeciesBaseSpritesPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Humanoid.Prototypes;

[Prototype("speciesBaseSprites", 1)]
public sealed class HumanoidSpeciesBaseSpritesPrototype : IPrototype
{
  [DataField("sprites", false, 1, true, false, null)]
  public Dictionary<HumanoidVisualLayers, string> Sprites = new Dictionary<HumanoidVisualLayers, string>();

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
