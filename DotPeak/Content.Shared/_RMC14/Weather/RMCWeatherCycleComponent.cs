// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weather.RMCWeatherCycleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared._RMC14.Weather;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCWeatherSystem)})]
public sealed class RMCWeatherCycleComponent : 
  Component,
  ISerializationGenerated<RMCWeatherCycleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<RMCWeatherEvent> WeatherEvents = new List<RMCWeatherEvent>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public RMCWeatherEvent? CurrentEvent;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MinTimeBetweenEvents;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan MinTimeVariance = TimeSpan.FromMinutes(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LastEventCooldown;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCWeatherCycleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCWeatherCycleComponent) target1;
    if (serialization.TryCustomCopy<RMCWeatherCycleComponent>(this, ref target, hookCtx, false, context))
      return;
    List<RMCWeatherEvent> target2 = (List<RMCWeatherEvent>) null;
    if (this.WeatherEvents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<RMCWeatherEvent>>(this.WeatherEvents, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<RMCWeatherEvent>>(this.WeatherEvents, hookCtx, context);
    target.WeatherEvents = target2;
    RMCWeatherEvent target3 = (RMCWeatherEvent) null;
    if (!serialization.TryCustomCopy<RMCWeatherEvent>(this.CurrentEvent, ref target3, hookCtx, false, context))
    {
      if (this.CurrentEvent == null)
        target3 = (RMCWeatherEvent) null;
      else
        serialization.CopyTo<RMCWeatherEvent>(this.CurrentEvent, ref target3, hookCtx, context);
    }
    target.CurrentEvent = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MinTimeBetweenEvents, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.MinTimeBetweenEvents, hookCtx, context);
    target.MinTimeBetweenEvents = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.MinTimeVariance, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.MinTimeVariance, hookCtx, context);
    target.MinTimeVariance = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastEventCooldown, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.LastEventCooldown, hookCtx, context);
    target.LastEventCooldown = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCWeatherCycleComponent target,
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
    RMCWeatherCycleComponent target1 = (RMCWeatherCycleComponent) target;
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
    RMCWeatherCycleComponent target1 = (RMCWeatherCycleComponent) target;
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
    RMCWeatherCycleComponent target1 = (RMCWeatherCycleComponent) target;
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
  virtual RMCWeatherCycleComponent Component.Instantiate() => new RMCWeatherCycleComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCWeatherCycleComponent_AutoState : IComponentState
  {
    public RMCWeatherEvent? CurrentEvent;
    public TimeSpan MinTimeBetweenEvents;
    public TimeSpan MinTimeVariance;
    public TimeSpan LastEventCooldown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCWeatherCycleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCWeatherCycleComponent, ComponentGetState>(new ComponentEventRefHandler<RMCWeatherCycleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCWeatherCycleComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCWeatherCycleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCWeatherCycleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCWeatherCycleComponent.RMCWeatherCycleComponent_AutoState()
      {
        CurrentEvent = component.CurrentEvent,
        MinTimeBetweenEvents = component.MinTimeBetweenEvents,
        MinTimeVariance = component.MinTimeVariance,
        LastEventCooldown = component.LastEventCooldown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCWeatherCycleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCWeatherCycleComponent.RMCWeatherCycleComponent_AutoState current))
        return;
      component.CurrentEvent = current.CurrentEvent;
      component.MinTimeBetweenEvents = current.MinTimeBetweenEvents;
      component.MinTimeVariance = current.MinTimeVariance;
      component.LastEventCooldown = current.LastEventCooldown;
    }
  }
}
