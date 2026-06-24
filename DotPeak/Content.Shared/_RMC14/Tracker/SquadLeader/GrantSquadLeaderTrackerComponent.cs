// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tracker.SquadLeader.GrantSquadLeaderTrackerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
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
namespace Content.Shared._RMC14.Tracker.SquadLeader;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SquadLeaderTrackerSystem)})]
public sealed class GrantSquadLeaderTrackerComponent : 
  Component,
  IClothingSlots,
  ISerializationGenerated<GrantSquadLeaderTrackerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public ProtoId<TrackerModePrototype> DefaultMode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<TrackerModePrototype>> TrackerModes = new HashSet<ProtoId<TrackerModePrototype>>();

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public SlotFlags Slots { get; set; } = SlotFlags.EARS;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GrantSquadLeaderTrackerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GrantSquadLeaderTrackerComponent) target1;
    if (serialization.TryCustomCopy<GrantSquadLeaderTrackerComponent>(this, ref target, hookCtx, false, context))
      return;
    SlotFlags target2 = SlotFlags.NONE;
    if (!serialization.TryCustomCopy<SlotFlags>(this.Slots, ref target2, hookCtx, false, context))
      target2 = this.Slots;
    target.Slots = target2;
    ProtoId<TrackerModePrototype> target3 = new ProtoId<TrackerModePrototype>();
    if (!serialization.TryCustomCopy<ProtoId<TrackerModePrototype>>(this.DefaultMode, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ProtoId<TrackerModePrototype>>(this.DefaultMode, hookCtx, context);
    target.DefaultMode = target3;
    HashSet<ProtoId<TrackerModePrototype>> target4 = (HashSet<ProtoId<TrackerModePrototype>>) null;
    if (this.TrackerModes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<TrackerModePrototype>>>(this.TrackerModes, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<HashSet<ProtoId<TrackerModePrototype>>>(this.TrackerModes, hookCtx, context);
    target.TrackerModes = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GrantSquadLeaderTrackerComponent target,
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
    GrantSquadLeaderTrackerComponent target1 = (GrantSquadLeaderTrackerComponent) target;
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
    GrantSquadLeaderTrackerComponent target1 = (GrantSquadLeaderTrackerComponent) target;
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
    GrantSquadLeaderTrackerComponent target1 = (GrantSquadLeaderTrackerComponent) target;
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
  virtual GrantSquadLeaderTrackerComponent Component.Instantiate()
  {
    return new GrantSquadLeaderTrackerComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GrantSquadLeaderTrackerComponent_AutoState : IComponentState
  {
    public SlotFlags Slots;
    public ProtoId<TrackerModePrototype> DefaultMode;
    public HashSet<ProtoId<TrackerModePrototype>> TrackerModes;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GrantSquadLeaderTrackerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GrantSquadLeaderTrackerComponent, ComponentGetState>(new ComponentEventRefHandler<GrantSquadLeaderTrackerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GrantSquadLeaderTrackerComponent, ComponentHandleState>(new ComponentEventRefHandler<GrantSquadLeaderTrackerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GrantSquadLeaderTrackerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GrantSquadLeaderTrackerComponent.GrantSquadLeaderTrackerComponent_AutoState()
      {
        Slots = component.Slots,
        DefaultMode = component.DefaultMode,
        TrackerModes = component.TrackerModes
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GrantSquadLeaderTrackerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GrantSquadLeaderTrackerComponent.GrantSquadLeaderTrackerComponent_AutoState current))
        return;
      component.Slots = current.Slots;
      component.DefaultMode = current.DefaultMode;
      component.TrackerModes = current.TrackerModes == null ? (HashSet<ProtoId<TrackerModePrototype>>) null : new HashSet<ProtoId<TrackerModePrototype>>((IEnumerable<ProtoId<TrackerModePrototype>>) current.TrackerModes);
    }
  }
}
