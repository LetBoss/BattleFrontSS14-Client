// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.MarkingPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Humanoid.Markings;

[Prototype(null, 1)]
public sealed class MarkingPrototype : IPrototype
{
  [DataField("RMCFollowSkinColor", false, 1, false, false, null)]
  public bool RMCFollowSkinColor = true;

  [IdDataField(1, null)]
  public string ID { get; private set; } = "uwu";

  public string Name { get; private set; }

  [DataField("bodyPart", false, 1, true, false, null)]
  public HumanoidVisualLayers BodyPart { get; private set; }

  [DataField("markingCategory", false, 1, true, false, null)]
  public MarkingCategories MarkingCategory { get; private set; }

  [DataField("speciesRestriction", false, 1, false, false, null)]
  public List<string>? SpeciesRestrictions { get; private set; }

  [DataField("sexRestriction", false, 1, false, false, null)]
  public Sex? SexRestriction { get; private set; }

  [DataField("followSkinColor", false, 1, false, false, null)]
  public bool FollowSkinColor { get; private set; }

  [DataField("forcedColoring", false, 1, false, false, null)]
  public bool ForcedColoring { get; private set; }

  [DataField("coloring", false, 1, false, false, null)]
  public MarkingColors Coloring { get; private set; } = new MarkingColors();

  [DataField(null, false, 1, false, false, null)]
  public bool CanBeDisplaced { get; private set; } = true;

  [DataField("sprites", false, 1, true, false, null)]
  public List<SpriteSpecifier> Sprites { get; private set; }

  public Marking AsMarking() => new Marking(this.ID, this.Sprites.Count);
}
