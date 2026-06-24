// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.RMCChemicalDispenserComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
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
namespace Content.Shared._RMC14.Chemistry;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedRMCChemistrySystem)})]
public sealed class RMCChemicalDispenserComponent : 
  Component,
  ISerializationGenerated<RMCChemicalDispenserComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Energy;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 MaxEnergy;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 CostPerUnit = (FixedPoint2) 0.1;
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public EntProtoId Network;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string ContainerSlotId = "chemical_dispenser_slot";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ReagentPrototype>[] Reagents = new ProtoId<ReagentPrototype>[21]
  {
    (ProtoId<ReagentPrototype>) "RMCAluminum",
    (ProtoId<ReagentPrototype>) "RMCCarbon",
    (ProtoId<ReagentPrototype>) "RMCChlorine",
    (ProtoId<ReagentPrototype>) "RMCCopper",
    (ProtoId<ReagentPrototype>) "RMCEthanol",
    (ProtoId<ReagentPrototype>) "RMCFluorine",
    (ProtoId<ReagentPrototype>) "RMCHydrogen",
    (ProtoId<ReagentPrototype>) "RMCIron",
    (ProtoId<ReagentPrototype>) "RMCLithium",
    (ProtoId<ReagentPrototype>) "RMCMercury",
    (ProtoId<ReagentPrototype>) "RMCNitrogen",
    (ProtoId<ReagentPrototype>) "RMCOxygen",
    (ProtoId<ReagentPrototype>) "RMCPhosphorus",
    (ProtoId<ReagentPrototype>) "RMCPotassium",
    (ProtoId<ReagentPrototype>) "RMCRadium",
    (ProtoId<ReagentPrototype>) "RMCSilicon",
    (ProtoId<ReagentPrototype>) "RMCSodium",
    (ProtoId<ReagentPrototype>) "RMCSugar",
    (ProtoId<ReagentPrototype>) "RMCSulfur",
    (ProtoId<ReagentPrototype>) "RMCSulphuricAcid",
    (ProtoId<ReagentPrototype>) "Water"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<ReagentPrototype>> FreeReagents = new HashSet<ProtoId<ReagentPrototype>>()
  {
    (ProtoId<ReagentPrototype>) "Water"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 DispenseSetting = (FixedPoint2) 5;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2[] Settings = new FixedPoint2[5]
  {
    (FixedPoint2) 5,
    (FixedPoint2) 10,
    (FixedPoint2) 20,
    (FixedPoint2) 30,
    (FixedPoint2) 40
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCChemicalDispenserComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCChemicalDispenserComponent) target1;
    if (serialization.TryCustomCopy<RMCChemicalDispenserComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Energy, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.Energy, hookCtx, context);
    target.Energy = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MaxEnergy, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.MaxEnergy, hookCtx, context);
    target.MaxEnergy = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.CostPerUnit, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.CostPerUnit, hookCtx, context);
    target.CostPerUnit = target4;
    EntProtoId target5 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Network, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId>(this.Network, hookCtx, context);
    target.Network = target5;
    string target6 = (string) null;
    if (this.ContainerSlotId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerSlotId, ref target6, hookCtx, false, context))
      target6 = this.ContainerSlotId;
    target.ContainerSlotId = target6;
    ProtoId<ReagentPrototype>[] target7 = (ProtoId<ReagentPrototype>[]) null;
    if (this.Reagents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ProtoId<ReagentPrototype>[]>(this.Reagents, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<ProtoId<ReagentPrototype>[]>(this.Reagents, hookCtx, context);
    target.Reagents = target7;
    HashSet<ProtoId<ReagentPrototype>> target8 = (HashSet<ProtoId<ReagentPrototype>>) null;
    if (this.FreeReagents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<ReagentPrototype>>>(this.FreeReagents, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<HashSet<ProtoId<ReagentPrototype>>>(this.FreeReagents, hookCtx, context);
    target.FreeReagents = target8;
    FixedPoint2 target9 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.DispenseSetting, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<FixedPoint2>(this.DispenseSetting, hookCtx, context);
    target.DispenseSetting = target9;
    FixedPoint2[] target10 = (FixedPoint2[]) null;
    if (this.Settings == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<FixedPoint2[]>(this.Settings, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<FixedPoint2[]>(this.Settings, hookCtx, context);
    target.Settings = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCChemicalDispenserComponent target,
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
    RMCChemicalDispenserComponent target1 = (RMCChemicalDispenserComponent) target;
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
    RMCChemicalDispenserComponent target1 = (RMCChemicalDispenserComponent) target;
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
    RMCChemicalDispenserComponent target1 = (RMCChemicalDispenserComponent) target;
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
  virtual RMCChemicalDispenserComponent Component.Instantiate()
  {
    return new RMCChemicalDispenserComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCChemicalDispenserComponent_AutoState : IComponentState
  {
    public FixedPoint2 Energy;
    public FixedPoint2 MaxEnergy;
    public FixedPoint2 CostPerUnit;
    public EntProtoId Network;
    public string ContainerSlotId;
    public ProtoId<ReagentPrototype>[] Reagents;
    public HashSet<ProtoId<ReagentPrototype>> FreeReagents;
    public FixedPoint2 DispenseSetting;
    public FixedPoint2[] Settings;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCChemicalDispenserComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCChemicalDispenserComponent, ComponentGetState>(new ComponentEventRefHandler<RMCChemicalDispenserComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCChemicalDispenserComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCChemicalDispenserComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCChemicalDispenserComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCChemicalDispenserComponent.RMCChemicalDispenserComponent_AutoState()
      {
        Energy = component.Energy,
        MaxEnergy = component.MaxEnergy,
        CostPerUnit = component.CostPerUnit,
        Network = component.Network,
        ContainerSlotId = component.ContainerSlotId,
        Reagents = component.Reagents,
        FreeReagents = component.FreeReagents,
        DispenseSetting = component.DispenseSetting,
        Settings = component.Settings
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCChemicalDispenserComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCChemicalDispenserComponent.RMCChemicalDispenserComponent_AutoState current))
        return;
      component.Energy = current.Energy;
      component.MaxEnergy = current.MaxEnergy;
      component.CostPerUnit = current.CostPerUnit;
      component.Network = current.Network;
      component.ContainerSlotId = current.ContainerSlotId;
      component.Reagents = current.Reagents;
      component.FreeReagents = current.FreeReagents == null ? (HashSet<ProtoId<ReagentPrototype>>) null : new HashSet<ProtoId<ReagentPrototype>>((IEnumerable<ProtoId<ReagentPrototype>>) current.FreeReagents);
      component.DispenseSetting = current.DispenseSetting;
      component.Settings = current.Settings;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, RMCChemicalDispenserComponent>(uid, component, ref args1);
    }
  }
}
