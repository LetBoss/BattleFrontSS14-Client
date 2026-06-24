// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Atmos.SteppingOnFireComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Atmos;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedRMCFlammableSystem)})]
public sealed class SteppingOnFireComponent : 
  Component,
  ISerializationGenerated<SteppingOnFireComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public double ArmorMultiplier = 1.0;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float Distance;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UpdateTime = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan UpdateAt = TimeSpan.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityCoordinates? LastPosition;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SteppingOnFireComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SteppingOnFireComponent) target1;
    if (serialization.TryCustomCopy<SteppingOnFireComponent>(this, ref target, hookCtx, false, context))
      return;
    double target2 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.ArmorMultiplier, ref target2, hookCtx, false, context))
      target2 = this.ArmorMultiplier;
    target.ArmorMultiplier = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Distance, ref target3, hookCtx, false, context))
      target3 = this.Distance;
    target.Distance = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateTime, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.UpdateTime, hookCtx, context);
    target.UpdateTime = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateAt, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.UpdateAt, hookCtx, context);
    target.UpdateAt = target5;
    EntityCoordinates? target6 = new EntityCoordinates?();
    if (!serialization.TryCustomCopy<EntityCoordinates?>(this.LastPosition, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntityCoordinates?>(this.LastPosition, hookCtx, context);
    target.LastPosition = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SteppingOnFireComponent target,
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
    SteppingOnFireComponent target1 = (SteppingOnFireComponent) target;
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
    SteppingOnFireComponent target1 = (SteppingOnFireComponent) target;
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
    SteppingOnFireComponent target1 = (SteppingOnFireComponent) target;
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
  virtual SteppingOnFireComponent Component.Instantiate() => new SteppingOnFireComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SteppingOnFireComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SteppingOnFireComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<SteppingOnFireComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      SteppingOnFireComponent component,
      ref EntityUnpausedEvent args)
    {
      component.UpdateAt += args.PausedTime;
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SteppingOnFireComponent_AutoState : IComponentState
  {
    public double ArmorMultiplier;
    public float Distance;
    public TimeSpan UpdateTime;
    public TimeSpan UpdateAt;
    public NetCoordinates? LastPosition;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SteppingOnFireComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<SteppingOnFireComponent, ComponentGetState>(new ComponentEventRefHandler<SteppingOnFireComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<SteppingOnFireComponent, ComponentHandleState>(new ComponentEventRefHandler<SteppingOnFireComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      SteppingOnFireComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new SteppingOnFireComponent.SteppingOnFireComponent_AutoState()
      {
        ArmorMultiplier = component.ArmorMultiplier,
        Distance = component.Distance,
        UpdateTime = component.UpdateTime,
        UpdateAt = component.UpdateAt,
        LastPosition = this.GetNetCoordinates(component.LastPosition)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SteppingOnFireComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is SteppingOnFireComponent.SteppingOnFireComponent_AutoState current))
        return;
      component.ArmorMultiplier = current.ArmorMultiplier;
      component.Distance = current.Distance;
      component.UpdateTime = current.UpdateTime;
      component.UpdateAt = current.UpdateAt;
      component.LastPosition = this.EnsureCoordinates<SteppingOnFireComponent>(current.LastPosition, uid);
    }
  }
}
