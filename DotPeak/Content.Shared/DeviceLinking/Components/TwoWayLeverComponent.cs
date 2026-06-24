// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceLinking.Components.TwoWayLeverComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared.DeviceLinking.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class TwoWayLeverComponent : 
  Component,
  ISerializationGenerated<TwoWayLeverComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TwoWayLeverState State;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool NextSignalLeft;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<SourcePortPrototype> LeftPort = ProtoId<SourcePortPrototype>.op_Implicit("Left");
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<SourcePortPrototype> RightPort = ProtoId<SourcePortPrototype>.op_Implicit("Right");
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<SourcePortPrototype> MiddlePort = ProtoId<SourcePortPrototype>.op_Implicit("Middle");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TwoWayLeverComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (TwoWayLeverComponent) component;
    if (serialization.TryCustomCopy<TwoWayLeverComponent>(this, ref target, hookCtx, false, context))
      return;
    TwoWayLeverState twoWayLeverState = TwoWayLeverState.Middle;
    if (!serialization.TryCustomCopy<TwoWayLeverState>(this.State, ref twoWayLeverState, hookCtx, false, context))
      twoWayLeverState = this.State;
    target.State = twoWayLeverState;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.NextSignalLeft, ref flag, hookCtx, false, context))
      flag = this.NextSignalLeft;
    target.NextSignalLeft = flag;
    ProtoId<SourcePortPrototype> protoId1 = new ProtoId<SourcePortPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<SourcePortPrototype>>(this.LeftPort, ref protoId1, hookCtx, false, context))
      protoId1 = serialization.CreateCopy<ProtoId<SourcePortPrototype>>(this.LeftPort, hookCtx, context, false);
    target.LeftPort = protoId1;
    ProtoId<SourcePortPrototype> protoId2 = new ProtoId<SourcePortPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<SourcePortPrototype>>(this.RightPort, ref protoId2, hookCtx, false, context))
      protoId2 = serialization.CreateCopy<ProtoId<SourcePortPrototype>>(this.RightPort, hookCtx, context, false);
    target.RightPort = protoId2;
    ProtoId<SourcePortPrototype> protoId3 = new ProtoId<SourcePortPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<SourcePortPrototype>>(this.MiddlePort, ref protoId3, hookCtx, false, context))
      protoId3 = serialization.CreateCopy<ProtoId<SourcePortPrototype>>(this.MiddlePort, hookCtx, context, false);
    target.MiddlePort = protoId3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TwoWayLeverComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TwoWayLeverComponent target1 = (TwoWayLeverComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TwoWayLeverComponent target1 = (TwoWayLeverComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TwoWayLeverComponent target1 = (TwoWayLeverComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual TwoWayLeverComponent Component.Instantiate() => new TwoWayLeverComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TwoWayLeverComponent_AutoState : IComponentState
  {
    public TwoWayLeverState State;
    public bool NextSignalLeft;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TwoWayLeverComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<TwoWayLeverComponent, ComponentGetState>(new ComponentEventRefHandler<TwoWayLeverComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<TwoWayLeverComponent, ComponentHandleState>(new ComponentEventRefHandler<TwoWayLeverComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      TwoWayLeverComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new TwoWayLeverComponent.TwoWayLeverComponent_AutoState()
      {
        State = component.State,
        NextSignalLeft = component.NextSignalLeft
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TwoWayLeverComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is TwoWayLeverComponent.TwoWayLeverComponent_AutoState current))
        return;
      component.State = current.State;
      component.NextSignalLeft = current.NextSignalLeft;
    }
  }
}
