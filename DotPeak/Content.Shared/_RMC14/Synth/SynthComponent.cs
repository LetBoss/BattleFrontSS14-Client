// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Synth.SynthComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Medical.HUD.Components;
using Content.Shared.Body.Organ;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.StatusIcon;
using Content.Shared.Tools;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Synth;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedSynthSystem)})]
public sealed class SynthComponent : 
  Component,
  ISerializationGenerated<SynthComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId AddComponents = (EntProtoId) "RMCSynthAddComponents";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId RemoveComponents = (EntProtoId) "RMCSynthRemoveComponents";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float? StunResistance = new float?(2.5f);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanUseGuns;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanUseMeleeWeapons = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ReagentPrototype> NewBloodReagent = (ProtoId<ReagentPrototype>) "RMCSynthBlood";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<DamageModifierSetPrototype> NewDamageModifier = (ProtoId<DamageModifierSetPrototype>) "RMCSynth";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId SpeciesName = (LocId) "rmc-species-name-synth";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId Generation = (LocId) "rmc-species-synth-generation-third";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public LocId FixedIdentityReplacement = (LocId) "cm-chatsan-replacement-synth";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>> HealthIconOverrides = new Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>>()
  {
    [RMCHealthIconTypes.Healthy] = (ProtoId<HealthIconPrototype>) "RMCHealthIconHealthySynth",
    [RMCHealthIconTypes.DeadDefib] = (ProtoId<HealthIconPrototype>) "RMCHealthIconDeadSynth",
    [RMCHealthIconTypes.DeadClose] = (ProtoId<HealthIconPrototype>) "RMCHealthIconDeadSynth",
    [RMCHealthIconTypes.DeadAlmost] = (ProtoId<HealthIconPrototype>) "RMCHealthIconDeadSynth",
    [RMCHealthIconTypes.DeadDNR] = (ProtoId<HealthIconPrototype>) "RMCHealthIconDeadDNRSynth",
    [RMCHealthIconTypes.Dead] = (ProtoId<HealthIconPrototype>) "RMCHealthIconDeadSynth",
    [RMCHealthIconTypes.HCDead] = (ProtoId<HealthIconPrototype>) "RMCHealthIconDeadSynth"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<OrganComponent> NewBrain = (EntProtoId<OrganComponent>) "RMCOrganSynthBrain";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RepairTime = TimeSpan.FromSeconds(0L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan SelfRepairTime = TimeSpan.FromSeconds(30L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 CritThreshold = FixedPoint2.New(199);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> RepairQuality = (ProtoId<ToolQualityPrototype>) "Welding";
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier? WelderDamageToRepair = new DamageSpecifier()
  {
    DamageDict = {
      ["Blunt"] = (FixedPoint2) -15,
      ["Piercing"] = (FixedPoint2) -15,
      ["Slash"] = (FixedPoint2) -15
    }
  };
  [DataField(null, false, 1, false, false, null)]
  public DamageSpecifier? CableCoilDamageToRepair = new DamageSpecifier()
  {
    DamageDict = {
      ["Caustic"] = (FixedPoint2) -15,
      ["Heat"] = (FixedPoint2) -15,
      ["Shock"] = (FixedPoint2) -15,
      ["Cold"] = (FixedPoint2) -15
    }
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<DamageGroupPrototype> WelderDamageGroup = (ProtoId<DamageGroupPrototype>) "Brute";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<DamageGroupPrototype> CableCoilDamageGroup = (ProtoId<DamageGroupPrototype>) "Burn";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string DamageVisualsColor = "#EEEEEE";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SynthComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SynthComponent) target1;
    if (serialization.TryCustomCopy<SynthComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId target2 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.AddComponents, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId>(this.AddComponents, hookCtx, context);
    target.AddComponents = target2;
    EntProtoId target3 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.RemoveComponents, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId>(this.RemoveComponents, hookCtx, context);
    target.RemoveComponents = target3;
    float? target4 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.StunResistance, ref target4, hookCtx, false, context))
      target4 = this.StunResistance;
    target.StunResistance = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanUseGuns, ref target5, hookCtx, false, context))
      target5 = this.CanUseGuns;
    target.CanUseGuns = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanUseMeleeWeapons, ref target6, hookCtx, false, context))
      target6 = this.CanUseMeleeWeapons;
    target.CanUseMeleeWeapons = target6;
    ProtoId<ReagentPrototype> target7 = new ProtoId<ReagentPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ReagentPrototype>>(this.NewBloodReagent, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<ProtoId<ReagentPrototype>>(this.NewBloodReagent, hookCtx, context);
    target.NewBloodReagent = target7;
    ProtoId<DamageModifierSetPrototype> target8 = new ProtoId<DamageModifierSetPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DamageModifierSetPrototype>>(this.NewDamageModifier, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<ProtoId<DamageModifierSetPrototype>>(this.NewDamageModifier, hookCtx, context);
    target.NewDamageModifier = target8;
    LocId target9 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.SpeciesName, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<LocId>(this.SpeciesName, hookCtx, context);
    target.SpeciesName = target9;
    LocId target10 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.Generation, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<LocId>(this.Generation, hookCtx, context);
    target.Generation = target10;
    LocId target11 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.FixedIdentityReplacement, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<LocId>(this.FixedIdentityReplacement, hookCtx, context);
    target.FixedIdentityReplacement = target11;
    Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>> target12 = (Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>>) null;
    if (this.HealthIconOverrides == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>>>(this.HealthIconOverrides, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>>>(this.HealthIconOverrides, hookCtx, context);
    target.HealthIconOverrides = target12;
    EntProtoId<OrganComponent> target13 = new EntProtoId<OrganComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<OrganComponent>>(this.NewBrain, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntProtoId<OrganComponent>>(this.NewBrain, hookCtx, context);
    target.NewBrain = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RepairTime, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.RepairTime, hookCtx, context);
    target.RepairTime = target14;
    TimeSpan target15 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.SelfRepairTime, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan>(this.SelfRepairTime, hookCtx, context);
    target.SelfRepairTime = target15;
    FixedPoint2 target16 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.CritThreshold, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<FixedPoint2>(this.CritThreshold, hookCtx, context);
    target.CritThreshold = target16;
    ProtoId<ToolQualityPrototype> target17 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.RepairQuality, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.RepairQuality, hookCtx, context);
    target.RepairQuality = target17;
    DamageSpecifier target18 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.WelderDamageToRepair, ref target18, hookCtx, false, context))
    {
      if (this.WelderDamageToRepair == null)
        target18 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.WelderDamageToRepair, ref target18, hookCtx, context);
    }
    target.WelderDamageToRepair = target18;
    DamageSpecifier target19 = (DamageSpecifier) null;
    if (!serialization.TryCustomCopy<DamageSpecifier>(this.CableCoilDamageToRepair, ref target19, hookCtx, false, context))
    {
      if (this.CableCoilDamageToRepair == null)
        target19 = (DamageSpecifier) null;
      else
        serialization.CopyTo<DamageSpecifier>(this.CableCoilDamageToRepair, ref target19, hookCtx, context);
    }
    target.CableCoilDamageToRepair = target19;
    ProtoId<DamageGroupPrototype> target20 = new ProtoId<DamageGroupPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DamageGroupPrototype>>(this.WelderDamageGroup, ref target20, hookCtx, false, context))
      target20 = serialization.CreateCopy<ProtoId<DamageGroupPrototype>>(this.WelderDamageGroup, hookCtx, context);
    target.WelderDamageGroup = target20;
    ProtoId<DamageGroupPrototype> target21 = new ProtoId<DamageGroupPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<DamageGroupPrototype>>(this.CableCoilDamageGroup, ref target21, hookCtx, false, context))
      target21 = serialization.CreateCopy<ProtoId<DamageGroupPrototype>>(this.CableCoilDamageGroup, hookCtx, context);
    target.CableCoilDamageGroup = target21;
    string target22 = (string) null;
    if (this.DamageVisualsColor == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.DamageVisualsColor, ref target22, hookCtx, false, context))
      target22 = this.DamageVisualsColor;
    target.DamageVisualsColor = target22;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SynthComponent target,
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
    SynthComponent target1 = (SynthComponent) target;
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
    SynthComponent target1 = (SynthComponent) target;
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
    SynthComponent target1 = (SynthComponent) target;
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
  virtual SynthComponent Component.Instantiate() => new SynthComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SynthComponent_AutoState : IComponentState
  {
    public float? StunResistance;
    public bool CanUseGuns;
    public bool CanUseMeleeWeapons;
    public ProtoId<ReagentPrototype> NewBloodReagent;
    public ProtoId<DamageModifierSetPrototype> NewDamageModifier;
    public LocId SpeciesName;
    public LocId Generation;
    public LocId FixedIdentityReplacement;
    public Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>> HealthIconOverrides;
    public EntProtoId<OrganComponent> NewBrain;
    public TimeSpan RepairTime;
    public TimeSpan SelfRepairTime;
    public FixedPoint2 CritThreshold;
    public ProtoId<ToolQualityPrototype> RepairQuality;
    public ProtoId<DamageGroupPrototype> WelderDamageGroup;
    public ProtoId<DamageGroupPrototype> CableCoilDamageGroup;
    public string DamageVisualsColor;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SynthComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SynthComponent, ComponentGetState>(new ComponentEventRefHandler<SynthComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SynthComponent, ComponentHandleState>(new ComponentEventRefHandler<SynthComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, SynthComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new SynthComponent.SynthComponent_AutoState()
      {
        StunResistance = component.StunResistance,
        CanUseGuns = component.CanUseGuns,
        CanUseMeleeWeapons = component.CanUseMeleeWeapons,
        NewBloodReagent = component.NewBloodReagent,
        NewDamageModifier = component.NewDamageModifier,
        SpeciesName = component.SpeciesName,
        Generation = component.Generation,
        FixedIdentityReplacement = component.FixedIdentityReplacement,
        HealthIconOverrides = component.HealthIconOverrides,
        NewBrain = component.NewBrain,
        RepairTime = component.RepairTime,
        SelfRepairTime = component.SelfRepairTime,
        CritThreshold = component.CritThreshold,
        RepairQuality = component.RepairQuality,
        WelderDamageGroup = component.WelderDamageGroup,
        CableCoilDamageGroup = component.CableCoilDamageGroup,
        DamageVisualsColor = component.DamageVisualsColor
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SynthComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SynthComponent.SynthComponent_AutoState current))
        return;
      component.StunResistance = current.StunResistance;
      component.CanUseGuns = current.CanUseGuns;
      component.CanUseMeleeWeapons = current.CanUseMeleeWeapons;
      component.NewBloodReagent = current.NewBloodReagent;
      component.NewDamageModifier = current.NewDamageModifier;
      component.SpeciesName = current.SpeciesName;
      component.Generation = current.Generation;
      component.FixedIdentityReplacement = current.FixedIdentityReplacement;
      component.HealthIconOverrides = current.HealthIconOverrides == null ? (Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>>) null : new Dictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>>((IDictionary<RMCHealthIconTypes, ProtoId<HealthIconPrototype>>) current.HealthIconOverrides);
      component.NewBrain = current.NewBrain;
      component.RepairTime = current.RepairTime;
      component.SelfRepairTime = current.SelfRepairTime;
      component.CritThreshold = current.CritThreshold;
      component.RepairQuality = current.RepairQuality;
      component.WelderDamageGroup = current.WelderDamageGroup;
      component.CableCoilDamageGroup = current.CableCoilDamageGroup;
      component.DamageVisualsColor = current.DamageVisualsColor;
    }
  }
}
