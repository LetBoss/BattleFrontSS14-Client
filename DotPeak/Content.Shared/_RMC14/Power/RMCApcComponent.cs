// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Power.RMCApcComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.Access;
using Content.Shared.PowerCell;
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
[AutoGenerateComponentState(true, false)]
[Robust.Shared.Analyzers.Access(new Type[] {typeof (SharedRMCPowerSystem)})]
public sealed class RMCApcComponent : 
  Component,
  ISerializationGenerated<RMCApcComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Area;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool MainBreakerButton;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ExternalPower;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ChargeModeButton;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCApcChargeStatus ChargeStatus;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCApcChannel[] Channels = new RMCApcChannel[Enum.GetValues<RMCPowerChannel>().Length];
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Locked = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CoverLockedButton = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string CellContainerSlot = "rmc_apc_power_cell";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<PowerCellComponent>? StartingCell = (EntProtoId<PowerCellComponent>?) "RMCPowerCellHigh";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ChargePercentage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCApcState State;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Broken;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> RepairTool = (ProtoId<ToolQualityPrototype>) "Screwing";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> CrowbarTool = (ProtoId<ToolQualityPrototype>) "Prying";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<AccessLevelPrototype>[] Access = new ProtoId<AccessLevelPrototype>[2]
  {
    (ProtoId<AccessLevelPrototype>) "CMAccessEngineering",
    (ProtoId<AccessLevelPrototype>) "CMAccessColonyEngineering"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillEngineer";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SkillLevel = 2;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCApcComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCApcComponent) target1;
    if (serialization.TryCustomCopy<RMCApcComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Area, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Area, hookCtx, context);
    target.Area = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.MainBreakerButton, ref target3, hookCtx, false, context))
      target3 = this.MainBreakerButton;
    target.MainBreakerButton = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ExternalPower, ref target4, hookCtx, false, context))
      target4 = this.ExternalPower;
    target.ExternalPower = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.ChargeModeButton, ref target5, hookCtx, false, context))
      target5 = this.ChargeModeButton;
    target.ChargeModeButton = target5;
    RMCApcChargeStatus target6 = RMCApcChargeStatus.NotCharging;
    if (!serialization.TryCustomCopy<RMCApcChargeStatus>(this.ChargeStatus, ref target6, hookCtx, false, context))
      target6 = this.ChargeStatus;
    target.ChargeStatus = target6;
    RMCApcChannel[] target7 = (RMCApcChannel[]) null;
    if (this.Channels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<RMCApcChannel[]>(this.Channels, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<RMCApcChannel[]>(this.Channels, hookCtx, context);
    target.Channels = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.Locked, ref target8, hookCtx, false, context))
      target8 = this.Locked;
    target.Locked = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.CoverLockedButton, ref target9, hookCtx, false, context))
      target9 = this.CoverLockedButton;
    target.CoverLockedButton = target9;
    string target10 = (string) null;
    if (this.CellContainerSlot == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.CellContainerSlot, ref target10, hookCtx, false, context))
      target10 = this.CellContainerSlot;
    target.CellContainerSlot = target10;
    EntProtoId<PowerCellComponent>? target11 = new EntProtoId<PowerCellComponent>?();
    if (!serialization.TryCustomCopy<EntProtoId<PowerCellComponent>?>(this.StartingCell, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<EntProtoId<PowerCellComponent>?>(this.StartingCell, hookCtx, context);
    target.StartingCell = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ChargePercentage, ref target12, hookCtx, false, context))
      target12 = this.ChargePercentage;
    target.ChargePercentage = target12;
    RMCApcState target13 = RMCApcState.Working;
    if (!serialization.TryCustomCopy<RMCApcState>(this.State, ref target13, hookCtx, false, context))
      target13 = this.State;
    target.State = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.Broken, ref target14, hookCtx, false, context))
      target14 = this.Broken;
    target.Broken = target14;
    ProtoId<ToolQualityPrototype> target15 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.RepairTool, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.RepairTool, hookCtx, context);
    target.RepairTool = target15;
    ProtoId<ToolQualityPrototype> target16 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.CrowbarTool, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.CrowbarTool, hookCtx, context);
    target.CrowbarTool = target16;
    ProtoId<AccessLevelPrototype>[] target17 = (ProtoId<AccessLevelPrototype>[]) null;
    if (this.Access == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ProtoId<AccessLevelPrototype>[]>(this.Access, ref target17, hookCtx, true, context))
      target17 = serialization.CreateCopy<ProtoId<AccessLevelPrototype>[]>(this.Access, hookCtx, context);
    target.Access = target17;
    EntProtoId<SkillDefinitionComponent> target18 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target18;
    int target19 = 0;
    if (!serialization.TryCustomCopy<int>(this.SkillLevel, ref target19, hookCtx, false, context))
      target19 = this.SkillLevel;
    target.SkillLevel = target19;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCApcComponent target,
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
    RMCApcComponent target1 = (RMCApcComponent) target;
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
    RMCApcComponent target1 = (RMCApcComponent) target;
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
    RMCApcComponent target1 = (RMCApcComponent) target;
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
  virtual RMCApcComponent Component.Instantiate() => new RMCApcComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCApcComponent_AutoState : IComponentState
  {
    public NetEntity? Area;
    public bool MainBreakerButton;
    public bool ExternalPower;
    public bool ChargeModeButton;
    public RMCApcChargeStatus ChargeStatus;
    public RMCApcChannel[] Channels;
    public bool Locked;
    public bool CoverLockedButton;
    public string CellContainerSlot;
    public EntProtoId<PowerCellComponent>? StartingCell;
    public float ChargePercentage;
    public RMCApcState State;
    public bool Broken;
    public ProtoId<ToolQualityPrototype> RepairTool;
    public ProtoId<ToolQualityPrototype> CrowbarTool;
    public ProtoId<AccessLevelPrototype>[] Access;
    public EntProtoId<SkillDefinitionComponent> Skill;
    public int SkillLevel;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCApcComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCApcComponent, ComponentGetState>(new ComponentEventRefHandler<RMCApcComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCApcComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCApcComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, RMCApcComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCApcComponent.RMCApcComponent_AutoState()
      {
        Area = this.GetNetEntity(component.Area),
        MainBreakerButton = component.MainBreakerButton,
        ExternalPower = component.ExternalPower,
        ChargeModeButton = component.ChargeModeButton,
        ChargeStatus = component.ChargeStatus,
        Channels = component.Channels,
        Locked = component.Locked,
        CoverLockedButton = component.CoverLockedButton,
        CellContainerSlot = component.CellContainerSlot,
        StartingCell = component.StartingCell,
        ChargePercentage = component.ChargePercentage,
        State = component.State,
        Broken = component.Broken,
        RepairTool = component.RepairTool,
        CrowbarTool = component.CrowbarTool,
        Access = component.Access,
        Skill = component.Skill,
        SkillLevel = component.SkillLevel
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCApcComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCApcComponent.RMCApcComponent_AutoState current))
        return;
      component.Area = this.EnsureEntity<RMCApcComponent>(current.Area, uid);
      component.MainBreakerButton = current.MainBreakerButton;
      component.ExternalPower = current.ExternalPower;
      component.ChargeModeButton = current.ChargeModeButton;
      component.ChargeStatus = current.ChargeStatus;
      component.Channels = current.Channels;
      component.Locked = current.Locked;
      component.CoverLockedButton = current.CoverLockedButton;
      component.CellContainerSlot = current.CellContainerSlot;
      component.StartingCell = current.StartingCell;
      component.ChargePercentage = current.ChargePercentage;
      component.State = current.State;
      component.Broken = current.Broken;
      component.RepairTool = current.RepairTool;
      component.CrowbarTool = current.CrowbarTool;
      component.Access = current.Access;
      component.Skill = current.Skill;
      component.SkillLevel = current.SkillLevel;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, RMCApcComponent>(uid, component, ref args1);
    }
  }
}
