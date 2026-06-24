// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.HardpointIntegrityComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Content.Shared.Tools;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class HardpointIntegrityComponent : 
  Component,
  ISerializationGenerated<HardpointIntegrityComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float RepairTimePerIntegrity = 0.01f;
  [DataField(null, false, 1, false, false, null)]
  public float RepairTimeMin = 0.25f;
  [DataField(null, false, 1, false, false, null)]
  public float RepairTimeMax = 3f;
  [DataField(null, false, 1, false, false, null)]
  public float MaxIntegrity = 100f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Integrity;
  [DataField(null, false, 1, false, false, null)]
  public FixedPoint2 RepairFuelCost = FixedPoint2.New(5);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? RepairSound;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ToolQualityPrototype> RepairToolQuality = (ProtoId<ToolQualityPrototype>) "Welding";
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<ToolQualityPrototype> FrameFinishToolQuality = (ProtoId<ToolQualityPrototype>) "Anchoring";
  [DataField(null, false, 1, false, false, null)]
  public float FrameWeldCapFraction = 0.75f;
  [DataField(null, false, 1, false, false, null)]
  public float FrameRepairEpsilon = 0.01f;
  [DataField(null, false, 1, false, false, null)]
  public float RepairCapFraction = 0.9f;
  [DataField(null, false, 1, false, false, null)]
  public float RepairChunkFraction = 0.05f;
  [DataField(null, false, 1, false, false, null)]
  public float RepairChunkMinimum = 0.01f;
  [DataField(null, false, 1, false, false, null)]
  public float FrameRepairChunkSeconds = 2f;
  [DataField(null, false, 1, false, false, null)]
  public float ModuleRepairSeconds = 5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BypassEntryOnZero;
  [NonSerialized]
  public bool Repairing;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HardpointIntegrityComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HardpointIntegrityComponent) target1;
    if (serialization.TryCustomCopy<HardpointIntegrityComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RepairTimePerIntegrity, ref target2, hookCtx, false, context))
      target2 = this.RepairTimePerIntegrity;
    target.RepairTimePerIntegrity = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RepairTimeMin, ref target3, hookCtx, false, context))
      target3 = this.RepairTimeMin;
    target.RepairTimeMin = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RepairTimeMax, ref target4, hookCtx, false, context))
      target4 = this.RepairTimeMax;
    target.RepairTimeMax = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxIntegrity, ref target5, hookCtx, false, context))
      target5 = this.MaxIntegrity;
    target.MaxIntegrity = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Integrity, ref target6, hookCtx, false, context))
      target6 = this.Integrity;
    target.Integrity = target6;
    FixedPoint2 target7 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.RepairFuelCost, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<FixedPoint2>(this.RepairFuelCost, hookCtx, context);
    target.RepairFuelCost = target7;
    SoundSpecifier target8 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RepairSound, ref target8, hookCtx, true, context))
      target8 = serialization.CreateCopy<SoundSpecifier>(this.RepairSound, hookCtx, context);
    target.RepairSound = target8;
    ProtoId<ToolQualityPrototype> target9 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.RepairToolQuality, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.RepairToolQuality, hookCtx, context);
    target.RepairToolQuality = target9;
    ProtoId<ToolQualityPrototype> target10 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.FrameFinishToolQuality, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.FrameFinishToolQuality, hookCtx, context);
    target.FrameFinishToolQuality = target10;
    float target11 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FrameWeldCapFraction, ref target11, hookCtx, false, context))
      target11 = this.FrameWeldCapFraction;
    target.FrameWeldCapFraction = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FrameRepairEpsilon, ref target12, hookCtx, false, context))
      target12 = this.FrameRepairEpsilon;
    target.FrameRepairEpsilon = target12;
    float target13 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RepairCapFraction, ref target13, hookCtx, false, context))
      target13 = this.RepairCapFraction;
    target.RepairCapFraction = target13;
    float target14 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RepairChunkFraction, ref target14, hookCtx, false, context))
      target14 = this.RepairChunkFraction;
    target.RepairChunkFraction = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RepairChunkMinimum, ref target15, hookCtx, false, context))
      target15 = this.RepairChunkMinimum;
    target.RepairChunkMinimum = target15;
    float target16 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FrameRepairChunkSeconds, ref target16, hookCtx, false, context))
      target16 = this.FrameRepairChunkSeconds;
    target.FrameRepairChunkSeconds = target16;
    float target17 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ModuleRepairSeconds, ref target17, hookCtx, false, context))
      target17 = this.ModuleRepairSeconds;
    target.ModuleRepairSeconds = target17;
    bool target18 = false;
    if (!serialization.TryCustomCopy<bool>(this.BypassEntryOnZero, ref target18, hookCtx, false, context))
      target18 = this.BypassEntryOnZero;
    target.BypassEntryOnZero = target18;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HardpointIntegrityComponent target,
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
    HardpointIntegrityComponent target1 = (HardpointIntegrityComponent) target;
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
    HardpointIntegrityComponent target1 = (HardpointIntegrityComponent) target;
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
    HardpointIntegrityComponent target1 = (HardpointIntegrityComponent) target;
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
  virtual HardpointIntegrityComponent Component.Instantiate() => new HardpointIntegrityComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class HardpointIntegrityComponent_AutoState : IComponentState
  {
    public float Integrity;
    public bool BypassEntryOnZero;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HardpointIntegrityComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HardpointIntegrityComponent, ComponentGetState>(new ComponentEventRefHandler<HardpointIntegrityComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<HardpointIntegrityComponent, ComponentHandleState>(new ComponentEventRefHandler<HardpointIntegrityComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      HardpointIntegrityComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new HardpointIntegrityComponent.HardpointIntegrityComponent_AutoState()
      {
        Integrity = component.Integrity,
        BypassEntryOnZero = component.BypassEntryOnZero
      };
    }

    private void OnHandleState(
      EntityUid uid,
      HardpointIntegrityComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is HardpointIntegrityComponent.HardpointIntegrityComponent_AutoState current))
        return;
      component.Integrity = current.Integrity;
      component.BypassEntryOnZero = current.BypassEntryOnZero;
    }
  }
}
