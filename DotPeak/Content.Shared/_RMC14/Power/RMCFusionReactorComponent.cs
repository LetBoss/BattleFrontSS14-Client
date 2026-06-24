// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Power.RMCFusionReactorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Tools;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Power;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedRMCPowerSystem)})]
public sealed class RMCFusionReactorComponent : 
  Component,
  ISerializationGenerated<RMCFusionReactorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Watts = 50000;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCFusionReactorState State;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string CellContainerSlot = "rmc_fusion_reactor_cell";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<RMCFusionCellComponent>? StartingCell = (EntProtoId<RMCFusionCellComponent>?) "RMCGeneratorFusionCell";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan CellDelay = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan CellRemoveDelay = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RepairDelay = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DestroyDelay = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float WeldingCost = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillEngineer";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> CrowbarQuality = (ProtoId<ToolQualityPrototype>) "Prying";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> WeldingQuality = (ProtoId<ToolQualityPrototype>) "Welding";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> CuttingQuality = (ProtoId<ToolQualityPrototype>) "Cutting";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> WrenchQuality = (ProtoId<ToolQualityPrototype>) "Anchoring";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RandomizeDamage;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCFusionReactorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCFusionReactorComponent) target1;
    if (serialization.TryCustomCopy<RMCFusionReactorComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Watts, ref target2, hookCtx, false, context))
      target2 = this.Watts;
    target.Watts = target2;
    RMCFusionReactorState target3 = RMCFusionReactorState.Working;
    if (!serialization.TryCustomCopy<RMCFusionReactorState>(this.State, ref target3, hookCtx, false, context))
      target3 = this.State;
    target.State = target3;
    string target4 = (string) null;
    if (this.CellContainerSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.CellContainerSlot, ref target4, hookCtx, false, context))
      target4 = this.CellContainerSlot;
    target.CellContainerSlot = target4;
    EntProtoId<RMCFusionCellComponent>? target5 = new EntProtoId<RMCFusionCellComponent>?();
    if (!serialization.TryCustomCopy<EntProtoId<RMCFusionCellComponent>?>(this.StartingCell, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId<RMCFusionCellComponent>?>(this.StartingCell, hookCtx, context);
    target.StartingCell = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CellDelay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.CellDelay, hookCtx, context);
    target.CellDelay = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CellRemoveDelay, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.CellRemoveDelay, hookCtx, context);
    target.CellRemoveDelay = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RepairDelay, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.RepairDelay, hookCtx, context);
    target.RepairDelay = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DestroyDelay, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.DestroyDelay, hookCtx, context);
    target.DestroyDelay = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WeldingCost, ref target10, hookCtx, false, context))
      target10 = this.WeldingCost;
    target.WeldingCost = target10;
    EntProtoId<SkillDefinitionComponent> target11 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target11;
    ProtoId<ToolQualityPrototype> target12 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.CrowbarQuality, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.CrowbarQuality, hookCtx, context);
    target.CrowbarQuality = target12;
    ProtoId<ToolQualityPrototype> target13 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.WeldingQuality, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.WeldingQuality, hookCtx, context);
    target.WeldingQuality = target13;
    ProtoId<ToolQualityPrototype> target14 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.CuttingQuality, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.CuttingQuality, hookCtx, context);
    target.CuttingQuality = target14;
    ProtoId<ToolQualityPrototype> target15 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.WrenchQuality, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.WrenchQuality, hookCtx, context);
    target.WrenchQuality = target15;
    bool target16 = false;
    if (!serialization.TryCustomCopy<bool>(this.RandomizeDamage, ref target16, hookCtx, false, context))
      target16 = this.RandomizeDamage;
    target.RandomizeDamage = target16;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCFusionReactorComponent target,
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
    RMCFusionReactorComponent target1 = (RMCFusionReactorComponent) target;
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
    RMCFusionReactorComponent target1 = (RMCFusionReactorComponent) target;
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
    RMCFusionReactorComponent target1 = (RMCFusionReactorComponent) target;
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
  virtual RMCFusionReactorComponent Component.Instantiate() => new RMCFusionReactorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCFusionReactorComponent_AutoState : IComponentState
  {
    public int Watts;
    public RMCFusionReactorState State;
    public string CellContainerSlot;
    public EntProtoId<RMCFusionCellComponent>? StartingCell;
    public TimeSpan CellDelay;
    public TimeSpan CellRemoveDelay;
    public TimeSpan RepairDelay;
    public TimeSpan DestroyDelay;
    public float WeldingCost;
    public EntProtoId<SkillDefinitionComponent> Skill;
    public ProtoId<ToolQualityPrototype> CrowbarQuality;
    public ProtoId<ToolQualityPrototype> WeldingQuality;
    public ProtoId<ToolQualityPrototype> CuttingQuality;
    public ProtoId<ToolQualityPrototype> WrenchQuality;
    public bool RandomizeDamage;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCFusionReactorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCFusionReactorComponent, ComponentGetState>(new ComponentEventRefHandler<RMCFusionReactorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCFusionReactorComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCFusionReactorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCFusionReactorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCFusionReactorComponent.RMCFusionReactorComponent_AutoState()
      {
        Watts = component.Watts,
        State = component.State,
        CellContainerSlot = component.CellContainerSlot,
        StartingCell = component.StartingCell,
        CellDelay = component.CellDelay,
        CellRemoveDelay = component.CellRemoveDelay,
        RepairDelay = component.RepairDelay,
        DestroyDelay = component.DestroyDelay,
        WeldingCost = component.WeldingCost,
        Skill = component.Skill,
        CrowbarQuality = component.CrowbarQuality,
        WeldingQuality = component.WeldingQuality,
        CuttingQuality = component.CuttingQuality,
        WrenchQuality = component.WrenchQuality,
        RandomizeDamage = component.RandomizeDamage
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCFusionReactorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCFusionReactorComponent.RMCFusionReactorComponent_AutoState current))
        return;
      component.Watts = current.Watts;
      component.State = current.State;
      component.CellContainerSlot = current.CellContainerSlot;
      component.StartingCell = current.StartingCell;
      component.CellDelay = current.CellDelay;
      component.CellRemoveDelay = current.CellRemoveDelay;
      component.RepairDelay = current.RepairDelay;
      component.DestroyDelay = current.DestroyDelay;
      component.WeldingCost = current.WeldingCost;
      component.Skill = current.Skill;
      component.CrowbarQuality = current.CrowbarQuality;
      component.WeldingQuality = current.WeldingQuality;
      component.CuttingQuality = current.CuttingQuality;
      component.WrenchQuality = current.WrenchQuality;
      component.RandomizeDamage = current.RandomizeDamage;
    }
  }
}
