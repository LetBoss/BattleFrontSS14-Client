// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Repairable.RMCRepairableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
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
namespace Content.Shared._RMC14.Repairable;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCRepairableSystem)})]
public sealed class RMCRepairableComponent : 
  Component,
  ISerializationGenerated<RMCRepairableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Heal = FixedPoint2.New(50);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(4L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillEngineer";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int SkillRequired;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SoundSpecifier? Sound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/welder.ogg");
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<ToolQualityPrototype> Quality = (ProtoId<ToolQualityPrototype>) "Welding";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 FuelUsed = FixedPoint2.New(0);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RepairableDamageLimit;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCRepairableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCRepairableComponent) target1;
    if (serialization.TryCustomCopy<RMCRepairableComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 target2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Heal, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<FixedPoint2>(this.Heal, hookCtx, context);
    target.Heal = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target3;
    EntProtoId<SkillDefinitionComponent> target4 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.SkillRequired, ref target5, hookCtx, false, context))
      target5 = this.SkillRequired;
    target.SkillRequired = target5;
    SoundSpecifier target6 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.Sound, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<SoundSpecifier>(this.Sound, hookCtx, context);
    target.Sound = target6;
    ProtoId<ToolQualityPrototype> target7 = new ProtoId<ToolQualityPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<ToolQualityPrototype>>(this.Quality, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<ProtoId<ToolQualityPrototype>>(this.Quality, hookCtx, context);
    target.Quality = target7;
    FixedPoint2 target8 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.FuelUsed, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<FixedPoint2>(this.FuelUsed, hookCtx, context);
    target.FuelUsed = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RepairableDamageLimit, ref target9, hookCtx, false, context))
      target9 = this.RepairableDamageLimit;
    target.RepairableDamageLimit = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCRepairableComponent target,
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
    RMCRepairableComponent target1 = (RMCRepairableComponent) target;
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
    RMCRepairableComponent target1 = (RMCRepairableComponent) target;
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
    RMCRepairableComponent target1 = (RMCRepairableComponent) target;
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
  virtual RMCRepairableComponent Component.Instantiate() => new RMCRepairableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCRepairableComponent_AutoState : IComponentState
  {
    public FixedPoint2 Heal;
    public TimeSpan Delay;
    public EntProtoId<SkillDefinitionComponent> Skill;
    public int SkillRequired;
    public SoundSpecifier? Sound;
    public ProtoId<ToolQualityPrototype> Quality;
    public FixedPoint2 FuelUsed;
    public float RepairableDamageLimit;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCRepairableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCRepairableComponent, ComponentGetState>(new ComponentEventRefHandler<RMCRepairableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCRepairableComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCRepairableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCRepairableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCRepairableComponent.RMCRepairableComponent_AutoState()
      {
        Heal = component.Heal,
        Delay = component.Delay,
        Skill = component.Skill,
        SkillRequired = component.SkillRequired,
        Sound = component.Sound,
        Quality = component.Quality,
        FuelUsed = component.FuelUsed,
        RepairableDamageLimit = component.RepairableDamageLimit
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCRepairableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCRepairableComponent.RMCRepairableComponent_AutoState current))
        return;
      component.Heal = current.Heal;
      component.Delay = current.Delay;
      component.Skill = current.Skill;
      component.SkillRequired = current.SkillRequired;
      component.Sound = current.Sound;
      component.Quality = current.Quality;
      component.FuelUsed = current.FuelUsed;
      component.RepairableDamageLimit = current.RepairableDamageLimit;
    }
  }
}
