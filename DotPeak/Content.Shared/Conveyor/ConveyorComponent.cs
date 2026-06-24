// Decompiled with JetBrains decompiler
// Type: Content.Shared.Conveyor.ConveyorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceLinking;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Conveyor;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class ConveyorComponent : 
  Component,
  ISerializationGenerated<ConveyorComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle Angle = Angle.Zero;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Speed = 2f;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public ConveyorState State;
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public bool Powered;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<SinkPortPrototype> ForwardPort = ProtoId<SinkPortPrototype>.op_Implicit("Forward");
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<SinkPortPrototype> ReversePort = ProtoId<SinkPortPrototype>.op_Implicit("Reverse");
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<SinkPortPrototype> OffPort = ProtoId<SinkPortPrototype>.op_Implicit("Off");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ConveyorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ConveyorComponent) component;
    if (serialization.TryCustomCopy<ConveyorComponent>(this, ref target, hookCtx, false, context))
      return;
    Angle angle = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.Angle, ref angle, hookCtx, false, context))
      angle = serialization.CreateCopy<Angle>(this.Angle, hookCtx, context, false);
    target.Angle = angle;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Speed, ref num, hookCtx, false, context))
      num = this.Speed;
    target.Speed = num;
    ProtoId<SinkPortPrototype> protoId1 = new ProtoId<SinkPortPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<SinkPortPrototype>>(this.ForwardPort, ref protoId1, hookCtx, false, context))
      protoId1 = serialization.CreateCopy<ProtoId<SinkPortPrototype>>(this.ForwardPort, hookCtx, context, false);
    target.ForwardPort = protoId1;
    ProtoId<SinkPortPrototype> protoId2 = new ProtoId<SinkPortPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<SinkPortPrototype>>(this.ReversePort, ref protoId2, hookCtx, false, context))
      protoId2 = serialization.CreateCopy<ProtoId<SinkPortPrototype>>(this.ReversePort, hookCtx, context, false);
    target.ReversePort = protoId2;
    ProtoId<SinkPortPrototype> protoId3 = new ProtoId<SinkPortPrototype>();
    if (!serialization.TryCustomCopy<ProtoId<SinkPortPrototype>>(this.OffPort, ref protoId3, hookCtx, false, context))
      protoId3 = serialization.CreateCopy<ProtoId<SinkPortPrototype>>(this.OffPort, hookCtx, context, false);
    target.OffPort = protoId3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ConveyorComponent target,
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
    ConveyorComponent target1 = (ConveyorComponent) target;
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
    ConveyorComponent target1 = (ConveyorComponent) target;
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
    ConveyorComponent target1 = (ConveyorComponent) target;
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
  virtual ConveyorComponent Component.Instantiate() => new ConveyorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ConveyorComponent_AutoState : IComponentState
  {
    public Angle Angle;
    public float Speed;
    public ConveyorState State;
    public bool Powered;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ConveyorComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ConveyorComponent, ComponentGetState>(new ComponentEventRefHandler<ConveyorComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ConveyorComponent, ComponentHandleState>(new ComponentEventRefHandler<ConveyorComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, ConveyorComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new ConveyorComponent.ConveyorComponent_AutoState()
      {
        Angle = component.Angle,
        Speed = component.Speed,
        State = component.State,
        Powered = component.Powered
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ConveyorComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is ConveyorComponent.ConveyorComponent_AutoState current))
        return;
      component.Angle = current.Angle;
      component.Speed = current.Speed;
      component.State = current.State;
      component.Powered = current.Powered;
    }
  }
}
