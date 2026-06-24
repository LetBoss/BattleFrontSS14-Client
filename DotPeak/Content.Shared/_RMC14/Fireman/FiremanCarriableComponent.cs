// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Fireman.FiremanCarriableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
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
namespace Content.Shared._RMC14.Fireman;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (FiremanCarrySystem)})]
public sealed class FiremanCarriableComponent : 
  Component,
  ISerializationGenerated<FiremanCarriableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Delay = TimeSpan.FromSeconds(3L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BeingCarried;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SkillDefinitionComponent> Skill = (EntProtoId<SkillDefinitionComponent>) "RMCSkillFireman";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan BreakDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool BreakingFree;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FiremanCarriableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (FiremanCarriableComponent) target1;
    if (serialization.TryCustomCopy<FiremanCarriableComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context);
    target.Delay = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.BeingCarried, ref target3, hookCtx, false, context))
      target3 = this.BeingCarried;
    target.BeingCarried = target3;
    EntProtoId<SkillDefinitionComponent> target4 = new EntProtoId<SkillDefinitionComponent>();
    if (!serialization.TryCustomCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId<SkillDefinitionComponent>>(this.Skill, hookCtx, context);
    target.Skill = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.BreakDelay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.BreakDelay, hookCtx, context);
    target.BreakDelay = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakingFree, ref target6, hookCtx, false, context))
      target6 = this.BreakingFree;
    target.BreakingFree = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FiremanCarriableComponent target,
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
    FiremanCarriableComponent target1 = (FiremanCarriableComponent) target;
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
    FiremanCarriableComponent target1 = (FiremanCarriableComponent) target;
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
    FiremanCarriableComponent target1 = (FiremanCarriableComponent) target;
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
  virtual FiremanCarriableComponent Component.Instantiate() => new FiremanCarriableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class FiremanCarriableComponent_AutoState : IComponentState
  {
    public TimeSpan Delay;
    public bool BeingCarried;
    public EntProtoId<SkillDefinitionComponent> Skill;
    public TimeSpan BreakDelay;
    public bool BreakingFree;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FiremanCarriableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<FiremanCarriableComponent, ComponentGetState>(new ComponentEventRefHandler<FiremanCarriableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<FiremanCarriableComponent, ComponentHandleState>(new ComponentEventRefHandler<FiremanCarriableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      FiremanCarriableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new FiremanCarriableComponent.FiremanCarriableComponent_AutoState()
      {
        Delay = component.Delay,
        BeingCarried = component.BeingCarried,
        Skill = component.Skill,
        BreakDelay = component.BreakDelay,
        BreakingFree = component.BreakingFree
      };
    }

    private void OnHandleState(
      EntityUid uid,
      FiremanCarriableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is FiremanCarriableComponent.FiremanCarriableComponent_AutoState current))
        return;
      component.Delay = current.Delay;
      component.BeingCarried = current.BeingCarried;
      component.Skill = current.Skill;
      component.BreakDelay = current.BreakDelay;
      component.BreakingFree = current.BreakingFree;
    }
  }
}
