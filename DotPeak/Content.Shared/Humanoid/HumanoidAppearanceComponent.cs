// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.HumanoidAppearanceComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Humanoid;
using Content.Shared.Corvax.TTS;
using Content.Shared.DisplacementMap;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Inventory;
using Robust.Shared.Analyzers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Humanoid;

[NetworkedComponent]
[RegisterComponent]
[AutoGenerateComponentState(true, false)]
public sealed class HumanoidAppearanceComponent : 
  Component,
  IRMCHumanoidAppearance,
  ISerializationGenerated<HumanoidAppearanceComponent>,
  ISerializationGenerated
{
  public MarkingSet ClientOldMarkings { get; set; } = new MarkingSet();

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public MarkingSet MarkingSet { get; set; } = new MarkingSet();

  [DataField(null, false, 1, false, false, null)]
  public Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer> BaseLayers { get; set; } = new Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer>();

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<HumanoidVisualLayers> PermanentlyHidden { get; set; } = new HashSet<HumanoidVisualLayers>();

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Gender Gender { get; set; }

  [DataField("voice", false, 1, false, false, null)]
  public ProtoId<TTSVoicePrototype> Voice { get; set; } = (ProtoId<TTSVoicePrototype>) "barni";

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Age { get; set; } = 18;

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> CustomBaseLayers { get; set; } = new Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>();

  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public ProtoId<SpeciesPrototype> Species { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public ProtoId<HumanoidProfilePrototype>? Initial { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Color SkinColor { get; set; } = Color.FromHex((ReadOnlySpan<char>) "#C0967F", new Color?());

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<HumanoidVisualLayers, SlotFlags> HiddenLayers { get; set; } = new Dictionary<HumanoidVisualLayers, SlotFlags>();

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Sex Sex { get; set; }

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
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
    ref HumanoidAppearanceComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HumanoidAppearanceComponent) target1;
    if (serialization.TryCustomCopy<HumanoidAppearanceComponent>(this, ref target, hookCtx, false, context))
      return;
    MarkingSet target2 = (MarkingSet) null;
    if (this.MarkingSet == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<MarkingSet>(this.MarkingSet, ref target2, hookCtx, false, context))
    {
      if (this.MarkingSet == null)
        target2 = (MarkingSet) null;
      else
        serialization.CopyTo<MarkingSet>(this.MarkingSet, ref target2, hookCtx, context, true);
    }
    target.MarkingSet = target2;
    Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer> target3 = (Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer>) null;
    if (this.BaseLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer>>(this.BaseLayers, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, HumanoidSpeciesSpriteLayer>>(this.BaseLayers, hookCtx, context);
    target.BaseLayers = target3;
    HashSet<HumanoidVisualLayers> target4 = (HashSet<HumanoidVisualLayers>) null;
    if (this.PermanentlyHidden == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<HumanoidVisualLayers>>(this.PermanentlyHidden, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<HashSet<HumanoidVisualLayers>>(this.PermanentlyHidden, hookCtx, context);
    target.PermanentlyHidden = target4;
    Gender target5 = Gender.Neuter;
    if (!serialization.TryCustomCopy<Gender>(this.Gender, ref target5, hookCtx, false, context))
      target5 = this.Gender;
    target.Gender = target5;
    ProtoId<TTSVoicePrototype> target6 = new ProtoId<TTSVoicePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<TTSVoicePrototype>>(this.Voice, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<ProtoId<TTSVoicePrototype>>(this.Voice, hookCtx, context);
    target.Voice = target6;
    int target7 = 0;
    if (!serialization.TryCustomCopy<int>(this.Age, ref target7, hookCtx, false, context))
      target7 = this.Age;
    target.Age = target7;
    Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> target8 = (Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>) null;
    if (this.CustomBaseLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>>(this.CustomBaseLayers, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>>(this.CustomBaseLayers, hookCtx, context);
    target.CustomBaseLayers = target8;
    ProtoId<SpeciesPrototype> target9 = new ProtoId<SpeciesPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<SpeciesPrototype>>(this.Species, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<ProtoId<SpeciesPrototype>>(this.Species, hookCtx, context);
    target.Species = target9;
    ProtoId<HumanoidProfilePrototype>? target10 = new ProtoId<HumanoidProfilePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<HumanoidProfilePrototype>?>(this.Initial, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<ProtoId<HumanoidProfilePrototype>?>(this.Initial, hookCtx, context);
    target.Initial = target10;
    Color target11 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.SkinColor, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<Color>(this.SkinColor, hookCtx, context);
    target.SkinColor = target11;
    Dictionary<HumanoidVisualLayers, SlotFlags> target12 = (Dictionary<HumanoidVisualLayers, SlotFlags>) null;
    if (this.HiddenLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, SlotFlags>>(this.HiddenLayers, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, SlotFlags>>(this.HiddenLayers, hookCtx, context);
    target.HiddenLayers = target12;
    Sex target13 = Sex.Male;
    if (!serialization.TryCustomCopy<Sex>(this.Sex, ref target13, hookCtx, false, context))
      target13 = this.Sex;
    target.Sex = target13;
    Color target14 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.EyeColor, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<Color>(this.EyeColor, hookCtx, context);
    target.EyeColor = target14;
    HashSet<HumanoidVisualLayers> target15 = (HashSet<HumanoidVisualLayers>) null;
    if (this.HideLayersOnEquip == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<HumanoidVisualLayers>>(this.HideLayersOnEquip, ref target15, hookCtx, true, context))
      target15 = serialization.CreateCopy<HashSet<HumanoidVisualLayers>>(this.HideLayersOnEquip, hookCtx, context);
    target.HideLayersOnEquip = target15;
    ProtoId<MarkingPrototype>? target16 = new ProtoId<MarkingPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<MarkingPrototype>?>(this.UndergarmentTop, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<ProtoId<MarkingPrototype>?>(this.UndergarmentTop, hookCtx, context);
    target.UndergarmentTop = target16;
    ProtoId<MarkingPrototype>? target17 = new ProtoId<MarkingPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<MarkingPrototype>?>(this.UndergarmentBottom, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<ProtoId<MarkingPrototype>?>(this.UndergarmentBottom, hookCtx, context);
    target.UndergarmentBottom = target17;
    Dictionary<HumanoidVisualLayers, DisplacementData> target18 = (Dictionary<HumanoidVisualLayers, DisplacementData>) null;
    if (this.MarkingsDisplacement == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<HumanoidVisualLayers, DisplacementData>>(this.MarkingsDisplacement, ref target18, hookCtx, true, context))
      target18 = serialization.CreateCopy<Dictionary<HumanoidVisualLayers, DisplacementData>>(this.MarkingsDisplacement, hookCtx, context);
    target.MarkingsDisplacement = target18;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HumanoidAppearanceComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HumanoidAppearanceComponent target1 = (HumanoidAppearanceComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HumanoidAppearanceComponent target1 = (HumanoidAppearanceComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    HumanoidAppearanceComponent target1 = (HumanoidAppearanceComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual HumanoidAppearanceComponent Component.Instantiate() => new HumanoidAppearanceComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HumanoidAppearanceComponent_AutoState : IComponentState
  {
    public MarkingSet MarkingSet;
    public HashSet<HumanoidVisualLayers> PermanentlyHidden;
    public Gender Gender;
    public int Age;
    public Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> CustomBaseLayers;
    public ProtoId<SpeciesPrototype> Species;
    public Color SkinColor;
    public Dictionary<HumanoidVisualLayers, SlotFlags> HiddenLayers;
    public Sex Sex;
    public Color EyeColor;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HumanoidAppearanceComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HumanoidAppearanceComponent, ComponentGetState>(new ComponentEventRefHandler<HumanoidAppearanceComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HumanoidAppearanceComponent, ComponentHandleState>(new ComponentEventRefHandler<HumanoidAppearanceComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      HumanoidAppearanceComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new HumanoidAppearanceComponent.HumanoidAppearanceComponent_AutoState()
      {
        MarkingSet = component.MarkingSet,
        PermanentlyHidden = component.PermanentlyHidden,
        Gender = component.Gender,
        Age = component.Age,
        CustomBaseLayers = component.CustomBaseLayers,
        Species = component.Species,
        SkinColor = component.SkinColor,
        HiddenLayers = component.HiddenLayers,
        Sex = component.Sex,
        EyeColor = component.EyeColor
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HumanoidAppearanceComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HumanoidAppearanceComponent.HumanoidAppearanceComponent_AutoState current))
        return;
      component.MarkingSet = current.MarkingSet;
      component.PermanentlyHidden = current.PermanentlyHidden == null ? (HashSet<HumanoidVisualLayers>) null : new HashSet<HumanoidVisualLayers>((IEnumerable<HumanoidVisualLayers>) current.PermanentlyHidden);
      component.Gender = current.Gender;
      component.Age = current.Age;
      component.CustomBaseLayers = current.CustomBaseLayers == null ? (Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>) null : new Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo>((IDictionary<HumanoidVisualLayers, CustomBaseLayerInfo>) current.CustomBaseLayers);
      component.Species = current.Species;
      component.SkinColor = current.SkinColor;
      component.HiddenLayers = current.HiddenLayers == null ? (Dictionary<HumanoidVisualLayers, SlotFlags>) null : new Dictionary<HumanoidVisualLayers, SlotFlags>((IDictionary<HumanoidVisualLayers, SlotFlags>) current.HiddenLayers);
      component.Sex = current.Sex;
      component.EyeColor = current.EyeColor;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, HumanoidAppearanceComponent>(uid, component, ref args1);
    }
  }
}
