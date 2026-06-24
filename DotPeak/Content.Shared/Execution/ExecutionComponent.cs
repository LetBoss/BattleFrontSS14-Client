// Decompiled with JetBrains decompiler
// Type: Content.Shared.Execution.ExecutionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Execution;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ExecutionComponent : 
  Component,
  ISerializationGenerated<ExecutionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DoAfterDuration = 5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DamageMultiplier = 9f;
  [DataField(null, false, 1, false, false, null)]
  public LocId InternalMeleeExecutionMessage = (LocId) "execution-popup-melee-initial-internal";
  [DataField(null, false, 1, false, false, null)]
  public LocId ExternalMeleeExecutionMessage = (LocId) "execution-popup-melee-initial-external";
  [DataField(null, false, 1, false, false, null)]
  public LocId CompleteInternalMeleeExecutionMessage = (LocId) "execution-popup-melee-complete-internal";
  [DataField(null, false, 1, false, false, null)]
  public LocId CompleteExternalMeleeExecutionMessage = (LocId) "execution-popup-melee-complete-external";
  [DataField(null, false, 1, false, false, null)]
  public LocId InternalSelfExecutionMessage = (LocId) "execution-popup-self-initial-internal";
  [DataField(null, false, 1, false, false, null)]
  public LocId ExternalSelfExecutionMessage = (LocId) "execution-popup-self-initial-external";
  [DataField(null, false, 1, false, false, null)]
  public LocId CompleteInternalSelfExecutionMessage = (LocId) "execution-popup-self-complete-internal";
  [DataField(null, false, 1, false, false, null)]
  public LocId CompleteExternalSelfExecutionMessage = (LocId) "execution-popup-self-complete-external";
  [DataField(null, false, 1, false, false, null)]
  public bool Executing;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ExecutionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ExecutionComponent) target1;
    if (serialization.TryCustomCopy<ExecutionComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DoAfterDuration, ref target2, hookCtx, false, context))
      target2 = this.DoAfterDuration;
    target.DoAfterDuration = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DamageMultiplier, ref target3, hookCtx, false, context))
      target3 = this.DamageMultiplier;
    target.DamageMultiplier = target3;
    LocId target4 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.InternalMeleeExecutionMessage, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<LocId>(this.InternalMeleeExecutionMessage, hookCtx, context);
    target.InternalMeleeExecutionMessage = target4;
    LocId target5 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ExternalMeleeExecutionMessage, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<LocId>(this.ExternalMeleeExecutionMessage, hookCtx, context);
    target.ExternalMeleeExecutionMessage = target5;
    LocId target6 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.CompleteInternalMeleeExecutionMessage, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<LocId>(this.CompleteInternalMeleeExecutionMessage, hookCtx, context);
    target.CompleteInternalMeleeExecutionMessage = target6;
    LocId target7 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.CompleteExternalMeleeExecutionMessage, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<LocId>(this.CompleteExternalMeleeExecutionMessage, hookCtx, context);
    target.CompleteExternalMeleeExecutionMessage = target7;
    LocId target8 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.InternalSelfExecutionMessage, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<LocId>(this.InternalSelfExecutionMessage, hookCtx, context);
    target.InternalSelfExecutionMessage = target8;
    LocId target9 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ExternalSelfExecutionMessage, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<LocId>(this.ExternalSelfExecutionMessage, hookCtx, context);
    target.ExternalSelfExecutionMessage = target9;
    LocId target10 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.CompleteInternalSelfExecutionMessage, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<LocId>(this.CompleteInternalSelfExecutionMessage, hookCtx, context);
    target.CompleteInternalSelfExecutionMessage = target10;
    LocId target11 = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.CompleteExternalSelfExecutionMessage, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<LocId>(this.CompleteExternalSelfExecutionMessage, hookCtx, context);
    target.CompleteExternalSelfExecutionMessage = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.Executing, ref target12, hookCtx, false, context))
      target12 = this.Executing;
    target.Executing = target12;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ExecutionComponent target,
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
    ExecutionComponent target1 = (ExecutionComponent) target;
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
    ExecutionComponent target1 = (ExecutionComponent) target;
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
    ExecutionComponent target1 = (ExecutionComponent) target;
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
  virtual ExecutionComponent Component.Instantiate() => new ExecutionComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ExecutionComponent_AutoState : IComponentState
  {
    public float DoAfterDuration;
    public float DamageMultiplier;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ExecutionComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ExecutionComponent, ComponentGetState>(new ComponentEventRefHandler<ExecutionComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ExecutionComponent, ComponentHandleState>(new ComponentEventRefHandler<ExecutionComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      ExecutionComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ExecutionComponent.ExecutionComponent_AutoState()
      {
        DoAfterDuration = component.DoAfterDuration,
        DamageMultiplier = component.DamageMultiplier
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ExecutionComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ExecutionComponent.ExecutionComponent_AutoState current))
        return;
      component.DoAfterDuration = current.DoAfterDuration;
      component.DamageMultiplier = current.DamageMultiplier;
    }
  }
}
