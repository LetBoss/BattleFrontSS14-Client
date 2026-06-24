// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Requisitions.Components.RequisitionsRailingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Requisitions.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (SharedRequisitionsSystem)})]
public sealed class RequisitionsRailingComponent : 
  Component,
  ISerializationGenerated<RequisitionsRailingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RequisitionsRailingMode Mode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string LoweredState = "lowered";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string RaisedState = "raised";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string LoweringState = "lowering";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string RaisingState = "raising";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan RailingRaiseDelay = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Fixture = "fix1";
  public object? LowerAnimation;
  public object? RaiseAnimation;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RequisitionsRailingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RequisitionsRailingComponent) target1;
    if (serialization.TryCustomCopy<RequisitionsRailingComponent>(this, ref target, hookCtx, false, context))
      return;
    RequisitionsRailingMode target2 = RequisitionsRailingMode.Lowered;
    if (!serialization.TryCustomCopy<RequisitionsRailingMode>(this.Mode, ref target2, hookCtx, false, context))
      target2 = this.Mode;
    target.Mode = target2;
    string target3 = (string) null;
    if (this.LoweredState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.LoweredState, ref target3, hookCtx, false, context))
      target3 = this.LoweredState;
    target.LoweredState = target3;
    string target4 = (string) null;
    if (this.RaisedState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.RaisedState, ref target4, hookCtx, false, context))
      target4 = this.RaisedState;
    target.RaisedState = target4;
    string target5 = (string) null;
    if (this.LoweringState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.LoweringState, ref target5, hookCtx, false, context))
      target5 = this.LoweringState;
    target.LoweringState = target5;
    string target6 = (string) null;
    if (this.RaisingState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.RaisingState, ref target6, hookCtx, false, context))
      target6 = this.RaisingState;
    target.RaisingState = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RailingRaiseDelay, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.RailingRaiseDelay, hookCtx, context);
    target.RailingRaiseDelay = target7;
    string target8 = (string) null;
    if (this.Fixture == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Fixture, ref target8, hookCtx, false, context))
      target8 = this.Fixture;
    target.Fixture = target8;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RequisitionsRailingComponent target,
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
    RequisitionsRailingComponent target1 = (RequisitionsRailingComponent) target;
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
    RequisitionsRailingComponent target1 = (RequisitionsRailingComponent) target;
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
    RequisitionsRailingComponent target1 = (RequisitionsRailingComponent) target;
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
  virtual RequisitionsRailingComponent Component.Instantiate()
  {
    return new RequisitionsRailingComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RequisitionsRailingComponent_AutoState : IComponentState
  {
    public RequisitionsRailingMode Mode;
    public string LoweredState;
    public string RaisedState;
    public string LoweringState;
    public string RaisingState;
    public TimeSpan RailingRaiseDelay;
    public string Fixture;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RequisitionsRailingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RequisitionsRailingComponent, ComponentGetState>(new ComponentEventRefHandler<RequisitionsRailingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RequisitionsRailingComponent, ComponentHandleState>(new ComponentEventRefHandler<RequisitionsRailingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RequisitionsRailingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RequisitionsRailingComponent.RequisitionsRailingComponent_AutoState()
      {
        Mode = component.Mode,
        LoweredState = component.LoweredState,
        RaisedState = component.RaisedState,
        LoweringState = component.LoweringState,
        RaisingState = component.RaisingState,
        RailingRaiseDelay = component.RailingRaiseDelay,
        Fixture = component.Fixture
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RequisitionsRailingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RequisitionsRailingComponent.RequisitionsRailingComponent_AutoState current))
        return;
      component.Mode = current.Mode;
      component.LoweredState = current.LoweredState;
      component.RaisedState = current.RaisedState;
      component.LoweringState = current.LoweringState;
      component.RaisingState = current.RaisingState;
      component.RailingRaiseDelay = current.RailingRaiseDelay;
      component.Fixture = current.Fixture;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, RequisitionsRailingComponent>(uid, component, ref args1);
    }
  }
}
