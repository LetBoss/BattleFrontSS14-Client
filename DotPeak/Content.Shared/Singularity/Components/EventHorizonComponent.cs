// Decompiled with JetBrains decompiler
// Type: Content.Shared.Singularity.Components.EventHorizonComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Singularity.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Singularity.Components;

[Access(new Type[] {typeof (SharedEventHorizonSystem)})]
[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentPause]
public sealed class EventHorizonComponent : 
  Component,
  ISerializationGenerated<EventHorizonComponent>,
  ISerializationGenerated
{
  [DataField("radius", false, 1, false, false, null)]
  public float Radius;
  [DataField(null, false, 1, false, false, null)]
  public bool ConsumeTiles = true;
  [DataField(null, false, 1, false, false, null)]
  public bool ConsumeEntities = true;
  [DataField("canBreachContainment", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool CanBreachContainment;
  [DataField("consumerFixtureId", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string? ConsumerFixtureId = "EventHorizonConsumer";
  [DataField("colliderFixtureId", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string? ColliderFixtureId = "EventHorizonCollider";
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public bool BeingConsumedByAnotherEventHorizon;
  [DataField("consumePeriod", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public TimeSpan TargetConsumePeriod = TimeSpan.FromSeconds(0.5);
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [DataField("nextConsumeWaveTime", false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan NextConsumeWaveTime;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EventHorizonComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EventHorizonComponent) target1;
    if (serialization.TryCustomCopy<EventHorizonComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Radius, ref target2, hookCtx, false, context))
      target2 = this.Radius;
    target.Radius = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.ConsumeTiles, ref target3, hookCtx, false, context))
      target3 = this.ConsumeTiles;
    target.ConsumeTiles = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.ConsumeEntities, ref target4, hookCtx, false, context))
      target4 = this.ConsumeEntities;
    target.ConsumeEntities = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanBreachContainment, ref target5, hookCtx, false, context))
      target5 = this.CanBreachContainment;
    target.CanBreachContainment = target5;
    string target6 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ConsumerFixtureId, ref target6, hookCtx, false, context))
      target6 = this.ConsumerFixtureId;
    target.ConsumerFixtureId = target6;
    string target7 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ColliderFixtureId, ref target7, hookCtx, false, context))
      target7 = this.ColliderFixtureId;
    target.ColliderFixtureId = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TargetConsumePeriod, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.TargetConsumePeriod, hookCtx, context);
    target.TargetConsumePeriod = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextConsumeWaveTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.NextConsumeWaveTime, hookCtx, context);
    target.NextConsumeWaveTime = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EventHorizonComponent target,
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
    EventHorizonComponent target1 = (EventHorizonComponent) target;
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
    EventHorizonComponent target1 = (EventHorizonComponent) target;
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
    EventHorizonComponent target1 = (EventHorizonComponent) target;
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
  virtual EventHorizonComponent Component.Instantiate() => new EventHorizonComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class EventHorizonComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<EventHorizonComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<EventHorizonComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      EventHorizonComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextConsumeWaveTime += args.PausedTime;
    }
  }
}
