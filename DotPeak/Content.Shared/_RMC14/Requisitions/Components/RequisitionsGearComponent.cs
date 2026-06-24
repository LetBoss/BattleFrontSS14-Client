// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Requisitions.Components.RequisitionsGearComponent
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
public sealed class RequisitionsGearComponent : 
  Component,
  ISerializationGenerated<RequisitionsGearComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RequisitionsGearMode Mode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string StaticState = "base";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string MovingState = "moving";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RequisitionsGearComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RequisitionsGearComponent) target1;
    if (serialization.TryCustomCopy<RequisitionsGearComponent>(this, ref target, hookCtx, false, context))
      return;
    RequisitionsGearMode target2 = RequisitionsGearMode.Static;
    if (!serialization.TryCustomCopy<RequisitionsGearMode>(this.Mode, ref target2, hookCtx, false, context))
      target2 = this.Mode;
    target.Mode = target2;
    string target3 = (string) null;
    if (this.StaticState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.StaticState, ref target3, hookCtx, false, context))
      target3 = this.StaticState;
    target.StaticState = target3;
    string target4 = (string) null;
    if (this.MovingState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.MovingState, ref target4, hookCtx, false, context))
      target4 = this.MovingState;
    target.MovingState = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RequisitionsGearComponent target,
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
    RequisitionsGearComponent target1 = (RequisitionsGearComponent) target;
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
    RequisitionsGearComponent target1 = (RequisitionsGearComponent) target;
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
    RequisitionsGearComponent target1 = (RequisitionsGearComponent) target;
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
  virtual RequisitionsGearComponent Component.Instantiate() => new RequisitionsGearComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RequisitionsGearComponent_AutoState : IComponentState
  {
    public RequisitionsGearMode Mode;
    public string StaticState;
    public string MovingState;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RequisitionsGearComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RequisitionsGearComponent, ComponentGetState>(new ComponentEventRefHandler<RequisitionsGearComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RequisitionsGearComponent, ComponentHandleState>(new ComponentEventRefHandler<RequisitionsGearComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RequisitionsGearComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RequisitionsGearComponent.RequisitionsGearComponent_AutoState()
      {
        Mode = component.Mode,
        StaticState = component.StaticState,
        MovingState = component.MovingState
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RequisitionsGearComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RequisitionsGearComponent.RequisitionsGearComponent_AutoState current))
        return;
      component.Mode = current.Mode;
      component.StaticState = current.StaticState;
      component.MovingState = current.MovingState;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, RequisitionsGearComponent>(uid, component, ref args1);
    }
  }
}
