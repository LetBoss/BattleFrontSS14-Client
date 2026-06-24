// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Communications.CommunicationsTowerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.ManageHive.Boons;
using Content.Shared.FixedPoint;
using Content.Shared.Radio;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Communications;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (CommunicationsTowerSystem)})]
public sealed class CommunicationsTowerComponent : 
  Component,
  ISerializationGenerated<CommunicationsTowerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public CommunicationsTowerState State = CommunicationsTowerState.Off;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<RadioChannelPrototype>> Channels = new HashSet<ProtoId<RadioChannelPrototype>>()
  {
    new ProtoId<RadioChannelPrototype>("Colony")
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 TechPoints = FixedPoint2.New(0.7);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Access(new Type[] {typeof (HiveBoonSystem)})]
  public bool XenoControlled;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CommunicationsTowerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CommunicationsTowerComponent) target1;
    if (serialization.TryCustomCopy<CommunicationsTowerComponent>(this, ref target, hookCtx, false, context))
      return;
    CommunicationsTowerState target2 = CommunicationsTowerState.Broken;
    if (!serialization.TryCustomCopy<CommunicationsTowerState>(this.State, ref target2, hookCtx, false, context))
      target2 = this.State;
    target.State = target2;
    HashSet<ProtoId<RadioChannelPrototype>> target3 = (HashSet<ProtoId<RadioChannelPrototype>>) null;
    if (this.Channels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<RadioChannelPrototype>>>(this.Channels, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<ProtoId<RadioChannelPrototype>>>(this.Channels, hookCtx, context);
    target.Channels = target3;
    FixedPoint2 target4 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.TechPoints, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<FixedPoint2>(this.TechPoints, hookCtx, context);
    target.TechPoints = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.XenoControlled, ref target5, hookCtx, false, context))
      target5 = this.XenoControlled;
    target.XenoControlled = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CommunicationsTowerComponent target,
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
    CommunicationsTowerComponent target1 = (CommunicationsTowerComponent) target;
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
    CommunicationsTowerComponent target1 = (CommunicationsTowerComponent) target;
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
    CommunicationsTowerComponent target1 = (CommunicationsTowerComponent) target;
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
  virtual CommunicationsTowerComponent Component.Instantiate()
  {
    return new CommunicationsTowerComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class CommunicationsTowerComponent_AutoState : IComponentState
  {
    public CommunicationsTowerState State;
    public HashSet<ProtoId<RadioChannelPrototype>> Channels;
    public FixedPoint2 TechPoints;
    public bool XenoControlled;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CommunicationsTowerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<CommunicationsTowerComponent, ComponentGetState>(new ComponentEventRefHandler<CommunicationsTowerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<CommunicationsTowerComponent, ComponentHandleState>(new ComponentEventRefHandler<CommunicationsTowerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      CommunicationsTowerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new CommunicationsTowerComponent.CommunicationsTowerComponent_AutoState()
      {
        State = component.State,
        Channels = component.Channels,
        TechPoints = component.TechPoints,
        XenoControlled = component.XenoControlled
      };
    }

    private void OnHandleState(
      EntityUid uid,
      CommunicationsTowerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is CommunicationsTowerComponent.CommunicationsTowerComponent_AutoState current))
        return;
      component.State = current.State;
      component.Channels = current.Channels == null ? (HashSet<ProtoId<RadioChannelPrototype>>) null : new HashSet<ProtoId<RadioChannelPrototype>>((IEnumerable<ProtoId<RadioChannelPrototype>>) current.Channels);
      component.TechPoints = current.TechPoints;
      component.XenoControlled = current.XenoControlled;
    }
  }
}
