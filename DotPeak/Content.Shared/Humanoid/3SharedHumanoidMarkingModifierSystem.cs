// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.HumanoidMarkingModifierState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Humanoid.Markings;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Humanoid;

[NetSerializable]
[Serializable]
public sealed class HumanoidMarkingModifierState : BoundUserInterfaceState
{
  public HumanoidMarkingModifierState(
    MarkingSet markingSet,
    string species,
    Sex sex,
    Color skinColor,
    Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> customBaseLayers)
  {
    this.MarkingSet = markingSet;
    this.Species = species;
    this.Sex = sex;
    this.SkinColor = skinColor;
    this.CustomBaseLayers = customBaseLayers;
  }

  public MarkingSet MarkingSet { get; }

  public string Species { get; }

  public Sex Sex { get; }

  public Color SkinColor { get; }

  public Color EyeColor { get; }

  public Color? HairColor { get; }

  public Color? FacialHairColor { get; }

  public Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> CustomBaseLayers { get; }
}
