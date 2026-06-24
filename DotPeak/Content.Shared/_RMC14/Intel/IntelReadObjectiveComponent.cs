// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Intel.IntelReadObjectiveComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
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
namespace Content.Shared._RMC14.Intel;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (IntelSystem)})]
public sealed class IntelReadObjectiveComponent : 
  Component,
  ISerializationGenerated<IntelReadObjectiveComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public IntelObjectiveState State;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 Value = FixedPoint2.New(0.1);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillIntel";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IntelReadObjectiveComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (IntelReadObjectiveComponent) target1;
    if (serialization.TryCustomCopy<IntelReadObjectiveComponent>(this, ref target, hookCtx, false, context))
      return;
    IntelObjectiveState target2 = IntelObjectiveState.Inactive;
    if (!serialization.TryCustomCopy<IntelObjectiveState>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target.State = target2;
    FixedPoint2 target3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Value, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FixedPoint2>(this.Value, hookCtx, context);
    target.Value = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target4;
    EntProtoId<SkillDefinitionComponent> target5 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IntelReadObjectiveComponent target,
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
    IntelReadObjectiveComponent target1 = (IntelReadObjectiveComponent) target;
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
    IntelReadObjectiveComponent target1 = (IntelReadObjectiveComponent) target;
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
    IntelReadObjectiveComponent target1 = (IntelReadObjectiveComponent) target;
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
  virtual IntelReadObjectiveComponent Component.Instantiate() => new IntelReadObjectiveComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class IntelReadObjectiveComponent_AutoState : IComponentState
  {
    public IntelObjectiveState State;
    public FixedPoint2 Value;
    public TimeSpan Delay;
    public EntProtoId<SkillDefinitionComponent> Skill;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class IntelReadObjectiveComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<IntelReadObjectiveComponent, ComponentGetState>(new ComponentEventRefHandler<IntelReadObjectiveComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<IntelReadObjectiveComponent, ComponentHandleState>(new ComponentEventRefHandler<IntelReadObjectiveComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      IntelReadObjectiveComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new IntelReadObjectiveComponent.IntelReadObjectiveComponent_AutoState()
      {
        State = component.State,
        Value = component.Value,
        Delay = component.Delay,
        Skill = component.Skill
      };
    }

    private void OnHandleState(
      EntityUid uid,
      IntelReadObjectiveComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is IntelReadObjectiveComponent.IntelReadObjectiveComponent_AutoState current))
        return;
      component.State = current.State;
      component.Value = current.Value;
      component.Delay = current.Delay;
      component.Skill = current.Skill;
    }
  }
}
