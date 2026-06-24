// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Squads.SquadLeaderHeadsetComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Marines.Squads;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SquadSystem)})]
public sealed class SquadLeaderHeadsetComponent : 
  Component,
  ISerializationGenerated<SquadLeaderHeadsetComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<RadioChannelPrototype>> Channels = new HashSet<ProtoId<RadioChannelPrototype>>()
  {
    (ProtoId<RadioChannelPrototype>) "MarineJTAC",
    (ProtoId<RadioChannelPrototype>) "MarineCommand"
  };
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid Leader;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SquadLeaderHeadsetComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SquadLeaderHeadsetComponent) target1;
    if (serialization.TryCustomCopy<SquadLeaderHeadsetComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<ProtoId<RadioChannelPrototype>> target2 = (HashSet<ProtoId<RadioChannelPrototype>>) null;
    if (this.Channels == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<RadioChannelPrototype>>>(this.Channels, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<HashSet<ProtoId<RadioChannelPrototype>>>(this.Channels, hookCtx, context);
    target.Channels = target2;
    EntityUid target3 = new EntityUid();
    if (!serialization.TryCustomCopy<EntityUid>(this.Leader, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid>(this.Leader, hookCtx, context);
    target.Leader = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SquadLeaderHeadsetComponent target,
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
    SquadLeaderHeadsetComponent target1 = (SquadLeaderHeadsetComponent) target;
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
    SquadLeaderHeadsetComponent target1 = (SquadLeaderHeadsetComponent) target;
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
    SquadLeaderHeadsetComponent target1 = (SquadLeaderHeadsetComponent) target;
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
  virtual SquadLeaderHeadsetComponent Component.Instantiate() => new SquadLeaderHeadsetComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SquadLeaderHeadsetComponent_AutoState : IComponentState
  {
    public HashSet<ProtoId<RadioChannelPrototype>> Channels;
    public NetEntity Leader;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SquadLeaderHeadsetComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SquadLeaderHeadsetComponent, ComponentGetState>(new ComponentEventRefHandler<SquadLeaderHeadsetComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SquadLeaderHeadsetComponent, ComponentHandleState>(new ComponentEventRefHandler<SquadLeaderHeadsetComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SquadLeaderHeadsetComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SquadLeaderHeadsetComponent.SquadLeaderHeadsetComponent_AutoState()
      {
        Channels = component.Channels,
        Leader = this.GetNetEntity(component.Leader)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SquadLeaderHeadsetComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SquadLeaderHeadsetComponent.SquadLeaderHeadsetComponent_AutoState current))
        return;
      component.Channels = current.Channels == null ? (HashSet<ProtoId<RadioChannelPrototype>>) null : new HashSet<ProtoId<RadioChannelPrototype>>((IEnumerable<ProtoId<RadioChannelPrototype>>) current.Channels);
      component.Leader = this.EnsureEntity<SquadLeaderHeadsetComponent>(current.Leader, uid);
    }
  }
}
