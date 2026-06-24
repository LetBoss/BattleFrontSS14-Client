// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mech.Components.MechComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
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
namespace Content.Shared.Mech.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class MechComponent : 
  Component,
  ISerializationGenerated<MechComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public FixedPoint2 Integrity;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public FixedPoint2 MaxIntegrity = (FixedPoint2) 250;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public FixedPoint2 Energy = (FixedPoint2) 0;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public FixedPoint2 MaxEnergy = (FixedPoint2) 0;
  [Robust.Shared.ViewVariables.ViewVariables]
  public ContainerSlot BatterySlot;
  [Robust.Shared.ViewVariables.ViewVariables]
  public readonly string BatterySlotId = "mech-battery-slot";
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float MechToPilotDamageMultiplier;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [AutoNetworkedField]
  public bool Broken;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public ContainerSlot PilotSlot;
  [Robust.Shared.ViewVariables.ViewVariables]
  public readonly string PilotSlotId = "mech-pilot-slot";
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public EntityUid? CurrentSelectedEquipment;
  [DataField("maxEquipmentAmount", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public int MaxEquipmentAmount = 3;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? EquipmentWhitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? PilotWhitelist;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public Container EquipmentContainer;
  [Robust.Shared.ViewVariables.ViewVariables]
  public readonly string EquipmentContainerId = "mech-equipment-container";
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float EntryDelay = 3f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float ExitDelay = 3f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public float BatteryRemovalDelay = 2f;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool Airtight;
  [DataField(null, false, 1, false, false, null)]
  public List<EntProtoId> StartingEquipment = new List<EntProtoId>();
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId MechCycleAction = (EntProtoId) "ActionMechCycleEquipment";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId MechUiAction = (EntProtoId) "ActionMechOpenUI";
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId MechEjectAction = (EntProtoId) "ActionMechEject";
  [DataField(null, false, 1, false, false, null)]
  public string? BaseState;
  [DataField(null, false, 1, false, false, null)]
  public string? OpenState;
  [DataField(null, false, 1, false, false, null)]
  public string? BrokenState;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? MechCycleActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? MechUiActionEntity;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? MechEjectActionEntity;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MechComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MechComponent) target1;
    if (serialization.TryCustomCopy<MechComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MaxIntegrity, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.MaxIntegrity, hookCtx, context);
    target.MaxIntegrity = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MaxEnergy, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.MaxEnergy, hookCtx, context);
    target.MaxEnergy = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MechToPilotDamageMultiplier, ref target4, hookCtx, false, context))
      target4 = this.MechToPilotDamageMultiplier;
    target.MechToPilotDamageMultiplier = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxEquipmentAmount, ref target5, hookCtx, false, context))
      target5 = this.MaxEquipmentAmount;
    target.MaxEquipmentAmount = target5;
    EntityWhitelist target6 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.EquipmentWhitelist, ref target6, hookCtx, false, context))
    {
      if (this.EquipmentWhitelist == null)
        target6 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.EquipmentWhitelist, ref target6, hookCtx, context);
    }
    target.EquipmentWhitelist = target6;
    EntityWhitelist target7 = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.PilotWhitelist, ref target7, hookCtx, false, context))
    {
      if (this.PilotWhitelist == null)
        target7 = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.PilotWhitelist, ref target7, hookCtx, context);
    }
    target.PilotWhitelist = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EntryDelay, ref target8, hookCtx, false, context))
      target8 = this.EntryDelay;
    target.EntryDelay = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ExitDelay, ref target9, hookCtx, false, context))
      target9 = this.ExitDelay;
    target.ExitDelay = target9;
    float target10 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BatteryRemovalDelay, ref target10, hookCtx, false, context))
      target10 = this.BatteryRemovalDelay;
    target.BatteryRemovalDelay = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.Airtight, ref target11, hookCtx, false, context))
      target11 = this.Airtight;
    target.Airtight = target11;
    List<EntProtoId> target12 = (List<EntProtoId>) null;
    if (this.StartingEquipment == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<EntProtoId>>(this.StartingEquipment, ref target12, hookCtx, true, context))
      target12 = serialization.CreateCopy<List<EntProtoId>>(this.StartingEquipment, hookCtx, context);
    target.StartingEquipment = target12;
    EntProtoId target13 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.MechCycleAction, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntProtoId>(this.MechCycleAction, hookCtx, context);
    target.MechCycleAction = target13;
    EntProtoId target14 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.MechUiAction, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<EntProtoId>(this.MechUiAction, hookCtx, context);
    target.MechUiAction = target14;
    EntProtoId target15 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.MechEjectAction, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<EntProtoId>(this.MechEjectAction, hookCtx, context);
    target.MechEjectAction = target15;
    string target16 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.BaseState, ref target16, hookCtx, false, context))
      target16 = this.BaseState;
    target.BaseState = target16;
    string target17 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.OpenState, ref target17, hookCtx, false, context))
      target17 = this.OpenState;
    target.OpenState = target17;
    string target18 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.BrokenState, ref target18, hookCtx, false, context))
      target18 = this.BrokenState;
    target.BrokenState = target18;
    EntityUid? target19 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.MechCycleActionEntity, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<EntityUid?>(this.MechCycleActionEntity, hookCtx, context);
    target.MechCycleActionEntity = target19;
    EntityUid? target20 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.MechUiActionEntity, ref target20, hookCtx, false, context))
      target20 = serialization.CreateCopy<EntityUid?>(this.MechUiActionEntity, hookCtx, context);
    target.MechUiActionEntity = target20;
    EntityUid? target21 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.MechEjectActionEntity, ref target21, hookCtx, false, context))
      target21 = serialization.CreateCopy<EntityUid?>(this.MechEjectActionEntity, hookCtx, context);
    target.MechEjectActionEntity = target21;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MechComponent target,
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
    MechComponent target1 = (MechComponent) target;
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
    MechComponent target1 = (MechComponent) target;
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
    MechComponent target1 = (MechComponent) target;
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
  virtual MechComponent Component.Instantiate() => new MechComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class MechComponent_AutoState : IComponentState
  {
    public FixedPoint2 Integrity;
    public FixedPoint2 MaxIntegrity;
    public FixedPoint2 Energy;
    public FixedPoint2 MaxEnergy;
    public bool Broken;
    public NetEntity? CurrentSelectedEquipment;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MechComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<MechComponent, ComponentGetState>(new ComponentEventRefHandler<MechComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<MechComponent, ComponentHandleState>(new ComponentEventRefHandler<MechComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, MechComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new MechComponent.MechComponent_AutoState()
      {
        Integrity = component.Integrity,
        MaxIntegrity = component.MaxIntegrity,
        Energy = component.Energy,
        MaxEnergy = component.MaxEnergy,
        Broken = component.Broken,
        CurrentSelectedEquipment = this.GetNetEntity(component.CurrentSelectedEquipment)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      MechComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is MechComponent.MechComponent_AutoState current))
        return;
      component.Integrity = current.Integrity;
      component.MaxIntegrity = current.MaxIntegrity;
      component.Energy = current.Energy;
      component.MaxEnergy = current.MaxEnergy;
      component.Broken = current.Broken;
      component.CurrentSelectedEquipment = this.EnsureEntity<MechComponent>(current.CurrentSelectedEquipment, uid);
    }
  }
}
