// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Humanoid.IRMCHumanoidAppearance
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DisplacementMap;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Inventory;
using Robust.Shared.Enums;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Humanoid;

public interface IRMCHumanoidAppearance
{
  MarkingSet ClientOldMarkings { get; set; }

  MarkingSet MarkingSet { get; set; }

  Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer> BaseLayers { get; set; }

  HashSet<HumanoidVisualLayers> PermanentlyHidden { get; set; }

  Gender Gender { get; set; }

  int Age { get; set; }

  Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> CustomBaseLayers { get; set; }

  ProtoId<SpeciesPrototype> Species { get; set; }

  ProtoId<HumanoidProfilePrototype>? Initial { get; }

  Color SkinColor { get; set; }

  Dictionary<HumanoidVisualLayers, SlotFlags> HiddenLayers { get; set; }

  Sex Sex { get; set; }

  Color EyeColor { get; set; }

  Color? CachedHairColor { get; set; }

  Color? CachedFacialHairColor { get; set; }

  HashSet<HumanoidVisualLayers> HideLayersOnEquip { get; set; }

  ProtoId<MarkingPrototype>? UndergarmentTop { get; set; }

  ProtoId<MarkingPrototype>? UndergarmentBottom { get; set; }

  Dictionary<HumanoidVisualLayers, DisplacementData> MarkingsDisplacement { get; set; }
}
