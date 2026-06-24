// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Sensor.SensorTowerComponent
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
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Sensor;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SensorTowerSystem)})]
public sealed class SensorTowerComponent : 
  Component,
  ISerializationGenerated<SensorTowerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillEngineer";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SkillLevel = 2;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SensorTowerState State;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> WeldingQuality = (ProtoId<ToolQualityPrototype>) "Welding";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan WeldingDelay = TimeSpan.FromSeconds(20L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float WeldingCost = 1f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> CuttingQuality = (ProtoId<ToolQualityPrototype>) "Cutting";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan CuttingDelay = TimeSpan.FromSeconds(12L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> WrenchQuality = (ProtoId<ToolQualityPrototype>) "Anchoring";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan WrenchDelay = TimeSpan.FromSeconds(15L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float BreakChance = 0.15f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BreakEvery = TimeSpan.FromSeconds(175L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextBreakAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DestroyDelay = TimeSpan.FromSeconds(4L);

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SensorTowerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SensorTowerComponent) target1;
    if (serialization.TryCustomCopy<SensorTowerComponent>(this, ref target, hookCtx, false, context))
      return;
    EntProtoId<SkillDefinitionComponent> target2 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.SkillLevel, ref target3, hookCtx, false, context))
      target3 = this.SkillLevel;
    target.SkillLevel = target3;
    SensorTowerState target4 = SensorTowerState.Weld;
    if (!serialization.TryCustomCopy<SensorTowerState>(this.State, ref target4, hookCtx, false, context))
      target4 = this.State;
    target.State = target4;
    ProtoId<ToolQualityPrototype> target5 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.WeldingQuality, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.WeldingQuality, hookCtx, context);
    target.WeldingQuality = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.WeldingDelay, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.WeldingDelay, hookCtx, context);
    target.WeldingDelay = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WeldingCost, ref target7, hookCtx, false, context))
      target7 = this.WeldingCost;
    target.WeldingCost = target7;
    ProtoId<ToolQualityPrototype> target8 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.CuttingQuality, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.CuttingQuality, hookCtx, context);
    target.CuttingQuality = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.CuttingDelay, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.CuttingDelay, hookCtx, context);
    target.CuttingDelay = target9;
    ProtoId<ToolQualityPrototype> target10 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.WrenchQuality, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.WrenchQuality, hookCtx, context);
    target.WrenchQuality = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.WrenchDelay, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.WrenchDelay, hookCtx, context);
    target.WrenchDelay = target11;
    float target12 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BreakChance, ref target12, hookCtx, false, context))
      target12 = this.BreakChance;
    target.BreakChance = target12;
    TimeSpan target13 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BreakEvery, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan>(this.BreakEvery, hookCtx, context);
    target.BreakEvery = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextBreakAt, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.NextBreakAt, hookCtx, context);
    target.NextBreakAt = target14;
    TimeSpan target15 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DestroyDelay, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan>(this.DestroyDelay, hookCtx, context);
    target.DestroyDelay = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SensorTowerComponent target,
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
    SensorTowerComponent target1 = (SensorTowerComponent) target;
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
    SensorTowerComponent target1 = (SensorTowerComponent) target;
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
    SensorTowerComponent target1 = (SensorTowerComponent) target;
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
  virtual SensorTowerComponent Component.Instantiate() => new SensorTowerComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SensorTowerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SensorTowerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<SensorTowerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      SensorTowerComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextBreakAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SensorTowerComponent_AutoState : IComponentState
  {
    public EntProtoId<
    #nullable enable
    SkillDefinitionComponent> Skill;
    public int SkillLevel;
    public SensorTowerState State;
    public ProtoId<ToolQualityPrototype> WeldingQuality;
    public TimeSpan WeldingDelay;
    public float WeldingCost;
    public ProtoId<ToolQualityPrototype> CuttingQuality;
    public TimeSpan CuttingDelay;
    public ProtoId<ToolQualityPrototype> WrenchQuality;
    public TimeSpan WrenchDelay;
    public float BreakChance;
    public TimeSpan BreakEvery;
    public TimeSpan NextBreakAt;
    public TimeSpan DestroyDelay;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SensorTowerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SensorTowerComponent, ComponentGetState>(new ComponentEventRefHandler<SensorTowerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SensorTowerComponent, ComponentHandleState>(new ComponentEventRefHandler<SensorTowerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SensorTowerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SensorTowerComponent.SensorTowerComponent_AutoState()
      {
        Skill = component.Skill,
        SkillLevel = component.SkillLevel,
        State = component.State,
        WeldingQuality = component.WeldingQuality,
        WeldingDelay = component.WeldingDelay,
        WeldingCost = component.WeldingCost,
        CuttingQuality = component.CuttingQuality,
        CuttingDelay = component.CuttingDelay,
        WrenchQuality = component.WrenchQuality,
        WrenchDelay = component.WrenchDelay,
        BreakChance = component.BreakChance,
        BreakEvery = component.BreakEvery,
        NextBreakAt = component.NextBreakAt,
        DestroyDelay = component.DestroyDelay
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SensorTowerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SensorTowerComponent.SensorTowerComponent_AutoState current))
        return;
      component.Skill = current.Skill;
      component.SkillLevel = current.SkillLevel;
      component.State = current.State;
      component.WeldingQuality = current.WeldingQuality;
      component.WeldingDelay = current.WeldingDelay;
      component.WeldingCost = current.WeldingCost;
      component.CuttingQuality = current.CuttingQuality;
      component.CuttingDelay = current.CuttingDelay;
      component.WrenchQuality = current.WrenchQuality;
      component.WrenchDelay = current.WrenchDelay;
      component.BreakChance = current.BreakChance;
      component.BreakEvery = current.BreakEvery;
      component.NextBreakAt = current.NextBreakAt;
      component.DestroyDelay = current.DestroyDelay;
    }
  }
}
