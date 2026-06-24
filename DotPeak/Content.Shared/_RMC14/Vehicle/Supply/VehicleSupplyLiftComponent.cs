// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.Supply.VehicleSupplyLiftComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
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
namespace Content.Shared._RMC14.Vehicle.Supply;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[AutoGenerateComponentPause]
public sealed class VehicleSupplyLiftComponent : 
  Component,
  ISerializationGenerated<VehicleSupplyLiftComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Radius = 2f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public VehicleSupplyLiftMode Mode;
  [DataField(null, false, 1, false, false, null)]
  public VehicleSupplyLiftMode? NextMode;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Busy;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string LoweredState = "supply_elevator_lowered";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string LoweringState = "supply_elevator_lowering";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string RaisedState = "supply_elevator_raised";
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string RaisingState = "supply_elevator_raising";
  public EntityUid? Audio;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan? ToggledAt;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan ToggleDelay = TimeSpan.FromSeconds(17L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan RaiseSoundDelay = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan RaiseDelay = TimeSpan.FromSeconds(12.5);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan LowerDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan LowerSoundDelay = TimeSpan.FromSeconds(2L);
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? LoweringSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/asrs_lowering.ogg");
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? RaisingSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/_RMC14/Machines/asrs_raising.ogg");
  public object? LoweringAnimation;
  public object? RaisingAnimation;
  [NonSerialized]
  public string PendingVehicle = string.Empty;
  [NonSerialized]
  public EntityUid? PendingVehicleEntity;
  [NonSerialized]
  public EntityUid? ActiveVehicle;
  [NonSerialized]
  public string ActiveVehicleId = string.Empty;
  [NonSerialized]
  public readonly HashSet<string> Deployed = new HashSet<string>();
  [NonSerialized]
  public readonly Dictionary<string, int> Stored = new Dictionary<string, int>();
  [NonSerialized]
  public readonly Dictionary<string, List<EntityUid>> StoredEntities = new Dictionary<string, List<EntityUid>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleSupplyLiftComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleSupplyLiftComponent) target1;
    if (serialization.TryCustomCopy<VehicleSupplyLiftComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Radius, ref target2, hookCtx, false, context))
      target2 = this.Radius;
    target.Radius = target2;
    VehicleSupplyLiftMode target3 = VehicleSupplyLiftMode.Lowered;
    if (!serialization.TryCustomCopy<VehicleSupplyLiftMode>(this.Mode, ref target3, hookCtx, false, context))
      target3 = this.Mode;
    target.Mode = target3;
    VehicleSupplyLiftMode? target4 = new VehicleSupplyLiftMode?();
    if (!serialization.TryCustomCopy<VehicleSupplyLiftMode?>(this.NextMode, ref target4, hookCtx, false, context))
      target4 = this.NextMode;
    target.NextMode = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Busy, ref target5, hookCtx, false, context))
      target5 = this.Busy;
    target.Busy = target5;
    string target6 = (string) null;
    if (this.LoweredState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.LoweredState, ref target6, hookCtx, false, context))
      target6 = this.LoweredState;
    target.LoweredState = target6;
    string target7 = (string) null;
    if (this.LoweringState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.LoweringState, ref target7, hookCtx, false, context))
      target7 = this.LoweringState;
    target.LoweringState = target7;
    string target8 = (string) null;
    if (this.RaisedState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.RaisedState, ref target8, hookCtx, false, context))
      target8 = this.RaisedState;
    target.RaisedState = target8;
    string target9 = (string) null;
    if (this.RaisingState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.RaisingState, ref target9, hookCtx, false, context))
      target9 = this.RaisingState;
    target.RaisingState = target9;
    TimeSpan? target10 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.ToggledAt, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan?>(this.ToggledAt, hookCtx, context);
    target.ToggledAt = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ToggleDelay, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.ToggleDelay, hookCtx, context);
    target.ToggleDelay = target11;
    TimeSpan target12 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RaiseSoundDelay, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<TimeSpan>(this.RaiseSoundDelay, hookCtx, context);
    target.RaiseSoundDelay = target12;
    TimeSpan target13 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RaiseDelay, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<TimeSpan>(this.RaiseDelay, hookCtx, context);
    target.RaiseDelay = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LowerDelay, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.LowerDelay, hookCtx, context);
    target.LowerDelay = target14;
    TimeSpan target15 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LowerSoundDelay, ref target15, hookCtx, false, context))
      target15 = serialization.CreateCopy<TimeSpan>(this.LowerSoundDelay, hookCtx, context);
    target.LowerSoundDelay = target15;
    SoundSpecifier target16 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.LoweringSound, ref target16, hookCtx, true, context))
      target16 = serialization.CreateCopy<SoundSpecifier>(this.LoweringSound, hookCtx, context);
    target.LoweringSound = target16;
    SoundSpecifier target17 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.RaisingSound, ref target17, hookCtx, true, context))
      target17 = serialization.CreateCopy<SoundSpecifier>(this.RaisingSound, hookCtx, context);
    target.RaisingSound = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleSupplyLiftComponent target,
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
    VehicleSupplyLiftComponent target1 = (VehicleSupplyLiftComponent) target;
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
    VehicleSupplyLiftComponent target1 = (VehicleSupplyLiftComponent) target;
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
    VehicleSupplyLiftComponent target1 = (VehicleSupplyLiftComponent) target;
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
  virtual VehicleSupplyLiftComponent Component.Instantiate() => new VehicleSupplyLiftComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VehicleSupplyLiftComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VehicleSupplyLiftComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<VehicleSupplyLiftComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      VehicleSupplyLiftComponent component,
      ref EntityUnpausedEvent args)
    {
      if (!component.ToggledAt.HasValue)
        return;
      component.ToggledAt = new TimeSpan?(component.ToggledAt.Value + args.PausedTime);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VehicleSupplyLiftComponent_AutoState : IComponentState
  {
    public VehicleSupplyLiftMode Mode;
    public bool Busy;
    public 
    #nullable enable
    string LoweredState;
    public string LoweringState;
    public string RaisedState;
    public string RaisingState;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VehicleSupplyLiftComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VehicleSupplyLiftComponent, ComponentGetState>(new ComponentEventRefHandler<VehicleSupplyLiftComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VehicleSupplyLiftComponent, ComponentHandleState>(new ComponentEventRefHandler<VehicleSupplyLiftComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VehicleSupplyLiftComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VehicleSupplyLiftComponent.VehicleSupplyLiftComponent_AutoState()
      {
        Mode = component.Mode,
        Busy = component.Busy,
        LoweredState = component.LoweredState,
        LoweringState = component.LoweringState,
        RaisedState = component.RaisedState,
        RaisingState = component.RaisingState
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VehicleSupplyLiftComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VehicleSupplyLiftComponent.VehicleSupplyLiftComponent_AutoState current))
        return;
      component.Mode = current.Mode;
      component.Busy = current.Busy;
      component.LoweredState = current.LoweredState;
      component.LoweringState = current.LoweringState;
      component.RaisedState = current.RaisedState;
      component.RaisingState = current.RaisingState;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, VehicleSupplyLiftComponent>(uid, component, ref args1);
    }
  }
}
