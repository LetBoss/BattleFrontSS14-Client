// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Humanoid.RMCHumanoidAppearance
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
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Humanoid;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class RMCHumanoidAppearance : 
  IRMCHumanoidAppearance,
  ISerializationGenerated<RMCHumanoidAppearance>,
  ISerializationGenerated
{
  public MarkingSet ClientOldMarkings { get; set; } = new MarkingSet();

  [DataField(null, false, 1, false, false, null)]
  public MarkingSet MarkingSet { get; set; } = new MarkingSet();

  [DataField(null, false, 1, false, false, null)]
  public Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer> BaseLayers { get; set; } = new Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer>();

  [DataField(null, false, 1, false, false, null)]
  public HashSet<HumanoidVisualLayers> PermanentlyHidden { get; set; } = new HashSet<HumanoidVisualLayers>();

  [DataField(null, false, 1, false, false, null)]
  public Gender Gender { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public int Age { get; set; } = 18;

  [DataField(null, false, 1, false, false, null)]
  public Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> CustomBaseLayers { get; set; } = new Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>();

  [DataField(null, false, 1, true, false, null)]
  public ProtoId<SpeciesPrototype> Species { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<HumanoidProfilePrototype>? Initial { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public Color SkinColor { get; set; } = Color.FromHex((ReadOnlySpan<char>) "#C0967F", new Color?());

  [DataField(null, false, 1, false, false, null)]
  public Dictionary<HumanoidVisualLayers, SlotFlags> HiddenLayers { get; set; } = new Dictionary<HumanoidVisualLayers, SlotFlags>();

  [DataField(null, false, 1, false, false, null)]
  public Sex Sex { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public Color EyeColor { get; set; } = Color.Brown;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public Color? CachedHairColor { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public Color? CachedFacialHairColor { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public HashSet<HumanoidVisualLayers> HideLayersOnEquip { get; set; } = new HashSet<HumanoidVisualLayers>()
  {
    HumanoidVisualLayers.Hair
  };

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<MarkingPrototype>? UndergarmentTop { get; set; } = new ProtoId<MarkingPrototype>?(new ProtoId<MarkingPrototype>("UndergarmentTopTanktop"));

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<MarkingPrototype>? UndergarmentBottom { get; set; } = new ProtoId<MarkingPrototype>?(new ProtoId<MarkingPrototype>("UndergarmentBottomBoxers"));

  [DataField(null, false, 1, false, false, null)]
  public Dictionary<HumanoidVisualLayers, DisplacementData> MarkingsDisplacement { get; set; } = new Dictionary<HumanoidVisualLayers, DisplacementData>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCHumanoidAppearance target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RMCHumanoidAppearance>(this, ref target, hookCtx, false, context))
      return;
    MarkingSet target1 = (MarkingSet) null;
    if (this.MarkingSet == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<MarkingSet>(this.MarkingSet, ref target1, hookCtx, false, context))
    {
      if (this.MarkingSet == null)
        target1 = (MarkingSet) null;
      else
        serialization.CopyTo<MarkingSet>(this.MarkingSet, ref target1, hookCtx, context, true);
    }
    target.MarkingSet = target1;
    Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer> target2 = (Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer>) null;
    if (this.BaseLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer>>(this.BaseLayers, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer>>(this.BaseLayers, hookCtx, context);
    target.BaseLayers = target2;
    HashSet<HumanoidVisualLayers> target3 = (HashSet<HumanoidVisualLayers>) null;
    if (this.PermanentlyHidden == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<HumanoidVisualLayers>>(this.PermanentlyHidden, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<HumanoidVisualLayers>>(this.PermanentlyHidden, hookCtx, context);
    target.PermanentlyHidden = target3;
    Gender target4 = Gender.Neuter;
    if (!serialization.TryCustomCopy<Gender>(this.Gender, ref target4, hookCtx, false, context))
      target4 = this.Gender;
    target.Gender = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.Age, ref target5, hookCtx, false, context))
      target5 = this.Age;
    target.Age = target5;
    Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> target6 = (Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>) null;
    if (this.CustomBaseLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>>(this.CustomBaseLayers, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>>(this.CustomBaseLayers, hookCtx, context);
    target.CustomBaseLayers = target6;
    ProtoId<SpeciesPrototype> target7 = new ProtoId<SpeciesPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<SpeciesPrototype>>(this.Species, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<ProtoId<SpeciesPrototype>>(this.Species, hookCtx, context);
    target.Species = target7;
    ProtoId<HumanoidProfilePrototype>? target8 = new ProtoId<HumanoidProfilePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<HumanoidProfilePrototype>?>(this.Initial, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<ProtoId<HumanoidProfilePrototype>?>(this.Initial, hookCtx, context);
    target.Initial = target8;
    Color target9 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.SkinColor, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<Color>(this.SkinColor, hookCtx, context);
    target.SkinColor = target9;
    Dictionary<HumanoidVisualLayers, SlotFlags> target10 = (Dictionary<HumanoidVisualLayers, SlotFlags>) null;
    if (this.HiddenLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, SlotFlags>>(this.HiddenLayers, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, SlotFlags>>(this.HiddenLayers, hookCtx, context);
    target.HiddenLayers = target10;
    Sex target11 = Sex.Male;
    if (!serialization.TryCustomCopy<Sex>(this.Sex, ref target11, hookCtx, false, context))
      target11 = this.Sex;
    target.Sex = target11;
    Color target12 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.EyeColor, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<Color>(this.EyeColor, hookCtx, context);
    target.EyeColor = target12;
    HashSet<HumanoidVisualLayers> target13 = (HashSet<HumanoidVisualLayers>) null;
    if (this.HideLayersOnEquip == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<HumanoidVisualLayers>>(this.HideLayersOnEquip, ref target13, hookCtx, true, context))
      target13 = serialization.CreateCopy<HashSet<HumanoidVisualLayers>>(this.HideLayersOnEquip, hookCtx, context);
    target.HideLayersOnEquip = target13;
    ProtoId<MarkingPrototype>? target14 = new ProtoId<MarkingPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<MarkingPrototype>?>(this.UndergarmentTop, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<ProtoId<MarkingPrototype>?>(this.UndergarmentTop, hookCtx, context);
    target.UndergarmentTop = target14;
    ProtoId<MarkingPrototype>? target15 = new ProtoId<MarkingPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<MarkingPrototype>?>(this.UndergarmentBottom, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<ProtoId<MarkingPrototype>?>(this.UndergarmentBottom, hookCtx, context);
    target.UndergarmentBottom = target15;
    Dictionary<HumanoidVisualLayers, DisplacementData> target16 = (Dictionary<HumanoidVisualLayers, DisplacementData>) null;
    if (this.MarkingsDisplacement == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, DisplacementData>>(this.MarkingsDisplacement, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, DisplacementData>>(this.MarkingsDisplacement, hookCtx, context);
    target.MarkingsDisplacement = target16;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCHumanoidAppearance target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCHumanoidAppearance target1 = (RMCHumanoidAppearance) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RMCHumanoidAppearance Instantiate() => new RMCHumanoidAppearance();
}
