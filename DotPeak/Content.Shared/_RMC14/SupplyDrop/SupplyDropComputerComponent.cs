// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.SupplyDrop.SupplyDropComputerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Squads;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.SupplyDrop;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedSupplyDropSystem)})]
public sealed class SupplyDropComputerComponent : 
  Component,
  ISerializationGenerated<SupplyDropComputerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2i Coordinates;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxCoordinate = 1000;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId<SquadTeamComponent>? Squad;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan Cooldown = TimeSpan.FromSeconds(500L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastLaunchAt;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextLaunchAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HasCrate;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UpdateEvery = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan NextUpdate;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SupplyDropComputerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SupplyDropComputerComponent) target1;
    if (serialization.TryCustomCopy<SupplyDropComputerComponent>(this, ref target, hookCtx, false, context))
      return;
    Vector2i target2 = new Vector2i();
    if (!serialization.TryCustomCopy<Vector2i>(this.Coordinates, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2i>(this.Coordinates, hookCtx, context);
    target.Coordinates = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxCoordinate, ref target3, hookCtx, false, context))
      target3 = this.MaxCoordinate;
    target.MaxCoordinate = target3;
    EntProtoId<SquadTeamComponent>? target4 = new EntProtoId<SquadTeamComponent>?();
    if (!serialization.TryCustomCopy<EntProtoId<SquadTeamComponent>?>(this.Squad, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntProtoId<SquadTeamComponent>?>(this.Squad, hookCtx, context);
    target.Squad = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Cooldown, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.Cooldown, hookCtx, context);
    target.Cooldown = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastLaunchAt, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.LastLaunchAt, hookCtx, context);
    target.LastLaunchAt = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextLaunchAt, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.NextLaunchAt, hookCtx, context);
    target.NextLaunchAt = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.HasCrate, ref target8, hookCtx, false, context))
      target8 = this.HasCrate;
    target.HasCrate = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateEvery, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.UpdateEvery, hookCtx, context);
    target.UpdateEvery = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextUpdate, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.NextUpdate, hookCtx, context);
    target.NextUpdate = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SupplyDropComputerComponent target,
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
    SupplyDropComputerComponent target1 = (SupplyDropComputerComponent) target;
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
    SupplyDropComputerComponent target1 = (SupplyDropComputerComponent) target;
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
    SupplyDropComputerComponent target1 = (SupplyDropComputerComponent) target;
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
  virtual SupplyDropComputerComponent Component.Instantiate() => new SupplyDropComputerComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SupplyDropComputerComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SupplyDropComputerComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<SupplyDropComputerComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      SupplyDropComputerComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastLaunchAt += args.PausedTime;
      component.NextLaunchAt += args.PausedTime;
      component.NextUpdate += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SupplyDropComputerComponent_AutoState : IComponentState
  {
    public Vector2i Coordinates;
    public int MaxCoordinate;
    public EntProtoId<
    #nullable enable
    SquadTeamComponent>? Squad;
    public TimeSpan Cooldown;
    public TimeSpan LastLaunchAt;
    public TimeSpan NextLaunchAt;
    public bool HasCrate;
    public TimeSpan UpdateEvery;
    public TimeSpan NextUpdate;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SupplyDropComputerComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SupplyDropComputerComponent, ComponentGetState>(new ComponentEventRefHandler<SupplyDropComputerComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SupplyDropComputerComponent, ComponentHandleState>(new ComponentEventRefHandler<SupplyDropComputerComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      SupplyDropComputerComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SupplyDropComputerComponent.SupplyDropComputerComponent_AutoState()
      {
        Coordinates = component.Coordinates,
        MaxCoordinate = component.MaxCoordinate,
        Squad = component.Squad,
        Cooldown = component.Cooldown,
        LastLaunchAt = component.LastLaunchAt,
        NextLaunchAt = component.NextLaunchAt,
        HasCrate = component.HasCrate,
        UpdateEvery = component.UpdateEvery,
        NextUpdate = component.NextUpdate
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SupplyDropComputerComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SupplyDropComputerComponent.SupplyDropComputerComponent_AutoState current))
        return;
      component.Coordinates = current.Coordinates;
      component.MaxCoordinate = current.MaxCoordinate;
      component.Squad = current.Squad;
      component.Cooldown = current.Cooldown;
      component.LastLaunchAt = current.LastLaunchAt;
      component.NextLaunchAt = current.NextLaunchAt;
      component.HasCrate = current.HasCrate;
      component.UpdateEvery = current.UpdateEvery;
      component.NextUpdate = current.NextUpdate;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, SupplyDropComputerComponent>(uid, component, ref args1);
    }
  }
}
