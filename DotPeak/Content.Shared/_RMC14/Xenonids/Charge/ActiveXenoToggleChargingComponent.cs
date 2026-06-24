// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Charge.ActiveXenoToggleChargingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Charge;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (XenoChargeSystem)})]
public sealed class ActiveXenoToggleChargingComponent : 
  Component,
  ISerializationGenerated<ActiveXenoToggleChargingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Distance;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Steps;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int Stage;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DirectionFlag Direction;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float SoundSteps;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DirectionFlag Deviated;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float DeviatedDistance;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan LastMovedAt;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle? LastRelativeRotation;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ActiveXenoToggleChargingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ActiveXenoToggleChargingComponent) target1;
    if (serialization.TryCustomCopy<ActiveXenoToggleChargingComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Distance, ref target2, hookCtx, false, context))
      target2 = this.Distance;
    target.Distance = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Steps, ref target3, hookCtx, false, context))
      target3 = this.Steps;
    target.Steps = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.Stage, ref target4, hookCtx, false, context))
      target4 = this.Stage;
    target.Stage = target4;
    DirectionFlag target5 = (DirectionFlag) 0;
    if (!serialization.TryCustomCopy<DirectionFlag>(this.Direction, ref target5, hookCtx, false, context))
      target5 = this.Direction;
    target.Direction = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SoundSteps, ref target6, hookCtx, false, context))
      target6 = this.SoundSteps;
    target.SoundSteps = target6;
    DirectionFlag target7 = (DirectionFlag) 0;
    if (!serialization.TryCustomCopy<DirectionFlag>(this.Deviated, ref target7, hookCtx, false, context))
      target7 = this.Deviated;
    target.Deviated = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DeviatedDistance, ref target8, hookCtx, false, context))
      target8 = this.DeviatedDistance;
    target.DeviatedDistance = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastMovedAt, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.LastMovedAt, hookCtx, context);
    target.LastMovedAt = target9;
    Angle? target10 = new Angle?();
    if (!serialization.TryCustomCopy<Angle?>(this.LastRelativeRotation, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<Angle?>(this.LastRelativeRotation, hookCtx, context);
    target.LastRelativeRotation = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ActiveXenoToggleChargingComponent target,
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
    ActiveXenoToggleChargingComponent target1 = (ActiveXenoToggleChargingComponent) target;
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
    ActiveXenoToggleChargingComponent target1 = (ActiveXenoToggleChargingComponent) target;
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
    ActiveXenoToggleChargingComponent target1 = (ActiveXenoToggleChargingComponent) target;
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
  virtual ActiveXenoToggleChargingComponent Component.Instantiate()
  {
    return new ActiveXenoToggleChargingComponent();
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ActiveXenoToggleChargingComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ActiveXenoToggleChargingComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<ActiveXenoToggleChargingComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      ActiveXenoToggleChargingComponent component,
      ref EntityUnpausedEvent args)
    {
      component.LastMovedAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ActiveXenoToggleChargingComponent_AutoState : IComponentState
  {
    public float Distance;
    public float Steps;
    public int Stage;
    public DirectionFlag Direction;
    public float SoundSteps;
    public DirectionFlag Deviated;
    public float DeviatedDistance;
    public TimeSpan LastMovedAt;
    public Angle? LastRelativeRotation;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ActiveXenoToggleChargingComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<ActiveXenoToggleChargingComponent, ComponentGetState>(new ComponentEventRefHandler<ActiveXenoToggleChargingComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<ActiveXenoToggleChargingComponent, ComponentHandleState>(new ComponentEventRefHandler<ActiveXenoToggleChargingComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      ActiveXenoToggleChargingComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new ActiveXenoToggleChargingComponent.ActiveXenoToggleChargingComponent_AutoState()
      {
        Distance = component.Distance,
        Steps = component.Steps,
        Stage = component.Stage,
        Direction = component.Direction,
        SoundSteps = component.SoundSteps,
        Deviated = component.Deviated,
        DeviatedDistance = component.DeviatedDistance,
        LastMovedAt = component.LastMovedAt,
        LastRelativeRotation = component.LastRelativeRotation
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ActiveXenoToggleChargingComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is ActiveXenoToggleChargingComponent.ActiveXenoToggleChargingComponent_AutoState current))
        return;
      component.Distance = current.Distance;
      component.Steps = current.Steps;
      component.Stage = current.Stage;
      component.Direction = current.Direction;
      component.SoundSteps = current.SoundSteps;
      component.Deviated = current.Deviated;
      component.DeviatedDistance = current.DeviatedDistance;
      component.LastMovedAt = current.LastMovedAt;
      component.LastRelativeRotation = current.LastRelativeRotation;
    }
  }
}
