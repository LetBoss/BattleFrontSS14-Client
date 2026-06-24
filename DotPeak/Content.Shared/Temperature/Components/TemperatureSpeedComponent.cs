// Decompiled with JetBrains decompiler
// Type: Content.Shared.Temperature.Components.TemperatureSpeedComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Temperature.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Temperature.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedTemperatureSystem)})]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
public sealed class TemperatureSpeedComponent : 
  Component,
  ISerializationGenerated<TemperatureSpeedComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<float, float> Thresholds = new Dictionary<float, float>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float? CurrentSpeedModifier;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [AutoPausedField]
  public TimeSpan? NextSlowdownUpdate;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TemperatureSpeedComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TemperatureSpeedComponent) target1;
    if (serialization.TryCustomCopy<TemperatureSpeedComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<float, float> target2 = (Dictionary<float, float>) null;
    if (this.Thresholds == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<float, float>>(this.Thresholds, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<float, float>>(this.Thresholds, hookCtx, context);
    target.Thresholds = target2;
    float? target3 = new float?();
    if (!serialization.TryCustomCopy<float?>(this.CurrentSpeedModifier, ref target3, hookCtx, false, context))
      target3 = this.CurrentSpeedModifier;
    target.CurrentSpeedModifier = target3;
    TimeSpan? target4 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextSlowdownUpdate, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan?>(this.NextSlowdownUpdate, hookCtx, context);
    target.NextSlowdownUpdate = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TemperatureSpeedComponent target,
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
    TemperatureSpeedComponent target1 = (TemperatureSpeedComponent) target;
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
    TemperatureSpeedComponent target1 = (TemperatureSpeedComponent) target;
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
    TemperatureSpeedComponent target1 = (TemperatureSpeedComponent) target;
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
  virtual TemperatureSpeedComponent Component.Instantiate() => new TemperatureSpeedComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TemperatureSpeedComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TemperatureSpeedComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<TemperatureSpeedComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      TemperatureSpeedComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.NextSlowdownUpdate.HasValue)
        component.NextSlowdownUpdate = new TimeSpan?(component.NextSlowdownUpdate.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class TemperatureSpeedComponent_AutoState : IComponentState
  {
    public float? CurrentSpeedModifier;
    public TimeSpan? NextSlowdownUpdate;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class TemperatureSpeedComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<TemperatureSpeedComponent, ComponentGetState>(new ComponentEventRefHandler<TemperatureSpeedComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<TemperatureSpeedComponent, ComponentHandleState>(new ComponentEventRefHandler<TemperatureSpeedComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      TemperatureSpeedComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new TemperatureSpeedComponent.TemperatureSpeedComponent_AutoState()
      {
        CurrentSpeedModifier = component.CurrentSpeedModifier,
        NextSlowdownUpdate = component.NextSlowdownUpdate
      };
    }

    private void OnHandleState(
      EntityUid uid,
      TemperatureSpeedComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is TemperatureSpeedComponent.TemperatureSpeedComponent_AutoState current))
        return;
      component.CurrentSpeedModifier = current.CurrentSpeedModifier;
      component.NextSlowdownUpdate = current.NextSlowdownUpdate;
    }
  }
}
