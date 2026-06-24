// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Prototypes.HumanoidProfilePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Preferences;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Humanoid.Prototypes;

[Prototype(null, 1)]
public sealed class HumanoidProfilePrototype : IPrototype
{
  [DataField("customBaseLayers", false, 1, false, false, null)]
  public Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> CustomBaseLayers = new Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>();

  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("profile", false, 1, false, false, null)]
  public HumanoidCharacterProfile Profile { get; private set; } = new HumanoidCharacterProfile();
}
