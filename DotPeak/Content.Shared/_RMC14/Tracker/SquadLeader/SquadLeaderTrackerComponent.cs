// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Tracker.SquadLeader.SquadLeaderTrackerComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Tracker.SquadLeader;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SquadLeaderTrackerSystem)})]
public sealed class SquadLeaderTrackerComponent : 
  Component,
  ISerializationGenerated<SquadLeaderTrackerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan UpdateAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UpdateEvery = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FireteamData Fireteams = new FireteamData();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public ProtoId<TrackerModePrototype>? Mode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Target;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<ProtoId<TrackerModePrototype>> TrackerModes = new HashSet<ProtoId<TrackerModePrototype>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SquadLeaderTrackerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SquadLeaderTrackerComponent) target1;
    if (serialization.TryCustomCopy<SquadLeaderTrackerComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateAt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.UpdateAt, hookCtx, context);
    target.UpdateAt = target2;
    TimeSpan target3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateEvery, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<TimeSpan>(this.UpdateEvery, hookCtx, context);
    target.UpdateEvery = target3;
    FireteamData target4 = (FireteamData) null;
    if (this.Fireteams == (FireteamData) null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<FireteamData>(this.Fireteams, ref target4, hookCtx, true, context))
    {
      if (this.Fireteams == (FireteamData) null)
        target4 = (FireteamData) null;
      else
        serialization.CopyTo<FireteamData>(this.Fireteams, ref target4, hookCtx, context, true);
    }
    target.Fireteams = target4;
    ProtoId<TrackerModePrototype>? target5 = new ProtoId<TrackerModePrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<TrackerModePrototype>?>(this.Mode, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<ProtoId<TrackerModePrototype>?>(this.Mode, hookCtx, context);
    target.Mode = target5;
    EntityUid? target6 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Target, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityUid?>(this.Target, hookCtx, context);
    target.Target = target6;
    HashSet<ProtoId<TrackerModePrototype>> target7 = (HashSet<ProtoId<TrackerModePrototype>>) null;
    if (this.TrackerModes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<TrackerModePrototype>>>(this.TrackerModes, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<HashSet<ProtoId<TrackerModePrototype>>>(this.TrackerModes, hookCtx, context);
    target.TrackerModes = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SquadLeaderTrackerComponent target,
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
    SquadLeaderTrackerComponent target1 = (SquadLeaderTrackerComponent) target;
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
    SquadLeaderTrackerComponent target1 = (SquadLeaderTrackerComponent) target;
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
    SquadLeaderTrackerComponent target1 = (SquadLeaderTrackerComponent) target;
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
  virtual SquadLeaderTrackerComponent Component.Instantiate() => new SquadLeaderTrackerComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SquadLeaderTrackerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SquadLeaderTrackerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<SquadLeaderTrackerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      SquadLeaderTrackerComponent component,
      ref EntityUnpausedEvent args)
    {
      component.UpdateAt += args.PausedTime;
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SquadLeaderTrackerComponent_AutoState : IComponentState
  {
    public TimeSpan UpdateEvery;
    public 
    #nullable enable
    FireteamData Fireteams;
    public ProtoId<TrackerModePrototype>? Mode;
    public NetEntity? Target;
    public HashSet<ProtoId<TrackerModePrototype>> TrackerModes;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SquadLeaderTrackerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SquadLeaderTrackerComponent, ComponentGetState>(new ComponentEventRefHandler<SquadLeaderTrackerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SquadLeaderTrackerComponent, ComponentHandleState>(new ComponentEventRefHandler<SquadLeaderTrackerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SquadLeaderTrackerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SquadLeaderTrackerComponent.SquadLeaderTrackerComponent_AutoState()
      {
        UpdateEvery = component.UpdateEvery,
        Fireteams = component.Fireteams,
        Mode = component.Mode,
        Target = this.GetNetEntity(component.Target),
        TrackerModes = component.TrackerModes
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SquadLeaderTrackerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SquadLeaderTrackerComponent.SquadLeaderTrackerComponent_AutoState current))
        return;
      component.UpdateEvery = current.UpdateEvery;
      component.Fireteams = current.Fireteams;
      component.Mode = current.Mode;
      component.Target = this.EnsureEntity<SquadLeaderTrackerComponent>(current.Target, uid);
      component.TrackerModes = current.TrackerModes == null ? (HashSet<ProtoId<TrackerModePrototype>>) null : new HashSet<ProtoId<TrackerModePrototype>>((IEnumerable<ProtoId<TrackerModePrototype>>) current.TrackerModes);
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, SquadLeaderTrackerComponent>(uid, component, ref args1);
    }
  }
}
